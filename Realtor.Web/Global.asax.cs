using IMS.Web;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Realtor.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(MvcApplication).ToString());

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            //configure log4net
            XmlConfigurator.Configure();
        }

        protected void Application_EndRequest()
        {

            var context = new HttpContextWrapper(this.Context);

            // If we're an ajax request and forms authentication caused a 302, 
            // then we actually need to do a 401
            if (FormsAuthentication.IsEnabled && context.Response.StatusCode == 302 && context.Request.IsAjaxRequest())
            {
                context.Response.Clear();
                context.Response.StatusCode = 401;
            }
        }
    }
}
