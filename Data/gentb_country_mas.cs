//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EthozCapital.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class gentb_country_mas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public gentb_country_mas()
        {
            this.crmtb_client_address_mas = new HashSet<crmtb_client_address_mas>();
        }
    
        public string cm_country_cod { get; set; }
        public string cm_country_nam { get; set; }
        public string cm_sht_nam { get; set; }
        public string cm_nationality { get; set; }
        public string cm_idd_cod { get; set; }
        public string cm_cur_des { get; set; }
        public string cm_cur_cod { get; set; }
        public string cm_cur_sign { get; set; }
        public string cm_created_by { get; set; }
        public System.DateTime cm_created_dat { get; set; }
        public string cm_sta_ind { get; set; }
        public string cm_sta_who { get; set; }
        public System.DateTime cm_sta_dat { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<crmtb_client_address_mas> crmtb_client_address_mas { get; set; }
    }
}