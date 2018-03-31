using sunamiapi.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using sunamiapi.classes;
using System.Globalization;
using System.Collections;
using sunamiapi.codeIncludes;
using Flurl.Http;
using System.Web.Http.Results;
using System.IO;
using System.Text;
using System.IO.Compression;
using System.Threading;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Mail;
using Z.EntityFramework.Plus;

namespace sunamiapi.Controllers.api
{
    public class CustomersController : ApiController
    {
        db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
        private bool allowsendsms = true;
        public DateTime beginDate;
        public DateTimeFormatInfo info = new CultureInfo("en-us", false).DateTimeFormat;//MMddyyyy
        public List<string> user_permissions = new List<string>();
        public List <int> user_offices = new List<int>();
        public string logedinUser = "";
        public CustomersController()
        {
            try
            {
                user_permissions = System.Web.HttpContext.Current.Request.QueryString.Get("user_permissions").Split(',').ToList();
            }
            catch
            {}
            try
            {
                string[] user_offices1 = System.Web.HttpContext.Current.Request.QueryString.Get("user_offices").Split(',');
                user_offices = user_offices1.Select(int.Parse).ToList();
            }
            catch
            {}
            try
            {
                logedinUser = System.Web.HttpContext.Current.Request.QueryString.Get("logedinUser").ToString();
            }
            catch (Exception h)
            {
                //throw h;
            }
            try
            {
                string user = se.tbl_users.FirstOrDefault(gg => gg.email == logedinUser).email;
            }
            catch(Exception h)
            {
                //throw h;
            }
            try
            {
                string beginDate1 = "07/01/2016";
                beginDate = Convert.ToDateTime(beginDate1, info);
            }
            catch
            {}
            try
            {
                se.Filter<tbl_customer>(x => x.Where(c => user_offices.Contains((int)c.office_id)));
                se.Filter<tbl_agents>(x => x.Where(c => user_offices.Contains((int)c.office_id)));
                se.Filter<tbl_event_logs>(x => x.Where(c => user_offices.Contains((int)c.office_id)));
                se.Filter<tbl_expenses>(x => x.Where(c => user_offices.Contains((int)c.office_id)));
                se.Filter<tbl_extra_item>(x => x.Where(c => user_offices.Contains((int)c.office_id)));
                se.Filter<tbl_issues>(x => x.Where(c => user_offices.Contains((int)c.office_id)));
                se.Filter<tbl_messages>(x => x.Where(c => user_offices.Contains((int)c.office_id)));
                se.Filter<tbl_mpesa_payments>(x => x.Where(c => user_offices.Contains((int)c.office_id)));
                se.Filter<tbl_payments>(x => x.Where(c => user_offices.Contains((int)c.office_id)));
                se.Filter<tbl_sunami_controller>(x => x.Where(c => user_offices.Contains((int)c.office_id)));
                se.Filter<tbl_switch_logs>(x => x.Where(c => user_offices.Contains((int)c.office_id)));
                se.Filter<tbl_system>(x => x.Where(c => user_offices.Contains((int)c.office_id)));
                se.Filter<tbl_uninstalled_systems>(x => x.Where(c => user_offices.Contains((int)c.office_id)));
            }
            catch { }           
        }

        public List<Icustomer> getCustomerLocations()
        {
            getDataLists gdl = new getDataLists();
            return gdl.getCustomerLocations(se);
        }

        [HttpPost]
        public List<paymentRatesClassPerClient> GetPaymentActiveRates([FromBody] StartEndDate[] value)
        {
            DateTime end1;
            List<paymentRatesClassPerClient> list = new List<paymentRatesClassPerClient>();
            try
            {
                //yyyy-mm-dd e.g. 2017-04-05 - date1
                beginDate = getDate(value[0].startDate);
                end1 = getDate(value[0].endDate);
                //get whole list of customers
                List<tbl_customer> list2 = se.tbl_customer.Where(g => g.install_date <= end1 && g.active_status == true).ToList();

                list = calcInvoiceBtwnDatesm(beginDate, end1, list2).OrderByDescending(g => g.Percent).ToList();
            }
            catch
            {
                end1 = DateTime.Today;
                List<tbl_customer> list2 = se.tbl_customer.Where(g => g.install_date <= end1 && g.active_status == true).ToList();

                list = calcInvoiceBtwnDatesm(beginDate, end1, list2).OrderByDescending(g => g.Percent).ToList();
            }
            return list;
        }

        [HttpPost]
        public List<paymentRatesClassPerClient> GetPaymentInactiveRates([FromBody] StartEndDate[] value)
        {
            DateTime end1;
            List<paymentRatesClassPerClient> list = new List<paymentRatesClassPerClient>();
            try
            {
                //yyyy-mm-dd e.g. 2017-04-05 - date1
                beginDate = getDate(value[0].startDate);
                end1 = getDate(value[0].endDate);
                List<tbl_customer> list2 = se.tbl_customer.Where(g => g.install_date <= end1 && g.active_status == false).ToList();

                list = calcInvoiceBtwnDatesm(beginDate, end1, list2).OrderByDescending(g => g.Percent).ToList();
            }
            catch
            {
                string enddate = DateTime.Today.ToString();
                end1 = Convert.ToDateTime(enddate, info);
                List<tbl_customer> list2 = se.tbl_customer.Where(g => g.install_date <= end1 && g.active_status == false).ToList();

                list = calcInvoiceBtwnDatesm(beginDate, end1, list2).OrderByDescending(g => g.Percent).ToList();
            }
            return list;
            //return null;
        }

        public List<paymentRatesClassPerClient> calcInvoiceBtwnDatesm(DateTime start, DateTime end, List<tbl_customer> list2)
        {
            List<paymentRatesClassPerClient> li = new List<paymentRatesClassPerClient>();
            foreach (var tc1 in list2)
            {
                string St;
                string En;
                string Comment = null;
                int? Count = 0;
                int? Paid = 0;
                bool? status = false;
                if (tc1.install_date >= start)
                {
                    St = tc1.install_date.Value.Date.ToString("dd/MM/yyyy");
                }
                else
                {
                    St = start.Date.ToString("dd/MM/yyyy");
                }
                En = end.Date.ToString("dd/MM/yyyy");

                extraPackageInvoicing ep = new classes.extraPackageInvoicing();
                Count += ep.extr_invoice(start, end, tc1, se);
                Comment += "\n" + ep.Comment;
                Paid = ep.Paid;

                if (Paid == null)
                {
                    Paid = 0;
                }

                int? Percent = 0;
                try
                {
                    Percent = (100 * Paid) / Count;
                }
                catch
                {
                    Percent = 0;
                }
                if (Percent == null)
                {
                    Percent = 0;
                }

                try
                {
                    tbl_system tse = se.tbl_system.FirstOrDefault(g => g.customer_id == tc1.customer_id);
                    if (tse.active_status == true)
                    {
                        status = true;
                    }
                    else if (tse.active_status == false)
                    {
                        status = false;
                    }
                }
                catch (Exception k)
                {
                    status = null;
                }

                li.Add(new paymentRatesClassPerClient
                {
                    Name = tc1.customer_name.ToUpper(),
                    Id = tc1.customer_id,
                    From = St,
                    To = En,
                    Amount = Paid,
                    Invoice = Count,
                    Percent = Percent,
                    Comment = Comment,
                    Phone = tc1.phone_numbers + "\n" + tc1.phone_numbers2 + "\n" + tc1.phone_numbers3,
                    Village = tc1.village_name.ToUpper(),
                    Status = status,
                    Description = tc1.Description
                });
            }
            return li;
        }

        public void sendEmail(string to, string subject, string body)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.Host = "mail.sunamiapp.net";
            client.EnableSsl = false;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("postmaster@sunamiapp.net", "Sunami_2016");

            MailMessage mm = new MailMessage("postmaster@sunamiapp.net", to, subject, body);
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            client.Send(mm);
        }

        [HttpGet]
        public List<Invoiceitem> invoiceItems()
        {
            List<Invoiceitem> list = new List<Invoiceitem>(se.tbl_extra_item.Select(r => new Invoiceitem { Item = r.item, Deposit = r.deposit, Amount = r.amount_per_day, PayDays = r.extra_pay_period }));
            return list; 
        }

