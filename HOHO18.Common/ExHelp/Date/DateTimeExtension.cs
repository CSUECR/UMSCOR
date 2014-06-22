using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 对可空的DateTime进行格式化输出
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToShortDateString(this DateTime? dt, string format = "yyyy/MM/dd")
        {
            var rtn = string.Empty;
            if (dt != null)
            {
                rtn = dt.Value.ToString(format);
            }
            return rtn;
        }
    }
}
