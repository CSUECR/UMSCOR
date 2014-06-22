using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace System
{
    /// <summary>
    /// 对象盒子辅助工具
    /// </summary>
    public static class VBoxHelp
    {

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static VBox<T> New<T>(T value)
        {
            return new VBox<T>(value);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static VBox<T> New<T>(Func<T> expr)
        {
            return new VBox<T>(expr);
        }

        /// <summary>
        /// 加载,可用于懒加载语法简写，例如：box=box.Load(()=>{return 1;});
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="box"></param>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static VBox<T> Load<T>(this VBox<T> box, Func<T> expr)
        {
            return box ?? new VBox<T>(expr);
        }

        /// <summary>
        /// 加载,可用于懒加载语法简写，例如：box=box.Load(1);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="box"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static VBox<T> Load<T>(this VBox<T> box, T value)
        {
            return box ?? value;
        }

        /// <summary>
        /// 将value转换为OBoxT类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static VBox<T> AsVBox<T>(this T value)
        {
            return new VBox<T>(value);
        }

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v"></param>
        /// <param name="defaultV"></param>
        /// <returns></returns>
        public static T TryValue<T>(this T? v, T defaultV = default(T))
            where T : struct
        {
            return v.GetValueOrDefault(defaultV);
        }

        /// <summary>
        /// 装化为可为空的类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v"></param>
        /// <returns></returns>
        public static T? AsN<T>(this T v)
            where T : struct
        {
            return v;
        }

    }
}
