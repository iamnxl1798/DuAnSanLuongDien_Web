using DuAn.Attribute;
using DuAn.Models.CustomModel;
using DuAn.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace DuAn.Controllers
{
   [CheckLogin(/*RoleID = new int[1] { 2 }*/)]
   public class HomeController : Controller
   {
      public ActionResult Index(string dateStr = "")
      {
         return View();
      }
      [HttpGet]
      public ActionResult CongSuatMaxMinPartialView()
      {
         var listDiemDo = DiemDoDAO.getAllDiemDo();
         ViewBag.listDiemDo = listDiemDo;
         return PartialView();
      }
      [HttpPost]
      public ActionResult CongSuatMaxMinDatatable(int idDiemDo = -1, string date = "")
      {
         try
         {
            using (var db = new Model1())
            {
               List<CongSuatMaxMinModelView> apg = new List<CongSuatMaxMinModelView>();
               apg = new List<CongSuatMaxMinModelView>();

               DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);

               var list = db.DiemDoes.Where(x => (idDiemDo == -1 || x.ID == idDiemDo));

               foreach (var item_dd in list)
               {
                  foreach (var item_k in db.Kenhs)
                  {
                     var list_sanluong = db.SanLuongs.Where(x => x.DiemDoID == item_dd.ID && x.Ngay == dt && x.KenhID == item_k.ID).ToList();
                     if (list_sanluong == null || list_sanluong.Count == 0)
                     {
                        continue;
                     }

                     int ckmax = 1;
                     int ckmin = 1;
                     double slmax = list_sanluong.Where(x => x.ChuKy == 1).Select(x => x.GiaTri).FirstOrDefault();
                     double slmin = list_sanluong.Where(x => x.ChuKy == 1).Select(x => x.GiaTri).FirstOrDefault();

                     foreach (var item_2 in list_sanluong)
                     {
                        if (item_2.GiaTri > slmax)
                        {
                           slmax = item_2.GiaTri;
                           ckmax = item_2.ChuKy;
                        }

                        if (item_2.GiaTri < slmin)
                        {
                           slmin = item_2.GiaTri;
                           ckmin = item_2.ChuKy;
                        }
                     }
                     CongSuatMaxMinModelView csmm = new CongSuatMaxMinModelView();
                     csmm.TenDiemDo = item_dd.TenDiemDo;
                     csmm.Kenh = item_k.Ten.ToString();
                     csmm.ChuKyMax = ckmax;
                     csmm.GiaTriMax = slmax;
                     csmm.ChuKyMin = ckmin;
                     csmm.GiaTriMin = slmin;

                     apg.Add(csmm);
                  }
               }
               if (apg.Count == 0)
               {
                  return PartialView(null);
               }
               return PartialView(apg);
            }
         }
         catch (Exception ex)
         {
            return PartialView(null);
         }
      }
      public ActionResult getModelDetail(int id, string date)
      {
         DateTime dateObject = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
         var result = DBContext.getChiTietDiemDo(id, dateObject);
         return PartialView(result);
      }
      public ActionResult getDoanhThuNgay(string date)
      {
         DateTime dateObject = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
         var result = DBContext.getDoanhThuNgayDetail(dateObject);
         if (result == null || result.sanLuongTrongNgay.Count == 0)
         {
            return null;
         }
         return PartialView(result);
      }
      public ActionResult getDoanhThuThang(string date)
      {
         DateTime dateObject = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
         var result = DBContext.getDoanhThuThang(dateObject);
         if (result == null || result.Count == 0)
         {
            return null;
         }
         return PartialView(result);
      }
      public ActionResult getDoanhThuNam(string date)
      {
         DateTime dateObject = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
         var result = DBContext.getDoanhThuNam(dateObject);
         if (result == null || result.Count == 0)
         {
            return null;
         }
         return PartialView(result);
      }
      public ActionResult getChiTietThangModal(string date)
      {
         DateTime dateObject = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
         var result = DBContext.getChiTietThang(dateObject);
         if (result == null || result.Count == 0)
         {
            return null;
         }
         return PartialView(result);
      }
      public ActionResult getChiTietNamModal(string date)
      {
         DateTime dateObject = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
         var result = DBContext.getChiTietNam(dateObject);
         if (result == null || result.Count == 0)
         {
            return null;
         }
         return PartialView(result);
      }
      public ActionResult getModalThongSo(string date = "", string id = "")
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
         DateTime dateObject = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
         var result = DBContext.exportExcel(dateObject);
         if (result == null)
         {
            return Json("Thiếu file báo cáo mẫu !!!", JsonRequestBehavior.AllowGet);
         }
         return result;
      }

      public ActionResult homePagePartialView(string dateStr = "")
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

      public string CheckPassword(string pass, string newPass)
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
         return result != null ? "success" : "";
      }
      public ActionResult ProfilePartial()
      {
         Account acc = (Account)Session["User"];
         return PartialView(acc);
      }

      /*public ActionResult layoutUser()
      {
          Account acc = (Account)Session["User"];
          return PartialView(acc);
      }*/
   }
}