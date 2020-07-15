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
        public int RoleID { get; set; }

        // call hàm này để check xem có được phép truy cập
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            List<int> listRole = new List<int>();
            listRole.Add(RoleID);
            listRole = RoleChecking.CheckRole(listRole, RoleID);

            var account = (Account)HttpContext.Current.Session["User"];
            using (Model1 db = new Model1())
            {
                var listRoleAcc = db.RoleAccounts.Find(account.RoleID).PermissionID.Split(',');
                foreach(var i in listRoleAcc)
                {
                    foreach(var u in listRole)
                    {
                        if(int.Parse(i) == u)
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
            /*filterContext.Result = new RedirectToRouteResult
            (
                new RouteValueDictionary(new { controller = "Account", action = "AccessDenied" })
            );*/
            //filterContext.Result = new ViewResult { ViewName = "AccessDenied" };
            //filterContext.Result = new RedirectResult("/Account/AccessDenied");
            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}