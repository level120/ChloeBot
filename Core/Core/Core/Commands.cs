using Discord;
using Discord.Commands;
using System.Threading.Tasks;

// https://discord.foxbot.me/docs/guides/commands/commands.html
// https://discord4j.readthedocs.io/en/latest/Making-embedded-content-using-EmbedBuilder/
namespace Core
{
    using Core.Maplestory;

    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task PingAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Ping!")
                .WithDescription("Pong!")
                .WithColor(Color.Orange);

            await ReplyAsync("", false, builder.Build());
        }

        [Command("soulworker"), Summary(@"소울워커")]
        [Alias("sw")]
        public async Task SoulworkerWork()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("소울워커의 새로운 게시글을 감시합니다!")
                .WithDescription("시작은 가능하지만 끌 수는 없답니다 :D")
                .WithImageUrl("")
                .WithColor(Color.Green);
            await ReplyAsync("", false, builder.Build());

            Task.Factory.StartNew(RunSoulworker);
        }

        private async void RunSoulworker()
        {
            EmbedBuilder builder = new EmbedBuilder();

            while (true)
            {
                // 크롤링 시작
                await LookingSoulworker.StartSoulWorkerMonitoringAsync();

                // 결과대기
                if (LookingSoulworker.news.Count != 0)
                {
                    foreach (var n in LookingSoulworker.news)
                    {
                        var titleString = "";

                        if (n.Contains("Notice")) titleString = "**[공지사항]**";
                        else if (n.Contains("Update")) titleString = "**[업데이트]**";
                        else if (n.Contains("GMMagazine")) titleString = "**[GM매거진]**";
                        else
                        {
                            titleString = "**[이벤트]**";
                            builder.WithTitle($"{titleString} 새로운 게시글이 올라왔어요!")
                                .WithDescription("")
                                .WithImageUrl(n)
                                .WithColor(Color.Orange);

                            await ReplyAsync("", false, builder.Build());
                            continue;
                        }

                        builder.WithTitle($"{titleString} 새로운 게시글이 올라왔어요!")
                            .WithDescription(n)
                            .WithImageUrl("")
                            .WithColor(Color.Orange);

                        await ReplyAsync("", false, builder.Build());
                    }

                    // 다음동작을 위해 결과삭제
                    LookingSoulworker.ResetResult();
                }
                await Task.Delay(60000);
            }
        }

        [Command("maplestory"), Summary(@"캐릭터명, 랭킹모드, 서버")]
        [Alias("m", "maple")]
        public async Task MapleStatus([Summary("maple")] string charName = "", eModeList mode = eModeList.Total, eServerList server = eServerList.normalAll)
        {
            EmbedBuilder builder = new EmbedBuilder();
            Maplestory.Maplestory maple = new Maplestory.Maplestory();

            // 크롤링 시작
            await Crawling.StartMapleCrawlerasync(maple.GetCrawlingUrls(server, mode, charName));

            // 결과대기
            while (Crawling.res_maple.Count == 0)
            {
                await Task.Delay(100);
            }

            builder.WithTitle($"{Crawling.res_maple[0].name}님의 랭킹정보")
                .WithImageUrl(Crawling.res_maple[0].imgUrl)
                .AddField("순위", $"{Crawling.res_maple[0].rank} 위\n({Crawling.res_maple[0].changeRank})", true)
                .AddField("캐릭터 정보", $"{Crawling.res_maple[0].name}\n{Crawling.res_maple[0].job}", true)
                .AddField("레벨", Crawling.res_maple[0].lv, true)
                .AddField("경험치", Crawling.res_maple[0].exp, true)
                .AddField("인기도", Crawling.res_maple[0].pop, true)
                .AddField("길드", Crawling.res_maple[0].guild, true)
                .WithColor(Color.Gold);

            // 다음동작을 위해 결과삭제
            Crawling.res_maple.RemoveRange(0, Crawling.res_maple.Count);

            await ReplyAsync("", false, builder.Build());
        }

        [Command("help"), Summary(@"도움말")]
        [Alias("h")]
        public async Task Helps()
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle("소울워커 새소식 알리미")
                .WithDescription("`!sw` 명령어 한 번만 입력")
                .WithColor(Color.Blue);

            await ReplyAsync("", false, builder.Build());
        }
    }
}

