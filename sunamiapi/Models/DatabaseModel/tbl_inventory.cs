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
    
    public partial class tbl_inventory
    {
        public int Id { get; set; }
        public string Item { get; set; }
        public Nullable<int> stock { get; set; }
        public System.DateTime date { get; set; }
        public string units { get; set; }
        public string comments { get; set; }
        public Nullable<int> office_id { get; set; }
    }
}
