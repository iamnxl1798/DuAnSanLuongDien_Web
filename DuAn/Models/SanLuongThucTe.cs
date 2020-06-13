namespace DuAn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SanLuongThucTe")]
    public partial class SanLuongThucTe
    {
        public int ID { get; set; }

        public int DiemDoID { get; set; }

        public double SanLuong { get; set; }

        public int KenhID { get; set; }

        [Column(TypeName = "date")]
        public DateTime Ngay { get; set; }

        public virtual DiemDo DiemDo { get; set; }

        public virtual Kenh Kenh { get; set; }
    }
}
