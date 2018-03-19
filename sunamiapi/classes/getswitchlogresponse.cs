using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class getswitchlogresponse
    {
        private string name;
        private string id;
        private string imei;
        private string sim;
        private DateTime? switch_Off_Date;
        private string switch_Off_payrate;
        private string switch_Off_by;
        private DateTime? switch_On_Date;
        private string switch_on_payrate;
        private string switch_on_by;

        public string Name { get => name; set => name = value; }
        public string Id { get => id; set => id = value; }
        public string Imei { get => imei; set => imei = value; }
        public string Sim { get => sim; set => sim = value; }
        public DateTime? Switch_Off_Date { get => switch_Off_Date; set => switch_Off_Date = value; }
        public string Switch_Off_payrate { get => switch_Off_payrate; set => switch_Off_payrate = value; }
        public string Switch_Off_by { get => switch_Off_by; set => switch_Off_by = value; }
        public DateTime? Switch_On_Date { get => switch_On_Date; set => switch_On_Date = value; }
        public string Switch_on_payrate { get => switch_on_payrate; set => switch_on_payrate = value; }
        public string Switch_on_by { get => switch_on_by; set => switch_on_by = value; }
    }
}