using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuAn.Models.CustomModel
{
    public class TongSanLuongTheoThoiGian
    {
        public double value { get; set; }
        public DateTime date { get; set; }
        public double giaTien { get; set; }
        public double doanhThu { get; set; }
        public string numberFormatValue { get { return String.Format("{0:### ### ### ###.##}", value); } }
        public string formatNumber(double num)
        {
            var result = String.Format("{0:### ### ### ###.##}", num);
            return result == "   " ? "0" : result;
        }
    }
}