using System.Web;
using System.Web.Optimization;

namespace tt2.Wx.Web2
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bc)
        {
            bc.Add(new ScriptBundle("~/sb/Default").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/CryptoJs/crypto-js.js",
                        "~/Scripts/Global.js"
                        ));




            bc.Add(new ScriptBundle("~/bc/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bc.Add(new ScriptBundle("~/bc/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bc.Add(new ScriptBundle("~/bc/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));




            bc.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));



        }
    }
}
