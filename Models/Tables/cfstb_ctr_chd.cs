using log4net;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace EthozCapital.Models.Tables
{
  public class cfstb_ctr_chd
  {
    private static ILog glog = log4net.LogManager.GetLogger(typeof(cfstb_ctr_chd));
    [Key]
    [Column(Order = 1)]
    public string cc_ctr_num { get; set; }
    [Key]
    [Column(Order = 2)]
    public Int16 cc_con_trl { get; set; }
    public string cc_ven_cod { get; set; }

  }
}