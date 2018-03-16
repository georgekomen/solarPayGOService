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
        int? amount;
        string number;
        string message;
        string _payMode;
        string recordedBy;
        int id;
        string customer_Name;
        string customer_Id;

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

        public string payMode { get => _payMode; set => _payMode = value; }
        public string RecordedBy { get => recordedBy; set => recordedBy = value; }
        public int Id { get => id; set => id = value; }
        public string Customer_Name { get => customer_Name; set => customer_Name = value; }
        public string Customer_Id { get => customer_Id; set => customer_Id = value; }
    }
}