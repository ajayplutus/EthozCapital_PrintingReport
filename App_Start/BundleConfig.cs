using System;
using System.Web;
using System.Web.Optimization;

namespace EthozCapital.App_Start
{
	public static class BundleConfig
	{
		//EZ:CP: Bundle scripts reference
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.IgnoreList.Clear();
			AddDefaultIgnorePatterns(bundles.IgnoreList);

			bundles.Add(new StyleBundle("~/assets/coloradmin").Include(
				"~/assets/plugins/jquery-ui/themes/base/jquery-ui.css",
				"~/assets/plugins/jquery-ui/themes/base/minified/jquery-ui.min.css",
				"~/assets/plugins/bootstrap/css/bootstrap.min.css",
				"~/assets/plugins/bootstrap/css/bootstrap.css",
				"~/assets/plugins/DataTables/media/css/jquery.dataTables.min.css",
				"~/assets/plugins/DataTables/extensions/Select/css/select.dataTables.min.css",
				"~/assets/plugins/font-awesome/css/font-awesome.css",
				"~/assets/plugins/font-awesome/css/font-awesome.min.css",
				"~/assets/plugins/ionicons/css/ionicons.css",
				"~/assets/plugins/ionicons/css/ionicons.min.css",
				//Download ionicons css
				//https://unpkg.com/ionicons@4.4.4/dist/css/ionicons.min.css               
				//Select2 Placeholders Combobox CSS - Start
				"~/assets/plugins/select2/dist/css/select2.min.css",
				"~/assets/plugins/select2/dist/css/select2.css",
				//Select2 Placeholders Combobox CSS - End
				//sweetalert CSS - Start
				"~/assets/css/sweetalert.css",
				//sweetalert CSS - End
				"~/assets/css/animate.css",
				"~/assets/css/animate.min.css",
				"~/assets/css/style.css",
				"~/assets/css/style.min.css",
				"~/assets/css/style-responsive.css",
				"~/assets/css/style-responsive.min.css",
				"~/assets/css/coloradmin_custom.css",
                "~/CustomCss/Global.css"
				));

			#region Style And Js Datatable
			bundles.Add(new StyleBundle("~/assets/datatable").Include(
				"~/assets/plugins/DataTables/media/css/dataTables.bootstrap.min.css",
				"~/assets/plugins/DataTables/media/css/jquery.dataTables.min.css",
				"~/assets/plugins/DataTables/extensions/Select/css/select.dataTables.min.css",
				"~/assets/plugins/DataTables/extensions/Buttons/css/buttons.dataTables.min.css"
				));

			bundles.Add(new ScriptBundle("~/scripts/datatables").Include(
				//DataTables 
				"~/assets/plugins/DataTables/media/js/jquery.dataTables.min.js",
				"~/assets/plugins/DataTables/media/js/dataTables.bootstrap.min.js",
				"~/assets/plugins/DataTables/extensions/Editor/js/dataTables.editor.min.js",
				"~/assets/plugins/DataTables/extensions/AutoFill/js/dataTables.autoFill.min.js",
				"~/Content/Datatable/dataTables.buttons.min.js",
				"~/assets/plugins/DataTables/extensions/ColReorder/js/dataTables.colReorder.min.js",
				"~/assets/plugins/DataTables/extensions/FixedColumns/js/dataTables.fixedColumns.min.js",
				"~/assets/plugins/DataTables/extensions/Scroller/js/dataTables.scroller.min.js",
				"~/assets/plugins/DataTables/extensions/Select/js/dataTables.select.min.js",
				"~/Content/Datatable/buttons.flash.min.js",
				 "~/Content/Datatable/buttons.html5.min.js",
				 "~/Content/Datatable/buttons.print.min.js",
				 "~/Content/Datatable/jszip.min.js",
				 "~/Content/Datatable/pdfmake.min.js"
				));
			#endregion  Style And Js Datatable

			#region Js Color Admin
			bundles.Add(new ScriptBundle("~/scripts/coloradmin").Include(
                "~/Scripts/jquery-1.10.2.js",
                "~/Scripts/jquery-1.12.4.min.js",
                "~/Scripts/jquery-ui-1.10.4.min.js",
                "~/assets/plugins/moment/moment.js",
                "~/assets/plugins/moment/moment.min.js",
                "~/assets/plugins/jquery-ui/ui/jquery-ui.js",
                "~/Scripts/i18n/grid.locale-en.js",
                //DataTables 
                "~/assets/plugins/flot/jquery.js",
                "~/assets/plugins/flot/jquery.min.js",
                "~/assets/plugins/bootstrap/js/bootstrap.min.js",
                "~/assets/plugins/DataTables/media/js/jquery.dataTables.min.js",
                "~/assets/plugins/DataTables/media/js/dataTables.bootstrap.min.js",
                "~/assets/plugins/DataTables/extensions/Select/js/dataTables.select.min.js",
                //DataTables Editor
                "~/assets/plugins/DataTables/extensions/Editor/js/dataTables.editor.js",
                "~/assets/plugins/DataTables/extensions/Editor/js/dataTables.editor.min.js",
                //DataTables Editor
                "~/assets/plugins/bootstrap/js/bootstrap.min.js",
                "~/assets/plugins/slimscroll/jquery.slimscroll.min.js",
                "~/assets/plugins/jquery-cookie/jquery.cookie.js",
                //sweetalert JS - Start
                "~/assets/js/sweetalert-dev.js",
                //sweetalert JS - End
                "~/assets/js/apps.js",
                "~/assets/js/apps.min.js",
                //Select2 Placeholders Combobox JS - Start
                "~/assets/plugins/select2/dist/js/select2.min.js",
                "~/assets/plugins/select2/dist/js/select2.js",
                //Select2 Placeholders Combobox JS - End
                //Bootstrap Validation - Start
                "~/assets/js/formValidation.min.js",
                "~/assets/js/bootstrap.min.js",
                //Bootstrap Validation - End

                //Forms + Validation - Start
                "~/assets/plugins/parsley/dist/parsley.js",
                "~/assets/plugins/bootstrap-wizard/js/bwizard.js",
                "~/assets/js/form-wizards-validation.demo.js",
                "~/assets/js/form-wizards-validation.demo.min.js"
				 ));

			BundleTable.EnableOptimizations = true;
		}
		#endregion Js Color Admin

		public static void AddDefaultIgnorePatterns(IgnoreList ignoreList)
		{
			if (ignoreList == null)
				throw new ArgumentNullException("ignoreList");
			ignoreList.Ignore("*.intellisense.js");
			ignoreList.Ignore("*-vsdoc.js");
			ignoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
		}
	}
}
