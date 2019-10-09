using System.ComponentModel;

namespace ChloeBot
{
    public enum eSoulworkerNewsType
    {
        [Description("공지사항")]
        notices,
        [Description("업데이트")]
        updates,
        [Description("이벤트")]
        events,
        [Description("GM매거진")]
        gmMagazine
    }
}