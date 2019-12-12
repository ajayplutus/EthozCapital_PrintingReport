using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using log4net;

namespace EthozCapital
{
    public class RouteConfig
    {
        private static ILog glog = log4net.LogManager.GetLogger(typeof(RouteConfig)); 

        public static void RegisterRoutes(RouteCollection routes)
        {
            glog.Debug("RegisterRoutes: Entry");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Default", id = UrlParameter.Optional }
            );

            glog.Debug("RegisterRoutes: Exit");
        }
    }
}
