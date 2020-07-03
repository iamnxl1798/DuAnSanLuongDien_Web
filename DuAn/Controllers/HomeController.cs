using DuAn.Attribute;
using DuAn.Models.CustomModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

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


        //[HttpGet]
        //public async Task<JsonResult> reloadPage(string dateStr = "")
        //{
        //    DateTime date = DateTime.Now.AddDays(-1);
        //    if (dateStr != "")
        //    {
        //        date = DateTime.ParseExact(dateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //    }
        //    var data = await DBContext.getDuKien(date.Date);
        //    return await Task.FromResult(Json(data, JsonRequestBehavior.AllowGet));
        //}
    }
}