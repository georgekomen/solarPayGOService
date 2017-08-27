using sunamiapi.Controllers.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class paychartclass
    {
        private List<chartdata> lineChartData;
        private List<string> lineChartLabels;

        public List<string> LineChartLabels
        {
            get
            {
                return lineChartLabels;
            }

            set
            {
                lineChartLabels = value;
            }
        }
        public List<chartdata> LineChartData
        {
            get
            {
                return lineChartData;
            }

            set
            {
                lineChartData = value;
            }
        }
    }
}