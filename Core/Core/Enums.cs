using System.ComponentModel;

namespace ChloeBot
{
    public enum SoulworkerNewsType
    {
        [Description("공지사항")]
        Notices,
        [Description("업데이트")]
        Updates,
        [Description("이벤트")]
        Events,
        [Description("GM매거진")]
        GmMagazine,
    }
}