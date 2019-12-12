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
    public class Sys_Users
    {
        private static ILog glog = log4net.LogManager.GetLogger(typeof(Sys_Users)); 

        [Key]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
 
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public string EmployeeCode { get; set; }

        //public static string ErrorFree(Users users)
        //{
        //    glog.Debug("ErrorFree: Entry");
        //    try
        //    {
        //        if (clsCommon.ToStr(users.Email) == "")
        //        {
        //            glog.Error("Invalid UserName.");
        //            return "Invalid UserName.";
        //        }

        //        if (clsCommon.ToStr(users.Password) == "")
        //        {
        //            glog.Error("Invalid Password.");
        //            return "Invalid Password.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        glog.Error(ex.Message);
        //        return ex.Message;
        //    }

        //    glog.Debug("ErrorFree: Exit");
        //    return clsGlobal.ErrorFree_Success;
        //}
    }
}