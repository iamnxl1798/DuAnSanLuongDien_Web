using DocDuLieuCongTo.Model;
using ServiceTool.Model;
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
    class ChiSoChotManage
    {
        ConfigClass conf;
        NotifyIcon notifyIconCSC;
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public ChiSoChotManage(NotifyIcon noti, ConfigClass cf)
        {
            this.conf = cf;
            this.notifyIconCSC = noti;
            // San Luong service 
            notifyIconCSC.Visible = true;
            notifyIconCSC.ShowBalloonTip(500);

        }

        // Define the event handlers.
        public void OnChangedCSC(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            ShowNotificationMessage(500, "Changed", $"{e.Name}", ToolTipIcon.None);

        }

        public void OnRenamedCSC(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            //Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
            ShowNotificationMessage(50, "Renamed", $"{ e.OldName} renamed to { e.Name}", ToolTipIcon.None);
        }

        public void OnDeleteCSC(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            ShowNotificationMessage(500, "Delete", $"{e.Name}", ToolTipIcon.None);
        }

        public void OnCreatedCSC(object source, FileSystemEventArgs e)
        {
            /*ShowNotificationMessage(500, "Create", $"{e.Name}", ToolTipIcon.None);*/
            while (true)
            {
                try
                {
                    DbContextService db = new DbContextService();

                    //Console.WriteLine("Try to access file !!!");
                    StreamReader reader = new StreamReader(e.FullPath);
                    //Console.WriteLine("Access file successfully !!!");
                    string fileName = e.Name.Split('.')[0];
                    string serial = fileName.Split('_')[0];
                    // check Serial Cong To
                    if (!CongToDAO.CheckSerialCongTo(serial))
                    {
                        ShowNotificationMessage(50, "Error", "Công to serial không tồn tại", ToolTipIcon.Error);
                        reader.Close();
                        return;
                    }
                    // read file
                    string line;
                    List<string> data = new List<string>();
                    while ((line = reader.ReadLine()) != null)
                    {
                        data.Add(line);
                    }
                    int numberRecord = int.Parse(Math.Floor((Decimal)data.Count / 58).ToString());

                    for (int i = 0; i < 3; i++)
                    {
                        data.RemoveAt(0);
                    }

                    for (int i = 0; i < numberRecord; i++)
                    {
                        DateTime dt = DateTime.Parse(data[i * 58 + 56 - 1].Split(',')[1]);
                        if (!ChiSoChotDAO.checkExistCSC(serial, dt))
                        {
                            ChiSoChot csc = new ChiSoChot();
                            csc.CongToSerial = serial;
                            csc.thang = dt;

                            csc.TongGiao = double.Parse(data[i * 58 + 7 - 1].Split(',')[1]);
                            csc.PhanKhangGiao = double.Parse(data[i * 58 + 14 - 1].Split(',')[1]);
                            csc.BinhThuongGiao = double.Parse(data[i * 58 + 22 - 1].Split(',')[1]);
                            csc.CaoDiemGiao = double.Parse(data[i * 58 + 23 - 1].Split(',')[1]);
                            csc.ThapDiemGiao = double.Parse(data[i * 58 + 24 - 1].Split(',')[1]);

                            csc.TongNhan = double.Parse(data[i * 58 + 6 - 1].Split(',')[1]);
                            csc.PhangKhangNhan = double.Parse(data[i * 58 + 13 - 1].Split(',')[1]);
                            csc.BinhThuongNhan = double.Parse(data[i * 58 + 19 - 1].Split(',')[1]);
                            csc.CaoDiemNhan = double.Parse(data[i * 58 + 20 - 1].Split(',')[1]);
                            csc.ThapDiemNhan = double.Parse(data[i * 58 + 21 - 1].Split(',')[1]);

                            var rs = ChiSoChotDAO.Create(csc);
                            if ( !rs.Equals("success"))
                            {
                                ShowNotificationMessage(50, "Error", rs, ToolTipIcon.Info);
                                reader.Close();
                                return;
                            }
                        }
                    }

                    ShowNotificationMessage(50, "Success", "Reading file 'Chỉ Số Chốt' finished!!!!", ToolTipIcon.Info);
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
            notifyIconCSC.ShowBalloonTip(timeout, "Chi So Chot Service", Title + " : " + Text, tl);
        }
    }
}
