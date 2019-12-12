using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
    public class Sys_TypeMatrixFieldProperties
    {
        private static ILog glog = log4net.LogManager.GetLogger(typeof(Sys_TypeMatrixFieldProperties));

        [Key]
        public int Id { get; set; }
        public string MasterPageFieldCode { get; set; }
        public string GroupCode { get; set; }
        public string MatrixGroupTypeCode { get; set; }
        public Nullable<bool> FP1Visible { get; set; }
        public Nullable<bool> FP2Enabled { get; set; }
        public Nullable<bool> FP3DefaultCheck { get; set; }
        public Nullable<bool> FP4Mandatory { get; set; }
        public Nullable<DateTime> EffectiveDate { get; set; }
        public string Status { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}