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
    
    public partial class cfstb_asset_brand_mas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public cfstb_asset_brand_mas()
        {
            this.cfstb_asset_model_chd = new HashSet<cfstb_asset_model_chd>();
        }
    
        public string cfs_brand_code { get; set; }
        public string cfs_brand_name { get; set; }
        public string cfs_brand_sta_ind { get; set; }
        public string cfs_brand_sta_who { get; set; }
        public System.DateTime cfs_brand_sta_dat { get; set; }
        public string cfs_brand_vehicle_ind { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cfstb_asset_model_chd> cfstb_asset_model_chd { get; set; }
    }
}
