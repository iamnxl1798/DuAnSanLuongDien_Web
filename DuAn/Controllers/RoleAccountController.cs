using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DuAn.Attribute;
using DuAn.Models;
using DuAn.Models.CustomModel;

namespace DuAn.Controllers
{
    //[CheckRole(RoleID = new int[1] { 2 })]
    public class RoleAccountController : Controller
    {
        private Model1 db = new Model1();

        [HttpPost]
        public ActionResult GetAllRole(string role)
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

    }
}
