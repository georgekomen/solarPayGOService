using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class Invoicecustomerbody
    {
        private string _customerId;
        private string _item;
        private string _loogedUser;
        private string _invoiceDate;

        public string customerId { get => _customerId; set => _customerId = value; }
        public string item { get => _item; set => _item = value; }
        public string loogedUser { get => _loogedUser; set => _loogedUser = value; }
        public string invoiceDate { get => _invoiceDate; set => _invoiceDate = value; }
    }
}