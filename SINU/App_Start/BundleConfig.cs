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
                         "~/Scripts/DataTables/datatables.js",
                          "~/Scripts/DataTables/Buttons-1.6.1/js/dataTables.buttons.min.js",
                          "~/Scripts/DataTables/Buttons-1.6.1/js/buttons.print.min.js",
                          "~/Scripts/DataTables/Buttons-1.6.1/js/buttons.colVis.min.js"));

        

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/JavaScript-Decimal.js"));



            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));



            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/umd/popper.js",
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-select.js",
                      "~/Scripts/bootstrap-datepicker.min.js",
                      "~/Scripts/stacktable.js",
                      "~/Scripts/respond.js"
                      ));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                       "~/Content/all.css",
                      "~/Content/bootstrap-select.css",
                      "~/Content/bootstrap-datepicker.standalone.min.css",
                      "~/Scripts/DataTables/datatables.css",
                      "~/Scripts/DataTables/DataTables-1.10.20/jquery.dataTables.min.css",
                      "~/Scripts/DataTables/Buttons-1.6.1/css/buttons.dataTables.min.css",
                     "~/Content/site.css"

                     ));
        }
    }
}
