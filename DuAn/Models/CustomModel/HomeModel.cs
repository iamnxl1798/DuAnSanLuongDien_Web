using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuAn.Models.CustomModel
{
    public class HomeModel
    {
        public double? duKienThang { get; set; }
        public double? duKienNam { get; set; }
        public double? thucTeThang { get; set; }
        public double? thucTeNam { get; set; }
        public string numberFormatThang { get { return String.Format("{0:### ### ### ###.##}", thucTeThang) == "   " ? "0" : String.Format("{0:### ### ### ###.##}", thucTeThang); } }
        public string numberFormatNam { get { return String.Format("{0:### ### ### ###.##}", thucTeNam) == "   " ? "0" : String.Format("{0:### ### ### ###.##}", thucTeNam); } }
        public string percentFormatNam { get { return String.Format("{0:#.##}", thucTeNam / duKienNam * 100).Length == 0 ? "0" : String.Format("{0:#.##}", thucTeNam / duKienNam * 100); } }
        public string percentFormatThang { get { return String.Format("{0:#.##}", thucTeThang / duKienThang * 100).Length == 0 ? "0" : String.Format("{0:#.##}", thucTeThang / duKienThang * 100); } }
        public List<DiemDoData> data { get; set; }
        public List<SanLuong> sanLuong { get; set; }
        public List<TongSanLuong_Ngay> sanLuongTrongNgay { get; set; }


    }
}