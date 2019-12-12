using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using EthozCapital.CustomLibraries;
using log4net;

namespace EthozCapital.Models.Tables
{
    public class Sys_ListOfValue
    {
        private static ILog glog = log4net.LogManager.GetLogger(typeof(Sys_ListOfValue));

        [Key]
        public int Id { get; set; }
        [Required]
        public string GroupCode { get; set; }
        public string GroupType { get; set; }
        public string GroupMemberDesc { get; set; }
        public string ParentID { get; set; }
        public string Status { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}