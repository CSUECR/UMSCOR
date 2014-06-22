using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace System
{
    /// <summary>
    /// 加载器
    /// </summary>
    public static class LoadHelp
    {
        /// <summary>
        /// 加载loader
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="v"></param>
        /// <param name="loader"></param>
        /// <param name="nullV"></param>
        /// <returns></returns>
        public static V Load<V>(this V v, Func<V> loader, V nullV = default(V))
        {
            if (v.Eql(nullV))
            {
                v = loader();
            }
            return v;
        }

        /// <summary>
        /// 使用new加载
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="v"></param>
        /// <param name="nullV"></param>
        /// <returns></returns>
        public static V Load<V>(this V v, V nullV = default(V))
            where V : new()
        {
            return Load(v, () => new V(), nullV);
        }

        /// <summary>
        /// 加载字典中的对象,如果不存在，使用loader创建后添加到字典
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="loader"></param>
        /// <returns></returns>
        public static V Load<K, V>(this IDictionary<K, V> dict, K key, Func<V> loader = null)
        {
            V value = default(V);
            var exists = dict.TryGetValue(key, out value);
            if (!exists)
            {
                if (loader != null)
                {
                    value = loader();
                }
                dict[key] = value;
            }
            return value;
        }

        /// <summary>
        /// 载字典中的对象,如果不存在，使用loader创建后添加到字典
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="loader"></param>
        /// <returns></returns>
        public static object Load(this IDictionary dict, object key, Func<object> loader = null)
        {

            object value = null;
            var exists = dict.Contains(key);
            if (!exists)
            {
                if (loader != null)
                {
                    value = loader();
                }
                dict[key] = value;
            }
            else
            {
                value = dict[key];
            }
            return value;


        }
        
    }
}
