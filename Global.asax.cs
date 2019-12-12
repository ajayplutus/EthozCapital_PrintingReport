using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EthozCapital.App_Start;
using System.Data.Entity;
using System.Security.Claims;
using System.Web.Helpers;
using log4net;
using System.Web.Optimization;

namespace EthozCapital
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static ILog glog = log4net.LogManager.GetLogger(typeof(MvcApplication));

        protected void Application_Start()
        {
            Database.SetInitializer<MainDbContext>(null);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            log4net.Config.XmlConfigurator.Configure();
            //AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            glog.Debug("Application_Start");
        }
    }
}
