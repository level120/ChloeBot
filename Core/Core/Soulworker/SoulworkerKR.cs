using System.Collections.Generic;

namespace ChloeBot.Soulworker
{
    public class SoulworkerKR
    {
        public static readonly string PrefixUrl = @"http://soulworker.game.onstove.com";
        public static readonly string[] Urls =
            {
                @"http://soulworker.game.onstove.com/Notice/List",
                @"http://soulworker.game.onstove.com/Update/List",
                @"http://soulworker.game.onstove.com/Event/List",
                @"http://soulworker.game.onstove.com/GMMagazine/List"
            };
        public readonly List<ContextType> ClassType = new List<ContextType>();

        public string NoticeInfos { get; set; }
        public string UpdateInfos { get; set; }
        public string EventInfos { get; set; }
        public string GMMagazineInfos { get; set; }

        public SoulworkerKR()
        {
            ClassType.Add(
                new ContextType
                    {
                        Table = "b-list-normal",
                        Category = "t-category",
                        Subject = "t-subject",
                        Date = "t-date",
                        Recommend = "t-recommend"
                });
            ClassType.Add(
                new ContextType
                    {
                        Table = "b-list-normal",
                        Category = "t-category",
                        Subject = "t-subject",
                        Date = "t-date",
                        Recommend = "t-recommend"
                    });
            ClassType.Add(
                new ContextType
                    {
                        Ul = "b-list-event",
                        Thumb = "thumb",
                        Subject = "t-subject",
                        Info = "t-info"
                    });
            ClassType.Add(
                new ContextType
                    {
                        Ul = "b-list-event b-list-gm",
                        Thumb = "thumb",
                        Subject = "t-subject",
                        Info = "t-info",
                        Like = "t-like"
                });
        }

        public class ContextType
        {
            /// <summary>
            /// Common
            /// </summary>
            public string Subject { get; set; }

            /// <summary>
            /// Notice, Update
            /// </summary>
            public string Table { get; set; }
            public string Category { get; set; }
            public string Date { get; set; }
            public string Recommend { get; set; }

            /// <summary>
            /// Event, GM Magazine
            /// </summary>
            public string Ul { get; set; }
            public string Thumb { get; set; }
            public string Info { get; set; }


            /// <summary>
            /// GM Magazine
            /// </summary>
            public string Like { get; set; }
        }
    }
}