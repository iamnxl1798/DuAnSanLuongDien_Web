using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuAn.Models.CustomModel
{
    public class MissingDataStatus
    {
        public string date { get; set; }
        public string name { get; set; }
        public int status { get; set; }

        public string getStatusString
        {
            get
            {
                if (status == 0)
                {
                    return "Đang cập nhật";
                }
                else if (status == 1)
                {
                    return "Đã cập nhật";
                }
                else
                {
                    return "Chưa cập nhật";
                }
            }
        }
        public string classAttribute { get {
                if (status == 0)
                {
                    return "warning";
                }
                else if (status == 1)
                {
                    return "success";
                }
                else
                {
                    return "primary";
                }
            } }
    }
}