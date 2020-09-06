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
            CongTy ct = (CongTy)Session["CongTy"];
            return View(ct);
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
            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];
                if (file != null && file.ContentLength > 0)
                {
                    var parentDir = new DirectoryInfo(Server.MapPath("\\")).Parent.Parent.FullName;
                    var pathString = parentDir + "\\DocDuLieuCongTo\\TestTheoDoi";

                    var fileName1 = Path.GetFileName(file.FileName);

                    bool isExists = System.IO.Directory.Exists(pathString);

                    if (!isExists)
                        System.IO.Directory.CreateDirectory(pathString);

                    var path = string.Format("{0}\\{1}", pathString, file.FileName);
                    file.SaveAs(path);
                }
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
                var parentDir = new DirectoryInfo(Server.MapPath("\\")).Parent.Parent.FullName;
                var folderPath = parentDir + "\\DocDuLieuCongTo\\TestTheoDoi";
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
            return Json(new { Message = message });
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
        public ActionResult ChangeCauHinhWeb(HttpPostedFileBase cauhinh_logo, string ten_congty, int id, string ten_viet_tat)
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


                if (rs != "success")
                {
                    return Json(new { success = false, message = rs });
                }

                Session["CongTy"] = ct;
                return Json(new { success = true, message = string.Empty });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Không thể truy cập cơ sở dữ liệu" });
            }


        }
    }
}