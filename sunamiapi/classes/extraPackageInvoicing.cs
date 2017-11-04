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
            int days = 0;
            comment = null;
            //GET LIST OF ALL EXTRa items a customer has
            var items = se.tbl_extra_package_customers.Where(r => r.customer_id == Id).Select(i => new { it = i.item }).ToList();
            //find if customer has extra item then get that item
            foreach (var ite in items)
            {
                try
                {
                    string item = ite.it;
                    tbl_extra_package_customers tp = se.tbl_extra_package_customers.FirstOrDefault(g => g.customer_id == Id && g.item == item);
                    if (tp.date_given <= end)
                    {
                        tbl_extra_item tep = se.tbl_extra_item.FirstOrDefault(e => e.item == item);
                        //string item = se.tbl_extra_package_customers.FirstOrDefault(r => r.customer_id == Id && r.date_given <= end).item;
                        //get deposit





                        //TODO - calcuate invoice in span of a period here
                        //if date given is greater than start date, include deposit otherwise do not
                        //if startdate if before date given .... calculate days from date given to end date .... otherwise use startdate
                        days = (end - tp.date_given).Days;





                        //get cumm_invoice -- get date given item
                        //get how much he pays per day
                        
                        //get cumm invoice
                        ext_daily_invoice += tep.amount_per_day;
                        invoice += (days * tep.amount_per_day) + tep.deposit;
                        if (se.tbl_extra_item.FirstOrDefault(g => g.item == item).amount_per_day >= 0)
                        {
                            comment += "\n" + item + " of KES" + tep.deposit + ", ";
                        }

                        if(days > 0 && tep.extra_pay_period > 0)
                        {
                            comment += "\nDaily payment of KES" + tep.amount_per_day + " for " + item + " for " + days + " day(s), ";
                        }
                    }
                    else
                    { }
                }
                catch
                {
                    //customer not in extra package scheme
                    continue;
                }
            }
            return invoice;
        }
    }
}