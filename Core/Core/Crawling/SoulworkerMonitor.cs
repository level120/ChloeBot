using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using ChloeBot.Soulworker;
using Discord;
using HtmlAgilityPack;

namespace ChloeBot.Crawling
{
    public class SoulworkerMonitor
    {
        private static List<string> News = new List<string>();
        private static List<string> Temp = new List<string>();
        private static List<string> Olds = new List<string>();

        private static HttpClient HttpClient = new HttpClient();
        private static HtmlDocument HtmlDocument = new HtmlDocument();

        private const int ContentsCount = 4;

        public static IList<EmbedBuilder> Run()
        {
            UpdateBoardData();
            var result = GetReplyBuilder().ToList();

            if (result.Any())
                News.Clear();

            return result;
        }

        private static IEnumerable<EmbedBuilder> GetReplyBuilder()
        {
            foreach (var imageUrl in News)
            {
                var titleString = string.Empty;
                var builder = new EmbedBuilder()
                {
                    Color = Color.Orange,
                };

                if (imageUrl.Contains("Notice")) titleString = "**[공지사항]**";
                else if (imageUrl.Contains("Update")) titleString = "**[업데이트]**";
                else if (imageUrl.Contains("GMMagazine")) titleString = "**[GM매거진]**";
                else
                {
                    titleString = "**[이벤트]**";
                    builder.WithTitle($"{titleString} 새로운 게시글이 올라왔어요!")
                        .WithDescription("")
                        .WithImageUrl(imageUrl);

                    yield return builder;
                }

                builder.WithTitle($"{titleString} 새로운 게시글이 올라왔어요!")
                    .WithDescription(imageUrl)
                    .WithImageUrl("");

                yield return builder;
            }
        }

        private static async void UpdateBoardData()
        {
            if (!Olds.Any())
                Olds = FileDbReader();

            for (var idx = 0; idx < ContentsCount; ++idx)
            {
                var url = SoulworkerKR.Urls[idx];
                var html = await HttpClient.GetStringAsync(url);
                HtmlDocument.LoadHtml(html);

                var structPageInfo = new SoulworkerKR();

                var targetType = string.Empty;
                var targetTypeName = string.Empty;

                if (idx < 2)
                {
                    targetType = "table";
                    targetTypeName = structPageInfo.ClassType[idx].Table;
                }
                else
                {
                    targetType = "ul";
                    targetTypeName = structPageInfo.ClassType[idx].Ul;
                }

                var target = new List<string>();
                var res = HtmlDocument.DocumentNode.Descendants(targetType)
                    .Where(node => node.GetAttributeValue("class", "") == targetTypeName)
                    .ToList();

                if (idx < 2)
                {
                    target = res.FirstOrDefault()
                        ?.Descendants("tbody").FirstOrDefault()
                        ?.Descendants("p")
                        .Where(p => p.GetAttributeValue("class", "").Equals("ellipsis"))
                        .Select(p => SoulworkerKR.PrefixUrl + p.ChildNodes.FirstOrDefault()?.GetAttributeValue("href", ""))
                        .ToList();
                }
                else if (idx == 2)
                {
                    res = HtmlDocument.DocumentNode.Descendants(targetType).ToList();

                    target = res.FirstOrDefault()
                        ?.Descendants("div")
                        .Where(p => p.GetAttributeValue("class", "").Equals("thumb"))
                        .Select(p => @"http:" + p.ChildNodes[1].ChildNodes[0].GetAttributeValue("src", ""))
                        .ToList();
                }
                else
                {
                    target = res.FirstOrDefault()
                        ?.Descendants("div")
                        .Where(p => p.GetAttributeValue("class", "").Equals("t-subject"))
                        .Select(p => SoulworkerKR.PrefixUrl + p.ChildNodes[1].GetAttributeValue("href", ""))
                        .ToList();
                }

                if (target != null && target.Any())
                    Temp.AddRange(target);
            }

            if (!ParseData())
                return;

            var existNews = Temp.Except(Olds).ToList();
            if (existNews.Any())
            {
                ChangeData(existNews);
                FileDBWritter(Temp);
            }
        }

        private static bool ParseData()
        {
            if (!Olds.Any())
            {
                FileDBWritter(Temp);
                Olds.AddRange(Temp);
                return false;
            }

            // todo: url 별로 구분해 Olds에 저장하고 마지막 숫자로 구분(이벤트는 별개로 진행, 숫자가 매우 크니 stringCompare 사용)

            return true;
        }

        private static void ChangeData(IEnumerable<string> existNews)
        {
            Olds.Clear();
            Olds.AddRange(Temp);

            News.Clear();
            News.AddRange(existNews);
        }

        private static List<string> FileDbReader()
        {
            var res = new List<string>();
            var dbName = @"db.txt";

            if (!File.Exists(dbName))
                File.Create(dbName).Close();

            res.AddRange(File.ReadAllLines(dbName));
            return res;
        }

        private static void FileDBWritter(IEnumerable<string> db)
        {
            File.WriteAllLines(@"db.txt", db);
        }
    }
}