using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.classes
{
    public class chartsdates
    {
        private DateTime bg;
        private DateTime en;

        public DateTime Bg
        {
            get
            {
                return bg;
            }

            set
            {
                bg = value;
            }
        }

        public DateTime En
        {
            get
            {
                return en;
            }

            set
            {
                en = value;
            }
        }
    }
}