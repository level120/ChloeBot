namespace ChloeBot.Maplestory
{
    public class MaplestoryKR
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