        [HttpPost]
        public string invoiceCustomer([FromBody]Invoicecustomerbody[] value)
        {
            string result = "";
            try
            {
                string customer_id = value[0].customerId;
                string item = value[0].item;
                if (!se.tbl_extra_package_customers.Where(r => r.customer_id == customer_id).Select(t => t.item).Contains(item))
                {
                    tbl_extra_package_customers epc = new tbl_extra_package_customers();
                    epc.customer_id = value[0].customerId;
                    epc.item = value[0].item;
                    epc.agentcode = value[0].agentcode;
                    epc.date_given = getDate(value[0].invoiceDate);
                    se.tbl_extra_package_customers.Add(epc);
                    se.SaveChanges();
                    result = "successfully invoiced customer";
                    this.logevent(value[0].loogedUser, value[0].customerId, DateTime.Today, "Invoiced customer a " + value[0].item, "Invoice Customer");
                }
                else
                {
                    result = "this item had already been invoiced to this customer";
                }
            }
            catch
            {
                result = "error invoicing customer";
            }
            return result;
        }


        public List<paychartclass> getPaymentChart()
        {
            List<MonthPayBreakDown> monthPayBreakDown = new List<MonthPayBreakDown>();
            //getting from db
            List<int?> pvals = new List<int?>();
            List<string> chartlabels = new List<string>();

            for (DateTime endDate = beginDate; endDate < DateTime.Today; endDate = endDate.AddMonths(1))
            {
                DateTime formatedTime = Convert.ToDateTime(endDate.Month.ToString() + "/01/" + endDate.Year.ToString(), info);
                DateTime endformatedTime = formatedTime.AddMonths(1).AddDays(-1);

                string monthName = info.GetMonthName(formatedTime.Month).ToString();
                string month = monthName.Substring(0, 3) + "," + formatedTime.Year.ToString();
                chartlabels.Add(month);

                List<tbl_customer> lsc = se.tbl_customer.Where(rr => rr.install_date <= endformatedTime).ToList();

                List<paymentRatesClassPerClient> list = calcInvoiceBtwnDatesm(formatedTime, endformatedTime, lsc);
                int? invoice = 0;
                int? paid = 0;
                int? percent = 0;
                try
                {
                    invoice = list.Sum(r1 => r1.Invoice);
                    paid = list.Sum(r2 => r2.Amount);
                    percent = (paid * 100) / invoice;
                }
                catch
                {
                    percent = 0;
                }

                pvals.Add(percent);
                monthPayBreakDown.Add(new MonthPayBreakDown { Month = month, DateRanges = formatedTime.ToString() + " - " + endformatedTime.ToString(), Invoice = "Ksh" + invoice.ToString(), Paid = "Ksh" + paid.ToString(), Performance = percent.ToString() + "%" });
            }

            List<chartdata> chartdata = new List<chartdata>();
            chartdata.Add(new chartdata() { data = pvals, label = "payment rate" });
            List<paychartclass> datatoreturn = new List<paychartclass>();
            datatoreturn.Add(new paychartclass() { LineChartData = chartdata, LineChartLabels = chartlabels, MonthPayBreakDown = monthPayBreakDown });
            return datatoreturn;
        }

        public List<summaryReport> getPaymentSummaryReport(string id)
        {
            int year1 = 0;
            int month1 = 0;
            DateTime monthStart = new DateTime();
            DateTime monthend1 = new DateTime();
            getDataLists gdl = null;
            try
            {
                //yyyy-mm-dd e.g. 2017-04-05
                year1 = int.Parse(id.Substring(0, 4));
                month1 = int.Parse(id.Substring(5, 2));
                //int month = DateTime.Today.Month;
                //int year = DateTime.Today.Year;
                monthStart = Convert.ToDateTime(month1 + "/01/" + year1, info);
                //DateTime monthend = DateTime.Today;
                monthend1 = monthStart.AddMonths(1).AddDays(-1);
                gdl = new getDataLists();
            }
            catch
            {
                //if id is empty
                month1 = DateTime.Today.Month;
                year1 = DateTime.Today.Year;
                monthStart = Convert.ToDateTime(month1 + "/01/" + year1, info);
                monthend1 = DateTime.Today;
                gdl = new getDataLists();
            }
            return gdl.getPaymentSummaryReport(monthStart, monthend1, beginDate, se);
        }

        [HttpPost]
        public List<msgAndGetsunamicontroller> postAddController([FromBody]postcontrollerbody[] value)
        {
            List<msgAndGetsunamicontroller> controllers = new List<msgAndGetsunamicontroller>();
            try
            {
                string imei = value[0].imei;
                string action = value[0].action;
                //{ imei: this.imei, sim: this.sim, provider: this.provider, version: this.version, loogeduser: this.disname }
                if (!se.tbl_sunami_controller.Select(h => h.imei).Contains(imei) && action == "create")
                {
                    tbl_sunami_controller sc = new tbl_sunami_controller();
                    sc.imei = value[0].imei;
                    sc.sim_no = value[0].sim;
                    sc.date_manufactured = DateTime.Today;
                    sc.provider = value[0].provider;
                    sc.version = value[0].version;
                    sc.recorded_by = value[0].loogeduser;
                    sc.office_id = user_offices[0];
                    se.tbl_sunami_controller.Add(sc);
                    se.SaveChanges();

                    controllers.Add(new msgAndGetsunamicontroller { message = "successfully created new controller", content = getSunamiControllers() });
                }
                else if (se.tbl_sunami_controller.Select(h => h.imei).Contains(imei) && action == "modify")
                {
                    tbl_sunami_controller sc = se.tbl_sunami_controller.FirstOrDefault(g => g.imei == imei);
                    sc.sim_no = value[0].sim;
                    sc.provider = value[0].provider;
                    sc.version = value[0].version;
                    sc.recorded_by = value[0].loogeduser;
                    se.SaveChanges();
                    this.logevent(value[0].loogeduser, "controller imei" + value[0].imei, DateTime.Today, "modified controller details", "system controller");

                    controllers.Add(new msgAndGetsunamicontroller { message = "successfully modified controller", content = getSunamiControllers() });
                }
            }
            catch
            {
            }
            return controllers;
        }

        [HttpPost]
        public string unlinkController([FromBody]IdUser[] value)
        {
            try
            {
                string imei = value[0].id;
                tbl_system sc = se.tbl_system.FirstOrDefault(r => r.imei_number == imei);             
                se.tbl_system.Remove(sc);
                se.SaveChanges();
                logevent(value[0].user, sc.customer_id, DateTime.Now, "unlinked imei: " + value[0].id, "unlink controller");
                return "successfully unlinked controller";
            }
            catch(Exception g)
            {
                return g.Message;
            }
        }

        [HttpPost]
        public string deleteController([FromBody]IdUser[] value)
        {
            try
            {
                string imei = value[0].id;
                if (se.tbl_system.Where(r1 => r1.imei_number == imei).Count() > 0)
                {
                    return "cannot delete controller, it is currently linked to a customer. You need to unlink it before deleting";
                }
                else
                {
                    tbl_sunami_controller sc = se.tbl_sunami_controller.FirstOrDefault(r => r.imei == imei);
                    logevent(value[0].user, sc.sim_no + "," + sc.provider + "," + sc.version, DateTime.Now, "deleted imei: " + value[0].id, "deleted controller");
                    se.tbl_sunami_controller.Remove(sc);
                    se.SaveChanges();
                    return "successfully deleted controller";
                }

            }
            catch
            {
                return "error occured. please contact system admin";
            }

        }

