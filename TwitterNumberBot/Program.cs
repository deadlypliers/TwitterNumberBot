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

        private static bool _running;
        private static bool _reset;
        private static bool _isSampling;
        private static bool _badTweetLogging;

        private static async Task Main(string[] args)
        {
            _closing = new AutoResetEvent(false);
            Configuration = BuildConfig();

            _running = true;
            _reset = true;
            _badTweetLogging = bool.Parse(Configuration["BadTweetLogging"]);
            
            while (_reset)
            {
                _reset = false;
                _running = true;

                await MainLoop();
            }

            Console.CancelKeyPress += OnExit;
            _closing.WaitOne();
        }

        private static async Task MainLoop()
        {
            // Stop sampling between midnight and 10 AM to avoid international spam
            if (DateTime.Now.Hour > 0 && DateTime.Now.Hour < 10)
            {
                var sleepSpan = DateTime.Parse($"{DateTime.Today.ToShortDateString()}T10:00:00") - DateTime.Now;
                Console.WriteLine($"Sleeping for {sleepSpan.TotalMinutes:F} minutes");
                Thread.Sleep(sleepSpan);
            }

            // Set up client wrappers
            _twitterWrapper = new TwitterWrapper(Configuration["TwitterApiKey"], Configuration["TwitterSecretKey"],
                    Configuration["TwitterBearerToken"], _badTweetLogging);

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
            var twitterTask = await Task.Factory.StartNew(async () =>
            {
                while (_running)
                {
                    Console.WriteLine("Starting tweet sampling...");
                    _isSampling = true;
                    await _twitterWrapper.SampleStream();
                }
            });

            var discordTask = await Task.Factory.StartNew(async () =>
            {
                await _discordWrapper.Login();

                while (_running)
                {
                    var checkStart = DateTime.Now;

                    if (_isSampling)
                    {
                        await CheckQueueAndWriteToDiscord();

                        if (_badTweetLogging)
                            WriteBadTweetLog();

                        _discordLastUpdate = DateTime.Now;

                        if (_twitterWrapper.TweetsReceived > _twitterWrapper.LastSampledTweetCount)
                        {
                            _twitterLastUpdate = DateTime.Now;
                            _twitterWrapper.LastSampledTweetCount = _twitterWrapper.TweetsReceived;
                        }
                    }

                    Thread.Sleep(5000 - Math.Min((int)(DateTime.Now - checkStart).TotalMilliseconds, 2500));
                }
            });

            await Task.Factory.StartNew(async () =>
            {
                while (_running)
                {
                    Thread.Sleep(2500);

                    if (DateTime.Now.Hour >= 10 &&
                        (new TimeSpan(DateTime.Now.Ticks - _discordLastUpdate.Ticks) <= TimeSpan.FromMinutes(1) 
                            || new TimeSpan(DateTime.Now.Ticks - _twitterLastUpdate.Ticks) <= TimeSpan.FromMinutes(1)))
                        continue;

                    Console.WriteLine("***** Heartbeat error detected or hit Stop Time, restarting *****");

                    _running = false;
                    _reset = true;

                    await ResetMain(twitterTask, discordTask);
                }
            });
        }

        private static async Task ResetMain(IDisposable twitterTask, IDisposable discordTask)
        {
            twitterTask.Dispose();
            discordTask.Dispose();
            _twitterWrapper.StopStream();
            await _discordWrapper.Logout();
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

            return;
        }

        private static async void OnExit(object sender, ConsoleCancelEventArgs e)
        {
            _running = false;
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
