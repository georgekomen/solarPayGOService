using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class payRecordClass2
    {
        private List<payrecordClass> payrecord;
        private int daily_invoice;
        private int total_invoice;
        private int? paid;
        private int? not_paid;
        private string name;
        private int? percent;

        public List<payrecordClass> Payrecord
        {
            get
            {
                return payrecord;
            }

            set
            {
                payrecord = value;
            }
        }

        public int Daily_invoice
        {
            get
            {
                return daily_invoice;
            }

            set
            {
                daily_invoice = value;
            }
        }

        public int Total_invoice
        {
            get
            {
                return total_invoice;
            }

            set
            {
                total_invoice = value;
            }
        }

        public int? Paid
        {
            get
            {
                return paid;
            }

            set
            {
                paid = value;
            }
        }

        public int? Not_paid
        {
            get
            {
                return not_paid;
            }

            set
            {
                not_paid = value;
            }
        }

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
    }
}