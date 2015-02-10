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
            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当您做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //slides
            bundles.Add(new ScriptBundle("~/bundles/slides").Include("~/Scripts/slides.min.jquery.js"));            
            //HOHO18.Common
            bundles.Add(new ScriptBundle("~/bundles/hoho18").Include("~/Scripts/Common/HOHO18.Common.js"));
            //MorSun.Common
            bundles.Add(new ScriptBundle("~/bundles/morsuncommon").Include("~/Scripts/Common/MorSun.Common.js"));
            //WeiXin.Common
            bundles.Add(new ScriptBundle("~/bundles/weixincommon").Include("~/Scripts/Common/WeiXin.Common.js"));

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
            bundles.Add(new StyleBundle("~/Content/JQueryQtipCss").Include("~/Content/JQueryQtipCustom/jquery.qtip.css"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryqtip").Include("~/Content/JQueryQtipCustom/jquery.qtip.js"));
            //treetable
            bundles.Add(new ScriptBundle("~/bundles/treetable").Include("~/Content/JQueryTreeTable/jquery.treetable.js"));
            bundles.Add(new StyleBundle("~/Content/treetableCss").Include("~/Content/JQueryTreeTable/jquery.treetable.css",
                "~/Content/JQueryTreeTable/jquery.treetable.theme.default.css"));

            //uniform
            bundles.Add(new ScriptBundle("~/bundles/uniform").Include("~/Content/uniform/jquery.uniform.js"));
            bundles.Add(new StyleBundle("~/Content/uniformdefaultCss").Include("~/Content/uniform/default/css/uniform.default.css"));
            bundles.Add(new StyleBundle("~/Content/uniformagentCss").Include("~/Content/uniform/agent/css/uniform.agent.css"));
            bundles.Add(new StyleBundle("~/Content/uniformaristoCss").Include("~/Content/uniform/aristo/css/uniform.aristo.css"));
            bundles.Add(new StyleBundle("~/Content/uniformjeansCss").Include("~/Content/uniform/jeans/css/uniform.jeans.css"));
            bundles.Add(new StyleBundle("~/Content/uniformmetroCss").Include("~/Content/uniform/metro/css/uniform.metro.css")); 

            //waypoints
            bundles.Add(new ScriptBundle("~/bundles/waypoint").Include("~/Scripts/waypoints.js"));
        }
    }
}