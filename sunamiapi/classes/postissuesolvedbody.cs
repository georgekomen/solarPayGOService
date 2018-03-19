using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class postissuesolvedbody
    {
        private int id;
        private string ssolver;
        private string scomment;

        public int Id { get => id; set => id = value; }
        public string Ssolver { get => ssolver; set => ssolver = value; }
        public string Scomment { get => scomment; set => scomment = value; }
    }
}