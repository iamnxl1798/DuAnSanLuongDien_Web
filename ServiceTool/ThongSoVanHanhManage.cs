using DocDuLieuCongTo.Model;
using ServiceTool.Model;
using ServiceTool.Model.DbModel;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class ThongSoVanHanhManage
    {
        ConfigClass conf;
        NotifyIcon notifyIconTSVH;
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public ThongSoVanHanhManage(NotifyIcon noti, ConfigClass cf)
        {
            this.conf = cf;
            this.notifyIconTSVH = noti;
            // San Luong service 
            notifyIconTSVH.Visible = true;
            notifyIconTSVH.ShowBalloonTip(500);

        }

        // Define the event handlers.
        public void OnChangedTSVH(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            ShowNotificationMessage(500, "Changed", $"{e.Name}", ToolTipIcon.None);

        }

        public void OnRenamedTSVH(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            //Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
            ShowNotificationMessage(50, "Renamed", $"{ e.OldName} renamed to { e.Name}", ToolTipIcon.None);
        }

        public void OnDeletedTSVH(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            ShowNotificationMessage(500, "Delete", $"{e.Name}", ToolTipIcon.None);
        }

        public void OnCreatedTSVH(object source, FileSystemEventArgs e)
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
                    DateTime dt = new DateTime();
                    var dt_Str = data[1].Split(',')[2];
                    var rs_Dt = DateTime.TryParseExact(dt_Str, "M/dd/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt );
                    if (!rs_Dt)
                    {
                        ShowNotificationMessage(50, "Error", "Định dạng thời gian không đúng", ToolTipIcon.Error);
                        reader.Close();
                        return;
                    }

                    if (!ThongSoVanHanhDAO.checkExistTSVH(serial, dt))
                    {
                        ThongSoVanHanh tsvh = new ThongSoVanHanh();
                        tsvh.Serial = serial;
                        tsvh.ThoiGianCongTo = dt;

                        tsvh.P_Nhan = double.Parse(data[6 - 1].Split(',')[1]);
                        tsvh.P_Giao = double.Parse(data[7 - 1].Split(',')[1]);

                        tsvh.Q_Nhan = double.Parse(data[13 - 1].Split(',')[1]);
                        tsvh.Q_Giao = double.Parse(data[14 - 1].Split(',')[1]);

                        tsvh.P_Nhan_BT = double.Parse(data[19 - 1].Split(',')[1]);
                        tsvh.P_Nhan_CD = double.Parse(data[20 - 1].Split(',')[1]);
                        tsvh.P_Nhan_TD = double.Parse(data[21 - 1].Split(',')[1]);

                        tsvh.P_Giao_BT = double.Parse(data[22 - 1].Split(',')[1]);
                        tsvh.P_Giao_CD = double.Parse(data[23 - 1].Split(',')[1]);
                        tsvh.P_Giao_TD = double.Parse(data[24 - 1].Split(',')[1]);

                        tsvh.PhaseA_Amps = double.Parse(data[56 - 1].Split(',')[1]);
                        tsvh.PhaseA_Volts = double.Parse(data[57 - 1].Split(',')[1]);
                        tsvh.PhaseA_PowerFactor = double.Parse(data[61 - 1].Split(',')[1]);
                        tsvh.PhaseA_Frequency = double.Parse(data[62 - 1].Split(',')[1]);
                        tsvh.PhaseA_Angle = double.Parse(data[63 - 1].Split(',')[1]);

                        tsvh.PhaseB_Amps = double.Parse(data[56 - 1].Split(',')[2]);
                        tsvh.PhaseB_Volts = double.Parse(data[57 - 1].Split(',')[2]);
                        tsvh.PhaseB_PowerFactor = double.Parse(data[61 - 1].Split(',')[2]);
                        tsvh.PhaseB_Frequency = double.Parse(data[62 - 1].Split(',')[2]);
                        tsvh.PhaseB_Angle = double.Parse(data[63 - 1].Split(',')[2]);

                        tsvh.PhaseC_Amps = double.Parse(data[56 - 1].Split(',')[3]);
                        tsvh.PhaseC_Volts = double.Parse(data[57 - 1].Split(',')[3]);
                        tsvh.PhaseC_PowerFactor = double.Parse(data[61 - 1].Split(',')[3]);
                        tsvh.PhaseC_Frequency = double.Parse(data[62 - 1].Split(',')[3]);
                        tsvh.PhaseC_Angle = double.Parse(data[63 - 1].Split(',')[3]);

                        tsvh.Phase_Rotation = data[64 - 1].Split(',')[4];

                        var rs = ThongSoVanHanhDAO.Create(tsvh);

                        if (!rs.Equals("success"))
                        {
                            ShowNotificationMessage(50, "Error", rs, ToolTipIcon.Error);
                            reader.Close();
                            return;
                        }
                    }

                    ShowNotificationMessage(50, "Success", "Reading file 'Thông số vận hành' finished!!!!", ToolTipIcon.Info);
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
            notifyIconTSVH.ShowBalloonTip(timeout, "Thong So Van Hanh Service", Title + " : " + Text, tl);
        }
    }
}

