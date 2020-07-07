using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using DuAn.Models;
using DuAn.Models.CustomModel;
using System.Linq.Dynamic;
using DuAn.Attribute;

namespace DuAn.Controllers
{
    /*[CheckRole(RoleID = new int[1] { 2 })]*/
    public class AccountController : Controller
    {
        private Model1 db = new Model1();

        [AllowAnonymous]
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        public JsonResult CheckLogin(string username, string password)
        {
            try
            {
                var rs = AccountDAO.CheckLogin(username, password);
                rs.RoleAccount = db.RoleAccounts.Find(rs.RoleID);

                if (rs != null)
                {
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

        public ActionResult ListUser()
        {
            return View();
        }
        public ActionResult TableDataUser()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult EditAccountForm(int accID)
        {
            Account acc = (Account)db.Accounts.Find(accID);
            if(acc == null || accID == 0)
            {
                return PartialView(new AccountDetail() { DOB = DateTime.Now});
            }
            AccountDetail acs = new AccountDetail
            {
                ID = acc.ID,
                Username = acc.Username,
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
        public JsonResult GetListAccountShort()
        {
            try
            {
                int length = int.Parse(HttpContext.Request["length"]);
                int start = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(int.Parse(Request["start"]) / length))) + 1;
                string searchValue = HttpContext.Request["search[value]"];
                string sortColumnName = HttpContext.Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];

                AccountPaging apg = new AccountPaging();
                apg.data = new List<AccountShort>();
                start = (start - 1) * length;
                List<Account> listAccount = db.Accounts.ToList<Account>();
                apg.recordsTotal = listAccount.Count;
                //filter
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
                    //sort UTF 8
                    sortColumnName = "RoleID";
                }
                listAccount = listAccount.OrderBy(sortColumnName + " " + sortDirection).ToList<Account>();
                //}
                apg.recordsFiltered = listAccount.Count;
                //paging
                listAccount = listAccount.Skip(start).Take(length).ToList<Account>();
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
        public string UpdateAccount(int id,string username, string fullname, string email, string address, string phone, string icode, string dob, int roleID)
        {
            try
            {
                Account acc = new Account()
                {
                    ID = id,
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
        [AllowAnonymous]
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
        public string InsertAccount(int id, string username, string fullname, string email, string address, string phone, string icode, string dob, int roleID)
        {
            try
            {
                if (!AccountDAO.CheckUsername(username))
                {
                    return "Username đã tồn tại !!!";
                }
                Account acc = new Account()
                {
                    Username = username,
                    //hard code password
                    Password = "123",
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
        public bool DeleteAccount(int AccID)
        {
            try
            {
                db.Accounts.Remove(db.Accounts.Find(AccID));
                db.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }
        
    }
}
