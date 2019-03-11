namespace Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Core.SoulworkerKR;
    using HtmlAgilityPack;

    public class LookingSoulworker
    {
        public static List<string> news = new List<string>();
        private static List<string> temp = new List<string>();
        private static List<string> olds = new List<string>();

        public static async Task StartSoulWorkerMonitoringAsync()
        {
            var contentsCount = 4;

            olds = FileDBReader();

            for (var idx = 0; idx < contentsCount; ++idx)
            {
                var url = SoulworkerKR.SoulworkerKR.Urls[idx];
                var httpClient = new HttpClient();
                var html = await httpClient.GetStringAsync(url);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                var structPageInfo = new SoulworkerKR.SoulworkerKR();

                var targetType = "";
                var targetTypeName = "";

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
                var res = htmlDocument.DocumentNode.Descendants(targetType).Where(
                    node => node.GetAttributeValue("class", "").Equals(targetTypeName)).ToList();

                if (idx < 2)
                {
                    target = res.FirstOrDefault()
                        .Descendants("tbody").FirstOrDefault()
                        .Descendants("p")
                        .Where(p => p.GetAttributeValue("class", "").Equals("ellipsis"))
                        .Select(p => SoulworkerKR.SoulworkerKR.PrefixUrl + p.ChildNodes.FirstOrDefault().GetAttributeValue("href", ""))
                        .ToList();
                }
                else if (idx == 2)
                {
                    res = htmlDocument.DocumentNode.Descendants(targetType).ToList();

                    target = res.FirstOrDefault()
                        .Descendants("div")
                        .Where(p => p.GetAttributeValue("class", "").Equals("thumb"))
                        .Select(p => @"http:" + p.ChildNodes[1].ChildNodes[0].GetAttributeValue("src", ""))
                        .ToList();
                }
                else
                {
                    target = res.FirstOrDefault()
                        .Descendants("div")
                        .Where(p => p.GetAttributeValue("class", "").Equals("t-subject"))
                        .Select(p => SoulworkerKR.SoulworkerKR.PrefixUrl + p.ChildNodes[1].GetAttributeValue("href", ""))
                        .ToList();
                }

                temp.AddRange(target.GetRange(0, 3));
            }

            var existNews = temp.Except(olds).ToList();

            if (existNews.Count != 0)
            {
                FileDBWritter(temp);
            }

            lock (news)
            {
                news = existNews;
            }
        }

        public static void ResetResult()
        {
            lock (news)
            {
                var count = news.Count;
                news.RemoveRange(0, count);
            }
        }

        private static List<string> FileDBReader()
        {
            var res = new List<string>();
            var dbName = @"db.txt";

            if (!File.Exists(dbName))
            {
                using (File.Create(dbName))
                {
                }
            }
            res.AddRange(File.ReadAllLines(dbName));

            return res;
        }

        private static void FileDBWritter(List<string> db)
        {
            using (StreamWriter w = new StreamWriter(@"db.txt"))
            {
                lock (w)
                {
                    foreach (var d in db)
                    {
                        w.WriteLine(d);
                    }
                }
            }
        }
    }
}