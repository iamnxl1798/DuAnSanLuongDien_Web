namespace DuAn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TongSanLuong_Ngay
    {
        public int ID { get; set; }

        [Column(TypeName = "date")]
        public DateTime Ngay { get; set; }

        public int? ChuKy { get; set; }

        public double? GiaTri { get; set; }

        public int CongThucID { get; set; }

        public virtual CongThucTongSanLuong CongThucTongSanLuong { get; set; }
    }
}
