using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class postuninstallbody
    {
        private string _date1;
        private string _customer_id;
        private string _recorded_by;
        private string _reason;

        public string date1 { get => _date1; set => _date1 = value; }
        public string customer_id { get => _customer_id; set => _customer_id = value; }
        public string recorded_by { get => _recorded_by; set => _recorded_by = value; }
        public string reason { get => _reason; set => _reason = value; }
    }
}