using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace System
{
    /// <summary>
    /// 对可能引发null异常的处理操作
    /// </summary>
    public static class NollHelp
    {


        /// <summary>
        /// 执行操作后返回本身
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T Self<T>(this T t, Action<T> action = null)
        {
            if (action != null)
            {
                action(t);
            }
            return t;
        }

        /// <summary>
        /// 逐一尝试funcs列表，返回第一个未发生异常的结果或者默认值defualt(T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcs"></param>
        /// <returns></returns>
        public static T TryOr<T>(this bool b, params Func<T>[] funcs)
        {
            var result = default(T);
            foreach (var func in funcs)
            {
                try
                {
                    result = func();
                    break;
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// 使用非空判定调用表达式结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static NollResult<T> Noll<T>(this Expression<Func<T>> expr)
        {
            var caller = Noller<T>.Get(expr);
            var rs = caller.Call(expr);
            return rs;
        }

        /// <summary>
        /// 获取非空判定呃表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Noller<T> GetNoller<T>(this Expression<Func<T>> expr)
        {
            var caller = Noller<T>.Get(expr);
            return caller;
        }

        /// <summary>
        ///如果在表达式链条中存在null可能则使用下一个表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exprs"></param>
        /// <returns></returns>
        public static T Noll<T>(this bool b, params Func<T>[] exprs)
        {
            var result = default(T);
            foreach (var func in exprs)
            {
                try
                {
                    result = func();
                    break;
                }
                catch { }
            }
            return result;

            //var result = default(T);
            //foreach (var expr in exprs)
            //{
            //    var rs = expr.Noll();
            //    if (rs.NoNull)
            //    {
            //        result = rs.Value;
            //        break;
            //    }
            //}

            //return result;
        }

        /// <summary>
        /// 如果空的话调用new()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T Noll<T>(this T t)
            where T : new()
        {
            if (t == null)
            {
                t = new T();
            }
            return t;
        }

        /// <summary>
        /// 字符串如果为空的话调用str的值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Noll(this string str)
        {
            return str ?? "";
        }

        ///// <summary>
        ///// 遍历每一个表达式，如果表达式中将出现null异常，则使用下一个表达式
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="b"></param>
        ///// <param name="exprs"></param>
        ///// <returns></returns>
        //public static T Noll<T>(this bool b, params Func<T>[] exprs)
        //{
        //    var result = default(T);

        //    foreach (var expr in exprs)
        //    {
        //        try
        //        {
        //            result = expr();
        //            break;
        //        }
        //        catch { }
        //    }

        //    return result;
        //}
    }
    
}
