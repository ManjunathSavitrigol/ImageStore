using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace ImageStore
{

    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //sweet alert
            bundles.Add(new ScriptBundle("~/bundle/sweetalert").Include(
                "~/Content/SweetAlert/sweetalert.min.js"));

            //jquery
            bundles.Add(new ScriptBundle("~/bundle/jquery").Include(
                "~/Content/Jquery/jquery.min.js"));

            //jquery table
            bundles.Add(new ScriptBundle("~/bundles/jquerytable_js").Include(
                          "~/Content/Jquery/dataTables.min.js"));
            bundles.Add(new StyleBundle("~/bundles/jquerytable_css").Include(
                         "~/Content/Jquery/dataTables.min.css"));


            //fontawesome
            bundles.Add(new StyleBundle("~/bundle/fontawesome").Include(
                      "~/Content/FontAwesome/css/font-awesome.css"));


            //rubik font
            bundles.Add(new StyleBundle("~/bundle/rubikfont").Include(
                      "~/Content/RubiFont/StyleSheet.css"));

            //Bootstrap
            bundles.Add(new ScriptBundle("~/bundle/bootstrap5_js").Include(
                "~/Content/Bootstrap5/bootstrap.min.js"));

            bundles.Add(new StyleBundle("~/bundle/bootstrap5_css").Include(
                "~/Content/Bootstrap5/bootstrap.min.css"));





            
        }
    }

}