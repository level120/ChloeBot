using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace ChloeBot.Core
{
    /// <summary>
    /// 일반 사용자들이 사용하지 않아야 하는 명령어들의 모음
    /// 이곳에서는 alias 사용을 비권장합니다
    /// </summary>
    [Group("system")]
    public class SystemCommands : ModuleBase<SocketCommandContext>
    {
        [Group("call")]
        public class CallCommands : ModuleBase<SocketCommandContext>
        {
            private Version AssemVersion => GetType().Assembly.GetName().Version;

            [Command("ping")]
            public async Task GetPingAsync()
            {
                await ReplyAsync("pong");
            }

            [Command("version")]
            public async Task VersionAsync()
            {
                var builder = new EmbedBuilder();

                builder.WithTitle("Chloe Bot Version Info")
                    .WithDescription($"{AssemVersion.Major}.{AssemVersion.Minor}.{AssemVersion.Build}")
                    .WithColor(Color.Orange);

                await ReplyAsync(embed: builder.Build());
            }

            [Command("latency")]
            public async Task GetLatencyAsync()
            {
                var builder = new EmbedBuilder();
                var msg = new StringBuilder();

                builder.WithTitle("Chloe bot latency")
                    .WithDescription(DiscordBot.Client.Latency.ToString())
                    .WithColor(Color.Purple);

                await ReplyAsync(embed: builder.Build());
            }

            [Command("status")]
            public async Task GetStatusAsync()
            {
                var builder = new EmbedBuilder();
                var msg = new StringBuilder();

                builder.WithTitle("Chloe bot status")
                    .WithDescription(DiscordBot.Client.Status.ToString())
                    .WithColor(Color.Purple);

                await ReplyAsync(embed: builder.Build());
            }

            [Group("generate")]
            public class GenerateCommands : ModuleBase<SocketCommandContext>
            {
                [Group("list")]
                public class ListCommands : ModuleBase<SocketCommandContext>
                {
                    [Command("guild")]
                    public async Task CreateGuildListAsync()
                    {
                        if (Context.User.Id != 275540899973300225)
                            return;

                        var builder = new EmbedBuilder();
                        var msg = new StringBuilder();

                        msg.AppendLine("Registered channel count: " + DiscordBot.Client.Guilds.Count);
                        msg.AppendLine("===========================");

                        foreach (var clientGuild in DiscordBot.Client.Guilds)
                            msg.AppendLine($"{clientGuild.Name} - {clientGuild.DefaultChannel.Name}");

                        builder.WithTitle("A registration guild list on the Chloe bot.")
                            .WithDescription(msg.ToString())
                            .WithColor(Color.Purple);

                        await ReplyAsync(embed: builder.Build());
                    }
                }
            }
        }
    }
}