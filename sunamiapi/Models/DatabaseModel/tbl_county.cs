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
    
    public partial class tbl_county
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_county()
        {
            this.tbl_sub_county = new HashSet<tbl_sub_county>();
        }
    
        public int id { get; set; }
        public string county_name { get; set; }
        public int tbl_country_id { get; set; }
    
        public virtual tbl_country tbl_country { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_sub_county> tbl_sub_county { get; set; }
    }
}
