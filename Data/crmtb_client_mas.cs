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
    
    public partial class crmtb_client_mas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public crmtb_client_mas()
        {
            this.crmtb_client_alert_mas = new HashSet<crmtb_client_alert_mas>();
            this.crmtb_client_entity_profile = new HashSet<crmtb_client_entity_profile>();
            this.crmtb_client_id_mas = new HashSet<crmtb_client_id_mas>();
            this.crmtb_client_address_mas = new HashSet<crmtb_client_address_mas>();
        }
    
        public string cm_client_cod { get; set; }
        public string cm_client_nam { get; set; }
        public string cm_typ_cod { get; set; }
        public string cm_client_rate { get; set; }
        public string cm_spl_sta { get; set; }
        public string cm_rem_ark { get; set; }
        public string cm_client_his { get; set; }
        public string cm_created_by { get; set; }
        public System.DateTime cm_created_dat { get; set; }
        public string cm_sta_ind { get; set; }
        public string cm_sta_who { get; set; }
        public System.DateTime cm_sta_dat { get; set; }
        public string cm_sou_cod { get; set; }
        public string cm_gl_typ_cod { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<crmtb_client_alert_mas> crmtb_client_alert_mas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<crmtb_client_entity_profile> crmtb_client_entity_profile { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<crmtb_client_id_mas> crmtb_client_id_mas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<crmtb_client_address_mas> crmtb_client_address_mas { get; set; }
    }
}
