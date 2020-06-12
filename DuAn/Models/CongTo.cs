namespace DuAn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CongTo")]
    public partial class CongTo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CongTo()
        {
            DiemDo_CongTo = new HashSet<DiemDo_CongTo>();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(10)]
        public string Serial { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DiemDo_CongTo> DiemDo_CongTo { get; set; }
    }
}
