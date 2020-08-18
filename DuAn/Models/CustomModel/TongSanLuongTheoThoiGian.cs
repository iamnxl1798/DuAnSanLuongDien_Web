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
        public string numberFormatValue { get { return String.Format("{0:### ### ### ###.##}", value); } }
    }
}