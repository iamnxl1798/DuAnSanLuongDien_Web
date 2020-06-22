using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuAn.Models.CustomModel
{
    public class AdminModel
    {
        public List<DiemDo> listDiemDo { get; set; }
        public List<Kenh> listKenh { get; set; }
        public DateTime getLastDate { get; set; }
        public int getDate{ get { return getLastDate.Day; } }
        public List<MissingDataStatus> missingData { get; set; }

    }
}