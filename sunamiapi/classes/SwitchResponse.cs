using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class SwitchResponse
    {
        private string _address;
        private string _imei;
        private string _status;

        public string address { get => _address; set => _address = value; }
        public string imei { get => _imei; set => _imei = value; }
        public string status { get => _status; set => _status = value; }
    }
}