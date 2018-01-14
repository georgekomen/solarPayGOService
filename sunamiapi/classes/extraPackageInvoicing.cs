using sunamiapi.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class extraPackageInvoicing
    {
        db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
        private int invoice;
        private string comment;
        private int ext_daily_invoice;

        public int Invoice
        {
            get
            {
                return invoice;
            }
        }

        public string Comment
        {
            get
            {
                return comment;
            }

            set
            {
                comment = value;
            }
        }

        public int Ext_daily_invoice
        {
            get
            {
                return ext_daily_invoice;
            }

            set
            {
                ext_daily_invoice = value;
            }
        }

        public int extr_invoice(DateTime? start, DateTime end, string Id)
        {
            int days_switched_off = 0;
            int days = 0;
            comment = null;
            int itemDeposit = 0;
            var items = se.tbl_extra_package_customers.Where(r => r.customer_id == Id).Select(i => new { _item = i.item }).ToList();
            foreach (var item1 in items)
            {
                itemDeposit = 0;
                days = 0;
                try
                {
                    string item = item1._item;
                    tbl_extra_package_customers tp = se.tbl_extra_package_customers.FirstOrDefault(g => g.customer_id == Id && g.item == item);
                    if (tp.date_given <= end) //if customer had been given the item
                    {
                        tbl_extra_item tep = se.tbl_extra_item.FirstOrDefault(e => e.item == item);
                        if (start <= tp.date_given)//if invoice start date is equal to date item was issued
                        {
                            itemDeposit = tep.deposit;
                            if(tp.date_taken == null)
                            {
                                days = (end - tp.date_given).Days;
                            }
                            else
                            {
                                if(end >= tp.date_taken)
                                {
                                    days = (tp.date_taken - tp.date_given).Value.Days;
                                }
                                else
                                {
                                    days = (end - tp.date_given).Days;
                                }
                            }
                        }
                        else if(start > tp.date_given)
                        {
                            if (tp.date_taken == null)
                            {
                                days = (end - start).Value.Days;
                            }
                            else
                            {
                                if (end >= tp.date_taken)
                                {
                                    days = (tp.date_taken - start).Value.Days;
                                }
                                else
                                {
                                    days = (end - start).Value.Days;
                                }
                            }
                        }



                        //stop invoicing when switched off
                        days_switched_off = 0;
                        //get days switched off
                        var tsl = se.tbl_switch_logs.Where(r => r.customer_id == Id).ToList();
                        foreach (var i in tsl)
                        {
                            if (i.switch_off_date >= tp.date_given)
                            {
                                if (i.switch_on_date != null)
                                {
                                    days_switched_off += (i.switch_on_date - i.switch_off_date).Value.Days;
                                }
                                else
                                {
                                    days_switched_off += (DateTime.Today - i.switch_off_date).Value.Days;
                                }
                            }
                            else
                            {
                                if (i.switch_on_date != null)
                                {
                                    days_switched_off += (i.switch_on_date - tp.date_given).Value.Days;
                                }
                                else
                                {
                                    days_switched_off += (DateTime.Today - tp.date_given).Days;
                                }
                            }
                        }


                        if (days_switched_off > 0)
                        {
                            comment += "\nDeduction of KES" + (days_switched_off * tep.amount_per_day).ToString() + " for " + days_switched_off.ToString() + " days for " + item + " switched off, ";
                        }
                        ext_daily_invoice += tep.amount_per_day;
                        invoice += ((days - days_switched_off) * tep.amount_per_day) + itemDeposit;
                        if (itemDeposit > 0)
                        {
                            comment += "\nDeposit of KES" + itemDeposit + " for " + item + ",";
                        }

                        if(days > 0 && tep.extra_pay_period > 0)
                        {
                            comment += "\nDaily payment of KES" + tep.amount_per_day + " for " + item + " for " + (days-days_switched_off).ToString() + " day(s), ";
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