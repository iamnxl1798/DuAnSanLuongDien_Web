namespace DuAn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Permission")]
    public partial class Permission
    {
        [Key]
        public int ID { get; set; }

        public string Text { get; set; }

        public string Parent { get; set; }
    }
}
