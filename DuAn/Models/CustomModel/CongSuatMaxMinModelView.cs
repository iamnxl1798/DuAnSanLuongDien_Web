using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuAn.Models.CustomModel
{
    public class CongSuatMaxMinModelView
    {
        public string TenDiemDo { get; set; }
        public string Kenh { get; set; }
        public double GiaTriMax { get; set; }
        public int ChuKyMax { get; set; }
        public double GiaTriMin { get; set; }
        public int ChuKyMin { get; set; }
    }
}