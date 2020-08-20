using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new Model1())
            {
                DateTime date = DateTime.Now.AddDays(-1);
                string congThuc = db.CongThucTongSanLuongs.Where(x => x.ThoiGianBatDau < date && x.ThoiGianKetThuc > date).Select(x => x.CongThuc).First();
                int congThucID = db.CongThucTongSanLuongs.Where(x => x.ThoiGianBatDau < date && x.ThoiGianKetThuc > date).Select(x => x.ID).First();
                string[] divided = congThuc.Split(' ');
                List<MyCustom> list = new List<MyCustom>();
                List<string> charList = new List<string>();
                var diemDo = db.DiemDoes.ToList();
                var kenh = db.Kenhs.ToList();
                for (int i = 0; i < divided.Length; i++)
                {
                    if (divided[i].Length > 1)
                    {
                        string[] temp = divided[i].Split('~');
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
                                value=Convert.ToDouble(temp[0])
                            });
                        }
                    }
                }
                var tongSanLuongNgay = new List<TongSanLuong_Ngay>();
                DateTime ngayTinh = new DateTime(2020, 7, 1);
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
                    for (int stringBlank=0; stringBlank < divided.Length; stringBlank++)
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
                    tongSanLuongNgay.Add(new TongSanLuong_Ngay()
                    {
                        ChuKy = i,
                        CongThucID = congThucID,
                        Ngay = date,
                        GiaTri = (double)value
                    });
                }
                foreach (TongSanLuong_Ngay item in tongSanLuongNgay)
                {   
                    db.TongSanLuong_Ngay.Add(item);
                }
                db.SaveChanges();
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
                        case "*":
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

        static void PrintState(Stack<decimal> stack)
        {
            //decimal[] arr = stack.ToArray();

            //for (int i = arr.Length - 1; i >= 0; i--)
            //{
            // Console.Write("{0,-8:F3}", arr[i]);
            //}

            //Console.WriteLine();
        }

        static String toRPN(String input)
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

        static bool IsOperator(string c)
        {
            return (c == "-" || c == "+" || c == "*" || c == "/");
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
}
