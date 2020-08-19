using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuAn.Models.CustomModel
{
    public class RequestPagingModel
    {
        public int length { get; set; }
        public int start { get; set; }
        public string searchValue { get; set; }
        public string sortColumnName { get; set; }
        public string sortDirection { get; set; }
        public string draw { get; set; }
    }
}