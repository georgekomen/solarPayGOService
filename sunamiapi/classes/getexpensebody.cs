using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class getexpensebody
    {
        private int id;
        private string account;
        private string amount;
        private DateTime? date;
        private string description;
        private bool image;
        private string recipient;
        private string _vendor;
        private string refCode;

        public int Id { get => id; set => id = value; }
        public string Account { get => account; set => account = value; }
        public string Amount { get => amount; set => amount = value; }
        public DateTime? Date { get => date; set => date = value; }
        public string Description { get => description; set => description = value; }
        public bool Image { get => image; set => image = value; }
        public string Recipient { get => recipient; set => recipient = value; }
        public string vendor { get => _vendor; set => _vendor = value; }
        public string RefCode { get => refCode; set => refCode = value; }
    }
}