﻿using DuAn.COMMON;
using DuAn.Models.CustomModel;
using DuAn.Models.DbModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Web.Optimization;
using iTextSharp.text.pdf;

namespace DuAn
{
   public class DBContext
   {
      static List<MissingDataStatus> data = new List<MissingDataStatus>();
      static DateTime startDay = DateTime.MinValue;
      static DateTime endDay = DateTime.MaxValue;
      #region Common
      public async static Task<HomeModel> getDuKien(DateTime date)
      {
         using (var db = new Model1())
         {
            try
            {
               //db.Database.Log = (s) => { };
               DateTime dateAddOneMonth = date.AddMonths(1);
               DateTime thang = new DateTime(date.Year, date.Month, 1);
               DateTime thangEnd = new DateTime(dateAddOneMonth.Year, dateAddOneMonth.Month, 1);
               DateTime nam = new DateTime(date.Year, 1, 1);
               DateTime namEnd = new DateTime(date.AddYears(1).Year, 1, 1);
               List<DiemDoData> list = new List<DiemDoData>();
               var temp1 = db.SanLuongs.Where(x => x.Ngay == date).OrderBy(x => x.DiemDo.ThuTuHienThi).ToList();
               List<int> temp = temp1.Select(x => x.DiemDoID).Distinct().ToList();
               var allListDiemDo = db.DiemDoes.AsNoTracking().ToList();

               foreach (int itemp in temp)
               {
                  var listDiemDo = allListDiemDo.Where(x => x.ID == itemp).FirstOrDefault();
                  var listSanLuong = temp1.Where(x => x.DiemDoID == itemp);
                  list.Add(new DiemDoData
                  {
                     tenDiemDo = listDiemDo?.TenDiemDo,
                     maDiemDo = (listDiemDo?.MaDiemDo).GetValueOrDefault(),
                     tinhChat = listDiemDo?.TinhChatDiemDo.TenTinhChat,
                     thuTuHienThi = (listDiemDo?.ThuTuHienThi).GetValueOrDefault(),
                     kwhGiao = listSanLuong.Where(x => x.KenhID == CommonContext.KWH_GIAO).Select(x => x.GiaTri).ToList(),
                     kwhNhan = listSanLuong.Where(x => x.KenhID == CommonContext.KWH_NHAN).Select(x => x.GiaTri).ToList(),
                     kvarhGiao = listSanLuong.Where(x => x.KenhID == CommonContext.KVARH_GIAO).Select(x => x.GiaTri).ToList(),
                     kvarhNhan = listSanLuong.Where(x => x.KenhID == CommonContext.KVARH_NHAN).Select(x => x.GiaTri).ToList()
                  });
               }
               db.Configuration.LazyLoadingEnabled = false;
               var dataTrongNgay = db.TongSanLuong_Ngay.Where(x => x.Ngay == date).ToList();
               var thucteThang = db.TongSanLuong_Thang.Where(x => x.Thang == date.Month && x.Nam == date.Year).Select(x => x.GiaTri).FirstOrDefault();
               var thucTeNam = db.TongSanLuong_Nam.Where(x => x.Nam == date.Year).Select(x => x.GiaTri).FirstOrDefault();
               var missingData = getMissingCount(date);
               var missingDataCount = new List<NumberOfMissingData>();
               var distincMissingType = missingData.Select(x => x.type).Distinct();
               foreach (string item in distincMissingType)
               {
                  var tempData = missingData.Where(x => x.type == item);
                  missingDataCount.Add(new NumberOfMissingData
                  {
                     type = item,
                     notYet = tempData.Where(x => x.status == 0 || x.status == -1).Count(),
                     done = tempData.Where(x => x.status == 1).Count()
                  });
               }
               var giaDien = db.GiaDiens.Where(x => x.NgayBatDau <= date && x.NgayKetThuc >= date).Select(x => x.Gia).FirstOrDefault();
               var result = new HomeModel();

               //var dukienThang_temp = db.SanLuongDuKiens.Where(x => x.LoaiID == 1 && x.ThoiGian == thang).Select(x => x.SanLuong).FirstOrDefault();
               result.duKienThang = db.SanLuongDuKiens.Where(x => x.LoaiID == CommonContext.LOAI_SAN_LUONG_THANG && x.ThoiGian == thang).Select(x => x.SanLuong).FirstOrDefault();
               result.duKienNam = db.SanLuongDuKiens.Where(x => x.LoaiID == CommonContext.LOAI_SAN_LUONG_NAM && x.ThoiGian == nam).Select(x => x.SanLuong).FirstOrDefault();
               result.thucTeThang = thucteThang.HasValue ? thucteThang.Value : 0;
               result.thucTeNam = thucTeNam.HasValue ? thucTeNam.Value : 0;
               result.data = list;
               result.sanLuongTrongNgay = dataTrongNgay;
               result.date = date;
               result.countMissingData = missingDataCount;
               result.giaDien = giaDien;
               result.doanhThuThang = getDoanhThuThang(date).Sum(x => x.doanhThu);
               result.doanhThuNam = getDoanhThuNam(date).Sum(x => x.doanhThu);

               return await Task.FromResult(result);
            }
            catch
            {
               return null;
            }
         }
      }
      public static HomeModel getDoanhThuNgayDetail(DateTime date)
      {
         using (var db = new Model1())
         {
            var dataTrongNgay = db.TongSanLuong_Ngay.Where(x => x.Ngay == date).ToList();
            var giaTien = db.GiaDiens.Where(x => x.NgayBatDau <= date && x.NgayKetThuc >= date).Select(x => x.Gia).FirstOrDefault();
            return new HomeModel
            {
               sanLuongTrongNgay = dataTrongNgay,
               giaDien = giaTien,
               date = date
            };
         }
      }
      public static Account ChangeInfo(HttpPostedFileBase avatar, string fullname, string email, string address, string phone, string dob, string id)
      {
         using (var db = new Model1())
         {
            int inInt = int.Parse(id);
            var acc = db.Accounts.Include(x => x.RoleAccount).FirstOrDefault(x => x.ID == inInt);
            string fileName = "default_avatar.png";
            try
            {
               if (avatar != null)
               {
                  //fileName = System.IO.Path.GetFileName(avatar.FileName) + "/" + id;
                  fileName = "avatar_" + id + ".png";
                  string path_avatar = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory.ToString()) + "\\images\\avatarAccount\\" + fileName;
                  // file is uploaded
                  avatar.SaveAs(path_avatar);
                  // file is uploaded
                  avatar.SaveAs(path_avatar);
                  acc.Avatar = fileName;
               }
               acc.Fullname = fullname;
               acc.Email = email;
               acc.Address = address;
               acc.Phone = phone;
               acc.DOB = DateTime.ParseExact(dob, "dd/MM/yyyy", CultureInfo.InvariantCulture);
               db.SaveChanges();
            }
            catch
            {
               return null;
            }
            return acc;
         }
      }
      public static DiemDoData getChiTietDiemDo(int id, DateTime date)
      {
         using (var db = new Model1())
         {
            try
            {
               DateTime sanLuongDate = date;
               var result = db.SanLuongs.Where(x => x.DiemDo.MaDiemDo == id && x.Ngay == sanLuongDate).ToList();
               DiemDoData obj = new DiemDoData()
               {
                  tenDiemDo = result.Select(x => x.DiemDo.TenDiemDo).FirstOrDefault(),
                  maDiemDo = result.Select(x => x.DiemDo.MaDiemDo).FirstOrDefault(),
                  kwhGiao = result.Where(x => x.KenhID == CommonContext.KWH_GIAO).Select(x => x.GiaTri).ToList(),
                  kwhNhan = result.Where(x => x.KenhID == CommonContext.KWH_NHAN).Select(x => x.GiaTri).ToList(),
                  kvarhGiao = result.Where(x => x.KenhID == CommonContext.KVARH_GIAO).Select(x => x.GiaTri).ToList(),
                  kvarhNhan = result.Where(x => x.KenhID == CommonContext.KVARH_NHAN).Select(x => x.GiaTri).ToList()
               };
               return obj;
            }
            catch
            {
               return null;
            }
         }
      }
      public static List<TongSanLuongTheoThoiGian> getDoanhThuThang(DateTime date)
      {
         using (var db = new Model1())
         {
            try
            {
               DateTime dateStart = new DateTime(date.Year, date.Month, 1);
               DateTime dateEnd = dateStart.AddMonths(1);
               var result = new List<TongSanLuongTheoThoiGian>();
               for (DateTime dateFor = dateStart; dateFor < dateEnd; dateFor = dateFor.AddDays(1))
               {
                  TongSanLuongTheoThoiGian tslttg = new TongSanLuongTheoThoiGian();
                  tslttg.date = dateFor;
                  tslttg.giaTien = GetGiaDienTheoNgay(dateFor);
                  tslttg.value = GetTongSanLuongNgay(dateFor);
                  tslttg.doanhThu = tslttg.value * tslttg.giaTien;
                  result.Add(tslttg);
               }
               result.RemoveAll(x => x.value == 0 || x.giaTien == 0);
               return result;
            }
            catch
            {
               return null;
            }
         }
      }
      public static List<TongSanLuongTheoThoiGian> getChiTietThang(DateTime date)
      {
         using (var db = new Model1())
         {
            try
            {
               DateTime dateStart = new DateTime(date.Year, date.Month, 1);
               DateTime dateEnd = dateStart.AddMonths(1);
               var result = new List<TongSanLuongTheoThoiGian>();
               var list = db.TongSanLuong_Ngay.Where(x => x.Ngay >= dateStart && x.Ngay < dateEnd);
               if (list != null)
               {
                  result = list
                      .GroupBy(x => x.Ngay)
                      .Select(x => new TongSanLuongTheoThoiGian()
                      {
                         date = x.Key,
                         value = x.Sum(c => c.GiaTri),
                      })
                      .OrderBy(x => x.date)
                      .ToList();
               }
               /*var result = db.TongSanLuong_Ngay.Where(x => x.Ngay >= dateStart && x.Ngay < dateEnd)..Select(cl => new TongSanLuongTheoThoiGian
               {
                   date = cl.Key,
                   value = cl.Sum(c => c.GiaTri),
               }).OrderBy(x => x.date).ToList();*/
               return result;
            }
            catch
            {
               return null;
            }
         }
      }

