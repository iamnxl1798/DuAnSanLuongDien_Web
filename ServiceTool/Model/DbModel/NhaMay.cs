namespace ServiceTool.Model.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NhaMay")]
    public partial class NhaMay
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NhaMay()
        {
            DiemDoes = new HashSet<DiemDo>();
        }

        public int ID { get; set; }

        [Required]
        public string TenNhaMay { get; set; }

        [Required]
        [StringLength(50)]
        public string TenVietTat { get; set; }

        [Required]
        public string DiaChi { get; set; }

        public int CongTyID { get; set; }

        public virtual CongTy CongTy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DiemDo> DiemDoes { get; set; }
    }
}
