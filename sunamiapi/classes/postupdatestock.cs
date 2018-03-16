using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class postupdatestock
    {
        private string _loogeduser;
        private int? _number;
        private string _method;
        private string _comment;
        private string _item;

        public string loogeduser { get => _loogeduser; set => _loogeduser = value; }
        public int? number { get => _number; set => _number = value; }
        public string method { get => _method; set => _method = value; }
        public string comment { get => _comment; set => _comment = value; }
        public string item { get => _item; set => _item = value; }
    }
}