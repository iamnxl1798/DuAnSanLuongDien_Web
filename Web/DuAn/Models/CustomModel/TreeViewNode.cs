using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuAn.Models.CustomModel
{
    public class TreeViewNode
    {
        public string id { get; set; }

        public string text { get; set; }

        public string parent { get; set; }
        public Dictionary<String, bool> state { get; set; }
    }
}