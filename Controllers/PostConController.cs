using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EthozCapital.Models;
using System.Security.Claims;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using log4net;
using Newtonsoft.Json;
using EthozCapital.CustomLibraries;

namespace EthozCapital.Controllers
{
    public class PostConController : Controller
    {
        private static ILog glog = log4net.LogManager.GetLogger(typeof(PostConController));

        public ActionResult PostConNew(string CTGroupCode, string SubConGroupCode)
        {
            return View();
        }
    }
}