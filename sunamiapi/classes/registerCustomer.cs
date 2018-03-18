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
        db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
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
        private int agentcode;
        private string description;
        private string recordedBy;

        //confirms created new 
        public string Confirm
        {
            get
            {
                return confirm;
            }

        }


        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public string LatG
        {
            get
            {
                return latG;
            }

            set
            {
                latG = value;
            }
        }

        public string LonG
        {
            get
            {
                return lonG;
            }

            set
            {
                lonG = value;
            }
        }

        public string Package
        {
            get
            {
                return package;
            }

            set
            {
                package = value;
            }
        }
        

        public string Country
        {
            get
            {
                return country;
            }

            set
            {
                country = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string Number
        {
            get
            {
                return number;
            }

            set
            {
                number = value;
            }
        }

        public string Village
        {
            get
            {
                return village;
            }

            set
            {
                village = value;
            }
        }

        public string Location
        {
            get
            {
                return location;
            }

            set
            {
                location = value;
            }
        }

        public string City
        {
            get
            {
                return city;
            }

            set
            {
                city = value;
            }
        }

        public string Occupation
        {
            get
            {
                return occupation;
            }

            set
            {
                occupation = value;
            }
        }

        public string Box
        {
            get
            {
                return box;
            }

            set
            {
                box = value;
            }
        }

        public string Witness
        {
            get
            {
                return witness;
            }

            set
            {
                witness = value;
            }
        }

        public string Witnessid
        {
            get
            {
                return witnessid;
            }

            set
            {
                witnessid = value;
            }
        }

        public string Number2
        {
            get
            {
                return number2;
            }

            set
            {
                number2 = value;
            }
        }

        public string Number3
        {
            get
            {
                return number3;
            }

            set
            {
                number3 = value;
            }
        }

        public DateTime Install_date
        {
            get
            {
                return install_date;
            }

            set
            {
                install_date = value;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }

        public string RecordedBy
        {
            get
            {
                return recordedBy;
            }

            set
            {
                recordedBy = value;
            }
        }

        public string Witness_mobile { get => witness_mobile; set => witness_mobile = value; }
        public int Agentcode { get => agentcode; set => agentcode = value; }

        public void record()
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrEmpty(id))
            {
                confirm += "Error! Id number not entered";
                return;
            }
            try
            {
                //CHECK IF CUSTOMER EXISTS IN TBL_CUSTOMER
                int g = se.tbl_customer.Where(o => o.customer_id == id).Count();
                if (g >= 1)
                {
                    //IF HE EXISTS - UPDATE ENTRY
                    tbl_customer tc = se.tbl_customer.FirstOrDefault(i => i.customer_id == id);
                    if (package != "" && package != null)
                    {
                        tc.package_type = package;
                    }
                    if (Install_date != null)
                    {
                        
                        tc.install_date = Install_date;
                    }
                    if (Id != "" && Id != null)
                    {
                        tc.customer_id = Id;
                    }
                    if (Country != "" && Country != null)
                    {
                        tc.country = Country;
                    }
                    if (Name != "" && Name != null)
                    {
                        tc.customer_name = Name;
                    }
                    
                    if (Village != "" && Village != null)
                    {
                        tc.village_name = Village;
                    }
                    if (Location != "" && Location != null)
                    {
                        tc.location = Location;
                    }
                    if (City != "" && City != null)
                    {
                        tc.city = City;
                    }
                    if (Occupation != "" && Occupation != null)
                    {
                        tc.occupation = Occupation;
                    }
                    if (Box != "" && Box != null)
                    {
                        tc.Po_Box_Address = Box;
                    }
                    if (Witness != "" && Witness != null)
                    {
                        tc.next_of_kin = Witness;
                    }

                    if (Witness_mobile != "" && Witness_mobile != null)
                    {
                        tc.witness_mobile_number = Witness_mobile;
                    }
                    if (Agentcode != 0 && Agentcode != null)
                    {
                        tc.agentcode = Agentcode;
                    }

                    if (Witnessid != "" && Witnessid != null)
                    {
                        tc.nok_mobile = Witnessid;
                    }
                    if (latG != "" && latG != null)
                    {
                        tc.lon = LonG;
                        tc.lat = LatG;
                    }
                    if (!string.IsNullOrWhiteSpace(recordedBy) || !string.IsNullOrEmpty(recordedBy))
                    {
                        tc.RecordedBy += "Modified By" + RecordedBy;
                    }
                    if (!string.IsNullOrWhiteSpace(description) || !string.IsNullOrEmpty(description))
                    {
                        tc.Description = Description;
                    }
                    if (Number != "" && Number != null)
                    {
                        //check if number exixts in db
                        int cn = se.tbl_customer.Where(g1 => g1.phone_numbers == Number || g1.phone_numbers2 == Number || g1.phone_numbers3 == Number).Count();
                        if (cn == 0)
                        {
                            tc.phone_numbers = Number;
                        }
                        else
                        {
                            //number exists in db
                        }
                    }
                    if (Number2 != "" && Number2 != null)
                    {
                        //check if number exixts in db
                        int cn = se.tbl_customer.Where(g1 => g1.phone_numbers == Number2 || g1.phone_numbers2 == Number2 || g1.phone_numbers3 == Number2).Count();
                        if (cn == 0)
                        {
                            tc.phone_numbers2 = Number2;
                        }
                        else
                        {
                            //number exists in db
                        }

                    }
                    if (Number3 != "" && Number3 != null)
                    {
                        //check if number exixts in db
                        int cn = se.tbl_customer.Where(g1 => g1.phone_numbers == Number3 || g1.phone_numbers2 == Number3 || g1.phone_numbers3 == Number3).Count();
                        if (cn == 0)
                        {
                            tc.phone_numbers3 = Number3;
                        }
                        else
                        {
                            //number exists in db
                        }

                    }
                    se.SaveChanges();
                    confirm += "modified an existing customer data";

                }
                else if (g < 1)
                {
                    // IF CUSTOMER DOESN'T EXISTS MAKE ENTRY
                    tbl_customer tc = new tbl_customer();
                    tc.customer_id = Id;
                    tc.package_type = package;
                    tc.country = Country;
                    tc.customer_name = Name;                   
                    tc.village_name = Village;
                    try
                    {
                        tc.location = Location;
                    }
                    catch { }
                    tc.city = City;
                    tc.occupation = Occupation;
                    tc.Po_Box_Address = Box;
                    tc.next_of_kin = Witness;
                    tc.nok_mobile = Witnessid;
                    tc.active_status = true;
                    tc.lon = LonG;
                    tc.lat = LatG;
                    tc.Description = Description;
                    tc.RecordedBy = RecordedBy;
                    try
                    {
                        tc.install_date = Install_date;
                    }
                    catch { }
                    int cn = se.tbl_customer.Where(g1 => g1.phone_numbers == Number || g1.phone_numbers2 == Number || g1.phone_numbers3 == Number).Count();
                    if (cn == 0)
                    {
                        tc.phone_numbers = Number;
                    }
                    else
                    {
                        //number exists in db
                    }

                    int cn1 = se.tbl_customer.Where(g1 => g1.phone_numbers == Number2 || g1.phone_numbers2 == Number2 || g1.phone_numbers3 == Number2).Count();
                    if (cn1 == 0)
                    {
                        tc.phone_numbers2 = Number2;
                    }
                    else
                    {
                        //number exists in db
                    }
                    int cn2 = se.tbl_customer.Where(g1 => g1.phone_numbers == Number3 || g1.phone_numbers2 == Number3 || g1.phone_numbers3 == Number3).Count();
                    if (cn2 == 0)
                    {
                        tc.phone_numbers3 = Number3;
                    }
                    else
                    {
                        //number exists in db
                    }
                    se.tbl_customer.Add(tc);
                    se.SaveChanges();
                    confirm += "registered new customer";
                }
            }
            catch (Exception kk)
            {
                confirm += "an error occured while trying to register customer. Please contact the system admin";
            }
        }
    }
}