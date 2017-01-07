using System.Web.Optimization;

namespace Innostar.UI.App_Start
{
	public class BundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/Scripts/jquery-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
						"~/Scripts/jquery-ui-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jQueryExtensions").Include(
						"~/Scripts/jQueryExtensions.js",
						"~/Scripts/waitingIndicator.js"));

			bundles.Add(new ScriptBundle("~/bundles/shared").Include(
						"~/Scripts/shared.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
				"~/Scripts/jquery.validate.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryxmpp").Include(
                "~/Scripts/jquery.xmpp.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryvalext").Include(
				"~/Scripts/jValExtensions.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryvalun")
				.Include("~/Scripts/jquery.unobtrusive*",
				"~/Scripts/jquery.validate.unobtrusive.js"));

			bundles.Add(new ScriptBundle("~/bundles/globalize")
				.Include("~/Scripts/globalize.js",
				"~/Scripts/globalize.culture.ru.js",
				"~/Scripts/globalize.culture.ru-RU"));

			bundles.Add(new ScriptBundle("~/bundles/spin").Include(
				"~/Scripts/spin.js"));

			bundles.Add(new ScriptBundle("~/bundles/purl").Include(
						"~/Scripts/purl.js"));

			bundles.Add(new ScriptBundle("~/bundles/jquery-paged-scroll").Include(
						"~/Scripts/jquery-paged-scroll.js"));

			bundles.Add(new ScriptBundle("~/bundles/jquery.mCustomScrollbar").Include(
					   "~/Scripts/jquery.mCustomScrollbar.js"));

			bundles.Add(new ScriptBundle("~/bundles/typeahead").Include(
						"~/Scripts/typeahead.js"));

			bundles.Add(new ScriptBundle("~/bundles/tagsCloud").Include(
						"~/Scripts/jqcloud-1.0.4.js",
						"~/Scripts/tagsCloud.js"));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						"~/Scripts/modernizr-*"));

			bundles.Add(new StyleBundle("~/Content/basecss").Include(
				"~/Content/site.css"));

			bundles.Add(new StyleBundle("~/Content/adminstyles").Include(
				"~/Content/site-admin.css"));

            bundles.Add(new ScriptBundle("~/bundles/usersmanagement").Include(
                        "~/Scripts/administration/usersManagement.js"));

			bundles.Add(new StyleBundle("~/Content/controls").Include(
				"~/Content/typeahead.js-bootstrap.css",
				"~/Content/jqcloud.css",
				"~/Content/jquery.mCustomScrollbar.css"));

			bundles.Add(new StyleBundle("~/Content/maps").Include(
				"~/Content/MapStyles.css"));

			bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
						"~/Content/themes/base/jquery.ui.core.css",
						"~/Content/themes/base/jquery.ui.resizable.css",
						"~/Content/themes/base/jquery.ui.selectable.css",
						"~/Content/themes/base/jquery.ui.accordion.css",
						"~/Content/themes/base/jquery.ui.autocomplete.css",
						"~/Content/themes/base/jquery.ui.button.css",
						"~/Content/themes/base/jquery.ui.dialog.css",
						"~/Content/themes/base/jquery.ui.slider.css",
						"~/Content/themes/base/jquery.ui.tabs.css",
						"~/Content/themes/base/jquery.ui.datepicker.css",
						"~/Content/themes/base/jquery.ui.progressbar.css",
						"~/Content/themes/base/jquery.ui.theme.css"));
		}
	}
}