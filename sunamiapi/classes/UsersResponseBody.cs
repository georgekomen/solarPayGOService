using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class UsersResponseBody
    {
        private string name;
        private string _email;

        public string Name { get => name; set => name = value; }
        public string email { get => _email; set => _email = value; }
    }
}