      public static List<TongSanLuongTheoThoiGian> getChiTietNam(DateTime date)
      {
         using (var db = new Model1())
         {
            try
            {
               DateTime dateStart = new DateTime(date.Year, 1, 1);
               DateTime dateEnd = dateStart.AddYears(1);
               var result = new List<TongSanLuongTheoThoiGian>();
               var list = db.TongSanLuong_Thang.Where(x => x.Nam == date.Year).ToList();
               if (list != null && list.Count > 0)
               {
                  result = list.Select(x => new TongSanLuongTheoThoiGian()
                  {
                     date = new DateTime(x.Nam, x.Thang, 1),
                     value = x.GiaTri.HasValue ? x.GiaTri.Value : 0
                  }).ToList();
               }
               /*for (DateTime month = dateStart; month < dateEnd; month = month.AddMonths(1))
               {
                   var addOneMonth = month.AddMonths(1);
                   result.Add(
                       db.TongSanLuong_Thang.Where(x => x.Thang == month.Month && x.Nam == month.Year)
                                            .Select(
                                                   x => new TongSanLuongTheoThoiGian
                                                   {
                                                       date = month,
                                                       value = x.GiaTri.HasValue ? x.GiaTri.Value : 0
                                                   })
                                            .FirstOrDefault()
                             );
               }*/
               result.RemoveAll(x => x == null);
               return result;
            }
            catch
            {
               return null;
            }
         }
      }
      public static List<TongSanLuongTheoThoiGian> getDoanhThuNam(DateTime date)
      {
         using (var db = new Model1())
         {
            try
            {
               DateTime dateStart = new DateTime(date.Year, 1, 1);
               DateTime dateEnd = new DateTime(date.Year + 1, 1, 1);
               var result = new List<TongSanLuongTheoThoiGian>();
               /*for (DateTime month = dateStart; month < dateEnd; month = month.AddMonths(1))
               {
                   var addOneMonth = month.AddMonths(1);
                   var list = new List<TongSanLuongTheoThoiGian>();
                   for (DateTime dateFor = month; dateFor < addOneMonth; dateFor = dateFor.AddDays(1))
                   {
                       var gia = db.GiaDiens.Where(x => x.NgayBatDau <= dateFor && x.NgayKetThuc >= dateFor).Select(x => x.Gia).FirstOrDefault();
                       var value = db.TongSanLuong_Ngay.Where(x => x.Ngay == dateFor).ToList().Count == 0 ? 0 : db.TongSanLuong_Ngay.Where(x => x.Ngay == dateFor).Sum(x => x.GiaTri);
                       list.Add(new TongSanLuongTheoThoiGian
                       {
                           date = dateFor,
                           giaTien = gia,
                           value = value,
                           doanhThu = gia * value
                       });
                   }
                   result.Add(new TongSanLuongTheoThoiGian
                   {
                       date = month,
                       giaTien = list.Where(x => x.giaTien != 0 && x.date >= dateStart && x.date < dateEnd).Select(x => x.giaTien).FirstOrDefault(),
                       value = list.Sum(x => x.value),
                       doanhThu = list.Sum(x => x.doanhThu)
                   });
               }*/
               for (DateTime month = dateStart; month < dateEnd; month = month.AddMonths(1))
               {
                  //var addOneMonth = month.AddMonths(1);
                  var list_ngay = getDoanhThuThang(month);
                  /*var list_ngay = new List<TongSanLuongTheoThoiGian>();
                  for (DateTime dateFor = month; dateFor < addOneMonth; dateFor = dateFor.AddDays(1))
                  {
                      TongSanLuongTheoThoiGian tslttg = new TongSanLuongTheoThoiGian();
                      tslttg.date = dateFor;
                      tslttg.giaTien = GetGiaDienTheoNgayNgay(dateFor);
                      tslttg.value = GetTongSanLuongNgay(dateFor);
                      tslttg.doanhThu = tslttg.value * tslttg.giaTien;
                      list_ngay.Add(tslttg);
                  }
                  list_ngay.RemoveAll(x => x.giaTien == -1 || x.value == -1 || x.giaTien == 0);*/
                  if (list_ngay.Count > 0)
                  {
                     result.Add(new TongSanLuongTheoThoiGian
                     {
                        date = month,
                        giaTien = -1,
                        value = list_ngay.Sum(x => x.value),
                        doanhThu = list_ngay.Sum(x => x.doanhThu)
                     });
                  }
               }
               //result.RemoveAll(x => x.doanhThu == 0);
               return result;
            }
            catch
            {
               return null;
            }
         }
      }
      private static double GetTongSanLuongNgay(DateTime dt)
      {
         try
         {
            using (var db = new Model1())
            {
               var sum = 0.0;
               var list = db.TongSanLuong_Ngay.Where(x => x.Ngay == dt).ToList();
               if (list != null && list.Count() > 0)
               {
                  sum = list.Sum(x => x.GiaTri);
               }
               return sum;
            }

         }
         catch
         {
            return -1;
         }
      }

      private static double GetGiaDienTheoNgay(DateTime dt)
      {
         try
         {
            using (var db = new Model1())
            {
               var value = db.GiaDiens.Where(x => x.NgayBatDau <= dt && x.NgayKetThuc >= dt).Select(x => x.Gia).FirstOrDefault();
               return value;
            }

         }
         catch
         {
            return -1;
         }
      }

      public static ThongSoPatialModel getThongSoVanHanh(string id, DateTime date)
      {
         using (var db = new Model1())
         {
            var data = db.ThongSoVanHanhs.ToList();
            var allDiemDo = db.DiemDoes.ToList();
            var listDate = data.Select(x => x.ThoiGianCongTo.Date).Distinct().ToList();
            var idInt = -1;
            if (!string.IsNullOrEmpty(id))
            {
               idInt = int.Parse(id);
               var join = (from bridge in db.DiemDo_CongTo select new { s = bridge.DiemDo, c = bridge.CongTo }).Where(x => x.s.ID == idInt);
               var serial = join.Select(x => x.c.Serial).FirstOrDefault();
               data = data.Where(x => x.Serial == serial).ToList();
            }
            if (date != DateTime.MinValue)
            {
               data = data.Where(x => x.ThoiGianCongTo.Date == date.Date).ToList();
            }
            data = data.OrderByDescending(x => x.ThoiGianCongTo).ToList();
            var finalData = new List<ThongSoAndTenDiemDo>();
            var diemDoIndex = -1;
            var thoiGianIndex = -1;
            for (int i = 0; i < data.Count; i++)
            {
               var serial = data[i].Serial;
               var joinedTable = (from bridge in db.DiemDo_CongTo select new { s = bridge.DiemDo, c = bridge.CongTo }).Where(x => x.c.Serial == serial);
               finalData.Add(new ThongSoAndTenDiemDo
               {
                  thongSo = data[i],
                  TenDiemDo = joinedTable.Select(x => x.s.TenDiemDo).FirstOrDefault(),
               });
            }
            thoiGianIndex = listDate.IndexOf(date);
            for (int i = 0; i < allDiemDo.Count; i++)
            {
               if (allDiemDo[i].ID == idInt)
               {
                  diemDoIndex = i;
               }
            }
            var result = new ThongSoPatialModel
            {
               allDiemDo = allDiemDo,
               thongSo = finalData,
               dateDistinc = listDate,
               diemDoIndex = diemDoIndex,
               thoiGianIndex = thoiGianIndex
            };
            return result;
         }
      }
      public static AdminModel getDataAdminModel()
      {
         using (var db = new Model1())
         {
            var result = new AdminModel()
            {
               listDiemDo = db.DiemDoes.ToList(),
               listKenh = db.Kenhs.ToList(),
               getLastDate = DateTime.Now
            };
            return result;
         }
      }

