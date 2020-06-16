﻿using System;
using System.Collections.Generic;
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
            return View();
        }

        public ActionResult SaveDropzoneJsUploadedFiles()
        {
            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];
                if (file != null && file.ContentLength > 0)
                {

                    var pathString = new DirectoryInfo(string.Format("{0}images", Server.MapPath(@"\"))).ToString();

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
            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];
                if (file != null && file.ContentLength > 0)
                {

                    var pathString = new DirectoryInfo(string.Format("{0}images", Server.MapPath(@"\"))).ToString();

                    var fileName1 = Path.GetFileName(file.FileName);

                    bool isExists = System.IO.Directory.Exists(pathString);

                    if (!isExists)
                        System.IO.Directory.CreateDirectory(pathString);

                    var path = string.Format("{0}\\{1}", pathString, file.FileName);
                    file.SaveAs(path);
                }
            }
            string folderPath = new DirectoryInfo(string.Format("{0}images", Server.MapPath(@"\"))).ToString();
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            FileInfo[] files = dir.GetFiles(name, SearchOption.TopDirectoryOnly);
            foreach (var item in files)
            {
                if (System.IO.File.Exists(Path.Combine(folderPath, name)))
                {
                    System.IO.File.Delete(Path.Combine(folderPath, name));
                }
            }
            return Json(new { Message = string.Empty });
        }


    }
}