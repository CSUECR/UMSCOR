using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;



namespace System
{
    /// <summary>
    /// 表达式帮助
    /// </summary>
    public static class ReflHelp
    {

        
        //属性检测
        #region prop

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="propSeler"></param>
        /// <returns></returns>
        public static PropertyInfo PropOf<P>(this bool b, Expression<Func<P>> propSeler)
        {
            var propExpr = (MemberExpression)propSeler.Body;
            return (PropertyInfo)propExpr.Member;

        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="propSeler"></param>
        /// <returns></returns>
        public static PropertyInfo PropOf<T, P>(this bool b, Expression<Func<T, P>> propSeler)
        {

            var propExpr = (MemberExpression)propSeler.Body;
            return (PropertyInfo)propExpr.Member;
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="propSeler"></param>
        /// <returns></returns>
        public static MethodInfo PropGetOf<T, P>(this bool b, Expression<Func<T, P>> propSeler)
        {

            var propExpr = (MemberExpression)propSeler.Body;
            var prop = (PropertyInfo)propExpr.Member;

            return prop.GetGetMethod();
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="propSeler"></param>
        /// <returns></returns>
        public static MethodInfo PropSetOf<T, P>(this bool b, Expression<Func<T, P>> propSeler)
        {

            var propExpr = (MemberExpression)propSeler.Body;
            var prop = (PropertyInfo)propExpr.Member;

            return prop.GetSetMethod();
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="propSeler"></param>
        /// <returns></returns>
        public static FieldInfo FieldOf<P>(this bool b, Expression<Func<P>> propSeler)
        {
            var propExpr = (MemberExpression)propSeler.Body;
            return (FieldInfo)propExpr.Member;
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="propSeler"></param>
        /// <returns></returns>
        public static FieldInfo FieldOf<T, P>(this bool b, Expression<Func<T, P>> propSeler)
        {

            var propExpr = (MemberExpression)propSeler.Body;
            return (FieldInfo)propExpr.Member;
        }

        

        #endregion

        //通过表达式获取方法
        #region mehod

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static MethodInfo MethodOf(this bool b, Action func)
        {
            return func.Method;
        }

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static MethodInfo MethodOf<T>(this bool b, Action<T> func)
        {
            return func.Method;
        }

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static MethodInfo MethodOf<T, T2>(this bool b, Action<T, T2> func)
        {
            return func.Method;
        }

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static MethodInfo MethodOf<T, T2, T3>(this bool b, Action<T, T2, T3> func)
        {
            return func.Method;
        }
        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static MethodInfo MethodOf<T, T2, T3, T4>(this bool b, Action<T, T2, T3, T4> func)
        {
            return func.Method;
        }

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static MethodInfo MethodOf<T>(this bool b, Func<T> func)
        {
            return func.Method;
        }

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static MethodInfo MethodOf<T, T2>(this bool b, Func<T, T2> func)
        {
            return func.Method;
        }

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static MethodInfo MethodOf<T, T2, T3>(this bool b, Func<T, T2, T3> func)
        {
            return func.Method;
        }
        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static MethodInfo MethodOf<T, T2, T3, T4>(Func<T, T2, T3, T4> func)
        {
            return func.Method;
        }

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static MethodInfo MethodOf<T, T2, T3, T4, T5>(this bool b, Func<T, T2, T3, T4, T5> func)
        {
            return func.Method;
        }

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static MethodInfo MethodOf<S, T>(this bool b, Expression<Func<S, Func<T>>> expr)
        {
            //(Func)s.ToString
            var convertExpr = expr.Body as UnaryExpression;

            var createExpr = convertExpr.Operand as MethodCallExpression;

            var methodExpr = createExpr.Arguments[2] as ConstantExpression;

            return methodExpr.Value as MethodInfo;
        }

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static MethodInfo MethodOf<S, T, T2>(this bool b, Expression<Func<S, Func<T, T2>>> expr)
        {
            //(Func)s.ToString
            var convertExpr = expr.Body as UnaryExpression;

            var createExpr = convertExpr.Operand as MethodCallExpression;

            var methodExpr = createExpr.Arguments[2] as ConstantExpression;

            return methodExpr.Value as MethodInfo;
        }

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static MethodInfo MethodOf<S, T, T2, T3>(this bool b, Expression<Func<S, Func<T, T2, T3>>> expr)
        {
            //(Func)s.ToString
            var convertExpr = expr.Body as UnaryExpression;

            var createExpr = convertExpr.Operand as MethodCallExpression;

            var methodExpr = createExpr.Arguments[2] as ConstantExpression;

            return methodExpr.Value as MethodInfo;
        }

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static MethodInfo MethodOf<S, T, T2, T3, T4>(this bool b, Expression<Func<S, Func<T, T2, T3, T4>>> expr)
        {
            //(Func)s.ToString
            var convertExpr = expr.Body as UnaryExpression;

            var createExpr = convertExpr.Operand as MethodCallExpression;

            var methodExpr = createExpr.Arguments[2] as ConstantExpression;

            return methodExpr.Value as MethodInfo;
        }

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static MethodInfo MethodOf<S, T, T2, T3, T4, T5>(this bool b, Expression<Func<S, Func<T, T2, T3, T4, T5>>> expr)
        {
            //(Func)s.ToString
            var convertExpr = expr.Body as UnaryExpression;

            var createExpr = convertExpr.Operand as MethodCallExpression;

            var methodExpr = createExpr.Arguments[2] as ConstantExpression;

            return methodExpr.Value as MethodInfo;
        }


        /// <summary>
        /// 获取方法
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static MethodInfo MethodOf<S, T, T2, T3, T4>(this bool b, Expression<Func<S, Action<T, T2, T3, T4>>> expr)
        {
            //(Func)s.ToString
            var convertExpr = expr.Body as UnaryExpression;

            var createExpr = convertExpr.Operand as MethodCallExpression;

            var methodExpr = createExpr.Arguments[2] as ConstantExpression;

            return methodExpr.Value as MethodInfo;
        }

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static MethodInfo MethodOf<S, T, T2>(this bool b, Expression<Func<S, Action<T, T2>>> expr)
        {
            //(Func)s.ToString
            var convertExpr = expr.Body as UnaryExpression;

            var createExpr = convertExpr.Operand as MethodCallExpression;

            var methodExpr = createExpr.Arguments[2] as ConstantExpression;

            return methodExpr.Value as MethodInfo;
        }

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static MethodInfo MethodOf<S, T>(this bool b, Expression<Func<S, Action<T>>> expr)
        {
            //(Func)s.ToString
            var convertExpr = expr.Body as UnaryExpression;

            var createExpr = convertExpr.Operand as MethodCallExpression;

            var methodExpr = createExpr.Arguments[2] as ConstantExpression;

            return methodExpr.Value as MethodInfo;
        }

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static MethodInfo MethodOf<S>(this bool b, Expression<Func<S, Action>> expr)
        {
            //(Func)s.ToString
            var convertExpr = expr.Body as UnaryExpression;

            var createExpr = convertExpr.Operand as MethodCallExpression;

            var methodExpr = createExpr.Arguments[2] as ConstantExpression;

            return methodExpr.Value as MethodInfo;
        }

        #endregion


    }
}
