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
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public int RoleID { get; set; }
        public string SaltPassword { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string IdentifyCode { get; set; }
        public string Email { get; set; }
        public DateTime DOB { get; set; }
        public string Avatar { get; set; }

        public virtual RoleAccount RoleAccount { get; set; }
    }
}
