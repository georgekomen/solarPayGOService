using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class postexpensebody
    {
        private string _category;
        private string _amount;
        private string _recipient;
        private string _dateset;
        private string _vendor;
        private string _account;
        private string _ref_code;
        private string _recordedBy;
        private string _pic1;

        public string category { get => _category; set => _category = value; }
        public string amount { get => _amount; set => _amount = value; }
        public string recipient { get => _recipient; set => _recipient = value; }
        public string dateset { get => _dateset; set => _dateset = value; }
        public string vendor { get => _vendor; set => _vendor = value; }
        public string account { get => _account; set => _account = value; }
        public string ref_code { get => _ref_code; set => _ref_code = value; }
        public string recordedBy { get => _recordedBy; set => _recordedBy = value; }
        public string pic1 { get => _pic1; set => _pic1 = value; }
    }
}