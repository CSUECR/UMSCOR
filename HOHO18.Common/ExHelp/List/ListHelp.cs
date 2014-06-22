using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using FastReflectionLib;
using HOHO18.Common.Helper;

namespace System
{
    public static class ListHelp
    {
        /// <summary>
        /// 格式化输出对应multiSelect控件的json数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="idPropetyName">对应的id属性名</param>
        /// <param name="namePropetyName">对应的名称字段属性名称</param>
        /// <param name="categoryPropetyName">类别字段属性名称默认可为空，当为空是多选控件的下拉列表也为null</param>
        /// <returns></returns>
        public static string ToPinYinJsonString<T>(this IEnumerable<T> enumerable, string idPropetyName, string namePropetyName, string categoryPropetyName="")
           where T : class
        {
            if (string.IsNullOrEmpty(idPropetyName) || string.IsNullOrEmpty(namePropetyName))
                throw new ArgumentNullException("idPropetyName,namePropetyName为null");
            var json = new StringBuilder();
            foreach (var item in enumerable)
            {
                var id = item.GetType().GetProperty(idPropetyName).GetValue(item, null);
                var name = item.GetType().GetProperty(namePropetyName).GetValue(item, null);
                object category=null;
                ///不为空时，获取类别的值。
                if(!string.IsNullOrEmpty(categoryPropetyName))
                    category = item.GetType().GetProperty(categoryPropetyName).GetValue(item, null);

                var nameToQuanPinYin = PinYin.QuanPinGo(name as string);
                var nameToFirstUppercase = PinYin.FirstLetterGo(name as string);

                //生成uid，real_name，real_unsafe ,type（也就是下拉框的类别）
                var traineeStr = string.Format("{{\"uid\":\"{0}\",\"real_name\":[\"{1}\",\"{2}\",\"{3}\"],\"real_name_unsafe\":\"{4}\",\"type\":\"{5}\"}},"
                    , id, name, nameToQuanPinYin, nameToFirstUppercase, name, category);

                json.Append(traineeStr);

            }

            if (json.Length>1 && enumerable.GetEnumerator() != null)
            {
                json.Remove(json.Length - 1, 1);
            }
            return string.Format("[{0}]", json);
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="pindex">当前页码</param>
        /// <param name="psize">每页条数</param>
        /// <returns></returns>
        public static IQueryable<T> PLimit<T>(
            this IQueryable<T> src, int pindex = 1, int psize = 20)
        {
            return src.Skip((pindex - 1) * psize).Take(psize);
        }

        /// <summary>
        /// 判断是否为空
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool IsWhite(this IEnumerable items, Func<object, bool> test = null)
        {
            var result = items == null;
            if (!result)
            {
                if (test == null)
                {
                    test = obj => obj != null;
                }
                foreach (var item in items)
                {
                    if (test(item))
                    {
                        return result;
                    }
                }
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 判断是否为空
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        /// <summary>
        /// 判断集合是否为空
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool IsEmpty(this IEnumerable items)
        {
            var result = items == null;
            if (!result)
            {
                foreach (var item in items)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 创建T类型的List集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static List<T> NewList<T>(this bool b, params T[] pars)
        {
            return new List<T>(pars);
        }

        /// <summary>
        /// 提取T集合中的元素类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static T VarOne<T>(IEnumerable<T> ts)
        {
            return default(T);
        }

        /// <summary>
        /// 遍历每一个
        /// </summary>
        /// <param name="items"></param>
        /// <param name="action"></param>
        public static void Each(this IEnumerable items, Action<object> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        /// <summary>
        /// 遍历每一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
        public static void Each<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        /// <summary>
        /// 查询items的位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int IndexOf<T>(this IEnumerable<T> items, T item)
        {
            var index = 0;
            foreach (var itm in items)
            {
                if (itm == null && item == null ||
                    item != null && item.Equals(item))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        /// <summary>
        /// 查询满足条件的位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="test"></param>
        /// <returns></returns>
        public static int IndexOf<T>(this IEnumerable<T> items, Func<T, bool> test)
        {
            var index = 0;
            foreach (var itm in items)
            {
                if (test(itm))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }
    }
}
