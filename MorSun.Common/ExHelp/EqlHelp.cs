using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using MorSun.Common;
using MorSun.Model;

namespace System
{
    /// <summary>
    /// 比较
    /// </summary>
    public static class EqlHelp
    {
        /// <summary>
        /// 比较是否相等
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="v"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool Eql<V>(this V v, V v2)
        {
            var result = false;

            //if 可能相等
            if (!(v == null && v2 != null) ||
                !(v != null && v2 == null))
            {
                //可能相等的情况
                if (v == null && v2 == null ||
                    object.ReferenceEquals(v, v2) ||
                    object.Equals(v, v2))
                {
                    result = true;
                }
            }
            return result;
        }

        ////迁移到UserCenter时删除，无此表，YOU
        //public static string Equal<V>(this V v, V v2, out string originalContent, out string afterOperateContent)
        //{
        //    originalContent = string.Empty;
        //    afterOperateContent = string.Empty;
        //    Type sourceType = v.GetType();
        //    Type destinationType = v2.GetType();
        //    GenericEFDao<MorSunEntities, FC_ENTITYSUB> Bll = new GenericEFDao<MorSunEntities, FC_ENTITYSUB>();
        //    IQueryable<FC_ENTITYSUB> query = Bll.All.Where(p => p.tbname == sourceType.Name);
        //    if (sourceType == destinationType)
        //    {
        //        PropertyInfo[] sourceProperties = sourceType.GetProperties();
        //        foreach (var pi in sourceProperties)
        //        {
        //            if ((sourceType.GetProperty(pi.Name).GetValue(v, null) == null && destinationType.GetProperty(pi.Name).GetValue(v2, null) == null))
        //            {

        //            }
        //            else if (sourceType.GetProperty(pi.Name).GetValue(v, null) == null || sourceType.GetProperty(pi.Name).GetValue(v, null).Equals(v) || destinationType.GetProperty(pi.Name).GetValue(v, null) == null || destinationType.GetProperty(pi.Name).GetValue(v, null).Equals(v))
        //            {

        //            }

        //            else if (!(((sourceType.GetProperty(pi.Name).GetValue(v2, null) == null) ? "" : sourceType.GetProperty(pi.Name).GetValue(v, null).ToString()) == ((destinationType.GetProperty(pi.Name).GetValue(v2, null) == null) ? "" : destinationType.GetProperty(pi.Name).GetValue(v2, null).ToString())))
        //            {
        //                if (pi.Name.IndexOf("EntityState") == -1 && pi.Name.IndexOf("ApplicationId") == -1 && pi.Name.IndexOf("AutoGeneticId") == -1 && pi.Name.IndexOf("ID") == -1)
        //                {
        //                    var Entify = query.Where(p => p.fdname == pi.Name).FirstOrDefault();
        //                    var name = string.Empty;

        //                    if (Entify != null)
        //                    {
        //                        name = Entify.chnname;
        //                    }
        //                    afterOperateContent += name + ":" + sourceType.GetProperty(pi.Name).GetValue(v, null).ToString() + ".\r\n";
        //                    originalContent += name + ":" + destinationType.GetProperty(pi.Name).GetValue(v2, null).ToString() + ".\r\n";
        //                }
        //            }
        //        }
        //        if (string.IsNullOrEmpty(originalContent) && string.IsNullOrEmpty(afterOperateContent))
        //        {
        //            return "false";
        //        }
        //        return "true";
        //    }
        //    else
        //    {
        //        throw new ArgumentException("Comparison object must be of the same type.", "comparisonObject");
        //    }
        //}

        /// <summary>
        /// 当条件成立时为value否则为defaut(V)
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="b"></param>
        /// <param name="test"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static V Then<V>(this bool b, bool test, V value)
        {
            var result = default(V);
            if (test)
            {
                result = value;
            }
            return result;
        }
    }
   
}
