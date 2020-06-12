namespace DuAn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LogTinhChatDiemDo")]
    public partial class LogTinhChatDiemDo
    {
        public int ID { get; set; }

        public DateTime ThoiGian { get; set; }

        [Required]
        [StringLength(20)]
        public string TrangThai { get; set; }

        public int TinhChatDiemDoID { get; set; }

        [Required]
        [StringLength(20)]
        public string TenTinhChat { get; set; }

        public int STT { get; set; }

        public virtual TinhChatDiemDo TinhChatDiemDo { get; set; }
    }
}
