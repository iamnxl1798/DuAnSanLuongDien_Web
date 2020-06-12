namespace DuAn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SanLuongDuKien")]
    public partial class SanLuongDuKien
    {
        public int ID { get; set; }

        [Column(TypeName = "date")]
        public DateTime ThoiGian { get; set; }

        public int LoaiID { get; set; }

        public double SanLuong { get; set; }

        public virtual LoaiSanLuong LoaiSanLuong { get; set; }
    }
}
