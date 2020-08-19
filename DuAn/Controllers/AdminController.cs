using DuAn.Attribute;
using DuAn.COMMON;
using DuAn.Models.CustomModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuAn.Controllers
{
    [CheckLogin(/*RoleID = new int[1] { 2 }*/)]
    [CheckTotalRole(RoleID = new int[2] { RoleContext.Expertise, RoleContext.Administration })]
    public class AdminController : Controller
    {
        // GET: Admin
        /*public ActionResult Index()
        {
            AdminModel item = DBContext.getDataAdminModel();
            return View(item);
        }*/
        [CheckTotalRole(RoleID = new int[1] { RoleContext.Expertise_Roles })]
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
        public ActionResult QuanTriCauHinh()
        {
            return View();
        }

        [CheckTotalRole(RoleID = new int[1] { RoleContext.Administration_UpdateFile })]
        public ActionResult viewMissingDataList(string fileName = "")
        {
            var data = DBContext.getMissingData(fileName);
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

        [CheckTotalRole(RoleID = new int[1] { RoleContext.Administration_UpdateFile })]
        public ActionResult MissingDataPartial()
        {
            var result = DBContext.getMissingData();
            return PartialView(result);
        }

        [CheckTotalRole(RoleID = new int[1] { RoleContext.Administration_UpdateRecipe })]
        public ActionResult InsertFormula()
        {
            var result = DBContext.getDataAdminModel();
            return PartialView(result);
        }
    }
}