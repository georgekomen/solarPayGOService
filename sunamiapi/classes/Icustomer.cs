using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class Icustomer
    {
        private string customer_Name;
        private string customer_Id;
        private float customer_Lat;
        private float customer_Lon;
        private bool? customer_Status;
        private string customer_Icon;

        public string Customer_Name { get => customer_Name; set => customer_Name = value; }
        public string Customer_Id { get => customer_Id; set => customer_Id = value; }
        public float Customer_Lat { get => customer_Lat; set => customer_Lat = value; }
        public float Customer_Lon { get => customer_Lon; set => customer_Lon = value; }
        public bool? Customer_Status { get => customer_Status; set => customer_Status = value; }
        public string Customer_Icon { get => customer_Icon; set => customer_Icon = value; }
    }
}