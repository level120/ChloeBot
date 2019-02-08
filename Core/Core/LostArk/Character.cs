using System;
using System.Collections.Generic;
using System.Text;

namespace Core.LostArk
{
    class Character
    {
        public string name { get; set; }
        public string server { get; set; }
        public string cls { get; set; }
        public string guild { get; set; }
        public string tag { get; set; }
        public string item_lv { get; set; }
        public string char_lv { get; set; }
        public string acc_lv { get; set; }
        public string pvp_lv { get; set; }
        public string cls_img { get; set; }

        public bool isMyTurn { get; set; }
    }
}
