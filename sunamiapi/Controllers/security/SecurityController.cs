using sunamiapi.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace sunamiapi.Controllers.security
{
    public class SecurityController : ApiController
    {
        db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
        
        [HttpPost]
        public void addCountry([FromBody]tbl_country value)
        {
            se.tbl_country.Add(value);
            se.SaveChanges();
        }

        [HttpPost]
        public void addCounty([FromBody]tbl_county value)
        {
            se.tbl_county.Add(value);
            se.SaveChanges();
        }

        [HttpPost]
        public void addSubcounty([FromBody]tbl_sub_county value)
        {
            se.tbl_sub_county.Add(value);
            se.SaveChanges();
        }
        
        [HttpPost]
        public void addOffice([FromBody]tbl_office value)
        {
            se.tbl_office.Add(value);
            se.SaveChanges();
        }

        [HttpGet]
        public List<tbl_country> getCountries()
        {
            return se.tbl_country.ToList();
        }

        [HttpGet]
        public List<tbl_county> getCounties()
        {
            return se.tbl_county.ToList();
        }

        [HttpGet]
        public List<tbl_sub_county> getSubCounties()
        {
            return se.tbl_sub_county.ToList();
        }

        [HttpGet]
        public List<tbl_office> getOffices()
        {
            return se.tbl_office.ToList();
        }
    }
}
