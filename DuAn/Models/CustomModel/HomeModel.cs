using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuAn.Models.CustomModel
{
    public class HomeModel
    {
        public double duKienThang { get; set; }
        public double duKienNam { get; set; }
        public double thucTeThang { get; set; }
        public double thucTeNam { get; set; }
        public List<DiemDoData> data { get; set; }
        public List<SanLuong> sanLuong { get; set; }
        public List<SanLuong> sanLuongTrongNgay { get; set; }


    }
}