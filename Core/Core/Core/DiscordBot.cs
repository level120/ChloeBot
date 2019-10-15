using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using ChloeBot.Crawling;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace ChloeBot.Core
{
    public class DiscordBot
    {
        private const string _tokenName = "Bot.token";
        private const string _tokenDownloadUrl = @"ftp://192.168.0.5/Bot.token";
        private string TOKEN { get; } = GetToken();
        //private readonly UInt64 APP_ID = 539448802638036992;

        private static DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        private static Task _monitor;

        private IReadOnlyCollection<SocketGuild> Guilds => Client.Guilds.ToList();

        public static DiscordSocketClient Client => _client ?? throw new ArgumentNullException();

        private static string GetToken()
        {
            // debug mode only
            if (File.Exists(_tokenName))
                return File.ReadAllText(_tokenName);

            return new WebClient().DownloadString(_tokenDownloadUrl);
        }

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();
            //You need to add the Token for your Discord Bot to the code below.

            //event subscriptions
            _client.Log += Log;

            await RegisterCommandsAsync();
            await _client.LoginAsync(TokenType.Bot, TOKEN);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private Task Monitoring()
        {
            if (_monitor == null)
            {
                _monitor = Task.Run(async () =>
                {
                    while (true)
                    {
                        var result = SoulworkerMonitor.Run();

                        if (result.Any())
                            await SendMessage(result);

                        await Task.Delay(60_000);
                    }
                });
            }
            return Task.CompletedTask;
        }

        private async Task SendMessage(IList<EmbedBuilder> result)
        {
            Console.Error.WriteLine($"{DateTime.Now:HH:mm:ss}\tRegistered channel count: {Guilds.Count}");
            foreach (var client in Guilds)
            {
                var channel = client.DefaultChannel;
                Console.Error.WriteLine($"{DateTime.Now:HH:mm:ss}\t[{client.Name}-{channel.Name}] The bot will send target count: {result.Count}");

                foreach (var builder in result)
                {
                    await channel.SendMessageAsync(embed: builder.Build());

                    if (!builder.Title.Contains("이벤트"))
                        await channel.SendMessageAsync(builder.Url);

                    Console.Error.WriteLine($"{DateTime.Now:HH:mm:ss}\t[{client.Name}-{channel.Name}] Send message successfully");
                }

                await Task.Delay(300);
            }
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]\t{arg}");
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.Ready += SetGamePlayAsync;
            _client.Ready += Monitoring;
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message) || message.Author.IsBot)
                return;

            int argPos = 0;

            if (message.HasStringPrefix("!", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);
                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                    Console.WriteLine(result.ErrorReason);
            }
        }

        public async Task SetGamePlayAsync()
        {
            await _client.SetGameAsync("소울워커 감시");
        }
    }
}
