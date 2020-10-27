using DuAn.Models.CustomModel;
using DuAn.Models.DbModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace DuAn
{
   public class MvcApplication : System.Web.HttpApplication
   {
      protected void Application_Start()
      {
         AreaRegistration.RegisterAllAreas();
         FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
         RouteConfig.RegisterRoutes(RouteTable.Routes);
         BundleConfig.RegisterBundles(BundleTable.Bundles);
      }

      protected void Session_Start(object sender, EventArgs e)
      {
         Session.Timeout = 86400;
      }

      protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
      {
         HttpCookie authCookie = Request.Cookies["login_form_cre"];
         if (authCookie != null)
         {
            try
            {
               FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

               var serializeModel = JsonConvert.DeserializeObject<CustomSerializeModel>(authTicket.UserData);

               CustomPrincipal principal = new CustomPrincipal(authTicket.Name);

               principal.UserId = serializeModel.UserId;
               principal.FullName = serializeModel.FullName;
               principal.Email = serializeModel.Email;

               HttpContext.Current.User = principal;
            }
            catch
            {
               FormsAuthentication.SignOut();
            }
         }
      }
   }
}