      public static void updateFormula(string formular, string name, string thoiGian)
      {
         using (var db = new Model1())
         {
            DateTime batDau = DateTime.MinValue;
            if (db.CongThucTongSanLuongs.Count() != 0)
            {
               var dateNow = DateTime.Now;
               var startTimeExceed = db.CongThucTongSanLuongs.Where(x => x.ThoiGianBatDau >= dateNow).ToList();
               var endTimeExceed = db.CongThucTongSanLuongs.Where(x => x.ThoiGianKetThuc >= dateNow).ToList();
               foreach (CongThucTongSanLuong date in startTimeExceed)
               {
                  date.ThoiGianBatDau = DateTime.Now.AddDays(-1);
               }
               foreach (CongThucTongSanLuong date in endTimeExceed)
               {
                  date.ThoiGianKetThuc = DateTime.Now;
               }
            }
            var item = new CongThucTongSanLuong()
            {
               Ten = name,
               CongThuc = formular,
               ThoiGianBatDau = DateTime.Now,
               ThoiGianKetThuc = DateTime.ParseExact(thoiGian, "dd/MM/yyyy", null)
            };
            db.CongThucTongSanLuongs.Add(item);
            db.SaveChanges();
         }
      }

      public static bool InsertGiaDien(string thoiGianBatDau, string thoiGianKetThuc, int giaDien)
      {
         using (var db = new Model1())
         {
            try
            {
               var start_date = DateTime.ParseExact(thoiGianBatDau, "dd/MM/yyyy", null);
               var end_date = DateTime.ParseExact(thoiGianKetThuc, "dd/MM/yyyy", null);
               var rs_1 = db.GiaDiens.Where(x => x.NgayBatDau < start_date && x.NgayKetThuc >= start_date && x.NgayKetThuc <= end_date).FirstOrDefault();
               if (rs_1 != null)
               {
                  rs_1.NgayKetThuc = start_date.AddDays(-1);
               }
               var rs_2 = db.GiaDiens.Where(x => x.NgayBatDau >= start_date && x.NgayKetThuc <= end_date).FirstOrDefault();
               if (rs_2 != null)
               {
                  db.GiaDiens.Remove(rs_2);
               }
               var rs_3 = db.GiaDiens.Where(x => x.NgayBatDau >= start_date && x.NgayBatDau <= end_date && x.NgayKetThuc > end_date).FirstOrDefault();
               if (rs_3 != null)
               {
                  rs_3.NgayBatDau = end_date.AddDays(1);
               }
               var rs_4 = db.GiaDiens.Where(x => x.NgayBatDau < start_date && x.NgayKetThuc > end_date).FirstOrDefault();
               if (rs_4 != null)
               {
                  rs_4.NgayKetThuc = start_date.AddDays(-1);
                  GiaDien dg_1 = new GiaDien();
                  dg_1.NgayBatDau = end_date.AddDays(1);
                  dg_1.NgayKetThuc = rs_4.NgayKetThuc;
                  dg_1.Gia = rs_4.Gia;
                  db.GiaDiens.Add(dg_1);
               }
               GiaDien dg = new GiaDien();
               dg.NgayBatDau = start_date;
               dg.NgayKetThuc = end_date;
               dg.Gia = giaDien;

               db.GiaDiens.Add(dg);
               db.SaveChanges();
               return true;
            }
            catch
            {
               return false;
            }
         }
      }

