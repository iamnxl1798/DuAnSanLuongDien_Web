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
         //ShowNotificationMessage(500, "Changed", $"{e.Name}", ToolTipIcon.None);

      }

      public void OnRenamedCSC(object source, RenamedEventArgs e)
      {
         // Specify what is done when a file is renamed.
         //Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
         //ShowNotificationMessage(50, "Renamed", $"{ e.OldName} renamed to { e.Name}", ToolTipIcon.None);
      }

      public void OnDeleteCSC(object source, FileSystemEventArgs e)
      {
         // Specify what is done when a file is changed, created, or deleted.
         //ShowNotificationMessage(500, "Delete", $"{e.Name}", ToolTipIcon.None);
      }
      StreamReader reader;
      public void OnCreatedCSC(object source, FileSystemEventArgs e)
      {
         Common.MoveFile(e, conf);
         /*ShowNotificationMessage(500, "Create", $"{e.Name}", ToolTipIcon.None);*/
         for (var count = 0; count < 20000; count++)
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
               int numberRecord = int.Parse(Math.Floor((Decimal)data.Count / 74).ToString());

               for (int i = 0; i < 3; i++)
               {
                  data.RemoveAt(0);
               }

               for (int i = 0; i < numberRecord; i++)
               {
                  DateTime dt = DateTime.Parse(data[i * 74 + 72 - 1].Split(',')[1]);
                  if (!ChiSoChotDAO.checkExistCSC(serial, dt))
                  {
                     ChiSoChot csc = new ChiSoChot();
                     csc.CongToSerial = serial;
                     csc.thang = dt;

                     var bool_check = true;

                     csc.TongGiao = Common.ParseDouble(data[i * 74 + 7 - 1].Split(',')[1], ref bool_check) ;
                     csc.PhanKhangGiao = Common.ParseDouble(data[i * 74 + 14 - 1].Split(',')[1], ref bool_check) ;
                     csc.BinhThuongGiao = Common.ParseDouble(data[i * 74 + 22 - 1].Split(',')[1], ref bool_check) ;
                     csc.CaoDiemGiao = Common.ParseDouble(data[i * 74 + 23 - 1].Split(',')[1], ref bool_check) ;
                     csc.ThapDiemGiao = Common.ParseDouble(data[i * 74 + 24 - 1].Split(',')[1], ref bool_check) ;

                     csc.TongNhan = Common.ParseDouble(data[i * 74 + 6 - 1].Split(',')[1], ref bool_check) ;
                     csc.PhangKhangNhan = Common.ParseDouble(data[i * 74 + 13 - 1].Split(',')[1], ref bool_check) ;
                     csc.BinhThuongNhan = Common.ParseDouble(data[i * 74 + 19 - 1].Split(',')[1], ref bool_check) ;
                     csc.CaoDiemNhan = Common.ParseDouble(data[i * 74 + 20 - 1].Split(',')[1], ref bool_check) ;
                     csc.ThapDiemNhan = Common.ParseDouble(data[i * 74 + 21 - 1].Split(',')[1], ref bool_check) ;

                     var rs = ChiSoChotDAO.Create(csc);
                     if (!rs.Equals("success"))
                     {
                        //ShowNotificationMessage(50, "Error", rs, ToolTipIcon.Info);
                        reader.Close();
                        return;
                     }
                  }
               }

               //ShowNotificationMessage(50, "Success", "Reading file 'Chỉ Số Chốt' finished!!!!", ToolTipIcon.Info);
               //reader.Close();
               break;
            }
            catch (IOException)
            {
               //Console.WriteLine("Wait to access file !!!");
               //ShowNotificationMessage(50, "Error", "Wait to access file !!!", ToolTipIcon.Error);
               Thread.Sleep(100);
            }
            catch
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
               catch
               {
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
