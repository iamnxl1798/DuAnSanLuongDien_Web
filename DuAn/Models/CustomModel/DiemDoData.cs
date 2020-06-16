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
        public double sumKwhGiao { get; set; }
        public double sumKwhNhan { get; set; }
        public double sumKvarhGiao { get; set; }
        public double sumKvarhNhan { get; set; } 
    }
}