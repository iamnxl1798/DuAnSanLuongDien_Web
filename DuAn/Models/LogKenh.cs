namespace DuAn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LogKenh")]
    public partial class LogKenh
    {
        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string TrangThai { get; set; }

        public DateTime ThoiGian { get; set; }

        public int KenhID { get; set; }

        [Required]
        [StringLength(10)]
        public string Ten { get; set; }

        public virtual Kenh Kenh { get; set; }
    }
}
