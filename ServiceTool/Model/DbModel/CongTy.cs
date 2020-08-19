namespace ServiceTool.Model.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CongTy")]
    public partial class CongTy
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CongTy()
        {
            NhaMays = new HashSet<NhaMay>();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string TenCongTy { get; set; }

        [Required]
        [StringLength(10)]
        public string TenVietTat { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NhaMay> NhaMays { get; set; }
    }
}