        [HttpPost]
        public string PostSMS([FromBody]postsmsbody[] value)
        {
            sendSms ss = new sendSms();
            var tc33 = se.tbl_customer.Select(ff => new { customer_id = ff.customer_id, phone_numbers = ff.phone_numbers, customer_name = ff.customer_name }).ToList();
            string msgs = null;
            string sim_no = null;
            try
            {
                string msg = value[0].message;
                string message = msg;
                List<recipients> numbers = value[0].recipients;
                foreach (var num in numbers)
                {
                    string cust_idd = num.idnumber;
                    var tc3 = tc33.FirstOrDefault(g3 => g3.customer_id == cust_idd);
                    sim_no = tc33.FirstOrDefault(o => o.customer_id == cust_idd).phone_numbers != null ? tc33.FirstOrDefault(o => o.customer_id == cust_idd).phone_numbers : "+254713014492";
                    string customername = tc3.customer_name;
                    string[] firstname = null;
                    firstname = customername.Split(' ').Length > 0 ? customername.Split(' ') : new string[] { customername };
                    if (message.StartsWith("send"))
                    {
                        //msgs = "Jambo " + firstname[0].ToUpper() + ", " + message.Replace(@"send", @"");
                        msgs = message.Replace(@"send", @"");
                        sendmsg(sim_no, msgs, cust_idd, ss);
                    }
                    else if (message.StartsWith("remind"))
                    {
                        List<tbl_customer> tc = se.tbl_customer.Where(gg => gg.customer_id == num.idnumber).ToList();
                        List<paymentRatesClassPerClient> list = calcInvoiceBtwnDatesm(beginDate, DateTime.Today, tc);
                        msg = message.Replace(@"remind", @"");
                        int? invoice = list[0].Invoice;
                        int? paid = list[0].Amount;
                        int? not_paid = invoice - paid;
                        if (not_paid < 0)
                        {
                            msgs = "Jambo " + firstname[0].ToUpper() + msg + ", asanti kwa uaminifu wako. Malipo ni kwa Mpesa Till number 784289. Nambari ya kuhudumiwa ni 0788103403.";//. Kuudumiwa piga: 0788103403                              
                            sendmsg(sim_no, msgs, cust_idd, ss);
                        }
                        else if (not_paid > 0)
                        {
                            msgs = "Jambo " + firstname[0].ToUpper() + msg + ", una deni la KSH" + not_paid.ToString() + ". Tafadhali tuma malipo yako kwa Mpesa Till number 784289. Nambari ya kuhudumiwa ni 0788103403.";
                            sendmsg(sim_no, msgs, cust_idd, ss);
                        }
                    }
                }
                logevent(value[0].sender, "e.g. " + value[0].recipients[0].idnumber, DateTime.Today, "SMS sent", "SMS");
                return "sent";
            }
            catch (Exception e)
            {
                return e.StackTrace;
            }
        }

        private void sendmsg(string sim_no, string msgs, string cust_id, sendSms ss)
        {
            if (allowsendsms == true)
            {
                ss.sendSmsThroughGateway(sim_no, msgs, cust_id);
            }
        }

        [HttpGet]
        public List<getmessagebody> getMessages()
        {
            string customer_name = "";
            var li1 = se.tbl_messages.OrderByDescending(t => t.id).ToList();
            List<getmessagebody> li = new List<getmessagebody>();
            foreach (var f in li1)
            {
                try
                {
                    customer_name = se.tbl_customer.FirstOrDefault(g => g.customer_id == f.customer_id).customer_name;
                }
                catch
                {
                    customer_name = "UNKNOWN CUSTOMER";
                }
                li.Add(new getmessagebody
                {
                    Name = customer_name,
                    Message = f.message,
                    Date = f.date
                });
            }
            return li;
        }
        

        [HttpGet]
        public List<getmessagebody> getMessagesPerCustomer(string id)
        {
            string customer_name = "";
            var li1 = se.tbl_messages.Where(rr=>rr.customer_id == id).OrderByDescending(t => t.id).ToList();
            List<getmessagebody> li = new List<getmessagebody>();
            foreach (var f in li1)
            {
                try
                {
                    customer_name = se.tbl_customer.FirstOrDefault(g => g.customer_id == f.customer_id).customer_name;
                }
                catch
                {
                    customer_name = "UNKNOWN CUSTOMER";
                }
                li.Add(new getmessagebody
                {
                    Name = customer_name,
                    Message = f.message,
                    Date = f.date
                });
            }
            return li;
        }

        public string getSwitch(string id, string id1)
        {
            string customer_id = id;
            string loogeduser = id1;
            string sim_no = "";
            string res = "";
            tbl_system ts = se.tbl_system.FirstOrDefault(i => i.customer_id == customer_id);
            if (ts.imei_number.Length > 2)
            {
                try
                {
                    sim_no = se.tbl_sunami_controller.FirstOrDefault(g => g.imei == se.tbl_system.
                    FirstOrDefault(f => f.customer_id == customer_id).imei_number).sim_no;
                }
                catch (Exception g)
                {
                    res = g.Message;
                    return res;
                }
                if (ts.active_status == true)
                {
                    res = switchOn(ts, sim_no, customer_id, loogeduser);
                }
                else
                {
                    res = switchOff(ts, sim_no, customer_id, loogeduser);
                }
                res = "successfully toggled";
            }
            else
            {
                res = "customer has not controller";
            }
            se.SaveChanges();
            return res;
        }

        //work in progress
        [HttpPost]
        public void smsDeliveryReport()
        {

        }

        [HttpPost]
        public void recordSwitchResponse([FromBody]SwitchResponse switchResponse)
        {
            string res = " ";
            tbl_system ts = new tbl_system();
            tbl_sunami_controller sc = new tbl_sunami_controller();
            tbl_customer tc = new tbl_customer();
            try
            {
                sc = se.tbl_sunami_controller.AsNoFilter().FirstOrDefault(ff => ff.sim_no == switchResponse.Address);
                ts = se.tbl_system.AsNoFilter().FirstOrDefault(rr => rr.imei_number == sc.imei);
                ts.last_connected_to_db_date = DateTime.Now;
                tc = se.tbl_customer.AsNoFilter().FirstOrDefault(tt => tt.customer_id == ts.customer_id);
                foreach(tbl_sunami_controller tsc in se.tbl_sunami_controller.Where(ff => ff.sim_no == switchResponse.Address)){
                    if(switchResponse.Imei.Length > 10)
                    {
                        tsc.imei = switchResponse.Imei;
                    }
                }
                user_offices.Add((int)tc.office_id);
            }
            catch
            {
                ts = se.tbl_system.AsNoFilter().FirstOrDefault(uu => uu.imei_number == switchResponse.Imei);
                ts.last_connected_to_db_date = DateTime.Today;
                tc = se.tbl_customer.FirstOrDefault(oo => oo.customer_id == ts.customer_id);
                user_offices.Add((int)tc.office_id);
            }
            logevent("system feedback: "+ tc.customer_name, tc.customer_id, DateTime.Now,"Mobile: " + switchResponse.Address+"IMEI: " + switchResponse.Imei+ "STATUS: "+ switchResponse.Status, "switch feedback");
            se.SaveChanges();
        }

        private dynamic fetchMessageFromAfricasTalking(int messageId)
        {
            fetchSMS ss = new fetchSMS();
            return ss.fetchSms(messageId);
            /*foreach (dynamic result in ss.fetchSms(messageId))
            {
                Console.Write((string)result["number"] + ",");
                Console.Write((string)result["status"] + ","); // status is either "Success" or "error message"
                Console.Write((string)result["messageId"] + ",");
                Console.WriteLine((string)result["cost"]);
            }*/
        }

        public string switchOff(tbl_system ts, string sim_no, string customer_id, string loogeduser)
        {
            if (ts.active_status == false)
            {
                tbl_customer tc = new tbl_customer();
                string res;
                try
                {
                    //switch off
                    ts.active_status = true;

                    if (allowsendsms == true)
                    {
                        //notify the customer that he has been put on
                        tc = se.tbl_customer.FirstOrDefault(g => g.customer_id == customer_id);
                        var customernames = tc.customer_name.Split(' ');
                        sendSms ss = new sendSms();
                        ss.sendSmsThroughGateway(sim_no, "smsc$1%$+254713014492%smsd$" + customernames[0] + "%s*solar yako imewashwa#e", customer_id);
                    }

                    tbl_switch_logs sl = new tbl_switch_logs();
                    sl.customer_id = customer_id;
                    sl.switched_off_by = loogeduser;
                    sl.switch_off_date = DateTime.Today;
                    try
                    {
                        List<tbl_customer> lst = new List<tbl_customer>();
                        lst.Add(tc);
                        List<paymentRatesClassPerClient> res1 = calcInvoiceBtwnDatesm(beginDate, DateTime.Today, lst);
                        sl.switch_off_payrate = res1[0].Percent.Value.ToString();
                    }
                    catch
                    {
                        sl.switch_off_payrate = "0";
                    }
                    se.tbl_switch_logs.Add(sl);
                    res = "";
                }
                catch (Exception g)
                {
                    res = g.Message;
                }
                return res;
            }
            else
            {
                return null;
            }

        }

