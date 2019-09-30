using System.Threading.Tasks;
using ChloeBot.Maplestory;
using ChloeBot.Soulworker;
using Discord;
using Discord.Commands;

// https://discord.foxbot.me/docs/guides/commands/commands.html
// https://discord4j.readthedocs.io/en/latest/Making-embedded-content-using-EmbedBuilder/
namespace ChloeBot.Core
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        private string GetAssemVersion => GetType().Assembly.GetName().Version.ToString();

        [Command("ver")]
        public async Task VersionAsync()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Chloe Bot Version Info")
                .WithDescription(GetAssemVersion)
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

        [Command("maplestory"), Summary(@"캐릭터명, 랭킹모드, 서버")]
        [Alias("m", "maple")]
        public async Task MapleStatus([Summary("maple")] string charName = "", eModeList mode = eModeList.Total, eServerList server = eServerList.normalAll)
        {
            var builder = new EmbedBuilder();
            var maple = new MaplestoryKR();

            // 크롤링 시작
            await Crawling.Crawling.StartMapleCrawlerasync(maple.GetCrawlingUrls(server, mode, charName));

            // 결과대기
            while (Crawling.Crawling.ResMaple.Count == 0)
            {
                await Task.Delay(100);
            }

            builder.WithTitle($"{Crawling.Crawling.ResMaple[0].name}님의 랭킹정보")
                .WithImageUrl(Crawling.Crawling.ResMaple[0].imgUrl)
                .AddField("순위", $"{Crawling.Crawling.ResMaple[0].rank} 위\n({Crawling.Crawling.ResMaple[0].changeRank})", true)
                .AddField("캐릭터 정보", $"{Crawling.Crawling.ResMaple[0].name}\n{Crawling.Crawling.ResMaple[0].job}", true)
                .AddField("레벨", Crawling.Crawling.ResMaple[0].lv, true)
                .AddField("경험치", Crawling.Crawling.ResMaple[0].exp, true)
                .AddField("인기도", Crawling.Crawling.ResMaple[0].pop, true)
                .AddField("길드", Crawling.Crawling.ResMaple[0].guild, true)
                .WithColor(Color.Gold);

            // 다음동작을 위해 결과삭제
            Crawling.Crawling.ResMaple.RemoveRange(0, Crawling.Crawling.ResMaple.Count);

            await ReplyAsync(embed: builder.Build());
        }
    }
}

