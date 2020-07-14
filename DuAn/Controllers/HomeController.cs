using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using DuAn.Models.DbModel;

namespace DuAn.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string dateStr="")
        {
            return View();
        }
        public ActionResult getModelDetail(int id, string date)
        {
            DateTime dateObject= DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var result = DBContext.getChiTietDiemDo(id,dateObject);
            return PartialView(result);
        }
        
        public ActionResult getModalThongSo(string date="",string id="")
        {
            DateTime dateObject = DateTime.MinValue;
            if (date.Length > 0)
            {
                dateObject = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            var thongso = DBContext.getThongSoVanHanh(id, dateObject);
            return PartialView(thongso);
        }

        public ActionResult exportExcel(string date)
        {
            DateTime dateObject= DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var result= DBContext.exportExcel(dateObject);
            return result;
        }

        public ActionResult homePagePartialView(string dateStr="")
        {
            DateTime date = DateTime.Now.AddDays(-1);
            if (dateStr != "")
            {
                date = DateTime.ParseExact(dateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            var data = DBContext.getDuKien(date.Date).Result;
            return PartialView(data);
        }

        public ActionResult ChangeInfoPartial()
        {
            Account acc = (Account)Session["User"];
            return PartialView(acc);
        }

        public ActionResult ChangePasswordPartial()
        {
            Account acc = (Account)Session["User"];
            return PartialView(acc);
        }

        public string CheckPassword(string pass,string newPass)
        {
            Account acc = (Account)Session["User"];
            return AccountDAO.ChangePassword(acc, pass, newPass);
        }

        public ActionResult Profile()
        {
            Account acc = (Account)Session["User"];
            return View(acc);
        }

        public string ChangeInfo(HttpPostedFileBase avatar, string fullname, string email, string address, string phone, string dob, string id)
        {
            Account result = DBContext.ChangeInfo(avatar, fullname, email, address, phone, dob, id);
            Session["User"] = result;
            return result!=null?"success":"";
        }
    }
}