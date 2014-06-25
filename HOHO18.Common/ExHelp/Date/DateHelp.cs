using HOHO18.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 日期帮助器
    /// </summary>
    public static class DateHelp
    {
        /// <summary>
        /// 转换为日期
        /// </summary>
        /// <param name="str"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public static DateTime ToDate(this string str, bool err = false)
        {
            //"2010-01-18".ToDate();
            //"2010-01-18".ToAs<DateTime>();

            var date = default(DateTime);
            var isDate = DateTime.TryParse(str ?? "", out date);
            if (!isDate && !err)
            {
                throw new InvalidCastException();
            }
            return date;
        }

        /// <summary>
        /// 转换为查询开始时间
        /// </summary>
        /// <param name="str">时间字符串</param>
        /// <param name="err">是否验证错误</param>
        /// <returns></returns>
        public static DateTime ToDateFrom(this string str, bool err = false)
        {
            //2011-04-28 00:00:00
            str += " 00:00:00 ";
            var date = default(DateTime);
            var isDate = DateTime.TryParse(str ?? "", out date);
            if (!isDate && !err)
            {
                throw new InvalidCastException();
            }
            return date;
        }

        /// <summary>
        /// 转换为查询结止时间
        /// </summary>
        /// <param name="str">时间字符串</param>
        /// <param name="err">是否验证错误</param>
        /// <returns></returns>
        public static DateTime ToDateTo(this string str, bool err = false)
        {
            //2011-04-28 23:59:59
            str += " 23:59:59 ";
            var date = default(DateTime);
            var isDate = DateTime.TryParse(str ?? "", out date);
            if (!isDate && !err)
            {
                throw new InvalidCastException();
            }
            return date;
        }

        /// <summary>
        /// 转换为日期
        /// </summary>
        /// <param name="str"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public static DateTime? ToDateN(this string str, bool err = false)
        {
            var result = default(DateTime?);
            var date = default(DateTime);
            var isDate = DateTime.TryParse(str ?? "", out date);
            if (isDate)
            {
                result = date;
            }
            else if(err)
            {
                throw new InvalidCastException();
            }
            return result;
        }

        /// <summary>
        /// 获取星期的中文表示
        /// </summary>
        /// <param name="date"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string WDay(this DateTime date, string format = "{0}")
        {
            var text = "日";
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    text = "一";
                    break;
                case DayOfWeek.Tuesday:
                    text = "二";
                    break;
                case DayOfWeek.Wednesday:
                    text = "三";
                    break;
                case DayOfWeek.Thursday:
                    text = "四";
                    break;
                case DayOfWeek.Friday:
                    text = "五";
                    break;
                case DayOfWeek.Saturday:
                    text = "六";
                    break;
                default:
                    text = "日";
                    break;
            }
            return string.Format(format, text);
        }
        //月最后一天
        public static DateTime MaxDay(this DateTime date)
        {
            var ret = default(DateTime);
            var iDay = DateTime.DaysInMonth(date.Year, date.Month);
            ret = new DateTime(date.Year, date.Month, iDay);
            return ret;
        }
        //月第一天
        public static DateTime MinDay(this DateTime date)
        {
            var ret = default(DateTime);
            ret= new DateTime(date.Year, date.Month, 1);
            return ret;
        }

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

        /// <summary>
        /// 返回几小时前、几分钟前
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ByNow(this DateTime dt)
        {
            return ChangeDateTime.DateStringFromNow(dt);
        }
    }
}
