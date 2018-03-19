using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class postlinkcontroller
    {
        private string _imei;
        private string _loogeduser;
        private string _customer_id;

        public string imei { get => _imei; set => _imei = value; }
        public string loogeduser { get => _loogeduser; set => _loogeduser = value; }
        public string customer_id { get => _customer_id; set => _customer_id = value; }
    }
}