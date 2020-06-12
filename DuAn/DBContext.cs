using DuAn.Models;
using DuAn.Models.CustomModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAn.COMMON;

namespace DuAn
{
    public class DBContext
    {
        public static HomeModel getDuKien()
        {
            using (var db = new Model1())
            {
                DateTime thang = new DateTime(2020, 4, 1);
                DateTime thangEnd = new DateTime(2020, 5, 1);
                DateTime nam = new DateTime(2020, 1, 1);
                DateTime namEnd = new DateTime(2021, 1, 1);
                DateTime sanLuong = new DateTime(2020, 4, 18);
                List<DiemDoData> list = new List<DiemDoData>();
                var temp1 = db.SanLuongs.Where(x => x.Ngay == sanLuong).OrderBy(x => x.DiemDo.TinhChatDiemDo.STT).ToList();
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
                        thuTuHienThi = listDiemDo.Select(x => x.TinhChatDiemDo.STT).FirstOrDefault(),
                        kwhGiao = listSanLuong.Where(x => x.KenhID == CommonContext.KWH_GIAO).Select(x => x.GiaTri).ToList(),
                        kwhNhan = listSanLuong.Where(x => x.KenhID == CommonContext.KWH_NHAN).Select(x => x.GiaTri).ToList(),
                        kvarhGiao = listSanLuong.Where(x => x.KenhID == CommonContext.KVARH_GIAO).Select(x => x.GiaTri).ToList(),
                        kvarhNhan = listSanLuong.Where(x => x.KenhID == CommonContext.KVARH_NHAN).Select(x => x.GiaTri).ToList()
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
                    sanLuongTrongNgay = sumChuKy
                };
                return result;
            }
        }
    }
}