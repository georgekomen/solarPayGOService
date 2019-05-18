using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class summaryReport
    {
        private int customerNumber;
        private int issues;
        private int maintenance;
        private int? payrate;
        private int newInstallations;
        private int remoteControls;
        private int uninstallations;

        public int CustomerNumber { get => customerNumber; set => customerNumber = value; }
        public int Issues { get => issues; set => issues = value; }
        public int Maintenance { get => maintenance; set => maintenance = value; }
        public int? Payrate { get => payrate; set => payrate = value; }
        public int NewInstallations { get => newInstallations; set => newInstallations = value; }
        public int RemoteControls { get => remoteControls; set => remoteControls = value; }
        public int Uninstallations { get => uninstallations; set => uninstallations = value; }
    }
}