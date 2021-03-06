﻿using System.Web;
using System.Web.Optimization;

namespace VirtualLibrary.App_Start
{
    public class BundleConfig
    {
        protected BundleConfig()
        {

        }

        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                     "~/Content/bootstrap.min.css",
                     "~/Content/timeline.css",
                     "~/Content/DataTables/css/jquery.dataTables.css",
                     "~/Content/sb-admin-2.css"));

        }
    }
}
