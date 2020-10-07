using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuAn.Models.CustomModel
{
   public class CapNhatDiemDo_CongToViewModel
   {
      public int DiemDoID { get; set; }
      public int CongToID { get; set; }
      public string Serial { get; set; }
      public string Type { get; set; }
      public DateTime ThoiGianBatDau { get; set; }
      public DateTime ThoiGianKetThuc { get; set; }
   }
}