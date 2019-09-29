using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ChloeBot.Soulworker
{
    internal sealed class Board
    {
        public const string NoticeBoardName = "Notice";
        public const string DetailBoardName = "Update";
        public const string GMBoardName = "GMMagazine";
        public const string EventBoardName = "Event"; // _event, Promotion etc..

        private static string _notice { get; set; } = string.Empty;
        private static string _detail { get; set; } = string.Empty;
        private static string _event { get; set; } = string.Empty;
        private static string _gmMagazine { get; set; } = string.Empty;
        private static List<string> _newItems { get; } = new List<string>();
        private static int _boardNums { get; } = Enum.GetNames(typeof(eSoulworkerNewsType)).Length;
        private static string _filePath;

        private static bool _isUpdated => _newItems.Any();
        private static bool _isEmptyData =>
            _notice == string.Empty || _detail == string.Empty ||
            _event == string.Empty || _gmMagazine == string.Empty;

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

                _notice = data[(int)eSoulworkerNewsType.notices];
                _detail = data[(int)eSoulworkerNewsType.updates];
                _event = data[(int)eSoulworkerNewsType.events];
                _gmMagazine = data[(int)eSoulworkerNewsType.gmMagazine];
            }
        }

        /// <summary>
        /// 최신 글 정보를 반영합니다.
        /// </summary>
        /// <param name="urls">게시판으로부터 파싱된 url 목록</param>
        public static void SetData(IList<string> urls)
        {
            var noticeItems = urls.Where(item => item.Contains(NoticeBoardName)).ToList();
            var detailItems = urls.Where(item => item.Contains(DetailBoardName)).ToList();
            var gmMagazineItems = urls.Where(item => item.Contains(GMBoardName)).ToList();
            var eventItems = urls.Except(noticeItems).Except(detailItems).Except(gmMagazineItems).ToList();

            if (_notice != string.Empty)
            {
                var item = GetNewItems(_notice, noticeItems)?.ToList();
                if (item?.Any() ?? false)
                    _newItems.AddRange(item);
            }

            if (_detail != string.Empty)
            {
                var item = GetNewItems(_detail, detailItems)?.ToList();
                if (item?.Any() ?? false)
                    _newItems.AddRange(item);
            }

            if (_gmMagazine != string.Empty)
            {
                var item = GetNewItems(_gmMagazine, gmMagazineItems)?.ToList();
                if (item?.Any() ?? false)
                    _newItems.AddRange(item);
            }

            if (_event != string.Empty)
            {
                var item = GetNewItems(_event, eventItems)?.ToList();
                if (item?.Any() ?? false)
                    _newItems.AddRange(item);
            }

            _notice = noticeItems.FirstOrDefault() ?? string.Empty;
            _detail = detailItems.FirstOrDefault() ?? string.Empty;
            _gmMagazine = gmMagazineItems.FirstOrDefault() ?? string.Empty;
            _event = eventItems.FirstOrDefault() ?? string.Empty;
        }

        private static IEnumerable<string> GetNewItems(string item, List<string> items)
        {
            var itemIdx = items.FindIndex(i => i == item);

            if (itemIdx < 1)
                return null;

            return items.GetRange(0, itemIdx);
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
                _notice,
                _detail,
                _event,
                _gmMagazine,
            };

            File.WriteAllLines(_filePath, data);
        }
    }
}