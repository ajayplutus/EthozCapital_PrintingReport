﻿using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
    public class Sys_TypeMatrixFunctionLogicMaster
    {
        private static ILog glog = log4net.LogManager.GetLogger(typeof(Sys_TypeMatrixFunctionLogicMaster));

        [Key]
        public int Id { get; set; }
        public string LogicCode { get; set; }
        public string Status { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}