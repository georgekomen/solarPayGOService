using sunamiapi.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class registerCustomer
    {
        private string confirm = "";
        private DateTime install_date;
        private string id;
        private string latG;
        private string lonG;
        private string package;
        private string country;
        private string name;
        private string number;
        private string number2;
        private string number3;
        private string village;
        private string location;
        private string city;
        private string occupation;
        private string box;
        private string witness;
        private string witnessid;
        private string witness_mobile;
        private string agentcode;
        private string description;
        private string recordedBy;
        private string gender;

        public string Confirm { get => confirm; set => confirm = value; }
        public DateTime Install_date { get => install_date; set => install_date = value; }
        public string Id { get => id; set => id = value; }
        public string LatG { get => latG; set => latG = value; }
        public string LonG { get => lonG; set => lonG = value; }
        public string Package { get => package; set => package = value; }
        public string Country { get => country; set => country = value; }
        public string Name { get => name; set => name = value; }
        public string Number { get => number; set => number = value; }
        public string Number2 { get => number2; set => number2 = value; }
        public string Number3 { get => number3; set => number3 = value; }
        public string Village { get => village; set => village = value; }
        public string Location { get => location; set => location = value; }
        public string City { get => city; set => city = value; }
        public string Occupation { get => occupation; set => occupation = value; }
        public string Box { get => box; set => box = value; }
        public string Witness { get => witness; set => witness = value; }
        public string Witnessid { get => witnessid; set => witnessid = value; }
        public string Witness_mobile { get => witness_mobile; set => witness_mobile = value; }
        public string Agentcode { get => agentcode; set => agentcode = value; }
        public string Description { get => description; set => description = value; }
        public string RecordedBy { get => recordedBy; set => recordedBy = value; }
        public string Gender { get => gender; set => gender = value; }

        public void record(db_a0a592_sunamiEntities se)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrEmpty(id))
            {
                confirm += "Error! Id number not entered";
                return;
            }
            try
            {
                //CHECK IF CUSTOMER EXISTS IN TBL_CUSTOMER
                if (se.tbl_customer.Select(o => o.customer_id).Contains(id))
                {
                    //IF HE EXISTS - UPDATE ENTRY
                    tbl_customer tc = se.tbl_customer.FirstOrDefault(i => i.customer_id == id);
                    if (package != "" && package != null)
                    {
                        tc.package_type = package;
                    }
                    if (install_date != null)
                    {
                        tc.install_date = install_date;
                    }
                    if (id != "" && id != null)
                    {
                        tc.customer_id = id;
                    }
                    if (country != "" && country != null)
                    {
                        tc.country = country;
                    }
                    if (name != "" && name != null)
                    {
                        tc.customer_name = name;
                    }
                    if (village != "" && village != null)
                    {
                        tc.village_name = village;
                    }
                    if (location != "" && location != null)
                    {
                        tc.location = location;
                    }
                    if (city != "" && city != null)
                    {
                        tc.city = city;
                    }
                    if (occupation != "" && occupation != null)
                    {
                        tc.occupation = occupation;
                    }
                    if (box != "" && box != null)
                    {
                        tc.Po_Box_Address = box;
                    }
                    if (witness != "" && witness != null)
                    {
                        tc.next_of_kin = witness;
                    }
                    if (witness_mobile != "" && witness_mobile != null)
                    {
                        tc.witness_mobile_number = witness_mobile;
                    }
                    if (agentcode != "" && agentcode != null)
                    {
                        tc.agentcode = agentcode;
                    }
                    if (witnessid != "" && witnessid != null)
                    {
                        tc.nok_mobile = witnessid;
                    }
                    if (latG != "" && latG != null)
                    {
                        tc.lon = lonG;
                        tc.lat = latG;
                    }
                    if (!string.IsNullOrWhiteSpace(recordedBy) || !string.IsNullOrEmpty(recordedBy))
                    {
                        tc.RecordedBy += "Modified By" + recordedBy;
                    }
                    if (!string.IsNullOrWhiteSpace(gender) || !string.IsNullOrEmpty(gender))
                    {
                        tc.gender = gender;
                    }
                    if (!string.IsNullOrWhiteSpace(description) || !string.IsNullOrEmpty(description))
                    {
                        tc.Description = description;
                    }
                    if (number != "" && number != null)
                    {
                        //check if number exixts in db
                        if (!se.tbl_customer.Select(g1=>g1.phone_numbers).Contains(number) && !se.tbl_customer.Select(g2 => g2.phone_numbers2).Contains(number) && !se.tbl_customer.Select(g3 => g3.phone_numbers3).Contains(number))
                        {
                            tc.phone_numbers = number;
                        }
                        else
                        {
                            //number exists in db
                        }
                    }
                    if (number2 != "" && number2 != null)
                    {
                        //check if number exixts in db
                        if (!se.tbl_customer.Select(g1 => g1.phone_numbers).Contains(number2) && !se.tbl_customer.Select(g2 => g2.phone_numbers2).Contains(number2) && !se.tbl_customer.Select(g3 => g3.phone_numbers3).Contains(number2))
                        {
                            tc.phone_numbers2 = number2;
                        }
                        else
                        {
                            //number exists in db
                        }
                    }
                    if (number3 != "" && number3 != null)
                    {
                        //check if number exixts in db
                        if (!se.tbl_customer.Select(g1 => g1.phone_numbers).Contains(number3) && !se.tbl_customer.Select(g2 => g2.phone_numbers2).Contains(number3) && !se.tbl_customer.Select(g3 => g3.phone_numbers3).Contains(number3))
                        {
                            tc.phone_numbers3 = number3;
                        }
                        else
                        {
                            //number exists in db
                        }
                    }
                    se.SaveChanges();
                    confirm += "modified an existing customer data";
                }
                else
                {
                    // IF CUSTOMER DOESN'T EXISTS MAKE ENTRY
                    tbl_customer tc = new tbl_customer();
                    tc.customer_id = id;
                    tc.package_type = package;
                    tc.country = country;
                    tc.customer_name = name;                   
                    tc.village_name = village;
                    tc.location = location;
                    tc.city = city;
                    tc.occupation = occupation;
                    tc.Po_Box_Address = box;
                    tc.next_of_kin = witness;
                    tc.nok_mobile = witnessid;
                    tc.active_status = true;
                    tc.lon = lonG;
                    tc.lat = latG;
                    tc.Description = description;
                    tc.RecordedBy = recordedBy;
                    tc.agentcode = agentcode;
                    tc.gender = gender;
                    tc.witness_mobile_number = witness_mobile;
                    tc.install_date = install_date;
                    
                    //check if number exixts in db
                    if (!se.tbl_customer.Select(g1 => g1.phone_numbers).Contains(number) && !se.tbl_customer.Select(g2 => g2.phone_numbers2).Contains(number) && !se.tbl_customer.Select(g3 => g3.phone_numbers3).Contains(number))
                    {
                        tc.phone_numbers = number;
                    }
                    else
                    {
                        //number exists in db
                    }
                    //check if number exixts in db
                    if (!se.tbl_customer.Select(g1 => g1.phone_numbers).Contains(number2) && !se.tbl_customer.Select(g2 => g2.phone_numbers2).Contains(number2) && !se.tbl_customer.Select(g3 => g3.phone_numbers3).Contains(number2))
                    {
                        tc.phone_numbers2 = number2;
                    }
                    else
                    {
                        //number exists in db
                    }
                    //check if number exixts in db
                    if (!se.tbl_customer.Select(g1 => g1.phone_numbers).Contains(number3) && !se.tbl_customer.Select(g2 => g2.phone_numbers2).Contains(number3) && !se.tbl_customer.Select(g3 => g3.phone_numbers3).Contains(number3))
                    {
                        tc.phone_numbers3 = number3;
                    }
                    else
                    {
                        //number exists in db
                    }
                    se.tbl_customer.Add(tc);
                    se.SaveChanges();
                    confirm += "registered new customer";
                }

                if (!se.tbl_extra_package_customers.Where(r => r.customer_id == id).Select(t => t.item).Contains(package))
                {
                    tbl_extra_package_customers epc = new tbl_extra_package_customers();
                    epc.customer_id = id;
                    epc.item = package;
                    epc.agentcode = agentcode;
                    epc.date_given = install_date;
                    se.tbl_extra_package_customers.Add(epc);
                    se.SaveChanges();
                    // this.logevent(rc.RecordedBy, rc.Id, DateTime.Today, "Invoiced customer a " + item, "Invoice Customer");
                }
                else
                {
                    tbl_extra_package_customers tepc = se.tbl_extra_package_customers.FirstOrDefault(f => f.customer_id == id && f.item == package);
                    tepc.date_given = install_date;
                    se.SaveChanges();
                }
                
            }
            catch (Exception kk)
            {
                throw new Exception(kk.StackTrace);
            }
        }
    }
}