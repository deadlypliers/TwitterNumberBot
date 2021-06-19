﻿using CsvHelper;
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

        private static AutoResetEvent _closing;

        private static bool _running;
        private static bool _reset;

        private static async Task Main(string[] args)
        {
            _closing = new AutoResetEvent(false);
            Configuration = BuildConfig();

            _running = true;
            _reset = true;
            
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
            if (DateTime.Now.Hour > 0 && DateTime.Now.Hour < 10)
                Thread.Sleep(DateTime.Parse($"{DateTime.Today.ToShortDateString()}T10:00:00") - DateTime.Now);

            _twitterWrapper = new TwitterWrapper(Configuration["TwitterApiKey"], Configuration["TwitterSecretKey"],
                    Configuration["TwitterBearerToken"]);

            _discordWrapper = new DiscordWrapper(Configuration["DiscordBotToken"], Configuration["DiscordGuildId"],
                Configuration["DiscordRoomId"]);

            _twitterWrapper.LastSampledTweetCount = 0;

            var twitterTask = await Task.Factory.StartNew(async () =>
            {
                while (_running)
                {
                    await _twitterWrapper.ConfigureRules();
                    Thread.Sleep(2000);
                    _twitterWrapper.ConfigureStream();
                    Console.WriteLine("Starting tweet sampling...");
                    await CheckTwitter();
                }
            });

            var discordTask = await Task.Factory.StartNew(async () =>
            {
                await _discordWrapper.Login();

                while (_running)
                {
                    var checkStart = DateTime.Now;

                    await CheckQueueAndWriteToDiscord();
                    WriteBadTweetLog();

                    _discordLastUpdate = DateTime.Now;

                    if (_twitterWrapper.TweetsReceived > _twitterWrapper.LastSampledTweetCount)
                    {
                        _twitterLastUpdate = DateTime.Now;
                        _twitterWrapper.LastSampledTweetCount = _twitterWrapper.TweetsReceived;
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
                        (DateTime.Now - _discordLastUpdate <= TimeSpan.FromMinutes(1) ||
                         DateTime.Now - _twitterLastUpdate <= TimeSpan.FromMinutes(3))) continue;

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

            if (!fileInfo.Directory.Exists)
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
            await StopDiscord();
            _closing.Set();
        }

        private static async Task CheckTwitter()
        {
            await _twitterWrapper.SampleStream();
        }

        private static async Task StopDiscord()
        {
            await _discordWrapper.Logout();
        }

        private static async Task CheckQueueAndWriteToDiscord()
        {
            Console.WriteLine(
                $"{DateTime.Now:s} - Tweets received: {_twitterWrapper.TweetsReceived} / With Numbers: {_twitterWrapper.TweetsWithValidNumbers} ({_twitterWrapper.TweetsWithValidNumbers / (_twitterWrapper.TweetsReceived == 0 ? 1 : _twitterWrapper.TweetsReceived) * 100:P})");

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
