using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
  public class Cfstb_serial_num
  {
    private static ILog glog = log4net.LogManager.GetLogger(typeof(Cfstb_serial_num));
    [Key]
    [Column(Order = 1)]
    public string cs_ctr_num { get; set; }
    [Key]
    [Column(Order = 2)]
    public Int16 cs_itm_num { get; set; }
    public Int16 cs_con_trl { get; set; }
    public string cs_ser_num { get; set; }
    public string cs_remarks { get; set; }
    public Nullable<char> cs_cost_ctr { get; set; }
    public string cs_reason { get; set; }
    [Key]
    [Column(Order = 3)]
    public char cs_ctr_roll { get; set; }
    public string cs_eng_no { get; set; }
    public string cs_chs_no { get; set; }
    public char cs_year_man { get; set; }
    public string cs_color { get; set; }
    public string cs_eng_rea { get; set; }
    public string cs_chs_rea { get; set; }
    public string cs_dep_nam { get; set; }
    public string cs_address { get; set; }
    public string cs_model_typ { get; set; }
    public char cs_indct_ind { get; set; }
    public Nullable<DateTime> cs_firtag_dat { get; set; }
    public char cs_fre_tag { get; set; }

  }
}