using System;
using System.Web;
using System.Web.Caching;

namespace HOHO18.Common
{
    /// <summary>
    /// 控制类，用于缓存操作
    /// </summary>
    public sealed class CacheAccess
    {
        /// <summary>
        /// 设置有过期时间的缓存 会覆盖
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheObject"></param>
        /// <param name="seconds"></param>
        public static void InsertToCacheByTime(string cacheKey,object cacheObject, int seconds)
        {
            Cache cache = HttpRuntime.Cache;
            cache.Insert(cacheKey, cacheObject, null,DateTime.Now.AddSeconds(seconds),TimeSpan.Zero);
        }

        /// <summary>
        /// 添加缓存，不可覆盖
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheObject"></param>
        /// <param name="seconds"></param>
        public static void AddToCacheByTime(string cacheKey, object cacheObject, int seconds)
        {
            Cache cache = HttpRuntime.Cache;
            cache.Add(cacheKey, cacheObject, null, DateTime.Now.AddSeconds(seconds), TimeSpan.Zero, CacheItemPriority.Normal, null);
        }

        /// <summary>
        /// 将对象加入到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="cacheObject">缓存对象</param>
        /// <param name="dependency">缓存依赖项</param>
        public static void SaveToCacheByDependency(string cacheKey, object cacheObject,CacheDependency dependency)
        {
            Cache cache = HttpRuntime.Cache;
            cache.Insert(cacheKey, cacheObject, dependency);
        }

        /// <summary>
        /// 从缓存中取得对象，不存在则返回null
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <returns>获取的缓存对象</returns>
        public static object GetFromCache(string cacheKey)
        {
            Cache cache = HttpRuntime.Cache;
            return cache[cacheKey];
        }
        

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="objObject"></param>
        public static void SetCache(string CacheKey, object objObject)
        {
            Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject);
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="objObject"></param>
        public static void SetCache(string CacheKey, object objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject, null, absoluteExpiration, slidingExpiration);
        }

        /// <summary>
        /// 清除某个键的缓存
        /// </summary>
        /// <param name="CacheKey"></param>
        public static void RemoveCache(string CacheKey)
        {
            Cache objCache = HttpRuntime.Cache;
            objCache.Remove(CacheKey);
        }
    }
}
