using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class paymentRatesClassPerClient
    {
        private string description;
        private string name;
        private string id;
        private int? amount;
        private int? invoice;
        private int? percent;
        private string from;
        private string to;
        private string comment;
        private bool? status;
        private string phone;
        private string village;

        public string Description { get => description; set => description = value; }
        public string Name { get => name; set => name = value; }
        public string Id { get => id; set => id = value; }
        public int? Amount { get => amount; set => amount = value; }
        public int? Invoice { get => invoice; set => invoice = value; }
        public int? Percent { get => percent; set => percent = value; }
        public string From { get => from; set => from = value; }
        public string To { get => to; set => to = value; }
        public string Comment { get => comment; set => comment = value; }
        public bool? Status { get => status; set => status = value; }
        public string Phone { get => phone; set => phone = value; }
        public string Village { get => village; set => village = value; }
    }
}