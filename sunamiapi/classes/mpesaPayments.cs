using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class mpesaPayments
    {
        //date //ref //amount //number //message
        DateTime? date;
        string reference;
        string amount;
        string number;
        string message;

        public DateTime? Date
        {
            get
            {
                return date;
            }

            set
            {
                date = value;
            }
        }

        public string Reference
        {
            get
            {
                return reference;
            }

            set
            {
                reference = value;
            }
        }

        public string Amount
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

        public string Number
        {
            get
            {
                return number;
            }

            set
            {
                number = value;
            }
        }

        public string Message
        {
            get
            {
                return message;
            }

            set
            {
                message = value;
            }
        }
    }
}