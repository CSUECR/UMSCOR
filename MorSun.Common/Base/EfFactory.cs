using System;
using System.Data.Objects;
using System.Web;
using MorSun.Model;
using System.Threading;

namespace MorSun.Common
{

    /// <summary>
    /// MorSunEntities的上下文工厂
    /// </summary>
    public class TheEf : EfFactory<MorSunEntities>
    {

    }

    /// <summary>
    /// Ef上下文工厂
    /// </summary>
    /// <typeparam name="TEntities"></typeparam>
    public class EfFactory<TEntities>
        where TEntities : ObjectContext, new()
    {

        //static object sy = new object();

        [ThreadStatic]
        static TEntities _entities;


        /// <summary>
        /// 获取当前请求中的Ef上下文
        /// </summary>
        public static TEntities Entities
        {
            get
            {
                TEntities entities = null;
                if (HttpContext.Current != null)
                {
                    entities = HttpContext.Current.Items[typeof(TEntities).Name] as TEntities;
                }
                else
                {
                    entities = _entities;
                }

                if (entities == null)
                {
                    Entities = entities = new TEntities();
                }

                return entities;
            }

            set
            {
                //如果web环境，使用web缓存
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[typeof(TEntities).Name] = value;

                    ////清除缓存
                    //if (HttpContext.Current != null && HttpContext.Current.ApplicationInstance != null)
                    //{
                    //    HttpContext.Current.ApplicationInstance.EndRequest += (ss, ee) =>
                    //    {
                    //        HttpContext.Current.Items.Remove(typeof(TEntities).Name);
                    //    };
                    //}

                }
                else
                {
                    //使用线程缓存
                    _entities = value;
                }
            }
        }

        
        
    }
}
