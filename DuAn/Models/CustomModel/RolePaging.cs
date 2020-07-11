using DuAn.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuAn.Models.CustomModel
{
    public class RolePaging
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<RoleModel> data { get; set; }
    }
    public class RoleModel
    {
        public int ID { get; set; }
        public string Role { get; set; }
        public string Actions { get; set; }
        public RoleModel(RoleAccount racc)
        {
            ID = racc.ID;
            Role = racc.Role;
            Actions = "";
        }
    }
}