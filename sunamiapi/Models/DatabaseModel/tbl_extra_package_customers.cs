//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace sunamiapi.Models.DatabaseModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_extra_package_customers
    {
        public int Id { get; set; }
        public string customer_id { get; set; }
        public string item { get; set; }
        public System.DateTime date_given { get; set; }
        public Nullable<System.DateTime> date_taken { get; set; }
        public string agentcode { get; set; }
    }
}
