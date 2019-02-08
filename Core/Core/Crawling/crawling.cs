using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

/*
 * https://medium.com/@thepen0411/web-crawling-tutorial-in-c-48d921ef956a
 */
namespace Core
{
    class Crawling
    {
        public static List<Maplestory.Character> res_maple = new List<Maplestory.Character>();
        public static LostArk.Character loa_char = new LostArk.Character();

        public static async Task StartLostArkCrawlerasync(string name)
        {
            var url = $"http://lostark.game.onstove.com/Profile/Character/{name}";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var res = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Equals("profile-character")).ToList();

            lock (loa_char)
            {
                loa_char.isMyTurn = true;

                loa_char.char_lv = res[0].Descendants("h3").FirstOrDefault().InnerHtml.Split()[0];
                loa_char.name = res[0].Descendants("h3").FirstOrDefault().InnerHtml.Split()[1];
                loa_char.server = res[0].Descendants("span").ElementAt(1).InnerHtml;
                loa_char.cls = res[0].Descendants("span").ElementAt(5).InnerHtml;
                loa_char.guild = res[0].Descendants("span").ElementAt(3).InnerText;
                loa_char.tag = res[0].Descendants("span").ElementAt(7).InnerText;
                loa_char.item_lv = res[0].Descendants("span").ElementAt(9).InnerText;
                loa_char.acc_lv = res[0].Descendants("span").ElementAt(11).InnerText;
                loa_char.pvp_lv = res[0].Descendants("span").ElementAt(13).InnerText;
                loa_char.cls_img = res[0].SelectNodes("//div[@class='profile-equipment__character']/img").FirstOrDefault().GetAttributeValue("src", "");
            }
        }

        public static async Task StartMapleCrawlerasync(string _url)
        {
            var url = _url;
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var res = htmlDocument.DocumentNode.Descendants("tr")
                .Where(node => node.GetAttributeValue("class", "").Equals("search_com_chk")).ToList();

            res_maple.RemoveRange(0, res_maple.Count);

            if (res.Count != 0)
            {
                foreach (var tr in res)
                {
                    var maple = new Maplestory.Character
                    {
                        rank = tr.Descendants("p").FirstOrDefault().InnerHtml.Substring(2).Split()[0],
                        changeRank = tr.Descendants("span").FirstOrDefault().ChildNodes[4].InnerHtml.Split()[0],
                        imgUrl = tr.ChildNodes[3].ChildNodes[1].Descendants("img").FirstOrDefault().ChildAttributes("src").FirstOrDefault().Value,
                        imgServer = tr.ChildNodes[3].ChildNodes[3].Descendants("img").FirstOrDefault().ChildAttributes("src").FirstOrDefault().Value,
                        lv = tr.ChildNodes[5].InnerText,
                        exp = tr.ChildNodes[7].InnerText,
                        pop = tr.ChildNodes[9].InnerText,
                        guild = tr.ChildNodes[11].InnerText,
                        job = tr.ChildNodes[3].ChildNodes[3].InnerText.Replace(" ", "").Split("\r\n")[2],
                        name = tr.ChildNodes[3].ChildNodes[3].InnerText.Replace(" ", "").Split("\r\n")[1]
                    };

                    res_maple.Add(maple);
                }
            }
            else
            {
                var maple = new Maplestory.Character
                {
                    rank = "-",
                    changeRank = "-",
                    imgUrl = "https://ssl.nx.com/s2/game/maplestory/renewal/common/rank_other.png",
                    imgServer = @":rotating_light:",
                    lv = "-",
                    exp = "-",
                    pop = "-",
                    guild = "-",
                    job = "-",
                    name = "결과없음"
                };

                res_maple.Add(maple);
            }
        }
    }
}
