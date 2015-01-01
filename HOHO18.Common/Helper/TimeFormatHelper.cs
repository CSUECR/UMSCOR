using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HOHO18.Common.Helper
{
    /// <summary>
    /// 对时间进行一些特殊的格式化输出(是自带函数里没有的)
    /// </summary>
    public class TimeFormatHelper
    {
        private static string[] Day = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };

        /// <summary>
        /// 根据时间显示离当前的时间,用最大单位表示(约),如:3秒前,3分钟前,3小时前,
        /// (从"天"级开始后面就会加上具体日期)3天前2010-1-1,3月前2010-3-1...
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static String ShowLatelyFormat(DateTime time)
        {
            TimeSpan spTime = DateTime.Now.Date.Subtract(time.Date);
            if (spTime.TotalDays > 0)
            {
                //先判断是年级别的还是月级别的,最后才是天级别的
                double day = spTime.TotalDays;
                if (day > 365) return time.Year.ToString().Substring(2) + "-" + time.Month.ToString() + "-" + (time.Day.ToString().Length < 2 ? "0" + time.Day.ToString() : time.Day.ToString());//((int)(day / 365)).ToString() + "年前" +
                else if (day > 30) return time.Year.ToString().Substring(2) + "-" + time.Month.ToString() + "-" + (time.Day.ToString().Length < 2 ? "0" + time.Day.ToString() : time.Day.ToString());//((int)(day / 30)).ToString() + "月前" +
                else if (day == 1) return "昨天";
                else if (day == 2) return "前天";
                else return day.ToString() + "天前";// + time.Year.ToString().Substring(2) + "-" + time.Month.ToString() + "-" + time.Day.ToString();
            }
            else
            {
                spTime = DateTime.Now.Subtract(time);
                if (spTime.TotalHours > 1)
                {
                    return ((int)spTime.TotalHours).ToString() + "小时前";
                }
                else if (spTime.TotalMinutes > 1)
                {
                    return ((int)spTime.TotalMinutes).ToString() + "分钟前";
                }
                else if (spTime.TotalSeconds > 0)
                {
                    return ((int)spTime.TotalSeconds).ToString() + "秒前";
                }
            }
            return "刚才";
        }

        /// <summary>
        /// 返回时间差
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static string ShowDiffTime(DateTime time)
        {
            string strResout = "{0}{1}前";
            //获得2时间的时间间隔秒计算  
            TimeSpan span = DateTime.Now - time;
            int iTatol = Convert.ToInt32(span.TotalSeconds);

            int iMinutes = 1 * 60;
            int iHours = iMinutes * 60;
            int iDay = iHours * 24;
            int iMonth = iDay * 30;
            int iYear = iMonth * 12;

            if (iTatol > iYear)
            {
                return string.Format(strResout, iTatol / iYear, "年");
            }
            else if (iTatol > iMonth)
            {
                return string.Format(strResout, iTatol / iMonth, "月");
            }
            else if (iTatol > iDay)
            {
                return string.Format(strResout, iTatol / iDay, "天");
            }
            else if (iTatol > iHours)
            {
                return string.Format(strResout, iTatol / iHours, "小时");
            }
            else if (iTatol > iMinutes)
            {
                return string.Format(strResout, iTatol / iMinutes, "分钟");
            }
            else
            {
                return string.Format(strResout, iTatol, "秒");
            }

        }

        /// <summary>
        /// 返回时间差
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static string ShowDiffTime(DateTime begtime,DateTime endtime)
        {
            string strResout = string.Empty;
            TimeSpan span = endtime - begtime;
            if (span.Days > 0)
            {
                strResout = span.Days.ToString() + "天" + span.Hours.ToString() + "小时";
            }
            else
            {
                strResout = span.Hours.ToString() + "小时" + span.Minutes.ToString() + "分钟";
            }
            return strResout;
        }
  


        /// <summary>
        /// 时间格式为2011年12月20日 (星期二)下午2点48分
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ShowWholeDate(DateTime time)
        {
            return  time.ToString("yyyy年M月d日(" + Day[Convert.ToInt16(time.DayOfWeek)] + ") "+GetTimeScale(time)+"h:m");
        }

        /// <summary>
        /// 获取时间段如：早上或下午或中午等
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetTimeScale(DateTime time)
        { 
             var h = time.Hour;
             if (h >= 0 && h <= 5) { return "凌晨"; }
             else if (h > 5 && h <= 8) { return "早晨"; }
             else if (h > 8 && h <= 13) { return "上午"; }
             else if (h > 13 && h <= 18) { return "下午"; }
             else if (h > 18 && h <= 24) { return "晚上"; }
             else return "";
        }

        /// <summary>
        /// 根据天数,返回这个天数的最大单位:如:10天,半个月,1个月,2个月
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static String ShowDayMaxUnitFormat(int day)
        {
            switch (day)
            {
                case 15:
                    return "半个月";
            }
            if (day > 29 && day % 30 == 0)
            {
                return (day / 30).ToString() + "个月";
            }
            return day.ToString() + "天";
        }

        #region 其他时间格式
        /// <summary>
        /// 今天
        /// </summary>   
        public static string Today() { return DateTime.Now.Date.ToShortDateString(); }
        /// <summary>
        /// 昨天,也就是今天的日期减一
        /// </summary>
        public static string Yesterday() { return DateTime.Now.AddDays(-1).ToShortDateString(); }
        /// <summary>
        ///明天,同理,加一  
        /// </summary>
        public static string Tomorrow() { return DateTime.Now.AddDays(1).ToShortDateString(); }

        /// <summary>
        /// 本周日(要知道本周第一天就得先知道今天是星期几,从而得知本周第一天就是几天前的那一天;每一周是从周日始至周六止[0-6])
        /// </summary>  
        public static string ChinaWeekFirstDay() { return DateTime.Now.AddDays(Convert.ToDouble((0 - Convert.ToInt16(DateTime.Now.DayOfWeek) + 1))).ToShortDateString(); }
        /// <summary>
        /// 本周六(要知道本周最后一天就得先知道今天是星期几,从而得知本周最后一天就是几天前的那一天;每一周是从周日始至周六止[0-6])
        /// </summary>   
        public static string ChinaWeekLastDay() { return DateTime.Now.AddDays(Convert.ToDouble((6 - Convert.ToInt16(DateTime.Now.DayOfWeek) + 1))).ToShortDateString(); }


        /// <summary>
        /// 本周日(要知道本周第一天就得先知道今天是星期几,从而得知本周第一天就是几天前的那一天;每一周是从周日始至周六止[0-6])
        /// </summary>  
        public static string ThisWeekFirstDay() { return DateTime.Now.AddDays(Convert.ToDouble((0 - Convert.ToInt16(DateTime.Now.DayOfWeek)))).ToShortDateString(); }
        /// <summary>
        /// 本周六(要知道本周最后一天就得先知道今天是星期几,从而得知本周最后一天就是几天前的那一天;每一周是从周日始至周六止[0-6])
        /// </summary>   
        public static string ThisWeekLastDay() { return DateTime.Now.AddDays(Convert.ToDouble((6 - Convert.ToInt16(DateTime.Now.DayOfWeek)))).ToShortDateString(); }

        /// <summary>
        /// 中文显示星期几   
        /// 由于DayOfWeek返回的是数字的星期几,需要把它转换成汉字以方便人们阅读,有些人会用SWITCH来一个一个地对照,其实我们有更优的策略
        /// </summary>
        /// <returns></returns>  
        public static string DayOfWeekToChinese()
        {
            return Day[Convert.ToInt16(DateTime.Now.DayOfWeek)];//使用它来获取
        }

        /// <summary>
        /// 上周日(要知道上周第一天就得先知道今天是星期几,从而得知上周第一天就是几天前的那一天;每一周是从周日始至周六止[0-6])
        /// </summary>   
        public static string LastWeekFirstDay() { return DateTime.Now.AddDays(Convert.ToDouble((0 - Convert.ToInt16(DateTime.Now.DayOfWeek))) - 7).ToShortDateString(); }
        /// <summary>
        /// 上周六(要知道上周最后一天就得先知道今天是星期几,从而得知上周最后一天就是几天前的那一天;每一周是从周日始至周六止[0-6])
        /// </summary>
        /// <returns></returns>
        public static string LastWeekLastDay() { return DateTime.Now.AddDays(Convert.ToDouble((6 - Convert.ToInt16(DateTime.Now.DayOfWeek))) - 7).ToShortDateString(); }
        /// <summary>
        /// 下周日(要知道下周第一天就得先知道今天是星期几,从而得知下周第一天就是几天前的那一天;每一周是从周日始至周六止[0-6])
        /// </summary>   
        public static string NextWeekFirstDay() { return DateTime.Now.AddDays(Convert.ToDouble((0 - Convert.ToInt16(DateTime.Now.DayOfWeek))) + 7).ToShortDateString(); }
        /// <summary>
        /// 下周六(要知道下周最后一天就得先知道今天是星期几,从而得知下周最后一天就是几天前的那一天;每一周是从周日始至周六止[0-6])
        /// </summary>
        /// <returns></returns>
        public static string NextWeekLastDay() { return DateTime.Now.AddDays(Convert.ToDouble((6 - Convert.ToInt16(DateTime.Now.DayOfWeek))) + 7).ToShortDateString(); }
        /// <summary>
        /// 本月,本月的第一天是1号,最后一天就是下个月的1号再减一天 
        /// </summary>  
        public static string ThisMonthFirstDay() { return DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-01"; }//第一天   
        /// <summary>
        /// 本月最后一天
        /// </summary>
        /// <returns></returns>
        public static string ThisMontyLastDay() { return DateTime.Parse(DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + "1").AddMonths(1).AddDays(-1).ToShortDateString(); }
        //最后一天   
        //巧用C#里ToString的字符格式化更简便   
        //DateTime.Now.ToString("yyyy-MM-01");  
        //DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1).ToShortDateString();  

        /// <summary>
        /// 上个月头一天,减去一个月份
        /// </summary>   
        public static string LastMonthFirstDay() { return DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(-1).ToShortDateString(); }
        /// <summary>
        /// 上个月最后一天
        /// </summary>
        /// <returns></returns>
        public static string LastMonthLastDay() { return DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-1).ToShortDateString(); }
        /// <summary>
        /// 下个月头一天,加上一个月份
        /// </summary>   
        public static string NextMonthFirstDay() { return DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(1).ToShortDateString(); }
        /// <summary>
        /// 下个月最后一天
        /// </summary>
        /// <returns></returns>
        public static string NextMonthLastDay() { return DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(2).AddDays(-1).ToShortDateString(); }

        /// <summary>
        /// 7天后   
        /// DateTime.Now.Date.ToShortDateString();
        /// </summary>  
        public static string DaysLaterOf7() { return DateTime.Now.AddDays(7).ToShortDateString(); }
        /// <summary>
        /// 7天前
        /// </summary>   
        public static string DaysBefore7() { return DateTime.Now.AddDays(-7).ToShortDateString(); }

        /// <summary>
        /// DateTime.Now.Date.ToShortDateString();  
        /// 本年度头一天,用ToString的字符格式化我们也很容易算出本年度的第一天和最后一天
        /// </summary>   
        public static string ThisYearFirstDay() { return DateTime.Parse(DateTime.Now.ToString("yyyy-01-01")).ToShortDateString(); }
        /// <summary>
        /// 本年度最后一天
        /// </summary>
        /// <returns></returns>
        public static string ThisYearLastDay() { return DateTime.Parse(DateTime.Now.ToString("yyyy-01-01")).AddYears(1).AddDays(-1).ToShortDateString(); }
        /// <summary>
        /// 上年度头一天
        /// </summary>   
        public static string LastYearFirstDay() { return DateTime.Parse(DateTime.Now.ToString("yyyy-01-01")).AddYears(-1).ToShortDateString(); }
        /// <summary>
        /// 上年度最后一天
        /// </summary>
        /// <returns></returns>
        public static string LastYearLastDay() { return DateTime.Parse(DateTime.Now.ToString("yyyy-01-01")).AddDays(-1).ToShortDateString(); }
        /// <summary>
        /// 下年度头一天
        /// </summary>   
        public static string NextYearFirstDay() { return DateTime.Parse(DateTime.Now.ToString("yyyy-01-01")).AddYears(1).ToShortDateString(); }
        /// <summary>
        /// 下年度最后一天
        /// </summary>
        /// <returns></returns>
        public static string NextYearLastDay() { return DateTime.Parse(DateTime.Now.ToString("yyyy-01-01")).AddYears(2).AddDays(-1).ToShortDateString(); }

        /// <summary>
        /// 本季度头一天，很多人都会觉得这里是难点,需要写个长长的过程来判断;其实不用的，我们都知道一年四个季度,一个季度三个月
        /// 首先我们把日期推到本季度第一个月,然后这个月的第一天就是本季度的第一天了   
        /// </summary>
        public static string ThisQuarterFirstDay() { return DateTime.Now.AddMonths(0 - ((DateTime.Now.Month - 1) % 3)).ToString("yyyy-MM-01"); }
        /// <summary>
        /// 同理,本季度的最后一天就是下个季度的第一天减一
        /// </summary>   
        public static string ThisQuarterLastDay() { return DateTime.Parse(DateTime.Now.AddMonths(3 - ((DateTime.Now.Month - 1) % 3)).ToString("yyyy-MM-01")).AddDays(-1).ToShortDateString(); }
        /// <summary>
        /// 下季度头一天
        /// </summary>   
        public static string NextQuarterFirstDay() { return DateTime.Now.AddMonths(3 - ((DateTime.Now.Month - 1) % 3)).ToString("yyyy-MM-01"); }
        /// <summary>
        /// 下季度最后一天
        /// </summary>
        /// <returns></returns>
        public static string NextQuarterLastDay() { return DateTime.Parse(DateTime.Now.AddMonths(6 - ((DateTime.Now.Month - 1) % 3)).ToString("yyyy-MM-01")).AddDays(-1).ToShortDateString(); }
        /// <summary>
        /// 上季度头一天
        /// </summary>   
        public static string LastQuarterFirstDay() { return DateTime.Now.AddMonths(-3 - ((DateTime.Now.Month - 1) % 3)).ToString("yyyy-MM-01"); }
        /// <summary>
        /// 上季度最后一天
        /// </summary>
        /// <returns></returns>
        public static string LastQuarterLastDay() { return DateTime.Parse(DateTime.Now.AddMonths(0 - ((DateTime.Now.Month - 1) % 3)).ToString("yyyy-MM-01")).AddDays(-1).ToShortDateString(); }
        /// <summary>
        /// 获得当月有多少天
        /// </summary>   
        public static int DaysInMonth() { return System.DateTime.DaysInMonth(System.DateTime.Now.Year, System.DateTime.Now.Month); }

        /// <summary>
        /// 获取某月的实际工作日(即不包括周六日)   
        /// int days=getDays(System.DateTime.Now);调用即可获得
        /// </summary>
        /// <param name="date1"></param>
        /// <returns></returns>   
        public static int GetDays(System.DateTime date1)
        {
            int m = System.DateTime.DaysInMonth(date1.Year, date1.Month);
            int mm = 0;
            for (int i = 1; i <= m; i++)
            {
                System.DateTime date = Convert.ToDateTime(date1.Year + "-" + date1.Month + "-" + i);
                switch (date.DayOfWeek)
                {
                    case System.DayOfWeek.Monday:
                    case System.DayOfWeek.Tuesday:
                    case System.DayOfWeek.Wednesday:
                    case System.DayOfWeek.Thursday:
                    case System.DayOfWeek.Friday:
                        mm = mm + 1;
                        break;
                }
            }
            return mm;
        }
        //
        public static int GetTimeZone()
        {
            DateTime now = DateTime.Now;
            var utcnow = now.ToUniversalTime();

            var sp = now - utcnow;

            return sp.Hours;
        }
        public static string ToLocalTime(DateTime dt)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));

            return "new Date(" + (dt.Ticks - startTime.Ticks) / 10000 + ")";
        }

        /// <summary>
        /// 获得任意两天的有效工作日
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public static int GetWordDays(System.DateTime date1, System.DateTime date2)
        {
            TimeSpan d1 = new TimeSpan(date1.Ticks);
            TimeSpan d2 = new TimeSpan(date2.Ticks);
            TimeSpan dd = d1.Subtract(d2).Duration();
            string m = dd.Days.ToString();//Microsoft.VisualBasic.DateAndTime.DateDiff(EnumDateCompare.day,date1,date2).ToString("f0");  
            int mm = 0;
            for (int i = 0; i <= Convert.ToInt32(m); i++)
            {
                System.DateTime date = Convert.ToDateTime(date1.AddDays(i));
                switch (date.DayOfWeek)
                {
                    case System.DayOfWeek.Monday:
                    case System.DayOfWeek.Tuesday:
                    case System.DayOfWeek.Wednesday:
                    case System.DayOfWeek.Thursday:
                    case System.DayOfWeek.Friday:
                        mm = mm + 1;
                        break;
                }
            }
            return mm;
        }
        //原来的有VB代码不能用
        //获得任意两日期之间的有效工作日(不包括周六日)   
        //DateTime date1=Convert.ToDateTime("2008-8-8");  
        //DateTime date2=Convert.ToDateTime("2008-10-1");  
        //int days = getDays(date1,date2);  
        //调用如上即可获得
        //private int getDays(System.DateTime date1,System.DateTime date2)  
        //{  
        //string m=Microsoft.VisualBasic.DateAndTime.DateDiff(EnumDateCompare.day,date1,date2).ToString("f0");  
        //int mm=0;  
        //for(int i=0;i<=Convert.ToInt32(m);i++)  
        //{  
        //System.DateTime date=Convert.ToDateTime(date1.AddDays(i));  
        //switch(date.DayOfWeek)  
        //{  
        //case System.DayOfWeek.Monday:  
        //case System.DayOfWeek.Tuesday:  
        //case System.DayOfWeek.Wednesday:  
        //case System.DayOfWeek.Thursday:  
        //case System.DayOfWeek.Friday:  
        //mm=mm+1;  
        //break;  
        //}  
        //}  
        //return mm;  
        //}  
        ////格式输出   
        //private void Page_Load(object sender,System.EventArgs e)  
        //{  
        //System.Globalization.DateTimeFormatInfo myDTFI=new System.Globalization.CultureInfo("en-US",false).DateTimeFormat;  
        ////中国为zh-cn   
        //DateTime myDT=System.DateTime.Now;  
        //Response.Write(myDT.ToString("f",myDTFI));  


        /// <summary>
        /// 获得本周的周六和周日
        /// </summary>
        /// <param name="date"></param>
        /// <param name="firstdate"></param>
        /// <param name="lastdate"></param>
        public static void ConvertDateToWeek(DateTime date, out DateTime firstdate, out DateTime lastdate)
        {
            DateTime first = System.DateTime.Now;
            DateTime last = System.DateTime.Now;
            switch (date.DayOfWeek)
            {
                case System.DayOfWeek.Monday:
                    first = date.AddDays(-1);
                    last = date.AddDays(5);
                    break;
                case System.DayOfWeek.Tuesday:
                    first = date.AddDays(-2);
                    last = date.AddDays(4);
                    break;
                case System.DayOfWeek.Wednesday:
                    first = date.AddDays(-3);
                    last = date.AddDays(3);
                    break;
                case System.DayOfWeek.Thursday:
                    first = date.AddDays(-4);
                    last = date.AddDays(2);
                    break;
                case System.DayOfWeek.Friday:
                    first = date.AddDays(-5);
                    last = date.AddDays(1);
                    break;
                case System.DayOfWeek.Saturday:
                    first = date.AddDays(-6);
                    last = date;
                    break;
                case System.DayOfWeek.Sunday:
                    first = date;
                    last = date.AddDays(6);
                    break;
            }
            firstdate = first;
            lastdate = last;
        }
        //调用   
        //DateTime firstdate=System.DateTime.Now;  
        //DateTime lastdate=System.DateTime.Now; 
        //DateTime date = DateTime.Now;
        //ConvertDateToWeek(date,out firstdate,out lastdate);  
        //获得当前日期是该年度的第几周   
        //DateTime dt=Convert.ToDateTime("2008-8-8");  
        //int weeks=dt.DayOfYear/7+1;  
        #endregion

        #region 公历转农历
         //天干 
         private static string[] TianGan = { "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸" };
         //地支 
         private static string[] DiZhi = { "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };
         //十二生肖 
         private static string[] ShengXiao = { "鼠", "牛", "虎", "兔", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" }; 
         //农历日期 
         private static string[] DayName =   {
              "*","初一","初二","初三","初四","初五",
              "初六","初七","初八","初九","初十",
              "十一","十二","十三","十四","十五", 
              "十六","十七","十八","十九","二十",
              "廿一","廿二","廿三","廿四","廿五",
              "廿六","廿七","廿八","廿九","三十"}; 
         //农历月份
         private static string[] MonthName = { "*", "正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "腊" }; 
         //公历月计数天 
         private static int[] MonthAdd = { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334 };
         //农历数据
         private static int[] LunarData = {2635,333387,1701,1748,267701,694,2391,133423,1175,396438
             ,3402,3749,331177,1453,694,201326,2350,465197,3221,3402
             ,400202,2901,1386,267611,605,2349,137515,2709,464533,1738
             ,2901,330421,1242,2651,199255,1323,529706,3733,1706,398762
             ,2741,1206,267438,2647,1318,204070,3477,461653,1386,2413 
             ,330077,1197,2637,268877,3365,531109,2900,2922,398042,2395
             ,1179,267415,2635,661067,1701,1748,398772,2742,2391,330031
             ,1175,1611,200010,3749,527717,1452,2742,332397,2350,3222 
             ,268949,3402,3493,133973,1386,464219,605,2349,334123,2709
             ,2890,267946,2773,592565,1210,2651,395863,1323,2707,265877};  

         /// <summary>
         /// 获取对应日期的农历
         /// </summary> 
         /// <param name="dtDay">公历日期</param>
         /// <returns></returns>
         public static string GetLunarCalendar(DateTime dtDay)
         {
             string sYear = dtDay.Year.ToString();
             string sMonth = dtDay.Month.ToString();
             string sDay = dtDay.Day.ToString();
             int year;
             int month;
             int day;
             try
             {
                 year = int.Parse(sYear);
                 month = int.Parse(sMonth);
                 day = int.Parse(sDay);
             }
             catch
             {
                 year = DateTime.Now.Year;
                 month = DateTime.Now.Month;
                 day = DateTime.Now.Day;
             }
             int nTheDate;
             int nIsEnd;
             int k, m, n, nBit, i;
             string calendar = string.Empty;
             //计算到初始时间1921年2月8日的天数：1921-2-8(正月初一)
             nTheDate = (year - 1921) * 365 + (year - 1921) / 4 + day + MonthAdd[month - 1] - 38;
             if ((year % 4 == 0) && (month > 2))
                 nTheDate += 1;
             //计算天干，地支，月，日
             nIsEnd = 0;
             m = 0;
             k = 0;
             n = 0;
             while (nIsEnd != 1)
             {
                 if (LunarData[m] < 4095)
                     k = 11;
                 else
                     k = 12;
                 n = k;
                 while (n >= 0)
                 {
                     //获取LunarData[m]的第n个二进制位的值 
                     nBit = LunarData[m];
                     for (i = 1; i < n + 1; i++)
                         nBit = nBit / 2;
                     nBit = nBit % 2;
                     if (nTheDate <= (29 + nBit))
                     {
                         nIsEnd = 1;
                         break;
                     }

                     nTheDate = nTheDate - 29 - nBit;
                     n = n - 1;
                 }
                 if (nIsEnd == 1)
                     break;
                 m = m + 1;
             }

             year = 1921 + m;
             month = k - n + 1;
             day = nTheDate;

             //return year+"-"+month+"-"+day;    

             // 格式化日期显示为三月廿四 
             if (k == 12)
             {
                 if (month == LunarData[m] / 65536 + 1)
                     month = 1 - month;
                 else if (month > LunarData[m] / 65536 + 1)
                     month = month - 1;
             }
             //生肖
             calendar = ShengXiao[(year - 4) % 60 % 12].ToString() + "年 ";
             //天干 
             calendar += TianGan[(year - 4) % 60 % 10].ToString();
             //地支
             calendar += DiZhi[(year - 4) % 60 % 12].ToString() + " ";
             //农历月 
             if (month < 1)
                 calendar += "闰" + MonthName[-1 * month].ToString() + "月";
             else
                 calendar += MonthName[month].ToString() + "月";
             //农历日 
             calendar += DayName[day].ToString() + "日";
             return calendar;
         }  
        #endregion

        /// <summary>
        /// 传一个日期及增加的天数，返回增加后的工作日日期
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static DateTime AddWorkDay(DateTime dt, decimal n)
        {
            DateTime temp = dt;
            while (n-- > 0)
            {
                temp = temp.AddDays(1);
                while (temp.DayOfWeek == System.DayOfWeek.Saturday || temp.DayOfWeek == System.DayOfWeek.Sunday)
                    temp = temp.AddDays(1);
            }
            return temp;
        }

    }
}
