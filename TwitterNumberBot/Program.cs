using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TwitterNumberBot.Discord;
using TwitterNumberBot.Twitter;

namespace TwitterNumberBot
{
    internal class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        private static TwitterWrapper _twitterWrapper;
        private static DiscordWrapper _discordWrapper;

        private static DateTime _twitterLastUpdate;
        private static DateTime _discordLastUpdate;
        private static DateTime _sampleStart;

        private static AutoResetEvent _closing;

        private static bool _reset;
        private static bool _isRunning;
        private static bool _isSampling;

        private static async Task Main(string[] args)
        {
            _closing = new AutoResetEvent(false);
            Configuration = BuildConfig();

            _reset = true;

            while (_reset)
            {
                await MainLoop();
            }

            Console.CancelKeyPress += OnExit;
            _closing.WaitOne();
        }

        private static async Task MainLoop()
        {
            // Stop sampling between midnight and 10 AM to avoid international spam
            if (DateTime.Now.Hour < 10)
            {
                var sleepSpan = DateTime.Parse($"{DateTime.Today.ToShortDateString()} 10:00:00") - DateTime.Now;
                Console.WriteLine($"Sleeping for {sleepSpan:g}...");
                Thread.Sleep(sleepSpan);
            }

            _reset = false;
            _isRunning = true;

            // Set up client wrappers
            _twitterWrapper = new TwitterWrapper(Configuration["TwitterApiKey"], Configuration["TwitterSecretKey"],
                Configuration["TwitterBearerToken"], Configuration["BadTweetLoggingPath"],
                bool.Parse(Configuration["BadTweetLogging"]));

            _discordWrapper = new DiscordWrapper(Configuration["DiscordBotToken"], Configuration["DiscordGuildId"],
                Configuration["DiscordRoomId"]);

            // Configure twitter client
            Console.WriteLine("Configuring tweet sampling rules...");

            await _twitterWrapper.ConfigureRules();
            _twitterWrapper.ConfigureStream();

            _sampleStart = DateTime.Now;
            _discordLastUpdate = DateTime.Now;
            _twitterLastUpdate = DateTime.Now;

            // Set up worker threads
            //Twitter Thread
            await Task.Factory.StartNew(async () =>
            {
                Console.WriteLine("Starting tweet sampling...");
                _isSampling = true;
                await _twitterWrapper.SampleStream();
            });

            //Discord Thread
            await Task.Factory.StartNew(async () =>
            {
                await _discordWrapper.Login();

                while (!_reset && _isRunning && _isSampling)
                {
                    Thread.Sleep(500);

                    if ((DateTime.Now - _discordLastUpdate).TotalSeconds <= 5)
                        continue;

                    _discordLastUpdate = DateTime.Now;

                    await CheckQueueAndWriteToDiscord();

                    if (_twitterWrapper.BadTweetLogging)
                        WriteBadTweetLog();

                    if (_twitterWrapper.TweetsReceived <= _twitterWrapper.LastSampledTweetCount) continue;

                    _twitterLastUpdate = DateTime.Now;
                    _twitterWrapper.LastSampledTweetCount = _twitterWrapper.TweetsReceived;
                }
            });

            //Heartbeat Check Thread
            await Task.Factory.StartNew(async () =>
            {
                Thread.Sleep(10000);

                while (!_reset && _isRunning && _isSampling)
                {
                    Thread.Sleep(500);

                    if (DateTime.Now.Hour >= 10 &&
                        (DateTime.Now - _discordLastUpdate).TotalMinutes <= 1 &&
                        (DateTime.Now - _twitterLastUpdate).TotalMinutes <= 2)
                        continue;

                    Console.WriteLine("***** Heartbeat error detected or hit Stop Time, restarting *****");

                    _reset = true;
                    _isRunning = false;
                    _isSampling = false;

                    _twitterWrapper.StopStream();
                    await _discordWrapper.Logout();
                }
            });
        }

        private static void WriteBadTweetLog()
        {
            var fileInfo = new FileInfo(_twitterWrapper.TweetLogPath);

            if (fileInfo.Directory != null && !fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            using var writer = new StreamWriter(fileInfo.FullName, true);
            using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.CurrentCulture) {Delimiter = ",", HasHeaderRecord = false, Escape = '\\'});
            var tweetList = new List<LogTweet>();

            while (_twitterWrapper.BadTweetQueue.Count > 0)
            {
                tweetList.Add(_twitterWrapper.BadTweetQueue.Dequeue());
            }

            csv.WriteRecords(tweetList);
        }

        private static async void OnExit(object sender, ConsoleCancelEventArgs e)
        {
            _isRunning = false;
            _isSampling = false;
            _twitterWrapper.StopStream();
            await _discordWrapper.Logout();
            _closing.Set();
        }

        private static async Task CheckQueueAndWriteToDiscord()
        {
            var tweetsPerMinute = (double)_twitterWrapper.TweetsReceived / (DateTime.Now - _sampleStart).TotalMinutes;
            var tweetsPerMonth = tweetsPerMinute * 60.0 * 14.0 * 30.0;

            Console.WriteLine($"[{DateTime.Now:s}]");
            Console.WriteLine($"\tTweets received: {_twitterWrapper.TweetsReceived:###,###,##0}");
            Console.WriteLine($"\tWith Numbers: {_twitterWrapper.TweetsWithValidNumbers:###,###,##0} ({(double)_twitterWrapper.TweetsWithValidNumbers / (double)(Math.Max(_twitterWrapper.TweetsReceived, 1)):P})");
            Console.WriteLine($"\t{tweetsPerMinute:N} tweets/min ({tweetsPerMonth:N} per month)");

            if (_twitterWrapper.TweetQueue.Count == 0)
                return;

            Console.WriteLine($"---Found {_twitterWrapper.TweetQueue.Count} tweets for Discord. Sending {Math.Min(5, _twitterWrapper.TweetQueue.Count)}...");

            for (var i = 0; i < 5 && _twitterWrapper.TweetQueue.Count > 0; i++)
            {
                _twitterWrapper.TweetQueue.TryDequeue(out var tweet);
                await _discordWrapper.WritePost(tweet);
            }
        }

        private static IConfigurationRoot BuildConfig()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.AddUserSecrets<Program>();
            return builder.Build();
        }
    }
}
