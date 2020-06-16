using DuAn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuAn.Attribute
{
    public class CheckRoleAttribute : AuthorizeAttribute
    {
        public int[] RoleID { get; set; }

        // call hàm này để check xem có được phép truy cập
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var account = (Account)HttpContext.Current.Session["User"];
            if (account == null) return false;
            if (!RoleID.Contains(account.RoleID))
            {
                return false;
            }
            return true;
        }

        //nếu không được phép truy cập sẽ call hàm này
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            HttpContext.Current.Session.Remove("User");
            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/RoleDenied.cshtml"
            };
        }
    }
}