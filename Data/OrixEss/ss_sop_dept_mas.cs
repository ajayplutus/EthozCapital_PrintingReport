//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EthozCapital.Data.OrixEss
{
    using System;
    using System.Collections.Generic;
    
    public partial class ss_sop_dept_mas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ss_sop_dept_mas()
        {
            this.ss_sop_file_mas = new HashSet<ss_sop_file_mas>();
        }
    
        public string dm_dept_cod { get; set; }
        public string dm_file_path { get; set; }
        public string dm_sta_ind { get; set; }
        public string dm_sta_who { get; set; }
        public Nullable<System.DateTime> dm_sta_dat { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ss_sop_file_mas> ss_sop_file_mas { get; set; }
    }
}