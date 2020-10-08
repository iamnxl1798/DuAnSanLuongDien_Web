using DuAn.Attribute;
using DuAn.COMMON;
using DuAn.Models.CustomModel;
using DuAn.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Web.Services.Description;
using System.Globalization;

namespace DuAn.Controllers
{
   [CheckLogin(/*RoleID = new int[1] { 2 }*/)]
   [CheckTotalRole(RoleID = new int[2] { RoleContext.Expertise, RoleContext.Administration })]
   public class AdminController : Controller
   {
      /*public ActionResult getLogoCongTy()
      {
          string logo = CongTyDAO.getCongTyById(5).Logo;
          return Json(logo, JsonRequestBehavior.AllowGet);
      }*/
      // GET: Admin
      /*public ActionResult Index()
      {
          AdminModel item = DBContext.getDataAdminModel();
          return View(item);
      }*/
      public ActionResult CapNhatGiaDien()
      {
         return View();
      }
      [CheckTotalRole(RoleID = new int[1] { RoleContext.Administration_UpdateFile })]
      public ActionResult CauHinhSanLuong()
      {
         AdminModel item = DBContext.getDataAdminModel();
         return View(item);
      }

      [CheckTotalRole(RoleID = new int[1] { RoleContext.Administration_UpdateRecipe })]
      public ActionResult CauHinhCongThuc()
      {
         return View();
      }

