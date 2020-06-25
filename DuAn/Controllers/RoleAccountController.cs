using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Dynamic;
using DuAn.Models;
using DuAn.Models.CustomModel;

namespace DuAn.Controllers
{
    //[CheckRole(RoleID = new int[1] { 2 })]
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
        public ActionResult TableDataRole()
        {
            return PartialView();
        }
        [HttpPost]
        public JsonResult GetAllRole()
        {
            try
            {
                /*db.Configuration.ProxyCreationEnabled = false;*/
                int length = int.Parse(HttpContext.Request["length"]);
                int start = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(int.Parse(Request["start"]) / length))) + 1;
                /*string searchValue = HttpContext.Request["search[value]"];
                string sortColumnName = HttpContext.Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];*/

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
                /*//filter
                if (!string.IsNullOrEmpty(searchValue))
                {
                    listRole = listRole.Where(x => x.Role.ToLower().Contains(searchValue.ToLower())).ToList<RoleAccount>();
                }
                //sorting
                listRole = listRole.OrderBy(sortColumnName + " " + sortDirection).ToList<RoleAccount>();*/

                apg.recordsFiltered = listRole.Count;
                //paging
                listRole = listRole.Skip(start).Take(length).ToList<RoleAccount>();

                foreach(var i in listRole)
                {
                    apg.data.Add(new RoleModel(i));
                }

                apg.draw = int.Parse(Request["draw"]);
                apg.draw = 1;
                return Json(apg);
                /*return Json(apg, JsonRequestBehavior.AllowGet);*/
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ActionResult ListRole()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PermissionTree()
        {
            return PartialView();
        }
        [HttpPost]
        public JsonResult GetPermissionTree()
        {
            try
            {
                List<TreeViewNode> ls = new List<TreeViewNode>();
                foreach (var i in db.Permissions)
                {
                    ls.Add(new TreeViewNode
                    {
                        id = i.ID.ToString(),
                        parent = i.Parent,
                        text = i.Text
                    });
                }
                return Json(ls);
            }
            catch (Exception ex)
            {
                return null;
            }


        }
    }

}
