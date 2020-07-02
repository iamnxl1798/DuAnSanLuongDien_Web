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

        public double TongGiao { get; set; }

        public double ThapDiemGiao { get; set; }

        public double BinhThuongGiao { get; set; }

        public double CaoDiemGiao { get; set; }

        public double PhanKhangGiao { get; set; }

        public double TongNhan { get; set; }

        public double ThapDiemNhan { get; set; }

        public double BinhThuongNhan { get; set; }

        public double CaoDiemNhan { get; set; }

        public double PhangKhangNhan { get; set; }
    }
}
