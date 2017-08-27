using sunamiapi.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class extra_package_invoicing
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
            int deposit = 0;
            int cumm_invoice = 0;
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
                        //string item = se.tbl_extra_package_customers.FirstOrDefault(r => r.customer_id == Id && r.date_given <= end).item;
                        //get deposit
                        deposit += se.tbl_extra_item.FirstOrDefault(g => g.item == item).deposit;
                        //get cumm_invoice -- get date given item
                        DateTime date_given = se.tbl_extra_package_customers.FirstOrDefault(t => t.customer_id == Id).date_given;
                        int days = (end - date_given).Days;
                        //get how much he pays per day
                        int per_day = se.tbl_extra_item.FirstOrDefault(e => e.item == item).amount_per_day;
                        //get cumm invoice
                        ext_daily_invoice += per_day;
                        cumm_invoice += days * per_day;
                        if (se.tbl_extra_item.FirstOrDefault(g => g.item == item).amount_per_day >= 0)
                        {
                            comment += "\n" + item;
                        }
                    }
                    else
                    {

                    }


                }
                catch
                {
                    //customer not in extra package scheme
                    continue;
                }
            }
            invoice = deposit + cumm_invoice;

            return invoice;
        }
    }
}