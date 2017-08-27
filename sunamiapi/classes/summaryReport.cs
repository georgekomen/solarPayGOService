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

        public int CustomerNumber
        {
            get
            {
                return customerNumber;
            }

            set
            {
                customerNumber = value;
            }
        }

        public int Issues
        {
            get
            {
                return issues;
            }

            set
            {
                issues = value;
            }
        }

        public int Maintenance
        {
            get
            {
                return maintenance;
            }

            set
            {
                maintenance = value;
            }
        }

        public int? Payrate
        {
            get
            {
                return payrate;
            }

            set
            {
                payrate = value;
            }
        }

        public int NewInstallations
        {
            get
            {
                return newInstallations;
            }

            set
            {
                newInstallations = value;
            }
        }

        public int RemoteControls
        {
            get
            {
                return remoteControls;
            }

            set
            {
                remoteControls = value;
            }
        }

        public int Uninstallations
        {
            get
            {
                return uninstallations;
            }

            set
            {
                uninstallations = value;
            }
        }
    }
}