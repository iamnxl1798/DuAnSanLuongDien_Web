using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Dynamic;
using DuAn.Models;
using DuAn.Models.CustomModel;
using DuAn.Models.DbModel;
using DuAn.Attribute;
using DuAn.COMMON;

namespace DuAn.Controllers
{
   [CheckLogin(/*RoleID = new int[1] { 2 }*/)]
   [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise })]
   public class RoleAccountController : Controller
   {
      private Model1 db = new Model1();

      [HttpPost]
      public ActionResult AllRole(string role)
      {
         List<AllRole> list = new List<AllRole>();
         try
         {
            Dictionary<string, string> role_Color = StatusContext.GetColorForRole();
            if (string.IsNullOrEmpty(role))
            {
               role = "Fullpower";
               //return null;
            }

            foreach (var i in this.db.RoleAccounts)
            {
               if (i.Role.ToLower().Equals(role.ToLower()))
               {
                  ViewBag.RoleIDAccEdit = i.ID;
                  ViewBag.RoleAccEdit = role;
                  ViewBag.RoleColorClass = role_Color[i.Role];
               }

               AllRole alr = new AllRole()
               {
                  ID = i.ID,
                  Role = i.Role,
                  ColorClass = role_Color[i.Role]
               };
               list.Add(alr);
            }
         }
         catch
         {
            return this.PartialView(null);
         }

         return this.PartialView(list);
      }

      [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Roles })]
      public ActionResult TableDataRole()
      {
         return this.PartialView();
      }

      [HttpPost]
      [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Roles })]
      public JsonResult GetAllRole()
      {
         try
         {
            /*db.Configuration.ProxyCreationEnabled = false;*/
            int length = int.Parse(HttpContext.Request["length"]);
            int start = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(int.Parse(Request["start"]) / length))) + 1;
            string searchValue = HttpContext.Request["search[value]"];
            string sortColumnName = HttpContext.Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            /*int length = 10;
            int start = 1;
            string searchValue = "";
            string sortColumnName = "ID";
            string sortDirection = "asc";*/
            RolePaging apg = new RolePaging();
            apg.data = new List<RoleModel>();
            start = (start - 1) * length;
            List<RoleAccount> listRole = this.db.RoleAccounts.ToList<RoleAccount>();
            Dictionary<string, string> role_Color = StatusContext.GetColorForRole();
            apg.recordsTotal = listRole.Count;
            //filter
            if (!string.IsNullOrEmpty(searchValue))
            {
               listRole = listRole.Where(x => x.Role.ToLower().Contains(searchValue.ToLower())).ToList<RoleAccount>();
            }
            //sorting
            if (sortColumnName.Equals("Role"))
            {
               //sort UTF 8
               sortColumnName = "ID";
            }

            listRole = listRole.OrderBy(sortColumnName + " " + sortDirection).ToList<RoleAccount>();

            apg.recordsFiltered = listRole.Count;
            //paging
            listRole = listRole.Skip(start).Take(length).ToList<RoleAccount>();

            foreach (var i in listRole)
            {
               apg.data.Add(new RoleModel(i, role_Color[i.Role]));
            }

            apg.draw = int.Parse(this.Request["draw"]);
            return this.Json(apg);
            /*return Json(apg, JsonRequestBehavior.AllowGet);*/
         }
         catch
         {
            return null;
         }
      }

      [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Roles })]
      public ActionResult ListRole()
      {
         return this.PartialView();
      }

      [HttpPost]
      [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Roles_Create })]
      public ActionResult CreateRoleForm()
      {
         return this.PartialView("EditRoleForm", new RoleAccount());
      }

      [HttpPost]
      [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Roles_Edit })]
      public ActionResult EditRoleForm(int roleID)
      {
         var rs = this.db.RoleAccounts.Find(roleID);
         if (rs == null)
         {
            return null;
         }

         return this.PartialView(rs);
      }

      [HttpPost]
      [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise })]
      public JsonResult GetPermissionTree(int roleID)
      {
         try
         {
            RoleAccount ra = this.db.RoleAccounts.Find(roleID);
            List<TreeViewNode> ls = new List<TreeViewNode>();
            foreach (var i in this.db.Permissions)
            {
               TreeViewNode tvn = new TreeViewNode
               {
                  id = i.ID.ToString(),
                  parent = i.Parent,
                  text = i.Description,
                  state = new Dictionary<string, bool>()
               };

               if (ra != null && roleID != 0 && ra.PermissionID != null && ra.PermissionID.Split(',').Contains(tvn.id.ToString()))
               {
                  tvn.state.Add("selected", true);
               }

               ls.Add(tvn);
            }

            return this.Json(ls);
         }
         catch
         {
            return null;
         }
      }

      [HttpPost]
      [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Roles })]
      public JsonResult CheckRolename(string rolename)
      {
         if (!RoleAccountDAO.checkRoleName(rolename))
         {
            return this.Json("Fail", JsonRequestBehavior.AllowGet);
         }

         return this.Json("Success", JsonRequestBehavior.AllowGet);
      }

      [HttpPost]
      [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Roles_Edit })]
      public string UpdateRole(int roleID, string roleName, List<string> listPermissionID)
      {
         try
         {
            if (listPermissionID == null)
            {
               listPermissionID = new List<string>();
            }

            RoleAccount ra = new RoleAccount()
            {
               ID = roleID,
               Role = roleName,
               PermissionID = string.Join(",", listPermissionID)
            };
            /*if (rs == null)
            {
                return "Role không tồn tại !!!";
            }*/
            RoleAccountDAO.UpdateRole(ra);
         }
         catch
         {
            return "Update Role không thành công !!!";
         }

         return "success";
      }

      [HttpPost]
      [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Roles_Create })]
      public string InsertRole(int roleID, string roleName, List<string> listPermissionID)
      {
         try
         {
            if (listPermissionID == null)
            {
               listPermissionID = new List<string>();
            }

            var rs = new RoleAccount()
            {
               Role = roleName,
               PermissionID = string.Join(",", listPermissionID)
            };
            RoleAccountDAO.InsertRole(rs);
         }
         catch
         {
            return "Insert Role không thành công !!!";
         }

         return "success";
      }

      [HttpPost]
      [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Roles_Delete })]
      public bool DeleteRole(int roleID)
      {
         try
         {
            RoleAccountDAO.Delete(roleID);
         }
         catch
         {
            return false;
         }

         return true;
      }
   }
}
