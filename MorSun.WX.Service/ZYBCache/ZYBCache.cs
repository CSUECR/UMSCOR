using System;
using System.Text;
using System.Collections.Generic;
using System.Web.Caching;
using MorSun.Model;
using HOHO18.Common;

namespace MorSun.WX.ZYB.Service
{
    /// <summary>
    /// 缓存类，缓存用户答题的题目。用户回答问题时， 系统知道他回答的是哪个问题。
    /// </summary>
    public class ZYBCache
    {
        private static string xmlSystemName = "XmlSystemName".GW();
        public static List<QACache> GetUserQACache(string qaCache = "QACache")
        {            
            //获取路径
            string path = System.Web.HttpContext.Current.Server.MapPath(xmlSystemName);

            //从缓存中读取
            List<QACache> keyValueObject = CacheAccess.GetFromCache(qaCache) as List<QACache>;

            if (keyValueObject == null)
            {
                CacheDependency fileDependency = new CacheDependency(path);

                List<QACache> qaCacheList = new List<QACache>();

                //保存到缓存中
                CacheAccess.SaveToCacheByDependency(qaCache, qaCacheList, fileDependency);
                keyValueObject = qaCacheList;
            }
            return keyValueObject;
        }
    }
}