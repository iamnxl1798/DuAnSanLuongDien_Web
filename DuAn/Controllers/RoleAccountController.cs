using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Dynamic;
using DuAn.Models;
using DuAn.Models.CustomModel;
using Abp.Extensions;
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
            try
            {
                ViewBag.RoleAccEdit = role;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return PartialView(db.RoleAccounts.ToList());
        }
        [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Roles })]
        public ActionResult TableDataRole()
        {
            return PartialView();
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
                List<RoleAccount> listRole = db.RoleAccounts.ToList<RoleAccount>();
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
                    apg.data.Add(new RoleModel(i));
                }

                apg.draw = int.Parse(Request["draw"]);
                return Json(apg);
                /*return Json(apg, JsonRequestBehavior.AllowGet);*/
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Roles })]
        public ActionResult ListRole()
        {
            return PartialView();
        }

        [HttpPost]
        [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Roles_Create })]
        public ActionResult CreateRoleForm()
        {
                return PartialView("EditRoleForm",new RoleAccount());
        }

        [HttpPost]
        [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Roles_Edit })]
        public ActionResult EditRoleForm(int RoleID)
        {
            var rs = db.RoleAccounts.Find(RoleID);
            if (rs == null) return null;
            return PartialView(rs);
        }

        [HttpPost]
        [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Roles })]
        public JsonResult GetPermissionTree(int RoleID)
        {

            try
            {
                RoleAccount ra = db.RoleAccounts.Find(RoleID);
                List<TreeViewNode> ls = new List<TreeViewNode>();
                foreach (var i in db.Permissions)
                {
                    TreeViewNode tvn = new TreeViewNode
                    {
                        id = i.ID.ToString(),
                        parent = i.Parent,
                        text = i.Text,
                        state = new Dictionary<string, bool>()
                    };

                    if (ra != null && RoleID != 0 && ra.PermissionID != null && ra.PermissionID.Split(',').Contains(tvn.id.ToString()))
                    {
                        tvn.state.Add("selected", true);
                    }
                    ls.Add(tvn);
                }

                return Json(ls);
            }
            catch (Exception ex)
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
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Roles_Edit })]
        public string UpdateRole(int RoleID, string RoleName, List<String> listPermissionID)
        {
            try
            {
                if (listPermissionID == null)
                {
                    listPermissionID = new List<string>();
                }
                RoleAccount ra = new RoleAccount()
                {
                    ID = RoleID,
                    Role = RoleName,
                    PermissionID = string.Join(",", listPermissionID)
                };
                /*if (rs == null)
                {
                    return "Role không tồn tại !!!";
                }*/
                RoleAccountDAO.UpdateRole(ra);

            }
            catch (Exception ex)
            {
                return "Update Role không thành công !!!";
            }
            return "success";
        }
        [HttpPost]
        [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Roles_Create })]
        public string InsertRole(int RoleID, string RoleName, List<String> listPermissionID)
        {
            try
            {
                if (listPermissionID == null)
                {
                    listPermissionID = new List<string>();
                }
                var rs = new RoleAccount()
                {
                    Role = RoleName,
                    PermissionID = string.Join(",", listPermissionID)
                };
                RoleAccountDAO.InsertRole(rs);

            }
            catch (Exception ex)
            {
                return "Insert Role không thành công !!!";
            }
            return "success";
        }
        [HttpPost]
        [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Roles_Delete })]
        public bool DeleteRole(int RoleID)
        {
            try
            {
                RoleAccountDAO.Delete(RoleID);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }

}
