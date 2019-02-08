using Discord;
using Discord.Commands;
using System.Threading.Tasks;

// https://discord.foxbot.me/docs/guides/commands/commands.html
// https://discord4j.readthedocs.io/en/latest/Making-embedded-content-using-EmbedBuilder/
namespace Core
{
    //[Group("!")]
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task PingAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Ping!")
                .WithDescription($"Pong! {Context.User.Mention} Ping 체크 완료!")
                .WithColor(Color.Orange);

            await ReplyAsync("", false, builder.Build());
        }

        [Command("userinfo"), Summary("Returns info about the current user, or the user parameter, if one passed.")]
        [Alias("user", "whois")]
        public async Task UserInfo([Summary("The (optional) user to get info for")] IUser user = null)
        {
            var userInfo = user ?? Context.Client.CurrentUser;
            await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
        }

        [Command("maplestory"), Summary(@"캐릭터명, 랭킹모드, 서버")]
        [Alias("m", "maple")]
        public async Task MapleStatus([Summary("maple")] string charName = "", int mode = (int)Maplestory.Maplestory.modeList.Total, int server = (int)Maplestory.Maplestory.serverList.normalAll)
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

        [Command("lostark"), Summary(@"캐릭터명")]
        [Alias("l", "loa")]
        public async Task LostArkStatus([Summary("lostark")] string charName = "")
        {
            EmbedBuilder builder = new EmbedBuilder();

            // 크롤링 시작
            await Crawling.StartLostArkCrawlerasync(charName);

            // 결과대기
            while (!Crawling.loa_char.isMyTurn)
            {
                await Task.Delay(100);
            }

            lock (Crawling.loa_char)
            {
                builder.WithTitle($"[{Crawling.loa_char.char_lv} {Crawling.loa_char.name}]님의 정보")
                    .WithImageUrl($"http:{Crawling.loa_char.cls_img}")
                    .AddField("서버", $"{Crawling.loa_char.server}", true)
                    .AddField("클래스", $"{Crawling.loa_char.cls}", true)
                    .AddField("길드", Crawling.loa_char.guild, true)
                    .AddField("아이템", Crawling.loa_char.item_lv, true)
                    .AddField("원정대", Crawling.loa_char.acc_lv, true)
                    .AddField("PVP", Crawling.loa_char.pvp_lv, true)
                    .WithColor(Color.DarkPurple);

                // 다음동작을 위해 결과삭제
                Crawling.loa_char.isMyTurn = false;
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("help"), Summary(@"도움말")]
        [Alias("h")]
        public async Task Helps()
        {
            EmbedBuilder builder = new EmbedBuilder();
            string description
                = $"[랭킹정보]\n" +
                $"종합\t: {Maplestory.Maplestory.modeList.Total}\n" +
                $"길드\t: {Maplestory.Maplestory.modeList.Guild}\n" +
                $"인기도\t: {Maplestory.Maplestory.modeList.Pop}\n" +
                $"도장\t: {Maplestory.Maplestory.modeList.Dojang}\n" +
                $"더시드\t: {Maplestory.Maplestory.modeList.Seed}\n" +
                $"\n사용방법\t : !m 아루루 랭킹정보\n" +
                $"\n사용방법\t : !m 아루루 {Maplestory.Maplestory.modeList.Total}\n" +
                $"\n\n\n" +
                $"[서버정보]\n" +
                $"전체\t: {Maplestory.Maplestory.serverList.normalAll}\n" +
                $"아케인\t: {Maplestory.Maplestory.serverList.akein}\n" +
                $"오로라\t: {Maplestory.Maplestory.serverList.aurora}\n" +
                $"버닝\t: {Maplestory.Maplestory.serverList.burning}\n" +
                $"크로아\t: {Maplestory.Maplestory.serverList.croa}\n" +
                $"엘리시움\t: {Maplestory.Maplestory.serverList.elesium}\n" +
                $"이노시스\t: {Maplestory.Maplestory.serverList.inosis}\n" +
                $"루나\t: {Maplestory.Maplestory.serverList.luna}\n" +
                $"노바\t: {Maplestory.Maplestory.serverList.nova}\n" +
                $"리부트1\t: {Maplestory.Maplestory.serverList.reboot1}\n" +
                $"리부트2\t: {Maplestory.Maplestory.serverList.reboot2}\n" +
                $"레드\t: {Maplestory.Maplestory.serverList.red}\n" +
                $"스카니아\t: {Maplestory.Maplestory.serverList.scania}\n" +
                $"유니온\t: {Maplestory.Maplestory.serverList.union}\n" +
                $"베라\t: {Maplestory.Maplestory.serverList.vera}\n" +
                $"제니스\t: {Maplestory.Maplestory.serverList.zenis}\n" +
                $"리부터전체\t: {Maplestory.Maplestory.serverList.rebootAll}\n" +
                $"\n사용방법\t : !m 아루루 랭킹정보 서버정보" +
                $"\n사용방법\t : !m 아루루 {Maplestory.Maplestory.modeList.Total} {Maplestory.Maplestory.serverList.akein}";

            builder.WithTitle("도움말")
                .WithDescription("--준비중--")
                .WithColor(Color.Green);

            await ReplyAsync("", false, builder.Build());
        }
    }
}

