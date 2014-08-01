using System.Web;
using System.Web.Optimization;

namespace MorSun
{
    public class BundleConfig
    {
        // 有关 Bundling 的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();
            bundles.IgnoreList.Ignore("*-vsdoc.js");
            bundles.IgnoreList.Ignore("*intellisense.js");
            bundles.IgnoreList.Ignore("*min.js");
            BundleTable.EnableOptimizations = true;

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));//,""
            bundles.Add(new ScriptBundle("~/bundles/validator").Include(
                        "~/Scripts/jquery.validator.ex.js"));
            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

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

            bundles.Add(new StyleBundle("~/Content/themes/morsun/css").Include(
                        "~/Content/themes/morsun/base.css",
                        "~/Content/themes/morsun/custom.css"
                        ));

            //JQueryQtipCustom
            bundles.Add(new StyleBundle("~/Content/JQueryQtipCustom/jquery.qtip.css").Include("~/Content/JQueryQtipCustom/jquery.qtip.css"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryqtip").Include("~/Content/JQueryQtipCustom/jquery.qtip.js"));

            //slides
            bundles.Add(new ScriptBundle("~/bundles/slides").Include("~/Scripts/slides.min.jquery.js"));
            //HOHO18.Common
            bundles.Add(new ScriptBundle("~/bundles/HOHO18").Include("~/Scripts/Common/HOHO18.Common.js"));
            //MorSun.Common
            bundles.Add(new ScriptBundle("~/bundles/MorSunCommon").Include("~/Scripts/Common/MorSun.Common.js"));
        }
    }
}