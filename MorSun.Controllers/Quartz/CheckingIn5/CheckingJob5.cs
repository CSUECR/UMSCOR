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

        public void SetAccessTokenCache()
        {
            LogHelper.Write("进入自动设置AT缓存", LogHelper.LogMessageType.Debug);
            
            var atURL = CFG.邦马网_获取AT网址.Replace("APPID", CFG.邦马网_应用ID).Replace("APPSECRET", CFG.邦马网_应用密钥);            
            var atS = GetHtmlHelper.GetPage(atURL, "");
            var wsTKJson = JsonConvert.DeserializeObject<wxTKJson>(atS);
            if(!String.IsNullOrEmpty(wsTKJson.errcode) || !String.IsNullOrEmpty(wsTKJson.errmsg))
            {
                LogHelper.Write("获取微信ToKen失败" + wsTKJson.errcode + " " + wsTKJson.errmsg, LogHelper.LogMessageType.Error);
            }
            else
            {     
                //保存到缓存中
                CacheAccess.AddToCacheByTime(CFG.邦马网_AT缓存键, wsTKJson.access_token, 5400);                
            }
        }

        public void Execute(IJobExecutionContext context)
        {           
            try
            { 
                //SetOlineQAUserCache(MorSun.Controllers.BasisController.GenerateQAUserCache());
                SetAccessTokenCache();
            }
            catch(Exception ex)
            {
                LogHelper.Write("微信TK缓存设置异常" + ex.Message, LogHelper.LogMessageType.Error);
            }

        }
    }
}
