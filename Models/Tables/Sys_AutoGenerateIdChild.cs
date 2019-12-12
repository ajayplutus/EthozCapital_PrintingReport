using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
    public class Sys_AutoGenerateIdChild
    {
        private static ILog glog = log4net.LogManager.GetLogger(typeof(Sys_AutoGenerateIdChild));
        public int ID { get; set; }
        //public int MasterID { get; set; }
        public string MasterCode { get; set; }
        public string YY { get; set; }
        public string MM { get; set; }
        public string LastNumber { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
  }
}