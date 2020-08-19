
using ServiceTool.Model.DbModel;
using System;

using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;

namespace DocDuLieuCongTo.Model
{
    public class DAO : DbContext
    {


        public static class CongToDAO
        {
            static readonly DbContextService db = new DbContextService();
            public static void InsertCongTo(CongTo ct)
            {
                var rs = db.CongToes.Where(x=>x.Serial == ct.Serial).FirstOrDefault();
                if (rs == null)
                {
                    db.CongToes.Add(ct);
                    db.SaveChanges();
                }
            }
            public static bool CheckSerialCongTo(string serial)
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

        public static class SanLuongDAO
        {
            static readonly DbContextService db = new DbContextService();
            public static bool checkExistSL(SanLuong sl)
            {
                using (var db = new DbContextService())
                {
                    var rs = db.SanLuongs.Where(x => x.DiemDoID == sl.DiemDoID && x.KenhID == sl.KenhID && x.Ngay == sl.Ngay && x.ChuKy == sl.ChuKy).FirstOrDefault();
                    if (rs != null)
                    {
                        return true;
                    }
                    return false;
                }
            }
            public static string InsertSanLuong(SanLuong sl)
            {
                try
                {
                    var rs = db.SanLuongs.Where(x => x.DiemDoID == sl.DiemDoID && x.KenhID == sl.KenhID && x.Ngay == sl.Ngay && x.ChuKy == sl.ChuKy).FirstOrDefault();
                    if(rs != null)
                    {
                        return "Bản ghi sản lượng đã tồn tại";
                    }
                    db.SanLuongs.Add(sl);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                return "success";
            }
        }

