using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class geteventlogsresponse
    {
        private string category;
        private string customerId;
        private DateTime? date;
        private string _event;
        private string loggedInUser;

        public string Category { get => category; set => category = value; }
        public string CustomerId { get => customerId; set => customerId = value; }
        public DateTime? Date { get => date; set => date = value; }
        public string Event { get => _event; set => _event = value; }
        public string LoggedInUser { get => loggedInUser; set => loggedInUser = value; }
    }
}