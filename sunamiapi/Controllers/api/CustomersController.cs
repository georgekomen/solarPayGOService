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

namespace sunamiapi.Controllers.api
{
    public class CustomersController : ApiController
    {
        private bool allowsendsms = true;
        public DateTime beginDate;
        public DateTimeFormatInfo info = new CultureInfo("en-us", false).DateTimeFormat;//MMddyyyy

        public CustomersController()
        {
            try
            {
                string beginDate1 = "06/01/2016";
                beginDate = Convert.ToDateTime(beginDate1, info);

            }
            catch
            {

            }
        }

        public List<Icustomer> getCustomerLocations()
        {
            getDataLists gdl = new getDataLists();
            return gdl.getCustomerLocations();
        }

        [HttpPost]
        public List<paymentRatesClassPerClient> GetPaymentActiveRates([FromBody] JArray value)
        {
            DateTime end1;
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            List<paymentRatesClassPerClient> list = new List<paymentRatesClassPerClient>();
            try
            {
                JToken token = JObject.Parse(value[0].ToString());
                string startDate = token.SelectToken("startDate").ToString();
                string endDate = token.SelectToken("endDate").ToString();

                //yyyy-mm-dd e.g. 2017-04-05 - date1
                beginDate = getDate(startDate);
                end1 = getDate(endDate);
                //get whole list of customers
                List<tbl_customer> list2 = se.tbl_customer.Where(g => g.install_date <= end1 && g.active_status == true).ToList();

                list = calcInvoiceBtwnDatesm(beginDate, end1, list2).OrderByDescending(g => g.Percent).ToList();
            }
            catch
            {
                string enddate = DateTime.Today.ToString();
                end1 = Convert.ToDateTime(enddate, info);
                List<tbl_customer> list2 = se.tbl_customer.Where(g => g.install_date <= end1 && g.active_status == true).ToList();

                list = calcInvoiceBtwnDatesm(beginDate, end1, list2).OrderByDescending(g => g.Percent).ToList();
            }
            se.Dispose();
            return list;

        }

        [HttpPost]
        public List<paymentRatesClassPerClient> GetPaymentInactiveRates([FromBody] JArray value)
        {
           /* DateTime end1;
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();

            List<paymentRatesClassPerClient> list = new List<paymentRatesClassPerClient>();
            try
            {
                JToken token = JObject.Parse(value[0].ToString());
                string startDate = token.SelectToken("startDate").ToString();
                string endDate = token.SelectToken("endDate").ToString();

                //yyyy-mm-dd e.g. 2017-04-05 - date1
                beginDate = getDate(startDate);
                end1 = getDate(endDate);
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
            se.Dispose();
            return list;*/
            return null;
        }

        public List<paymentRatesClassPerClient> calcInvoiceBtwnDatesm(DateTime start, DateTime end, List<tbl_customer> list2)
        {
            tbl_customer tableCustomer = new tbl_customer();
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            List<paymentRatesClassPerClient> li = new List<paymentRatesClassPerClient>();
            string Comment;
            int? Paid;
            int Count;
            string St;
            string En;

            foreach (var tc1 in list2)
            {
                tableCustomer = se.tbl_customer.FirstOrDefault(g => g.customer_id == tc1.customer_id);
                Comment = null;
                Count = 0;
                Paid = 0;
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
                try
                {
                    Paid = se.tbl_payments.Where(g => g.customer_id == tc1.customer_id && g.payment_date >= start && g.payment_date <= end).Sum(t => t.amount_payed);
                }
                catch
                {
                    Paid = 0;
                }


                extraPackageInvoicing ep = new classes.extraPackageInvoicing();
                Count += ep.extr_invoice(start, end, tc1.customer_id);
                Comment += "\n" + ep.Comment;

                if (Paid == null)
                {
                    Paid = 0;
                }

                int? Percent = 0;
                try
                {
                    Percent = (100 * Paid) / Count;
                }
                catch {

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
                    Description = tableCustomer.Description
                });
            }
            se.Dispose();
            return li;
        }

