namespace ServiceTool.Model.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GiaDien")]
    public partial class GiaDien
    {
        public int ID { get; set; }

        public double Gia { get; set; }

        [Column(TypeName = "date")]
        public DateTime NgayBatDau { get; set; }

        [Column(TypeName = "date")]
        public DateTime NgayKetThuc { get; set; }
    }
}