        public string switchOn(tbl_system ts, string sim_no, string customer_id, string loogeduser)
        {
            if (ts.active_status == true)
            {
                tbl_customer tc = new tbl_customer();
                string res;
                try
                {
                    //switch on
                    ts.active_status = false;

                    if (allowsendsms == true)
                    {
                        //notify the customer that he has been switched off
                        tc = se.tbl_customer.FirstOrDefault(g => g.customer_id == customer_id);
                        var customernames = tc.customer_name.Split(' ');
                        sendSms ss = new sendSms();
                        ss.sendSmsThroughGateway(sim_no, "smsc$0%$+254713014492%smsd$" + customernames[0] + "%s*solar yako imezimwa#e", customer_id);
                    }
                    int sl1 = se.tbl_switch_logs.Where(h => h.customer_id == customer_id).Max(j => j.Id);
                    tbl_switch_logs sl = se.tbl_switch_logs.FirstOrDefault(h => h.customer_id == customer_id && h.Id == sl1);
                    sl.customer_id = customer_id;
                    sl.switched_on_by = loogeduser;
                    sl.switch_on_date = DateTime.Today;
                    try
                    {
                        List<tbl_customer> lst = new List<tbl_customer>();
                        lst.Add(tc);
                        List<paymentRatesClassPerClient> res1 = calcInvoiceBtwnDatesm(beginDate, DateTime.Today, lst);
                        sl.switch_on_payrate = res1[0].Percent.Value.ToString();
                    }
                    catch
                    {
                        sl.switch_on_payrate = "0";
                    }

                    res = "";
                }
                catch (Exception g)
                {
                    res = g.Message;

                }
                return res;
            }
            else
            {
                return null;
            }
        }

        public List<mpesaPayments> getmpesaPayments()
        {
            List<mpesaPayments> mp = new List<mpesaPayments>();
            var list1 = se.tbl_payments.Select(g => new { code = g.transaction_code }).FromCache().ToList();
            var list2 = se.tbl_mpesa_payments.FromCache().ToList();
            //select from list2 where not in list1
            var list3 = list2.Where(p => list1.Any(p2 => p2.code == p.transaction_code)).Select(r => new
            {
                Date = r.date,
                Reference = r.transaction_code,
                Amount = r.amount,
                Phone = r.number,
                Message = r.message
            }).OrderByDescending(f => f.Date).ToList();
            foreach (var tmp in list3)
            {
                try
                {
                    if (tmp.Phone != "" && tmp.Phone != null)
                    {
                        //process unprocessed payments
                        paymentRecording pr = new paymentRecording();
                        pr.recordpayment(tmp.Message.ToString(), se, false);
                    }
                    mp.Add(new mpesaPayments
                    {
                        Date = tmp.Date,
                        Reference = tmp.Reference,
                        Amount = int.Parse(tmp.Amount),
                        Number = tmp.Phone,
                        Message = tmp.Message
                    });
                }
                catch
                {
                }
            }
            return mp;
        }

        public List<mpesaPayments> getcashRecords()
        {
            //date //ref //amount //number //message

            var list1 = se.tbl_payments.Where(h => h.payment_method == "cash").FromCache().ToList().OrderByDescending(gh => gh.Id);
            List<mpesaPayments> mp = new List<mpesaPayments>(from i in list1
                                               select new mpesaPayments
                                               {
                                                   Id = i.Id,
                                                   Customer_Id = i.customer_id,
                                                   Customer_Name = se.tbl_customer.FirstOrDefault(h4 => h4.customer_id == i.customer_id).customer_name,
                                                   Date = i.payment_date,
                                                   Reference = i.transaction_code,
                                                   Amount = i.amount_payed,
                                                   RecordedBy = i.person_recording
                                               });

            return mp;
        }

        public List<mpesaPayments> getbankRecords()
        {
            //date //ref //amount //number //message

            var list1 = se.tbl_payments.Where(h => h.payment_method.Contains("bank")).FromCache().ToList().OrderByDescending(gh => gh.Id);
            List<mpesaPayments> mp = new List<mpesaPayments>(from i in list1
                                               select new mpesaPayments
                                               {
                                                   Id = i.Id,
                                                   Customer_Id = i.customer_id,
                                                   Customer_Name = se.tbl_customer.FirstOrDefault(h4 => h4.customer_id == i.customer_id).customer_name,
                                                   Date = i.payment_date,
                                                   Reference = i.transaction_code,
                                                   Amount = i.amount_payed,
                                                   RecordedBy = i.person_recording,
                                                   payMode = i.payment_method
                                               });

            return mp;
        }

        [HttpPost]
        public string deletePayment([FromBody]IdUser[] value)
        {
            try
            {
                int id_ = 0;
                id_ = int.Parse(value[0].id);
                tbl_payments sc = se.tbl_payments.FirstOrDefault(r => r.Id == id_);
                logevent(value[0].user, sc.customer_id, DateTime.Today, "deleted payment amount: " + sc.amount_payed + " ref: " + sc.transaction_code + " method: " + sc.payment_method, "delete payment");
                se.tbl_payments.Remove(sc);
                se.SaveChanges();

            }
            catch
            {
            }
            return "successfully deleted payment";
        }

        public List<mpesaPayments> getMpesaRecords()
        {
            //date //ref //amount //number //message

            var list1 = se.tbl_payments.Where(h => h.payment_method == "mpesa").FromCache().ToList().OrderByDescending(gh => gh.Id);
            List<mpesaPayments> mp = new List<mpesaPayments>(from i in list1
                                               select new mpesaPayments
                                               {
                                                   Id = i.Id,
                                                   Customer_Id = i.customer_id,
                                                   Customer_Name = se.tbl_customer.FirstOrDefault(h4 => h4.customer_id == i.customer_id).customer_name,
                                                   Date = i.payment_date,
                                                   Reference = i.transaction_code,
                                                   Amount = i.amount_payed,
                                                   RecordedBy = i.person_recording
                                               });

            return mp;
        }

        public List<mpesaPayments> getUnprocessedMpesaPayments()
        {
            List<mpesaPayments> mp = new List<mpesaPayments>();
            var list1 = se.tbl_payments.Select(g => new { code = g.transaction_code }).ToList();
            var list2 = se.tbl_mpesa_payments.ToList();
            //select from list2 where not in list1
            var list3 = list2.Where(p => !list1.Any(p2 => p2.code == p.transaction_code)).Select(r => new
            {
                Date = r.date,
                Reference = r.transaction_code,
                Amount = r.amount,
                Phone = r.number,
                Message = r.message
            }).OrderByDescending(f => f.Date).ToList();
            foreach (var tmp in list3)
            {
                try
                {
                    if (tmp.Phone != "" && tmp.Phone != null)
                    {
                        //process unprocessed payments
                        paymentRecording pr = new paymentRecording();
                        pr.recordpayment(tmp.Message.ToString(), se, false);
                    }
                    mp.Add(new mpesaPayments
                    {
                        Date = tmp.Date,
                        Reference = tmp.Reference,
                        Amount = int.Parse(tmp.Amount),
                        Number = tmp.Phone,
                        Message = tmp.Message
                    });
                }
                catch
                {

                }
            }
            return mp;
        }

        public List<payRecordClass2> getPaymentPerCustomer(string id)
        {
            int daily_invoice = 0;
            int? total_invoice = 0;
            int? paid = 0;
            int? not_paid = 0;
            string name = "";
            int? percent = 0;
            List<payrecordClass> pd = new List<payrecordClass>();
            try
            {
                //get statistics
                List<tbl_customer> lst = new List<tbl_customer>();
                lst = se.tbl_customer.Where(g => g.customer_id == id).ToList();
                List<paymentRatesClassPerClient> res1 = calcInvoiceBtwnDatesm(beginDate, DateTime.Today, lst);
                daily_invoice = 0; //@TODO
                total_invoice = res1[0].Invoice;
                paid = res1[0].Amount;
                not_paid = res1[0].Invoice - res1[0].Amount;
                percent = (paid * 100) / total_invoice;
                name = se.tbl_customer.FirstOrDefault(g => g.customer_id == id).customer_name;
                try
                {
                    var list = (from tp in se.tbl_payments
                                join tc in se.tbl_customer on tp.customer_id equals tc.customer_id
                                orderby tp.Id descending
                                where tp.payment_date >= beginDate && tp.payment_date <= DateTime.Today && tp.customer_id == id
                                select new { tc.customer_name, tp.customer_id, tp.amount_payed, tp.payment_date, tp.payment_method, pp = tp.phone_number, TransactionCode = tp.transaction_code, pr = tp.person_recording }
                              ).ToList();
                    foreach (var it in list)
                    {
                        pd.Add(new payrecordClass() { Amount = it.amount_payed, PayDate = it.payment_date.Value.Date.ToString("dd/MM/yyy"), PayMethod = it.payment_method, MpesaNumber = it.pp, Code = it.TransactionCode, Receiver = it.pr });
                    }
                }
                catch
                { }
            }
            catch { }
            List<payRecordClass2> prc2 = new List<payRecordClass2>();
            prc2.Add(new payRecordClass2() { Payrecord = pd, Name = name, Daily_invoice = daily_invoice, Not_paid = not_paid, Paid = paid, Total_invoice = total_invoice, Percent = percent });
            return prc2;
        }

