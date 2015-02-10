using HOHO18.Common.WEB;
using MorSun.Bll;
using MorSun.Common.类别;
using MorSun.Common.配置;
using MorSun.Controllers.ViewModel;
using MorSun.Model;
using MorSun.WX.ZYB.Service;
using Quartz;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using HOHO18.Common;
using MorSun.Common.常量集;
using System.Web.Caching;
using System.Web;
using HOHO18.Common.Web;
using Newtonsoft.Json;

namespace MorSun.Controllers.Quartz
{
    public class CheckingJob5:IJob
    {       
        
        public void SaveToCacheByDependency(string cacheKey, object cacheObject, CacheDependency dependency)
        {
            Cache cache = HttpRuntime.Cache;
            cache.Insert(cacheKey, cacheObject, dependency);
            LogHelper.Write("自动Insert缓存", LogHelper.LogMessageType.Debug);
        }

        public string SetWXTKCache()
        {
            var atURL = CFG.邦马网_获取AT网址.Replace("APPID", CFG.邦马网_应用ID).Replace("APPSECRET", CFG.邦马网_应用密钥);
            //LogHelper.Write("获取微信ToKen的URL" + atURL, LogHelper.LogMessageType.Info);
            var atS = GetHtmlHelper.GetPage(atURL, "");
            var wxTKJson = JsonConvert.DeserializeObject<wxTKJson>(atS);
            //LogHelper.Write("获取微信ToKen" + wsTKJson.access_token, LogHelper.LogMessageType.Info);
            if (!String.IsNullOrEmpty(wxTKJson.errcode) || !String.IsNullOrEmpty(wxTKJson.errmsg))
            {
                LogHelper.Write("获取微信ToKen失败" + wxTKJson.errcode + " " + wxTKJson.errmsg, LogHelper.LogMessageType.Error);
            }
            else
            {
                //保存到缓存中
                CacheAccess.AddToCacheByTime(CFG.邦马网_AT缓存键, wxTKJson.access_token, 5400);
            }

            return wxTKJson.access_token;
        }

        public string GetWXTKCache()
        {
            var s = CacheAccess.GetFromCache(CFG.邦马网_AT缓存键) as string;
            if (String.IsNullOrEmpty(s))
            {
                s = SetWXTKCache();
            }
            return s;
        }

        public void SetWXTICCache()
        {
            var s = GetWXTKCache();
            var ticURL = CFG.邦马网_获取TIC网址.Replace("ACCESS_TOKEN", s);
            
            //LogHelper.Write("获取微信ToKen的URL" + atURL, LogHelper.LogMessageType.Info);
            var ticS = GetHtmlHelper.GetPage(ticURL, "");
            var wxTICJson = JsonConvert.DeserializeObject<wxTICJson>(ticS);
            //LogHelper.Write("获取微信ToKen" + wsTKJson.access_token, LogHelper.LogMessageType.Info);
            if (String.IsNullOrEmpty(wxTICJson.ticket))
            {
                LogHelper.Write("获取微信Ticket失败", LogHelper.LogMessageType.Error);
            }
            else
            {
                //保存到缓存中
                CacheAccess.AddToCacheByTime(CFG.邦马网_TIC缓存键, wxTICJson.ticket, 5400);
            }
        }

        public void Execute(IJobExecutionContext context)
        {           
            try
            { 
                //SetOlineQAUserCache(MorSun.Controllers.BasisController.GenerateQAUserCache());
                SetWXTKCache();
                SetWXTICCache();
            }
            catch(Exception ex)
            {
                LogHelper.Write("微信TK缓存设置异常" + ex.Message, LogHelper.LogMessageType.Error);
            }

        }
    }
}
