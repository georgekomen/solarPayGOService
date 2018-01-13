using Newtonsoft.Json;
using sunamiapi.codeIncludes;
using sunamiapi.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class paymentRecording
    {
        //pay flag-new or already recorded
        private string loggedin;
        private string code;
        private string message1;
        private string message;//sms
        private string customer_id;
        private string payMode;
        private string json;
        private DateTime mdate; //mpesa message date
        private string mpesa_amount;
        private string paynumber;
        private Dictionary<string, string> response1;//hold parameters for message reply
        private string phone_imei;
        private string mpesa_number;
        private string customer_name;
        public string Json
        {
            get
            {
                return json;
            }
        }

        public string PayMode
        {
            set
            {
                payMode = value;
            }
        }

        public string Mpesa_amount
        {
            set
            {
                mpesa_amount = value;
            }
        }

        public DateTime Mdate
        {
            set
            {
                mdate = value;
            }
        }

        public string Id
        {
            set
            {
                customer_id = value;
            }
        }


        public string Message
        {
            get
            {
                return message;
            }

            set
            {
                message = value;
            }
        }
        

        public string Code
        {
            get
            {
                return code;
            }

            set
            {
                code = value;
            }
        }

        public string Loggedin
        {
            get
            {
                return loggedin;
            }

            set
            {
                loggedin = value;
            }
        }

        public string Phone_imei
        {
            get
            {
                return phone_imei;
            }

            set
            {
                phone_imei = value;
            }
        }
        public string Paynumber
        {
            get
            {
                return paynumber;
            }

            set
            {
                paynumber = value;
            }
        }

        public void recordpayment(string msg, db_a0a592_sunamiEntities se)
        {
            //if its from phone or mledger the paymode is mpesa
            payMode = "mpesa";
            //PREPARE RESPONSE
            string date = "";
            response1 = new Dictionary<string, string>();
            try
            {
                //if not payment received.record it here
                if (msg.Contains("transferred") || msg.Contains("repaid") || msg.Contains("bought") || msg.Contains("Withdraw") || msg.Contains("Give"))
                {
                    string ms = msg;
                    var md = ms.Split(' ');
                    code = md[0].ToString();
                    date = md[2].ToString();
                    int f = date.IndexOf("/", 3) + 1;
                    date = date.Substring(0, f) + DateTime.Today.Year.ToString();
                    DateTimeFormatInfo info = new CultureInfo("en-us", true).DateTimeFormat;
                    info.ShortDatePattern = "dd-MM-yyyy";
                    mdate = Convert.ToDateTime(date, info);
                    mpesa_amount = md[5].ToString();
                    int d = mpesa_amount.IndexOf('.');
                    mpesa_amount = mpesa_amount.Substring(5, d - 5).ToString().Replace(@",", @"");


                    if (se.tbl_mpesa_payments.Select(r1 => r1.transaction_code).Contains(code))
                    {
                        json = "unprocessed transaction recorded";
                        return;
                    }
                    else
                    {
                        if (se.tbl_payments.Select(r1 => r1.transaction_code).Contains(code))
                        {
                            json = "transaction code already recorded";
                            return;
                        }
                        else
                        {
                            tbl_mpesa_payments tm = new tbl_mpesa_payments();
                            tm.message = msg;
                            tm.amount = mpesa_amount;
                            tm.transaction_code = code;
                            tm.date = mdate;
                            tm.phone_imei = phone_imei;
                            se.tbl_mpesa_payments.Add(tm);
                            se.SaveChanges();
                            json = "not received money message";
                        }
                    }


                }
                else if(msg.Contains("Confirmed.on"))
                {
                    string ms = msg;
                    var md = ms.Split(' ');
                    code = md[0].ToString();
                    date = md[2].ToString();
                    int f = date.IndexOf("/", 3) + 1;
                    date = date.Substring(0, f) + DateTime.Today.Year.ToString();
                    DateTimeFormatInfo info = new CultureInfo("en-us", true).DateTimeFormat;
                    info.ShortDatePattern = "dd-MM-yyyy";
                    mdate = Convert.ToDateTime(date, info);
                    mpesa_amount = md[5].ToString();
                    int d = mpesa_amount.IndexOf('.');
                    mpesa_amount = mpesa_amount.Substring(5, d - 5).ToString().Replace(@",", @"");
                    paynumber = md[8].ToString();
                    paynumber = "0" + paynumber.Substring(3, paynumber.Length - 3).ToString();


                    if(se.tbl_payments.Select(r1 => r1.transaction_code).Contains(code))
                    {
                        json = "payment already recorded";
                        return;
                    }
                    else
                    {
                        if(se.tbl_mpesa_payments.Select(r1 => r1.transaction_code).Contains(code))
                        {
                            json = "message already recorded";
                        }
                        else
                        {
                            tbl_mpesa_payments tm = new tbl_mpesa_payments();
                            tm.date = mdate;
                            tm.message = msg;
                            tm.amount = mpesa_amount;
                            tm.number = paynumber;
                            tm.transaction_code = code;
                            tm.phone_imei = phone_imei;
                            se.tbl_mpesa_payments.Add(tm);
                            se.SaveChanges();
                        }
                        //record in payments
                        process_transaction(se);
                    }
                }
            }
            catch (Exception k)
            {
                json = k.Message;
            }
        }

        public void process_transaction(db_a0a592_sunamiEntities se)
        {
            tbl_customer tc1 = new tbl_customer();
            try
            {
                if (!string.IsNullOrEmpty(paynumber) || !string.IsNullOrWhiteSpace(paynumber))
                {
                    //get customer id- for mpesa and mledger from their phone numbers
                    try
                    {
                        tc1 = se.tbl_customer.FirstOrDefault(i => i.phone_numbers == paynumber || i.phone_numbers2 == paynumber || i.phone_numbers3 == paynumber);
                        mpesa_number = paynumber;
                        customer_id = tc1.customer_id;
                        var customernames = tc1.customer_name.Split(' ');
                        customer_name = customernames[0];
                    }
                    catch(Exception g)
                    {
                        //phone number making payments not found in db
                        json = g.Message;
                        //failed to process payment-payment number not in db
                        //sending sms to unrecorded phone numbers
                        message = "Sunami solar imepokea malipo yako ya Ksh" + mpesa_amount + " .Tafadhali tupigie simu ili utueleze accounti yako";
                        //response1.Add("message", message1);
                        //response1.Add("number", paynumber);
                        sendSmsThroughGateway(mpesa_number);
                        return;
                    }
                }
                //get number of customer from cash payment's id number-number that receives sms
                try
                {
                    tc1 = se.tbl_customer.FirstOrDefault(i => i.customer_id == customer_id);
                    customer_id = tc1.customer_id;
                    var customernames = tc1.customer_name.Split(' ');
                    customer_name = customernames[0];
                    paynumber = tc1.phone_numbers;
                }
                catch (Exception g)
                {
                    json = g.Message;
                    return;
                }
                int mpesa_amount1 = int.Parse(mpesa_amount);
                //paymode
                //check if same payment had been recorded
                try
                {
                    int tpp1 = se.tbl_payments.Where(i => i.amount_payed == mpesa_amount1 && i.payment_date == mdate && i.customer_id == customer_id && i.transaction_code == code).Count();
                    if (tpp1 >= 1)
                    {
                        json = "payment already recorded";
                        return;
                    }
                    //also ensure the transaction code doesn't exist anywea
                    if (!string.IsNullOrEmpty(code))
                    {
                        int tcc = se.tbl_payments.Where(j => j.transaction_code == code).Count();
                        if (tcc >= 1)
                        {
                            json = "payment already recorded";
                            return;
                        }
                    }
                }
                catch(Exception k)
                {
                    json = k.Message;
                }
                tbl_payments tp = new tbl_payments();
                //if payment was made before installation then credit payment as made on that installation date
                if (tc1.install_date > mdate)
                {
                    tp.payment_date = tc1.install_date;
                }
                else
                {
                    tp.payment_date = mdate;
                }
                tp.amount_payed = mpesa_amount1;
                tp.customer_id = customer_id;
                tp.payment_method = payMode;
                tp.transaction_code = code;
                if (!string.IsNullOrEmpty(code))
                {
                    tp.phone_number = paynumber;
                }
                if (string.IsNullOrEmpty(loggedin))
                {
                    tp.person_recording = "automatic";
                }
                else
                {
                    tp.person_recording = loggedin;
                }
                if (!string.IsNullOrEmpty(code))
                {
                    tp.date_recorded = mdate;
                }
                else if (string.IsNullOrEmpty(code))
                {
                    tp.date_recorded = DateTime.Now;
                }
                tp.balance = 0;                               
                se.tbl_payments.Add(tp);
                se.SaveChanges();
                message1 = "Thank you for your payment of Ksh" + mpesa_amount;
                response1.Add("message", message1);
                response1.Add("number", paynumber);
                preparesms(se);
                json = "successfully recorded payment";
            }
            catch (Exception kl)
            {
                
            }
        }

        private void preparesms(db_a0a592_sunamiEntities se)
        {
            int? paid = 0;
            int invoice = 0;
            try
            {
                try
                {
                    //get installation date
                    DateTime? instd = se.tbl_customer.FirstOrDefault(h => h.customer_id == customer_id).install_date;//Value.Date.ToString("dd/MM/yyyy");
                                                                                                            //add all payments....................................
                    paid = se.tbl_payments.Where(g => g.customer_id == customer_id).Sum(t => t.amount_payed);
                    //get when he should make next payment -- get todays date
                    DateTime toda = DateTime.Today;
                    //get invoice to date...............................
                    extra_package_invoicing ep = new classes.extra_package_invoicing();
                    invoice += ep.extr_invoice(instd, toda, customer_id);
                    //or how much he still needs to buy
                } catch { }
                int? bal = invoice - paid;

                if (bal <= 0)
                {
                    message = customer_name+", Sunami solar inakushukuru kwa malipo yako ya Ksh" + mpesa_amount;
                }

                else
                {
                    message = customer_name +", Sunami solar inakushukuru kwa malipo yako ya Ksh" + mpesa_amount + ".Bado unadaiwa Ksh" + bal + " ya siku zilizopita";
                }
                sendSmsThroughGateway(paynumber);
            }
            catch { }

        }


        private void sendSmsThroughPhone()
        {
            //string json = JsonConvert.SerializeObject(points, Formatting.Indented);
            json = JsonConvert.SerializeObject(response1, Formatting.Indented);
        }

        public void sendSmsThroughGateway(string num)
        {
            sendSms ss = new sendSms();
            ss.sendSmsThroughGateway(num, message);
        }

    }
}