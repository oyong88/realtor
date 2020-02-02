using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace IMS.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            //base bundle...
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                        "~/Scripts/app.js"
                        ));


            bundles.Add(new StyleBundle("~/bundles/css").Include(
                        "~/content/bootstrap.min.css",
                        "~/content/bootstrap-theme.min.css",
                        "~/content/site.css",
                        "~/content/bootstrap-datetimepicker.css",
                        "~/content/Site.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}