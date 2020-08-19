using Abp.Authorization;
using Abp.Localization;
using DuAn.Models.Authorization;
using System.Web.UI;

namespace DuAn.Authorization
{
    public class WebAuthorizationProvider : AuthorizationProvider
    {
        /*private readonly bool _isMultiTenancyEnabled;

        public AppAuthorizationProvider(bool isMultiTenancyEnabled)
        {
            _isMultiTenancyEnabled = isMultiTenancyEnabled;
        }

        public AppAuthorizationProvider(IMultiTenancyConfig multiTenancyConfig)
        {
            _isMultiTenancyEnabled = multiTenancyConfig.IsEnabled;
        }*/

        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull(WebPermissions.Pages) ?? context.CreatePermission(WebPermissions.Pages, L("Pages"));

            var admins = pages.CreateChildPermission(WebPermissions.Pages_Admin, L("Administration"));
            admins.CreateChildPermission(WebPermissions.Pages_Admin_UpdateData, L("UpdateData"));
            admins.CreateChildPermission(WebPermissions.Pages_Admin_UpdateRecipe, L("UpdateRecipe"));

            var experts = pages.CreateChildPermission(WebPermissions.Pages_Expert, L("Expertise"));

            var experts_accounts = experts.CreateChildPermission(WebPermissions.Pages_Expert_Accounts, L("Accouts"));
            experts_accounts.CreateChildPermission(WebPermissions.Pages_Expert_Accounts_Create, L("CreateAccount"));
            experts_accounts.CreateChildPermission(WebPermissions.Pages_Expert_Accounts_Delete, L("DeleteAccount"));
            experts_accounts.CreateChildPermission(WebPermissions.Pages_Expert_Accounts_Edit, L("EditAccount"));
            experts_accounts.CreateChildPermission(WebPermissions.Pages_Expert_Accounts_ChangePermissions, L("ChangePermissions"));

            var experts_roles = experts.CreateChildPermission(WebPermissions.Pages_Expert_Roles, L("Roles"));
            experts_roles.CreateChildPermission(WebPermissions.Pages_Expert_Roles_Create, L("CreateRole"));
            experts_roles.CreateChildPermission(WebPermissions.Pages_Expert_Roles_Delete, L("DeleteRole"));
            experts_roles.CreateChildPermission(WebPermissions.Pages_Expert_Roles_Edit, L("EditRole"));


            var leaders = pages.CreateChildPermission(WebPermissions.Pages_Leader, L("Leading"));
            leaders.CreateChildPermission(WebPermissions.Pages_Leader_ExportingData, L("ExportingData"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, /*AbpZeroTemplateConsts.LocalizationSourceName*/ name);
        }
    }
}