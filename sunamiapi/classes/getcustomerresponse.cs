using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class getcustomerresponse
    {
        private string name;
        private string iD;
        private string customer_id;
        private string occupation;
        private string mobile;
        private string mobile2;
        private string mobile3;
        private string _village;
        private string _location;
        private string _city;
        private DateTime? _installdate;
        private string witness;
        private string witness_ID;
        private bool? _status;
        private string package;

        public string Name { get => name; set => name = value; }
        public string ID { get => iD; set => iD = value; }
        public string Occupation { get => occupation; set => occupation = value; }
        public string Mobile { get => mobile; set => mobile = value; }
        public string Mobile2 { get => mobile2; set => mobile2 = value; }
        public string Mobile3 { get => mobile3; set => mobile3 = value; }
        public string village { get => _village; set => _village = value; }
        public string location { get => _location; set => _location = value; }
        public string city { get => _city; set => _city = value; }
        public DateTime? installdate { get => _installdate; set => _installdate = value; }
        public string Witness { get => witness; set => witness = value; }
        public string Witness_ID { get => witness_ID; set => witness_ID = value; }
        public bool? status { get => _status; set => _status = value; }
        public string Package { get => package; set => package = value; }
        public string Customer_id { get => customer_id; set => customer_id = value; }
    }
}