﻿using System.Data.Entity;
using DuAn.Models.CustomModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAn.COMMON;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using OfficeOpenXml;
using DuAn.Models.DbModel;
using System.Globalization;
using iTextSharp.text.pdf.qrcode;

namespace DuAn
{
    public class DBContext
    {
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
                    var thucteThangg = db.TongSanLuong_ThangNam.Where(x => x.Ngay <= date).OrderByDescending(x=>x.Ngay).Take(1).Select(x => x.GiaTriThang).FirstOrDefault();
                    var thucTeNam = db.TongSanLuong_ThangNam.Where(x => x.Ngay <= date).OrderByDescending(x => x.Ngay).Take(1).Select(x => x.GiaTriNam).FirstOrDefault();
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
                    var giaTien = db.GiaDiens.Where(x => x.NgayBatDau <= date && x.NgayKetThuc >= date).Select(x => x.Gia).FirstOrDefault();
                    var result = new HomeModel
                    {
                        duKienThang = db.SanLuongDuKiens.Where(x => x.LoaiID == CommonContext.LOAI_SAN_LUONG_THANG && x.ThoiGian == thang).Select(x => x.SanLuong).FirstOrDefault(),
                        duKienNam = db.SanLuongDuKiens.Where(x => x.LoaiID == CommonContext.LOAI_SAN_LUONG_NAM && x.ThoiGian == nam).Select(x => x.SanLuong).FirstOrDefault(),
                        thucTeThang = thucteThangg,
                        thucTeNam = thucTeNam,
                        data = list,
                        sanLuongTrongNgay = dataTrongNgay,
                        date = date,
                        countMissingData = missingDataCount,
                        giaDien = giaTien,
                        doanhThuThang=getDoanhThuThang(date).Sum(x=>x.doanhThu),
                        doanhThuNam=getDoanhThuNam(date).Sum(x=>x.doanhThu)

                    };
                    return await Task.FromResult(result);
                }
                catch (Exception ex)
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
                    giaDien = giaTien
                };
            }
        }
        public static Account ChangeInfo(HttpPostedFileBase avatar, string fullname, string email, string address, string phone, string dob, string id)
        {
            using (var db = new Model1())
            {
                int inInt = int.Parse(id);
                var acc = db.Accounts.Include(x => x.RoleAccount).FirstOrDefault(x => x.ID == inInt);
                try
                {
                    if (avatar != null)
                    {
                        string fileName = System.IO.Path.GetFileName(avatar.FileName);
                        string path_avatar = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory.ToString()) + "\\images\\avatarAccount\\" + fileName;
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
                catch (Exception ex)
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
                    DateTime dateEnd = new DateTime(date.Year, date.Month + 1, 1);
                    var result = new List<TongSanLuongTheoThoiGian>();
                    for (DateTime dateFor = dateStart; dateFor < dateEnd; dateFor = dateFor.AddDays(1))
                    {
                        var gia = db.GiaDiens.Where(x => x.NgayBatDau <= dateFor && x.NgayKetThuc >= dateFor).Select(x => x.Gia).FirstOrDefault();
                        var value = db.TongSanLuong_Ngay.Where(x => x.Ngay == dateFor).ToList().Count == 0 ? 0 : db.TongSanLuong_Ngay.Where(x => x.Ngay == dateFor).Sum(x => x.GiaTri);
                        result.Add(new TongSanLuongTheoThoiGian
                        {
                            date = dateFor,
                            giaTien = gia,
                            value = value,
                            doanhThu = gia * value
                        });
                    }
                    result.RemoveAll(x => x.value == 0 || x.value == 0 || x.giaTien == 0);
                    return result;
                }
                catch(Exception e)
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
                    DateTime dateEnd = new DateTime(date.Year, date.Month + 1, 1);
                    var result = db.TongSanLuong_Ngay.Where(x => x.Ngay >= dateStart && x.Ngay < dateEnd).GroupBy(l => l.Ngay).Select(cl => new TongSanLuongTheoThoiGian
                    {
                        date = cl.Key,
                        value = cl.Sum(c => c.GiaTri),
                    }).OrderBy(x => x.date).ToList();
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
                    DateTime dateEnd = new DateTime(date.Year + 1, 1, 1);
                    var result = new List<TongSanLuongTheoThoiGian>();
                    for (DateTime month = dateStart; month < dateEnd; month = month.AddMonths(1))
                    {
                        var addOneMonth = month.AddMonths(1);
                        result.Add(
                            db.TongSanLuong_ThangNam.Where(x => x.Ngay >= month && x.Ngay < addOneMonth).OrderByDescending(x => x.Ngay).Select(
                                x => new TongSanLuongTheoThoiGian
                                {
                                    date = month,
                                    value = x.GiaTriThang
                                }
                                ).Take(1).FirstOrDefault());
                    }
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
                    for (DateTime month = dateStart; month < dateEnd; month = month.AddMonths(1))
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
                                doanhThu=gia*value
                            });
                        }
                        result.Add(new TongSanLuongTheoThoiGian
                        {
                            date = month,
                            giaTien=list.Where(x=>x.giaTien!=0&&x.date>=dateStart&&x.date<dateEnd).Select(x=>x.giaTien).FirstOrDefault(),
                            value = list.Sum(x => x.value),
                            doanhThu=list.Sum(x=>x.doanhThu)
                        });
                    }
                    result.RemoveAll(x => x.doanhThu == 0);
                    return result;
                }
                catch
                {
                    return null;
                }
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
                if (id != "")
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
                    getLastDate = db.CongThucTongSanLuongs.Count() != 0 ? db.CongThucTongSanLuongs.OrderByDescending(x => x.ThoiGianKetThuc).Select(x => x.ThoiGianKetThuc).First().AddDays(1) : DateTime.MinValue.AddMonths(1),
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
                    batDau = db.CongThucTongSanLuongs.OrderByDescending(x => x.ThoiGianKetThuc).Select(x => x.ThoiGianKetThuc).First().AddDays(1);
                }
                var item = new CongThucTongSanLuong()
                {
                    Ten = name,
                    CongThuc = formular,
                    ThoiGianBatDau = batDau,
                    ThoiGianKetThuc = DateTime.ParseExact(thoiGian, "dd/MM/yyyy", null)
                };
                db.CongThucTongSanLuongs.Add(item);
                db.SaveChanges();
            }
        }
        public static string InsertGiaDien(string thoiGianBatDau, string thoiGianKetThuc, int giaDien)
        {
            using (var db = new Model1())
            {
                try
                {
                    var item = new GiaDien()
                    {
                        NgayBatDau = DateTime.ParseExact(thoiGianBatDau, "dd/MM/yyyy", null),
                        NgayKetThuc = DateTime.ParseExact(thoiGianKetThuc, "dd/MM/yyyy", null),
                        Gia = giaDien
                    };
                    db.GiaDiens.Add(item);
                    db.SaveChanges();
                    return "";
                }
                catch(Exception e)
                {
                    return "Error";
                }
            }
        }
        static List<MissingDataStatus> data = new List<MissingDataStatus>();
        static DateTime startDay = DateTime.MinValue;
        static DateTime endDay = DateTime.MaxValue;
        public static List<MissingDataStatus> getMissingData(string name = "")
        {
            using (var db = new Model1())
            {
                if (data.Count() == 0 && name.Length == 0)
                {
                    startDay = db.SanLuongs.Min(x => x.Ngay);
                    endDay = DateTime.Now.AddDays(-1);
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
                        return getMissingData();
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
                        wookSheet.Cells[rowIndex, 8].Value = item.dienNangGiao.bieuTong;
                        wookSheet.Cells[rowIndex, 11].Value = item.dienNangGiao.bieuTong;
                        wookSheet.Cells[rowIndex, 12].Value = 1000;
                        wookSheet.Cells[rowIndex, 13].Value = item.dienNangGiao.bieuTong * 1000;
                        wookSheet.Cells[rowIndex, 15].Value = item.dienNangGiao.bieuTong * 1000;
                        rowIndex += 1;
                        wookSheet.Cells[rowIndex, 1].Value = "b";
                        wookSheet.Cells[rowIndex, 2].Value = "Điện năng tác dụng giao - Biểu 1";
                        wookSheet.Cells[rowIndex, 7].Value = 0;
                        wookSheet.Cells[rowIndex, 8].Value = item.dienNangGiao.bieu1;
                        wookSheet.Cells[rowIndex, 11].Value = item.dienNangGiao.bieu1;
                        wookSheet.Cells[rowIndex, 12].Value = 1000;
                        wookSheet.Cells[rowIndex, 13].Value = item.dienNangGiao.bieu1 * 1000;
                        wookSheet.Cells[rowIndex, 15].Value = item.dienNangGiao.bieu1 * 1000;
                        rowIndex += 1;
                        wookSheet.Cells[rowIndex, 1].Value = "c";
                        wookSheet.Cells[rowIndex, 2].Value = "Điện năng tác dụng giao - Biểu 2";
                        wookSheet.Cells[rowIndex, 7].Value = 0;
                        wookSheet.Cells[rowIndex, 8].Value = item.dienNangGiao.bieu2;
                        wookSheet.Cells[rowIndex, 11].Value = item.dienNangGiao.bieu2;
                        wookSheet.Cells[rowIndex, 12].Value = 1000;
                        wookSheet.Cells[rowIndex, 13].Value = item.dienNangGiao.bieu2 * 1000;
                        wookSheet.Cells[rowIndex, 15].Value = item.dienNangGiao.bieu2 * 1000;
                        rowIndex += 1;
                        wookSheet.Cells[rowIndex, 1].Value = "d";
                        wookSheet.Cells[rowIndex, 2].Value = "Điện năng tác dụng giao - Biểu 3";
                        wookSheet.Cells[rowIndex, 7].Value = 0;
                        wookSheet.Cells[rowIndex, 8].Value = item.dienNangGiao.bieu3;
                        wookSheet.Cells[rowIndex, 11].Value = item.dienNangGiao.bieu3;
                        wookSheet.Cells[rowIndex, 12].Value = 1000;
                        wookSheet.Cells[rowIndex, 13].Value = item.dienNangGiao.bieu3 * 1000;
                        wookSheet.Cells[rowIndex, 15].Value = item.dienNangGiao.bieu3 * 1000;
                        rowIndex += 1;
                        wookSheet.Cells[rowIndex, 1].Value = "e";
                        wookSheet.Cells[rowIndex, 2].Value = "Điện năng phản kháng Giao";
                        wookSheet.Cells[rowIndex, 7].Value = 0;
                        wookSheet.Cells[rowIndex, 8].Value = item.dienNangGiao.phanKhang;
                        wookSheet.Cells[rowIndex, 11].Value = item.dienNangGiao.phanKhang;
                        wookSheet.Cells[rowIndex, 12].Value = 1000;
                        wookSheet.Cells[rowIndex, 13].Value = item.dienNangGiao.phanKhang * 1000;
                        wookSheet.Cells[rowIndex, 15].Value = item.dienNangGiao.phanKhang * 1000;

                        //============================================================================
                        rowIndex += 1;
                        wookSheet.Row(rowIndex).Style.Font.Bold = true;
                        wookSheet.Cells[rowIndex, 1].Value = 2;
                        wookSheet.Cells[rowIndex, 2].Value = "Điện năng nhận";
                        rowIndex += 1;
                        wookSheet.Cells[rowIndex, 1].Value = "a";
                        wookSheet.Cells[rowIndex, 2].Value = "Điện năng tác dụng nhận - Biểu tổng";
                        wookSheet.Cells[rowIndex, 7].Value = 0;
                        wookSheet.Cells[rowIndex, 8].Value = item.dienNangNhan.bieuTong;
                        wookSheet.Cells[rowIndex, 11].Value = item.dienNangNhan.bieuTong;
                        wookSheet.Cells[rowIndex, 12].Value = 1000;
                        wookSheet.Cells[rowIndex, 13].Value = item.dienNangNhan.bieuTong * 1000;
                        wookSheet.Cells[rowIndex, 15].Value = item.dienNangNhan.bieuTong * 1000;
                        rowIndex += 1;
                        wookSheet.Cells[rowIndex, 1].Value = "b";
                        wookSheet.Cells[rowIndex, 2].Value = "Điện năng tác dụng nhận - Biểu 1";
                        wookSheet.Cells[rowIndex, 7].Value = 0;
                        wookSheet.Cells[rowIndex, 8].Value = item.dienNangNhan.bieu1;
                        wookSheet.Cells[rowIndex, 11].Value = item.dienNangNhan.bieu1;
                        wookSheet.Cells[rowIndex, 12].Value = 1000;
                        wookSheet.Cells[rowIndex, 13].Value = item.dienNangNhan.bieu1 * 1000;
                        wookSheet.Cells[rowIndex, 15].Value = item.dienNangNhan.bieu1 * 1000;
                        rowIndex += 1;
                        wookSheet.Cells[rowIndex, 1].Value = "c";
                        wookSheet.Cells[rowIndex, 2].Value = "Điện năng tác dụng nhận - Biểu 2";
                        wookSheet.Cells[rowIndex, 7].Value = 0;
                        wookSheet.Cells[rowIndex, 8].Value = item.dienNangNhan.bieu2;
                        wookSheet.Cells[rowIndex, 11].Value = item.dienNangNhan.bieu2;
                        wookSheet.Cells[rowIndex, 12].Value = 1000;
                        wookSheet.Cells[rowIndex, 13].Value = item.dienNangNhan.bieu2 * 1000;
                        wookSheet.Cells[rowIndex, 15].Value = item.dienNangNhan.bieu2 * 1000;
                        rowIndex += 1;
                        wookSheet.Cells[rowIndex, 1].Value = "d";
                        wookSheet.Cells[rowIndex, 2].Value = "Điện năng tác dụng nhận - Biểu 3";
                        wookSheet.Cells[rowIndex, 7].Value = 0;
                        wookSheet.Cells[rowIndex, 8].Value = item.dienNangNhan.bieu3;
                        wookSheet.Cells[rowIndex, 11].Value = item.dienNangNhan.bieu3;
                        wookSheet.Cells[rowIndex, 12].Value = 1000;
                        wookSheet.Cells[rowIndex, 13].Value = item.dienNangNhan.bieu3 * 1000;
                        wookSheet.Cells[rowIndex, 15].Value = item.dienNangNhan.bieu3 * 1000;
                        rowIndex += 1;
                        wookSheet.Cells[rowIndex, 1].Value = "e";
                        wookSheet.Cells[rowIndex, 2].Value = "Điện năng phản kháng Nhận";
                        wookSheet.Cells[rowIndex, 7].Value = 0;
                        wookSheet.Cells[rowIndex, 8].Value = item.dienNangNhan.phanKhang;
                        wookSheet.Cells[rowIndex, 11].Value = item.dienNangNhan.phanKhang;
                        wookSheet.Cells[rowIndex, 12].Value = 1000;
                        wookSheet.Cells[rowIndex, 13].Value = item.dienNangNhan.phanKhang * 1000;
                        wookSheet.Cells[rowIndex, 15].Value = item.dienNangNhan.phanKhang * 1000;
                        rowIndex += 1;
                    }

                    FileResult result = new FileContentResult(package.GetAsByteArray(), GetContenTypeFile(fileDir));
                    result.FileDownloadName = Path.GetFileNameWithoutExtension(fileDir) + Path.GetExtension(fileDir);
                    return result;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string GetContenTypeFile(string path)
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
    }


    public static class AccountDAO
    {
        public static string MaHoaMatKhau(String password)
        {
            //Tạo MD5 
            MD5 mh = MD5.Create();
            //Chuyển kiểu chuổi thành kiểu byte
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
            //mã hóa chuỗi đã chuyển
            byte[] hash = mh.ComputeHash(inputBytes);
            //tạo đối tượng StringBuilder (làm việc với kiểu dữ liệu lớn)
            String sb = "";
            for (int i = 0; i < hash.Length; i++)
            {
                sb += hash[i].ToString("x");
            }
            return sb;
        }

        private static string RandomSaltHash()
        {
            string rs = "";
            Random rd = new Random();
            for (int i = 0; i < 20; i++)
            {
                rs += Convert.ToString((Char)rd.Next(65, 90));
            }
            return rs;
        }
        public static Account CheckLogin(string username, string password)
        {
            using (Model1 db = new Model1())
            {
                try
                {
                    var rs = db.Accounts.SingleOrDefault(x => x.Username == username);
                    if (rs != null && rs.Password == MaHoaMatKhau(rs.SaltPassword + password))
                    {
                        return rs;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return null;
            }
        }
        public static void AddAccount(Account acc)
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
                }
                catch (Exception ex)
                {
                    throw ex;
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
                    return "";
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
            catch (Exception ex)
            {
                throw ex;
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
            catch (Exception ex)
            {
                throw ex;
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
            catch (Exception ex)
            {
                throw ex;
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
            catch (Exception ex)
            {
                throw ex;
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
    }

}