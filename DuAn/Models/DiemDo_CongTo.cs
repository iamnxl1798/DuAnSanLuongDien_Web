namespace DuAn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DiemDo-CongTo")]
    public partial class DiemDo_CongTo
    {
        public int ID { get; set; }

        public int DiemDoID { get; set; }

        public int CongToID { get; set; }

        public DateTime ThoiGianBatDau { get; set; }

        public DateTime? ThoiGianKetThuc { get; set; }

        public virtual CongTo CongTo { get; set; }

        public virtual DiemDo DiemDo { get; set; }
    }
}
