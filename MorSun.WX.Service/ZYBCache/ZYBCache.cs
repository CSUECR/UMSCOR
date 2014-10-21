using System;
using System.Text;
using System.Collections.Generic;
using System.Web.Caching;
using MorSun.Model;
using HOHO18.Common;

namespace MorSun.WX.ZYB.Service
{
    /// <summary>
    /// 缓存类，缓存在线答题用户的题目(每个用户一个缓存)。用户回答问题时， 系统知道他回答的是哪个问题，以及缓存用户待答题和已答题数据。
    /// </summary>
    public static class ZYBCache
    {
        private static string xmlSystemName = "XmlSystemName".GW();
        /// <summary>
        /// 用户答题缓存
        /// </summary>
        /// <param name="uid">传入的参数是 dt + uid uid是微信ID</param>
        /// <returns></returns>
        public static QACache GetUserQACache(string uid)
        {            
            //获取路径
            string path = System.Web.HttpContext.Current.Server.MapPath(xmlSystemName);

            //从缓存中读取
            var model = CacheAccess.GetFromCache(uid) as QACache;

            if (model == null)
            {
                CacheDependency fileDependency = new CacheDependency(path);

                var qaCache = new QACache();
                qaCache.WeiXinId = uid.Substring(2);
                //保存到缓存中
                CacheAccess.SaveToCacheByDependency(uid, qaCache, fileDependency);
                model = qaCache;
            }
            return model;
        }

        /// <summary>
        /// 设置用户答题缓存
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="qaCache"></param>
        public static void SetUserQACache(string uid, QACache qaCache)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath(xmlSystemName);
            CacheDependency fileDependency = new CacheDependency(path);
            
            //保存到缓存中
            CacheAccess.SaveToCacheByDependency(uid, qaCache, fileDependency);
        }
    }
}