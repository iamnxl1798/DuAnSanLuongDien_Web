using ServiceTool.Model;
using ServiceTool.Model.CustomModel;
using ServiceTool.Model.DbModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DocDuLieuCongTo.Model.DAO;

namespace ServiceTool
{
    class SanLuongManage
    {
        ConfigClass conf;
        NotifyIcon notifyIconSL;
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public SanLuongManage(NotifyIcon noti, ConfigClass cf)
        {
            this.conf = cf;
            this.notifyIconSL = noti;
            // San Luong service 
            notifyIconSL.Visible = true;
            notifyIconSL.ShowBalloonTip(500);
        }

        // Define the event handlers.
        public void OnChangedSanLuong(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            ShowNotificationMessage(500, "Changed", $"{e.Name}", ToolTipIcon.None);

        }

        public void OnRenamedSanLuong(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            //Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
            ShowNotificationMessage(50, "Renamed", $"{ e.OldName} renamed to { e.Name}", ToolTipIcon.None);
        }

        public void OnDeleteSanLuong(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            ShowNotificationMessage(500, "Delete", $"{e.Name}", ToolTipIcon.None);
        }

        public void OnCreatedSanLuong(object source, FileSystemEventArgs e)
        {

            while (true)
            {
                try
                {
                    DbContextService db = new DbContextService();
                    // get cong thuc 
                    DateTime date = DateTime.Now.AddDays(-1);
                    string congThuc = db.CongThucTongSanLuongs.Where(x => x.ThoiGianBatDau < date && x.ThoiGianKetThuc > date).Select(x => x.CongThuc).First();
                    int congThucID = db.CongThucTongSanLuongs.Where(x => x.ThoiGianBatDau < date && x.ThoiGianKetThuc > date).Select(x => x.ID).First();
                    string[] divided = congThuc.Split(' ');// tach cac phan tu trong cong thuc
                    // add Phan Tu trong cong thuc
                    List<PhanTu> listPhanTu = new List<PhanTu>();
                    for (int i = 0; i < divided.Length; i++)
                    {
                        if (divided[i].Length > 1) // divice la phan tu 
                        {
                            string[] temp = divided[i].Split('~');
                            var tenDiemDo = temp[0];
                            var tenKenh = temp[1];
                            PhanTu pt = new PhanTu();
                            pt.DiemDoID = db.DiemDoes.Where(x => x.TenDiemDo == tenDiemDo).Select(x => x.ID).FirstOrDefault();
                            if (pt.DiemDoID == 0)
                            {
                                ShowNotificationMessage(50, "Error", "Điểm đo trong Công thức không tồn tại !!!", ToolTipIcon.Error);
                                return;
                            }
                            pt.KenhID = db.Kenhs.Where(x => x.Ten == tenKenh).Select(x => x.ID).FirstOrDefault();
                            if (pt.KenhID == 0)
                            {
                                ShowNotificationMessage(50, "Error", "Kênh trong Công thức không tồn tại !!!", ToolTipIcon.Error);
                                return;
                            }
                            pt.index = i;
                            listPhanTu.Add(pt);
                        }
                    }
                    bool checkDiemDoTSLN = true;

                    //Console.WriteLine("Try to access file !!!");
                    StreamReader reader = new StreamReader(e.FullPath);
                    //Console.WriteLine("Access file successfully !!!");
                    string fileName = e.Name;
                    // get DiemDoID
                    int get_DiemDoID = DiemDoDAO.GetDiemDoID(int.Parse(fileName.Split('.')[0].Substring(5)));
                    if (get_DiemDoID == 0)
                    {
                        ShowNotificationMessage(50, "Error", "Điểm đo không tồn tại !!!", ToolTipIcon.Error);
                        reader.Close();
                        return;
                    }
                    // check điểm đo có trong công thức hay không
                    int indexPhanTu = 0;
                    foreach (var i in listPhanTu)
                    {
                        if (i.DiemDoID == get_DiemDoID)
                        {
                            checkDiemDoTSLN = true;
                            indexPhanTu = listPhanTu.IndexOf(i);
                            ShowNotificationMessage(50, "Thông báo", "Điểm đo có trong công thức !!!", ToolTipIcon.Info);
                            break;
                        }
                    }
                    if (!checkDiemDoTSLN)
                    {
                        ShowNotificationMessage(50, "Thông báo", "Điểm đo không có trong công thức !!!", ToolTipIcon.Info);
                        checkDiemDoTSLN = false;
                    }


                    string day = fileName.Substring(0, 2);
                    string month = fileName.Substring(2, 2);
                    //fix year
                    string year = (DateTime.Now.Year).ToString();
                    DateTime time = DateTime.Parse(month + "/" + day + "/" + year);
                    // read file
                    string line;
                    int count = 1;
                    while ((line = reader.ReadLine()) != null)
                    {

                        if (count == 1 || count == 2 || count == 8 || count == 9)
                        {

                            string[] word = line.Split(',');
                            int get_KenhID = KenhDAO.GetKenhID(word[1]);
                            if (get_KenhID == 0)
                            {
                                ShowNotificationMessage(50, "Error", "Kênh không tồn tại !!!", ToolTipIcon.Error);
                                reader.Close();
                                return;
                            }
                            //check kenh
                            bool checkKenhTSLN = false;
                            if (listPhanTu[indexPhanTu].KenhID == get_KenhID)
                            {
                                checkKenhTSLN = true;
                            }


                            for (int i = 2; i < word.Length; i++)
                            {
                                SanLuong sl = new SanLuong
                                {
                                    Ngay = time,
                                    DiemDoID = get_DiemDoID,
                                    KenhID = get_KenhID
                                };
                                sl.ChuKy = short.Parse((i - 1).ToString());
                                /*sl.GiaTri = Convert.ToDouble(string.Format("{0:0.##}", word[i]) );*/
                                sl.GiaTri = Convert.ToDouble(word[i]);
                                if (!SanLuongDAO.checkExistSL(sl))
                                {
                                    var rs = SanLuongDAO.InsertSanLuong(sl);
                                    if (rs != "success")
                                    {
                                        ShowNotificationMessage(50, "Error", rs, ToolTipIcon.Error);
                                        reader.Close();
                                        return;
                                    }

                                    // tinh luy ke
                                    if (checkDiemDoTSLN && checkKenhTSLN)
                                    {
                                        // tinh tong san luong ngay
                                        bool checkInsertDay = false;
                                        bool checkInsertMonth = false;
                                        bool checkInsertYear = false;
                                        bool checkInsertMonthYear = false;
                                        TongSanLuong_Ngay tslng = TongSanLuong_NgayDAO.GetByNgayAndChuKy(time, i - 1);
                                        TongSanLuong_Thang tslt = TongSanLuong_ThangDAO.GetByTime(time);
                                        TongSanLuong_Nam tsln = TongSanLuong_NamDAO.GetByTime(time);
                                        TongSanLuong_ThangNam tsltn = TongSanLuong_ThangNamDAO.GetByTime(time);
                                        if (tslng == null)
                                        {
                                            // create new tslng
                                            tslng = new TongSanLuong_Ngay()
                                            {
                                                Ngay = sl.Ngay,
                                                ChuKy = sl.ChuKy,
                                                CongThucID = congThucID,
                                                GiaTri = 0
                                            };
                                            checkInsertDay = true;
                                        }
                                        if (tslt == null)
                                        {
                                            // create new tslt
                                            tslt = new TongSanLuong_Thang()
                                            {
                                                Thang = time.Month,
                                                Nam = time.Year,
                                                GiaTri = 0
                                            };
                                            checkInsertMonth = true;
                                        }
                                        if (tsln == null)
                                        {
                                            // create new tsln
                                            tsln = new TongSanLuong_Nam()
                                            {
                                                Nam = time.Year,
                                                GiaTri = 0
                                            };
                                            checkInsertYear = true;
                                        }
                                        if (tsltn == null)
                                        {
                                            // create new tsltn
                                            tsltn = new TongSanLuong_ThangNam()
                                            {
                                                Ngay = time,
                                                GiaTriNam = TongSanLuong_ThangNamDAO.GetClosestYearValue(time),
                                                GiaTriThang = TongSanLuong_ThangNamDAO.GetClosestMonthValue(time)
                                            };
                                            checkInsertMonthYear = true;
                                        }
                                        //tinh toan gia tri
                                        if (listPhanTu[indexPhanTu].index == 0 || divided[listPhanTu[indexPhanTu].index - 1].Equals("+"))
                                        {
                                            tslng.GiaTri += sl.GiaTri;
                                            tslt.GiaTri += sl.GiaTri;
                                            tsln.GiaTri += sl.GiaTri;

                                            tsltn.GiaTriThang += sl.GiaTri.Value;
                                            tsltn.GiaTriNam += sl.GiaTri.Value;
                                        }
                                        else
                                        {
                                            tslng.GiaTri -= sl.GiaTri;
                                            tslt.GiaTri -= sl.GiaTri;
                                            tsln.GiaTri -= sl.GiaTri;

                                            tsltn.GiaTriThang -= sl.GiaTri.Value;
                                            tsltn.GiaTriNam -= sl.GiaTri.Value;
                                        }

                                        // insert or update ngay
                                        if (checkInsertDay)
                                        {
                                            var rs_checkInsertDay = TongSanLuong_NgayDAO.Insert(tslng);
                                            if (rs_checkInsertDay != "success")
                                            {
                                                ShowNotificationMessage(50, "Error", rs_checkInsertDay, ToolTipIcon.Error);
                                                reader.Close();
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            var rs_checkUpdateDay = TongSanLuong_NgayDAO.Update(tslng);
                                            if (rs_checkUpdateDay != "success")
                                            {
                                                ShowNotificationMessage(50, "Error", rs_checkUpdateDay, ToolTipIcon.Error);
                                                reader.Close();
                                                return;
                                            }
                                        }
                                        // insert or update thang
                                        if (checkInsertMonth)
                                        {
                                            var rs_checkInsertMon = TongSanLuong_ThangDAO.Insert(tslt);
                                            if (rs_checkInsertMon != "success")
                                            {
                                                ShowNotificationMessage(50, "Error", rs_checkInsertMon, ToolTipIcon.Error);
                                                reader.Close();
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            var rs_checkInsertMon = TongSanLuong_ThangDAO.Update(tslt);
                                            if (rs_checkInsertMon != "success")
                                            {
                                                ShowNotificationMessage(50, "Error", rs_checkInsertMon, ToolTipIcon.Error);
                                                reader.Close();
                                                return;
                                            }
                                        }
                                        // insert or update nam
                                        if (checkInsertYear)
                                        {
                                            var rs_checkInsertYear = TongSanLuong_NamDAO.Insert(tsln);
                                            if (rs_checkInsertYear != "success")
                                            {
                                                ShowNotificationMessage(50, "Error", rs_checkInsertYear, ToolTipIcon.Error);
                                                reader.Close();
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            var rs_checkInsertYear = TongSanLuong_NamDAO.Update(tsln);
                                            if (rs_checkInsertYear != "success")
                                            {
                                                ShowNotificationMessage(50, "Error", rs_checkInsertYear, ToolTipIcon.Error);
                                                reader.Close();
                                                return;
                                            }
                                        }
                                        // insert or update thang-nam
                                        if (checkInsertMonthYear)
                                        {
                                            var rs_checkInsertYear = TongSanLuong_ThangNamDAO.Insert(tsltn);
                                            if (rs_checkInsertYear != "success")
                                            {
                                                ShowNotificationMessage(50, "Error", rs_checkInsertYear, ToolTipIcon.Error);
                                                reader.Close();
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            var rs_checkInsertYear = TongSanLuong_ThangNamDAO.Update(tsltn);
                                            if (rs_checkInsertYear != "success")
                                            {
                                                ShowNotificationMessage(50, "Error", rs_checkInsertYear, ToolTipIcon.Error);
                                                reader.Close();
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        count++;
                    }
                    ShowNotificationMessage(50, "Success", "Reading file 'Sản Lượng' finished!!!!", ToolTipIcon.Info);
                    reader.Close();
                    break;
                }
                catch (IOException)
                {
                    //Console.WriteLine("Wait to access file !!!");
                    ShowNotificationMessage(50, "Error", "Wait to access file !!!", ToolTipIcon.Error);
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);
                    ShowNotificationMessage(50, "Error", ex.Message, ToolTipIcon.Error);
                    break;
                }
                finally
                {
                    try
                    {
                        string fileName = e.Name;
                        string sourcePath = conf.ThuMucQuet;
                        string targetPath = conf.ThuMucChuyen;
                        //Combine file và đường dẫn
                        string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
                        string destFile = System.IO.Path.Combine(targetPath, fileName);
                        //Copy file từ file nguồn đến file đích
                        System.IO.File.Copy(sourceFile, destFile, true);
                        ShowNotificationMessage(50, "Di chuyển file !!!", "Thành công", ToolTipIcon.None);
                    }
                    catch (Exception ex)
                    {
                        ShowNotificationMessage(50, "Error !!!", ex.Message, ToolTipIcon.Error);
                    }
                }

            }
        }

        public void ShowNotificationMessage(int timeout, string Title, string Text, ToolTipIcon tl)
        {
            notifyIconSL.ShowBalloonTip(timeout, "San Luong Service", Title + " : " + Text, tl);
        }

    }
}
