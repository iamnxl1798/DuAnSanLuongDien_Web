using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuAn.Models.CustomModel
{
   public class DiemDoViewModel
   {
      public int ID { get; set; }
      public int MaDiemDo { get; set; }
      public string TenDiemDo { get; set; }
      public string CongToSerial { get; set; }
      public int CongToID { get; set; }
      public string TinhChat { get; set; }
      public int TinhChatID { get; set; }
      public int? ThuTuHienThi { get; set; }
   }
}