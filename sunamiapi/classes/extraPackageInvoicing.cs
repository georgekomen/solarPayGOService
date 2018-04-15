using sunamiapi.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Z.EntityFramework.Plus;

namespace sunamiapi.classes
{
    public class extraPackageInvoicing
    {
        private int invoice;
        private string comment;
        private int ext_daily_invoice;
        private Boolean calculatedPaid = false;
        private int paid;
        public int Paid { get => paid; set => paid = value; }
        public int Invoice { get => invoice; set => invoice = value; }
        public string Comment { get => comment; set => comment = value; }
        public int Ext_daily_invoice { get => ext_daily_invoice; set => ext_daily_invoice = value; }

        public int extr_invoice(DateTime start, DateTime end, tbl_customer tc1, db_a0a592_sunamiEntities se)
        {
            int days_switched_off = 0;
            comment = null;
            paid = 0;
            foreach (tbl_extra_package_customers tp in se.tbl_extra_package_customers.AsNoFilter().Where(r => r.customer_id == tc1.customer_id))
            {
                int itemDeposit = 0;
                int days = 0;
                try
                {
                    if ((tp.date_taken >= start || tp.date_taken == null) && tp.date_given <= end) //if customer had been given the item
                    {
                        tbl_extra_item tep = se.tbl_extra_item.AsNoFilter().FirstOrDefault(e => e.item == tp.item);
                        if (start <= tp.date_given)//if invoice start date is equal to date item was issued
                        {
                            itemDeposit = (int)tep.deposit;
                            if(tp.date_taken == null)
                            {
                                days = (end - (DateTime)tp.date_given).Days;
                            }
                            else
                            {
                                if(end >= tp.date_taken)
                                {
                                    days = ((DateTime)tp.date_taken - (DateTime)tp.date_given).Days;
                                }
                                else
                                {
                                    days = (end - (DateTime)tp.date_given).Days;
                                }
                            }
                        }
                        else
                        {
                            if (tp.date_taken == null)
                            {
                                days = (end - start).Days;
                            }
                            else
                            {
                                if (end >= tp.date_taken)
                                {
                                    days = ((DateTime)tp.date_taken - start).Days;
                                }
                                else
                                {
                                    days = (end - start).Days;
                                }
                            }
                        }

                        //stop invoicing when switched off
                        days_switched_off = 0;
                        //get days switched off
                        var tsl = se.tbl_switch_logs.AsNoFilter().Where(r => r.customer_id == tc1.customer_id).ToList();
                        foreach (var i in tsl)
                        {
                            if (i.switch_off_date >= tp.date_given)
                            {
                                if (i.switch_on_date != null)
                                {
                                    days_switched_off += ((DateTime)i.switch_on_date - (DateTime)i.switch_off_date).Days;
                                }
                                else
                                {
                                    days_switched_off += (DateTime.Today - (DateTime)i.switch_off_date).Days;
                                }
                            }
                            else
                            {
                                if (i.switch_on_date != null)
                                {
                                    days_switched_off += ((DateTime)i.switch_on_date - (DateTime)tp.date_given).Days;
                                }
                                else
                                {
                                    days_switched_off += (DateTime.Today - (DateTime)tp.date_given).Days;
                                }
                            }
                        }

                        if (days_switched_off > 0)
                        {
                            comment += "\nDeduction of KES" + (days_switched_off * tep.amount_per_day).ToString() + " for " + days_switched_off.ToString() + " days for " + tp.item + " switched off, ";
                        }
                        ext_daily_invoice += (int)tep.amount_per_day;
                        invoice += ((days - days_switched_off) * (int)tep.amount_per_day) + itemDeposit;
                        if (itemDeposit > 0)
                        {
                            comment += "\nDeposit of KES" + itemDeposit + " for " + tp.item + ",";
                        }

                        if(days > 0 && tep.extra_pay_period > 0)
                        {
                            comment += "\nDaily payment of KES" + tep.amount_per_day + " for " + tp.item + " for " + (days-days_switched_off).ToString() + " day(s), ";
                        }
                        if (!calculatedPaid)
                        {
                            try
                            {
                                paid = se.tbl_payments.AsNoFilter().Where(g => g.customer_id == tc1.customer_id && (DateTime)g.payment_date >= start && (DateTime)g.payment_date <= end).Sum(t => (int)t.amount_payed);
                            }
                            catch {
                                paid = 0;
                            }
                                calculatedPaid = true;
                        }
                    }
                    else
                    { }
                }
                catch(Exception e)
                {
                    comment += "\n Error " + e.ToString() + e.StackTrace.ToString();
                    //customer not in extra package scheme
                    continue;
                }
            }
            return invoice;
        }
    }
}