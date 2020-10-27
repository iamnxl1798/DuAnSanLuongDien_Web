
using DuAn.Models.CustomModel;
using DuAn.Models.DbModel;
using Newtonsoft.Json;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace DuAn.Attribute
{
   public class CheckLoginAttribute : AuthorizeAttribute
   {

      // call hàm này để check xem có được phép truy cập
      protected override bool AuthorizeCore(HttpContextBase httpContext)
      {
         if (HttpContext.Current.User.Identity.IsAuthenticated || HttpContext.Current.Session["User"] != null)
         {
            if (HttpContext.Current.Session["User"] == null)
            {
               HttpCookie authCookie = HttpContext.Current.Request.Cookies["login_form_cre"];
               FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
               var serializeModel = JsonConvert.DeserializeObject<CustomSerializeModel>(authTicket.UserData);
               Account acc = AccountDAO.GetAccountByID(serializeModel.UserId);
               HttpContext.Current.Session["User"] = acc;
            }

            HttpContext.Current.Session["CongTy"] = CongTyDAO.getCongTyByDefault();
            HttpContext.Current.Session["NhaMay"] = NhaMayDAO.GetNhaMayByDefault();
            return true;
         }
         return false;
      }

      //nếu không được phép truy cập sẽ call hàm này
      protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
      {
         HttpContext.Current.Session.Remove("User");
         filterContext.Result = new RedirectToRouteResult
         (
             new RouteValueDictionary(new { controller = "Account", action = "Login" })
         );
      }
   }
}