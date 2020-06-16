﻿using DuAn.Attribute;
using DuAn.Models.CustomModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuAn.Controllers
{
    public class HomeController : Controller
    {
        [CheckRoleAttribute(RoleID = new int[2] { 2, 3 })]
        public ActionResult Index()
        {
            var data = DBContext.getDuKien();
            return View(data);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}