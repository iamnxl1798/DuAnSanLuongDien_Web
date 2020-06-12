namespace DuAn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SanLuong")]
    public partial class SanLuong
    {
        public int ID { get; set; }

        [Column(TypeName = "date")]
        public DateTime Ngay { get; set; }

        public int DiemDoID { get; set; }

        public int KenhID { get; set; }

        public short ChuKy { get; set; }

        public double GiaTri { get; set; }

        public virtual DiemDo DiemDo { get; set; }

        public virtual Kenh Kenh { get; set; }
    }
}
