using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class GuidExtension
    {
        /// <summary>
        /// 返回安全的字符串（null的处理）
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string ToSecureString(this Guid? guid)
        {
            var result = string.Empty;
            if (guid != null && guid != Guid.Empty)
            {
                result = guid.ToString();
            }
            return result;
        }
    }
}
