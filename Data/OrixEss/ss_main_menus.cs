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
    
    public partial class ss_main_menus
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ss_main_menus()
        {
            this.ss_sub_menus = new HashSet<ss_sub_menus>();
        }
    
        public int mm_mnu_id { get; set; }
        public string mm_mnu_des { get; set; }
        public string mm_mnu_val { get; set; }
        public Nullable<int> mm_mnu_ord { get; set; }
        public string mm_mnu_lnk { get; set; }
        public string mm_mnu_img { get; set; }
        public string mm_sta_ind { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ss_sub_menus> ss_sub_menus { get; set; }
    }
}
