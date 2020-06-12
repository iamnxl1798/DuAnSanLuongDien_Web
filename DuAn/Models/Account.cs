namespace DuAn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Account")]
    public partial class Account
    {
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string username { get; set; }

        [Required]
        public string password { get; set; }

        public int RoleID { get; set; }

        public virtual RoleAccount RoleAccount { get; set; }
    }
}
