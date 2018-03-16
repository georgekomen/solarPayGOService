using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class postcontrollerbody
    {
        private string _imei;
        private string _sim;
        private string _provider;
        private string _version;
        private string _loogeduser;
        private string _action;

        public string imei { get => _imei; set => _imei = value; }
        public string sim { get => _sim; set => _sim = value; }
        public string provider { get => _provider; set => _provider = value; }
        public string version { get => _version; set => _version = value; }
        public string loogeduser { get => _loogeduser; set => _loogeduser = value; }
        public string action { get => _action; set => _action = value; }
    }
}