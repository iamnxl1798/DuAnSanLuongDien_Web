using DocDuLieuCongTo.Model;
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
                    StreamReader reader = new StreamReader(e.FullPath);
                    string fileName = e.Name;
                    // get DiemDoID
                    int get_DiemDoID = DiemDoDAO.GetDiemDoID(int.Parse(fileName.Split('.')[0].Substring(5)));
                    if (get_DiemDoID == 0)
                    {
                        ShowNotificationMessage(50, "Error", "Điểm đo không tồn tại !!!", ToolTipIcon.Error);
                        reader.Close();
                        return;
                    }
                    string day = fileName.Substring(0, 2);
                    string month = fileName.Substring(2, 2);

                    int this_year = DateTime.Now.Year;
                    string year = fileName.Substring(4, 1);
                    if(year == (this_year % 10).ToString())
                    {
                        year = this_year.ToString();
                    }
                    else
                    {
                        var temp = this_year - 1;
                        year = ((temp - temp % 10) + int.Parse(year)).ToString();
                    }
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
                            //vong lap chu ky
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
                                }

                                //tinh tong san luong ngay
                                var calDay = TongSanLuong_NgayDAO.Calculator(sl.ChuKy,sl.Ngay);
                                if(calDay == "success")
                                {
                                    ShowNotificationMessage(50, "Success", "Cập nhật TongSanLuong_Ngay thành công !!", ToolTipIcon.Info);
                                }
                            }
                        }

                        count++;
                    }
                    // tinh tong san luong thang
                    var calMonth = TongSanLuong_ThangDAO.Calculator(time);
                    if (calMonth == "success")
                    {
                        ShowNotificationMessage(50, "Success", "Cập nhật TongSanLuong_Thang thành công !!", ToolTipIcon.Info);
                        var calYear = TongSanLuong_NamDAO.Calculator(time);
                        if (calYear == "success")
                        {
                            ShowNotificationMessage(50, "Success", "Cập nhật TongSanLuong_Nam thành công !!", ToolTipIcon.Info);
                        }
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
