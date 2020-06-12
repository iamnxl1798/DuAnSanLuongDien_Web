namespace DuAn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TinhChatDiemDo")]
    public partial class TinhChatDiemDo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TinhChatDiemDo()
        {
            DiemDoes = new HashSet<DiemDo>();
            LogTinhChatDiemDoes = new HashSet<LogTinhChatDiemDo>();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string TenTinhChat { get; set; }

        public int STT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DiemDo> DiemDoes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LogTinhChatDiemDo> LogTinhChatDiemDoes { get; set; }
    }
}
