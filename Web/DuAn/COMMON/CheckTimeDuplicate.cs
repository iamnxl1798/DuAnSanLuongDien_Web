using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuAn.COMMON
{
   public class CheckTimeDuplicate
   {
      public static bool CheckTwoPeriodsIsDuplicate(DateTime dt_start_1, DateTime? dt_end_1, DateTime dt_start_2, DateTime? dt_end_2)
      {
         if (dt_end_1.HasValue && dt_end_2.HasValue && (dt_start_1 > dt_end_1 || dt_start_2 > dt_end_2))
         {
            throw new Exception("Thời gian bắt đầu phải nhỏ hơn hoặc bằng thời gian kết thúc");
         }

         if (!dt_end_1.HasValue && !dt_end_2.HasValue)
         {
            return true;
         }
         else if (!dt_end_1.HasValue && dt_end_2.HasValue)
         {
            if (dt_start_2 <= dt_end_1 && dt_end_2 >= dt_start_1)
            {
               return true;
            }
         }
         else if (dt_end_1.HasValue && !dt_end_2.HasValue)
         {
            if (dt_start_2 <= dt_end_1)
            {
               return true;
            }
         }
         else
         {
            if (dt_start_2 <= dt_end_1 && dt_end_2 >= dt_start_1)
            {
               return true;
            }
         }
         return false;

      }
   }
}