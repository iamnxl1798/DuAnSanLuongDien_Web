using DuAn.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace DuAn.Models.CustomModel
{
   public class CustomPrincipal : IPrincipal
   {
      #region Identity Properties

      public int UserId { get; set; }
      public string FullName { get; set; }
      public string Email { get; set; }
      #endregion

      public IIdentity Identity
      {
         get; private set;
      }

      public bool IsInRole(string role)
      {
         return true;
      }

      public CustomPrincipal(string username)
      {
         Identity = new GenericIdentity(username);
      }
   }
}