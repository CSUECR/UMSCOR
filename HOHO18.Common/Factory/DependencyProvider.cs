using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Caching;

namespace HOHO18.Common.Factory
{
    /// <summary>
    /// 依赖注入提供者
    /// 使用反射机制实现
    /// </summary>
    public sealed class DependencyProvider
    {
        /// <summary>
        /// 取得数据访问层对象
        /// 首先检查缓存中是否存在，如果不存在，则利用反射机制返回对象
        /// </summary>
        /// <param name="className">数据访问类名称</param>
        /// <returns>数据访问层对象</returns>
        public static object GetDALObject(string className)
        {
            /// <summary>
            /// 取得数据访问层名称，首先检查缓存，不存在则到配置文件中读取
            /// 缓存依赖项为Web.Config文件
            /// </summary>
            object dal = CacheAccess.GetFromCache("DAL");
            if (dal == null)
            {
                CacheDependency fileDependency = new CacheDependency(HttpContext.Current.Server.MapPath("~/Web.Config"));
                dal = ConfigurationManager.AppSettings["DAL"];
                CacheAccess.SaveToCacheByDependency("DAL", dal, fileDependency);
            }

            string dalName = (string)dal;

            /// <summary>
            /// 取得数据访问层对象，首先检查缓存，不存在则利用反射机制加载
            /// 缓存依赖项为Web.Config文件
            /// </summary>
            string fullClassName = dalName + "." + className;
            object dalObject = CacheAccess.GetFromCache(className);
            if (dalObject == null)
            {
                CacheDependency fileDependency = new CacheDependency(HttpContext.Current.Server.MapPath("~/Web.Config"));
                dalObject = Assembly.Load(dalName).CreateInstance(fullClassName);
                CacheAccess.SaveToCacheByDependency(className, dalObject, fileDependency);
            }

            return dalObject;
        }

        /// <summary>
        /// 取得业务逻辑层对象
        /// 首先检查缓存中是否存在，如果不存在，则利用反射机制返回对象
        /// </summary>
        /// <param name="className">业务逻辑类名称</param>
        /// <returns>业务逻辑层对象</returns>
        public static object GetBLLObject(string className)
        {
            /// <summary>
            /// 取得业务逻辑层名称，首先检查缓存，不存在则到配置文件中读取
            /// 缓存依赖项为Web.Config文件
            /// </summary>
            object bll = CacheAccess.GetFromCache("BLL");
            if (bll == null)
            {
                CacheDependency fileDependency = new CacheDependency(HttpContext.Current.Server.MapPath("~/Web.Config"));
                bll = ConfigurationManager.AppSettings["BLL"];
                CacheAccess.SaveToCacheByDependency("BLL", bll, fileDependency);
            }

            string bllName = (string)bll;

            /// <summary>
            /// 取得业务逻辑层对象，首先检查缓存，不存在则利用反射机制加载
            /// 缓存依赖项为Web.Config文件
            /// </summary>
            string fullClassName = bllName + "." + className;
            object bllObject = CacheAccess.GetFromCache(className);
            if (bllObject == null)
            {
                CacheDependency fileDependency = new CacheDependency(HttpContext.Current.Server.MapPath("~/Web.Config"));
                bllObject = Assembly.Load(bllName).CreateInstance(fullClassName);
                CacheAccess.SaveToCacheByDependency(className, bllObject, fileDependency);
            }

            return bllObject;
        }
    }
}
