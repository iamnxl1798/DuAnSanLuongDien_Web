
using DuAn;
using ServiceTool.Model.CustomModel;
using ServiceTool.Model.DbModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DocDuLieuCongTo.Model
{
    public class DAO : DBContext
    {
        public static class CongToDAO
        {
            static readonly DbContextService db = new DbContextService();
            public static void InsertCongTo(CongTo ct)
            {
                var rs = db.CongToes.Where(x => x.Serial == ct.Serial).FirstOrDefault();
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
                var rs = db.SanLuongs.Where(x => x.DiemDoID == sl.DiemDoID && x.KenhID == sl.KenhID && x.Ngay == sl.Ngay && x.ChuKy == sl.ChuKy).FirstOrDefault();
                if (rs != null)
                {
                    return true;
                }
                return false;
            }
            public static string InsertSanLuong(SanLuong sl)
            {
                try
                {
                    var rs = db.SanLuongs.Where(x => x.DiemDoID == sl.DiemDoID && x.KenhID == sl.KenhID && x.Ngay == sl.Ngay && x.ChuKy == sl.ChuKy).FirstOrDefault();
                    if (rs != null)
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
                    if (rs != null)
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
            public static string checkEnoughData(int chuki, DateTime dt)
            {
                try
                {
                    string congThuc = CongThucDAO.getCongThuc(dt);
                    if (congThuc == "failed")
                    {
                        return "failed";
                    }
                    string[] divided = congThuc.Split(' ');// tach cac phan tu trong cong thuc

                    List<PhanTu> listPhanTu = new List<PhanTu>();
                    for (int i = 0; i < divided.Length; i++)
                    {
                        if (divided[i].Length > 1) // divice la phan tu 
                        {
                            string[] temp = divided[i].Split('@');
                            if (temp.Length > 1)
                            {
                                var tenDiemDo = temp[0];
                                var tenKenh = temp[1];
                                PhanTu pt = new PhanTu();
                                pt.DiemDoID = db.DiemDoes.Where(x => x.TenDiemDo == tenDiemDo).Select(x => x.ID).FirstOrDefault();
                                if (pt.DiemDoID == 0)
                                {
                                    return "Điểm đo trong Công thức không tồn tại !!!";
                                }
                                pt.KenhID = db.Kenhs.Where(x => x.Ten == tenKenh).Select(x => x.ID).FirstOrDefault();
                                if (pt.KenhID == 0)
                                {
                                    return "Kênh trong Công thức không tồn tại !!!";
                                }
                                pt.index = i;
                                listPhanTu.Add(pt);
                            }
                        }
                    }
                    foreach (var item in listPhanTu)
                    {
                        var rs = db.SanLuongs.Where(x => x.ChuKy == chuki && x.DiemDoID == item.DiemDoID && x.KenhID == item.KenhID && x.Ngay == dt).FirstOrDefault();
                        if (rs == null)
                        {
                            return "failed";
                        }
                    }
                    return "success";
                }
                catch (Exception ex)
                {
                    return "failed";
                }
            }
            public static string Calculator(int chuki, DateTime dt)
            {
                try
                {
                    var enoughData = TongSanLuong_NgayDAO.checkEnoughData(chuki, dt);
                    if (enoughData == "success")
                    {
                        string congThuc = CongThucDAO.getCongThuc(dt);
                        int congThucID = db.CongThucTongSanLuongs.Where(x => x.ThoiGianBatDau < dt && x.ThoiGianKetThuc > dt).Select(x => x.ID).First();
                        string[] divided = congThuc.Split(' ');
                        List<MyCustom> list = new List<MyCustom>();
                        List<string> charList = new List<string>();
                        var diemDo = db.DiemDoes.ToList();
                        var kenh = db.Kenhs.ToList();
                        for (int i = 0; i < divided.Length; i++)
                        {
                            if (divided[i].Length > 1)
                            {
                                string[] temp = divided[i].Split('@');
                                if (temp.Length > 1)
                                {
                                    var temDiemDo = temp[0];
                                    var tenKenh = temp[1];
                                    var tempDD = diemDo.Where(x => x.TenDiemDo == temDiemDo).Select(x => x.ID).First();
                                    var tempKenh = kenh.Where(x => x.Ten == tenKenh).Select(x => x.ID).First();
                                    list.Add(new MyCustom()
                                    {
                                        DiemDoID = tempDD,
                                        KenhID = tempKenh,
                                        index = i
                                    });
                                }
                                else
                                {
                                    list.Add(new MyCustom()
                                    {
                                        index = i,
                                        value = Convert.ToDouble(temp[0])
                                    });
                                }
                            }
                        }
                        var tongSanLuongNgay = new List<TongSanLuong_Ngay>();
                        DateTime ngayTinh = dt;
                        for (int i = 1; i <= 48; i++)
                        {
                            var listTemp = db.SanLuongs.Where(x => x.ChuKy == i && x.Ngay == ngayTinh).ToList();
                            string formula = "";
                            for (int j = 0; j < list.Count; j++)
                            {
                                var diemDoTemp = list[j].DiemDoID;
                                var kenhTemp = list[j].KenhID;
                                if (diemDoTemp != 0)
                                {
                                    divided[list[j].index] = listTemp.Where(x => x.DiemDoID == diemDoTemp && x.KenhID == kenhTemp).Select(x => x.GiaTri).FirstOrDefault().ToString();
                                }
                                else
                                {
                                    divided[list[j].index] = list[j].value.ToString();
                                }
                            }
                            for (int stringBlank = 0; stringBlank < divided.Length; stringBlank++)
                            {
                                if (divided[stringBlank] == "" && stringBlank != divided.Length - 1)
                                {
                                    formula += "0 ";
                                }
                                else
                                {
                                    formula += divided[stringBlank] + ' ';
                                }
                            }
                            string rpn = toRPN(formula.Trim()).Trim();
                            var value = CalculateRPN(rpn);
                            TongSanLuong_Ngay tslng = new TongSanLuong_Ngay();
                            tslng.ChuKy = i;
                            tslng.CongThucID = congThucID;
                            tslng.Ngay = dt;
                            tslng.GiaTri = (double)value;

                            var rs = Insert(tslng);
                            if (rs != "success")
                            {
                                return rs;
                            }
                        }
                    }
                    else
                    {
                        return enoughData;
                    }
                    return "success";

                }
                catch (Exception ex)
                {
                    return "failed";
                }
            }
            static decimal CalculateRPN(string rpn)
            {
                string[] rpnTokens = rpn.Split(' ');
                Stack<decimal> stack = new Stack<decimal>();
                decimal number = decimal.Zero;

                foreach (string token in rpnTokens)
                {
                    if (decimal.TryParse(token, out number))
                    {
                        stack.Push(number);
                    }
                    else
                    {
                        switch (token)
                        {
                            case "^":
                            case "pow":
                                {
                                    number = stack.Pop();
                                    stack.Push((decimal)Math.Pow((double)stack.Pop(), (double)number));
                                    break;
                                }
                            case "ln":
                                {
                                    stack.Push((decimal)Math.Log((double)stack.Pop(), Math.E));
                                    break;
                                }
                            case "sqrt":
                                {
                                    stack.Push((decimal)Math.Sqrt((double)stack.Pop()));
                                    break;
                                }
                            case "x":
                                {
                                    stack.Push(stack.Pop() * stack.Pop());
                                    break;
                                }
                            case "/":
                                {
                                    number = stack.Pop();
                                    double result = 0.0;
                                    try
                                    {
                                        result = (double)(stack.Pop() / number);
                                    }
                                    catch
                                    {
                                        result = 0;
                                    }
                                    stack.Push((decimal)result);
                                    break;
                                }
                            case "+":
                                {
                                    stack.Push(stack.Pop() + stack.Pop());
                                    break;
                                }
                            case "-":
                                {
                                    number = stack.Pop();
                                    stack.Push(stack.Pop() - number);
                                    break;
                                }
                            default:
                                Console.WriteLine("Error in CalculateRPN(string) Method!");
                                break;
                        }
                    }
                    //PrintState(stack);
                }

                return stack.Pop();
            }
            static String toRPN(String input)
            {
                try
                {


                    Stack<string> stack = new Stack<string>();
                    StringBuilder formula = new StringBuilder();
                    String[] arr = input.Split(' ');
                    for (int i = 0; i < arr.Length; i++)
                    {
                        string x = arr[i];
                        double num = 0;
                        if (x == "(")
                            stack.Push(x);
                        else if (x == ")")
                        {
                            while (stack.Count > 0 && stack.Peek() != "(")
                            {
                                formula.Append(stack.Pop());
                                formula.Append(' ');
                            }
                            stack.Pop();
                        }
                        else if (double.TryParse(x, out num))
                        {
                            formula.Append(x);
                            formula.Append(' ');
                        }
                        else if (IsOperator(x))
                        {
                            while (stack.Count > 0 && stack.Peek() != "(" && Prior(x) <= Prior(stack.Peek()))
                            {
                                formula.Append(stack.Pop());
                                formula.Append(' ');
                            }
                            stack.Push(x);
                        }
                        else
                        {
                            string y = stack.Pop();
                            if (y != "(")
                            {
                                formula.Append(y);
                                formula.Append(' ');
                            }
                        }
                    }
                    while (stack.Count > 0)
                    {
                        formula.Append(stack.Pop());
                        formula.Append(' ');
                    }
                    return formula.ToString();
                }
                catch (Exception ex)
                {
                    return "false";
                }
            }

            static bool IsOperator(string c)
            {
                return (c == "-" || c == "+" || c == "x" || c == "/");
            }

            static int Prior(string c)
            {
                switch (c)
                {
                    case "=":
                        return 1;
                    case "+":
                        return 2;
                    case "-":
                        return 2;
                    case "*":
                        return 3;
                    case "/":
                        return 3;
                    case "^":
                        return 4;
                    default:
                        throw new ArgumentException("Rossz parameter");
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
            public static string Calculator(DateTime dt)
            {
                try
                {
                    var thang = dt.Month;
                    var nam = dt.Year;

                    var list_sl_ngay = db.TongSanLuong_Ngay.Where(x => x.Ngay.Month == thang && x.Ngay.Year == nam).Select(x => x.GiaTri);
                    if (list_sl_ngay != null && list_sl_ngay.Count() != 0)
                    {
                        var sum = 0.0;
                        foreach (var item in list_sl_ngay)
                        {
                            sum += item.Value;
                        }

                        var rs = db.TongSanLuong_Thang.Where(x => x.Thang == thang && x.Nam == nam).FirstOrDefault();
                        if (rs == null)
                        {
                            TongSanLuong_Thang tsnl = new TongSanLuong_Thang();
                            tsnl.Thang = dt.Month;
                            tsnl.Nam = dt.Year;
                            tsnl.GiaTri = sum;
                            db.TongSanLuong_Thang.Add(tsnl);
                        }
                        else
                        {
                            rs.GiaTri = sum;
                        }
                        db.SaveChanges();
                        return "success";
                    }
                    else
                    {
                        return "failed";
                    }

                }
                catch (Exception ex)
                {
                    return "failed";
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
            public static string Calculator(DateTime dt)
            {
                try
                {
                    using (var db = new DbContextService())
                    {
                        var nam = dt.Year;

                        var list_sl_thang = db.TongSanLuong_Thang.Where(x => x.Nam == nam).Select(x => x.GiaTri);
                        if (list_sl_thang != null && list_sl_thang.Count() != 0)
                        {
                            var sum = 0.0;
                            foreach (var item in list_sl_thang)
                            {
                                sum += item.Value;
                            }

                            var rs = db.TongSanLuong_Nam.Where(x => x.Nam == nam).FirstOrDefault();
                            if (rs == null)
                            {
                                TongSanLuong_Nam tsnn = new TongSanLuong_Nam();
                                tsnn.Nam = dt.Year;
                                tsnn.GiaTri = sum;
                                db.TongSanLuong_Nam.Add(tsnn);
                            }
                            else
                            {
                                rs.GiaTri = sum;
                            }
                            db.SaveChanges();
                            return "success";
                        }
                        else
                        {
                            return "failed";
                        }
                    }
                }
                catch (Exception ex)
                {
                    return "failed";
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
    public static class CongThucDAO
    {
        public static string getCongThuc(DateTime dt)
        {
            try
            {

                using (var db = new DbContextService())
                {
                    var ct = db.CongThucTongSanLuongs.Where(x => x.ThoiGianBatDau < dt && x.ThoiGianKetThuc > dt).Select(x => x.CongThuc).FirstOrDefault();
                    if (ct == null)
                    {
                        return "failed";
                    }
                    return ct;
                }
            }
            catch (Exception ex)
            {
                return "failed";
            }
        }
    }

}
