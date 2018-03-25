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
        private List<MonthPayBreakDown> monthPayBreakDown;

     
        public List<MonthPayBreakDown> MonthPayBreakDown { get => monthPayBreakDown; set => monthPayBreakDown = value; }
        public List<chartdata> LineChartData { get => lineChartData; set => lineChartData = value; }
        public List<string> LineChartLabels { get => lineChartLabels; set => lineChartLabels = value; }
    }

    public class MonthPayBreakDown{
        private string month;
        private string invoice;
        private string paid;
        private string performance;
        private string dateRanges;

        public string Month { get => month; set => month = value; }
        public string Invoice { get => invoice; set => invoice = value; }
        public string Paid { get => paid; set => paid = value; }
        public string Performance { get => performance; set => performance = value; }
        public string DateRanges { get => dateRanges; set => dateRanges = value; }
    }
}