using DocDuLieuCongTo.Model;
using ServiceTool.Model;
using ServiceTool.Model.CustomModel;
using ServiceTool.Model.DbModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
         //ShowNotificationMessage(500, "Changed", $"{e.Name}", ToolTipIcon.None);

      }

      public void OnRenamedTSVH(object source, RenamedEventArgs e)
      {
         // Specify what is done when a file is renamed.
         //Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
         //ShowNotificationMessage(50, "Renamed", $"{ e.OldName} renamed to { e.Name}", ToolTipIcon.None);
      }

      public void OnDeletedTSVH(object source, FileSystemEventArgs e)
      {
         // Specify what is done when a file is changed, created, or deleted.
         //ShowNotificationMessage(500, "Delete", $"{e.Name}", ToolTipIcon.None);
      }
      StreamReader reader;
      public void OnCreatedTSVH(object source, FileSystemEventArgs e)
      {
         Common.MoveFile(e, conf);
         /*ShowNotificationMessage(500, "Create", $"{e.Name}", ToolTipIcon.None);*/
         for (int count = 0; count <= 2000; count++)
         {
            try
            {
               DbContextService db = new DbContextService();

               //Console.WriteLine("Try to access file !!!");
               reader = new StreamReader(e.FullPath);
               //Console.WriteLine("Access file successfully !!!");
               string fileName = e.Name.Split('.')[0];
               string serial = fileName.Split('_')[0];
               // check Serial Cong To
               if (!CongToDAO.CheckSerialCongTo(serial))
               {
                  //ShowNotificationMessage(50, "Error", "Công to serial không tồn tại", ToolTipIcon.Error);
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
               var rs_Dt = DateTime.TryParseExact(dt_Str, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
               if (!rs_Dt)
               {
                  //ShowNotificationMessage(50, "Error", "Định dạng thời gian không đúng", ToolTipIcon.Error);
                  reader.Close();
                  return;
               }

               if (!ThongSoVanHanhDAO.checkExistTSVH(serial, dt))
               {
                  ThongSoVanHanh tsvh = new ThongSoVanHanh();
                  tsvh.Serial = serial;
                  tsvh.ThoiGianCongTo = dt;

                  var bool_rs = true;
                  var he_so_nhan = 0.001;//from W to kW

                  tsvh.P_Nhan = Common.ParseDouble(data[6 - 1].Split(',')[1], ref bool_rs) ;
                  tsvh.P_Giao = Common.ParseDouble(data[7 - 1].Split(',')[1], ref bool_rs);

                  tsvh.Q_Nhan = Common.ParseDouble(data[13 - 1].Split(',')[1], ref bool_rs) ;
                  tsvh.Q_Giao = Common.ParseDouble(data[14 - 1].Split(',')[1], ref bool_rs) ;

                  tsvh.P_Nhan_BT = Common.ParseDouble(data[19 - 1].Split(',')[1], ref bool_rs) ;
                  tsvh.P_Nhan_CD = Common.ParseDouble(data[20 - 1].Split(',')[1], ref bool_rs) ;
                  tsvh.P_Nhan_TD = Common.ParseDouble(data[21 - 1].Split(',')[1], ref bool_rs) ;

                  tsvh.P_Giao_BT = Common.ParseDouble(data[22 - 1].Split(',')[1], ref bool_rs) ;
                  tsvh.P_Giao_CD = Common.ParseDouble(data[23 - 1].Split(',')[1], ref bool_rs) ;
                  tsvh.P_Giao_TD = Common.ParseDouble(data[24 - 1].Split(',')[1], ref bool_rs) ;

                  tsvh.PhaseA_Amps = Common.ParseDouble(data[72 - 1].Split(',')[1], ref bool_rs);
                  tsvh.PhaseA_Volts = Common.ParseDouble(data[73 - 1].Split(',')[1], ref bool_rs);
                  tsvh.PhaseA_PowerFactor = Common.ParseDouble(data[77 - 1].Split(',')[1], ref bool_rs);
                  tsvh.PhaseA_Frequency = Common.ParseDouble(data[78 - 1].Split(',')[1], ref bool_rs);
                  tsvh.PhaseA_Angle = Common.ParseDouble(data[79 - 1].Split(',')[1], ref bool_rs);

                  tsvh.PhaseB_Amps = Common.ParseDouble(data[72 - 1].Split(',')[2], ref bool_rs);
                  tsvh.PhaseB_Volts = Common.ParseDouble(data[73 - 1].Split(',')[2], ref bool_rs);
                  tsvh.PhaseB_PowerFactor = Common.ParseDouble(data[77 - 1].Split(',')[2], ref bool_rs);
                  tsvh.PhaseB_Frequency = Common.ParseDouble(data[78 - 1].Split(',')[2], ref bool_rs);
                  tsvh.PhaseB_Angle = Common.ParseDouble(data[79 - 1].Split(',')[2], ref bool_rs);

                  tsvh.PhaseC_Amps = Common.ParseDouble(data[72 - 1].Split(',')[3], ref bool_rs);
                  tsvh.PhaseC_Volts = Common.ParseDouble(data[73 - 1].Split(',')[3], ref bool_rs);
                  tsvh.PhaseC_PowerFactor = Common.ParseDouble(data[77 - 1].Split(',')[3], ref bool_rs);
                  tsvh.PhaseC_Frequency = Common.ParseDouble(data[78 - 1].Split(',')[3], ref bool_rs);
                  tsvh.PhaseC_Angle = Common.ParseDouble(data[79 - 1].Split(',')[3], ref bool_rs);

                  tsvh.Phase_Rotation = data[80 - 1].Split(',')[4];

                  if (!bool_rs)
                  {
                     //ShowNotificationMessage(50, "Error", "Lỗi format number in file", ToolTipIcon.Error);
                     reader.Close();
                     return;
                  }

                  var rs = ThongSoVanHanhDAO.Create(tsvh);

                  if (!rs.Equals("success"))
                  {
                     //ShowNotificationMessage(50, "Error", rs, ToolTipIcon.Error);
                     reader.Close();
                     return;
                  }
               }

               //ShowNotificationMessage(50, "Success", "Reading file 'Thông số vận hành' finished!!!!", ToolTipIcon.Info);
               //reader.Close();
               break;
            }
            catch (IOException)
            {
               //Console.WriteLine("Wait to access file !!!");
               //ShowNotificationMessage(50, "Error", "Wait to access file !!!", ToolTipIcon.Error);
               Thread.Sleep(100);
            }
            catch (Exception ex)
            {
               //Console.WriteLine(ex.Message);
               //ShowNotificationMessage(50, "Error", ex.Message, ToolTipIcon.Error);
               break;
            }
            finally
            {
               try
               {
                  reader.Close();
               }
               catch (Exception ex)
               {
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

