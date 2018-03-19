using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class postrecordstockitem
    {
        private string _loogeduser;
        private string _itemName;
        private string stock;
        private string _units;

        public string loogeduser { get => _loogeduser; set => _loogeduser = value; }
        public string itemName { get => _itemName; set => _itemName = value; }
        public string Stock { get => stock; set => stock = value; }
        public string units { get => _units; set => _units = value; }
    }
}