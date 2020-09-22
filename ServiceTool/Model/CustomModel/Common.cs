using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTool.Model.CustomModel
{
   public static class Common
   {
      public static double ParseDouble(string double_str, ref bool check_bool)
      {
         if (!check_bool)
         {
            return 0;
         }
         var db = 0.0;
         check_bool = double.TryParse(double_str.Trim().Replace(' ', ','), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out db);
         if (check_bool)
         {
            return db;
         }
         return 0;
      }

      public static void MoveFile(FileSystemEventArgs e, ConfigClass conf)
      {
         for (int i = 0; i < 20000; i++)
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
               //ShowNotificationMessage(50, "Di chuyển file !!!", "Thành công", ToolTipIcon.None);
               break;
            }
            catch (Exception ex)
            {
               //ShowNotificationMessage(50, "Error !!!", ex.Message, ToolTipIcon.Error);
            }
         }
      }
   }
}
