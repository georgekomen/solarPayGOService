using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class paymentRatesClass
    {
        private string totalPaid;
        private string totalInvoice;
        private string percent;
        private List<paymentRatesClassPerClient> clientPayRates;

        public string TotalPaid { get => totalPaid; set => totalPaid = value; }
        public string TotalInvoice { get => totalInvoice; set => totalInvoice = value; }
        public string Percent { get => percent; set => percent = value; }
        public List<paymentRatesClassPerClient> ClientPayRates { get => clientPayRates; set => clientPayRates = value; }
    }
}