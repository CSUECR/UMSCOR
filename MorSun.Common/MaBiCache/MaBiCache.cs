using System;
using System.Text;
using System.Collections.Generic;
using System.Web.Caching;
using MorSun.Model;
using HOHO18.Common;

namespace MorSun.Common
{
    /// <summary>
    /// 缓存类，缓存用户的马币数据
    /// </summary>
    public static class MaBiCache
    {
        private static string xmlSystemName = "XmlSystemName".GW();
        /// <summary>
        /// 用户马币缓存
        /// </summary>
        /// <param name="uid">传入的参数是 mb + uid  uid 是微信ID</param>
        /// <returns></returns>
        public static UserMaBiCache GetUserMaBiCache(string uid)
        {            
            //获取路径
            string path = System.Web.HttpContext.Current.Server.MapPath(xmlSystemName);

            //从缓存中读取
            var model = CacheAccess.GetFromCache(uid) as UserMaBiCache;

            if (model == null)
            {
                CacheDependency fileDependency = new CacheDependency(path);

                var uMaBi = new UserMaBiCache();
                uMaBi.WeiXinId = uid.Substring(2);
                //保存到缓存中
                CacheAccess.SaveToCacheByDependency(uid, uMaBi, fileDependency);
                model = uMaBi;
            }
            return model;
        }

        /// <summary>
        /// 设置用户马币缓存
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="uMaBi"></param>
        public static void SetUserMaBiCache(string uid, UserMaBiCache uMaBi)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath(xmlSystemName);
            CacheDependency fileDependency = new CacheDependency(path);
            
            //保存到缓存中
            CacheAccess.SaveToCacheByDependency(uid, uMaBi, fileDependency);
        }        
    }
}