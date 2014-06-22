using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace System
{
    public static class TreeHelp
    {

#if !NoExHelp2010
        /// <summary>
        /// 获取根节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <returns></returns>
        public static IQueryable<T> Roots<T>(this IQueryable<T> src)
        {
            var treeExpr = Expression.Parameter(typeof(T));

            var parentProp = typeof(T).GetProperty("ParentId");

            Expression<Func<T, bool>> test = Expression.Lambda<Func<T, bool>>(
                Expression.Equal(
                    Expression.Property(treeExpr, parentProp),
                    Expression.Convert(Expression.Constant(null), parentProp.PropertyType)
                ),
                treeExpr);

            return src.Where(test);
        }
#endif
    }
}
