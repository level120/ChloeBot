using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ChloeBot.Maplestory;
using HtmlAgilityPack;

/*
 * https://medium.com/@thepen0411/web-crawling-tutorial-in-c-48d921ef956a
 */
namespace ChloeBot.Crawling
{
    public class Crawling
    {
        public static List<Character> ResMaple = new List<Character>();

        public static async Task StartMapleCrawlerasync(string url)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var res = htmlDocument.DocumentNode.Descendants("tr")
                .Where(node => node.GetAttributeValue("class", "").Equals("search_com_chk")).ToList();

            ResMaple.RemoveRange(0, ResMaple.Count);

            if (res.Count != 0)
            {
                foreach (var tr in res)
                {
                    var maple = new Character
                    {
                        rank = tr.Descendants("p").FirstOrDefault()?.InnerHtml.Substring(2).Split()[0] ?? string.Empty,
                        changeRank = tr.Descendants("span").FirstOrDefault()?.ChildNodes[4].InnerHtml.Split()[0] ?? string.Empty,
                        imgUrl = tr.ChildNodes[3].ChildNodes[1].Descendants("img").FirstOrDefault()?.ChildAttributes("src").FirstOrDefault()?.Value ?? string.Empty,
                        imgServer = tr.ChildNodes[3].ChildNodes[3].Descendants("img").FirstOrDefault()?.ChildAttributes("src").FirstOrDefault()?.Value ?? string.Empty,
                        lv = tr.ChildNodes[5].InnerText,
                        exp = tr.ChildNodes[7].InnerText,
                        pop = tr.ChildNodes[9].InnerText,
                        guild = tr.ChildNodes[11].InnerText,
                        job = tr.ChildNodes[3].ChildNodes[3].InnerText.Replace(" ", "").Split("\r\n")[2],
                        name = tr.ChildNodes[3].ChildNodes[3].InnerText.Replace(" ", "").Split("\r\n")[1]
                    };

                    ResMaple.Add(maple);
                }
            }
            else
            {
                var maple = new Character
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

                ResMaple.Add(maple);
            }
        }
    }
}
