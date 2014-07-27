﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MorSun.Controllers;
using HOHO18.Common.WEB;

namespace MorSun
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //log4net.Config.XmlConfigurator.Configure();
            LogHelper.Init();
            AreaRegistration.RegisterAllAreas();
            ///todo:替换默认模型绑定器
            ModelBinders.Binders.DefaultBinder = new SmartModelBinder();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            //关闭任务调度器
            //MorSunScheduler.Instance.Stop(true);
            //解决应用池回收问题 
            System.Threading.Thread.Sleep(5000);
            string strUrl = "http://" + "ServiceDomain".GetXmlConfig();
            System.Net.HttpWebRequest _HttpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(strUrl);
            System.Net.HttpWebResponse _HttpWebResponse = (System.Net.HttpWebResponse)_HttpWebRequest.GetResponse();
            System.IO.Stream _Stream = _HttpWebResponse.GetResponseStream();//得到回写的字节流 
        }
    }

    
}