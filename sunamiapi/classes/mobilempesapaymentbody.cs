using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class mobilempesapaymentbody
    {
        private string _address;
        private string _imei;
        private string _msg;

        public string address { get => _address; set => _address = value; }
        public string imei { get => _imei; set => _imei = value; }
        public string msg { get => _msg; set => _msg = value; }
    }
}