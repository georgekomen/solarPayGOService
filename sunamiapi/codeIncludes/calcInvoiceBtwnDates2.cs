/*using sunamiapi.classes;
using sunamiapi.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.codeIncludes
{
    public class calcInvoiceBtwnDates2
    {
        db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
        private string Comment;
        private int Pack_amount;
        private int Count;//invoice
        private int? Paid;
        private int System_deposit;
        private string St;
        private string En;
        private int? Percent = 0;
        private bool? Status;

        public int Pack_amount1
        {
            get
            {
                return Pack_amount;
            }

            set
            {
                Pack_amount = value;
            }
        }

        public int Count1
        {
            get
            {
                return Count;
            }

            set
            {
                Count = value;
            }
        }

        public int? Paid1
        {
            get
            {
                return Paid;
            }

            set
            {
                Paid = value;
            }
        }

        public int? Percent1
        {
            get
            {
                return Percent;
            }

            set
            {
                Percent = value;
            }
        }

        public void idcalcInvoiceBtwnDates(DateTime start, DateTime end, string id)
        {
            
            //get whole list of customers
            var tc1 = se.tbl_customer.Where(g => g.customer_id == id).Select(h => new { active = h.active_status, name = h.customer_name, id = h.customer_id, ind = h.install_date, pt = h.package_type, phone = h.phone_numbers, village = h.village_name }).Single();

            //check if active or uninstalled
            Comment = null;
            Count1 = 0;
            Pack_amount1 = 0;
            Paid1 = 0;
            System_deposit = 0;

            int days = 0;
            //get begin date --- start
            //get end date ----end
            //calculate number of days
            DateTime? instal_d = tc1.ind;
            if (instal_d >= start)
            {
                //if installation date is greater than the start picked date---it had not been
                //installed by then
                if (tc1.active == true)
                {
                    //if system is active then get number of days (end-install date)
                    days = (end - instal_d).Value.Days;
                    En = end.Date.ToString("dd/MM/yyyy");
                }
                else
                {
                    //if not active - days= uninstall date - install date
                    DateTime un_date = se.tbl_uninstalled_systems.FirstOrDefault(r => r.customer_id == tc1.id).uninstall_date;
                    days = (un_date - instal_d).Value.Days;
                    Comment += "\nUninstalled";
                    En = un_date.Date.ToString("dd/MM/yyyy");
                }
                //get how much he should pay per day
                string package_type = tc1.pt.ToString();
                Pack_amount1 = se.tbl_packages.FirstOrDefault(t => t.type == package_type).amount_per_day;

                //check if he is supposed to pay deposit
                DateTime dep_date = se.tbl_packages.FirstOrDefault(r => r.type == package_type).date_deposit;
                if (instal_d >= dep_date)
                {
                    //invoice installation deposit
                    System_deposit = se.tbl_packages.FirstOrDefault(t => t.type == package_type).deposit;
                    Comment += "\nInstallation deposit";
                }
                //add up all
                Count1 = Pack_amount1 * days;
                Count1 += System_deposit;
                //show period
                St = instal_d.Value.Date.ToString("dd/MM/yyyy");

                Paid1 = se.tbl_payments.Where(g => g.customer_id == tc1.id && g.payment_date >= instal_d && g.payment_date <= end).Sum(t => t.amount_payed);
                extraPackageInvoicing ep = new classes.extraPackageInvoicing();
                Count1 += ep.extr_invoice(instal_d, end, tc1.id);
                Comment += "\n" + ep.Comment;
            }
            else
            {
                //if installation date is greater than the start picked date---it had not been
                //installed by then
                if (tc1.active == true)
                {
                    //if system is active then get number of days (end-install date)
                    days = (end - start).Days;
                }
                else
                {
                    //if not active - days= uninstall date - install date
                    DateTime un_date = se.tbl_uninstalled_systems.FirstOrDefault(r => r.customer_id == tc1.id).uninstall_date;
                    days = (un_date - instal_d).Value.Days;
                    Comment += "\nUninstalled";
                }
                //get how much he should pay per day
                string package_type = tc1.pt.ToString();
                Pack_amount1 = se.tbl_packages.FirstOrDefault(t => t.type == package_type).amount_per_day;

                //check if he is supposed to pay deposit
                DateTime dep_date = se.tbl_packages.FirstOrDefault(r => r.type == package_type).date_deposit;
                if (instal_d >= dep_date)
                {
                    //invoice installation deposit
                    System_deposit = se.tbl_packages.FirstOrDefault(t => t.type == package_type).deposit;
                    Comment += "\nInstallation deposit";
                }

                //add up all
                Count1 = Pack_amount1 * days;
                //show period
                St = start.Date.ToString("dd/MM/yyyy");
                En = end.Date.ToString("dd/MM/yyyy");
                Paid1 = se.tbl_payments.Where(g => g.customer_id == tc1.id && g.payment_date >= start && g.payment_date <= end).Sum(t => t.amount_payed);

                //get extra package amount
                extraPackageInvoicing ep = new classes.extraPackageInvoicing();
                Count1 += ep.extr_invoice(instal_d, end, tc1.id);
                Comment += "\n" + ep.Comment;
            }
            try
            {
                Percent1 = (100 * Paid1) / Count1;
            }
            catch { }
            if (Percent1 == null)
            {
                Percent1 = 0;
            }
            if (Paid1 == null)
            {
                Paid1 = 0;
            }
            try
            {
                Status = se.tbl_system.FirstOrDefault(g => g.customer_id == tc1.id).active_status;
            }catch(Exception n)
            {
                Status = null;
            }
        }
    }
}*/