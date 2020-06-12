namespace DuAn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LogCongTy")]
    public partial class LogCongTy
    {
        public int ID { get; set; }

        public DateTime ThoiGian { get; set; }

        [Required]
        [StringLength(20)]
        public string TrangThai { get; set; }

        public int CongTyID { get; set; }

        [Required]
        [StringLength(50)]
        public string TenCongTy { get; set; }

        [Required]
        [StringLength(10)]
        public string TenVietTat { get; set; }

        public virtual CongTy CongTy { get; set; }
    }
}
