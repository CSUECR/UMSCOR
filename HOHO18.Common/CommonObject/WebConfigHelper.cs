using System;
using System.Configuration;
using System.Web.Caching;
using System.Web;

namespace HOHO18.Common
{
    /// <summary>
    /// web.config������    
    /// </summary>
    public sealed class WebConfigHelper
    {
        /// <summary>
        /// �õ�AppSettings�е������ַ�����Ϣ
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetWebConfig(string key)
        {
            var CacheKey = "AppSettings-" + key;
            var objModel = CacheAccess.GetFromCache(CacheKey);
            if (objModel == null)
            {
                try
                {
                    objModel = ConfigurationManager.AppSettings[key];
                    if (objModel != null)
                    {
                        CacheDependency fileDependency = new CacheDependency(HttpContext.Current.Server.MapPath("~/Web.Config"));
                        CacheAccess.SaveToCacheByDependency(CacheKey, objModel, fileDependency);
                        //ԭ����CacheAccess.SetCache(CacheKey, objModel, DateTime.Now.AddMinutes(180), TimeSpan.Zero);
                    }
                }
                catch
                { }
            }
            return objModel.ToString();
        }        
    }
}
