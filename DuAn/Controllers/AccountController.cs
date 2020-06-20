using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using DuAn.Models;
using DuAn.Models.CustomModel;
using static DuAn.Models.Model1;
using System.Linq.Dynamic;

namespace DuAn.Controllers
{
    //[CheckRole(RoleID = new int[1] { 2 })]
    public class AccountController : Controller
    {
        private Model1 db = new Model1();

        [AllowAnonymous]
        // GET: Account
        public ActionResult Login()
        {
            /*for (int i = 13; i < 100; i++)
            {
                Random rd = new Random();
                Account acc = new Account
                {
                    Username = "test_" + i,
                    SaltPassword = "1234567",
                    Password = AccountDAO.MaHoaMatKhau("123"),
                    Fullname = "test_" + i,
                    Phone = rd.Next(999).ToString(),
                    Address = "123456",
                    IdentifyCode = rd.Next(999).ToString(),
                    Email = rd.Next(999).ToString() + "@gmail.com",
                    DOB = DateTime.Parse("0" + (rd.Next(8)+1) + "/0" + (rd.Next(8) + 1) + "/200" + (rd.Next(8) + 1)),
                    RoleID = rd.Next(4)
                };
                AccountDAO.AddAccount(acc);
            }*/
            return View();
        }
        [AllowAnonymous]
        public JsonResult CheckLogin(string username, string password)
        {
            try
            {
                var rs = AccountDAO.CheckLogin(username, password);

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
        public ActionResult EditAccount()
        {
            int accID = int.Parse(HttpContext.Request["accID"]);
            Account acc = (Account)db.Accounts.Find(accID);
            AccountShort acs = new AccountShort
            {
                ID = acc.ID,
                Username = acc.Username,
                Fullname = acc.Fullname,
                Phone = acc.Phone,
                Email = acc.Email,
                Role = acc.RoleAccount.Role,
                Avatar = acc.Avatar,
                Actions = ""
            };
            return PartialView(acs);
        }
        [HttpPost]
        public JsonResult GetDetailAccount()
        {
            int length = int.Parse(HttpContext.Request["length"]);
            int start = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(int.Parse(Request["start"])/length))) + 1;
            string searchValue = HttpContext.Request["search[value]"];
            string sortColumnName = HttpContext.Request["columns["+Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            AccountPaging apg = new AccountPaging();
            apg.data = new List<AccountShort>();
            start = (start - 1) * length;
            List<Account> listAccount = db.Accounts.ToList<Account>();
            apg.recordsTotal = listAccount.Count;
            //filter
            if (!string.IsNullOrEmpty(searchValue))
            {
                listAccount = listAccount.Where(x => x.Username.ToLower().Contains(searchValue.ToLower())).ToList<Account>();
            }
            //sorting
            if (sortColumnName.Equals("Role"))
            {
                //sort UTF 8
            }
            else
            {
                listAccount = listAccount.OrderBy(sortColumnName + " " + sortDirection).ToList<Account>();
            }
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
            return Json(apg, JsonRequestBehavior.AllowGet);
        }
    }
}
