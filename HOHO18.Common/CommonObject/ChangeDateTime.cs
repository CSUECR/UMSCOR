using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HOHO18.Common
{
    public sealed class ChangeDateTime
    {
        /// <summary>
        /// 把秒转换为几天几时。。
        /// </summary>
        /// <param name="second">秒</param>
        /// <returns></returns>
        public static string SecondToString(long second)
        {
            long temp = second;
            StringBuilder sp = new StringBuilder();
            if (temp / 86400 > 0)
            {
                sp.Append((temp / 86400).ToString() + "天");
                temp = temp % 86400;
            }
            if (temp / 3600 > 0)
            {
                sp.Append((temp / 3600).ToString() + "小时");
                temp = temp % 3600;
            }
            if (temp / 60 > 0)
            {
                sp.Append((temp / 60).ToString() + "分钟");
                temp = temp % 60;
            }
            if (temp != 0) sp.Append(temp.ToString() + "秒");
            return sp.ToString();
        }
        /// <summary>
        /// 把发表时间改成几个月,几天前,几小时前,几分钟前,或几秒前 
        /// </summary>
        /// <param name="dt">时间</param>
        /// <returns></returns>
        public static string DateStringFromNow(DateTime dt)
        {
            TimeSpan span = DateTime.Now - dt;
            if (span.TotalDays > 60)
            {
                return dt.ToShortDateString();
            }
            else
                if (span.TotalDays > 30)
                {
                    return
                    "1个月前";
                }
                else
                    if (span.TotalDays > 14)
                    {
                        return
                        "2周前";
                    }
                    else
                        if (span.TotalDays > 7)
                        {
                            return
                            "1周前";
                        }
                        else
                            if (span.TotalDays > 1)
                            {
                                return
                                string.Format("{0}天前", (int)Math.Floor(span.TotalDays));
                            }
                            else
                                if (span.TotalHours > 1)
                                {
                                    return
                                    string.Format("{0}小时前", (int)Math.Floor(span.TotalHours));
                                }
                                else
                                    if (span.TotalMinutes > 1)
                                    {
                                        return
                                        string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
                                    }
                                    else
                                        if (span.TotalSeconds >= 1)
                                        {
                                            return
                                            string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds));
                                        }
                                        else
                                        {
                                            return
                                            "1秒前";
                                        }
        }

        /// <summary>
        /// 把秒转换成分钟
        /// </summary>
        /// <returns></returns>
        public static int SecondToMinute(int Second)
        {
            decimal mm = (decimal)((decimal)Second / (decimal)60);
            return Convert.ToInt32(Math.Ceiling(mm));
        }       

        #region 返回时间差
        public static string DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            string dateDiff = null;
            try
            {
                //TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                //TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                //TimeSpan ts = ts1.Subtract(ts2).Duration();
                TimeSpan ts = DateTime2 - DateTime1;
                if (ts.Days >= 1)
                {
                    dateDiff = DateTime1.Month.ToString() + "月" + DateTime1.Day.ToString() + "日";
                }
                else
                {
                    if (ts.Hours > 1)
                    {
                        dateDiff = ts.Hours.ToString() + "小时前";
                    }
                    else
                    {
                        dateDiff = ts.Minutes.ToString() + "分钟前";
                    }
                }
            }
            catch
            { }
            return dateDiff;
        }
        #endregion

        #region DateTime时间格式转换为Unix时间戳格式
        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>double</returns>  
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            int intResult = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            intResult = Convert.ToInt32((time - startTime).TotalSeconds);
            return intResult;
        }
        #endregion
    }
}
