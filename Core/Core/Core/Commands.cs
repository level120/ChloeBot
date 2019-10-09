using System.Threading.Tasks;
using ChloeBot.Soulworker;
using Discord;
using Discord.Commands;

// https://discord.foxbot.me/docs/guides/commands/commands.html
// https://discord4j.readthedocs.io/en/latest/Making-embedded-content-using-EmbedBuilder/
namespace ChloeBot.Core
{
    /// <summary>
    /// ChloeBot의 모든 명령어
    /// </summary>
    public class Commands : ModuleBase<SocketCommandContext>
    {
        private string AssemVersion => GetType().Assembly.GetName().Version.ToString();

        [Command("ver")]
        public async Task VersionAsync()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Chloe Bot Version Info")
                .WithDescription(AssemVersion)
                .WithColor(Color.Orange);

            await ReplyAsync(embed: builder.Build());
        }

        [Command("notice"), Alias("n")]
        public async Task LastNoticeAsync()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("마지막으로 등록된 공지사항 글이에요!")
                .WithUrl(Board.Notice)
                .WithColor(Color.Orange);

            await ReplyAsync(embed: builder.Build());
            await ReplyAsync(builder.Url);
        }

        [Command("update"), Alias("u")]
        public async Task LastUpdateAsync()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("마지막으로 등록된 업데이트 글이에요!")
                .WithUrl(Board.Detail)
                .WithColor(Color.Orange);

            await ReplyAsync(embed: builder.Build());
            await ReplyAsync(builder.Url);
        }

        [Command("event"), Alias("e")]
        public async Task LastEventAsync()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("마지막으로 등록된 이벤트 글이에요!")
                .WithImageUrl(Board.Event)
                .WithColor(Color.Orange);

            await ReplyAsync(embed: builder.Build());
        }

        [Command("gmmagazine"), Alias("gm")]
        public async Task LastGmMagazineAsync()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("마지막으로 등록된 GM매거진 글이에요!")
                .WithUrl(Board.GMMagazine)
                .WithColor(Color.Orange);

            await ReplyAsync(embed: builder.Build());
            await ReplyAsync(builder.Url);
        }

        [Command("help"), Summary(@"help")]
        [Alias("h")]
        public async Task SoulworkerWork()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("소울워커의 새로운 게시글을 감시합니다!")
                .WithDescription("그 이외 다른 기능은 없습니다!")
                .WithImageUrl(@"https://i.imgur.com/hsV3Tk1.png")
                .WithColor(Color.Green);
            await ReplyAsync(embed: builder.Build());
        }
    }
}

