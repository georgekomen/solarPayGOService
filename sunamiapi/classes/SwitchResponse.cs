using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class SwitchResponse
    {
        private string address;
        private string imei;
        private string status;

        public string Address { get => address; set => address = value; }
        public string Status { get => status; set => status = value; }
        public string Imei { get => imei; set => imei = value; }
    }
}