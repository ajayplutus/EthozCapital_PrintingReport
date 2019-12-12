using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using log4net;

namespace EthozCapital.App_Start
{
    public class Startup
    {        
        private static ILog glog = log4net.LogManager.GetLogger(typeof(Startup)); 
        
        public void Configuration(IAppBuilder app)
        {
            glog.Debug("Configuration: Entry");
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString("/auth/login")
            });
            glog.Debug("Configuration: Exit");
        }
    }
}