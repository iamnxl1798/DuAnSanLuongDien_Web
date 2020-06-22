using DuAn.Models;
using DuAn.Models.CustomModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAn.COMMON;
using System.IO;

namespace DuAn
{
    public class DBContext
    {
        public static HomeModel getDuKien()
        {
            using (var db = new Model1())
            {
                try
                {
                    DateTime thang = new DateTime(2020, 4, 1);
                    DateTime thangEnd = new DateTime(2020, 5, 1);
                    DateTime nam = new DateTime(2020, 1, 1);
                    DateTime namEnd = new DateTime(2021, 1, 1);
                    DateTime sanLuong = new DateTime(2020, 4, 18);
                    List<DiemDoData> list = new List<DiemDoData>();
                    var temp1 = db.SanLuongs.Where(x => x.Ngay == sanLuong).OrderBy(x => x.DiemDo.ThuTuHienThu).ToList();
                    List<int> temp = temp1.Select(x => x.DiemDoID).Distinct().ToList();
                    foreach (int itemp in temp)
                    {
                        var listDiemDo = db.DiemDoes.Where(x => x.ID == itemp);
                        var listSanLuong = db.SanLuongs.Where(x => x.DiemDoID == itemp && x.Ngay == sanLuong);
                        list.Add(new DiemDoData()
                        {
                            tenDiemDo = listDiemDo.Select(x => x.TenDiemDo).FirstOrDefault(),
                            maDiemDo = listDiemDo.Select(x => x.MaDiemDo).FirstOrDefault(),
                            tinhChat = listDiemDo.Select(x => x.TinhChatDiemDo.TenTinhChat).FirstOrDefault(),
                            thuTuHienThi = listDiemDo.Select(x => x.ThuTuHienThu).FirstOrDefault(),
                            sumKwhGiao = listSanLuong.Where(x => x.KenhID == CommonContext.KWH_GIAO).Select(x => x.GiaTri).Sum(),
                            sumKwhNhan = listSanLuong.Where(x => x.KenhID == CommonContext.KWH_NHAN).Select(x => x.GiaTri).Sum(),
                            sumKvarhGiao = listSanLuong.Where(x => x.KenhID == CommonContext.KVARH_GIAO).Select(x => x.GiaTri).Sum(),
                            sumKvarhNhan = listSanLuong.Where(x => x.KenhID == CommonContext.KVARH_NHAN).Select(x => x.GiaTri).Sum()
                        });
                    }
                    var dataTrongNgay = db.SanLuongs.Where(x => x.Ngay == sanLuong);
                    var ChuKy = dataTrongNgay.Where(x => x.KenhID == 2).GroupBy(x => x.ChuKy).ToList();
                    var sumChuKy = ChuKy.Select(x => { var item = x.First(); return new SanLuong { ChuKy = item.ChuKy, GiaTri = x.Sum(e => e.GiaTri) }; }).ToList();
                    var result = new HomeModel()
                    {
                        duKienThang = db.SanLuongDuKiens.Where(x => x.LoaiID == CommonContext.LOAI_SAN_LUONG_THANG && x.ThoiGian == thang).Select(x => x.SanLuong).FirstOrDefault(),
                        duKienNam = db.SanLuongDuKiens.Where(x => x.LoaiID == CommonContext.LOAI_SAN_LUONG_NAM && x.ThoiGian == nam).Select(x => x.SanLuong).FirstOrDefault(),
                        thucTeThang = db.SanLuongThucTes.Where(x => x.Ngay == thang).Select(x => x.SanLuong).Sum(),
                        thucTeNam = db.SanLuongThucTes.Where(x => x.Ngay == nam).Select(x => x.SanLuong).Sum(),
                        data = list,
                        sanLuongTrongNgay = sumChuKy,
                    };
                    return result;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static DiemDoData getChiTietDiemDo(int id)
        {
            using (var db = new Model1())
            {
                try
                {
                    DateTime sanLuongDate = new DateTime(2020, 4, 18);
                    var result = db.SanLuongs.Where(x => x.DiemDo.MaDiemDo == id && x.Ngay == sanLuongDate);
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
        public static AdminModel getDataAdminModel()
        {
            using (var db = new Model1())
            {
                var result = new AdminModel()
                {
                    listDiemDo = db.DiemDoes.ToList(),
                    listKenh = db.Kenhs.ToList(),
                    getLastDate = db.CongThucTongSanLuongs.Count() != 0 ? db.CongThucTongSanLuongs.OrderByDescending(x => x.ThoiGianKetThuc).Select(x => x.ThoiGianKetThuc).First().AddDays(1) : DateTime.MinValue.AddMonths(1),
                    missingData = getMissingData()
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

        public static List<MissingDataStatus> getMissingData()
        {
            using (var db = new Model1())
            {
                DateTime startDay = db.SanLuongs.Min(x => x.Ngay);
                DateTime endDay = db.SanLuongs.Max(x => x.Ngay);
                List<MissingDataStatus> data = new List<MissingDataStatus>();
                var listDiemDo = db.DiemDoes;
                for (DateTime date = startDay; date <= endDay; date = date.AddDays(1))
                {
                    foreach (DiemDo item in listDiemDo)
                    {
                        if (db.SanLuongs.Where(x => x.Ngay == date).Where(x => x.DiemDoID == item.ID).Count() == 0)
                        {
                            string fileName = date.Day.ToString("00") + date.Month.ToString("00") + (date.Year % 10).ToString() + item.ID.ToString() + ".CSV";
                            var pathString = new DirectoryInfo(string.Format("{0}images\\{1}", AppDomain.CurrentDomain.BaseDirectory.ToString(), fileName)).ToString();
                            if (File.Exists(pathString))
                            {
                                data.Add(new MissingDataStatus()
                                {
                                    date = date.ToString("dd/MM/yyyy"),
                                    name = item.TenDiemDo,
                                    status = "Đang cập nhật"
                                });
                            }
                            else
                            {
                                data.Add(new MissingDataStatus()
                                {
                                    date = date.ToString("dd/MM/yyyy"),
                                    name = item.TenDiemDo,
                                    status = "Chưa cập nhật"
                                });
                            }
                        }
                    }
                }
                return data;
            }
        }
    }
}