using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class getissueresponse
    {
        private string _customer;
        private int id;
        private string _reporter;
        private string _issue;
        private DateTime? _date;
        private string _priority;
        private string _status;
        private DateTime? _solvedOn;
        private string _solvedBy;
        private string _comment;

        public string customer { get => _customer; set => _customer = value; }
        public int Id { get => id; set => id = value; }
        public string reporter { get => _reporter; set => _reporter = value; }
        public string issue { get => _issue; set => _issue = value; }
        public DateTime? date { get => _date; set => _date = value; }
        public string priority { get => _priority; set => _priority = value; }
        public string status { get => _status; set => _status = value; }
        public DateTime? solvedOn { get => _solvedOn; set => _solvedOn = value; }
        public string solvedBy { get => _solvedBy; set => _solvedBy = value; }
        public string comment { get => _comment; set => _comment = value; }
    }
}