using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using DuAn.Attribute;
using DuAn.Models;
using static DuAn.Models.Model1;

namespace DuAn.Controllers
{
    [CheckRole(RoleID = new int[1] { 2 })]
    public class AccountController : Controller
    {
        private Model1 db = new Model1();

        [AllowAnonymous]
        // GET: Account
        public ActionResult Login()
        {
            /*for (int i = 1; i < 10; i++)
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
                    DOB = DateTime.Parse("0" + i + "/0" + i + "/200" + i),
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
        [HttpPost]
        public JsonResult TableDataUser()
        {
            var rs = (from acc in db.Accounts
                     select new
                     {
                         ID = acc.ID,
                         Username = acc.Username,
                         Fullname = acc.Fullname,
                         Phone = acc.Phone,
                         Email = acc.Email,
                         Role = acc.RoleAccount.Role,
                         Status = 1,
                         Type = 2,
                         Actions = 1
                     }).ToList();
            /*Dictionary<String, object> json = new Dictionary<String, object>();
            json.Add("data", rs);*/
            return Json(rs);
        }
    }
}
