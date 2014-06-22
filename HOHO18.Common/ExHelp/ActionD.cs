using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class ActionD
    {
        /// <summary>
        /// 做一些处理，返回对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T DoAction<T>(this T t, Action<T> action)
        {
            action(t);
            return t;
        }
    }
}
