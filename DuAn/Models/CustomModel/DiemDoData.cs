using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuAn.Models.CustomModel
{
    public class DiemDoData
    {
        public string tenDiemDo { get; set; }
        public string tinhChat { get; set; }
        public int maDiemDo { get; set; }
        public int thuTuHienThi { get; set; }
        public List<double> kwhGiao { get; set; }
        public List<double> kwhNhan { get; set; }
        public List<double> kvarhGiao { get; set; }
        public List<double> kvarhNhan { get; set; }
        public double sumKwhGiao { get { return kwhGiao.Sum(); } }
        public double sumKwhNhan { get { return kwhNhan.Sum(); } }
        public double sumKvarhGiao { get { return kvarhGiao.Sum(); } }
        public double sumKvarhNhan { get { return kvarhNhan.Sum(); } }
    }
}