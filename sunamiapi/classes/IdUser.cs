using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class IdUser
    {
        private string _id;
        private string _user;

        public string id { get => _id; set => _id = value; }
        public string user { get => _user; set => _user = value; }
    }
}