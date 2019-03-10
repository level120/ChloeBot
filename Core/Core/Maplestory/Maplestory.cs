namespace Core.Maplestory
{
    using System.ComponentModel;

    public enum eServerList
    {
        [Description("전체서버")]
        normalAll = 0,
        [Description("리부트2")]
        reboot2,
        [Description("리부트1")]
        reboot1,
        [Description("오로라")]
        aurora,
        [Description("레드")]
        red,
        [Description("이노시스")]
        inosis,
        [Description("유니온")]
        union,
        [Description("스카니아")]
        scania,
        [Description("루나")]
        luna,
        [Description("제니스")]
        zenis,
        [Description("크로아")]
        croa,
        [Description("베라")]
        vera,
        [Description("엘레시움")]
        elesium,
        [Description("아케인")]
        akein,
        [Description("노바")]
        nova,
        [Description("버닝")]
        burning,
        [Description("리부트 전체")]
        rebootAll = 254,
    };

    public enum eModeList
    {
        [Description("전체 랭킹")]
        Total = 0,
        [Description("인기도 랭킹")]
        Pop,
        [Description("길드 랭킹")]
        Guild,
        [Description("도장 랭킹")]
        Dojang,
        [Description("시드 랭킹")]
        Seed,
    };

    class Maplestory
    {
        private readonly string urlBody = @"https://maplestory.nexon.com/Ranking/World/";
        private readonly string total = "Total";    // 종합 랭킹
        private readonly string pop = "Pop";        // 인기도 랭킹
        private readonly string guild = "Guild";    // 길드 랭킹
        private readonly string dojang = "Dojang";  // 무릉도장 랭킹
        private readonly string seed = "Seed";      // 더 시드 랭킹

        /*
         * server : 서버명(serverList)
         * mode : 랭킹목록(eModeList)
         */
        public string GetCrawlingUrls(eServerList server, eModeList mode)
        {
            var returnUrls = $"{urlBody}";

            switch (mode)
            {
                case eModeList.Total:
                    returnUrls += $"{total}?w={(int)server}";
                    break;
                case eModeList.Pop:
                    returnUrls += $"{pop}?w={(int)server}";
                    break;
                case eModeList.Guild:
                    returnUrls += $"{guild}?w={(int)server}";
                    break;
                case eModeList.Dojang:
                    returnUrls += $"{dojang}?w={(int)server}";
                    break;
                case eModeList.Seed:
                    returnUrls += $"{seed}?w={(int)server}";
                    break;
                default:
                    return "Failed urls";
            }

            return returnUrls;
        }

        public string GetCrawlingUrls(string charName)
        {
            return $"{GetCrawlingUrls(eServerList.normalAll, eModeList.Total)}&c={charName}";
        }

        public string GetCrawlingUrls(eServerList server, eModeList mode, string charName)
        {
            return $"{GetCrawlingUrls(server, mode)}&c={charName}";
        }
    }
}