      public static List<MissingDataStatus> getMissingData(string start, string end, string name = "")
      {
         using (var db = new Model1())
         {
            if (data.Count() == 0 && name.Length == 0)
            {
               startDay = DateTime.Now.AddDays(-2);
               endDay = DateTime.Now.AddDays(-1);
               IEnumerable<DiemDo> listDiemDo = db.DiemDoes;
               for (DateTime date = startDay; date <= endDay; date = date.AddDays(1))
               {
                  foreach (DiemDo item in listDiemDo)
                  {
                     if (db.SanLuongs.Where(x => x.Ngay == date).Where(x => x.DiemDoID == item.ID).Count() == 0)
                     {
                        string fileName = date.Day.ToString("00") + date.Month.ToString("00") + (date.Year % 10).ToString() + item.MaDiemDo.ToString() + ".CSV";
                        //var pathString = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory.ToString()).Parent.Parent.FullName + "\\DocDuLieuCongTo\\TestTheoDoi\\" + fileName;
                        var pathString = @"C:\SLCTO\ESMR\" + fileName;
                        if (File.Exists(pathString))
                        {
                           data.Add(new MissingDataStatus()
                           {
                              date = date.ToString("dd/MM/yyyy"),
                              name = item.TenDiemDo,
                              status = 0,
                              type = item.TinhChatDiemDo.TenTinhChat
                           });
                        }
                        else
                        {
                           data.Add(new MissingDataStatus()
                           {
                              date = date.ToString("dd/MM/yyyy"),
                              name = item.TenDiemDo,
                              status = -1,
                              type = item.TinhChatDiemDo.TenTinhChat
                           });
                        }
                     }
                  }
               }
               return data;
            }
            else if (!string.IsNullOrEmpty(start) || !string.IsNullOrEmpty(end))
            {
               data.Clear();
               if (string.IsNullOrEmpty(end))
               {
                  endDay = DateTime.Now.AddDays(-1);
               }
               else
               {
                  endDay = DateTime.ParseExact(end, "dd/MM/yyyy", null);
               }
               startDay = DateTime.ParseExact(start, "dd/MM/yyyy", null);
               IEnumerable<DiemDo> listDiemDo = db.DiemDoes;
               for (DateTime date = startDay; date <= endDay; date = date.AddDays(1))
               {
                  foreach (DiemDo item in listDiemDo)
                  {
                     if (db.SanLuongs.Where(x => x.Ngay == date).Where(x => x.DiemDoID == item.ID).Count() == 0)
                     {
                        string fileName = date.Day.ToString("00") + date.Month.ToString("00") + (date.Year % 10).ToString() + item.MaDiemDo.ToString() + ".CSV";
                        var pathString = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory.ToString()).Parent.Parent.FullName + "\\DocDuLieuCongTo\\TestTheoDoi\\" + fileName;
                        if (File.Exists(pathString))
                        {
                           data.Add(new MissingDataStatus()
                           {
                              date = date.ToString("dd/MM/yyyy"),
                              name = item.TenDiemDo,
                              status = 0,
                              type = item.TinhChatDiemDo.TenTinhChat
                           });
                        }
                        else
                        {
                           data.Add(new MissingDataStatus()
                           {
                              date = date.ToString("dd/MM/yyyy"),
                              name = item.TenDiemDo,
                              status = -1,
                              type = item.TinhChatDiemDo.TenTinhChat
                           });
                        }
                     }
                  }
               }
               return data;
            }
            else if (name.Length == 0) { return data; }
            else
            {
               string fileName = name.Split('.')[0];
               int MaDiemDo = int.Parse(fileName.Substring(5));
               string day = fileName.Substring(0, 2);
               string month = fileName.Substring(2, 2);
               string year = fileName.Substring(4, 1);
               year = DateTime.Now.Year.ToString().Substring(0, 3) + year;
               DateTime date = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
               if (date < startDay || date > endDay)
               {
                  data.Clear();
                  return getMissingData(date.ToString("dd/MM/yyyy"), string.Empty);
               }
               var diemDo = db.DiemDoes.Where(x => x.MaDiemDo == MaDiemDo).FirstOrDefault();
               int id = diemDo.ID;
               string tenDiemDo = diemDo.TenDiemDo;
               if (db.SanLuongs.Where(x => x.Ngay == date).Where(x => x.DiemDoID == id).Count() > 0)
               {
                  int index = data.FindIndex(x => x.name == tenDiemDo && x.date == date.ToString("dd/MM/yyyy"));
                  if (index != -1)
                  {
                     var obj = data[index];
                     obj.status = 1;
                     data.RemoveAt(index);
                     data.Insert(0, obj);
                  }
               }
               else
               {
                  int index = data.FindIndex(x => x.name == tenDiemDo && x.date == date.ToString("dd/MM/yyyy"));
                  if (index != -1)
                  {
                     var obj = data[index];
                     obj.status = 0;
                     data.RemoveAt(index);
                     data.Insert(0, obj);
                  }
               }
               return data;
            }
         }
      }

      public static List<MissingDataStatus> getMissingCount(DateTime date)
      {
         using (var db = new Model1())
         {
            var result = new List<MissingDataStatus>();
            var listDiemDo = db.DiemDoes.ToList();
            foreach (DiemDo item in listDiemDo)
            {
               if (db.SanLuongs.Where(x => x.Ngay == date).Where(x => x.DiemDoID == item.ID).Count() == 0)
               {
                  result.Add(new MissingDataStatus()
                  {
                     date = date.ToString("dd/MM/yyyy"),
                     name = item.TenDiemDo,
                     status = -1,
                     type = item.TinhChatDiemDo.TenTinhChat
                  });
               }
               else
               {
                  result.Add(new MissingDataStatus
                  {
                     date = date.ToString("dd/MM/yyyy"),
                     name = item.TenDiemDo,
                     status = 1,
                     type = item.TinhChatDiemDo.TenTinhChat
                  });
               }
            }
            return result;
         }
      }

      public static FileResult exportExcel(DateTime date)
      {
         try
         {

            using (var db = new Model1())
            {
               DateTime thang = new DateTime(date.Year, date.Month, 1);
               var rawData = db.ChiSoChots.Where(x => x.thang == thang).ToList();
               List<ExportExcelModel> data = new List<ExportExcelModel>();
               foreach (ChiSoChot item in rawData)
               {
                  int congToID = db.CongToes.Where(x => x.Serial == item.CongToSerial).Select(x => x.ID).FirstOrDefault();
                  int diemDoID = db.DiemDo_CongTo.Where(x => x.CongToID == congToID).Select(x => x.DiemDoID).FirstOrDefault();
                  var diemDo = db.DiemDoes.Where(x => x.ID == diemDoID).FirstOrDefault();
                  KenhCustom giao = new KenhCustom()
                  {
                     bieuTong = item.TongGiao,
                     phanKhang = item.PhanKhangGiao,
                     bieu1 = item.BinhThuongGiao,
                     bieu2 = item.CaoDiemGiao,
                     bieu3 = item.ThapDiemGiao
                  };
                  KenhCustom nhan = new KenhCustom()
                  {
                     bieuTong = item.TongNhan,
                     phanKhang = item.PhangKhangNhan,
                     bieu1 = item.BinhThuongNhan,
                     bieu2 = item.CaoDiemNhan,
                     bieu3 = item.ThapDiemNhan
                  };
                  ExportExcelModel result = new ExportExcelModel()
                  {
                     maDiemDo = diemDo.MaDiemDo,
                     type = diemDo.TinhChatDiemDo.TenTinhChat,
                     dienNangGiao = giao,
                     dienNangNhan = nhan
                  };
                  data.Add(result);
               }
               data.OrderBy(x => x.type);
               return GenerateExcel(data, "rpt_PhieuTongHop_GNDN_NMD_ChiTiet.xlsx", date);
            }
         }
         catch
         {
            return null;
         }
      }
      public static FileResult GenerateExcel(List<ExportExcelModel> data, string fileDir, DateTime date)
      {
         try
         {
            FileInfo newFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.ToString() + fileDir);
            if (!newFile.Exists)
            {
               return null;
            }


            using (ExcelPackage package = new ExcelPackage(newFile))
            {
               ExcelWorksheet wookSheet = package.Workbook.Worksheets[1];
               int rowIndex = 12;
               wookSheet.Row(4).Style.Font.Bold = true;
               wookSheet.Cells[4, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
               wookSheet.Cells[4, 4].Value = "BIÊN BẢN XÁC NHẬN CHỈ SỐ CÔNG TƠ VÀ ĐIỆN NĂNG THÁNG " + date.Month + " NĂM " + date.Year;
               foreach (ExportExcelModel item in data)
               {
                  var he_so_nhan = 1000;
                  wookSheet.Row(rowIndex).Style.Font.Bold = true;
                  wookSheet.Cells[rowIndex, 1].Value = ToRoman(data.IndexOf(item) + 1);
                  wookSheet.Cells[rowIndex, 2].Value = item.type;
                  wookSheet.Cells[rowIndex, 5].Style.Font.Bold = false;
                  wookSheet.Cells[rowIndex, 5].Value = item.maDiemDo;
                  rowIndex += 1;
                  wookSheet.Row(rowIndex).Style.Font.Bold = true;
                  wookSheet.Cells[rowIndex, 1].Value = 1;
                  wookSheet.Cells[rowIndex, 2].Value = "Điện năng giao";
                  rowIndex += 1;
                  wookSheet.Cells[rowIndex, 1].Value = "a";
                  wookSheet.Cells[rowIndex, 2].Value = "Điện năng tác dụng giao - Biểu tổng";
                  wookSheet.Cells[rowIndex, 7].Value = 0;
                  wookSheet.Cells[rowIndex, 8].Value = item.dienNangGiao.bieuTong * 0.001;
                  wookSheet.Cells[rowIndex, 11].Value = item.dienNangGiao.bieuTong * 0.001;

                  wookSheet.Cells[rowIndex, 12].Value = he_so_nhan;
                  wookSheet.Cells[rowIndex, 13].Value = item.dienNangGiao.bieuTong * 0.001 * he_so_nhan;
                  wookSheet.Cells[rowIndex, 15].Value = item.dienNangGiao.bieuTong * 0.001 * he_so_nhan;
                  rowIndex += 1;
                  wookSheet.Cells[rowIndex, 1].Value = "b";
                  wookSheet.Cells[rowIndex, 2].Value = "Điện năng tác dụng giao - Biểu 1";
                  wookSheet.Cells[rowIndex, 7].Value = 0;
                  wookSheet.Cells[rowIndex, 8].Value = item.dienNangGiao.bieu1 * 0.001;
                  wookSheet.Cells[rowIndex, 11].Value = item.dienNangGiao.bieu1 * 0.001;
                  wookSheet.Cells[rowIndex, 12].Value = he_so_nhan;
                  wookSheet.Cells[rowIndex, 13].Value = item.dienNangGiao.bieu1 * 0.001 * he_so_nhan;
                  wookSheet.Cells[rowIndex, 15].Value = item.dienNangGiao.bieu1 * 0.001 * he_so_nhan;
                  rowIndex += 1;
                  wookSheet.Cells[rowIndex, 1].Value = "c";
                  wookSheet.Cells[rowIndex, 2].Value = "Điện năng tác dụng giao - Biểu 2";
                  wookSheet.Cells[rowIndex, 7].Value = 0;
                  wookSheet.Cells[rowIndex, 8].Value = item.dienNangGiao.bieu2 * 0.001;
                  wookSheet.Cells[rowIndex, 11].Value = item.dienNangGiao.bieu2 * 0.001;
                  wookSheet.Cells[rowIndex, 12].Value = he_so_nhan;
                  wookSheet.Cells[rowIndex, 13].Value = item.dienNangGiao.bieu2 * 0.001 * he_so_nhan;
                  wookSheet.Cells[rowIndex, 15].Value = item.dienNangGiao.bieu2 * 0.001 * he_so_nhan;
                  rowIndex += 1;
                  wookSheet.Cells[rowIndex, 1].Value = "d";
                  wookSheet.Cells[rowIndex, 2].Value = "Điện năng tác dụng giao - Biểu 3";
                  wookSheet.Cells[rowIndex, 7].Value = 0;
                  wookSheet.Cells[rowIndex, 8].Value = item.dienNangGiao.bieu3 * 0.001;
                  wookSheet.Cells[rowIndex, 11].Value = item.dienNangGiao.bieu3 * 0.001;
                  wookSheet.Cells[rowIndex, 12].Value = he_so_nhan;
                  wookSheet.Cells[rowIndex, 13].Value = item.dienNangGiao.bieu3 * 0.001 * he_so_nhan;
                  wookSheet.Cells[rowIndex, 15].Value = item.dienNangGiao.bieu3 * 0.001 * he_so_nhan;
                  rowIndex += 1;
                  wookSheet.Cells[rowIndex, 1].Value = "e";
                  wookSheet.Cells[rowIndex, 2].Value = "Điện năng phản kháng Giao";
                  wookSheet.Cells[rowIndex, 7].Value = 0;
                  wookSheet.Cells[rowIndex, 8].Value = item.dienNangGiao.phanKhang * 0.001;
                  wookSheet.Cells[rowIndex, 11].Value = item.dienNangGiao.phanKhang * 0.001;
                  wookSheet.Cells[rowIndex, 12].Value = he_so_nhan;
                  wookSheet.Cells[rowIndex, 13].Value = item.dienNangGiao.phanKhang * 0.001 * he_so_nhan;
                  wookSheet.Cells[rowIndex, 15].Value = item.dienNangGiao.phanKhang * 0.001 * he_so_nhan;

                  //============================================================================
                  rowIndex += 1;
                  wookSheet.Row(rowIndex).Style.Font.Bold = true;
                  wookSheet.Cells[rowIndex, 1].Value = 2;
                  wookSheet.Cells[rowIndex, 2].Value = "Điện năng nhận";
                  rowIndex += 1;
                  wookSheet.Cells[rowIndex, 1].Value = "a";
                  wookSheet.Cells[rowIndex, 2].Value = "Điện năng tác dụng nhận - Biểu tổng";
                  wookSheet.Cells[rowIndex, 7].Value = 0;
                  wookSheet.Cells[rowIndex, 8].Value = item.dienNangNhan.bieuTong * 0.001;
                  wookSheet.Cells[rowIndex, 11].Value = item.dienNangNhan.bieuTong * 0.001;
                  wookSheet.Cells[rowIndex, 12].Value = he_so_nhan;
                  wookSheet.Cells[rowIndex, 13].Value = item.dienNangNhan.bieuTong * 0.001 * he_so_nhan;
                  wookSheet.Cells[rowIndex, 15].Value = item.dienNangNhan.bieuTong * 0.001 * he_so_nhan;
                  rowIndex += 1;
                  wookSheet.Cells[rowIndex, 1].Value = "b";
                  wookSheet.Cells[rowIndex, 2].Value = "Điện năng tác dụng nhận - Biểu 1";
                  wookSheet.Cells[rowIndex, 7].Value = 0;
                  wookSheet.Cells[rowIndex, 8].Value = item.dienNangNhan.bieu1 * 0.001;
                  wookSheet.Cells[rowIndex, 11].Value = item.dienNangNhan.bieu1 * 0.001;
                  wookSheet.Cells[rowIndex, 12].Value = he_so_nhan;
                  wookSheet.Cells[rowIndex, 13].Value = item.dienNangNhan.bieu1 * 0.001 * he_so_nhan;
                  wookSheet.Cells[rowIndex, 15].Value = item.dienNangNhan.bieu1 * 0.001 * he_so_nhan;
                  rowIndex += 1;
                  wookSheet.Cells[rowIndex, 1].Value = "c";
                  wookSheet.Cells[rowIndex, 2].Value = "Điện năng tác dụng nhận - Biểu 2";
                  wookSheet.Cells[rowIndex, 7].Value = 0;
                  wookSheet.Cells[rowIndex, 8].Value = item.dienNangNhan.bieu2 * 0.001;
                  wookSheet.Cells[rowIndex, 11].Value = item.dienNangNhan.bieu2 * 0.001;
                  wookSheet.Cells[rowIndex, 12].Value = he_so_nhan;
                  wookSheet.Cells[rowIndex, 13].Value = item.dienNangNhan.bieu2 * 0.001 * he_so_nhan;
                  wookSheet.Cells[rowIndex, 15].Value = item.dienNangNhan.bieu2 * 0.001 * he_so_nhan;
                  rowIndex += 1;
                  wookSheet.Cells[rowIndex, 1].Value = "d";
                  wookSheet.Cells[rowIndex, 2].Value = "Điện năng tác dụng nhận - Biểu 3";
                  wookSheet.Cells[rowIndex, 7].Value = 0;
                  wookSheet.Cells[rowIndex, 8].Value = item.dienNangNhan.bieu3 * 0.001;
                  wookSheet.Cells[rowIndex, 11].Value = item.dienNangNhan.bieu3 * 0.001;
                  wookSheet.Cells[rowIndex, 12].Value = he_so_nhan;
                  wookSheet.Cells[rowIndex, 13].Value = item.dienNangNhan.bieu3 * 0.001 * he_so_nhan;
                  wookSheet.Cells[rowIndex, 15].Value = item.dienNangNhan.bieu3 * 0.001 * he_so_nhan;
                  rowIndex += 1;
                  wookSheet.Cells[rowIndex, 1].Value = "e";
                  wookSheet.Cells[rowIndex, 2].Value = "Điện năng phản kháng Nhận";
                  wookSheet.Cells[rowIndex, 7].Value = 0;
                  wookSheet.Cells[rowIndex, 8].Value = item.dienNangNhan.phanKhang * 0.001;
                  wookSheet.Cells[rowIndex, 11].Value = item.dienNangNhan.phanKhang * 0.001;
                  wookSheet.Cells[rowIndex, 12].Value = he_so_nhan;
                  wookSheet.Cells[rowIndex, 13].Value = item.dienNangNhan.phanKhang * 0.001 * he_so_nhan;
                  wookSheet.Cells[rowIndex, 15].Value = item.dienNangNhan.phanKhang * 0.001 * he_so_nhan;
                  rowIndex += 1;
               }

               FileResult result = new FileContentResult(package.GetAsByteArray(), GetContenTypeFile(fileDir));
               result.FileDownloadName = Path.GetFileNameWithoutExtension(fileDir) + Path.GetExtension(fileDir);
               return result;
            }

         }
         catch
         {
            return null;
         }
      }

      public static string GetContenTypeFile(string path)
      {
         try
         {
            var contentType = string.Empty;
            var extension = Path.GetExtension(path);
            switch (extension)
            {
               case ".xlsx":
                  contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                  break;
               case ".docx":
                  contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                  break;
               case ".xls":
                  contentType = "application/vnd.ms-excel";
                  break;
               case ".doc":
                  contentType = "application/msword";
                  break;
               case ".pdf":
                  contentType = "application/pdf";
                  break;
               case ".xml":
                  contentType = "application/xml";
                  break;
               case ".zip":
                  contentType = "application/zip";
                  break;
               case ".rar":
                  contentType = "application/octet-stream";
                  break;
               case ".csv":
                  contentType = "text/csv";
                  break;
               default:
                  contentType = "text/plain";
                  break;
            }
            return contentType;
         }
         catch
         {
            return "File không được hỗ trợ";
         }
      }

      public static string ToRoman(int number)
      {
         if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
         if (number < 1) return string.Empty;
         if (number >= 1000) return "M" + ToRoman(number - 1000);
         if (number >= 900) return "CM" + ToRoman(number - 900);
         if (number >= 500) return "D" + ToRoman(number - 500);
         if (number >= 400) return "CD" + ToRoman(number - 400);
         if (number >= 100) return "C" + ToRoman(number - 100);
         if (number >= 90) return "XC" + ToRoman(number - 90);
         if (number >= 50) return "L" + ToRoman(number - 50);
         if (number >= 40) return "XL" + ToRoman(number - 40);
         if (number >= 10) return "X" + ToRoman(number - 10);
         if (number >= 9) return "IX" + ToRoman(number - 9);
         if (number >= 5) return "V" + ToRoman(number - 5);
         if (number >= 4) return "IV" + ToRoman(number - 4);
         if (number >= 1) return "I" + ToRoman(number - 1);
         throw new ArgumentOutOfRangeException("something bad happened");
      }
      public static string GetDiemDo_CongToViewModel(int congto_id, int diemdo_id, out CapNhatDiemDo_CongToViewModel ct)
      {
         try
         {
            ct = new CapNhatDiemDo_CongToViewModel();
            using (var db = new Model1())
            {
               var rs = db.CongToes.Where(x => x.ID == congto_id).FirstOrDefault();
               if (rs == null)
               {
                  ct = null;
                  return "Không tìm thấy Công tơ !!!";
               }
               else
               {
                  var lk = db.DiemDo_CongTo.Where(x => x.CongToID == congto_id && x.DiemDoID == diemdo_id).FirstOrDefault();
                  return "success";
               }
            }
         }
         catch
         {
            ct = null;
            return "Đã có lỗi xảy ra khi lấy thông tin Công tơ";
         }
      }
      #endregion
   }

   public static class AccountDAO
   {
      public static string MaHoaMatKhau(string password)
      {
         //Tạo MD5 
         MD5 mh = MD5.Create();
         //Chuyển kiểu chuổi thành kiểu byte
         byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
         //mã hóa chuỗi đã chuyển
         byte[] hash = mh.ComputeHash(inputBytes);
         //tạo đối tượng StringBuilder (làm việc với kiểu dữ liệu lớn)
         string sb = string.Empty;
         for (int i = 0; i < hash.Length; i++)
         {
            sb += hash[i].ToString("x");
         }
         return sb;
      }

      private static string RandomSaltHash()
      {
         string rs = string.Empty;
         Random rd = new Random();
         for (int i = 0; i < 20; i++)
         {
            rs += Convert.ToString((char)rd.Next(65, 90));
         }
         return rs;
      }

      public static Account GetAccountByUsername(string username)
      {
         using (Model1 db = new Model1())
         {
            try
            {
               Account acc = db.Accounts.SingleOrDefault(x => x.Username == username);
               acc.RoleAccount = db.RoleAccounts.Find(acc.RoleID);
               return acc;
            }
            catch
            {
               throw new Exception("Có lỗi xảy ra khi lấy thông tin đăng nhập qua username");
            }
         }
      }

      public static Account GetAccountByID(int id)
      {
         using (Model1 db = new Model1())
         {
            try
            {
               Account acc = db.Accounts.Find(id);
               acc.RoleAccount = db.RoleAccounts.Find(acc.RoleID);
               return acc;
            }
            catch
            {
               throw new Exception("Có lỗi xảy ra khi lấy thông tin đăng nhập qua id");
            }
         }
      }

      public static bool CheckLogin(string username, string password)
      {
         using (Model1 db = new Model1())
         {
            try
            {
               var rs = db.Accounts.SingleOrDefault(x => x.Username == username);
               if (rs != null && rs.Password == MaHoaMatKhau(rs.SaltPassword + password))
               {
                  return true;
               }
            }
            catch
            {
               throw new Exception("Có lỗi xảy ra khi kiểm tra thông tin đăng nhập");
            }
            return false;
         }
      }

      public static int AddAccount(Account acc)
      {
         using (Model1 db = new Model1())
         {
            try
            {
               acc.SaltPassword = RandomSaltHash();
               // ma hoa mat khau
               acc.Password = MaHoaMatKhau(acc.SaltPassword + acc.Password);
               db.Accounts.Add(acc);
               db.SaveChanges();
               return acc.ID;
            }
            catch
            {
               return -1;
            }
         }
      }

      public static string ChangePassword(Account acc, string pass, string newPass)
      {
         using (Model1 db = new Model1())
         {
            var currentAcc = db.Accounts.SingleOrDefault(x => x.Username == acc.Username);
            if (currentAcc != null && currentAcc.Password == MaHoaMatKhau(currentAcc.SaltPassword + pass))
            {
               string newHash = RandomSaltHash();
               currentAcc.Password = MaHoaMatKhau(newHash + newPass);
               currentAcc.SaltPassword = newHash;
               db.SaveChanges();
               return "success";
            }
            else
            {
               return string.Empty;
            }
         }
      }

      public static bool CheckUsername(string username)
      {
         using (Model1 db = new Model1())
         {
            foreach (Account c in db.Accounts)
            {
               if (c.Username == username)
               {
                  return false;
               }
            }
            return true;
         }
      }

      public static void UpdateAccout(Account acc)
      {
         try
         {
            using (var db = new Model1())
            {
               var result = db.Accounts.SingleOrDefault(b => b.ID == acc.ID);
               if (result != null)
               {
                  if (!string.IsNullOrEmpty(acc.Avatar))
                  {
                     result.Avatar = acc.Avatar;
                  }
                  //check password current
                  if (acc.Password != result.Password)
                  {
                     result.SaltPassword = RandomSaltHash();
                     result.Password = MaHoaMatKhau(result.SaltPassword + acc.Password);
                  }
                  result.Fullname = acc.Fullname;
                  result.Phone = acc.Phone;
                  result.Email = acc.Email;
                  result.Address = acc.Address;
                  result.IdentifyCode = acc.IdentifyCode;
                  result.RoleID = acc.RoleID;
                  result.DOB = acc.DOB;
                  db.SaveChanges();
               }
            }
         }
         catch
         {
            throw new Exception("Lỗi update tài khoản");
         }
      }

   }
   public static class RoleAccountDAO
   {
      public static bool checkRoleName(string role)
      {
         using (var db = new Model1())
         {
            foreach (var i in db.RoleAccounts)
            {
               if (i.Role.ToLower().Equals(role.ToLower()))
               {
                  return false;
               }
            }
         }
         return true;
      }
      public static void UpdateRole(RoleAccount role)
      {
         try
         {
            using (var db = new Model1())
            {
               var result = db.RoleAccounts.SingleOrDefault(b => b.ID == role.ID);
               if (result != null)
               {
                  result.Role = role.Role;
                  result.PermissionID = role.PermissionID;
                  db.SaveChanges();
               }
            }
         }
         catch
         {
            throw new Exception("Lỗi update Vai trò");
         }
      }
      public static void InsertRole(RoleAccount role)
      {
         try
         {
            using (var db = new Model1())
            {
               if (checkRoleName(role.Role))
               {
                  db.RoleAccounts.Add(role);
                  db.SaveChanges();
               }
            }
         }
         catch
         {
            throw new Exception("Lỗi tạo mỡi vai trò");
         }
      }
      public static void Delete(int roleID)
      {
         try
         {
            using (var db = new Model1())
            {
               var result = db.RoleAccounts.SingleOrDefault(b => b.ID == roleID);
               if (result != null)
               {
                  db.RoleAccounts.Remove(result);
                  db.SaveChanges();
               }
            }
         }
         catch
         {
            throw new Exception("Không thể xóa vai trò");
         }
      }
   }

   public static class DiemDoDAO
   {
      public static List<DiemDo> getAllDiemDo()
      {
         var list = new List<DiemDo>();
         using (var db = new Model1())
         {
            list = db.DiemDoes.ToList();
         }
         return list;
      }
      public static bool GetDiemDoById(int id, out DiemDo dd)
      {
         dd = new DiemDo();
         try
         {
            using (Model1 db = new Model1())
            {
               dd = db.DiemDoes.Where(x => x.ID == id).FirstOrDefault();
               if (dd == null)
               {
                  return false;
               }
            }
         }
         catch
         {
            dd = null;
            return false;
         }
         return true;
      }
      public static bool GetDiemDoPaging(out PagingModel<DiemDoViewModel> pm, RequestPagingModel rpm, int? id_tcdd, bool allow_history)
      {
         try
         {
            pm = new PagingModel<DiemDoViewModel>();
            using (var db = new Model1())
            {
               //LogHelper.QueryInfo(dbContext);
               var list = from dd in db.DiemDoes
                          join lk in db.DiemDo_CongTo on dd.ID equals lk.DiemDoID into x
                          from lk in x.DefaultIfEmpty()
                          select new
                          {
                             diem_do = dd,
                             lien_ket = lk
                          }
                          into sub_list
                          join ct in db.CongToes on sub_list.lien_ket.CongToID equals ct.ID into y
                          from ct in y.DefaultIfEmpty()
                          select new
                          {
                             ID = sub_list.diem_do.ID,
                             MaDiemDo = sub_list.diem_do.MaDiemDo,
                             TenDiemDo = sub_list.diem_do.TenDiemDo,
                             CongToSerial = ct != null ? ct.Serial : string.Empty,
                             CongToID = ct != null ? ct.ID : -1,
                             TinhChat = sub_list.diem_do.TinhChatDiemDo.TenTinhChat,
                             TinhChatID = sub_list.diem_do.TinhChatDiemDo.ID,
                             ThuTuHienThi = sub_list.diem_do.ThuTuHienThi,
                             lienket = sub_list.lien_ket
                          };
               if (id_tcdd != null)
               {
                  list = list.Where(x => x.TinhChatID == id_tcdd);
               }
               if (!allow_history)
               {
                  list = list.Where(x => x.lienket == null || (x.lienket.ThoiGianBatDau <= DateTime.Now && (x.lienket.ThoiGianKetThuc >= DateTime.Now || x.lienket.ThoiGianKetThuc == null)));
               }
               pm.recordsFiltered = list.Count();
               //filter
               if (!string.IsNullOrEmpty(rpm.searchValue))
               {

                  list = list.Where(x => x.TenDiemDo.ToLower().Contains(rpm.searchValue.ToLower()) ||
                                        x.MaDiemDo.ToString().ToLower().Contains(rpm.searchValue.ToLower()) ||
                                        x.CongToSerial.ToString().ToLower().Contains(rpm.searchValue.ToLower()) ||
                                        x.TinhChat.ToString().ToLower().Contains(rpm.searchValue.ToLower()));

               }
               pm.recordsTotal = list.Count();
               //sorting
               if (!string.IsNullOrEmpty(rpm.sortColumnName))
               {
                  list = list.OrderBy(rpm.sortColumnName + " " + rpm.sortDirection);
               }
               //paging
               list = list.Skip(rpm.start).Take(rpm.length);
               pm.data = list.AsEnumerable().Select(x => new DiemDoViewModel()
               {
                  ID = x.ID,
                  MaDiemDo = x.MaDiemDo,
                  TenDiemDo = x.TenDiemDo,
                  CongToSerial = x.CongToSerial,
                  CongToID = x.CongToID,
                  TinhChat = x.TinhChat,
                  TinhChatID = x.TinhChatID,
                  ThuTuHienThi = x.ThuTuHienThi,
                  LienKetID = x.lienket != null ? x.lienket.ID : -1,
                  ThoiGianBatDau = x.lienket != null ? x.lienket.ThoiGianBatDau.ToString("dd/MM/yyyy") : string.Empty,
                  ThoiGianKetThuc = x.lienket != null ? (x.lienket.ThoiGianKetThuc != null ? x.lienket.ThoiGianKetThuc.Value.ToString("dd/MM/yyyy") : string.Empty) : string.Empty,
               }).ToList();
               pm.draw = int.Parse(rpm.draw);
            }
            return true;
         }
         catch
         {
            pm = new PagingModel<DiemDoViewModel>();
            return false;
         }
      }
      public static string CreateDiemDo(int MaDiemDo, string TenDiemDo, int ThuTuHienThi, int nha_may_id, int id_tinh_chat_diem_do)
      {
         try
         {
            using (var db = new Model1())
            {
               var check_exist = db.DiemDoes.Where(x => x.NhaMayID == nha_may_id && (x.MaDiemDo == MaDiemDo || x.TenDiemDo == TenDiemDo)).FirstOrDefault();
               if (check_exist != null)
               {
                  return "Điểm đo hoặc Tên điểm đo đã tồn tại !!!";
               }
               else
               {
                  DiemDo dd = new DiemDo();
                  dd.MaDiemDo = MaDiemDo;
                  dd.NhaMayID = nha_may_id;
                  dd.TenDiemDo = TenDiemDo;
                  dd.ThuTuHienThi = ThuTuHienThi;
                  dd.TinhChatID = id_tinh_chat_diem_do;
                  db.DiemDoes.Add(dd);
                  db.SaveChanges();
               }
               return "success";
            }
         }
         catch
         {
            return "Không thể truy cập cơ sở dữ liệu";
         }
      }

      public static string UpdateDiemDo(int MaDiemDo, string TenDiemDo, int ThuTuHienThi, int nha_may_id, int id_tinh_chat_diem_do, int id_diemdo)
      {
         try
         {
            using (var db = new Model1())
            {
               var list = db.DiemDoes.Where(x => x.NhaMayID == nha_may_id);
               var check_exist_ma = list.Where(x => x.MaDiemDo == MaDiemDo || x.TenDiemDo == TenDiemDo).FirstOrDefault();
               if (check_exist_ma != null && check_exist_ma.ID != id_diemdo)
               {
                  return "Mã Điểm đo hoặc Tên điểm đo tại nhà máy đã tồn tại";
               }
               var check_exist = list.Where(x => x.ID == id_diemdo).FirstOrDefault();
               if (check_exist == null)
               {
                  return "Không tìm thấy điểm đo";
               }

               check_exist.MaDiemDo = MaDiemDo;
               check_exist.TenDiemDo = TenDiemDo;
               check_exist.ThuTuHienThi = ThuTuHienThi;
               check_exist.NhaMayID = nha_may_id;
               check_exist.TinhChatID = id_tinh_chat_diem_do;
               db.SaveChanges();

               return "success";
            }
         }
         catch
         {
            return "Không thể truy cập cơ sử dữ liệu";
         }
      }
      /*public static string EditCongToDiemDo(CongTo ct)
      {
         try
         {

         }
         catch 
         {
            return "Đã có lỗi xảy ra khi thay đổi thông tin công tơ"
         }
      }
      public static string ChangeCongToDiemDo(CongTo ct)
      {

      }*/
   }
   public static class CongTyDAO
   {
      public static CongTy getCongTyById(int id)
      {
         CongTy ct = new CongTy();
         using (var db = new Model1())
         {
            ct = db.CongTies.SingleOrDefault(x => x.ID == id);
            if (ct == null)
            {
               return null;
            }
            return ct;
         }
      }

      public static CongTy getCongTyByDefault()
      {
         CongTy ct = new CongTy();
         using (var db = new Model1())
         {
            ct = db.CongTies.Select(x => x).FirstOrDefault();
            if (ct == null)
            {
               return null;
            }
            return ct;
         }
      }

      public static string updateCongTyInformation(CongTy ct)
      {
         using (var db = new Model1())
         {
            var rs = db.CongTies.SingleOrDefault(x => x.ID == ct.ID);
            if (rs == null)
            {
               return "Không tìm thấy thông tin công ty";
            }
            rs.Logo = ct.Logo;
            rs.TenVietTat = ct.TenVietTat;
            rs.TenCongTy = ct.TenCongTy;
            db.SaveChanges();
            return "success";
         }
      }
   }

   public static class SanLuongDuKienDAO
   {
      public static bool GetSanLuongDuKienPaging(out PagingModel<SanLuongDuKienViewModel> pm, RequestPagingModel rpm, int loai_SLDK, int? thang, int? nam)
      {
         try
         {
            pm = new PagingModel<SanLuongDuKienViewModel>();
            using (var db = new Model1())
            {
               //LogHelper.QueryInfo(dbContext);
               var list = db.SanLuongDuKiens.Select(x => x);


               // get data theo thang
               if (loai_SLDK == 1)
               {
                  if (thang.HasValue && nam.HasValue)
                  {
                     list = list.Where(x => x.LoaiID == loai_SLDK && x.ThoiGian.Year == nam && x.ThoiGian.Month == thang);
                  }
                  else if (thang.HasValue && !nam.HasValue)
                  {
                     list = list.Where(x => x.LoaiID == loai_SLDK && x.ThoiGian.Month == thang);
                  }
                  else if (!thang.HasValue && nam.HasValue)
                  {
                     list = list.Where(x => x.LoaiID == loai_SLDK && x.ThoiGian.Year == nam);
                  }
                  else
                  {
                     list = list.Where(x => x.LoaiID == loai_SLDK);
                  }

               }
               else
               {
                  // get data theo nam
                  if (nam.HasValue)
                  {
                     list = list.Where(x => x.LoaiID == loai_SLDK && x.ThoiGian.Year == nam);
                  }
                  else
                  {
                     list = list.Where(x => x.LoaiID == loai_SLDK);
                  }

               }

               pm.recordsFiltered = list.Count();
               //no filter
               //sorting
               if (!string.IsNullOrEmpty(rpm.sortColumnName))
               {
                  list = list.OrderBy(rpm.sortColumnName + " " + rpm.sortDirection);
               }
               pm.recordsTotal = list.Count();
               //paging
               list = list.Skip(rpm.start).Take(rpm.length);
               pm.data = list.AsEnumerable().Select(x => new SanLuongDuKienViewModel()
               {
                  ID = x.ID,
                  ThoiGian_Str = loai_SLDK == 1 ? x.ThoiGian.ToString("MM/yyyy") : x.ThoiGian.ToString("yyyy"),
                  SanLuong = x.SanLuong
               }).ToList();
               pm.draw = int.Parse(rpm.draw);
            }
            return true;
         }
         catch
         {
            pm = new PagingModel<SanLuongDuKienViewModel>();
            return false;
         }
      }
      public static bool GetSanLuongDuKienById(int id, out SanLuongDuKien sldk)
      {
         sldk = new SanLuongDuKien();
         try
         {
            using (Model1 db = new Model1())
            {
               sldk = db.SanLuongDuKiens.Where(x => x.ID == id).FirstOrDefault();
               if (sldk == null)
               {
                  return false;
               }
            }
         }
         catch
         {
            sldk = null;
            return false;
         }
         return true;
      }
      public static string CreateSanLuongDuKien(int loai_sldk, int thang, int nam, double giatri)
      {
         var sldk = new SanLuongDuKien();
         try
         {
            using (var db = new Model1())
            {
               //thang
               if (loai_sldk == 1)
               {
                  var rs = db.SanLuongDuKiens.Where(x => x.LoaiID == loai_sldk && x.ThoiGian.Month == thang && x.ThoiGian.Year == nam && x.ThoiGian.Day == 1).FirstOrDefault();
                  if (rs != null)
                  {
                     return "Bản ghi đã tồn tại";
                  }
                  else
                  {
                     sldk.LoaiID = loai_sldk;
                     sldk.ThoiGian = new DateTime(nam, thang, 1);
                     sldk.SanLuong = giatri;
                  }
               }
               else
               {
                  //nam
                  var rs = db.SanLuongDuKiens.Where(x => x.LoaiID == loai_sldk && x.ThoiGian.Month == 1 && x.ThoiGian.Year == nam && x.ThoiGian.Day == 1).FirstOrDefault();
                  if (rs != null)
                  {
                     return "Bản ghi đã tồn tại";
                  }
                  else
                  {
                     sldk.LoaiID = loai_sldk;
                     sldk.ThoiGian = new DateTime(nam, 1, 1);
                     sldk.SanLuong = giatri;
                  }
               }
               db.SanLuongDuKiens.Add(sldk);
               db.SaveChanges();
               return "success";
            }
         }
         catch
         {
            return "Đã xảy ra lỗi khi thêm mới Sản lượng dự kiến";
         }
      }

      public static string UpdateSanLuongDuKien(int id, double giatri)
      {
         var sldk = new SanLuongDuKien();
         try
         {
            using (var db = new Model1())
            {

               var rs = db.SanLuongDuKiens.Where(x => x.ID == id).FirstOrDefault();
               if (rs != null)
               {
                  rs.SanLuong = giatri;
               }
               else
               {
                  return "Bản ghi không tồn tại";

               }
               db.SaveChanges();
               return "success";
            }
         }
         catch
         {
            return "Đã xảy ra lỗi khi cập nhật Sản lượng dự kiến";
         }
      }
   }

   public static class NhaMayDAO
   {
      public static List<NhaMay> GetAllNhaMay()
      {
         using (var db = new Model1())
         {
            var list = db.NhaMays.ToList();
            return list;
         }
      }

      public static NhaMay GetNhaMayById(int id)
      {
         NhaMay nm = new NhaMay();
         using (var db = new Model1())
         {
            nm = db.NhaMays.Where(x => x.ID == id).FirstOrDefault();
            if (nm == null)
            {
               return null;
            }
            return nm;
         }
      }

      public static NhaMay GetNhaMayByDefault()
      {
         NhaMay nm = new NhaMay();
         using (var db = new Model1())
         {
            nm = db.NhaMays.Select(x => x).FirstOrDefault();
            if (nm == null)
            {
               return null;
            }
            return nm;
         }
      }
      public static string updateNhaMayInformation(NhaMay nm)
      {
         try
         {
            using (var db = new Model1())
            {
               var rs = db.NhaMays.SingleOrDefault(x => x.ID == nm.ID);
               if (rs == null)
               {
                  return "Không tìm thấy thông tin Nhà máy";
               }
               rs.TenVietTat = nm.TenVietTat;
               rs.TenNhaMay = nm.TenNhaMay;
               rs.DiaChi = nm.DiaChi;
               db.SaveChanges();
               return "success";
            }
         }
         catch
         {
            return "Đã có lỗi xảy ra trong quá trình cập nhật!!";
         }
      }
   }
   public static class TinhChatDiemDoDAO
   {
      public static List<TinhChatDiemDo> GetAllTCDD()
      {
         using (var db = new Model1())
         {
            var list = db.TinhChatDiemDoes.ToList();
            return list;
         }
      }
   }

   public static class CongToDAO
   {
      public static CongTo GetCongToByID(int congto_id)
      {
         try
         {
            using (var db = new Model1())
            {
               return db.CongToes.Where(x => x.ID == congto_id).FirstOrDefault();
            }
         }
         catch
         {
            throw new Exception("Đã có lỗi xảy ra khi lấy thông tin Công tơ");
         }
      }
      public static bool CheckCongToExistBySerial(string serial)
      {
         try
         {
            using (var db = new Model1())
            {
               var rs = db.CongToes.Where(x => x.Serial == serial).FirstOrDefault();
               if (rs == null)
               {
                  return false;
               }
               else
               {
                  return true;
               }
            }
         }
         catch
         {
            throw new Exception("Đã có lỗi xảy ra khi kiểm tra công tơ");
         }
      }
      public static bool CheckCongToExistBySerialExceptOneID(string serial, int id)
      {
         try
         {
            using (var db = new Model1())
            {
               var rs = db.CongToes.Where(x => x.Serial == serial && x.ID != id).FirstOrDefault();
               if (rs == null)
               {
                  return false;
               }
               else
               {
                  return true;
               }
            }
         }
         catch
         {
            throw new Exception("Đã có lỗi xảy ra khi kiểm tra công tơ");
         }
      }

      public static string GetCongToTheoDiemDoByCongToID(int congto_id, int diemdo_id, out CapNhatDiemDo_CongToViewModel ct)
      {
         try
         {
            ct = new CapNhatDiemDo_CongToViewModel();
            using (var db = new Model1())
            {
               var rs = db.CongToes.Where(x => x.ID == congto_id).FirstOrDefault();
               if (rs == null)
               {
                  ct = null;
                  return "Không tìm thấy Công tơ !!!";
               }
               else
               {
                  var lk = db.DiemDo_CongTo.Where(x => x.DiemDoID == diemdo_id && x.CongToID == congto_id).FirstOrDefault();
                  if (lk == null)
                  {
                     ct = null;
                     return "Điểm đo không có công tơ";
                  }
                  ct.CongToID = lk.CongTo.ID;
                  ct.Serial = lk.CongTo.Serial;
                  ct.DiemDoID = lk.DiemDoID;
                  ct.ThoiGianBatDau = lk.ThoiGianBatDau;
                  ct.ThoiGianKetThuc = lk.ThoiGianKetThuc;
                  ct.Type = lk.CongTo.Type;
                  return "success";
               }
            }
         }
         catch
         {
            ct = null;
            return "Đã có lỗi xảy ra khi lấy thông tin Công tơ";
         }
      }
      public static bool CreateCongTo(ref CongTo ct)
      {
         try
         {
            string serial = ct.Serial;
            using (var db = new Model1())
            {
               var rs = db.CongToes.Where(x => x.Serial.ToLower() == serial.ToLower()).FirstOrDefault();
               if (rs != null)
               {
                  return false;
               }
               else
               {
                  db.CongToes.Add(ct);
                  db.SaveChanges();
                  return true;
               }
            }
         }
         catch
         {
            throw new Exception("Đã có lỗi xảy ra khi thêm mới công tơ");
         }
      }
      public static CongTo GetCongToBySerial(string serial)
      {
         try
         {
            using (var db = new Model1())
            {
               return db.CongToes.Where(x => x.Serial == serial).FirstOrDefault();
            }
         }
         catch
         {
            throw new Exception("Đã có lỗi xảy ra khi lấy thông tin Công tơ");
         }
      }
      public static bool UpdateCongTo(CongTo ct)
      {
         try
         {
            using (var db = new Model1())
            {
               var rs = CongToDAO.GetCongToByID(ct.ID);
               if (rs == null)
               {
                  throw new Exception("Không tìm thấy công tơ");
               }
               rs.Serial = ct.Serial;
               rs.Type = ct.Type;
               db.SaveChanges();
               return true;
            }
         }
         catch
         {
            throw new Exception("Đã có lỗi xảy ra khi update công tơ");
         }
      }

   }


   public static class LienKetDiemDoCongToDAO
   {
      public static DiemDo_CongTo GetLienKetById(int id_lienket)
      {
         try
         {
            using (var db = new Model1())
            {
               return db.DiemDo_CongTo.Find(id_lienket);
            }
         }
         catch
         {
            throw new Exception("Đã có lỗi xảy ra khi lấy thông tin liên kết điểm đo - công tơ");
         }
      }
      public static bool CreateLienKet(int congto_id, int diemdo_id, DateTime start, DateTime? end)
      {
         try
         {
            using (var db = new Model1())
            {
               DiemDo_CongTo dd_ct = new DiemDo_CongTo();
               dd_ct.DiemDoID = diemdo_id;
               dd_ct.CongToID = congto_id;
               dd_ct.ThoiGianBatDau = start;
               dd_ct.ThoiGianKetThuc = end;
               db.DiemDo_CongTo.Add(dd_ct);
               db.SaveChanges();
               return true;
            }
         }
         catch
         {
            throw new Exception("Đã có lỗi xảy ra khi tạo liên kết điểm đo - công tơ");
         }
      }
      public static bool CheckCongToUsingFollowTimeByID(int id_congto, int id_lienket, DateTime dt_start, DateTime? dt_end)
      {

         try
         {
            using (var db = new Model1())
            {
               CongTo ct = CongToDAO.GetCongToByID(id_congto);
               if (ct == null)
               {
                  throw new Exception("Không tìm thấy công tơ");
               }
               else
               {
                  var list_lienket = db.DiemDo_CongTo.Where(x => x.CongToID == id_congto && x.ID != id_lienket).ToList();
                  foreach (var item in list_lienket)
                  {
                     if (CheckTimeDuplicate.CheckTwoPeriodsIsDuplicate(item.ThoiGianBatDau, item.ThoiGianKetThuc, dt_start, dt_end))
                     {
                        return true;
                     }
                  }
               }
               return false;
            }
         }
         catch
         {
            throw new Exception("Đã có lỗi xảy ra khi kiểm tra công tơ");
         }
      }
      public static bool CapNhatThoiGian(int lienket_id, DateTime start, DateTime? end)
      {
         try
         {
            using (var db = new Model1())
            {
               var rs = db.DiemDo_CongTo.Find(lienket_id);
               if (rs == null)
               {
                  throw new Exception();
               }
               rs.ThoiGianBatDau = start;
               rs.ThoiGianKetThuc = end;
               db.SaveChanges();
               return true;
            }
         }
         catch
         {
            throw new Exception("Đã có lỗi xảy ra khi cập nhật liên kết điểm đo - công tơ");
         }
      }
   }


}