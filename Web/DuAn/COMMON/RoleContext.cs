using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuAn.COMMON
{
    public class RoleContext
    {
        public const int Expertise = 3;
        public const int Administration = 2;
        public const int Leading = 4;
        public const int Administration_UpdateFile = 5;
        public const int Administration_UpdateRecipe = 6;
        public const int Administration_ChangeConfig = 17;
        public const int Expertise_Accounts = 7;
        public const int Expertise_Accounts_Create = 9;
        public const int Expertise_Accounts_Edit = 11;
        public const int Expertise_Accounts_Delete = 10;
        public const int Expertise_Accounts_ChangePermissions = 12;
        public const int Expertise_Roles = 8;
        public const int Expertise_Roles_Create = 13;
        public const int Expertise_Roles_Edit = 15;
        public const int Expertise_Roles_Delete = 14;
        public const int Leading_Exporting_Data = 16;
    }
}