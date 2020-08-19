using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ServiceTool.Model
{
    public class ConfigClass
    {
        public string ThuMucQuet { get; set; }
        public string ThuMucChuyen { get; set; }
        public bool AutoRun { get; set; }
        public ConfigClass(String tmq, string tmc, bool at)
        {
            ThuMucChuyen = tmc;
            ThuMucQuet = tmq;
            AutoRun = at;
        }
        public ConfigClass()
        {
        }

        static public ConfigClass GetConfig()
        {
            ConfigClass conf;
            StreamReader reader = new StreamReader(@"..\..\Config.json");
            String str = reader.ReadToEnd();
            reader.Close();
            if (str == "" || str == null)
            {
                conf = new ConfigClass(@"D:\E-Solution\WebGiamSatSanLuongDien\DocDuLieuCongTo\TestTheoDoi", @"D:\E-Solution\WebGiamSatSanLuongDien\DocDuLieuCongTo\TestChuyen", true);
                SetConfig(conf);
            }
            else
            {
                conf = JsonConvert.DeserializeObject<ConfigClass>(str);
            }

            return conf;
        }

        static public void SetConfig(ConfigClass conf)
        {
            StreamWriter writer = new StreamWriter(@"..\..\Config.json");
            writer.WriteLine(JsonConvert.SerializeObject(conf));
            writer.Close();
        }
    }
}
