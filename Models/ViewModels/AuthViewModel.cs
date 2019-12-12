using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EthozCapital.CustomLibraries;
using System.Diagnostics;
using log4net;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace EthozCapital.Models.ViewModels
{
    public class AuthViewModel
    {        
        private static clsGlobal _clsGlobal;
        private static ILog glog = log4net.LogManager.GetLogger(typeof(AuthViewModel));

        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]

        public string pstrMenu { get; set; }

        public static AuthViewModel ReturnNavigationPartial(string userGroupCode)
        {
            AuthViewModel mm = new AuthViewModel();
            _clsGlobal = new clsGlobal();  
            try
            {
                DataTable dt = new DataTable();
                dt = _clsGlobal.GetMenu(userGroupCode, System.Configuration.ConfigurationManager.AppSettings["VirtualDirectory"]);

                if (dt.Rows.Count > 0)
                {
                    mm.pstrMenu = dt.Rows[0]["FinalCode"].ToString();
                }
                else
                {
                    mm.pstrMenu = "";
                }
            }
            catch (Exception ex)
            {
                glog.Error(ex.Message);
            }
            return mm;
        }

        public string Value { get; set; }
    }
}