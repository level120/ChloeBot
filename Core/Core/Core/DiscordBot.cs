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
        private const string TokenName = "Bot.token";
        private const string TokenDownloadUrl = @"ftp://192.168.0.5/Bot.token";
        private string? Token { get; } = GetToken();
        private IServiceProvider? _services;

        public static DiscordSocketClient Client { get; } = new DiscordSocketClient();
        private static Task _monitor;

        private IReadOnlyCollection<SocketGuild> Guilds => Client.Guilds.ToArray();
        private CommandService Commands { get; } = new CommandService();

        private static string? GetToken()
        {
            // debug mode only
            return File.Exists(TokenName)
                ? File.ReadAllText(TokenName)
                : new WebClient().DownloadString(TokenDownloadUrl);
        }

        public async Task RunBotAsync()
        {
            _services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(Commands)
                .BuildServiceProvider();
            //You need to add the Token for your Discord Bot to the code below.

            //event subscriptions
            Client.Log += Log;

            if (string.IsNullOrEmpty(Token))
                throw new ArgumentException($"Token 값이 올바르지 않습니다(Token: {Token})", nameof(Token));

            await RegisterCommandsAsync();
            await Client.LoginAsync(TokenType.Bot, Token);
            await Client.StartAsync();
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

        private async Task SendMessage(ICollection<EmbedBuilder> result)
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
            }
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]\t{arg}");
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            Client.Ready += SetGamePlayAsync;
            Client.Ready += Monitoring;
            Client.MessageReceived += HandleCommandAsync;

            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message) || message.Author.IsBot)
                return;

            int argPos = 0;

            if (message.HasStringPrefix("!", ref argPos) || message.HasMentionPrefix(Client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(Client, message);
                var result = await Commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                    Console.WriteLine(result.ErrorReason);
            }
        }

        public async Task SetGamePlayAsync()
        {
            await Client.SetGameAsync("Node.js로 마이그레이션");
        }
    }
}
