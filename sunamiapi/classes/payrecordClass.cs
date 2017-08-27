using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
	public class payrecordClass
	{
        private int? amount;
        private string payDate;
        private string payMethod;
        private string code;
        private string mpesaNumber;
        private string receiver;

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

        public string PayDate
        {
            get
            {
                return payDate;
            }

            set
            {
                payDate = value;
            }
        }

        public string PayMethod
        {
            get
            {
                return payMethod;
            }

            set
            {
                payMethod = value;
            }
        }

        public string Code
        {
            get
            {
                return code;
            }

            set
            {
                code = value;
            }
        }

        public string MpesaNumber
        {
            get
            {
                return mpesaNumber;
            }

            set
            {
                mpesaNumber = value;
            }
        }

        public string Receiver
        {
            get
            {
                return receiver;
            }

            set
            {
                receiver = value;
            }
        }

    }
}