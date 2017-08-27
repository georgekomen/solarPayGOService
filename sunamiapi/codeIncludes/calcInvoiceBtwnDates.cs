using sunamiapi.classes;
using sunamiapi.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.codeIncludes
{
    public class calcInvoiceBtwnDates
    {
        db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();

        public int? calcPayRate(DateTime start, DateTime end)//for graph
        {
            int Total_count = 0;//invoice
            int? Total_paid = 0;
            int? TotalPercent = 0;
            string Comment;
            int Pack_amount;
            int Count;//invoice
            int? Paid;
            int System_deposit;
            string St;
            string En;
            int? Percent = 0;

            //get whole list of customers
            var list2 = se.tbl_customer.Where(g => g.install_date <= end && g.active_status ==true).Select(h => new { active = h.active_status, name = h.customer_name, id = h.customer_id, ind = h.install_date, pt = h.package_type }).ToList();
            foreach (var tc1 in list2)
            {
                //check if active or uninstalled
                Comment = null;
                Count = 0;
                Pack_amount = 0;
                Paid = 0;
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
                    Pack_amount = se.tbl_packages.FirstOrDefault(t => t.type == package_type).amount_per_day;

                    //check if he is supposed to pay deposit
                    DateTime dep_date = se.tbl_packages.FirstOrDefault(r => r.type == package_type).date_deposit;
                    if (instal_d >= dep_date)
                    {
                        //invoice installation deposit
                        System_deposit = se.tbl_packages.FirstOrDefault(t => t.type == package_type).deposit;
                        Comment += "\nInstallation deposit";
                    }
                    //add up all
                    Count = Pack_amount * days;
                    Count += System_deposit;
                    //show period
                    St = instal_d.Value.Date.ToString("dd/MM/yyyy");

                    Paid = se.tbl_payments.Where(g => g.customer_id == tc1.id && g.payment_date >= instal_d && g.payment_date <= end).Sum(t => t.amount_payed);
                    extraPackageInvoicing ep = new classes.extraPackageInvoicing();
                    Count += ep.extr_invoice(instal_d, end, tc1.id);
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
                    Pack_amount = se.tbl_packages.FirstOrDefault(t => t.type == package_type).amount_per_day;

                    //check if he is supposed to pay deposit
                    DateTime dep_date = se.tbl_packages.FirstOrDefault(r => r.type == package_type).date_deposit;
                    if (instal_d >= dep_date)
                    {
                        //invoice installation deposit
                        System_deposit = se.tbl_packages.FirstOrDefault(t => t.type == package_type).deposit;
                        Comment += "\nInstallation deposit";
                    }

                    //add up all
                    Count = Pack_amount * days;
                    //show period
                    St = start.Date.ToString("dd/MM/yyyy");
                    En = end.Date.ToString("dd/MM/yyyy");
                    Paid = se.tbl_payments.Where(g => g.customer_id == tc1.id && g.payment_date >= start && g.payment_date <= end).Sum(t => t.amount_payed);
                    //get extra package amount
                    extraPackageInvoicing ep = new classes.extraPackageInvoicing();
                    Count += ep.extr_invoice(instal_d, end, tc1.id);
                    Comment += "\n" + ep.Comment;
                }
                try
                {
                    Percent = (100 * Paid) / Count;
                }
                catch { }
                if (Percent == null)
                {
                    Percent = 0;
                }
                if (Paid == null)
                {
                    Paid = 0;
                }
                //calculate total invoice and paid
                Total_paid += Paid;
                Total_count += Count;
            }
            try
            {
                TotalPercent = (100 * Total_paid) / Total_count;

            }
            catch
            {

            }
            return TotalPercent;
        }
    }
}