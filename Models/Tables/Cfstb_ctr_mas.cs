using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
  public class Cfstb_ctr_mas
  {
    private static ILog glog = log4net.LogManager.GetLogger(typeof(Cfstb_ctr_mas));

    [Key]
    public string Cm_ctr_num { get; set; }
  }
}