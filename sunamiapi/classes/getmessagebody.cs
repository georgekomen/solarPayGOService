using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class getmessagebody
    {
        private string name;
        private string message;
        private DateTime? date;

        public string Name { get => name; set => name = value; }
        public string Message { get => message; set => message = value; }
        public DateTime? Date { get => date; set => date = value; }
    }
}