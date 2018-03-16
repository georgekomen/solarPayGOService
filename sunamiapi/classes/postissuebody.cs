using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class postissuebody
    {
        private string _id;
        private string _issue;
        private string _reporter;
        private string _priority;

        public string id { get => _id; set => _id = value; }
        public string issue { get => _issue; set => _issue = value; }
        public string reporter { get => _reporter; set => _reporter = value; }
        public string priority { get => _priority; set => _priority = value; }
    }
}