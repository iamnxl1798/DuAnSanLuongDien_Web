using DuAn.Models.DbModel;
using System.Collections.Generic;


namespace DuAn.Models.CustomModel
{
    public static class RoleChecking
    {
        public static List<int> CheckRole(List<int> listRole, int id)
        {
            using (Model1 db = new Model1())
            {
                foreach (var i in db.Permissions)
                {
                    if (i.Parent != "#")
                    {
                        if (int.Parse(i.Parent) == id)
                        {
                            listRole.Add(i.ID);
                            listRole = AddListToList(listRole, CheckRole(listRole, i.ID));
                        }
                    }
                }
            }
            return listRole;
        }

        public static List<int> AddListToList(List<int> a, List<int> b)
        {
            foreach(var i in b)
            {
                if (!a.Contains(i))
                {
                    a.Add(i);
                }
            }
            return a;
        }
    }
}