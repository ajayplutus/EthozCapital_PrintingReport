using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
    public class Sys_AutoGenerateIdMaster
    {
        private static ILog glog = log4net.LogManager.GetLogger(typeof(Sys_AutoGenerateIdMaster));
        [Key]
        [Column(Order = 1)]
        public string Group { get; set; }
        [Key]
        [Column(Order = 2)]
        public string Code { get; set; }
        public int ID { get; set; }
        public string Description { get; set; }
        public string PaddingChar { get; set; }
        public string Prefix { get; set; }
        public Byte? Width { get; set; }
        public string YearInd { get; set; }
        public string MonthInd { get; set; }
        public string PrefixSeparator { get; set; }
        public string PrefixPosition { get; set; }
        public string YearSeparator { get; set; }
        public string MonthSeparator { get; set; }
        //public string ExpectedResult { get; set; }
        public string Status { get; set; }
    }
}