namespace DuAn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ThongSoVanHanh")]
    public partial class ThongSoVanHanh
    {
        [Required]
        [StringLength(10)]
        public string Serial { get; set; }

        public DateTime ThoiGianCongTo { get; set; }

        public double P_Giao { get; set; }

        public double P_Nhan { get; set; }

        public double Q_Giao { get; set; }

        public double Q_Nhan { get; set; }

        public double P_Giao_TD { get; set; }

        public double P_Giao_BT { get; set; }

        public double P_Giao_CD { get; set; }

        public double P_Nhan_TD { get; set; }

        public double P_Nhan_BT { get; set; }

        public double P_Nhan_CD { get; set; }

        public double PhaseA_Amps { get; set; }

        public double PhaseA_Volts { get; set; }

        public double PhaseA_PowerFactor { get; set; }

        public double PhaseA_Frequency { get; set; }

        public double PhaseA_Angle { get; set; }

        public double PhaseB_Amps { get; set; }

        public double PhaseB_Volts { get; set; }

        public double PhaseB_PowerFactor { get; set; }

        public double PhaseB_Frequency { get; set; }

        public double PhaseB_Angle { get; set; }

        public double PhaseC_Amps { get; set; }

        public double PhaseC_Volts { get; set; }

        public double PhaseC_PowerFactor { get; set; }

        public double PhaseC_Frequency { get; set; }

        public double PhaseC_Angle { get; set; }

        [Required]
        [StringLength(20)]
        public string Phase_Rotation { get; set; }

        public int ID { get; set; }
    }
}
