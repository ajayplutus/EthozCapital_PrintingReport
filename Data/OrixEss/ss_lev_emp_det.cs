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
    
    public partial class ss_lev_emp_det
    {
        public string sd_ref_cod { get; set; }
        public string sd_lev_cod { get; set; }
        public Nullable<decimal> sd_lev_bf { get; set; }
        public Nullable<decimal> sd_lev_ent { get; set; }
        public Nullable<decimal> sd_cur_ent { get; set; }
        public Nullable<decimal> sd_lev_add { get; set; }
        public Nullable<decimal> sd_lev_tot { get; set; }
        public Nullable<decimal> sd_lev_tak { get; set; }
        public Nullable<decimal> sd_lev_bal { get; set; }
        public Nullable<int> sd_hd_max { get; set; }
        public string sd_sys_sta { get; set; }
        public string sd_prd_cod { get; set; }
        public int recordno { get; set; }
    
        public virtual ss_lev_emp_mas ss_lev_emp_mas { get; set; }
    }
}
