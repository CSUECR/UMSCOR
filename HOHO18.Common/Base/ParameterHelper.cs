using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.Collections;

namespace HOHO18.Common.Base
{
    public sealed class ParameterHelper
    {
        /// <summary>
        /// 判断参数是否空
        /// </summary>
        /// <param name="canShu">参数</param>
        /// <returns>返回布尔型,空(true)非空(false)</returns>
        public static bool IsNullOrEmpty(object canShu)
        {
            try
            {
                if ((null == canShu) || "" == canShu.ToString().Trim() || "".Equals(canShu.ToString().Trim()))
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// 判断参数是否空
        /// </summary>
        /// <param name="canShu">判断时间是否为空</param>
        /// <returns>返回布尔型,空(true)非空(false)</returns>
        public static bool IsNullOrEmpty(DateTime canShu)
        {
            try
            {
                if (null == canShu)
                {
                    return true;
                }
                if (canShu.ToString().Equals("0001-01-01 0:00:00"))
                {
                    return true;
                }
                if (canShu.ToString("yyyy-MM-dd").Equals("0001-01-01"))
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// 判断参数是否空
        /// </summary>
        /// <param name="canShu">判断时间是否为空</param>
        /// <returns>返回布尔型,空(true)非空(false)</returns>
        public static bool IsNullOrEmpty(Guid canShu)
        {
            try
            {
                if (null == canShu)
                {
                    return true;
                }
                if (canShu==Guid.Empty)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return true;
            }
        }
    }
}
