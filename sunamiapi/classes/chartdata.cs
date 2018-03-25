using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class chartdata
    {
        private List<int?> data1;
        private string label1;

        public List<int?> data { get => data1; set => data1 = value; }
        public string label { get => label1; set => label1 = value; }
    }
}