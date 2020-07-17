using DuAn.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuAn.COMMON
{
    public class StatusContext
    {
        public static List<string> Color = new List<string>
        {
            "label-light-success",
            "label-light-warning",
            "label-light-primary",            
            "label-light-info",           
            "label-light-danger"
            /*"label-light-secondary",*/
            
        };
        public static Dictionary<string, string> GetColorForRole()
        {
            Dictionary<string, string> Role_Account = new Dictionary<string, string>();
            int count = Color.Count;
            using (Model1 db = new Model1())
            {
                foreach(var i in db.RoleAccounts)
                {
                    Role_Account.Add(i.Role, Color[--count]);
                    if(count == 0)
                    {
                        count = Color.Count;
                    }
                }
            };
            return Role_Account;
        }
    }
}