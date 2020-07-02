using DuAn.Models;
using DuAn.Models.CustomModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAn.COMMON;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using DocumentFormat.OpenXml.Drawing.Charts;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using DocumentFormat.OpenXml.Office.CustomUI;
using System.Threading.Tasks;

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
                    var temp1 = db.SanLuongs.Where(x => x.Ngay == date).OrderBy(x => x.DiemDo.ThuTuHienThu).ToList();
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
                            thuTuHienThi = (listDiemDo?.ThuTuHienThu).GetValueOrDefault(),
                            sumKwhGiao = listSanLuong.Where(x => x.KenhID == CommonContext.KVARH_GIAO).Select(x => x.GiaTri).Sum(),
                            sumKwhNhan = listSanLuong.Where(x => x.KenhID == CommonContext.KWH_NHAN).Select(x => x.GiaTri).Sum(),
                            sumKvarhGiao = listSanLuong.Where(x => x.KenhID == CommonContext.KVARH_GIAO).Select(x => x.GiaTri).Sum(),
                            sumKvarhNhan = listSanLuong.Where(x => x.KenhID == CommonContext.KVARH_NHAN).Select(x => x.GiaTri).Sum()
                        });
                    }
                    db.Configuration.LazyLoadingEnabled = false;
                    var dataTrongNgay = db.TongSanLuong_Ngay.Where(x => x.Ngay == date).ToList();
                    var thucteThangg = db.TongSanLuong_Thang.Where(x => x.Thang == date.Month && x.Nam == date.Year).Select(x => x.GiaTri).FirstOrDefault().GetValueOrDefault();
                    var thucTeNam = db.TongSanLuong_Nam.Where(x => x.Nam == date.Year).Select(x => x.GiaTri).FirstOrDefault().GetValueOrDefault();
                    var result = new HomeModel
                    {
                        duKienThang = db.SanLuongDuKiens.Where(x => x.LoaiID == CommonContext.LOAI_SAN_LUONG_THANG && x.ThoiGian == thang).Select(x => x.SanLuong).FirstOrDefault(),
                        duKienNam = db.SanLuongDuKiens.Where(x => x.LoaiID == CommonContext.LOAI_SAN_LUONG_NAM && x.ThoiGian == nam).Select(x => x.SanLuong).FirstOrDefault(),
                        thucTeThang = thucteThangg,
                        thucTeNam = thucTeNam,
                        data = list,
                        sanLuongTrongNgay = dataTrongNgay,
                    };
                    return await Task.FromResult(result);
                }
                catch (Exception ex)
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
        static List<MissingDataStatus> data = new List<MissingDataStatus>();
        public static List<MissingDataStatus> getMissingData(string name = "")
        {
            using (var db = new Model1())
            {
                IEnumerable<DiemDo> listDiemDo = db.DiemDoes;
                if (data.Count() == 0 && name.Length == 0)
                {
                    DateTime startDay = db.SanLuongs.Min(x => x.Ngay);
                    DateTime endDay = DateTime.Now.AddDays(-1);
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
                                        status = 0
                                    });
                                }
                                else
                                {
                                    data.Add(new MissingDataStatus()
                                    {
                                        date = date.ToString("dd/MM/yyyy"),
                                        name = item.TenDiemDo,
                                        status = -1
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
                    int id = db.DiemDoes.Where(x => x.MaDiemDo == MaDiemDo).Select(x => x.ID).FirstOrDefault();
                    if (db.SanLuongs.Where(x => x.Ngay == date).Where(x => x.DiemDoID == id).Count() > 0)
                    {
                        int index = data.FindIndex(x => x.date == date.ToString("dd/MM/yyyy"));
                        var obj = data[index];
                        obj.status = 1;
                        data.RemoveAt(index);
                        data.Insert(0, obj);
                    }
                    else
                    {
                        int index = data.FindIndex(x => x.date == date.ToString("dd/MM/yyyy"));
                        var obj = data[index];
                        obj.status = 0;
                        data.RemoveAt(index);
                        data.Insert(0, obj);
                    }
                    return data;
                }
            }
        }

        public static FileResult exportExcel()
        {
            using (var db = new Model1())
            {
                DateTime thang = new DateTime(2020, 05, 01);
                var rawData = db.ChiSoChots.Where(x => x.thang == thang);
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
                return GenerateExcel(data, "rpt_PhieuTongHop_GNDN_NMD_ChiTiet.xlsx");
            }
        }
        public static FileResult GenerateExcel(List<ExportExcelModel> data, string fileDir)
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
                        wookSheet.Cells[rowIndex, 13].Value = item.dienNangGiao.bieuTong;
                        wookSheet.Cells[rowIndex, 15].Value = item.dienNangGiao.bieuTong;
                        rowIndex += 1;
                        wookSheet.Cells[rowIndex, 1].Value = "b";
                        wookSheet.Cells[rowIndex, 2].Value = "Điện năng tác dụng giao - Biểu 1";
                        wookSheet.Cells[rowIndex, 7].Value = 0;
                        wookSheet.Cells[rowIndex, 8].Value = item.dienNangGiao.bieu1;
                        wookSheet.Cells[rowIndex, 11].Value = item.dienNangGiao.bieu1;
                        wookSheet.Cells[rowIndex, 12].Value = 1000;
                        wookSheet.Cells[rowIndex, 13].Value = item.dienNangGiao.bieu1;
                        wookSheet.Cells[rowIndex, 15].Value = item.dienNangGiao.bieu1;
                        rowIndex += 1;
                        wookSheet.Cells[rowIndex, 1].Value = "c";
                        wookSheet.Cells[rowIndex, 2].Value = "Điện năng tác dụng giao - Biểu 2";
                        wookSheet.Cells[rowIndex, 7].Value = 0;
                        wookSheet.Cells[rowIndex, 8].Value = item.dienNangGiao.bieu2;
                        wookSheet.Cells[rowIndex, 11].Value = item.dienNangGiao.bieu2;
                        wookSheet.Cells[rowIndex, 12].Value = 1000;
                        wookSheet.Cells[rowIndex, 13].Value = item.dienNangGiao.bieu2;
                        wookSheet.Cells[rowIndex, 15].Value = item.dienNangGiao.bieu2;
                        rowIndex += 1;
                        wookSheet.Cells[rowIndex, 1].Value = "d";
                        wookSheet.Cells[rowIndex, 2].Value = "Điện năng tác dụng giao - Biểu 3";
                        wookSheet.Cells[rowIndex, 7].Value = 0;
                        wookSheet.Cells[rowIndex, 8].Value = item.dienNangGiao.bieu3;
                        wookSheet.Cells[rowIndex, 11].Value = item.dienNangGiao.bieu3;
                        wookSheet.Cells[rowIndex, 12].Value = 1000;
                        wookSheet.Cells[rowIndex, 13].Value = item.dienNangGiao.bieu3;
                        wookSheet.Cells[rowIndex, 15].Value = item.dienNangGiao.bieu3;
                        rowIndex += 1;
                        wookSheet.Cells[rowIndex, 1].Value = "e";
                        wookSheet.Cells[rowIndex, 2].Value = "Điện năng phản kháng Giao";
                        wookSheet.Cells[rowIndex, 7].Value = 0;
                        wookSheet.Cells[rowIndex, 8].Value = item.dienNangGiao.phanKhang;
                        wookSheet.Cells[rowIndex, 11].Value = item.dienNangGiao.phanKhang;
                        wookSheet.Cells[rowIndex, 12].Value = 1000;
                        wookSheet.Cells[rowIndex, 13].Value = item.dienNangGiao.phanKhang;
                        wookSheet.Cells[rowIndex, 15].Value = item.dienNangGiao.phanKhang;

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
                        wookSheet.Cells[rowIndex, 13].Value = item.dienNangNhan.bieuTong;
                        wookSheet.Cells[rowIndex, 15].Value = item.dienNangNhan.bieuTong;
                        rowIndex += 1;
                        wookSheet.Cells[rowIndex, 1].Value = "b";
                        wookSheet.Cells[rowIndex, 2].Value = "Điện năng tác dụng nhận - Biểu 1";
                        wookSheet.Cells[rowIndex, 7].Value = 0;
                        wookSheet.Cells[rowIndex, 8].Value = item.dienNangNhan.bieu1;
                        wookSheet.Cells[rowIndex, 11].Value = item.dienNangNhan.bieu1;
                        wookSheet.Cells[rowIndex, 12].Value = 1000;
                        wookSheet.Cells[rowIndex, 13].Value = item.dienNangNhan.bieu1;
                        wookSheet.Cells[rowIndex, 15].Value = item.dienNangNhan.bieu1;
                        rowIndex += 1;
                        wookSheet.Cells[rowIndex, 1].Value = "c";
                        wookSheet.Cells[rowIndex, 2].Value = "Điện năng tác dụng nhận - Biểu 2";
                        wookSheet.Cells[rowIndex, 7].Value = 0;
                        wookSheet.Cells[rowIndex, 8].Value = item.dienNangNhan.bieu2;
                        wookSheet.Cells[rowIndex, 11].Value = item.dienNangNhan.bieu2;
                        wookSheet.Cells[rowIndex, 12].Value = 1000;
                        wookSheet.Cells[rowIndex, 13].Value = item.dienNangNhan.bieu2;
                        wookSheet.Cells[rowIndex, 15].Value = item.dienNangNhan.bieu2;
                        rowIndex += 1;
                        wookSheet.Cells[rowIndex, 1].Value = "d";
                        wookSheet.Cells[rowIndex, 2].Value = "Điện năng tác dụng nhận - Biểu 3";
                        wookSheet.Cells[rowIndex, 7].Value = 0;
                        wookSheet.Cells[rowIndex, 8].Value = item.dienNangNhan.bieu3;
                        wookSheet.Cells[rowIndex, 11].Value = item.dienNangNhan.bieu3;
                        wookSheet.Cells[rowIndex, 12].Value = 1000;
                        wookSheet.Cells[rowIndex, 13].Value = item.dienNangNhan.bieu3;
                        wookSheet.Cells[rowIndex, 15].Value = item.dienNangNhan.bieu3;
                        rowIndex += 1;
                        wookSheet.Cells[rowIndex, 1].Value = "e";
                        wookSheet.Cells[rowIndex, 2].Value = "Điện năng phản kháng Nhận";
                        wookSheet.Cells[rowIndex, 7].Value = 0;
                        wookSheet.Cells[rowIndex, 8].Value = item.dienNangNhan.phanKhang;
                        wookSheet.Cells[rowIndex, 11].Value = item.dienNangNhan.phanKhang;
                        wookSheet.Cells[rowIndex, 12].Value = 1000;
                        wookSheet.Cells[rowIndex, 13].Value = item.dienNangNhan.phanKhang;
                        wookSheet.Cells[rowIndex, 15].Value = item.dienNangNhan.phanKhang;
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
}