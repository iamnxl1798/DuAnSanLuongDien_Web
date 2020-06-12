namespace DuAn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LogDiemDo")]
    public partial class LogDiemDo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public DateTime ThoiGian { get; set; }

        [Required]
        [StringLength(20)]
        public string TrangThai { get; set; }

        public int DiemDoID { get; set; }

        public int MaDiemDo { get; set; }

        [Required]
        [StringLength(10)]
        public string TenDiemDo { get; set; }

        [Required]
        [StringLength(10)]
        public string NhaMayID { get; set; }

        public int TinhChatID { get; set; }

        public virtual DiemDo DiemDo { get; set; }
    }
}
