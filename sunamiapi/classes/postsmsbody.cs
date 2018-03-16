using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class postsmsbody
    {
        private string _message;
        private List<recipients> _recipients;

        public string message { get => _message; set => _message = value; }
        public List<recipients> recipients { get => _recipients; set => _recipients = value; }
    }

    public class recipients
    {
        private string _idnumber;
        private string invoice;
        private string paid;

        public string idnumber { get => _idnumber; set => _idnumber = value; }
        public string Invoice { get => invoice; set => invoice = value; }
        public string Paid { get => paid; set => paid = value; }
    }
}