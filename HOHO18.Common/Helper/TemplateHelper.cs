using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;


namespace HOHO18.Common.Helper
{
    public class TemplateHelper
    {
        /// <summary>
        /// 当前使用的模版后缀(包括点)
        /// </summary>
        public static String CurrTemplateSuffix = System.Configuration.ConfigurationManager.AppSettings["CurrTemplateSuffix"];


        /// <summary>
        /// 获取模版生成的HTML数据
        /// </summary>
        /// <param name="templatePath"></param>
        /// <param name="viewData"></param>
        /// <returns></returns>
        public static String GetHtmlByTemplate(string templatePath, ViewDataDictionary viewData,Encoding encoding)
        {
            //System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
            StringBuilder sb = new StringBuilder();
            StringWriter strWriter = new StringWriter();
            System.Web.Mvc.ViewPage tempViewPage = new System.Web.Mvc.ViewPage();
            //System.Web.UI.Page.CreateHtmlTextWriterFromType(
            tempViewPage.ViewData = viewData;
            //加载模版.
            tempViewPage.Controls.Add(tempViewPage.LoadControl(templatePath));

            //执行得到页面HTML
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(new System.IO.StringWriter(sb));
            tempViewPage.RenderControl(htw);
            htw.Close();
            htw.Dispose();

            return sb.ToString();
        }

        /// <summary>
        /// 更新模版到页面,并传入模版需要的数据
        /// </summary>
        /// <param name="templatePath">模版的虚拟路径</param>
        /// <param name="pageLocalPath">页面的本地路径</param>
        /// <param name="viewData">模版需要的数据</param>
        public static void RefreshTemplateToPage(string templatePath, string pageLocalPath, string templateEncodingName, ViewDataDictionary viewData)
        {
            FileInfo f = new FileInfo(pageLocalPath);
            if (!f.Directory.Exists)
            {
                f.Directory.Create();
            }
            Encoding encoding = Encoding.GetEncoding(templateEncodingName);
            File.WriteAllText(pageLocalPath, GetHtmlByTemplate(templatePath, viewData, encoding), encoding);
        }

        /// <summary>
        /// 载入用户自定义控件
        /// </summary>
        /// <param name="name">控件名称,其他参数使用默认的配置(CurrTemplateSuffix,ModelControlsDirectory)</param>
        public static String LoadControl(String name, ViewDataDictionary viewData)
        {
            return LoadControl(name, CurrTemplateSuffix, System.Configuration.ConfigurationManager.AppSettings["ModelControlsDirectory"], viewData);
        }
        /// <summary>
        /// 载入网点的自定义控件
        /// </summary>
        /// <param name="name">控件名称,其他参数使用默认的配置(CurrTemplateSuffix,ModelControlsDirectory)</param>
        //public static String LoadDeptControl(String name, ViewDataDictionary viewData, Guid deptID, bool isWap)
        //{
        //    string deptDir = TFWSNS.Controllers.HttpModules.DomainHttpModule.deptDirectory.Replace("{DeptID}", deptID.ToString().ToLower());
        //    return LoadControl(name, CurrTemplateSuffix, deptDir + ((isWap ? DomainHttpModule.deptWapDirectory : "")) + System.Configuration.ConfigurationManager.AppSettings["DeptsModelControlsDirectory"], viewData);
        //}
        /// <summary>
        /// 载入网点的自定义控件
        /// </summary>
        /// <param name="name">控件名称,其他参数使用默认的配置(CurrTemplateSuffix,ModelControlsDirectory)</param>
        //public static String LoadDeptControl(String name, ViewDataDictionary viewData, Guid deptID)
        //{
        //    return LoadDeptControl(name, viewData, deptID, false);
        //}
        /// <summary>
        /// 载入用户自定义控件
        /// </summary>
        /// <param name="name">控件名称</param>
        /// <param name="suffix">控件使用的后缀(包括点)</param>
        /// <param name="dir">控件所在的虚拟目录,最后带"/"</param>
        public static String LoadControl(String name, String suffix, String dir, ViewDataDictionary viewData)
        {
            System.Web.UI.HtmlTextWriter t = new System.Web.UI.HtmlTextWriter(new StringWriter());
            System.Web.Mvc.ViewUserControl v = new ViewUserControl();
            v.ViewData = viewData;
            v.Controls.Add(v.LoadControl(dir + name + suffix));
            v.RenderControl(t);
            return t.InnerWriter.ToString();
        }
    }
}
