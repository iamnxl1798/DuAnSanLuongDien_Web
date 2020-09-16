using DuAn.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuAn.Models.CustomModel
{
    public class ThongSoPatialModel
    {
        public List<ThongSoAndTenDiemDo> thongSo { get; set; }
        public List<DiemDo> allDiemDo { get; set; }
        public string getTime(DateTime date) { return date.ToString("dd/MM/yyyy hh:mm"); }
        public List<DateTime> dateDistinc { get; set; }
        public int diemDoIndex{get;set;}
        public int thoiGianIndex { get; set; }
        public string getDiemDoAttr(int index)
        {
            return index == diemDoIndex ? "selected" : "";
        }
        public string getThoiGianAttr(int index)
        {
            return index == thoiGianIndex ? "selected" : "";
        }
        public string formatNumber(double num)
        {
            var result = String.Format("{0:### ### ### ##0.##}", num);
            return result== "   "? "0":result;
        }
    }
}