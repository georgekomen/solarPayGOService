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
    
    public partial class tbl_switch_logs
    {
        public int Id { get; set; }
        public string customer_id { get; set; }
        public string switch_off_payrate { get; set; }
        public Nullable<System.DateTime> switch_off_date { get; set; }
        public Nullable<System.DateTime> switch_on_date { get; set; }
        public string switch_on_payrate { get; set; }
        public string switched_off_by { get; set; }
        public string switched_on_by { get; set; }
        public Nullable<int> office_id { get; set; }
    }
}
