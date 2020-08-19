using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

namespace DuAn.Models.CustomModel
{
    public class NumberOfMissingData
    {
        public string type { get; set; }
        public int done { get; set; }
        public int notYet { get; set; }
    }
}