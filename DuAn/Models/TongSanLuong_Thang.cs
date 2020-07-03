namespace DuAn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TongSanLuong_Thang
    {
        public int ID { get; set; }

        public int Thang { get; set; }

        public int Nam { get; set; }

        public double? GiaTri { get; set; }
    }
}
