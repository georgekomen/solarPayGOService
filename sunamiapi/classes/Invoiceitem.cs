using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class Invoiceitem
    {
        private string item;
        private int deposit;
        private int amount;
        private int payDays;

        public string Item { get => item; set => item = value; }
        public int Deposit { get => deposit; set => deposit = value; }
        public int Amount { get => amount; set => amount = value; }
        public int PayDays { get => payDays; set => payDays = value; }
    }
}