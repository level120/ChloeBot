using System;
using System.IO;
using System.Linq;
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
        private string TOKEN { get; } = File.ReadAllText("Bot.token");
        //private readonly UInt64 APP_ID = 539448802638036992;

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

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
            _client.Ready += Monitoring;
            _client.UserJoined += AnnounceUserJoined;

            await RegisterCommandsAsync();
            await _client.LoginAsync(TokenType.Bot, TOKEN);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private async Task Monitoring()
        {
            while (true)
            {
                var result = SoulworkerMonitor.Run();

                if (result.Any())
                {
                    foreach (var clientGuild in _client.Guilds)
                    {
                        var channel = _client.GetChannel(clientGuild.Id);

                        if (channel is IMessageChannel c)
                        {
                            foreach (var builder in result)
                                await c.SendMessageAsync(embed: builder.Build());
                        }
                    }
                }

                await Task.Delay(60_000);
            }
        }

        private async Task AnnounceUserJoined(SocketGuildUser user)
        {
            var guild = user.Guild;
            var channel = guild.DefaultChannel;
            await channel.SendMessageAsync($"Welcome, {user.Mention}");
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.Ready += SetGamePlayAsync;
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            if (message is null || message.Author.IsBot) return;

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
            await _client.SetGameAsync("소울워커 감시 중..");
        }
    }
}
