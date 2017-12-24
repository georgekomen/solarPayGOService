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
        private string id;
        private string payMode;
        private string json;
        private int bal_record;
        private int? bal;
        private DateTime mdate; //mpesa message date
        private string mpesa_amount;
        private string paynumber;
        private DateTime to_this_day;
        private Dictionary<string, string> response1;//hold parameters for message reply
        private string phone_imei;
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
                id = value;
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
                        se.Dispose();                   
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
            try
            {
                if (!string.IsNullOrEmpty(paynumber) || !string.IsNullOrWhiteSpace(paynumber))
                {
                    //get customer id- for mpesa and mledger from their phone numbers
                    try
                    {
                        tbl_customer tc1 = se.tbl_customer.FirstOrDefault(i => i.phone_numbers == paynumber || i.phone_numbers2 == paynumber || i.phone_numbers3 == paynumber);
                        id = tc1.customer_id;
                    }
                    catch(Exception g)
                    {
                        //phone number making payments not found in db
                        json = g.Message;
                        return;
                    }
                }

                //get number of customer from cash payment's id number-number that receives sms
                try
                {
                    tbl_customer tc1 = se.tbl_customer.FirstOrDefault(i => i.customer_id == id);
                    paynumber = tc1.phone_numbers;
                }
                catch (Exception g)
                {
                    json = g.Message;
                }

                int mpesa_amount1 = int.Parse(mpesa_amount);
                //paymode
                //check if same payment had been recorded
                try
                {
                    int tpp1 = se.tbl_payments.Where(i => i.amount_payed == mpesa_amount1 && i.payment_date == mdate && i.customer_id == id && i.transaction_code == code).Count();
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
                //if not already recorded get package amount-calculate balances and dates
                tbl_customer tc = se.tbl_customer.FirstOrDefault(i => i.customer_id == id);
                string ts = se.tbl_customer.FirstOrDefault(o => o.customer_id == tc.customer_id).package_type;
                int amount_per_daya = se.tbl_packages.FirstOrDefault(k => k.type == ts).amount_per_day;
                tbl_payments tp = new tbl_payments();
                //if payment was made before installation then credit payment as made on that installation date
                if (tc.install_date > mdate)
                {
                    tp.payment_date = tc.install_date;
                }
                else
                {
                    tp.payment_date = mdate;
                }
                tp.amount_payed = mpesa_amount1;
                tp.customer_id = id;
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
                             
                //calculate number of days from amount payed
                try
                {

                    //continuing payment
                    int tp1 = se.tbl_payments.Where(i => i.customer_id == id).Max(i => i.Id);
                    tbl_payments tpp = se.tbl_payments.FirstOrDefault(i => i.Id == tp1);
                    //handle balances--------------------------------------------------------------------------
                    try
                    {
                        //get balance-catch if bal is null
                        bal = tpp.balance;
                    }
                    catch
                    {
                        bal = 0;
                    }
                    string bal1 = bal.ToString();
                    int total2 = (mpesa_amount1 + int.Parse(bal1));
                    int add_days = total2 / amount_per_daya;
                    //get balance
                    bal_record = total2 % amount_per_daya; //get modulus---balance
                    //save balance
                    if (total2 < amount_per_daya)
                    {
                        tp.balance = total2;
                    }
                    else
                    {
                        tp.balance = bal_record;
                    }
                    //handle balances-----------------------------------------------------------------------------
                   
                }
                catch
                {
                    //handle balances-----------------------------------------------------------------------------
                    bal_record = mpesa_amount1 % amount_per_daya; //get modulus---balance
                    //save balance
                    tp.balance = bal_record;
                    //handle balances-----------------------------------------------------------------------------
                }
                se.tbl_payments.Add(tp);
                se.SaveChanges();
                json = "successfully recorded payment";
                message1 = "Thank you for your payment of Ksh" + mpesa_amount + ". Your system will be active till " + to_this_day.ToString();

                response1.Add("message", message1);
                response1.Add("number", paynumber);
                preparesms(se); 
            }
            catch (Exception kl)
            {
                //failed to process payment-payment number not in db
                //sending sms to unrecorded phone numbers
                message = "Sunami imepoke malipo Ksh" + mpesa_amount + " kutoka kwa nambari hili. Tafadhali tupigie simu ili utueleze accounti yako";
                //response1.Add("message", message1);
                //response1.Add("number", paynumber);
                sendSmsThroughGateway();
            }
        }

        private void preparesms(db_a0a592_sunamiEntities se)
        {
            try
            {
                //get installation date
                DateTime? instd = se.tbl_customer.FirstOrDefault(h => h.customer_id == id).install_date;//Value.Date.ToString("dd/MM/yyyy");
                                                                                                        //add all payments....................................
                int? paid = se.tbl_payments.Where(g => g.customer_id == id).Sum(t => t.amount_payed);
                //get when he should make next payment -- get todays date
                DateTime toda = DateTime.Today;
                //get invoice to date...............................
                //get package type
                string packtype = se.tbl_customer.FirstOrDefault(g => g.customer_id == id).package_type;
                int pack_amount = se.tbl_packages.FirstOrDefault(t => t.type == packtype).amount_per_day;
                //get invoice
                int days = (toda - instd).Value.Days;
                int invoice = pack_amount * days;
                //add incase in an advanced package
                extra_package_invoicing ep = new classes.extra_package_invoicing();
                invoice += ep.extr_invoice(instd, toda, id);
                //or how much he still needs to buy
                int? bal = invoice - paid;

                if (bal <= 0)
                {
                    //get extra daily invoice from extra package class
                    pack_amount += ep.Ext_daily_invoice;
                    //get days paid for
                    int? days2 = paid / pack_amount;
                    //get next pay day
                    DateTime? nxtPay = instd.Value.AddDays(days2.Value);
                    message = "Sunami inakushukuru kwa malipo yako ya shillingi " + mpesa_amount + ". Umelipia hadi tarehe " + nxtPay.Value.Date.ToString("dd/MM/yyyy");
                }

                else
                {
                    message = "Sunami inakushukuru kwa malipo yako ya shillingi " + mpesa_amount + ". Bado unadaiwa shillingi " + bal + " ya siku zilizopita";
                }
                sendSmsThroughGateway();
            }
            catch { }

        }


        private void sendSmsThroughPhone()
        {
            //string json = JsonConvert.SerializeObject(points, Formatting.Indented);
            json = JsonConvert.SerializeObject(response1, Formatting.Indented);
        }

        public void sendSmsThroughGateway()
        {
            sendSms ss = new sendSms();
            ss.sendSmsThroughGateway(paynumber, message);
        }

    }
}