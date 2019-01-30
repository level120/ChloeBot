using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Maplestory
{
    class Maplestory
    {
        private readonly string urlBody = @"https://maplestory.nexon.com/Ranking/World/";
        private readonly string total = "Total";    // 종합 랭킹
        private readonly string pop = "Pop";        // 인기도 랭킹
        private readonly string guild = "Guild";    // 길드 랭킹
        private readonly string dojang = "Dojang";  // 무릉도장 랭킹
        private readonly string seed = "Seed";      // 더 시드 랭킹

        public enum serverList { normalAll=0, reboot2, reboot1, aurora, red, inosis, union, scania, luna, zenis, croa, vera, elesium, akein, nova, burning, rebootAll=254 };
        public enum modeList { Total=0, Pop, Guild, Dojang, Seed };

        /*
         * server : 서버명(serverList)
         * mode : 랭킹목록(modeList)
         */
        public String GetCrawlingUrls(Int32 server, Int32 mode)
        {
            String returnUrls = $"{urlBody}";

            switch (mode)
            {
                case (int) modeList.Total:
                    returnUrls += $"{total}?w={server}";
                    break;
                case (int)modeList.Pop:
                    if (server == (int)serverList.rebootAll) { return "Only use total"; }
                    returnUrls += $"{pop}?w={server}";
                    break;
                case (int)modeList.Guild:
                    if (server == (int)serverList.rebootAll) { return "Only use total"; }
                    returnUrls += $"{guild}?w={server}";
                    break;
                case (int)modeList.Dojang:
                    if (server == (int)serverList.rebootAll) { return "Only use total"; }
                    returnUrls += $"{dojang}?w={server}";
                    break;
                case (int)modeList.Seed:
                    if (server == (int)serverList.rebootAll) { return "Only use total"; }
                    returnUrls += $"{seed}?w={server}";
                    break;
                default:
                    return "Failed urls";
            }

            return returnUrls;
        }

        public String GetCrawlingUrls(String charName)
        {
            return $"{GetCrawlingUrls((int)serverList.normalAll, (int)modeList.Total)}&c={charName}";
        }

        public String GetCrawlingUrls(Int32 server, Int32 mode, String charName)
        {
            return $"{GetCrawlingUrls(server, mode)}&c={charName}";
        }
    }
}
