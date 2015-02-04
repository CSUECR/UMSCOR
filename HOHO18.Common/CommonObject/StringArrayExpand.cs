using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class StringArrayExpand
    {

        /// <summary>
        /// 和javascript 的String.join一样的功能,还多了个appendStrArr功能
        /// </summary>
        /// <param name="str">原字符串数组</param>
        /// <param name="splitStr">分割字符串</param>
        /// <param name="appendStrArr">后面要多加的字符串数组</param>
        /// <returns></returns>
        public static String JoinArray(this String[] str,String splitStr,params String[] appendStrArr)
        {
            return JoinArray(str, splitStr, appendStrArr);
        }

        /// <summary>
        /// 和javascript 的String.join一样的功能,还多了个appendStrArr功能
        /// </summary>
        /// <param name="str">原字符串数组</param>
        /// <param name="splitStr">分割字符串</param>
        /// <param name="appendStrArr">后面要多加的字符串数组</param>
        /// <returns></returns>
        public static String JoinArray(this IEnumerable<String> str, String splitStr, params String[] appendStrArr)
        {
            StringBuilder reStr = new StringBuilder();

            foreach (var t in str)
            {
                reStr.Append(String.IsNullOrEmpty(t) ? splitStr : t + splitStr);
            }
            if (appendStrArr != null)
            {
                for (int i = 0; i < appendStrArr.Length; i++)
                {
                    String t = appendStrArr[i];
                    reStr.Append(String.IsNullOrEmpty(t) ? splitStr : t + splitStr);
                }
            }
            if (reStr.Length > 0) reStr.Remove(reStr.Length - splitStr.Length, splitStr.Length);
            return reStr.ToString();
        }

        /// <summary>
        /// GUID 字符串转换为GUID LIST
        /// </summary>
        /// <param name="str"></param>
        /// <param name="splitStr"></param>
        /// <returns></returns>
        public static List<Guid> ToGuidList(this String str, String splitStr)
        {
            if (String.IsNullOrEmpty(splitStr))
            {
                splitStr = ",";
            }
            String[] sArray = str.Split(splitStr.ToArray(), StringSplitOptions.RemoveEmptyEntries);

            List<Guid> newGuidList = new List<Guid>();
            
            foreach (var s in sArray)
            {
                try
                {
                    newGuidList.Add(new Guid(s));
                }
                catch
                {
                }                
            }
            return newGuidList;
        }
    }
}
