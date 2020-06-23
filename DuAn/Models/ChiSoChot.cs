namespace DuAn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ChiSoChot")]
    public partial class ChiSoChot
    {
        public int ID { get; set; }

        [StringLength(10)]
        public string CongToSerial { get; set; }

        [Column(TypeName = "date")]
        public DateTime? thang { get; set; }

        public double Tong { get; set; }

        public double ThapDiem { get; set; }

        public double BinhThuong { get; set; }

        public double CaoDiem { get; set; }

        public double PhanKhang { get; set; }
    }
}
