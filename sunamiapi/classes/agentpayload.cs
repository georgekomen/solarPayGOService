using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class agentpayload
    {
        private string _firstname;
        private string _lastname;
        private DateTime _dateofenrolment;
        private string _idnumber;
        private string _country;
        private string _phonenumber;
        private string _email;
        private string _location;
        
        public string firstname { get => _firstname; set => _firstname = value; }
        public string lastname { get => _lastname; set => _lastname = value; }
        public DateTime dateofenrolment { get => _dateofenrolment; set => _dateofenrolment = value; }
        public string idnumber { get => _idnumber; set => _idnumber = value; }
        public string country { get => _country; set => _country = value; }
        public string phonenumber { get => _phonenumber; set => _phonenumber = value; }
        public string email { get => _email; set => _email = value; }
        public string location { get => _location; set => _location = value; }
    }
}