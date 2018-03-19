using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class StartEndDate
    {
        private string _endDate;
        private string _startDate;

        public string endDate { get => _endDate; set => _endDate = value; }
        public string startDate { get => _startDate; set => _startDate = value; }
    }
}