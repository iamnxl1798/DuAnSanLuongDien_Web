namespace DuAn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TongSanLuong_ThangNam
    {
        public int ID { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Ngay { get; set; }

        public int KenhID { get; set; }

        public int LoaiID { get; set; }

        public double? GiaTri { get; set; }

        public virtual LoaiSanLuong LoaiSanLuong { get; set; }
    }
}
