using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ChloeBot.Soulworker
{
    /// <summary>
    /// 게시글과 관련된 정보
    /// </summary>
    internal sealed class Board
    {
        public const string NoticeBoardName = "Notice";
        public const string DetailBoardName = "Update";
        public const string GMBoardName = "GMMagazine";
        public const string EventBoardName = "Event"; // _event, Promotion etc..

        public static string Notice { get; private set; } = string.Empty;
        public static string Detail { get; private set; } = string.Empty;
        public static string Event { get; private set; } = string.Empty;
        public static string GMMagazine { get; private set; } = string.Empty;
        private static List<string> _newItems { get; } = new List<string>();
        private static int _boardNums { get; } = Enum.GetNames(typeof(SoulworkerNewsType)).Length;
        private static string _filePath;

        private static bool _isUpdated => _newItems.Any();
        private static bool _isEmptyData =>
            Notice == string.Empty || Detail == string.Empty ||
            Event == string.Empty || GMMagazine == string.Empty;

        /// <summary>
        /// 파일에 저장된 정보로 최신글 동향을 복구합니다
        /// </summary>
        /// <param name="filePath">백업된 파일경로</param>
        public static void RecoveryItems(string filePath)
        {
            if (!_isEmptyData)
                return;

            _filePath = filePath;

            if (File.Exists(_filePath))
            {
                var data = File.ReadAllLines(_filePath);

                if (data.Length != _boardNums)
                    return;

                Notice = data[(int)SoulworkerNewsType.Notices];
                Detail = data[(int)SoulworkerNewsType.Updates];
                Event = data[(int)SoulworkerNewsType.Events];
                GMMagazine = data[(int)SoulworkerNewsType.GmMagazine];
            }
        }

        /// <summary>
        /// 최신 글 정보를 반영합니다.
        /// </summary>
        /// <param name="urls">Crawler로부터 받은 url 목록</param>
        public static void SetData(IList<string> urls)
        {
            var noticeItems = urls.Where(item => item.Contains(NoticeBoardName)).ToList();
            var detailItems = urls.Where(item => item.Contains(DetailBoardName)).ToList();
            var gmMagazineItems = urls.Where(item => item.Contains(GMBoardName)).ToList();
            var eventItems = urls.Except(noticeItems).Except(detailItems).Except(gmMagazineItems).ToList();

            if (Notice != string.Empty)
            {
                var item = GetNewItems(Notice, noticeItems)?.ToList();
                if (item?.Any() ?? false)
                    _newItems.AddRange(item);
            }

            if (Detail != string.Empty)
            {
                var item = GetNewItems(Detail, detailItems)?.ToList();
                if (item?.Any() ?? false)
                    _newItems.AddRange(item);
            }

            if (GMMagazine != string.Empty)
            {
                var item = GetNewItems(GMMagazine, gmMagazineItems)?.ToList();
                if (item?.Any() ?? false)
                    _newItems.AddRange(item);
            }

            if (Event != string.Empty)
            {
                var item = GetNewItems(Event, eventItems)?.ToList();
                if (item?.Any() ?? false)
                    _newItems.AddRange(item);
            }

            Notice = noticeItems.FirstOrDefault() ?? string.Empty;
            Detail = detailItems.FirstOrDefault() ?? string.Empty;
            GMMagazine = gmMagazineItems.FirstOrDefault() ?? string.Empty;
            Event = eventItems.FirstOrDefault() ?? string.Empty;
        }

        private static IEnumerable<string> GetNewItems(string item, List<string> items)
        {
            var itemIdx = items.FindIndex(i => i == item);
            return itemIdx < 1
                ? Enumerable.Empty<string>()
                : items.GetRange(0, itemIdx);
        }

        /// <summary>
        /// 새로운 소식을 가져옵니다.
        /// 만약 새로운 소식이 없다면 빈 문자열 리스트를 반환합니다.
        /// </summary>
        /// <returns>업데이트 된 url 목록</returns>
        public static IList<string> GetNews()
        {
            if (!_isUpdated)
                return new List<string>();

            var result = new List<string>(_newItems.Count);
            result.AddRange(_newItems);
            result.Reverse();

            StoreItems();

            _newItems.Clear();
            return result;
        }

        private static void StoreItems()
        {
            var data = new[]
            {
                Notice,
                Detail,
                Event,
                GMMagazine,
            };

            File.WriteAllLines(_filePath, data);
        }
    }
}