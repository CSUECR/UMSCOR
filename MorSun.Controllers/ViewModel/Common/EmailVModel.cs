using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;

namespace MorSun.Controllers.ViewModel
{
    public class EmailVModel : BaseVModel<wmfMail>
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        public override IQueryable<wmfMail> List
        {
            get
            {
                var l = All;
                if (FlagTrashed == "1")
                {
                    l = l.Where(n => n.FlagTrashed == true);
                }
                else if (FlagTrashed == "0")
                {
                    l = l.Where(n => n.FlagTrashed == false);
                }
                if (MailCategory != null&&MailCategory!=Guid.Empty)
                {
                    l = l.Where(n => n.MailCategory == MailCategory);
                    if (MailCategory != Guid.Parse(MorSun.Common.类别.Reference.邮箱栏目_草稿箱))
                    {
                        l = l.Where(p => p.RequestUser != null);
                    }
                }
                if (RequestUser!=null&&RequestUser!=Guid.Empty)
                {
                    
                    if (FlagTrashed == "1")
                    {
                        l = l.Where(n => n.RequestUser == RequestUser||n.Receiver==RequestUser);
                    }
                    else
                    {
                        l = l.Where(n => n.RequestUser == RequestUser);
                    }
                }
                if (isRead!=null)
                {
                    l = l.Where(n => n.IsRead == isRead);
                }
                if (!string.IsNullOrEmpty(ReceiveUser))
                {
                    var ID = Guid.Parse(MorSun.Common.类别.Reference.邮箱栏目_草稿箱);
                    var receiveID = Guid.Parse(ReceiveUser);
                    l = l.Where(n => n.MailCategory!=ID);
                    l = l.Where(n => n.Receiver == receiveID);
                }
                if (FlagTrashed == "true")
                {
                    l = l.Where(n => n.FlagTrashed == true);
                }
                var dateFrom=DateHelp.ToDateFrom(DateTime.Now.ToShortDateString());
                var dateTo = DateHelp.ToDateTo(DateTime.Now.ToShortDateString());
                todayDateFrom = dateFrom;
                todayDateTo = dateTo;
                var tempFrom = dateFrom.AddDays(-1);
                var tempTo = dateTo.AddDays(-1);
                yesterdayDateFrom = tempFrom;
                yesterdayDateTo = tempTo;
                tempFrom = dateFrom.AddDays(-2);
                tempTo = dateTo.AddDays(-2);
                threeDateFrom = tempFrom;
                threeDateTo = tempTo;
                tempFrom = dateFrom.AddDays(-3);
                tempTo = dateTo.AddDays(-3);
                fourDateFrom = tempFrom;
                fourDateTo = tempTo;
                tempFrom = dateFrom.AddDays(-4);
                tempTo = dateTo.AddDays(-4);
                fiveDateFrom = tempFrom;
                fiveDateTo = tempTo;
                tempFrom = dateFrom.AddDays(-5);
                tempTo = dateTo.AddDays(-5);
                sixDateFrom = tempFrom;
                sixDateTo = tempTo;
                tempFrom = dateFrom.AddDays(-6);
                tempTo = dateTo.AddDays(-6);
                sevenDateFrom = tempFrom;
                sevenDateTo = tempTo;
                tempFrom = dateTo.AddDays(-7);
                tempTo = dateFrom.AddDays(-14);
                lastWeekDateFrom = tempFrom;
                lastWeekDateTo = tempTo;
                tempFrom = dateFrom.AddDays(-14);
                tempTo = dateTo.AddDays(-14);
                earlierDateFrom = tempFrom;
                earlierDateTo = tempTo;
                //排序
                l = l.OrderByDescending(n => n.RegTime);
                //邮件总数量及未读邮件总数量
                if (!string.IsNullOrEmpty(ReceiveUser))
                {
                   mailAllNum=l.Count().ToString();
                   mailNoReadNum = l.Count(p=>p.IsRead==false).ToString();
                }
                if (MailCategory == Guid.Parse(MorSun.Common.类别.Reference.邮箱栏目_草稿箱))
                {
                    mailAllNum = l.Count().ToString();
                    mailNoReadNum = l.Count(p => p.IsRead == false).ToString();
                }
                if (FlagTrashed == "1")
                {
                    mailAllNum = l.Count().ToString();
                    mailNoReadNum = l.Count(p => p.IsRead == false).ToString();
                }
                if (MailCategory == Guid.Parse(MorSun.Common.类别.Reference.邮箱栏目_发件箱))
                {
                    mailAllNum = l.Count().ToString();
                    mailNoReadNum = l.Count(p => p.IsRead == false).ToString();
                }
                return l;
            }
        }

        /// <summary>
        /// 被选中的编号
        /// </summary>
        public virtual string CheckedId { get; set; }

        public virtual string CurrentUrl { get; set; }
        //邮件总数
        public virtual string mailAllNum { get; set; }
        //邮件未读总数
        public virtual string mailNoReadNum { get; set; }

        //删除标记
        public virtual string FlagDeleted { get; set; }

        //回收站标记
        public virtual string FlagTrashed { get; set; }
        //收箱类别
        public virtual Guid MailCategory { get; set; }
        //收件人
        public virtual string ReceiveUser { get; set; }
        //发件人
        public virtual Guid RequestUser { get; set; }

        public virtual bool? isRead { get; set; }
        //今天
        public virtual string todayNum { get; set; }
        //昨天
        public virtual string yesterdayNum { get; set; }
        //星期
        public virtual string threeNum { get; set; }
        //星期
        public virtual string fourNum { get; set; }
        //星期
        public virtual string fiveNum { get; set; }
        //星期
        public virtual string sixNum { get; set; }
        //星期
        public virtual string sevenNum { get; set; }
        //上周
        public virtual string lastWeekNum { get; set; }
        //更早
        public virtual string earlierNum { get; set; }

        //今天
        public virtual DateTime todayDateFrom { get; set; }
        public virtual DateTime todayDateTo { get; set; }
        //昨天
        public virtual DateTime yesterdayDateFrom { get; set; }
        public virtual DateTime yesterdayDateTo { get; set; }
        //星期
        public virtual DateTime threeDateFrom { get; set; }
        public virtual DateTime threeDateTo { get; set; }
        //星期
        public virtual DateTime fourDateFrom { get; set; }
        public virtual DateTime fourDateTo { get; set; }
        //星期
        public virtual DateTime fiveDateFrom { get; set; }
        public virtual DateTime fiveDateTo { get; set; }
        //星期
        public virtual DateTime sixDateFrom { get; set; }
        public virtual DateTime sixDateTo { get; set; }
        //星期
        public virtual DateTime sevenDateFrom { get; set; }
        public virtual DateTime sevenDateTo { get; set; }
        //上周
        public virtual DateTime lastWeekDateFrom { get; set; }
        public virtual DateTime lastWeekDateTo { get; set; }
        //更早
        public virtual DateTime earlierDateFrom { get; set; }
        public virtual DateTime earlierDateTo { get; set; }

        public virtual bool showFlag { get; set; }
        public virtual bool yesterdayFlag { get; set; }
        public virtual bool threeFlag { get; set; }
        public virtual bool fourFlag { get; set; }
        public virtual bool fiveFlag { get; set; }
        public virtual bool sixFlag { get; set; }
        public virtual bool sevenFlag { get; set; }
        public virtual bool lastWeekFlag { get; set; }
        public virtual bool earlierFlag { get; set; }
    }
}
