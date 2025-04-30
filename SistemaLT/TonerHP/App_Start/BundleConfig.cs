using CapaEntidad;
using System.Data;
using System.Web;
using System.Web.Optimization;
using System.Web.UI.WebControls;

namespace TonerHP
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/complementos").Include(
                //"~/Scripts/fontawesome/all.min.js",
                "~/Scripts/dataTables.min.js",
                //"~/Scripts/jquery-3.7.0.min.js",
                "~/Scripts/moment.min.js",
                "~/Scripts/datetime-moment.js",
                "~/Scripts/DataTables/dataTables.buttons.js",
                "~/Scripts/DataTables/buttons.dataTables.js",
                "~/Scripts/DataTables/jszip.min.js",
                "~/Scripts/DataTables/pdfmake.min.js",
                "~/Scripts/DataTables/vfs_fonts.js",
                "~/Scripts/DataTables/buttons.html5.min.js",
                "~/Scripts/DataTables/buttons.print.min.js",
                "~/Scripts/DataTables/buttons.colVis.min.js",
                "~/Scripts/DataTables/jdataTables.responsive.js",
                "~/Scripts/loadingoverlay/loadingoverlay.min.js",
                "~/Scripts/sweetalert2.min.js",
                "~/Scripts/Datepicker.js",
                "~/Scripts/jquery-ui.js",
                "~/Scripts/scripts.js",
                "~/Scripts/select2.min.js",
                "~/Scripts/daterangepicker.min.js"
                ));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.bundle.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/site.css",
                      "~/Content/dataTables.dataTables.min.css",
                      "~/Content/DataTables/css/responsive.dataTables.min.css",
                      "~/Content/DataTables/buttons.dataTables.css",
                      "~/Content/images",
                      "~/Content/sweetalert2.min.css",
                      "~/Content/jquery-ui.min.css",
                      "~/Content/select2.min.css",
                     "~/Content/bootstrapselect2.min.css",
                      "~/Content/daterangepicker.css"
                      ));
        }
    }
}
