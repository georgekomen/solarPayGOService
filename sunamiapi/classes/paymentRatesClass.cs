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

        public string TotalPaid
        {
            get
            {
                return totalPaid;
            }

            set
            {
                totalPaid = value;
            }
        }

        public string TotalInvoice
        {
            get
            {
                return totalInvoice;
            }

            set
            {
                totalInvoice = value;
            }
        }

        public string Percent
        {
            get
            {
                return percent;
            }

            set
            {
                percent = value;
            }
        }

        public List<paymentRatesClassPerClient> ClientPayRates
        {
            get
            {
                return clientPayRates;
            }

            set
            {
                clientPayRates = value;
            }
        }
    }
}