        public static class DiemDoDAO
        {
            static readonly DbContextService db = new DbContextService();
            public static int GetDiemDoID(int MaDiemDo)
            {
                try
                {
                    foreach (var rs in db.DiemDoes)
                    {
                        if (rs.MaDiemDo == MaDiemDo)
                        {
                            return rs.ID;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return 0;
            }

            /*public static bool InsertDiemDo(DiemDo dd)
            {

            }*/
        }

        public static class KenhDAO
        {
            static readonly DbContextService db = new DbContextService();
            public static int GetKenhID(string TenKenh)
            {
                foreach (var rs in db.Kenhs)
                {
                    if (rs.Ten.Equals(TenKenh))
                    {
                        return rs.ID;
                    }
                }
                return 0;
            }
        }

        /*static class SanLuongThucTeDAO
        {
             static readonly DbContextService db = new DbContextService();
            public static bool InsertSLTT(TongSanLuong_Ngay sltt)
            {
                try
                {
                    db.SanLuongThucTes.Add(sltt);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                return true;
            }
        }*/

        public static class DiemDo_CongToDAO
        {
            static readonly DbContextService db = new DbContextService();
            public static int GetCongToID(int DiemDoID)
            {
                foreach (var rs in db.DiemDo_CongTo)
                {
                    if (rs.DiemDoID == DiemDoID && rs.ThoiGianKetThuc == null)
                    {
                        return rs.CongToID;
                    }
                }
                return 0;
            }
        }

        public static class TongSanLuong_NgayDAO
        {
            static readonly DbContextService db = new DbContextService();
            public static TongSanLuong_Ngay GetByNgayAndChuKy(DateTime ngay, int chuky)
            {
                foreach (var rs in db.TongSanLuong_Ngay)
                {
                    if (rs.Ngay == ngay && rs.ChuKy == chuky)
                    {
                        return rs;
                    }
                }
                return null;
            }
            public static string Insert(TongSanLuong_Ngay tslng)
            {
                tslng.GiaTri = Convert.ToDouble(string.Format("{0:0.##}", tslng.GiaTri.ToString()));
                try
                {
                    var rs = db.TongSanLuong_Ngay.Where(x => x.Ngay == tslng.Ngay && x.ChuKy == tslng.ChuKy).FirstOrDefault();
                    if(rs != null)
                    {
                        return "Bản ghi TongSanLuong_Ngay đã tồn tại";
                    }
                    db.TongSanLuong_Ngay.Add(tslng);
                    db.SaveChanges();
                    return "success";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

            }
            public static string Update(TongSanLuong_Ngay tslng)
            {
                tslng.GiaTri = Convert.ToDouble(string.Format("{0:0.##}", tslng.GiaTri.ToString()));
                try
                {
                    var rs = db.TongSanLuong_Ngay.Find(tslng.ID);
                    if (rs == null) return "Không tìm thấy TongSanLuong_Ngay tương ứng";
                    rs.GiaTri = tslng.GiaTri;
                    db.SaveChanges();
                    return "success";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

            }
        }

        public static class TongSanLuong_ThangDAO
        {
            static readonly DbContextService db = new DbContextService();
            public static TongSanLuong_Thang GetByTime(DateTime ngay)
            {
                foreach (var rs in db.TongSanLuong_Thang)
                {

                    if (rs.Thang == ngay.Month && rs.Nam == ngay.Year/* && rs.Ngay.Day == 1*/)
                    {
                        return rs;
                    }

                }
                return null;
            }
            public static string Insert(TongSanLuong_Thang tslt)
            {
                tslt.GiaTri = Convert.ToDouble(string.Format("{0:0.##}", tslt.GiaTri.ToString()));

                try
                {
                    var rs = db.TongSanLuong_Thang.Where(x => x.Thang == tslt.Thang && x.Nam == tslt.Nam).FirstOrDefault();
                    if (rs != null)
                    {
                        return "Bản ghi TongSanLuong_Thang đã tồn tại";
                    }
                    db.TongSanLuong_Thang.Add(tslt);
                    db.SaveChanges();
                    return "success";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

            }
            public static string Update(TongSanLuong_Thang tslt)
            {
                tslt.GiaTri = Convert.ToDouble(string.Format("{0:0.##}", tslt.GiaTri.ToString()));
                try
                {
                    var rs = db.TongSanLuong_Thang.Find(tslt.ID);
                    if (rs == null) return "Không tìm thấy TongSanLuong_Thang tương ứng";
                    rs.GiaTri = tslt.GiaTri;
                    db.SaveChanges();
                    return "success";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

            }
        }
        public static class TongSanLuong_NamDAO
        {
            static readonly DbContextService db = new DbContextService();
            public static TongSanLuong_Nam GetByTime(DateTime ngay)
            {
                foreach (var rs in db.TongSanLuong_Nam)
                {

                    if (rs.Nam == ngay.Year/* && rs.Ngay.Day == 1*/)
                    {
                        return rs;
                    }

                }
                return null;
            }
            public static string Insert(TongSanLuong_Nam tsln)
            {
                tsln.GiaTri = Convert.ToDouble(string.Format("{0:0.##}", tsln.GiaTri.ToString()));

                try
                {
                    var rs = db.TongSanLuong_Nam.Where(x => x.Nam == tsln.Nam).FirstOrDefault();
                    if (rs != null)
                    {
                        return "Bản ghi TongSanLuong_Nam đã tồn tại";
                    }
                    //db.TongSanLuong_Nam.Add(tsln);
                    db.Entry(tsln).State = EntityState.Added;
                    db.SaveChanges();
                    return "success";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

            }
            public static string Update(TongSanLuong_Nam tsln)
            {
                tsln.GiaTri = Convert.ToDouble(string.Format("{0:0.##}", tsln.GiaTri.ToString()));
                try
                {
                    var rs = db.TongSanLuong_Nam.Find(tsln.ID);
                    if (rs == null) return "Không tìm thấy bản ghi TongSanLuong_Nam tương ứng";
                    rs.GiaTri = tsln.GiaTri;
                    /*db.Entry(tsln).State = EntityState.Modified;*/
                    db.SaveChanges();
                    return "success";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

            }
        }

        public static class TongSanLuong_ThangNamDAO
        {
            static readonly DbContextService db = new DbContextService();
            public static double GetClosestYearValue(DateTime ngay)
            {
                var rs = (from s in db.TongSanLuong_ThangNam
                          where s.Ngay <= ngay && s.Ngay.Year == ngay.Year
                          orderby s.Ngay descending
                          select s.GiaTriNam).FirstOrDefault();

                return rs;
            }
            public static double GetClosestMonthValue(DateTime ngay)
            {
                var rs = (from s in db.TongSanLuong_ThangNam
                          where s.Ngay <= ngay && s.Ngay.Month == ngay.Month && s.Ngay.Year == ngay.Year
                          orderby s.Ngay descending
                          select s.GiaTriThang).FirstOrDefault();

                return rs;
            }
            public static TongSanLuong_ThangNam GetByTime(DateTime ngay)
            {
                foreach (var rs in db.TongSanLuong_ThangNam)
                {

                    if (rs.Ngay == ngay)
                    {
                        return rs;
                    }

                }
                return null;
            }
            public static string Insert(TongSanLuong_ThangNam tsltn)
            {
                tsltn.GiaTriThang = Convert.ToDouble(string.Format("{0:0.##}", tsltn.GiaTriThang.ToString()));
                tsltn.GiaTriNam = Convert.ToDouble(string.Format("{0:0.##}", tsltn.GiaTriNam.ToString()));
                try
                {
                    var rs = db.TongSanLuong_ThangNam.Where(x => x.Ngay == tsltn.Ngay).FirstOrDefault();
                    if (rs != null)
                    {
                        return "Bản ghi TongSanLuong_ThangNam đã tồn tại";
                    }
                    db.TongSanLuong_ThangNam.Add(tsltn);
                    db.SaveChanges();
                    return "success";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

            }
            public static string Update(TongSanLuong_ThangNam tsltn)
            {
                tsltn.GiaTriThang = Convert.ToDouble(string.Format("{0:0.##}", tsltn.GiaTriThang.ToString()));
                tsltn.GiaTriNam = Convert.ToDouble(string.Format("{0:0.##}", tsltn.GiaTriNam.ToString()));
                try
                {
                    var rs = db.TongSanLuong_ThangNam.Find(tsltn.ID);
                    if (rs == null) return "Không tìm thấy bản ghi TongSanLuong_ThangNam tương ứng";
                    rs.GiaTriThang = tsltn.GiaTriThang;
                    rs.GiaTriNam = tsltn.GiaTriNam;
                    db.SaveChanges();
                    return "success";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

            }
        }
    }
    public static class ChiSoChotDAO
    {
        public static bool checkExistCSC(string serial, DateTime dt)
        {
            using (var db = new DbContextService())
            {
                var rs = db.ChiSoChots.Where(x => x.CongToSerial == serial && x.thang == dt).FirstOrDefault();
                if (rs != null)
                {
                    return true;
                }
                return false;
            }
        }
        public static string Create(ChiSoChot csc)
        {
            using (var db = new DbContextService())
            {
                try
                {
                    var rs = db.ChiSoChots.Where(x => x.CongToSerial == csc.CongToSerial && x.thang == csc.thang).FirstOrDefault();
                    if (rs != null)
                    {
                        return "Bản ghi ChiSoChot đã tồn tại";
                    }
                    db.ChiSoChots.Add(csc);
                    db.SaveChanges();
                    return "success";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

    }
    public static class ThongSoVanHanhDAO
    {
        public static bool checkExistTSVH(string serial, DateTime dt)
        {
            using (var db = new DbContextService())
            {
                var rs = db.ThongSoVanHanhs.Where(x => x.Serial == serial && x.ThoiGianCongTo == dt).FirstOrDefault();
                if (rs != null)
                {
                    return true;
                }
                return false;
            }
        }
        public static string Create(ThongSoVanHanh tsvh)
        {
            using (var db = new DbContextService())
            {
                try
                {
                    var rs = db.ThongSoVanHanhs.Where(x => x.Serial == tsvh.Serial && x.ThoiGianCongTo == tsvh.ThoiGianCongTo).FirstOrDefault();
                    if (rs != null)
                    {
                        return "Bản ghi ThongSoVanHanh đã tồn tại";
                    }
                    db.ThongSoVanHanhs.Add(tsvh);
                    db.SaveChanges();
                    return "success";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
    }
}
