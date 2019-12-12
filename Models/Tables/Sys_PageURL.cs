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
    public class Sys_PageURL
	{
        private static ILog glog = log4net.LogManager.GetLogger(typeof(Sys_Users)); 

        [Key]
        public int Id { get; set; }

        public string PageURLCode { get; set; }
 
        public string URL { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }
    }
}