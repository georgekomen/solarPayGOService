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
        private int invoice;
        private int? percent;
        private string from;
        private string to;
        private string comment;
        private bool? status;
        private string phone;
        private string village;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public int? Amount
        {
            get
            {
                return amount;
            }

            set
            {
                amount = value;
            }
        }

        public int Invoice
        {
            get
            {
                return invoice;
            }

            set
            {
                invoice = value;
            }
        }

        public int? Percent
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

        public string From
        {
            get
            {
                return from;
            }

            set
            {
                from = value;
            }
        }

        public string To
        {
            get
            {
                return to;
            }

            set
            {
                to = value;
            }
        }

        public string Comment
        {
            get
            {
                return comment;
            }

            set
            {
                comment = value;
            }
        }
        
        public bool? Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
            }
        }

        public string Phone
        {
            get
            {
                return phone;
            }

            set
            {
                phone = value;
            }
        }

        public string Village
        {
            get
            {
                return village;
            }

            set
            {
                village = value;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }
    }
}