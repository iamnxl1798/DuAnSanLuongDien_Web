namespace DuAn.Models.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LogNhaMay")]
    public partial class LogNhaMay
    {
        public DateTime ThoiGian { get; set; }

        [Required]
        [StringLength(20)]
        public string TrangThai { get; set; }

        public int NhaMayID { get; set; }

        [Required]
        public string TenNhaMay { get; set; }

        [Required]
        [StringLength(50)]
        public string TenVietTat { get; set; }

        [Required]
        public string DiaChi { get; set; }

        public int CongTyID { get; set; }

        public int ID { get; set; }

        public virtual NhaMay NhaMay { get; set; }
    }
}
