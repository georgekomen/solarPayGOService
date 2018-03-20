using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class postnewcustomer
    {
        private string _agentcode;
        private string _witnessnumber;
        private DateTime? _installdate;
        private bool? _status;
        private string _id;
        private string _box;
        private string _city;
        private string _latG;
        private string _lonG;
        private string _name;
        private string _number1;
        private string _number2;
        private string _number3;
        private string _occupation;
        private string _village;
        private string _witness;
        private string _witnessid;
        private string _description;
        private string _recordedBy;
        private string _date1;
        private string _location;
        private string _country;
        private string _package;
        private string _gender;

        public string id { get => _id; set => _id = value; }
        public string box { get => _box; set => _box = value; }
        public string city { get => _city; set => _city = value; }
        public string latG { get => _latG; set => _latG = value; }
        public string lonG { get => _lonG; set => _lonG = value; }
        public string name { get => _name; set => _name = value; }
        public string number1 { get => _number1; set => _number1 = value; }
        public string number2 { get => _number2; set => _number2 = value; }
        public string number3 { get => _number3; set => _number3 = value; }
        public string occupation { get => _occupation; set => _occupation = value; }
        public string village { get => _village; set => _village = value; }
        public string witness { get => _witness; set => _witness = value; }
        public string witnessid { get => _witnessid; set => _witnessid = value; }
        public string description { get => _description; set => _description = value; }
        public string recordedBy { get => _recordedBy; set => _recordedBy = value; }
        public string date1 { get => _date1; set => _date1 = value; }
        public string location { get => _location; set => _location = value; }
        public string country { get => _country; set => _country = value; }
        public string package { get => _package; set => _package = value; }
        public DateTime? installdate { get => _installdate; set => _installdate = value; }
        public bool? status { get => _status; set => _status = value; }
        public string agentcode { get => _agentcode; set => _agentcode = value; }
        public string witnessnumber { get => _witnessnumber; set => _witnessnumber = value; }
        public string gender { get => _gender; set => _gender = value; }
    }
}