        private async void mledger()
        {
            //get JSON messages
            try
            {
                //request from mledger
                string url = "https://mledger.safaricom.com/svc/2.0/my/transactions/restore?";
                var responseString = await url
                         // .PostUrlEncodedAsync(new { PhoneNumber = "254795271832", CutOffDate = "2015-09-18", Key = "8E463479-D819-42CD-8588-5783684CA34E", P = "aaab5c0c73f14cf887ba32d5d501abcb", U = "62b52617de6e48ae98e322eb2c502f1d", Ver = "5.0.95", DeviceID = "", OSVersion = "4.2.2", OSPlatForm = "Alps+TECNO+S9S-reallytek82_tb_jb5" })
                         .PostUrlEncodedAsync(new { PhoneNumber = "254795271832", CutOffDate = "2017-01-01", Key = "8E463479-D819-42CD-8588-5783684CA34E", P = "aaab5c0c73f14cf887ba32d5d501abcb", U = "62b52617de6e48ae98e322eb2c502f1d" })
                         .ReceiveStream();
                string res;
                using (var decompress = new GZipStream(responseString, CompressionMode.Decompress))
                using (var sr = new StreamReader(decompress))
                {
                    res = sr.ReadToEnd();
                }
                //convert response to json array
                JArray j = JArray.Parse(res);
                int msgcount = j.Count();
                //loop through all messages in the json response
                for (int i = 0; i <= msgcount; i++)
                {
                    //convert to json object--retrieve and send to db
                    JToken token = JObject.Parse(j[i].ToString());
                    string msg1 = token.SelectToken("OriginalText").ToString();
                    record_mpesa_msg_mledger(msg1);
                }
            }
            catch
            {

            }
        }

