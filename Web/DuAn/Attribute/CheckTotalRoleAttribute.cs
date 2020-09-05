
using DuAn.Models;
using DuAn.Models.CustomModel;
using DuAn.Models.DbModel;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DuAn.Attribute
{
    public class CheckTotalRoleAttribute : AuthorizeAttribute
    {
        public int[] RoleID { get; set; }

        // call hàm này để check xem có được phép truy cập
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            List<int> listRole = new List<int>();
            foreach (var i in RoleID.ToList())
            {
                listRole.Add(i);
                listRole = RoleChecking.CheckRole(listRole, i);
            }
            var account = (Account)HttpContext.Current.Session["User"];
            using (Model1 db = new Model1())
            {
                var listRoleAcc = db.RoleAccounts.Find(account.RoleID).PermissionID.Split(',');
                foreach (var i in listRoleAcc)
                {
                    foreach (var u in listRole)
                    {
                        if (!string.IsNullOrEmpty(i) && int.Parse(i) == u)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        //nếu không được phép truy cập sẽ call hàm này
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //filterContext.Result = new RedirectResult("/Account/AccessDenied");
            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}