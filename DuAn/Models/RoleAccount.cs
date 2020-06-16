namespace DuAn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RoleAccount")]
    public partial class RoleAccount
    {   
        public RoleAccount()
        {
            Accounts = new HashSet<Account>();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
    }
}
