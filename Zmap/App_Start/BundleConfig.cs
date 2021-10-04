using System.Web;
using System.Web.Optimization;

namespace Zmap
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/angular.min.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/custom.js",
                        "~/Scripts/jquery.min.js",
                        "~/Scripts/jquery.mixitup.min.js",
                        "~/Scripts/jquery-ui.js",
                        "~/Scripts/materialize.min.js",
                        "~/Scripts/owl.carousel.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/materialize.css",
                      "~/Content/owl.carousel.min.css",
                      "~/Content/owl.theme.min.css",
                      "~/Content/responsive.css",
                      "~/Content/style.css"));
        }
    }
}
