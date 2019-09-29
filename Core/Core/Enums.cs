using System;
using System.ComponentModel;

namespace ChloeBot
{
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