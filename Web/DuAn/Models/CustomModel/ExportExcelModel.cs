using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuAn.Models.CustomModel
{
    public class ExportExcelModel
    {
        public string type { get; set; }
        public int maDiemDo { get; set; }
        public KenhCustom dienNangGiao { get; set; }
        public KenhCustom dienNangNhan { get; set; }
    }
}