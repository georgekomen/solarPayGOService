using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class getsunamicontroller
    {
        private string imei;
        private string sim_Number;
        private string provider;
        private string version;
        private DateTime? reg_Date;
        private string recorded_By;

        public string Imei { get => imei; set => imei = value; }
        public string Sim_Number { get => sim_Number; set => sim_Number = value; }
        public string Provider { get => provider; set => provider = value; }
        public string Version { get => version; set => version = value; }
        public DateTime? Reg_Date { get => reg_Date; set => reg_Date = value; }
        public string Recorded_By { get => recorded_By; set => recorded_By = value; }
    }
}