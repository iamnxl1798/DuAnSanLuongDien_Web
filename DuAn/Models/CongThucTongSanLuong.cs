namespace DuAn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CongThucTongSanLuong")]
    public partial class CongThucTongSanLuong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CongThucTongSanLuong()
        {
            TongSanLuong_Ngay = new HashSet<TongSanLuong_Ngay>();
        }

        public int ID { get; set; }

        [StringLength(200)]
        public string Ten { get; set; }

        public string CongThuc { get; set; }

        [Column(TypeName = "date")]
        public DateTime ThoiGianKetThuc { get; set; }

        [Column(TypeName = "date")]
        public DateTime ThoiGianBatDau { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TongSanLuong_Ngay> TongSanLuong_Ngay { get; set; }
    }
}
