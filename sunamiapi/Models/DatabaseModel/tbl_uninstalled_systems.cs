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
    
    public partial class tbl_uninstalled_systems
    {
        public int Id { get; set; }
        public string customer_id { get; set; }
        public Nullable<System.DateTime> uninstall_date { get; set; }
        public string Reason { get; set; }
        public string recorded_by { get; set; }
        public string previousRecords { get; set; }
        public Nullable<int> office_id { get; set; }
    }
}
