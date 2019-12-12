using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
    public class PreContract_RecourseGuarantor
    {
        private static ILog glog = log4net.LogManager.GetLogger(typeof(PreContract_RecourseGuarantor));
        [Key, Column(Order = 0)]
        public string RefNumber { get; set; }
        public string ContractNumber { get; set; }
        //[Key, Column(Order = 1)]
        public int RolloverNumber { get; set; }
        [Key, Column(Order = 1)]
        public int ItemNumber { get; set; }
        public string GuarantorType { get; set; }
        public string Guarantor { get; set; }
        public string GuarantorAddress { get; set; }
        public string GuarantorDept { get; set; }
        public string GuarantorConPerson { get; set; }
        public string LetterType { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
    }
}