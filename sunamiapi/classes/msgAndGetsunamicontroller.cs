using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class msgAndGetsunamicontroller
    {
        private string _message;
        private List<getsunamicontroller> _content;

        public string message { get => _message; set => _message = value; }
        public List<getsunamicontroller> content { get => _content; set => _content = value; }
    }
}