        public static string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new System.IO.Compression.GZipStream(msi, System.IO.Compression.CompressionMode.Decompress))
                {
                    //gs.CopyTo(mso);
                    CopyTo(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        ////zipping to gzip -- compressing to gzip --- for learning purposes
        //public static byte[] Zip(string str)
        //{
        //    var bytes = Encoding.UTF8.GetBytes(str);

        //    using (var msi = new MemoryStream(bytes))
        //    using (var mso = new MemoryStream())
        //    {
        //        using (var gs = new GZipStream(mso, CompressionMode.Compress))
        //        {
        //            //msi.CopyTo(gs);
        //            CopyTo(msi, gs);
        //        }

        //        return mso.ToArray();
        //    }
        //}

        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];
            int cnt;
            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        private void record_mpesa_msg_mledger(string msg1)
        {
            //find in class
            paymentRecording pr = new classes.paymentRecording();
            pr.recordpayment(msg1, se, false);
            string res = pr.Json;
        }

        [HttpPost]
        public string postReceive_mpesa([FromBody]mobilempesapaymentbody[] value)
        {
            string res = "";
            // only record if message is from mpesa
            if (value[0].address == "MPESA")
            {
                try
                {
                    string imei = value[0].imei;
                    int tblm = se.tbl_mpesa_phone_imei.Where(i => i.imei == imei).Count();
                    if (tblm < 1)
                    {
                        res = null;
                    }
                    else
                    {
                        res = record_mpesa_msg_phone(value[0].msg, value[0].imei);
                    }
                }
                catch (Exception k)
                { }
            }
            return res;
        }

        private string record_mpesa_msg_phone(string msg1, string imei_no)
        {
            paymentRecording pr = new classes.paymentRecording();
            pr.Phone_imei = imei_no;
            pr.recordpayment(msg1, se, true);
            string res = pr.Json;
            return res;
        }

        public List<postnewcustomer> getCustomerDetails()
        {
            List<postnewcustomer> list = new List<postnewcustomer>(from i in se.tbl_customer
                                                     //where i.active_status == true
                                                 orderby i.Id descending
                                                 select new postnewcustomer
                                                 {
                                                     name = i.customer_name,
                                                     id = i.customer_id,
                                                     occupation = i.occupation,
                                                     number1 = i.phone_numbers,
                                                     number2 = i.phone_numbers2,
                                                     number3 = i.phone_numbers3,
                                                     box = i.Po_Box_Address,
                                                     package = i.package_type,
                                                     latG = i.lat,
                                                     lonG = i.lon,
                                                     description = i.Description,
                                                     village = i.village_name,
                                                     location = i.location,
                                                     city = i.city,
                                                     installdate = i.install_date,
                                                     witness = i.next_of_kin,
                                                     witnessid = i.nok_mobile,
                                                     status = i.active_status,
                                                     country = i.country,
                                                     agentcode = i.agentcode,
                                                     witnessnumber = i.witness_mobile_number,
                                                     gender = i.gender
                                                 }
                      );
            return list;
        }

        public postnewcustomer getSingleCustomerDetails(string id)
        {
            try
            {
                tbl_customer i = se.tbl_customer.FirstOrDefault(t1 => t1.customer_id == id);
                postnewcustomer customer = (new postnewcustomer
                {
                    name = i.customer_name,
                    id = i.customer_id,
                    occupation = i.occupation,
                    number1 = i.phone_numbers,
                    number2 = i.phone_numbers2,
                    number3 = i.phone_numbers3,
                    box = i.Po_Box_Address,
                    package = i.package_type,
                    latG = i.lat,
                    lonG = i.lon,
                    description = i.Description,
                    village = i.village_name,
                    location = i.location,
                    city = i.city,
                    installdate = i.install_date,
                    witness = i.next_of_kin,
                    witnessid = i.nok_mobile,
                    status = i.active_status,
                    country = i.country,
                    agentcode = i.agentcode,
                    witnessnumber = i.witness_mobile_number,
                    gender = i.gender
                });
                return customer;
            }
            catch
            {
                return null;
            }
        }

        public List<getsunamisystemresponse> getSystemDetails()
        {
            List<getsunamisystemresponse> list = new List<getsunamisystemresponse>(from ts in se.tbl_system
                                                 join tc in se.tbl_customer on ts.customer_id equals tc.customer_id
                                                 join tsc in se.tbl_sunami_controller on ts.imei_number equals tsc.imei
                                                 orderby tc.Id descending
                                                 select new getsunamisystemresponse
                                                 {
                                                     Owner = tc.customer_name,
                                                     installdate = "i" + tc.install_date,
                                                     Active_Status = ts.active_status,
                                                     Last_System_Communication = "l" + ts.last_connected_to_db_date,
                                                     Imei = ts.imei_number,
                                                     SystemPhoneNumber = tsc.sim_no
                                                 });
            return list;
        }


        public List<getsunamisystemresponse> getSystemDetailsPerCustomer(String id)
        {
            List<getsunamisystemresponse> list = new List<getsunamisystemresponse>(from ts in se.tbl_system
                                                                                   where ts.customer_id == id
                                                                                   join tc in se.tbl_customer on ts.customer_id equals tc.customer_id
                                                                                   join tsc in se.tbl_sunami_controller on ts.imei_number equals tsc.imei
                                                                                   orderby tc.Id descending
                                                                                   select new getsunamisystemresponse
                                                                                   {
                                                                                       Owner = tc.customer_name,
                                                                                       installdate = "i" + tc.install_date,
                                                                                       Active_Status = ts.active_status,
                                                                                       Last_System_Communication = "l" + ts.last_connected_to_db_date,
                                                                                       Imei = ts.imei_number,
                                                                                       SystemPhoneNumber = tsc.sim_no
                                                                                   });
            return list;
        }

        public List<getswitchlogresponse> getswitchlogs()
        {
            List<getswitchlogresponse> list = new List<getswitchlogresponse>(from ts in se.tbl_switch_logs
                                                 join tc in se.tbl_customer on ts.customer_id equals tc.customer_id
                                                 join tsci in se.tbl_system on ts.customer_id equals tsci.customer_id
                                                 join tscn in se.tbl_sunami_controller on tsci.imei_number equals tscn.imei
                                                 orderby ts.Id descending
                                                 select new getswitchlogresponse
                                                 {
                                                     Name = tc.customer_name,
                                                     Id = tc.customer_id,
                                                     Imei = tsci.imei_number,
                                                     Sim = tscn.sim_no,
                                                     Switch_Off_Date = ts.switch_off_date,
                                                     Switch_Off_payrate = ts.switch_off_payrate,
                                                     Switch_Off_by = ts.switched_off_by,
                                                     Switch_On_Date = ts.switch_on_date,
                                                     Switch_on_payrate = ts.switch_on_payrate,
                                                     Switch_on_by = ts.switched_on_by
                                                 }
                    );
            return list;
        }


        public List<getswitchlogresponse> getswitchlogsPerCustomer(string id)
        {
            List<getswitchlogresponse> list = new List<getswitchlogresponse>(from ts in se.tbl_switch_logs
                                                                             where ts.customer_id == id
                                                                             join tc in se.tbl_customer on ts.customer_id equals tc.customer_id
                                                                             join tsci in se.tbl_system on ts.customer_id equals tsci.customer_id
                                                                             join tscn in se.tbl_sunami_controller on tsci.imei_number equals tscn.imei
                                                                             orderby ts.Id descending
                                                                             select new getswitchlogresponse
                                                                             {
                                                                                 Name = tc.customer_name,
                                                                                 Id = tc.customer_id,
                                                                                 Imei = tsci.imei_number,
                                                                                 Sim = tscn.sim_no,
                                                                                 Switch_Off_Date = ts.switch_off_date,
                                                                                 Switch_Off_payrate = ts.switch_off_payrate,
                                                                                 Switch_Off_by = ts.switched_off_by,
                                                                                 Switch_On_Date = ts.switch_on_date,
                                                                                 Switch_on_payrate = ts.switch_on_payrate,
                                                                                 Switch_on_by = ts.switched_on_by
                                                                             }
                    );
            return list;
        }


        public List<getsunamicontroller> getSunamiControllers()
        {
            List<getsunamicontroller> list = new List<getsunamicontroller>(from ts in se.tbl_sunami_controller
                                                 orderby ts.Id descending
                                                 select new getsunamicontroller
                                                 {
                                                     Imei = ts.imei,
                                                     Sim_Number = ts.sim_no,
                                                     Provider = ts.provider,
                                                     Version = ts.version,
                                                     Reg_Date = ts.date_manufactured,
                                                     Recorded_By = ts.recorded_by
                                                 }
                    );
            return list;
        }

        public List<object> getFreeImei()
        {
            var list1 = se.tbl_system.Select(g => new { imei1 = g.imei_number }).ToList();
            var list2 = se.tbl_sunami_controller.Select(f => new { imei2 = f.imei }).ToList();
            List<object> list = new List<object>(list2.Where(t => !list1.Any(t2 => t2.imei1 == t.imei2)).Select(r => new { FreeImei = r.imei2 }));
            return list;
        }

        public List<NameIdResponse> getCustomersWithNoController()
        {
            var list1 = se.tbl_system.Select(g => new { ids1 = g.customer_id }).ToList();
            var list2 = se.tbl_customer.Select(f => new { ids2 = f.customer_id, Name = f.customer_name }).ToList();
            List<NameIdResponse> list = new List<NameIdResponse>(list2.Where(t => !list1.Any(t2 => t2.ids1 == t.ids2)).Select(r => new NameIdResponse { Name = r.Name, Id = r.ids2 }));
            return list;
        }

        [HttpPost]
        public string postLinkController([FromBody]postlinkcontroller[] value)
        {
            string res = "";
            try
            {
                string customer_id = value[0].customer_id;
                int ts4 = se.tbl_system.Where(i => i.customer_id == customer_id).Count();
                if (ts4 == 0)
                {
                    tbl_system ts = new tbl_system();
                    ts.customer_id = value[0].customer_id;
                    ts.active_status = false;
                    ts.imei_number = value[0].imei;
                    ts.current_lat = null;
                    ts.current_lon = null;
                    ts.office_id = user_offices[0];
                    ts.last_connected_to_db_date = DateTime.Today;
                    se.tbl_system.Add(ts);
                    se.SaveChanges();
                    res = "linking sucessful";
                }
                else
                {
                    res = "customer already linked to another controller";
                }
            }
            catch (Exception t)
            {
                res = "customer id might not be existing";
            }
            return res;
        }

        public List<getissueresponse> getIssues()
        {
            List<getissueresponse> list = new List<getissueresponse>(from tc in se.tbl_customer
                                                 join ti in se.tbl_issues on tc.customer_id equals ti.customer_id
                                                 orderby ti.Id descending
                                                 select new getissueresponse
                                                 {
                                                     customer = tc.customer_name,
                                                     Id = ti.Id,
                                                     reporter = ti.reporter,
                                                     issue = ti.issue,
                                                     date = ti.date,
                                                     priority = ti.priority,
                                                     status = ti.status,
                                                     solvedOn = ti.solvedOn,
                                                     solvedBy = ti.solvedBy,
                                                     comment = ti.comments
                                                 });
            return list;
        }

        [HttpGet]
        public List<getissueresponse> getIssuesPerCustomer(string id)
        {
            List<getissueresponse> list = 
                new List<getissueresponse>(from tc in se.tbl_customer
                                            where tc.customer_id == id
                                            join ti in se.tbl_issues on tc.customer_id equals ti.customer_id
                                            orderby ti.Id descending
                                            select new getissueresponse
                                            {
                                                customer = tc.customer_name,
                                                Id = ti.Id,
                                                reporter = ti.reporter,
                                                issue = ti.issue,
                                                date = ti.date,
                                                priority = ti.priority,
                                                status = ti.status,
                                                solvedOn = ti.solvedOn,
                                                solvedBy = ti.solvedBy,
                                                comment = ti.comments
                                            });
            return list;
        }

        [HttpPost]
        public string postNewIssues([FromBody]postissuebody[] value)
        {
            string res = "";
            try
            {
                tbl_issues ti = new tbl_issues();
                ti.customer_id = value[0].id;
                ti.priority = value[0].priority;
                ti.issue = value[0].issue;
                ti.date = DateTime.Now;
                ti.status = "pending";
                ti.office_id = user_offices[0];
                ti.reporter = value[0].reporter;
                se.tbl_issues.Add(ti);
                se.SaveChanges();
                res = "succesfull reported an issue";
            }
            catch
            {
                res = "error recording the issue in the database";
            }
            return res;
        }

        [HttpPost]
        public string postSolveIssues([FromBody]postissuesolvedbody[] value)
        {
            string res = "";
            try
            {
                int id = value[0].Id;
                tbl_issues ti = se.tbl_issues.FirstOrDefault(f => f.Id == id);
                ti.status = "solved";
                ti.solvedOn = DateTime.Now;
                ti.solvedBy = value[0].Ssolver;
                ti.comments = value[0].Scomment;
                se.SaveChanges();
                res = "succesfully solved an issue";
            }
            catch
            {
                res = "error marking issue as solved in database";
            }
            return res;
        }

        public List<getuninstalledsystems> getUninstalledSystems()
        {
            List<getuninstalledsystems> list = new List<getuninstalledsystems>(from tu in se.tbl_uninstalled_systems
                                                 join tc in se.tbl_customer
                                                 on tu.customer_id equals tc.customer_id
                                                 orderby tu.Id descending
                                                 select new getuninstalledsystems
                                                 {
                                                     Name = tc.customer_name,
                                                     Id = tc.customer_id,
                                                     install_date = tc.install_date,
                                                     Uninstalled_on = tu.uninstall_date,
                                                     Reason = tu.Reason,
                                                     unistalledBy = tu.recorded_by,
                                                     previousRecords = tu.previousRecords
                                                 });
            return list;
        }

        [HttpPost]
        public string postUninstall([FromBody]postuninstallbody[] value)
        {
            string customer_id = value[0].customer_id;
            string res = "";
            try
            {
                //yyyy-mm-dd e.g. 2017-04-05 - date1
                DateTime date2 = getDate(value[0].date1);

                //mark customer as inactive in customer db
                tbl_customer tc = se.tbl_customer.FirstOrDefault(w => w.customer_id == customer_id);
                tc.active_status = false;
                se.SaveChanges();

                string possesions = "";
                try
                {
                    //inactivate in tblsystem too
                    tbl_system tc2 = se.tbl_system.FirstOrDefault(w4 => w4.customer_id == customer_id);
                    possesions += tc2.imei_number;
                    se.tbl_system.Remove(tc2);
                    se.SaveChanges();
                }
                catch
                { }

                tbl_uninstalled_systems us = new tbl_uninstalled_systems();
                us.customer_id = value[0].customer_id;
                us.uninstall_date = date2;
                us.recorded_by = value[0].recorded_by;
                us.Reason = value[0].reason;
                us.office_id = user_offices[0];
                us.previousRecords = possesions;
                se.tbl_uninstalled_systems.Add(us);

                // set all invoiced items taken dates
                foreach (tbl_extra_package_customers tep in se.tbl_extra_package_customers.Where(r => r.customer_id == customer_id))
                {
                    if (tep.date_taken == null)
                    {
                        tep.date_taken = DateTime.Today;
                    }
                    else
                    {
                        //was already take from the customer
                    }
                }

                se.SaveChanges();
                res = "successfully uninstalled";
            }
            catch
            {
                res = "error while uninstalling. please report to admin";
            }
            return res;
        }

        private DateTime getDate(string date1)
        {
            int year1 = 0;
            int month1 = 0;
            int day1 = 0;
            DateTime date2 = new DateTime();
            //yyyy-mm-dd e.g. 2017-04-05
            year1 = int.Parse(date1.Substring(0, 4));
            month1 = int.Parse(date1.Substring(5, 2));
            day1 = int.Parse(date1.Substring(8, 2));
            date2 = Convert.ToDateTime(month1 + "/" + day1 + "/" + year1, info);
            return date2;
        }

        public List<postnewcustomer> getActiveCustomersDetails()
        {
            // List<object> list = new List<object>(se.tbl_customer.Where(r => r.active_status == true).Select(g => new
            List<postnewcustomer> list = new List<postnewcustomer>(se.tbl_customer.Select(g => new postnewcustomer
            {
                name = g.customer_name,
                id = g.customer_id
            }).OrderBy(G => G.name));
            return list;
        }

        [HttpPost]
        public string postExpense([FromBody]postexpensebody[] value)
        {
            string res = "error";
            //yyyy-mm-dd e.g. 2017-04-05 - date1
            DateTime date2 = getDate(value[0].dateset);
            byte[] pic = Convert.FromBase64String(value[0].pic1);

            //get recipients email
            try
            {
                string recipient = value[0].recipient;
                string email = se.tbl_users.FirstOrDefault(f2 => f2.name == recipient).email;
                value[0].recipient = email;
            }
            catch
            {}

            try
            {
                tbl_expenses exp = new tbl_expenses();
                exp.account = value[0].account;
                exp.amount = value[0].amount;
                exp.vendor = value[0].vendor;
                exp.recipient = value[0].recipient;
                exp.dateset = date2;
                exp.ref_code = value[0].ref_code;
                exp.image = pic;
                exp.office_id = user_offices[0];
                exp.description = value[0].category;
                exp.RecordedBy = value[0].recordedBy;
                se.tbl_expenses.Add(exp);
                se.SaveChanges();
                res = "successfully recorded new expense";
            }
            catch
            {
                res = "error recording new expense. please contact admin";
            }
            return res;
        }

        byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public List<getexpensebody> getAllExpenses()
        {
            var expenses = se.tbl_expenses.ToList().OrderByDescending(ff=>ff.Id);

            List<getexpensebody> li = new List<getexpensebody>();
            foreach (var f in expenses)
            {
                string usernm = se.tbl_users.FirstOrDefault(f1 => f1.email == f.recipient).name;
                if (f.image.Length < 2)
                {
                    li.Add(new getexpensebody
                    {
                        Id = f.Id,
                        Account = f.account,
                        Amount = f.amount,
                        Date = f.dateset,
                        Description = f.description,//category
                        Image = false,
                        Recipient = usernm,
                        vendor = f.vendor,
                        RefCode = f.ref_code
                    });
                }
                else
                {
                    li.Add(new getexpensebody
                    {
                        Id = f.Id,
                        Account = f.account,
                        Amount = f.amount,
                        Date = f.dateset,
                        Description = f.description,//category
                        Image = true,
                        Recipient = usernm,
                        RefCode = f.ref_code
                    });
                }
            }
            return li;
        }

        public byte[] getReceipt(string id)
        {
            int idd = int.Parse(id);
            byte[] pic = se.tbl_expenses.FirstOrDefault(f => f.Id == idd).image;
            return pic;
        }

        private byte[] thumbnail(Bitmap thumb)
        {
            byte[] result = null;
            if (thumb != null)
            {
                MemoryStream stream = new MemoryStream();
                thumb.Save(stream, thumb.RawFormat);
                result = stream.ToArray();
            }
            return result;
        }

        public byte[] Base64StringToBitmap(byte[] byteBuffer)
        {
            Bitmap bmpReturn = new Bitmap(100, 100);
            MemoryStream memoryStream = new MemoryStream(byteBuffer);
            memoryStream.Position = 0;
            bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);
            memoryStream.Close();
            memoryStream = null;
            byteBuffer = null;
            return thumbnail(bmpReturn);
        }

        public List<getexpensecategory> getExpenseCategories()
        {
            List<getexpensecategory> list = new List<getexpensecategory>(se.tbl_expense_categories.Select(g => new getexpensecategory
            {
                Name = g.Name,
                Details = g.Details
            }).OrderBy(G => G.Name));
            return list;
        }

        public List<UsersResponseBody> getUserNames()
        {
            List<UsersResponseBody> list1 = new List<UsersResponseBody>();
            var users = se.tbl_users.ToList().OrderBy(f => f.name);
            foreach (var g in users)
            {
                list1.Add(new UsersResponseBody
                {
                    Name = g.name,
                    email = g.email
                });
            }
            return list1;
        }

        [HttpPost]
        public string postNewCustomer([FromBody]postnewcustomer[] value)
        {
            DateTime date2 = DateTime.Today;
            string res = "error";
            try
            {
                registerCustomer rc = new registerCustomer();
                rc.Id = value[0].id;
                rc.Agentcode = value[0].agentcode;
                rc.Witness_mobile = value[0].witnessnumber;
                rc.Box = value[0].box;
                rc.City = value[0].city;
                rc.LatG = value[0].latG;
                rc.LonG = value[0].lonG;
                rc.Name = value[0].name;
                rc.Number = value[0].number1;
                rc.Number2 = value[0].number2;
                rc.Number3 = value[0].number3;
                rc.Occupation = value[0].occupation;
                rc.Village = value[0].village;
                rc.Witness = value[0].witness;
                rc.Witnessid = value[0].witnessid;
                rc.Description = value[0].description;
                rc.Gender = value[0].gender;
                rc.RecordedBy = value[0].recordedBy;
                rc.Country = value[0].country;
                rc.Package = value[0].package;
                date2 = getDate(value[0].date1);//yyyy-mm-dd e.g. 2017-04-05 - date1
                rc.Install_date = date2;
                rc.Location = value[0].location;
                rc.Office_id = user_offices[0];

                rc.record(se);
                res = rc.Confirm;
                // invoice new customer
                //TODO - call invoice function - avoid duplication of code
                string item = value[0].package;
                string customer_id = value[0].id;
               
                // end of invoicing
            }
            catch (Exception kk)
            {
                throw new Exception(kk.StackTrace);
            }
            if (!res.Contains("registered new customer"))
            {
                logevent(value[0].recordedBy, value[0].id, DateTime.Now, res, "customer registration");
            }
            return res;
        }

        [HttpGet]
        public List<object> getUser(string id)
        {
            try
            {
                List<int> offices = new List<int>();
                int office_id = (int)se.tbl_users.FirstOrDefault(f => f.email == id).tbl_office_id;
                offices.Add(office_id);
                string currency = se.tbl_office.FirstOrDefault(ff => ff.id == office_id).tbl_sub_county.tbl_county.tbl_country.currency;
                List<object> list = new List<object>(se.tbl_users.Where(f => f.email == id).Select(g => new
                {
                    currency = currency,
                    allowed = g.allow,
                    level = g.level,
                    office_id = offices
                }));
                return list;
            }
            catch
            {
                List<object> list1 = new List<object>();
                list1.Add(new { allowed = false, level = "0" });
                return list1;
            }
        }

        [HttpPost]
        public string postmakePayment([FromBody]postmakepaymentbody[] value)
        {
            string res = "";
            paymentRecording pr = new paymentRecording();
            try
            {
                string bankname = null;
                string loggedUser = value[0].loggedUser;
                string PayMode = value[0].PayMode;
                string Code = value[0].Code;
                string Id = value[0].Customer_Id;
                try
                {
                    bankname = value[0].bankname;
                }
                catch { }

                if (PayMode == "mpesa")
                {
                    tbl_mpesa_payments tmp = se.tbl_mpesa_payments.FirstOrDefault(g => g.transaction_code == Code);
                    pr.Mdate = DateTime.Parse(tmp.date.ToString());
                    pr.Mpesa_amount = tmp.amount;
                }


                else if (PayMode == "cash")
                {
                    pr.Mdate = getDate(value[0].date1);
                    pr.Mpesa_amount = value[0].amount;
                }

                else if (PayMode == "bank")
                {
                    pr.Mdate = getDate(value[0].date1);
                    pr.Mpesa_amount = value[0].amount;
                    if (bankname.Length > 2)
                    {
                        PayMode = PayMode + "_" + bankname;
                    }
                }

                else if (PayMode == "mtn_uganda")
                {
                    pr.Mdate = getDate(value[0].date1);
                    pr.Mpesa_amount = value[0].amount;
                }

                else if (PayMode == "airtel_uganda")
                {
                    pr.Mdate = getDate(value[0].date1);
                    pr.Mpesa_amount = value[0].amount;
                }

                pr.Loggedin = loggedUser;
                pr.PayMode = PayMode;
                pr.Code = Code;
                pr.Customer_id = Id;
                pr.process_transaction(se, true);
                res += pr.Json;
                return res;

            }
            catch (Exception g)
            {
                res = g.Message;
                return res;
            }
        }

        private void logevent(string loggeduser, string customerid, DateTime date, string event1, string category)
        {
            try
            {
                tbl_event_logs ev = new tbl_event_logs();
                ev.category_affected = category;
                ev.customer_id = customerid;
                ev.date = date;
                ev.@event = event1;
                ev.loggedin_user = loggeduser;
                ev.office_id = user_offices[0];
                se.tbl_event_logs.Add(ev);
                se.SaveChanges();
            }
            catch (Exception f)
            {
                throw f;
            }
        }

        [HttpGet]
        public List<geteventlogsresponse> eventlogs()
        {
            List<geteventlogsresponse> list = new List<geteventlogsresponse>();
            list = new List<geteventlogsresponse>(from t in se.tbl_event_logs
                                    orderby t.Id descending
                                    select new geteventlogsresponse
                                    {
                                        Category = t.category_affected,
                                        CustomerId = t.customer_id,
                                        Date = t.date,
                                        Event = t.@event,
                                        LoggedInUser = t.loggedin_user
                                    });
            return list;
        }

        [HttpGet]
        public List<geteventlogsresponse> eventlogsPerCustomer(string id)
        {
            List<geteventlogsresponse> list = new List<geteventlogsresponse>();
            list = new List<geteventlogsresponse>(from t in se.tbl_event_logs
                                                  where t.customer_id == id
                                                  orderby t.Id descending
                                                  select new geteventlogsresponse
                                                  {
                                                      Category = t.category_affected,
                                                      CustomerId = t.customer_id,
                                                      Date = t.date,
                                                      Event = t.@event,
                                                      LoggedInUser = t.loggedin_user
                                                  });
            return list;
        }

        [HttpGet]
        public List<tbl_inventory> getInventory(string id)
        {
            try
            {
                List<tbl_inventory> inventory = new List<tbl_inventory>();
                DateTime date1 = getDate(id).AddDays(1);
               
                var items = se.tbl_inventory.Select(f => f.Item).Distinct().ToList();
                foreach (var item in items)
                {
                    DateTime? itemlistdate = se.tbl_inventory.Where(g => g.date <= date1 && g.Item == item).Max(f => f.date);
                    tbl_inventory itemlist = se.tbl_inventory.FirstOrDefault(g => g.date == itemlistdate && g.Item == item);
                    inventory.Add(itemlist);
                }
                return inventory;
            }
            catch (Exception f)
            {
                return null;
            }
        }

        [HttpGet]
        public List<tbl_inventory> getStockDetails(string id)
        {
            try
            {
                
                List<tbl_inventory> inventory = new List<tbl_inventory>(se.tbl_inventory.Where(u => u.Item == id));
                return inventory;
            }
            catch (Exception f)
            {
                return null;
            }
        }

        [HttpPost]
        public string postRecordItem([FromBody]postrecordstockitem[] value)
        {
            try
            {
                
                tbl_inventory ti = new tbl_inventory();
                ti.date = DateTime.Now;
                ti.Item = value[0].itemName;
                ti.units = value[0].units;
                ti.stock = int.Parse(value[0].Stock);
                ti.office_id = user_offices[0];
                se.tbl_inventory.Add(ti);
                se.SaveChanges();
                return "New item successfully recorded";
            }
            catch (Exception f)
            {
                return f.Message;
            }
        }

        public string postUpdateStock([FromBody] postupdatestock[] value)
        {
            try
            {
                int? stock = 0;
                
                string item = value[0].item;
                int maxinv = se.tbl_inventory.Where(y => y.Item == item).Max(f => f.Id);
                tbl_inventory ti = se.tbl_inventory.SingleOrDefault(fr => fr.Id == maxinv && fr.Item == item);

                if (value[0].method == "dispense")
                {
                    stock = ti.stock - value[0].number;
                    if (stock < 0)
                    {
                        return "OUT OF STOCK, you cannot dispense " + value[0].number;
                    }
                }
                else if (value[0].method == "add")
                {
                    stock = ti.stock + value[0].number;
                }
                tbl_inventory tia = new tbl_inventory();
                tia.comments = value[0].comment;
                tia.date = DateTime.Now;
                tia.Item = value[0].item;
                tia.stock = stock;
                tia.units = ti.units;
                tia.office_id = user_offices[0];
                se.tbl_inventory.Add(tia);
                se.SaveChanges();
                return "successfully updated stocks";
            }
            catch (Exception f)
            {
                return f.Message;
            }
        }
        
        [HttpGet]
        public List<object> getCustomerInvoicedItems(string id)
        {
            List<object> list = new List<object>();
            list = new List<object>(from ei in se.tbl_extra_package_customers
                                    where ei.customer_id == id
                                    orderby ei.Id descending
                                    select new
                                    {
                                        item = ei.item,
                                        invoiceDate = ei.date_given
                                    });
            return list;
        }
        
        [HttpPost]
        public tbl_agents registerAgent([FromBody]agentpayload value)
        {
            try
            {
                string idnumber = value.idnumber;
                
                tbl_agents agent = new tbl_agents();
                agent.dateofenrolment = value.dateofenrolment;
                agent.firstname = value.firstname;
                agent.lastname = value.lastname;
                agent.idnumber = value.idnumber;
                agent.country = value.country;
                agent.email = value.email;
                agent.location = value.location;
                agent.phonenumber = value.phonenumber;
                agent.office_id = user_offices[0];
                se.tbl_agents.Add(agent);
                se.SaveChanges();
                tbl_agents agent1  = se.tbl_agents.FirstOrDefault(gg => gg.idnumber == idnumber);
                return agent1;

            }
            catch (Exception e)
            {
                String error = e.Message;
                return null;
            }
        }

        [HttpGet]
        public List<tbl_agents> getAgents()
        {
            List<tbl_agents> list = se.tbl_agents.ToList();
            return list;
        }

        [HttpGet]
        public List<tbl_extra_package_customers> getAgentSales(string id)
        {
            List<tbl_extra_package_customers> list = se.tbl_extra_package_customers.Where(rr=>rr.agentcode == id).ToList();
            return list;
        }
    }
}

