﻿using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using DuAn.Attribute;
using DuAn.Models.DbModel;

namespace DuAn.Controllers
{
    [CheckLogin(/*RoleID = new int[1] { 2 }*/)]
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
        public ActionResult ProfilePartial()
        {
            Account acc = (Account)Session["User"];
            return PartialView(acc);
        }

        public ActionResult layoutUser()
        {
            Account acc = (Account)Session["User"];
            return PartialView(acc);
        }
    }
}