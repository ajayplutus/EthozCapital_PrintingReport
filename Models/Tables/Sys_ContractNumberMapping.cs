using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
    public class Sys_ContractNumberMapping
    {
        private static ILog glog = log4net.LogManager.GetLogger(typeof(Sys_ContractNumberMapping));
        [Key]
        [Column(Order = 1)]
        public string SubContractTypeCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ContractStatus { get; set; }

        public int ID { get; set; }
        public int ItemNumber { get; set; }
        public string AutoGenerateIdMasterCode { get; set; }
        public string Var1_SubProductType { get; set; }
        public string Var1_AutoGenerateIdMasterCode { get; set; }
        public string Var2_LEFSRateType { get; set; }
        public string Var2_AutoGenerateIdMasterCode { get; set; }
        public string Status { get; set; }
    }
}