using DuAn.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuAn.Models.CustomModel
{
    public class HomeModel
    {
        public DateTime date { get; set; }
        public List<NumberOfMissingData> countMissingData { get; set; }
        public int daThuThap { get
            {
                return countMissingData.Select(x => x.done).Sum();
            } }
        public int chuaThuThap
        {
            get
            {
                return countMissingData.Select(x => x.notYet).Sum();
            }
        }
        public double doanhThuThang { get; set; }
        public double doanhThuNam { get; set; }
        public double? duKienThang { get; set; }
        public double? duKienNam { get; set; }
        public double thucTeThang { get; set; }
        public double thucTeNam { get; set; }
        public string numberFormatDuKienThang { get { return duKienThang == null ? "0" : String.Format("{0:### ### ### ##0.##}", duKienThang); } }
        public string numberFormatDuKienNam { get { return duKienNam == null ? "0" : String.Format("{0:### ### ### ##0.##}", duKienNam); } }
        public string numberFormatThang { get { return String.Format("{0:### ### ### ##0.##}", thucTeThang) == "   " ? "0" : String.Format("{0:### ### ### ##0.##}", thucTeThang); } }
        public string numberFormatNam { get { return String.Format("{0:### ### ### ##0.##}", thucTeNam) == "   " ? "0" : String.Format("{0:### ### ### ##0.##}", thucTeNam); } }
        public string percentFormatNam { get { return String.Format("{0:0.##}", thucTeNam / duKienNam * 100).Length == 0 ? "0" : String.Format("{0:0.##}", thucTeNam / duKienNam * 100); } }
        public string percentFormatThang { get { return String.Format("{0:0.##}", thucTeThang / duKienThang * 100).Length == 0 ? "0" : String.Format("{0:0.##}", thucTeThang / duKienThang * 100); } }
        public List<DiemDoData> data { get; set; }
        public List<TongSanLuong_Ngay> sanLuongTrongNgay { get; set; }
        public string formatNumber(double num)
        {
            var result = String.Format("{0:### ### ### ##0.##}", num);
            /*if (result.Split('.').Count() > 1 && string.IsNullOrEmpty(result.Split('.')[0].Trim()))
            {
                result = "0." + result.Split('.')[1];
            }*/
            return result == "   " ? "0" : result;
        }
        public double giaDien { get; set; }
    }
}