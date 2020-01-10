using System.Web;
using System.Web.Optimization;

namespace SINU
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                         "~/Scripts/jquery-{version}.js",
                         "~/Scripts/jquery.unobtrusive-ajax.min.js",
                         "~/Scripts/DataTables/datatables.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-ui").Include(
                     "~/Scripts/jquery-ui-*",
                     "~/Scripts/JQuery-Conbobox.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-datepicker.min.js",
                      "~/Scripts/stacktable.js",
                      "~/Scripts/respond.js"
                      ));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/themes/base/all.css",
                      "~/Content/bootstrap.css",
                      "~/Content/themes/base/jquery-ui.css",
                      "~/Content/Conbobox.css",
                      "~/Content/bootstrap-datepicker.standalone.min.css",
                      "~/Scripts/DataTables/datatables.css",
                      "~/Content/site.css"
                      
                     ));
        }
    }
}
