using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class NameIdResponse
    {
        private string name;
        private string id;

        public string Name { get => name; set => name = value; }
        public string Id { get => id; set => id = value; }
    }
}