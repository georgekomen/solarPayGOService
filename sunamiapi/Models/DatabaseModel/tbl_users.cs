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
    
    public partial class tbl_users
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_users()
        {
            this.tbl_grants = new HashSet<tbl_grants>();
        }
    
        public int Id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string level { get; set; }
        public Nullable<bool> allow { get; set; }
        public Nullable<System.DateTime> time { get; set; }
        public string token { get; set; }
        public Nullable<int> tbl_office_id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_grants> tbl_grants { get; set; }
        public virtual tbl_office tbl_office { get; set; }
    }
}
