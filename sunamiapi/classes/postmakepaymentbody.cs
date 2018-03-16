using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class postmakepaymentbody
    {
        private string _loggedUser;
        private string payMode;
        private string code;
        private string customer_Id;
        private string _bankname;
        private string _date1;
        private string _amount;

        public string loggedUser { get => _loggedUser; set => _loggedUser = value; }
        public string PayMode { get => payMode; set => payMode = value; }
        public string Code { get => code; set => code = value; }
        public string Customer_Id { get => customer_Id; set => customer_Id = value; }
        public string bankname { get => _bankname; set => _bankname = value; }
        public string date1 { get => _date1; set => _date1 = value; }
        public string amount { get => _amount; set => _amount = value; }
    }
}