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
    
    public partial class crmtb_client_entity_profile
    {
        public string ep_client_cod { get; set; }
        public string ep_entity_cod { get; set; }
        public string ep_profiletype_cod { get; set; }
        public string ep_remark { get; set; }
        public string ep_created_by { get; set; }
        public System.DateTime ep_created_dat { get; set; }
        public string ep_sta_ind { get; set; }
        public string ep_sta_who { get; set; }
        public System.DateTime ep_sta_dat { get; set; }
    
        public virtual crmtb_client_mas crmtb_client_mas { get; set; }
    }
}