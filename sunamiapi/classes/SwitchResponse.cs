using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class SwitchResponse
    {
        private string address;
        private string IMEI;
        private string status;

        public string Address { get => address; set => address = value; }
        public string IMEI1 { get => IMEI; set => IMEI = value; }
        public string Status { get => status; set => status = value; }
    }
}