using sunamiapi.classes;
using sunamiapi.Controllers.api;
using sunamiapi.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.codeIncludes
{
    public class getDataLists
    {
        public List<Icustomer> getCustomerLocations(db_a0a592_sunamiEntities se)
        {
            List<Icustomer> li = new List<Icustomer>();
            var list =
            (from tc in se.tbl_customer
             where tc.active_status == true
             orderby tc.customer_name ascending
             select new { Customer_Name = tc.customer_name, Customer_Id = tc.customer_id, Customer_Lat = tc.lat, Customer_Lon = tc.lon }
            ).ToList();
            
            foreach (var i in list)
            {

                //filter out those with no gps coordinates taken
                if (!string.IsNullOrEmpty(i.Customer_Lat))
                {
                    bool? stt=false;//false is on
                    try
                    {
                       stt = se.tbl_system.FirstOrDefault(g => g.customer_id == i.Customer_Id).active_status;
                    }
                    catch
                    {

                    }
                    string selected = null;
                    string off = "./contents/icons/notPaid.png";
                    string on = "./contents/icons/onOk.png";

                    //sort icon urls depending on status
                    if (stt == false)
                    {
                        selected = on;
                    }
                    if (stt == true)
                    {
                        selected = off;
                    }

                    li.Add(new Icustomer()
                    {
                        Customer_Name = i.Customer_Name,
                        Customer_Id = i.Customer_Id,
                        Customer_Lat = float.Parse(i.Customer_Lat),
                        Customer_Lon = float.Parse(i.Customer_Lon),
                        Customer_Status = stt,
                        Customer_Icon = selected

                    });
                }
            }

            return li;
        }

        public List<summaryReport> getPaymentSummaryReport(DateTime start, DateTime end, DateTime beginDate, db_a0a592_sunamiEntities se)
        {
            int customerNumber = se.tbl_customer.Where(k => k.active_status == true).Count();
            int issues = se.tbl_issues.Where(g => g.date >= start && g.date <= end).Count();
            int maintenance = se.tbl_issues.Where(g => g.date >= start && g.date <= end && g.status == "solved").Count();

            List<tbl_customer> lst = new List<tbl_customer>();
            lst = se.tbl_customer.Where(rr => rr.install_date <= end).ToList();
            CustomersController cc = new CustomersController();
            List<paymentRatesClassPerClient> res1 =  cc.calcInvoiceBtwnDatesm(start, end, lst);
            int? invoice = res1.Sum(t => t.Invoice);
            int? paid = res1.Sum(r => r.Amount);
            int? percent = 0;
            try
            {
                percent = (paid * 100) / invoice;
            } catch {
                percent = 0;
            }

            int? payrate = percent;
            int newInstallations = se.tbl_customer.Where(f => f.install_date >= start && f.install_date <= end).Count();
            int remoteControls = se.tbl_system.Where(g => g.imei_number != "" && g.imei_number.Length >= 13).Count();
            int uninstallations = se.tbl_uninstalled_systems.Where(h => h.uninstall_date >= start && h.uninstall_date <= end).Count();

            List<summaryReport> summary = new List<summaryReport>();
            summary.Add(new summaryReport()
            {
                CustomerNumber = customerNumber,
                Issues = issues,
                Maintenance = maintenance,
                Payrate = payrate,
                NewInstallations = newInstallations,
                RemoteControls = remoteControls,
                Uninstallations = uninstallations
            });
            return summary;
        }
    }
}