using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastReflectionLib;

namespace System
{
    /// <summary>
    /// 类型帮助器
    /// </summary>
    public static class TypeHelp
    {
        /// <summary>
        /// 获取Nullable中值的类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetNullableIn(this Type type)
        {
            //typeof(int?).GetNullableIn()==typeof(int)
            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                //获取Nullable<>中值的类型
                type = type.GetGenericArguments().First();
            }
            return type;
        }

        /// <summary>
        /// 获取type类型的默认值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object DefaultValue(this Type type)
        {
            //typeof(int).DefaultValue()==0==default(int)
            var ary = Array.CreateInstance(type, 1);
            var result = ary.GetValue(0);
            return result;
        }

        /// <summary>
        /// 获取type类型的默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T DefaultValue<T>(this Type type)
        {
            return default(T);
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static T New<T>(this Type type, params object[] pars)
        {
            //typeof(Student).New<Student>("Nimei")== new Studnet("Nimei");
            var result = default(T);

            var parTypes = Type.GetTypeArray(pars);

            var newInfo = type.GetConstructor(parTypes);

            if (newInfo != null)
            {
                result =(T)newInfo.FastInvoke(pars);
            }
            else if (!type.IsValueType)
            {
                throw new ArgumentException();
            }

            return result;
        }


        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static object New(this Type type, params object[] pars)
        {
            object result = null;

            var parTypes = Type.GetTypeArray(pars);

            var newInfo = type.GetConstructor(parTypes);

            if (newInfo != null)
            {
                result = newInfo.FastInvoke(pars);
            }

            else if (type.IsValueType)
            {
                result = type.DefaultValue();
            }
            else
            {
                throw new ArgumentException();
            }

            return result;
        }
    }
}
