namespace DuAn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DiemDo")]
    public partial class DiemDo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DiemDo()
        {
            DiemDo_CongTo = new HashSet<DiemDo_CongTo>();
            LogDiemDoes = new HashSet<LogDiemDo>();
            SanLuongs = new HashSet<SanLuong>();
            SanLuongThucTes = new HashSet<SanLuongThucTe>();
        }

        public int ID { get; set; }

        public int MaDiemDo { get; set; }

        [Required]
        [StringLength(10)]
        public string TenDiemDo { get; set; }

        public int NhaMayID { get; set; }

        public int TinhChatID { get; set; }

        public int ThuTuHienThu { get; set; }

        public virtual NhaMay NhaMay { get; set; }

        public virtual TinhChatDiemDo TinhChatDiemDo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DiemDo_CongTo> DiemDo_CongTo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LogDiemDo> LogDiemDoes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SanLuong> SanLuongs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SanLuongThucTe> SanLuongThucTes { get; set; }
    }
}
