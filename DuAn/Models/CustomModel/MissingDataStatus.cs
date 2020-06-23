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
        public string status { get; set; }
        public string classAttribute { get { return status == "Chưa cập nhật" ? "primary" : "warning"; } }
    }
}