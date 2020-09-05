using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using DuAn.Models;
using DuAn.Models.CustomModel;
using System.Linq.Dynamic;
using DuAn.Attribute;
using System.Web;
using DuAn.Models.DbModel;
using System.IO;
using DuAn.COMMON;

namespace DuAn.Controllers
{
    [CheckLogin()]
    [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise })]
    public class AccountController : Controller
    {
        private Model1 db = new Model1();

        [AllowAnonymous]
        // GET: Account
        public ActionResult Login()
        {
            CongTy ct = CongTyDAO.getCongTyById(1);
            return View(ct);  
        }
        [AllowAnonymous]
        public JsonResult CheckLogin(string username, string password)
        {
            try
            {
                var rs = AccountDAO.CheckLogin(username, password);
                if (rs != null)
                {
                    rs.RoleAccount = db.RoleAccounts.Find(rs.RoleID);
                    Session["User"] = rs;
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            return Json("Fail", JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            Session.Remove("User");
            return RedirectToAction("Login");
        }
        [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Accounts })]
        public ActionResult ListUser()
        {
            return PartialView();
        }

        [HttpPost]
        [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Accounts })]
        public ActionResult TableDataUser()
        {
            return PartialView();
        }
        [HttpPost]
        [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Accounts_Create })]
        public ActionResult CreateAccountForm()
        {
            return PartialView("EditAccountForm", new AccountDetail() { DOB = DateTime.Now });
        }

        [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Accounts_Edit })]
        public ActionResult EditAccountForm(int accID)
        {
            Account acc = (Account)db.Accounts.Find(accID);
            if (acc == null)
            {
                return null ;
            }
            AccountDetail acs = new AccountDetail
            {
                ID = acc.ID,
                Username = acc.Username,
                Password = acc.Password,
                Fullname = acc.Fullname,
                Phone = acc.Phone,
                Email = acc.Email,
                Role = acc.RoleAccount.Role,
                Avatar = acc.Avatar,
                Address = acc.Address,
                IdentifyCode = acc.IdentifyCode,
                DOB = acc.DOB
            };
            return PartialView(acs);
        }

        [HttpPost]
        [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Accounts })]
        public JsonResult GetListAccountShort()
        {
            try
            {
                int length = int.Parse(HttpContext.Request["length"]);
                int start = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(int.Parse(Request["start"]) / length))) + 1;
                string searchValue = HttpContext.Request["search[value]"];
                string sortColumnName = HttpContext.Request["columns[" + Request["order[0][column]"] + "][name]"];
                string searchRoleValue = HttpContext.Request["columns[5][search][value]"];// search theo role
                string sortDirection = Request["order[0][dir]"];

                AccountPaging apg = new AccountPaging();
                apg.data = new List<AccountShort>();
                start = (start - 1) * length;
                List<Account> listAccount = db.Accounts.ToList<Account>();
                apg.recordsTotal = listAccount.Count;
                //filter
                // search theo Role
                if (!string.IsNullOrEmpty(searchRoleValue))
                {
                    listAccount = listAccount.Where(x
                        => x.RoleAccount.Role.ToLower().Equals(searchRoleValue.ToLower())
                    ).ToList<Account>();
                }
                // search total
                if (!string.IsNullOrEmpty(searchValue))
                {
                    listAccount = listAccount.Where(x => x.Username.ToLower().Contains(searchValue.ToLower()) ||
                        x.Email.ToLower().Contains(searchValue.ToLower()) ||
                        x.Fullname.ToLower().Contains(searchValue.ToLower()) ||
                        x.Phone.ToLower().Contains(searchValue.ToLower()) ||
                        x.RoleAccount.Role.ToLower().Contains(searchValue.ToLower())
                    ).ToList<Account>();
                }
                //sorting
                if (sortColumnName.Equals("Role"))
                {
                    sortColumnName = "RoleID";
                }
                listAccount = listAccount.OrderBy(sortColumnName + " " + sortDirection).ToList<Account>();
                //}
                apg.recordsFiltered = listAccount.Count;
                //paging
                listAccount = listAccount.Skip(start).Take(length).ToList<Account>();
                Dictionary<string, string> Role_Color = StatusContext.GetColorForRole();
                int count = StatusContext.Color.Count;
                for (int i = 0; i < listAccount.Count; i++)
                {
                    AccountShort acs = new AccountShort
                    {
                        ID = listAccount[i].ID,
                        Username = listAccount[i].Username,
                        Fullname = listAccount[i].Fullname,
                        Phone = listAccount[i].Phone,
                        Email = listAccount[i].Email,
                        Role = listAccount[i].RoleAccount.Role,
                        RoleColorClass = Role_Color[listAccount[i].RoleAccount.Role],
                        Avatar = listAccount[i].Avatar,
                        Actions = ""
                    };
                    apg.data.Add(acs);
                }
                apg.draw = int.Parse(Request["draw"]);
                return Json(apg);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpPost]
        [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Accounts_Edit })]
        public string UpdateAccount(int id, HttpPostedFileBase avatar, string password, string username, string fullname, string email, string address, string phone, string icode, string dob, int roleID)
        {
            try
            {
                if (avatar != null)
                {
                    string fileName = System.IO.Path.GetFileName(avatar.FileName);
                    string path_avatar = System.IO.Path.Combine(Server.MapPath("~/images/avatarAccount"), fileName);
                    // file is uploaded
                    avatar.SaveAs(path_avatar);
                }

                Account acc = new Account()
                {
                    ID = id,
                    Password = password,
                    Avatar = (avatar != null ? avatar.FileName : ""),
                    Fullname = fullname,
                    Email = email,
                    Address = address,
                    Phone = phone,
                    IdentifyCode = icode,
                    DOB = DateTime.ParseExact(dob, "dd - MMMM - yyyy", null),
                    RoleID = roleID
                };
                AccountDAO.UpdateAccout(acc);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "success";
        }
        [HttpPost]
        public JsonResult CheckUsername(string username)
        {
            if (!AccountDAO.CheckUsername(username))
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Accounts_Create })]
        public string InsertAccount(int id, HttpPostedFileBase avatar, string password, string username, string fullname, string email, string address, string phone, string icode, string dob, int roleID)
        {
            try
            {
                if (avatar != null)
                {
                    string fileName = System.IO.Path.GetFileName(avatar.FileName);
                    string path_avatar = System.IO.Path.Combine(Server.MapPath("~/images/avatarAccount"), fileName);
                    // file is uploaded
                    avatar.SaveAs(path_avatar);
                }
                Account acc = new Account()
                {
                    Username = username,
                    Password = password,
                    Avatar = (avatar != null ? avatar.FileName : "default.png"),
                    Fullname = fullname,
                    Email = email,
                    Address = address,
                    Phone = phone,
                    IdentifyCode = icode,
                    DOB = DateTime.ParseExact(dob, "dd - MMMM - yyyy", null),
                    RoleID = roleID
                };
                AccountDAO.AddAccount(acc);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "success";
        }
        [HttpPost]
        [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Accounts_Delete })]
        public bool DeleteAccount(int AccID)
        {
            try
            {
                db.Accounts.Remove(db.Accounts.Find(AccID));
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
