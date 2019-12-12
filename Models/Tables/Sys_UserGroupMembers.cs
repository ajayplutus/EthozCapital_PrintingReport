﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using EthozCapital.CustomLibraries;
using log4net;

namespace EthozCapital.Models.Tables
{
    public class Sys_UserGroupMembers
    {
        private static ILog glog = log4net.LogManager.GetLogger(typeof(Sys_UserGroupMembers));

        [Key]
        public int Id { get; set; }

        public string GroupCode { get; set; }
        public string EmployeeCode { get; set; }
        public string Status { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}