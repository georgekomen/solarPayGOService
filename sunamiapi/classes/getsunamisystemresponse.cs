using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class getsunamisystemresponse
    {
        private string owner;
        private string _installdate;
        private bool? active_Status;
        private string last_System_Communication;
        private string imei;
        private string systemPhoneNumber;

        public string Owner { get => owner; set => owner = value; }
        public string installdate { get => _installdate; set => _installdate = value; }
        public bool? Active_Status { get => active_Status; set => active_Status = value; }
        public string Last_System_Communication { get => last_System_Communication; set => last_System_Communication = value; }
        public string Imei { get => imei; set => imei = value; }
        public string SystemPhoneNumber { get => systemPhoneNumber; set => systemPhoneNumber = value; }
    }
}