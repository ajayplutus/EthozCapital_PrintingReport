﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ORIX_ESS_DB_DevEntities : DbContext
    {
        public ORIX_ESS_DB_DevEntities()
            : base("name=ORIX_ESS_DB_DevEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ss_abs_det> ss_abs_det { get; set; }
        public virtual DbSet<ss_abs_mas> ss_abs_mas { get; set; }
        public virtual DbSet<ss_app_officer> ss_app_officer { get; set; }
        public virtual DbSet<ss_att_access> ss_att_access { get; set; }
        public virtual DbSet<ss_att_aws> ss_att_aws { get; set; }
        public virtual DbSet<ss_att_deduct> ss_att_deduct { get; set; }
        public virtual DbSet<ss_att_rem_ark> ss_att_rem_ark { get; set; }
        public virtual DbSet<ss_att_rem_ark2> ss_att_rem_ark2 { get; set; }
        public virtual DbSet<ss_att_sum_mary> ss_att_sum_mary { get; set; }
        public virtual DbSet<ss_buddy> ss_buddy { get; set; }
        public virtual DbSet<ss_ccl_param> ss_ccl_param { get; set; }
        public virtual DbSet<ss_clm_det> ss_clm_det { get; set; }
        public virtual DbSet<ss_clm_emp_mas> ss_clm_emp_mas { get; set; }
        public virtual DbSet<ss_clm_ent_mas> ss_clm_ent_mas { get; set; }
        public virtual DbSet<ss_clm_mas> ss_clm_mas { get; set; }
        public virtual DbSet<ss_clm_typ> ss_clm_typ { get; set; }
        public virtual DbSet<ss_date_release> ss_date_release { get; set; }
        public virtual DbSet<ss_dept_det> ss_dept_det { get; set; }
        public virtual DbSet<ss_ecard_mails> ss_ecard_mails { get; set; }
        public virtual DbSet<ss_email_alert> ss_email_alert { get; set; }
        public virtual DbSet<ss_emp_address_book> ss_emp_address_book { get; set; }
        public virtual DbSet<ss_emp_appoint_ment> ss_emp_appoint_ment { get; set; }
        public virtual DbSet<ss_emp_category> ss_emp_category { get; set; }
        public virtual DbSet<ss_emp_grp_det> ss_emp_grp_det { get; set; }
        public virtual DbSet<ss_emp_grp_mas> ss_emp_grp_mas { get; set; }
        public virtual DbSet<ss_emp_mas> ss_emp_mas { get; set; }
        public virtual DbSet<ss_emp_mas_add> ss_emp_mas_add { get; set; }
        public virtual DbSet<ss_emp_offday_det> ss_emp_offday_det { get; set; }
        public virtual DbSet<ss_emp_offday_mas> ss_emp_offday_mas { get; set; }
        public virtual DbSet<ss_emp_pic> ss_emp_pic { get; set; }
        public virtual DbSet<ss_exp_lev> ss_exp_lev { get; set; }
        public virtual DbSet<ss_gentb_mails> ss_gentb_mails { get; set; }
        public virtual DbSet<ss_lev_det> ss_lev_det { get; set; }
        public virtual DbSet<ss_lev_det_log> ss_lev_det_log { get; set; }
        public virtual DbSet<ss_lev_emp_mas> ss_lev_emp_mas { get; set; }
        public virtual DbSet<ss_lev_ent_mas> ss_lev_ent_mas { get; set; }
        public virtual DbSet<ss_lev_mas> ss_lev_mas { get; set; }
        public virtual DbSet<ss_lev_typ> ss_lev_typ { get; set; }
        public virtual DbSet<ss_mail_cclist> ss_mail_cclist { get; set; }
        public virtual DbSet<ss_main_menus> ss_main_menus { get; set; }
        public virtual DbSet<ss_ot_det> ss_ot_det { get; set; }
        public virtual DbSet<ss_ot_mas> ss_ot_mas { get; set; }
        public virtual DbSet<ss_par_ams> ss_par_ams { get; set; }
        public virtual DbSet<ss_para_meter> ss_para_meter { get; set; }
        public virtual DbSet<ss_rep_officer> ss_rep_officer { get; set; }
        public virtual DbSet<ss_req_mas> ss_req_mas { get; set; }
        public virtual DbSet<ss_sop_dept_mas> ss_sop_dept_mas { get; set; }
        public virtual DbSet<ss_sop_file_mas> ss_sop_file_mas { get; set; }
        public virtual DbSet<ss_sub_menus> ss_sub_menus { get; set; }
        public virtual DbSet<ss_swap_mas> ss_swap_mas { get; set; }
        public virtual DbSet<ss_usr_access> ss_usr_access { get; set; }
        public virtual DbSet<ss_vacant_mas> ss_vacant_mas { get; set; }
        public virtual DbSet<ss_ver_officer> ss_ver_officer { get; set; }
        public virtual DbSet<ss_yr_period> ss_yr_period { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<ss_att_det> ss_att_det { get; set; }
        public virtual DbSet<ss_clm_emp_det> ss_clm_emp_det { get; set; }
        public virtual DbSet<ss_dept_chd> ss_dept_chd { get; set; }
        public virtual DbSet<ss_lev_emp_det> ss_lev_emp_det { get; set; }
        public virtual DbSet<ss_lev_emp_det_his> ss_lev_emp_det_his { get; set; }
        public virtual DbSet<ss_lev_emp_det_temp> ss_lev_emp_det_temp { get; set; }
    }
}
