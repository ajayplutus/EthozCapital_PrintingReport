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
    
    public partial class ss_emp_offday_mas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ss_emp_offday_mas()
        {
            this.ss_emp_offday_det = new HashSet<ss_emp_offday_det>();
        }
    
        public string om_ref_cod { get; set; }
        public string om_emp_cod { get; set; }
        public string om_off_yr { get; set; }
        public string om_day_sta { get; set; }
        public string om_sta_ind { get; set; }
        public string om_sta_who { get; set; }
        public Nullable<System.DateTime> om_sta_dat { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ss_emp_offday_det> ss_emp_offday_det { get; set; }
    }
}