        [HttpGet]
        public List<object> invoiceItems()
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            return new List<object>(se.tbl_extra_item.Select(r => new { Item = r.item, Deposit = r.deposit, Amount = r.amount_per_day, PayDays = r.extra_pay_period }));
        }

        [HttpPost]
        public string invoiceCustomer([FromBody]JArray value)
        {
            string result = "";
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            try
            {
                JToken token = JObject.Parse(value[0].ToString());
                string customerId = token.SelectToken("customerId").ToString();
                string item = token.SelectToken("item").ToString();
                string loogedUser = token.SelectToken("loogedUser").ToString();
                string invoiceDate = token.SelectToken("invoiceDate").ToString();

                if (!se.tbl_extra_package_customers.Where(r => r.customer_id == customerId).Select(t => t.item).Contains(item))
                {
                    tbl_extra_package_customers epc = new tbl_extra_package_customers();
                    epc.customer_id = customerId;
                    epc.item = item;
                    epc.date_given = getDate(invoiceDate);
                    se.tbl_extra_package_customers.Add(epc);
                    se.SaveChanges();
                    result = "successfully invoiced customer";
                    this.logevent(loogedUser, customerId, DateTime.Today, "Invoiced customer a " + item, "Invoice Customer");
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
            finally
            {
                se.Dispose();               
            }
            return result;
        }

        public int? GetCalcPayRate(DateTime start, DateTime end, db_a0a592_sunamiEntities se)
        {
            int? percent = 0;
            int? paid = 0;
            int? invoice = 0;
            List<tbl_customer> lst = new List<tbl_customer>();
            lst = se.tbl_customer.ToList();
            List<paymentRatesClassPerClient> res1 = calcInvoiceBtwnDatesm(start, end, lst);
            
            try
            {
                invoice = res1.Sum(t => t.Invoice);
                paid = res1.Sum(r => r.Amount);
                percent = (paid * 100) / invoice;
            }
            catch {
                percent = 0;
            }
            return percent;
        }

        public List<paychartclass> getPaymentChart()
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();

            //getting from db
            List<int?> pvals = new List<int?>();
            List<string> chartlabels = new List<string>();

            for(DateTime endDate = beginDate; endDate < DateTime.Today; endDate = endDate.AddMonths(1))
            {
                string monthName = info.GetMonthName(endDate.Month).ToString();
                string month = monthName.Substring(0, 3) + "," + endDate.Year.ToString().Substring(1, 3);
                chartlabels.Add(month);
                int? percent = GetCalcPayRate(beginDate, endDate, se);
                pvals.Add(percent);
            }
            
            List<chartdata> chartdata = new List<chartdata>();
            chartdata.Add(new chartdata() { data = pvals, label = "payment rate" });
            List<paychartclass> datatoreturn = new List<paychartclass>();
            datatoreturn.Add(new paychartclass() { LineChartData = chartdata, LineChartLabels = chartlabels });
            se.Dispose();

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
                monthend1 = monthStart.AddMonths(1);
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
            return gdl.getPaymentSummaryReport(monthStart, monthend1, beginDate);
        }
     
        [HttpPost]
        public List<object> postAddController([FromBody]JArray value)
        {
            List<object> controllers = new List<object>();
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            try
            {
                //{ imei: this.imei, sim: this.sim, provider: this.provider, version: this.version, loogeduser: this.disname }
                JToken token = JObject.Parse(value[0].ToString());
                string imei = token.SelectToken("imei").ToString();
                string sim = token.SelectToken("sim").ToString();
                string provider = token.SelectToken("provider").ToString();
                string version = token.SelectToken("version").ToString();
                string loogeduser = token.SelectToken("loogeduser").ToString();
                string action = token.SelectToken("action").ToString();

                if (!se.tbl_sunami_controller.Select(h => h.imei).Contains(imei) && action == "create")
                {
                    tbl_sunami_controller sc = new tbl_sunami_controller();
                    sc.imei = imei;
                    sc.sim_no = sim;
                    sc.date_manufactured = DateTime.Today;
                    sc.provider = provider;
                    sc.version = version;
                    sc.recorded_by = loogeduser;
                    se.tbl_sunami_controller.Add(sc);
                    se.SaveChanges();
                    
                    controllers.Add(new { message = "successfully created new controller" });
                    controllers.Add(new { content = getSunamiControllers() });
                } else if(se.tbl_sunami_controller.Select(h => h.imei).Contains(imei) && action == "modify")
                {
                    tbl_sunami_controller sc = se.tbl_sunami_controller.FirstOrDefault(g => g.imei == imei);
                    sc.sim_no = sim;
                    sc.provider = provider;
                    sc.version = version;
                    sc.recorded_by = loogeduser;
                    se.SaveChanges();
                    this.logevent(loogeduser, "controller imei" + imei, DateTime.Today, "modified controller details", "system controller");
                    
                    controllers.Add(new { message = "successfully modified controller" });
                    controllers.Add(new { content = getSunamiControllers() });
                }
            }
            catch
            {
            }
            se.Dispose();
            
            return controllers;
        }

        [HttpPost]
        public string unlinkController([FromBody]JArray id)
        {
            string imei = "";
            string user = "";
            JToken token = JObject.Parse(id[0].ToString());
            try
            {
                imei = token.SelectToken("id").ToString();
                user = token.SelectToken("user").ToString();
            }
            catch { }

            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            try
            {
                tbl_system sc = se.tbl_system.FirstOrDefault(r => r.imei_number == imei);

                logevent(user, sc.customer_id, DateTime.Today, "unlinked imei: " + id, "unlink controller");
                se.tbl_system.Remove(sc);
                se.SaveChanges();
                
            }
            catch
            {
            }
            se.Dispose();
            return "successfully unlinked controller";
        }

        [HttpPost]
        public string deleteController([FromBody]JArray id)
        {
            string imei = "";
            string user = "";
            JToken token = JObject.Parse(id[0].ToString());
            try
            {
                imei = token.SelectToken("id").ToString();
                user = token.SelectToken("user").ToString();
            }
            catch { }

            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            try
            {
                if (se.tbl_system.Where(r1 => r1.imei_number == imei).Count() > 0)
                {
                    se.Dispose();
                    return "cannot delete controller, it is currently linked to a customer. You need to unlink it before deleting";
                }
                else
                {
                    tbl_sunami_controller sc = se.tbl_sunami_controller.FirstOrDefault(r => r.imei == imei);

                    logevent(user, sc.sim_no+","+sc.provider+","+sc.version, DateTime.Today, "deleted imei: " + id, "deleted controller");
                    se.tbl_sunami_controller.Remove(sc);
                    se.SaveChanges();
                    se.Dispose();
                    return "successfully deleted controller";
                }

            }
            catch
            {
                se.Dispose();
                return "error occured. please contact system admin";
            }
            
        }

        [HttpPost]
        public string PostSMS([FromBody]JArray value)
        {
            db_a0a592_sunamiEntities se1;
            try
            {
                se1 = new db_a0a592_sunamiEntities();
            }
            catch (Exception g)
            {
                return g.Message;

            }
            var tc33 = se1.tbl_customer.Select(ff => new { customer_id = ff.customer_id, phone_numbers = ff.phone_numbers, customer_name = ff.customer_name }).ToList();

            string msgs = null;
            string sim_no = null;
            try
            {
                JToken token = JObject.Parse(value[0].ToString());
                string msg = token.SelectToken("message").ToString();

                JArray numbers = JArray.Parse(token.SelectToken("recipients").ToString());
                int i = numbers.Count();
                for (int g = 0; g < i; g++)
                {
                    JToken number = JObject.Parse(numbers[g].ToString());
                    string cust_idd = number.SelectToken("idnumber").ToString();
                    //check tbl customer
                    var tc3 = tc33.FirstOrDefault(g3 => g3.customer_id == cust_idd);
                    try
                    {
                        //if he has never payed with mpesa then use reg number
                        sim_no = tc33.FirstOrDefault(o => o.customer_id == cust_idd).phone_numbers;
                    }
                    catch
                    {

                    }

                    try
                    {
                        string customername = tc3.customer_name;

                        var firstname = customername.Split(' ');

                        if (msg.StartsWith("send"))
                        {
                            msgs = "Jambo " + firstname[0] + "\n" + msg.Replace(@"send", @"");
                            sendmsg(sim_no, msgs + ", Sunami solar", se1, cust_idd);
                            //return "successfully sent";
                        }
                        else if(msg.StartsWith("remind"))
                        {
                            msg = msg.Replace(@"remind", @"");
                            int invoice = int.Parse(number.SelectToken("Invoice").ToString());
                            int paid = int.Parse(number.SelectToken("Paid").ToString());
                            int not_paid = invoice - paid;

                            if (not_paid < 0 && string.IsNullOrEmpty(msgs))
                            {
                                msgs = "Jambo " + firstname[0] + "\n" + msg + " twakushukuru kwa uaminifu wako. Endelea kutuma malipo yako kwa Mpesa till number: 784289 (Buy goods & services), Sunami solar";//. Kuudumiwa piga: 0788103403
                                sendmsg(sim_no, msgs, se1, cust_idd);
                                // return "successfully sent";
                            }
                            else if (not_paid > 0 && string.IsNullOrEmpty(msgs))
                            {
                                msgs = "Jambo " + firstname[0] + "\n" + msg + " una deni ya KSH" + not_paid.ToString("C").Trim('$') + ". Tuma malipo yako kwa Mpesa till number: 784289 (Buy goods & services), Sunami Solar";
                                sendmsg(sim_no, msgs, se1, cust_idd);
                                // return "successfully sent";
                            }
                        }
                    }
                    catch
                    {

                    }
                }
                se1.Dispose();
                return "messages sent successfully";
            }
            catch
            {
                se1.Dispose();
                return "error sending messages";
            }
        }

        private void sendmsg(string sim_no, string msgs, db_a0a592_sunamiEntities se1, string idk)
        {
            if (allowsendsms == true)
            {
                sendSms ss = new sendSms();
                ss.sendSmsThroughGateway(sim_no, msgs, idk);
                Thread.Sleep(600);
            }
        }

        public List<object> getMessages()
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            string customer_name = "";
            var li1 = se.tbl_messages.OrderByDescending(t=>t.date).ToList();
            List<object> li = new List<object>();
            foreach (var f in li1)
            {
                try
                {
                    customer_name = se.tbl_customer.FirstOrDefault(g => g.customer_id == f.customer_id).customer_name;
                }
                catch { }
                li.Add(new
                {
                    Name = customer_name,
                    Message = f.message,
                    Date = f.date
                });
            }
            se.Dispose();
            return li;
        }

        public string getSwitch(string id, string id1)
        {
            db_a0a592_sunamiEntities se;
            try
            {
                se = new db_a0a592_sunamiEntities();
            }
            catch (Exception g)
            {
                return g.Message;

            }
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
                    res = switchOn(ts, sim_no, se, customer_id, loogeduser);
                }
                else
                {
                    res = switchOff(ts, sim_no, se, customer_id, loogeduser);
                }
                res = "successfully toggled";
            }
            else
            {
                res = "customer has not controller";
            }
            se.SaveChanges();
            se.Dispose();
            return res;
        }

        public string switchOff(tbl_system ts, string sim_no, db_a0a592_sunamiEntities se, string customer_id, string loogeduser)
        {
            tbl_customer tc = new tbl_customer();
            string res;
            try
            {
                //switch off
                ts.active_status = true;

                if (allowsendsms == true)
                {
                    sendSms ss = new sendSms();
                    ss.sendSmsThroughGateway(sim_no, "smsc$1%$1%", customer_id);

                    //notify the customer that he has been put on
                    tc = se.tbl_customer.FirstOrDefault(g => g.customer_id == customer_id);
                    var customernames = tc.customer_name.Split(' ');
                    ss.sendSmsThroughGateway(tc.phone_numbers, customernames[0] + ", Sunami solar inakujulisha kuwa solar yako imewashwa", customer_id);
                }

                tbl_switch_logs sl = new tbl_switch_logs();
                sl.customer_id = customer_id;
                sl.switched_off_by = loogeduser;
                sl.switch_off_date = DateTime.Today;
                try
                {
                    List<tbl_customer> lst = new List<tbl_customer>();
                    lst.Add(tc);
                    List< paymentRatesClassPerClient> res1 = calcInvoiceBtwnDatesm(beginDate, DateTime.Today, lst);
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

        public string switchOn(tbl_system ts, string sim_no, db_a0a592_sunamiEntities se, string customer_id, string loogeduser)
        {
            tbl_customer tc = new tbl_customer();
            string res;
            try
            {
                //switch on
                ts.active_status = false;

                if (allowsendsms == true)
                {
                    sendSms ss = new sendSms();
                    ss.sendSmsThroughGateway(sim_no, "smsc$0%$0%", customer_id);

                    //notify the customer that he has been switched off
                    tc = se.tbl_customer.FirstOrDefault(g => g.customer_id == customer_id);
                    var customernames = tc.customer_name.Split(' ');
                    ss.sendSmsThroughGateway(tc.phone_numbers, customernames[0] + ", Sunami solar inakujulisha kuwa solar yako inazimwa leo kutokana na deni. Tafadhali lipa ili iwashwe tena", customer_id);
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
                    sl.switch_off_payrate = res1[0].Percent.Value.ToString();
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

        /*public List<object> getmpesaPayments()
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            //date //ref //amount //number //message
            List<object> mp = new List<object>();
            var list3 = (from i in se.tbl_mpesa_payments
                         orderby i.date descending
                         select new
                         {
                             Date = i.date,
                             Reference = i.transaction_code,
                             Amount = i.amount,
                             Number = i.number,
                             Message = i.message
                         }).ToList();
            foreach (var i in list3)
            {
                mp.Add(new
                {
                    Date = i.Date,
                    Reference = i.Reference,
                    Amount = i.Amount,
                    Number = i.Number,
                    Message = i.Message
                });
            }
            se.Dispose();
            return mp;
        }*/

        public List<object> getmpesaPayments()
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            List<object> mp = new List<object>();
            var list1 = se.tbl_payments.Select(g => new { code = g.transaction_code }).ToList();
            var list2 = se.tbl_mpesa_payments.ToList();
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
                    mp.Add(new
                    {
                        Date = tmp.Date,
                        Reference = tmp.Reference,
                        Amount = tmp.Amount,
                        Number = tmp.Phone,
                        Message = tmp.Message
                    });
                }
                catch
                {
                }
            }
            se.Dispose();
            return mp;
        }

        public List<object> getcashRecords()
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            //date //ref //amount //number //message

            var list1 = se.tbl_payments.Where(h => h.payment_method == "cash").ToList().OrderByDescending(gh => gh.payment_date);
            List<object> mp = new List<object>(from i in list1
                                               select new
                                               {
                                                   Id = i.Id,
                                                   Customer_Id = i.customer_id,
                                                   Customer_Name = se.tbl_customer.FirstOrDefault(h4 => h4.customer_id == i.customer_id).customer_name,
                                                   Date = i.payment_date,
                                                   Reference = i.transaction_code,
                                                   Amount = i.amount_payed,
                                                   RecordedBy = i.person_recording
                                               });

            se.Dispose();
            return mp;
        }

        public List<object> getbankRecords()
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            //date //ref //amount //number //message

            var list1 = se.tbl_payments.Where(h => h.payment_method.Contains("bank")).ToList().OrderByDescending(gh => gh.payment_date);
            List<object> mp = new List<object>(from i in list1
                                               select new
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

            se.Dispose();
            return mp;
        }

        [HttpPost]
        public string deletePayment([FromBody]JArray id)
        {
            int id1 = 0;
            string user = "";
            JToken token = JObject.Parse(id[0].ToString());
            try
            {
                id1 = int.Parse(token.SelectToken("id").ToString());
                user = token.SelectToken("user").ToString();
            }
            catch { }
            
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            try
            {
                tbl_payments sc = se.tbl_payments.FirstOrDefault(r => r.Id == id1);
                logevent(user, sc.customer_id, DateTime.Today, "deleted payment amount: " + sc.amount_payed + " ref: " + sc.transaction_code+" method: "+sc.payment_method, "delete payment");
                se.tbl_payments.Remove(sc);
                se.SaveChanges();
                
            }
            catch
            {
            }
            se.Dispose();
            return "successfully deleted payment";
        }

        public List<Object> getMpesaRecords()
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            //date //ref //amount //number //message

            var list1 = se.tbl_payments.Where(h => h.payment_method == "mpesa").ToList().OrderByDescending(gh => gh.payment_date);
            List<object> mp = new List<object>(from i in list1
                                               select new
                                               {
                                                   Id = i.Id,
                                                   Customer_Id = i.customer_id,
                                                   Customer_Name = se.tbl_customer.FirstOrDefault(h4 => h4.customer_id == i.customer_id).customer_name,
                                                   Date = i.payment_date,
                                                   Reference = i.transaction_code,
                                                   Amount = i.amount_payed,
                                                   RecordedBy = i.person_recording
                                               });

            se.Dispose();
            return mp;
        }

        public List<object> getUnprocessedMpesaPayments()
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            List<object> mp = new List<object>();
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
                    mp.Add(new
                    {
                        Date = tmp.Date,
                        Reference = tmp.Reference,
                        Amount = tmp.Amount,
                        Number = tmp.Phone,
                        Message = tmp.Message
                    });
                }
                catch
                {

                }
            }
            se.Dispose();
            return mp;
        }

        public List<payRecordClass2> getPaymentPerCustomer(string id)
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            int daily_invoice = 0;
            int total_invoice = 0;
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
                                orderby tp.payment_date descending
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
            se.Dispose();
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
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            //find in class
            paymentRecording pr = new classes.paymentRecording();
            pr.recordpayment(msg1, se, false);
            string res = pr.Json;
            se.Dispose();
        }

        [HttpPost]
        public string postReceive_mpesa([FromBody]JArray value)
        {
            Thread.Sleep(1000);
            db_a0a592_sunamiEntities se;
            try
            {
                se = new db_a0a592_sunamiEntities();
            }
            catch (Exception g)
            {
                return g.Message;

            }
            string res = "";
            JToken token = JObject.Parse(value[0].ToString());

            // only record if message is from mpesa
            if (token.SelectToken("address").ToString() == "MPESA")
            {
                try
                {
                    string imei_no = token.SelectToken("imei").ToString();
                    string msg1 = token.SelectToken("msg").ToString();
                    int tblm = se.tbl_mpesa_phone_imei.Where(i => i.imei == imei_no).Count();
                    if (tblm < 1)
                    {
                        res = null;
                    }
                    else
                    {
                        res = record_mpesa_msg_phone(msg1, imei_no);
                    }
                }
                catch (Exception k)
                { }
            }

            se.Dispose();
            return res;
        }

        private string record_mpesa_msg_phone(string msg1, string imei_no)
        {
            paymentRecording pr = new classes.paymentRecording();
            db_a0a592_sunamiEntities se;
            try
            {
                se = new db_a0a592_sunamiEntities();
            }
            catch (Exception g)
            {
                return g.Message;

            }
            pr.Phone_imei = imei_no;
            pr.recordpayment(msg1, se, true);
            string res = pr.Json;
            se.Dispose();
            return res;
        }

        public List<object> getCustomerDetails()
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            List<object> list = new List<object>(from i in se.tbl_customer
                                                 //where i.active_status == true
                                                 orderby i.install_date descending
                                                 select new
                                                 {
                                                     Name = i.customer_name,
                                                     ID = i.customer_id,
                                                     Occupation = i.occupation,
                                                     Mobile = i.phone_numbers,
                                                     village = i.village_name,
                                                     location = i.location,
                                                     city = i.city,
                                                     installdate = i.install_date,
                                                     Witness = i.next_of_kin,
                                                     Witness_ID = i.nok_mobile,
                                                     status = i.active_status
                                                 }
                      );
            se.Dispose();
            return list;
        }

        public object getSingleCustomerDetails(string id)
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            tbl_customer i = se.tbl_customer.FirstOrDefault(t1 => t1.customer_id == id);
            object customer = (new {
                Name = i.customer_name,
                ID = i.customer_id,
                Occupation = i.occupation,
                Mobile = i.phone_numbers,
                Mobile2 = i.phone_numbers2,
                Mobile3 = i.phone_numbers3,
                village = i.village_name,
                location = i.location,
                city = i.city,
                installdate = i.install_date.Value.Date,
                Witness = i.next_of_kin,
                Witness_ID = i.nok_mobile,
                status = i.active_status,
                Package = i.package_type
            });
                
            se.Dispose();
            return customer;
        }

        public List<object> getSystemDetails()
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            List<object> list = new List<object>(from ts in se.tbl_system
                                                 join tc in se.tbl_customer on ts.customer_id equals tc.customer_id
                                                 join tsc in se.tbl_sunami_controller on ts.imei_number equals tsc.imei
                                                 orderby tc.install_date ascending
                                                 select new
                                                 {
                                                     Owner = tc.customer_name,
                                                     installdate = "i" + tc.install_date,
                                                     Active_Status = ts.active_status,
                                                     Last_System_Communication = "l" + ts.last_connected_to_db_date,
                                                     Imei = ts.imei_number,
                                                     SystemPhoneNumber = tsc.sim_no
                                                 }
                    );
            se.Dispose();
            return list;
        }

        public List<object> getswitchlogs()
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            List<object> list = new List<object>(from ts in se.tbl_switch_logs
                                                 join tc in se.tbl_customer on ts.customer_id equals tc.customer_id
                                                 join tsci in se.tbl_system on ts.customer_id equals tsci.customer_id
                                                 join tscn in se.tbl_sunami_controller on tsci.imei_number equals tscn.imei
                                                 orderby ts.switch_off_date descending
                                                 select new
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
            se.Dispose();
            return list;
        }

        public List<object> getSunamiControllers()
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            List<object> list = new List<object>(from ts in se.tbl_sunami_controller
                                                 orderby ts.Id ascending
                                                 select new
                                                 {
                                                     Imei = ts.imei,
                                                     Sim_Number = ts.sim_no,
                                                     Provider = ts.provider,
                                                     Version = ts.version,
                                                     Reg_Date = ts.date_manufactured,
                                                     Recorded_By = ts.recorded_by
                                                 }
                    );
            se.Dispose();
            return list;
        }

        public List<object> getFreeImei()
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            var list1 = se.tbl_system.Select(g => new { imei1 = g.imei_number }).ToList();
            var list2 = se.tbl_sunami_controller.Select(f => new { imei2 = f.imei }).ToList();
            List<object> list = new List<object>(list2.Where(t => !list1.Any(t2 => t2.imei1 == t.imei2)).Select(r => new { FreeImei = r.imei2 }));
            se.Dispose();
            return list;
        }

        public List<object> getCustomersWithNoController()
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            var list1 = se.tbl_system.Select(g => new { ids1 = g.customer_id }).ToList();
            var list2 = se.tbl_customer.Select(f => new { ids2 = f.customer_id, Name = f.customer_name }).ToList();
            List<object> list = new List<object>(list2.Where(t => !list1.Any(t2 => t2.ids1 == t.ids2)).Select(r => new { Name = r.Name, Id = r.ids2 }));
            se.Dispose();
            return list;
        }

        [HttpPost]
        public string postLinkController([FromBody]JArray value)
        {
            db_a0a592_sunamiEntities se;
            try
            {
                se = new db_a0a592_sunamiEntities();
            }
            catch (Exception g)
            {
                return g.Message;
            }
            string res = "";
            try
            {
                //{ imei: this.imei, sim: this.sim, provider: this.provider, version: this.version, loogeduser: this.disname }
                JToken token = JObject.Parse(value[0].ToString());
                string imei = token.SelectToken("imei").ToString();
                string loogeduser = token.SelectToken("loogeduser").ToString();
                string customer_id = token.SelectToken("customer_id").ToString();

                int ts4 = se.tbl_system.Where(i => i.customer_id == customer_id).Count();
                if (ts4 == 0)
                {
                    tbl_system ts = new tbl_system();
                    ts.customer_id = customer_id;
                    ts.active_status = false;
                    ts.imei_number = imei;
                    ts.current_lat = null;
                    ts.current_lon = null;
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
            se.Dispose();
            return res;
        }

        public List<object> getIssues()
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            List<object> list = new List<object>(from tc in se.tbl_customer
                                                 join ti in se.tbl_issues on tc.customer_id equals ti.customer_id
                                                 orderby ti.date ascending
                                                 select new
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
            se.Dispose();
            return list;
        }

        [HttpPost]
        public string postNewIssues([FromBody]JArray value)
        {
            db_a0a592_sunamiEntities se;
            try
            {
                se = new db_a0a592_sunamiEntities();
            }
            catch (Exception g)
            {
                return g.Message;
            }
            string res = "";
            try
            {
                JToken token = JObject.Parse(value[0].ToString());
                string id = token.SelectToken("id").ToString();
                string issue = token.SelectToken("issue").ToString();
                string reporter = token.SelectToken("reporter").ToString();
                string priority = token.SelectToken("priority").ToString();
                tbl_issues ti = new tbl_issues();
                ti.customer_id = id;
                ti.priority = priority;
                ti.issue = issue;
                ti.date = DateTime.Now;
                ti.status = "pending";
                ti.reporter = reporter;
                se.tbl_issues.Add(ti);
                se.SaveChanges();
                res = "succesfull reported an issue";
            }
            catch
            {
                res = "error recording the issue in the database";
            }
            se.Dispose();
            return res;
        }

        [HttpPost]
        public string postSolveIssues([FromBody]JArray value)
        {
            db_a0a592_sunamiEntities se;
            try
            {
                se = new db_a0a592_sunamiEntities();
            }
            catch (Exception g)
            {
                return g.Message;
            }
            string res = "";
            try
            {
                JToken token = JObject.Parse(value[0].ToString());
                int Scustomer = int.Parse(token.SelectToken("Id").ToString());
                string solver = token.SelectToken("Ssolver").ToString();
                string Scomment = token.SelectToken("Scomment").ToString();

                tbl_issues ti = se.tbl_issues.FirstOrDefault(f => f.Id == Scustomer);
                ti.status = "solved";
                ti.solvedOn = DateTime.Now;
                ti.solvedBy = solver;
                ti.comments = Scomment;
                se.SaveChanges();
                res = "succesfully solved an issue";
            }
            catch
            {
                res = "error marking issue as solved in database";
            }
            se.Dispose();
            return res;
        }

        public List<object> getUninstalledSystems()
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            List<object> list = new List<object>(from tu in se.tbl_uninstalled_systems
                                                 join tc in se.tbl_customer
                                                 on tu.customer_id equals tc.customer_id
                                                 orderby tu.uninstall_date descending
                                                 select new
                                                 {
                                                     Name = tc.customer_name,
                                                     Id = tc.customer_id,
                                                     install_date = tc.install_date,
                                                     Uninstalled_on = tu.uninstall_date,
                                                     Reason = tu.Reason,
                                                     unistalledBy = tu.recorded_by,
                                                     previousRecords = tu.previousRecords
                                                 });
            se.Dispose();
            return list;
        }

        [HttpPost]
        public string postUninstall([FromBody]JArray value)
        {
            db_a0a592_sunamiEntities se;
            try
            {
                se = new db_a0a592_sunamiEntities();
            }
            catch (Exception g)
            {
                return g.Message;
            }
            JToken token = JObject.Parse(value[0].ToString());
            string date1 = token.SelectToken("date1").ToString();
            string customer_id = token.SelectToken("customer_id").ToString();
            string recorded_by = token.SelectToken("recorded_by").ToString();
            string reason = token.SelectToken("reason").ToString();

            string res = "";
            try
            {
                //yyyy-mm-dd e.g. 2017-04-05 - date1
                DateTime date2 = getDate(date1);

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
                us.customer_id = customer_id;
                us.uninstall_date = date2;
                us.recorded_by = recorded_by;
                us.Reason = reason;
                us.previousRecords = possesions;
                se.tbl_uninstalled_systems.Add(us);
                
                // set all invoiced items taken dates
                foreach(tbl_extra_package_customers tep in se.tbl_extra_package_customers.Where(r=>r.customer_id == customer_id))
                {
                    if (tep.date_taken == null)
                    {
                        tep.date_taken = DateTime.Today;
                    } else {
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
            se.Dispose();
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

        public List<object> getActiveCustomersDetails()
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            // List<object> list = new List<object>(se.tbl_customer.Where(r => r.active_status == true).Select(g => new
            List<object> list = new List<object>(se.tbl_customer.Select(g => new
            {
                Name = g.customer_name,
                Customer_id = g.customer_id
            }).OrderBy(G => G.Name));
            se.Dispose();
            return list;
        }

        [HttpPost]
        public string postExpense([FromBody]JArray value)
        {
            db_a0a592_sunamiEntities se;
            try
            {
                se = new db_a0a592_sunamiEntities();
            }
            catch (Exception g)
            {
                return g.Message;
            }
            string res = "error";
            JToken token = JObject.Parse(value[0].ToString());
            string description = token.SelectToken("category").ToString();
            string amount = token.SelectToken("amount").ToString();
            string recipient = token.SelectToken("recipient").ToString();
            string dateset = token.SelectToken("dateset").ToString();
            string vendor = token.SelectToken("vendor").ToString();

            //yyyy-mm-dd e.g. 2017-04-05 - date1
            DateTime date2 = getDate(dateset);

            string account = token.SelectToken("account").ToString();
            string ref_code = token.SelectToken("ref_code").ToString();
            string recordedBy = token.SelectToken("recordedBy").ToString();

            //if image was sent as base64
            string pic1 = token.SelectToken("pic1").ToString();
            byte[] pic = Convert.FromBase64String(pic1);

            //get recipients email
            try
            {
                string email = se.tbl_users.FirstOrDefault(f2 => f2.name == recipient).email;
                recipient = email;
            }
            catch
            {

            }

            try
            {
                tbl_expenses exp = new tbl_expenses();
                exp.account = account;
                exp.amount = amount;
                exp.vendor = vendor;
                exp.recipient = recipient;
                exp.dateset = date2;
                exp.ref_code = ref_code;
                exp.image = pic;
                exp.description = description;
                exp.RecordedBy = recordedBy;
                se.tbl_expenses.Add(exp);
                se.SaveChanges();
                res = "successfully recorded new expense";
            }
            catch
            {
                res = "error recording new expense. please contact admin";
            }
            se.Dispose();
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

        public List<object> getAllExpenses()
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            var expenses = se.tbl_expenses.ToList();

            List<object> li = new List<object>();
            foreach (var f in expenses)
            {
                string usernm = se.tbl_users.FirstOrDefault(f1 => f1.email == f.recipient).name;
                if (f.image.Length < 2)
                {
                    li.Add(new
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
                    li.Add(new
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
            se.Dispose();
            return li;
        }

        public byte[] getReceipt(string id)
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            int idd = int.Parse(id);
            byte[] pic = se.tbl_expenses.FirstOrDefault(f => f.Id == idd).image;
            se.Dispose();
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

        public List<Object> getExpenseCategories()
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            List<object> list = new List<object>(se.tbl_expense_categories.Select(g => new
            {
                Name = g.Name,
                Details = g.Details
            }).OrderBy(G => G.Name));
            se.Dispose();
            return list;
        }

        public List<Object> getUserNames()
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            List<object> list1 = new List<object>();
            var users = se.tbl_users.ToList().OrderBy(f => f.name);
            foreach (var g in users)
            {
                list1.Add(new
                {
                    Name = g.name,
                    email = g.email
                });
            }
            se.Dispose();
            return list1;
        }

        [HttpPost]
        public string postNewCustomer([FromBody]JArray value)
        {
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            DateTime date2 = DateTime.Today;
            string item = "";
            string res = "error";
            JToken token = JObject.Parse(value[0].ToString());
            try
            {
                registerCustomer rc = new registerCustomer();
                rc.Id = token.SelectToken("id").ToString();
                rc.Box = token.SelectToken("box").ToString();
                rc.City = token.SelectToken("city").ToString();
                rc.LatG = token.SelectToken("latG").ToString();
                rc.LonG = token.SelectToken("lonG").ToString();
                rc.Name = token.SelectToken("name").ToString();
                rc.Number = token.SelectToken("number1").ToString();
                rc.Number2 = token.SelectToken("number2").ToString();
                rc.Number3 = token.SelectToken("number3").ToString();
                rc.Occupation = token.SelectToken("occupation").ToString();
                rc.Village = token.SelectToken("village").ToString();
                rc.Witness = token.SelectToken("witness").ToString();
                rc.Witnessid = token.SelectToken("witnessid").ToString();
                rc.Description = token.SelectToken("description").ToString();
                try
                {
                    rc.RecordedBy = token.SelectToken("recordedBy").ToString();
                }
                catch
                {
                    rc.RecordedBy = "anonymous";
                }
                rc.Country = "Kenya";

                try
                {
                    //rc.Package = token.SelectToken("package").ToString();
                    rc.Package = "single_2018(100)";
                }
                catch {
                    rc.Package = "single_2018(100)";
                }
                try
                {
                    date2 = getDate(token.SelectToken("date1").ToString());//yyyy-mm-dd e.g. 2017-04-05 - date1
                    rc.Install_date = date2;
                }
                catch {
                    
                }
                try
                {
                    rc.Location = token.SelectToken("location").ToString();
                }
                catch
                {
                    rc.Location = token.SelectToken("village").ToString();
                }
                rc.record();
                res = rc.Confirm;



                // invoice new customer
                //TODO - call invoice function - avoid duplication of code
                item = rc.Package;
                
                if (!se.tbl_extra_package_customers.Where(r => r.customer_id == rc.Id).Select(t => t.item).Contains(item))
                {
                    tbl_extra_package_customers epc = new tbl_extra_package_customers();
                    epc.customer_id = rc.Id;
                    epc.item = item;
                    epc.date_given = date2;
                    se.tbl_extra_package_customers.Add(epc);
                    se.SaveChanges();
                    // this.logevent(rc.RecordedBy, rc.Id, DateTime.Today, "Invoiced customer a " + item, "Invoice Customer");
                } else
                {
                    tbl_extra_package_customers tepc = se.tbl_extra_package_customers.FirstOrDefault(f => f.customer_id == rc.Id && f.item == item);
                    tepc.date_given = date2;
                    se.SaveChanges();
                }
                // end of invoicing

            }
            catch (Exception kk)
            {
                //res = "Error retrieving info from json sent";
                res = kk.Message;

            }
            if (!res.Contains("registered new customer"))
            {
                logevent(token.SelectToken("recordedBy").ToString(), token.SelectToken("id").ToString(), DateTime.Now, res, "customer registration");
            }
            se.Dispose();
            return res;
        }

        public List<object> getUser(string id)
        {
            try
            {
                db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
                List<object> list = new List<object>(se.tbl_users.Where(f => f.email == id).Select(g => new
                {
                    allowed = g.allow,
                    level = g.level
                }));
                se.Dispose();
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
        public string postmakePayment([FromBody]JArray value)
        {
            string res = "";
            // Thread.Sleep(1000);
            db_a0a592_sunamiEntities se;
            try
            {
                se = new db_a0a592_sunamiEntities();
            }
            catch (Exception g)
            {
                res = g.Message;
                return res;
            }
            paymentRecording pr = new paymentRecording();
            try
            {
                string bankname = null;
                JToken token = JObject.Parse(value[0].ToString());
                string loggedUser = token.SelectToken("loggedUser").ToString();
                string PayMode = token.SelectToken("PayMode").ToString();
                string Code = token.SelectToken("Code").ToString();
                string Id = token.SelectToken("Customer_Id").ToString();
                try
                {
                    bankname = token.SelectToken("bankname").ToString();
                }catch { }

                if (PayMode == "mpesa")
                {
                    tbl_mpesa_payments tmp = se.tbl_mpesa_payments.FirstOrDefault(g => g.transaction_code == Code);
                    pr.Mdate = DateTime.Parse(tmp.date.ToString());
                    pr.Mpesa_amount = tmp.amount;
                }


                else if (PayMode == "cash")
                {
                    pr.Mdate = getDate(token.SelectToken("date1").ToString());
                    pr.Mpesa_amount = token.SelectToken("amount").ToString();
                    if(Code.Length > 5)
                    {
                        PayMode = "mpesa";
                    }
                }

                else if(PayMode == "bank")
                {
                    pr.Mdate = getDate(token.SelectToken("date1").ToString());
                    pr.Mpesa_amount = token.SelectToken("amount").ToString();
                    if(bankname.Length > 2)
                    {
                        PayMode = PayMode + "_"+ bankname;
                    }
                }

                

                pr.Loggedin = loggedUser;
                pr.PayMode = PayMode;
                pr.Code = Code;
                pr.Id = Id;
                pr.process_transaction(se, true);
                res += pr.Json;
                se.Dispose();
                return res;

            }
            catch (Exception g)
            {
                res = g.Message;
                se.Dispose();
                return res;
            }
        }

        private void logevent(string loggeduser, string customerid, DateTime date, string event1, string category)
        {
            try
            {
                db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
                tbl_event_logs ev = new tbl_event_logs();
                ev.category_affected = category;
                ev.customer_id = customerid;
                ev.date = date;
                ev.@event = event1;
                ev.loggedin_user = loggeduser;
                se.tbl_event_logs.Add(ev);
                se.SaveChanges();
                se.Dispose();
            }
            catch (Exception f)
            {

            }
        }

        [HttpGet]
        public List<object> eventlogs()
        {
            List<object> list = new List<object>();
            db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
            list = new List<object>(from t in se.tbl_event_logs
                                    orderby t.date descending
                                    select new {
                                                    Category = t.category_affected,
                                                    CustomerId = t.customer_id,
                                                    Date = t.date,
                                                    Event = t.@event,
                                                    LoggedInUser = t.loggedin_user
                                               });
            return list;
        }

        [HttpGet]
        public List<object> getInventory(string id)
        {
            try
            {
                List<object> inventory = new List<object>();
                DateTime date1 = getDate(id).AddDays(1);
                db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
                var items = se.tbl_inventory.Select(f => f.Item).Distinct().ToList();
                foreach (var item in items)
                {
                    DateTime itemlistdate = se.tbl_inventory.Where(g => g.date <= date1 && g.Item == item).Max(f => f.date);
                    tbl_inventory itemlist = se.tbl_inventory.FirstOrDefault(g => g.date == itemlistdate && g.Item == item);
                    inventory.Add(itemlist);
                }
                se.Dispose();
                return inventory;
            }
            catch (Exception f)
            {
                return null;
            }
        }

        [HttpGet]
        public List<object> getStockDetails(string id)
        {
            try
            {
                db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
                List<object> inventory = new List<object>(se.tbl_inventory.Where(u => u.Item == id));
                se.Dispose();
                return inventory;
            }
            catch (Exception f)
            {
                return null;
            }
        }

        [HttpPost]
        public string postRecordItem([FromBody]JArray value)
        {
            try
            {
                db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
                JToken token = JObject.Parse(value[0].ToString());
                string loggedUser = token.SelectToken("loogeduser").ToString();
                string item = token.SelectToken("itemName").ToString();
                string currentStock = token.SelectToken("Stock").ToString();
                string units = token.SelectToken("units").ToString();
                tbl_inventory ti = new tbl_inventory();
                ti.date = DateTime.Now;
                ti.Item = item;
                ti.units = units;
                ti.stock = int.Parse(currentStock);
                se.tbl_inventory.Add(ti);
                se.SaveChanges();
                se.Dispose();
                return "New item successfully recorded";
            }
            catch (Exception f)
            {
                return f.Message;
            }
        }

        public string postUpdateStock([FromBody] JArray value)
        {
            try
            {
                int? stock = 0;
                db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
                JToken token = JObject.Parse(value[0].ToString());
                string loogeduser = token.SelectToken("loogeduser").ToString();
                int? number = int.Parse(token.SelectToken("number").ToString());
                string method = token.SelectToken("method").ToString();//dispense and add
                string comment = token.SelectToken("comment").ToString();
                string item = token.SelectToken("item").ToString();
                int maxinv = se.tbl_inventory.Where(y => y.Item == item).Max(f => f.Id);
                tbl_inventory ti = se.tbl_inventory.SingleOrDefault(fr => fr.Id == maxinv && fr.Item==item);

                if (method == "dispense")
                {
                    stock = ti.stock - number;
                    if(stock < 0)
                    {
                        return "OUT OF STOCK, you cannot dispense "+number;
                    }
                }
                else if (method == "add")
                {
                    stock = ti.stock + number;
                }
                tbl_inventory tia = new tbl_inventory();
                tia.comments = comment;
                tia.date = DateTime.Now;
                tia.Item = item;
                tia.stock = stock;
                tia.units = ti.units;
                se.tbl_inventory.Add(tia);
                se.SaveChanges();
                se.Dispose();
                return "successfully updated stocks";
            }
            catch (Exception f)
            {
                return f.Message;
            }
        }
    }
}

