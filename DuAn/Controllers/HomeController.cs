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
            DateTime date = DateTime.Now.AddDays(-1);
            if (dateStr != "")
            {
                date = DateTime.ParseExact(dateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            var data = DBContext.getDuKien(date.Date).Result;
            return View(data);
        }
        public ActionResult getModelDetail(int id)
        {
            var result = DBContext.getChiTietDiemDo(id);
            return PartialView(result);
        }

        public ActionResult exportExcel()
        {
            var result= DBContext.exportExcel();
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