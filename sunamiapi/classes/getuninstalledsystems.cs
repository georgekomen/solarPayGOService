using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class getuninstalledsystems
    {
        private string name;
        private string id;
        private DateTime? _install_date;
        private DateTime? uninstalled_on;
        private string reason;
        private string _unistalledBy;
        private string _previousRecords;

        public string Name { get => name; set => name = value; }
        public string Id { get => id; set => id = value; }
        public DateTime? install_date { get => _install_date; set => _install_date = value; }
        public DateTime? Uninstalled_on { get => uninstalled_on; set => uninstalled_on = value; }
        public string Reason { get => reason; set => reason = value; }
        public string unistalledBy { get => _unistalledBy; set => _unistalledBy = value; }
        public string previousRecords { get => _previousRecords; set => _previousRecords = value; }
    }
}