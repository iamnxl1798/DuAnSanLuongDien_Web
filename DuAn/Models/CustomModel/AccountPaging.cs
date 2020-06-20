using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuAn.Models.CustomModel
{
    public class AccountPaging
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<AccountShort> data { get; set; }
    }
    public class AccountShort
    {
        public int ID { get; set; }
        public string Username { get; set; }

        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public string Role { get; set; }
        public string Actions { get; set; }
    }
}