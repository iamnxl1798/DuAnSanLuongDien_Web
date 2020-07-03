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
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            AdminModel item = DBContext.getDataAdminModel();
            return View(item);
        }

        public ActionResult viewMissingDataList(string fileName = "")
        {
            var data = DBContext.getMissingData(fileName);
            return PartialView(data);
        }

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

        public ActionResult UpdateFormula(string formula, string name, string thoiGian)
        {
            DBContext.updateFormula(formula, name, thoiGian);
            return Json(new { Message = "Cập nhật thành công" });
        }


    }
}