using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DuAn.Models.Authorization
{
    public static class WebPermissions
    {
        public const string Pages = "Pages";


        public const string Pages_Admin = "Pages.Administration";
        public const string Pages_Admin_UpdateData = "Pages.Administration.UpdateData";
        public const string Pages_Admin_UpdateRecipe = "Pages.Administration.UpdateRecipe";


        public const string Pages_Expert = "Pages.Expertise";

        public const string Pages_Expert_Accounts = "Pages.Expertise.Accounts";
        public const string Pages_Expert_Accounts_Edit = "Pages.Expertise.Accounts.Edit";
        public const string Pages_Expert_Accounts_Create = "Pages.Expertise.Accounts.Create";
        public const string Pages_Expert_Accounts_Delete = "Pages.Expertise.Accounts.Delete";
        public const string Pages_Expert_Accounts_ChangePermissions = "Pages.Expertise.Accounts.ChangePermissions";

        public const string Pages_Expert_Roles = "Pages.Expertise.Roles";
        public const string Pages_Expert_Roles_Create = "Pages.Expertise.Roles.Create";
        public const string Pages_Expert_Roles_Edit = "Pages.Expertise.Roles.Edit";
        public const string Pages_Expert_Roles_Delete = "Pages.Expertise.Roles.Delete";


        public const string Pages_Leader = "Pages.Leading";
        public const string Pages_Leader_ExportingData = "Pages.Leading.ExportingData";
    }
}