      [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Roles })]
      public ActionResult QuanTriVaiTro()
      {
         return View();
      }

      [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Accounts })]
      public ActionResult QuanTriNguoiDung()
      {
         return View();
      }
      [CheckTotalRole(RoleID = new int[1] { RoleContext.Administration_ChangeConfig })]
      public ActionResult QuanTriCauHinh()
      {
         //id cong ty default
         CauHinhWebViewModel model = new CauHinhWebViewModel();
         CongTy ct = (CongTy)Session["CongTy"];
         NhaMay nm = (NhaMay)Session["NhaMay"];
         model.ct = ct;
         model.nm = nm;
         return View(model);
      }

      [CheckTotalRole(RoleID = new int[1] { RoleContext.Administration_UpdateFile })]
      public ActionResult viewMissingDataList(string fileName = "", string startDate = "", string endDate = "")
      {
         var data = DBContext.getMissingData(startDate, endDate, fileName);
         return PartialView(data.ToList());
      }

      [CheckTotalRole(RoleID = new int[1] { RoleContext.Administration_UpdateFile })]
      public ActionResult SaveDropzoneJsUploadedFiles()
      {
         try
         {
            foreach (string fileName in Request.Files)
            {
               HttpPostedFileBase file = Request.Files[fileName];
               if (file != null && file.ContentLength > 0)
               {
                  //var parentDir = new DirectoryInfo(Server.MapPath("\\")).Parent.Parent.FullName;
                  //var pathString = parentDir + "\\DocDuLieuCongTo\\TestTheoDoi";
                  var pathString = @"C:\SLCTO\ESMETERING";

                  var fileName1 = Path.GetFileName(file.FileName);

                  bool isExists = System.IO.Directory.Exists(pathString);

                  if (!isExists)
                     System.IO.Directory.CreateDirectory(pathString);

                  var path = string.Format("{0}\\{1}", pathString, file.FileName);
                  file.SaveAs(path);
               }
            }
         }
         catch (Exception ex)
         {
            return Json(new { Message = "Lỗi lưu file !!" });
         }
         return Json(new { Message = string.Empty });
      }

      [CheckTotalRole(RoleID = new int[1] { RoleContext.Administration_UpdateFile })]
      public ActionResult RemoveFile(string name)
      {
         try
         {
            foreach (string fileName in Request.Files)
            {
               HttpPostedFileBase file = Request.Files[fileName];
               if (file != null && file.ContentLength > 0)
               {
                  var parentDirRmv = new DirectoryInfo(Server.MapPath("\\")).Parent.Parent.FullName;
                  var pathString = parentDirRmv + "\\DocDuLieuCongTo\\TestTheoDoi";

                  var fileName1 = Path.GetFileName(file.FileName);

                  bool isExists = System.IO.Directory.Exists(pathString);

                  if (!isExists)
                     System.IO.Directory.CreateDirectory(pathString);

                  var path = string.Format("{0}\\{1}", pathString, file.FileName);
                  file.SaveAs(path);
               }
            }
            //var parentDir = new DirectoryInfo(Server.MapPath("\\")).Parent.Parent.FullName;
            //var folderPath = parentDir + "\\DocDuLieuCongTo\\TestTheoDoi";
            var folderPath = @"C:\SLCTO\ESMETERING";
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            FileInfo[] files = dir.GetFiles(name, SearchOption.TopDirectoryOnly);
            foreach (var item in files)
            {
               if (System.IO.File.Exists(Path.Combine(folderPath, name)))
               {
                  System.IO.File.Delete(Path.Combine(folderPath, name));
               }
            }
            return Json(new { Message = "Thanhcong" });
         }
         catch (Exception ex)
         {
            return Json(new { Message = "Xuly" });
         }
      }

      [CheckTotalRole(RoleID = new int[1] { RoleContext.Administration_UpdateRecipe })]
      public ActionResult UpdateFormula(string formula, string name, string thoiGian)
      {
         DBContext.updateFormula(formula, name, thoiGian);
         return Json(new { Message = "Cập nhật thành công" });
      }
      public ActionResult InsertGiaDien(string thoiGianBatDau, string thoiGianKetThuc, int giaDien)
      {
         var message = DBContext.InsertGiaDien(thoiGianBatDau, thoiGianKetThuc, giaDien);
         if (message)
         {
            return Json(new { success = true, message = string.Empty });
         }
         return Json(new { success = false, Message = "Không thể cập nhật giá điện" });
      }

      [CheckTotalRole(RoleID = new int[1] { RoleContext.Administration_UpdateFile })]
      public ActionResult MissingDataPartial()
      {
         var result = DBContext.getMissingData("", "");
         return PartialView(result);
      }

      [CheckTotalRole(RoleID = new int[1] { RoleContext.Administration_UpdateRecipe })]
      public ActionResult InsertFormula()
      {
         var result = DBContext.getDataAdminModel();
         return PartialView(result);
      }

      [CheckTotalRole(RoleID = new int[1] { RoleContext.Administration_ChangeConfig })]
      [HttpPost]
      public ActionResult ChangeCauHinhWeb(HttpPostedFileBase cauhinh_logo, string ten_congty, int id, string ten_viet_tat, string ten_viet_tat_nm, string ten_nha_may, int id_nm, string dia_chi_nm)
      {
         string fileName = "default_logo.png";
         try
         {
            if (cauhinh_logo != null)
            {
               //fileName = System.IO.Path.GetFileName(cauhinh_logo.FileName)+"/"+ id;
               fileName = "logoFactory_" + id + ".png";
               string path_avatar = System.IO.Path.Combine(Server.MapPath("~/images/logoFactory"), fileName);
               // file is uploaded
               cauhinh_logo.SaveAs(path_avatar);
            }

            CongTy ct = CongTyDAO.getCongTyById(id);
            if (ct == null)
            {
               return Json(new { success = false, message = "Không tìm thấy thông tin công ty" });
            }
            ct.Logo = fileName;
            ct.TenCongTy = ten_congty;
            ct.TenVietTat = ten_viet_tat;

            var rs = CongTyDAO.updateCongTyInformation(ct);

            NhaMay nm = NhaMayDAO.GetNhaMayById(id_nm);
            if (nm == null)
            {
               return Json(new { success = false, message = "Không tìm thấy thông tin nhà máy" });
            }
            nm.TenNhaMay = ten_nha_may;
            nm.TenVietTat = ten_viet_tat_nm;
            nm.DiaChi = dia_chi_nm;

            var rs_nm = NhaMayDAO.updateNhaMayInformation(nm);

            if (rs != "success")
            {
               return Json(new { success = false, message = rs });
            }
            if (rs_nm != "success")
            {
               return Json(new { success = false, message = rs_nm });
            }
            Session["CongTy"] = ct;
            Session["NhaMay"] = nm;
            return Json(new { success = true, message = string.Empty });
         }
         catch (Exception ex)
         {
            return Json(new { success = false, message = "Không thể truy cập cơ sở dữ liệu" });
         }


      }

      #region Cập nhật sản lượng dự kiến
      public ActionResult CapNhatSLDK_Datatable(int loaiDuKien, int? thang, int? nam)
      {

         ViewBag.Nam = nam;
         ViewBag.Thang = thang;
         ViewBag.loaiDuKien = loaiDuKien;

         return PartialView();
      }
      public ActionResult CapNhatSanLuongDuKienView()
      {
         return View();
      }
      [HttpPost]
      public ActionResult SanLuongDuKienPaging(int? thang, int? nam, int loai_sldk = 1)
      {
         try
         {
            using (Model1 db = new Model1())
            {
               RequestPagingModel rpm = new RequestPagingModel();
               rpm.length = int.Parse(HttpContext.Request["length"]);
               rpm.start = int.Parse(Request["start"]);
               rpm.searchValue = HttpContext.Request["search[value]"];
               rpm.sortColumnName = HttpContext.Request["columns[" + Request["order[0][column]"] + "][name]"];
               rpm.sortDirection = Request["order[0][dir]"];
               rpm.draw = Request["draw"];

               PagingModel<SanLuongDuKienViewModel> pm = new PagingModel<SanLuongDuKienViewModel>();
               var response = SanLuongDuKienDAO.GetSanLuongDuKienPaging(out pm, rpm, loai_sldk, thang, nam);

               if (!response)
               {
                  return Json(new { success = false, message = "Lỗi truy cập cơ sở dữ liệu" }, JsonRequestBehavior.AllowGet);
                  //return Json(null, JsonRequestBehavior.AllowGet);
               }
               return Json(pm, JsonRequestBehavior.AllowGet);
            }
         }
         catch (Exception ex)
         {
            return null;
         }
      }
      [HttpPost]
      public ActionResult CapNhatSLDKManageModal(int id)
      {
         SanLuongDuKien sldk = new SanLuongDuKien();
         if (id == 0)
         {
            //return Json(new { success = true, data = View() });
            return PartialView();
         }
         var rs = SanLuongDuKienDAO.GetSanLuongDuKienById(id, out sldk);
         //return Json(new { success = rs, data = View(sldk) });
         return PartialView(sldk);
      }

      [HttpPost]
      public ActionResult CapNhatSLDK_CreateOrUpdate(int id_sldk, double giatri_sldk_modal, int? sldk_thang_modal = 1, int nam_sldk_modal = 1, int loai_sldk_modal = 1)
      {
         var rs = "";
         //nam
         if (!sldk_thang_modal.HasValue || loai_sldk_modal == 2)
         {
            sldk_thang_modal = -1;
         }
         if (id_sldk == 0)
         {
            //create
            rs = SanLuongDuKienDAO.CreateSanLuongDuKien(loai_sldk_modal, sldk_thang_modal.Value, nam_sldk_modal, giatri_sldk_modal);
         }
         else
         {
            // update
            rs = SanLuongDuKienDAO.UpdateSanLuongDuKien(id_sldk, giatri_sldk_modal);
         }

         if (rs.Equals("success"))
         {
            return Json(new { success = true, message = "Thành Công!!!" });
         }
         else
         {
            return Json(new { success = false, message = rs });
         }

      }
      #endregion

      #region Cập nhật Điểm đo
      public ActionResult CapNhatDiemDoView()
      {
         ViewBag.ListNhaMay = NhaMayDAO.GetAllNhaMay();
         ViewBag.ListTCDD = TinhChatDiemDoDAO.GetAllTCDD();
         return View();
      }
      public ActionResult CapNhatDiemDo_Datatable(/*int id_nha_may,*/ int id_tinh_chat_diem_do)
      {

         //ViewBag.id_nha_may = id_nha_may;
         ViewBag.id_tinh_chat_diem_do = id_tinh_chat_diem_do;

         return PartialView();
      }
      public ActionResult CapNhatDiemDoPaging(int id_tcdd_db)
      {
         try
         {
            using (Model1 db = new Model1())
            {
               RequestPagingModel rpm = new RequestPagingModel();
               rpm.length = int.Parse(HttpContext.Request["length"]);
               rpm.start = int.Parse(Request["start"]);
               rpm.searchValue = HttpContext.Request["search[value]"];
               rpm.sortColumnName = HttpContext.Request["columns[" + Request["order[0][column]"] + "][name]"];
               rpm.sortDirection = Request["order[0][dir]"];
               rpm.draw = Request["draw"];

               int? tcdd_id = null;
               if (id_tcdd_db != -1)
               {
                  tcdd_id = id_tcdd_db;
               }

               PagingModel<DiemDoViewModel> pm = new PagingModel<DiemDoViewModel>();
               var response = DiemDoDAO.GetDiemDoPaging(out pm, rpm, tcdd_id);

               if (!response)
               {
                  return Json(new { success = false, message = "Lỗi truy cập cơ sở dữ liệu" }, JsonRequestBehavior.AllowGet);
                  //return Json(null, JsonRequestBehavior.AllowGet);
               }
               return Json(pm, JsonRequestBehavior.AllowGet);
            }
         }
         catch (Exception ex)
         {
            return null;
         }
      }

      public ActionResult CapNhatDiemDo_CongToModal(int DiemDoID = -1, int CongToID = -1)
      {
         CapNhatDiemDo_CongToViewModel ct = new CapNhatDiemDo_CongToViewModel();
         // Cap nhat Cong To
         if (CongToID != -1)
         {
            ViewBag.CongToManage = "edit";
            ViewBag.DiemDoID = DiemDoID;
            var rs = CongToDAO.GetCongToTheoDiemDoByCongToID(CongToID, DiemDoID, out ct);
            return View(ct);
         }
         // Thay doi cong to
         else
         {
            ViewBag.CongToManage = "change";
            ViewBag.DiemDoID = DiemDoID;
            return View();
         }

      }

      public ActionResult CapNhatDiemDoManageModal(int id)
      {
         ViewBag.ListNhaMay = NhaMayDAO.GetAllNhaMay();
         ViewBag.ListTCDD = TinhChatDiemDoDAO.GetAllTCDD();
         DiemDo dd = new DiemDo();
         if (id == 0)
         {
            //return Json(new { success = true, data = View() });
            return PartialView();
         }
         var rs = DiemDoDAO.GetDiemDoById(id, out dd);
         //return Json(new { success = rs, data = View(sldk) });
         return PartialView(dd);
      }
      [HttpPost]
      public ActionResult CapNhatDiemDo_CreateOrUpdate(int? MaDiemDo, int? ThuTuHienThi, int nha_may_id, int id_tinh_chat_diem_do, int id_diemdo, string TenDiemDo = "")
      {
         if (MaDiemDo == null)
         {
            return Json(new { success = false, message = "Mã điểm đo không được để trống!!!" });
         }
         if (ThuTuHienThi == null)
         {
            return Json(new { success = false, message = "Thứ tự hiển thị không được để trống!!!" });
         }
         if (string.IsNullOrEmpty(TenDiemDo))
         {
            return Json(new { success = false, message = "Tên điểm đo không được để trống!!!" });
         }
         var rs = "";
         if (id_diemdo == 0)
         {
            //create
            rs = DiemDoDAO.CreateDiemDo(MaDiemDo.Value, TenDiemDo, ThuTuHienThi.Value, nha_may_id, id_tinh_chat_diem_do, id_diemdo);
         }
         else
         {
            // update
            rs = DiemDoDAO.UpdateDiemDo(MaDiemDo.Value, TenDiemDo, ThuTuHienThi.Value, nha_may_id, id_tinh_chat_diem_do, id_diemdo);
         }

         if (rs.Equals("success"))
         {
            return Json(new { success = true, message = "Thành Công!!!" });
         }
         else
         {
            return Json(new { success = false, message = rs });
         }

      }
      [HttpPost]
      public ActionResult CapNhatDiemDo_CreateOrUpdateCongTo(int id_congto, int id_diemdo, string serial, string loai_congto, string ct_dd_thoigianketthuc, string ct_dd_thoigianbatdau)
      {
         try
         {
            DateTime dt_start = new DateTime();
            var rs_start = DateTime.TryParseExact(ct_dd_thoigianbatdau, "dd - mm - yyyy", null, DateTimeStyles.None, out dt_start);
            if (!rs_start)
            {
               return Json(new { success = false, message = "Thời gian bắt đầu không đúng định dạng" });
            }
            DateTime? dt_end = new DateTime();
            if (string.IsNullOrEmpty(ct_dd_thoigianketthuc))
            {
               dt_end = null;
            }
            else
            {
               DateTime dt = new DateTime();
               var rs_end = DateTime.TryParseExact(ct_dd_thoigianketthuc, "dd - mm - yyyy", null, DateTimeStyles.None, out dt);
               if (!rs_end)
               {
                  return Json(new { success = false, message = "Thời gian kết thúc không đúng định dạng" });
               }
               dt_end = dt;
            }

            if (id_congto != -1)
            {
               CongTo ct = new CongTo()
               {
                  ID = id_congto,
                  Serial = serial,
                  Type = loai_congto
               };
               //cap nhat thong tin cong to 
               var rs_update_congto = CongToDAO.UpdateCongTo(ct);
               var rs_update_lienket = LienKetDiemDoCongToDAO.CapNhatThoiGian(id_congto, id_diemdo, dt_start, dt_end);
            }
            else
            {
               // lien ket cong to voi diem do
               CongTo ct = new CongTo();
               ct.Serial = serial;
               ct.Type = loai_congto;
               var rs_checkexist = CongToDAO.CheckCongToExistBySerial(serial);// kiem tra cong to con tai tai hay chua
               if (rs_checkexist)
               {
                  // neu cong to da ton tai
                  var rs_check_congto_using = LienKetDiemDoCongToDAO.CheckCongToUsingBySerial(serial);// kiem tra co dang duoc su dung hay chua
                  if (rs_check_congto_using)
                  {
                     // neu da duoc su dung
                     return Json(new { success = false, message = "Công tơ đã được sử dụng" });
                  }
                  else
                  {
                     // neu chua duoc su dung => cap nhat lien ket
                     CongTo temp_ct = CongToDAO.GetCongToBySerial(serial);
                     var rs_create_lk = LienKetDiemDoCongToDAO.CreateLienKet(temp_ct.ID, id_diemdo, dt_start, dt_end);
                  }
               }
               else
               {
                  //neu cong to chua ton tai => tao moi
                  var rs_create = CongToDAO.CreateCongTo(ref ct);
                  if (!rs_create)
                  {
                     return Json(new { success = false, message = "Công tơ đã tồn tại" });
                  }
                  else
                  {
                     var rs_create_lk = LienKetDiemDoCongToDAO.CreateLienKet(ct.ID, id_diemdo, dt_start, dt_end);
                  }
               }

            }
            return Json(new { success = true, message = "Thành Công" });
         }
         catch (Exception ex)
         {
            return Json(new { success = false, message = ex.Message });
         }

      }
      #endregion
   }
}