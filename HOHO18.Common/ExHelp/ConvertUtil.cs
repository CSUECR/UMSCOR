using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Linq.Expressions;

namespace HOHO18.Common.ExHelp
{
    public static class ConvertUtil
    {
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TValue To<TValue>(this object value)
        {
            var type = typeof(TValue);
            var val = To(type, value);
            var v = (TValue)val;
            return v;
        }

        /// <summary>
        /// 转换类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object To(Type type, object value)
        {
            object v = null;
            if (value != null)
            {
                var vstr = value.ToString();
                if (!string.IsNullOrEmpty(vstr))
                {
                    if (type.IsGenericType)
                    {
                        type = type.GetGenericArguments()[0];
                    }
                    v = Convert.ChangeType(value, type);
                }
            }
            if (v == null && type.IsValueType)
            {
                var ary = Array.CreateInstance(type, 1);
                v = ary.GetValue(0);
                //v = type.InvokeMember(null, BindingFlags.CreateInstance,
                //    null, null, null);
            }
            return v;
        }

        /// <summary>
        /// 不区分大小写的比较
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static bool Eql(this string str1, string str2)
        {
            var result = str1 == str2;
            if (!result && str1 != null)
            {
                result = str1.Equals(str2, StringComparison.OrdinalIgnoreCase);
            }
            return result;
        }

        /// <summary>
        /// 判断对象是否是集合类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsList(this object obj)
        {
            var result = false;
            if (obj != null)
            {
                var type = obj.GetType();
                if (type != typeof(string))
                {
                    var iname = typeof(IEnumerable).FullName;
                    var listType = type.GetInterface(iname);
                    if (listType != null)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 如果异常，自动调用下一个表达式
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="vs"></param>
        /// <returns></returns>
        public static V TryOr<V>(params Func<V>[] vs)
        {
            foreach (var v in vs)
            {
                try
                {
                    return v();
                }
                catch { }
            }
            return default(V);
        }

        ///// <summary>
        ///// 如果异常，自动调用下一个表达式
        ///// </summary>
        ///// <typeparam name="V"></typeparam>
        ///// <param name="defaultV"></param>
        ///// <param name="vs"></param>
        ///// <returns></returns>
        //public static V TryOr<V>(V defaultV, params Func<V>[] vs)
        //{
        //    foreach (var v in vs)
        //    {
        //        try
        //        {
        //            return v();
        //        }
        //        catch { }
        //    }
        //    return defaultV;
        //}
    }
}
