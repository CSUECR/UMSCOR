#define debug
#undef original
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using HOHO18.Common.Model;
using MorSun.Bll;
using MorSun.Common.ExHelp;
using System.Web.Mvc;
using MorSun.Controllers.ViewModel;
using MorSun.Controllers.Quartz.CheckingIn;
using MorSun.Controllers.Quartz;
using HOHO18.Common.Web;
using System.Web;
using System.Data;
using HOHO18.Common.Base;
using HOHO18.Common;
using MorSun.Controllers.Filter;
using MorSun.Common.类别;
using MorSun.Bll.Service;
using MorSun.Common.Privelege;

namespace MorSun.Controllers.CommonController
{
    public class KQController : BaseController<kqPort>
    {
        protected override string ResourceId
        {
            get { return MorSun.Common.Privelege.资源.考勤机; }
        }

        private int iMachineNumber = 1;
        private bool bIsConnected = false;

        /// <summary>
        /// 判断用户考勤必须唯一
        /// </summary>
        /// <param name="userKqRecordList"></param>
        /// <param name="newkqId"></param>
        /// <returns></returns>
        private bool IsExistUserKq(List<kqRecord> userKqRecordList, Guid newkqId)
        {
            var falg = true;
            foreach (var userKqRecord in userKqRecordList)
            {
                if (userKqRecord.ID == newkqId)
                {
                    falg = false;
                    break;
                }
            }
            return falg;
        }

        /// <summary>
        /// 读取用户指定时间段考勤记录
        /// 个人考勤功能如下：
        /// #1、按日历显示个人考勤数据（员工可以看本月之前的考勤数据，不能看今天以后的考勤）
        /// #2、注意免签（考勤）的员工直接忽略，不要进行下面的步骤。页面上体现免签。
        /// #3、系统先查看员工排班情况，未排清楚要先补全记录。访问员工考勤页面时，当查看到员工相关日期之内还没有排班记录，先补全排班记录（生成排班表的功能后面讲），调用生成排班表的功能。记录补全完之后才开始过滤的功能。
        /// #4、从班次类型对应表里取出应该要打卡的时间，来决定考勤记录显示在日历上的个数（晨曦是显示3次，值班的下午班下午可以不打卡，值班完打卡），有异常的用红色链接字体来标识，用户可以点击链接进行后续操作。异常记录包括迟到、未打卡
        /// #5、从班次类型表里取出打卡的有效时间，上班时间，来对考勤记录进行过滤
        /// #6、日历上今天（包括今天）之前显示的是员工的考勤记录，今天之后显示的是排班表记录
        /// #7、先对迟到进行统计，把每次迟到的分钟数列出来，最后再统计总的迟到次数以及总的迟到时间，放在日历的左下角。请假、外出后面来做。
        /// #8、查看排班表的按钮（可以查看员工每个月的排班信息，有记录的都看得到）
        /// #9、写备注的功能（分为员工写备注，人事经理写备注）都是点击日历上的日期空白部分，
        ///    弹出一个框[本人可以修改员工备注，人事经理可以修改经理备注（注意人事经理看自己的考勤）]，
        ///    填写限制50个字内的备注（已经有备注的则把原来的备注填在文本框里），
        ///    写完之后，数据保存到数据库，日历上无刷新更新备注。
        ///    注意：本人备注只能写在员工备注字段，人事经理（不是本人并且有编辑排班表权限）写的备注保存到经理备注。
        ///    日历的左下角把这个月的人事经理备注都列出来，好统计。
        ///    日历上人事经理的备注用红色字体表示，包括左下角的备注
        /// #10、早退，统计早退
        /// #11、用户可以查看当天的所有打卡记录
        /// </summary>
        /// <param name="userIdStr">用户ID</param>
        /// <param name="yearMonth">要查询的年月</param>
        /// <param name="endtime">截至时间</param>
        /// <returns></returns>
        public string GetUserKQJson(Guid? userIdStr, string yearMonth)
        {
            var cdStr = string.Empty;

            DateTime begintime = DateTime.Parse(yearMonth);
            DateTime endtime = begintime.AddMonths(1).AddSeconds(-1);

            //YearMonth = begintime;
            var userId = UserID;
            string krStr = string.Empty;
            if (userIdStr != null && userIdStr != Guid.Empty)
            {
                userId = Guid.Parse(userIdStr.ToString());
            }

            var 备注 = XmlHelper.GetPagesString<kqClassPlan>("备注");
            var 经理备注 = XmlHelper.GetPagesString<kqClassPlan>("经理备注");
            var userInfoBll = new wmfUserInfoBll();
            var userInfo = userInfoBll.GetModel(userId);
            if (userInfo != null)
            {
                if (!string.IsNullOrEmpty(userInfo.CheckNumber))
                {
                    var yearJia = "无入职时间";
                    var newYearJia = "无入职时间";
                    var stuJia = "无入职时间";
                    var newStuJia = "无入职时间";
                    if (userInfo.DatesEmployed != null)
                    {
                        //原来的假期
                        var DatesEmployed = DateTime.Parse(userInfo.DatesEmployed.ToString());
                        yearJia = MorSun.Controllers.UserController.GetYearLeave(DatesEmployed).ToString("f1");
                        stuJia = MorSun.Controllers.UserController.GetStudentLeave(DatesEmployed).ToString("f1");

                        //剩下的假期
                        newYearJia = userInfo.WinterVacation.ToString("f1");
                        newStuJia = userInfo.StudyLeave.ToString("f1");
                    }

                    //去向信息
                    var personnelToModel = new wmfPersonnelToVModel();

                    //去向 开始时间
                    personnelToModel.PersonnelToBeginTime = begintime;

                    //去向 结束时间
                    personnelToModel.PersonnelToEndTime = endtime;

                    //所有审核通过的记录
                    personnelToModel.IsAudit = true;

                    //去向信息
                    var personnelToList = personnelToModel.AnalysisList.Where(p => p.UserId == userId && p.ToName != "行政登记");

                    //是否免签考勤
                    var IsNoCheck = userInfo.IsNoCheck;

                    //分析后的考勤记录
                    List<kqRecord> userKqRecordList = new List<kqRecord>();

                    //是否免签考勤
                    if (IsNoCheck)
                    {
                        var cday = DateTime.Now - begintime;
                        for (int j = 0; j <= cday.Days; j++)
                        {
                            //免打卡
                            var userkq = new kqRecord();
                            userkq.ID = Guid.NewGuid();
                            userkq.RecordTime = begintime.AddDays(j);
                            userkq.ClassesName = "";// kqclassesref.ClassesName + " 上班";
                            userkq.kqRecordTitle = "免签";
                            userkq.Stauts = "免签";
                            userKqRecordList.Add(userkq);
                        }
                    }
                    else
                    {
                        //如果用户还没有排班表则 自动通过用户GUID生成排班表
                        //测试字符串
                        //var userclassPlanStr = "{ \"ErrorMessage\": \"错误信息内容\", \"URL\": \"/index\", \"Type\": \"error\" }";
                        var userclassPlanStr = new MorSun.Controllers.UserDefaultScheduleTemplatesController().AutoGenerateScheduleTableByUserID(begintime, userId, false);
                        if (userclassPlanStr != "true")
                        {
                            return "[" + userclassPlanStr + "]";
                        }

                        //根据时间段获取考勤信息-
                        List<kqRecord> krList = null;
                        //员工有考勤机ID，取出相应的考勤记录
                        if (userInfo.RollMachine != null && userInfo.RollMachine != Guid.Empty)
                        {
                            krList = new KRVModel().All.Where(r => r.CId == userInfo.CheckNumber && r.kqPort.ID == userInfo.RollMachine && r.RecordTime >= begintime && r.RecordTime <= endtime).OrderBy(r => r.RecordTime).ToList();
                        }
                        //员工没有考勤机ID，则取出全部。
                        else
                        {
                            krList = new KRVModel().All.Where(r => r.CId == userInfo.CheckNumber && r.RecordTime >= begintime && r.RecordTime <= endtime).OrderBy(r => r.RecordTime).ToList();
                        }

                        //排班表
                        var kqClassPlanbll = new BaseBll<kqClassPlan>();
                        //获取该用户指定时间段内的班次
                        var userkqClassPlanList = kqClassPlanbll.All.Where(p => p.UserId == userId && p.PlanDate >= begintime && p.PlanDate < endtime).OrderBy(p => p.Sort);
                        if (userkqClassPlanList != null && userkqClassPlanList.Count() > 0)
                        {
                            //班次类型对应表
                            var kqClassesRelationbll = new BaseBll<kqClassesRelation>();
                            var kqClassesRelationList = kqClassesRelationbll.All.Where(k => k.FlagTrashed == false);

                            var kqClassesSequencebll = new BaseBll<kqClassesSequence>();
                            var kqClassesSequenceList = kqClassesSequencebll.All.Where(k => k.FlagTrashed == false);

                            //用户每天的班次
                            foreach (var userkqClassPlan in userkqClassPlanList)
                            {
                                //班次时间
                                var planDate = userkqClassPlan.PlanDate;

                                //班次开始时间
                                var planDateBeginTime = planDate;
                                //班次结束时间
                                var planDateEndTime = ((DateTime)planDate).AddDays(1).AddSeconds(-1);

                                //读取班次
                                var userkqClassesSequenceList = kqClassesSequenceList.Where(p => p.ID == userkqClassPlan.CSId).OrderBy(p => p.Sort);


                                //读取班次具体安排
                                if (userkqClassesSequenceList != null && userkqClassesSequenceList.Count() > 0)
                                {
                                    //今天打卡的所有记录
                                    var planDateKrList = krList.Where(p => DateTime.Parse(p.RecordTime.ToString("yyyy-MM-dd")) == DateTime.Parse(planDate.ToString("yyyy-MM-dd")));

                                    foreach (var userkqClassesSequence in userkqClassesSequenceList)
                                    {
                                        //日历上今天（包括今天）之前显示的是员工的考勤记录，今天之后显示的是排班表记录
                                        if (planDate <= DateTime.Now)
                                        {
                                            //班次类型对应表数据
                                            var userkqClassesRelationList = kqClassesRelationList.Where(p => p.CSId == userkqClassPlan.CSId).OrderBy(p => p.Sort);

                                            if (userkqClassesRelationList != null && userkqClassesRelationList.Count() > 0)
                                            {
                                                //几天有几个班次
                                                var dayCount = 1;
                                                //用户每天班次的具体安排时间
                                                foreach (var userkqClassesRelation in userkqClassesRelationList)
                                                {
                                                    //上班是否要打卡
                                                    var StartWorkClockIn = userkqClassesRelation.StartWorkClockIn.ToAs<bool>();

                                                    //下班是否要打卡
                                                    var EndWorkClockIn = userkqClassesRelation.EndWorkClockIn.ToAs<bool>();

                                                    //具体班次信息
                                                    var kqclassessequence = userkqClassesRelation.kqClassesSequence;

                                                    //具体班次类型
                                                    var kqclassesref = userkqClassesRelation.kqClassesRef;

                                                    //班次类型ID(空值为休息日)
                                                    if (kqclassesref != null)
                                                    {
                                                        //上班是否迟到
                                                        var isLate = false;

                                                        //////////////////////////////////////////////////上班打卡模块   开始//////////////////////////////////////////////

                                                        //班次开始时间
                                                        var classesRefstartTime = kqclassesref.StartTime;

                                                        //班次结束时间
                                                        var classesRefEndTime = kqclassesref.EndTime;
                                                        //上班打卡有效时间(单位是分钟)
                                                        var StartWorkValid = kqclassesref.StartWorkValid;

                                                        //迟到缓冲时间(单位是分钟)(长度要控制，不能超过12小时)
                                                        var LateMinute = kqclassesref.LateMinute;

                                                        //早退缓冲时间(单位是分钟)(长度要控制，不能超过12小时)
                                                        var LeaveMinute = kqclassesref.LeaveMinute;

                                                        //最迟上班打卡时间
                                                        var KQstartTime = DateTime.Parse(classesRefstartTime.ToString()).AddMinutes((double)LateMinute);

                                                        //最早下班打卡时间
                                                        var KQendTime = DateTime.Parse(classesRefEndTime.ToString()).AddMinutes(-(double)LeaveMinute);

                                                        //上班班打卡起始时间
                                                        var classesRefstartTime_QS = DateTime.Parse(classesRefstartTime.ToAs<string>()).AddMinutes(-StartWorkValid.ToAs<int>());

                                                        //上班打卡--打开记录
                                                        var userkqClassesQuenceList = planDateKrList.Where(p => DateTime.Parse(p.RecordTime.ToString("HH:mm")) <= DateTime.Parse(classesRefEndTime.ToString("HH:mm")) && DateTime.Parse(p.RecordTime.ToString("HH:mm")) >= DateTime.Parse(classesRefstartTime_QS.ToString("HH:mm"))).OrderByDescending(p => p.RecordTime);

                                                        //上班要打卡
                                                        if (StartWorkClockIn)
                                                        {
                                                            //未打卡
                                                            if (userkqClassesQuenceList == null || userkqClassesQuenceList.Count() == 0)
                                                            {
                                                                var userkq = new kqRecord();
                                                                userkq.ID = Guid.NewGuid();
                                                                userkq.RecordTime = DateTime.Parse(planDate.ToString("yyyy-MM-dd") + " " + classesRefstartTime.ToString("HH:mm")).AddMinutes(10);
                                                                userkq.ClassesName = "上班:";
                                                                if (DateTime.Parse(planDate.ToString("yyyy-MM-dd") + " " + classesRefEndTime.ToString("HH:mm")) < DateTime.Now)
                                                                {
                                                                    userkq.kqRecordTitle = "<span class='NotPunch'>未打卡</span>";
                                                                    //未打卡，上班、未打卡样式
                                                                    userkq.Stauts = "DivGoToWork DivNotPunch";
                                                                }
                                                                else
                                                                {
                                                                    //还未到点不需要提示，上班、未到到点没有打卡记录样式
                                                                    userkq.kqRecordTitle = "";
                                                                    userkq.Stauts = "DivGoToWork DivNotPoint";
                                                                }
                                                                userKqRecordList.Add(userkq);
                                                            }
                                                            else
                                                            {
                                                                var userkqRecord = userkqClassesQuenceList.Where(p => IsExistUserKq(userKqRecordList, p.ID) && DateTime.Parse(p.RecordTime.ToString("HH:mm")) <= DateTime.Parse(KQstartTime.ToString("HH:mm")) && DateTime.Parse(p.RecordTime.ToString("HH:mm")) >= DateTime.Parse(classesRefstartTime_QS.ToString("HH:mm"))).OrderBy(p => p.RecordTime).FirstOrDefault();

                                                                //正常
                                                                if (userkqRecord != null)
                                                                {
                                                                    var isExist = IsExistUserKq(userKqRecordList, userkqRecord.ID);
                                                                    if (isExist)
                                                                    {
                                                                        var userkq = new kqRecord();
                                                                        userkq.ID = userkqRecord.ID;
                                                                        userkq.RecordTime = userkqRecord.RecordTime;
                                                                        userkq.ClassesName = "上班:";
                                                                        //正常，上班、正常样式
                                                                        userkq.Stauts = "DivGoToWork DivNormal";
                                                                        userkq.kqRecordTitle = userkqRecord.RecordTime.ToString("HH:mm");
                                                                        userKqRecordList.Add(userkq);
                                                                    }
                                                                    else
                                                                    {
                                                                        userkqRecord = userkqClassesQuenceList.Where(p => DateTime.Parse(p.RecordTime.ToString("HH:mm")) > DateTime.Parse(KQstartTime.ToString("HH:mm")) && DateTime.Parse(p.RecordTime.ToString("HH:mm")) < DateTime.Parse(classesRefEndTime.ToString("HH:mm"))).OrderBy(p => p.RecordTime).FirstOrDefault();
                                                                        //迟到
                                                                        if (userkqRecord != null)
                                                                        {
                                                                            //迟到
                                                                            isLate = true;

                                                                            var userkq = new kqRecord();
                                                                            userkq.ID = userkqRecord.ID;
                                                                            userkq.RecordTime = userkqRecord.RecordTime;

                                                                            TimeSpan d1 = new TimeSpan(DateTime.Parse(userkqRecord.RecordTime.ToString("HH:mm")).Ticks);
                                                                            TimeSpan d2 = new TimeSpan(DateTime.Parse(KQstartTime.ToString("HH:mm")).Ticks);
                                                                            TimeSpan dd = d1.Subtract(d2).Duration();

                                                                            //var cdTimeSpan = DateTime.Parse(userkqRecord.RecordTime.ToString("HH:mm")) - DateTime.Parse(KQstartTime.ToString("HH:mm"));
                                                                            //迟到总分钟数
                                                                            userkq.LateMinNum = dd.TotalMinutes;

                                                                            userkq.ClassesName = "上班:";

                                                                            //迟到
                                                                            userkq.Stauts = "DivGoToWork DivLate";
                                                                            userkq.kqRecordTitle = "<span class='Late'>" + userkqRecord.RecordTime.ToString("HH:mm") + "(迟到)</span>";
                                                                            userKqRecordList.Add(userkq);

                                                                            //记录迟到记录
                                                                            //cdRecordList.Add(userkq);

                                                                            cdStr += "{ \"cdTime\": \"" + userkq.RecordTime + "\",\"cdClassName\": \"" + userkq.ClassesName + "\", \"cdMinNum\": \"" + userkq.LateMinNum + "\", \"cdType\": \"DivLate\", \"Type\": \"cdstr\"},";
                                                                        }
                                                                        else
                                                                        {
                                                                            var userkq = new kqRecord();
                                                                            userkq.ID = Guid.NewGuid();
                                                                            userkq.RecordTime = DateTime.Parse(planDate.ToString("yyyy-MM-dd") + " " + classesRefstartTime.ToString("HH:mm")).AddMinutes(10);
                                                                            userkq.ClassesName = "上班:";
                                                                            userkq.kqRecordTitle = "<span class='NotPunch'>未打卡</span>";
                                                                            //未打卡
                                                                            userkq.Stauts = "DivGoToWork DivNotPunch";
                                                                            userKqRecordList.Add(userkq);
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    userkqRecord = userkqClassesQuenceList.Where(p => DateTime.Parse(p.RecordTime.ToString("HH:mm")) > DateTime.Parse(KQstartTime.ToString("HH:mm")) && DateTime.Parse(p.RecordTime.ToString("HH:mm")) < DateTime.Parse(classesRefEndTime.ToString("HH:mm"))).OrderBy(p => p.RecordTime).FirstOrDefault();
                                                                    //迟到
                                                                    if (userkqRecord != null)
                                                                    {
                                                                        //迟到
                                                                        isLate = true;

                                                                        var userkq = new kqRecord();
                                                                        userkq.ID = userkqRecord.ID;
                                                                        userkq.RecordTime = userkqRecord.RecordTime;

                                                                        TimeSpan d1 = new TimeSpan(DateTime.Parse(userkqRecord.RecordTime.ToString("HH:mm")).Ticks);
                                                                        TimeSpan d2 = new TimeSpan(DateTime.Parse(KQstartTime.ToString("HH:mm")).Ticks);
                                                                        TimeSpan dd = d1.Subtract(d2).Duration();

                                                                        //var cdTimeSpan = DateTime.Parse(userkqRecord.RecordTime.ToString("HH:mm")) - DateTime.Parse(KQstartTime.ToString("HH:mm"));
                                                                        //迟到总分钟数
                                                                        userkq.LateMinNum = dd.TotalMinutes;

                                                                        //var cdTimeSpan = DateTime.Parse(userkqRecord.RecordTime.ToString("HH:mm")) - DateTime.Parse(KQstartTime.ToString("HH:mm"));
                                                                        ////迟到分钟数
                                                                        //userkq.LateMinNum = cdTimeSpan.Minutes;

                                                                        userkq.ClassesName = "上班:";

                                                                        //迟到，上班、迟到样式
                                                                        userkq.Stauts = "DivGoToWork DivLate";
                                                                        userkq.kqRecordTitle = "<span class='Late'>" + userkqRecord.RecordTime.ToString("HH:mm") + "(迟到)</span>";
                                                                        userKqRecordList.Add(userkq);

                                                                        //记录迟到记录
                                                                        //cdRecordList.Add(userkq);
                                                                        cdStr += "{ \"cdTime\": \"" + userkq.RecordTime + "\",\"cdClassName\": \"" + userkq.ClassesName + "\", \"cdMinNum\": \"" + userkq.LateMinNum + "\", \"cdType\": \"DivLate\", \"Type\": \"cdstr\"},";
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        //////////////////////////////////////////////////上班打卡模块   结束//////////////////////////////////////////////


                                                        var xbStr = "下班:";
                                                        if (dayCount == 3)
                                                        {
                                                            xbStr = "晚班:";
                                                        }
                                                        //////////////////////////////////////////////////下班打卡模块   开始//////////////////////////////////////////////
                                                        //下班要打卡
                                                        if (EndWorkClockIn)
                                                        {
                                                            //下班打卡
                                                            //下班打卡有效时间(单位是分钟)
                                                            var EndWorkValid = kqclassesref.EndWorkValid;

                                                            //下班打卡终止时间
                                                            var classesRefEndTime_ZZ = DateTime.Parse(classesRefEndTime.ToAs<string>()).AddMinutes(EndWorkValid.ToAs<int>());

                                                            userkqClassesQuenceList = planDateKrList.Where(p => DateTime.Parse(p.RecordTime.ToString("HH:mm")) >= DateTime.Parse(KQendTime.ToString("HH:mm")) && DateTime.Parse(p.RecordTime.ToString("HH:mm")) <= DateTime.Parse(classesRefEndTime_ZZ.ToString("HH:mm"))).OrderBy(p => p.RecordTime);

                                                            //未打卡
                                                            if (userkqClassesQuenceList == null || userkqClassesQuenceList.Count() == 0)
                                                            {
                                                                var userkq = new kqRecord();
                                                                userkq.ID = Guid.NewGuid();
                                                                userkq.RecordTime = DateTime.Parse(planDate.ToString("yyyy-MM-dd") + " " + classesRefEndTime_ZZ.ToString("HH:mm")).AddMinutes(-1);
                                                                //下班 or 晚班
                                                                userkq.ClassesName = xbStr;
                                                                if (DateTime.Parse(planDate.ToString("yyyy-MM-dd") + " " + classesRefEndTime_ZZ.ToString("HH:mm")) < DateTime.Now)
                                                                {
                                                                    //判断早退问题
                                                                    //上班时间打卡次数
                                                                    var sbKqDKList = planDateKrList.Where(p => DateTime.Parse(p.RecordTime.ToString("HH:mm")) > DateTime.Parse(classesRefstartTime.ToString("HH:mm")) && DateTime.Parse(p.RecordTime.ToString("HH:mm")) < DateTime.Parse(KQendTime.ToString("HH:mm"))).OrderByDescending(p => p.RecordTime);

                                                                    //迟到 或 没有迟到 的情况
                                                                    if ((isLate && sbKqDKList.Count() >= 2) || !isLate)
                                                                    {
                                                                        var kqzt = sbKqDKList.FirstOrDefault();
                                                                        if (kqzt != null)
                                                                        {
                                                                            userkq.kqRecordTitle = "<span class='Late'>" + kqzt.RecordTime.ToString("HH:mm") + "(早退)</span>";
                                                                            //早退，下班、早退样式
                                                                            userkq.Stauts = "DivGoOffWork DivLeaveEarly";

                                                                            TimeSpan d1 = new TimeSpan(DateTime.Parse(KQendTime.ToString("HH:mm")).Ticks);
                                                                            TimeSpan d2 = new TimeSpan(DateTime.Parse(kqzt.RecordTime.ToString("HH:mm")).Ticks);
                                                                            TimeSpan dd = d1.Subtract(d2).Duration();

                                                                            //var cdTimeSpan = DateTime.Parse(userkqRecord.RecordTime.ToString("HH:mm")) - DateTime.Parse(KQstartTime.ToString("HH:mm"));

                                                                            //var cdTimeSpan = DateTime.Parse(KQendTime.ToString("HH:mm")) - DateTime.Parse(kqzt.RecordTime.ToString("HH:mm"));

                                                                            //早退总分钟数
                                                                            userkq.LateMinNum = dd.TotalMinutes;

                                                                            //记录早退记录
                                                                            //cdRecordList.Add(userkq);
                                                                            cdStr += "{ \"cdTime\": \"" + kqzt.RecordTime + "\",\"cdClassName\": \"" + userkq.ClassesName + "\", \"cdMinNum\": \"" + userkq.LateMinNum + "\", \"cdType\": \"DivLeaveEarly\", \"Type\": \"cdstr\"},";
                                                                        }
                                                                        else
                                                                        {
                                                                            userkq.kqRecordTitle = "<span class='NotPunch'>未打卡</span>";
                                                                            //未打卡，下班、未打卡样式
                                                                            userkq.Stauts = "DivGoOffWork DivNotPunch";
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        userkq.kqRecordTitle = "<span class='NotPunch'>未打卡</span>";
                                                                        //未打卡，下班、未打卡样式
                                                                        userkq.Stauts = "DivGoOffWork DivNotPunch";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    //还未到点不需要提示
                                                                    userkq.kqRecordTitle = "";
                                                                    //还未到点，下班、未到点没有记录样式
                                                                    userkq.Stauts = "DivGoOffWork DivNotPoint";
                                                                }
                                                                userKqRecordList.Add(userkq);
                                                            }
                                                            else
                                                            {
                                                                var userkqRecord = userkqClassesQuenceList.FirstOrDefault();
                                                                //正常
                                                                if (userkqRecord != null)
                                                                {
                                                                    //判断是否已经存在该条打卡记录信息
                                                                    var isExist = IsExistUserKq(userKqRecordList, userkqRecord.ID);
                                                                    if (isExist)
                                                                    {
                                                                        var userkq = new kqRecord();
                                                                        userkq.ID = userkqRecord.ID;
                                                                        userkq.RecordTime = userkqRecord.RecordTime;

                                                                        //下班 or 晚班
                                                                        userkq.ClassesName = xbStr;

                                                                        //正常，下班、正常样式
                                                                        userkq.Stauts = "DivGoOffWork DivNormal";
                                                                        userkq.kqRecordTitle = userkqRecord.RecordTime.ToString("HH:mm");
                                                                        userKqRecordList.Add(userkq);
                                                                    }
                                                                    //存在的话直接为未打卡
                                                                    else
                                                                    {
                                                                        var userkq = new kqRecord();
                                                                        userkq.ID = Guid.NewGuid();
                                                                        userkq.RecordTime = DateTime.Parse(planDate.ToString("yyyy-MM-dd") + " " + classesRefEndTime_ZZ.ToString("HH:mm")).AddMinutes(-1);

                                                                        //下班 or 晚班
                                                                        userkq.ClassesName = xbStr;
                                                                        userkq.kqRecordTitle = "<span class='NotPunch'>未打卡</span>";
                                                                        //未打卡，下班、未打卡样式
                                                                        userkq.Stauts = "DivGoOffWork DivNotPunch";
                                                                        userKqRecordList.Add(userkq);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        //////////////////////////////////////////////////下班打卡模块   结束//////////////////////////////////////////////
                                                    }
                                                    else
                                                    {
                                                        var userkq = new kqRecord();
                                                        userkq.ID = Guid.NewGuid();
                                                        userkq.RecordTime = DateTime.Parse(planDate.ToString("yyyy-MM-dd"));
                                                        userkq.ClassesName = "<span class='SpanWeeKRest'>" + userkqClassesSequence.CSName + "</span>";
                                                        //userkq.kqRecordTitle = "未打卡";
                                                        //班次类型ID(空值为休息日)
                                                        //休息日样式
                                                        userkq.Stauts = "DivRestDay";
                                                        userKqRecordList.Add(userkq);
                                                    }

                                                    //记录今天是第几个班次
                                                    dayCount++;
                                                }
                                            }
                                            else
                                            {
                                                var userkq = new kqRecord();
                                                userkq.ID = Guid.NewGuid();
                                                userkq.RecordTime = DateTime.Parse(planDate.ToString("yyyy-MM-dd"));
                                                userkq.ClassesName = "<span class='SpanWeeKRest'>" + userkqClassesSequence.CSName + "</span>";
                                                //userkq.kqRecordTitle = "未打卡";
                                                //班次类型ID(空值为休息日)
                                                //休息日样式
                                                userkq.Stauts = "DivRestDay";
                                                userKqRecordList.Add(userkq);
                                            }
                                        }
                                        //今天之后显示的是排班表记录
                                        else
                                        {
                                            var userkq = new kqRecord();
                                            userkq.ID = Guid.NewGuid();
                                            userkq.RecordTime = DateTime.Parse(planDate.ToString("yyyy-MM-dd"));
                                            userkq.ClassesName = (userkqClassesSequence.wmfReference != null ? userkqClassesSequence.wmfReference.ItemValue : userkqClassesSequence.CSName);
                                            //userkq.kqRecordTitle = "未打卡";
                                            //显示的是排班表记录 样式
                                            userkq.Stauts = "DivSchedulingRecords";
                                            userKqRecordList.Add(userkq);
                                        }
                                    }
                                }

                                //员工备注
                                if (!string.IsNullOrEmpty(userkqClassPlan.PersonnelRemark))
                                {
                                    userkqClassPlan.PersonnelRemark = HttpUtility.HtmlDecode(userkqClassPlan.PersonnelRemark);
                                    var userkq = new kqRecord();
                                    userkq.ID = Guid.NewGuid();
                                    userkq.RecordTime = DateTime.Parse(planDate.ToString("yyyy-MM-dd"));
                                    userkq.ClassesName = 备注 + ":";
                                    userkq.kqRecordTitle = "<span class='PersonnelRemark'>" + (userkqClassPlan.PersonnelRemark.Length > 8 ? userkqClassPlan.PersonnelRemark.Substring(0, 8) + "..." : userkqClassPlan.PersonnelRemark) + "</span>";
                                    //员工备注,员工备注 样式
                                    userkq.Stauts = "DivPersonnelRemark";
                                    userKqRecordList.Add(userkq);
                                }
                                //经理备注
                                if (!string.IsNullOrEmpty(userkqClassPlan.ManagerRemark))
                                {
                                    userkqClassPlan.ManagerRemark = HttpUtility.HtmlDecode(userkqClassPlan.ManagerRemark);
                                    var userkq = new kqRecord();
                                    userkq.ID = Guid.NewGuid();
                                    userkq.RecordTime = DateTime.Parse(planDate.ToString("yyyy-MM-dd"));
                                    userkq.ClassesName = 经理备注 + ":";
                                    userkq.kqRecordTitle = "<span class='ManagerRemark'>" + (userkqClassPlan.ManagerRemark.Length > 6 ? userkqClassPlan.ManagerRemark.Substring(0, 6) + "..." : userkqClassPlan.ManagerRemark) + "</span>";
                                    //经理备注，经理备注 样式
                                    userkq.Stauts = "DivManagerRemark";
                                    userKqRecordList.Add(userkq);

                                    //var jluserkq = new kqRecord();
                                    //jluserkq.ID = Guid.NewGuid();
                                    //jluserkq.RecordTime = DateTime.Parse(planDate.ToString("yyyy-MM-dd"));
                                    //jluserkq.ClassesName = userkqClassPlan.ManagerRemark;
                                    ////经理备注，经理备注 样式
                                    //jluserkq.Stauts = "DivManagerRemark";
                                    //加入日历左下角统计处
                                    //cdRecordList.Add(jluserkq);
                                    cdStr += "{ \"cdTime\": \"" + DateTime.Parse(planDate.ToString("yyyy-MM-dd")) + "\",\"cdClassName\": \"" + userkqClassPlan.ManagerRemark + "\", \"cdType\": \"DivManagerRemark\", \"Type\": \"cdstr\"},";
                                }
                            }
                        }
                    }
                    //请假
                    var wfService = new WorkflowService();
                    //已审批通过的
                    var contentIds = wfService.GetWorkflowStatusAuditContentIds(WorkflowStatusType.Success, 资源.工作流请假);
                    var askforLeaveList = new BaseBll<AskForLeave>().All.Where(u => u.FlagTrashed == false && u.UserID == userId && contentIds.Contains(u.ID))
                        .Where(u => u.BeginTime > begintime && u.EndTime < endtime);
                    if (askforLeaveList.Any())
                    {
                        foreach (var askforLeave in askforLeaveList)
                        {
                            var ToStrType = "";
                            if (askforLeave.QingJiaType == Guid.Parse(MorSun.Common.类别.Reference.请假_事假))
                            {
                                ToStrType = "事假";
                            }
                            else if (askforLeave.QingJiaType == Guid.Parse(MorSun.Common.类别.Reference.请假_病假))
                            {
                                ToStrType = "病假";
                            }
                            else if (askforLeave.QingJiaType == Guid.Parse(MorSun.Common.类别.Reference.请假_学习假))
                            {
                                ToStrType = "学习假";
                            }
                            else if (askforLeave.QingJiaType == Guid.Parse(Reference.请假_休假))
                            {
                                ToStrType = "休假";
                            }
                            else if (askforLeave.QingJiaType == Guid.Parse(MorSun.Common.类别.Reference.请假_年假))
                            {
                                ToStrType = "年假";
                            }
                            else if (askforLeave.QingJiaType == Guid.Parse(MorSun.Common.类别.Reference.请假_其它))
                            {
                                ToStrType = "其它";
                            }
                            cdStr += "{ \"ToName\": \"" + "" + "\",\"StartTime\": \"" + askforLeave.BeginTime.ToString("yyyy-MM-dd HH:mm") + "\",\"EndTime\": \"" + askforLeave.EndTime.ToString("yyyy-MM-dd HH:mm") + "\", \"cdType\": \"" + askforLeave.QingJiaType.ToSecureString() + "\", \"QJType\": \"" + ToStrType + "\", \"Type\": \"cdstr\"},";
                        }
                    }

                    //统计去向记录
                    //if (personnelToList != null && personnelToList.Count() > 0)
                    //{
                    //    var ToStrType = "";
                    //    foreach (var personnelTo in personnelToList)
                    //    {
                    //        ToStrType = "";
                    //        if (personnelTo.ToID == Guid.Parse(MorSun.Common.类别.Reference.请假_事假))
                    //        {
                    //            ToStrType = "事假";
                    //        }
                    //        else if (personnelTo.ToID == Guid.Parse(MorSun.Common.类别.Reference.请假_病假))
                    //        {
                    //            ToStrType = "病假";
                    //        }
                    //        else if (personnelTo.ToID == Guid.Parse(MorSun.Common.类别.Reference.请假_学习假))
                    //        {
                    //            ToStrType = "学习假";
                    //        }
                    //        else if (personnelTo.ToID == Guid.Parse(MorSun.Common.类别.Reference.请假_年假))
                    //        {
                    //            ToStrType = "年假";
                    //        }
                    //        else if (personnelTo.ToID == Guid.Parse(MorSun.Common.类别.Reference.请假_其它))
                    //        {
                    //            ToStrType = "其它";
                    //        }
                    //        cdStr += "{ \"ToName\": \"" + personnelTo.ToName + "\",\"StartTime\": \"" + personnelTo.StartTime.ToString("yyyy-MM-dd HH:mm") + "\",\"EndTime\": \"" + personnelTo.EndTime.ToString("yyyy-MM-dd HH:mm") + "\", \"cdType\": \"" + personnelTo.PersonnelToType + "\", \"QJType\": \"" + ToStrType + "\", \"Type\": \"cdstr\"},";
                    //    }
                    //}

                    //年假统计
                    cdStr += "{ \"ToName\": \"" + yearJia + "\",\"StartTime\": \"" + newYearJia + "\",\"EndTime\": \"\", \"cdType\": \"nianjia\", \"QJType\": \"\", \"Type\": \"cdstr\"},";
                    //学习假
                    cdStr += "{ \"ToName\": \"" + stuJia + "\",\"StartTime\": \"" + newStuJia + "\",\"EndTime\": \"\", \"cdType\": \"xuexijia\", \"QJType\": \"\", \"Type\": \"cdstr\"},";

                    var krCount = userKqRecordList.Count();
                    var i = 1;
                    foreach (var item in userKqRecordList)
                    {
                        krStr += "{ \"container\": \"#jMonthCalendar\", \"head\": \"#CalendarHead\", \"body\": \"#CalendarBody\", \"EventID\": " + i + ", \"StartDateTime\": \"" + item.RecordTime.ToString("yyyy-MM-dd") + "\", \"Title\": \"" + item.ClassesName + item.kqRecordTitle + "\", \"URL\": \"#\", \"CssClass\": \"" + item.Stauts + "\", \"Type\": \"right\" }";
                        if (i != krCount)
                        {
                            krStr += ",";
                        }
                        i++;
                    }
                }
                else
                {
                    if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.员工, MorSun.Common.Privelege.操作.修改))
                    {
                        krStr = "{ \"ErrorMessage\": \"" + XmlHelper.GetKeyNameValidation<kqClassPlan>("该员工还没有考勤号，是否现在编辑") + "\", \"URL\": \"/User/DocumentManage?UId=" + userId + "&scrollTo=nav\", \"Type\": \"error\" }";
                    }
                    else
                    {
                        krStr = "{ \"ErrorMessage\": \"" + XmlHelper.GetKeyNameValidation<kqClassPlan>("不存在考勤号，请联系管理员编辑考勤号") + "\", \"URL\": \"\", \"Type\": \"error\" }";
                    }
                }
            }




            //统计记录
            if (!string.IsNullOrEmpty(cdStr))
            {
                //截取掉最后一个逗号
                cdStr = cdStr.Substring(0, cdStr.Length - 1);
                krStr += "," + cdStr;
            }

            krStr = "[" + krStr + "]";
            return krStr;
        }



        /// <summary>
        /// 连接考勤机
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        private string ConnectKQ(string ip, int port, zkemkeeper.CZKEMClass axCZKEM)
        {
            if (axCZKEM == null)
                axCZKEM = new zkemkeeper.CZKEMClass();
            int idwErrorCode = 0;
            bool bIsConnected = axCZKEM.Connect_Net(ip, port);
            string msg = "";
            if (bIsConnected)
            {
                bIsConnected = true;
                axCZKEM.RegEvent(iMachineNumber, 65535);
                msg = "true";
            }
            else
            {
                axCZKEM.GetLastError(ref idwErrorCode);
                msg = "连接失败，错误代码：" + idwErrorCode.ToString();
            }
            return msg;
        }

        /// <summary>
        /// 删除连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        private void DisConnectKQ(zkemkeeper.CZKEMClass axCZKEM)
        {
            if (axCZKEM == null)
                axCZKEM = new zkemkeeper.CZKEMClass();
            axCZKEM.Disconnect();
            bIsConnected = false;
        }

        /// <summary>
        /// 获取考勤数据
        /// </summary>
        /// <param name="id">考勤机ID</param>
        /// <param name="clearData">是否清除考勤机数据</param>
        /// <returns></returns>
        private string GetGeneralLogData(Guid id, bool clearData, zkemkeeper.CZKEMClass axCZKEM)
        {
            if (axCZKEM == null)
                axCZKEM = new zkemkeeper.CZKEMClass();

            string msg = "";

            string sdwEnrollNumber = "";
            int idwTMachineNumber = 0;
            int idwEMachineNumber = 0;
            int idwVerifyMode = 0;
            int idwInOutMode = 0;
            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;
            int idwSecond = 0;
            int idwWorkcode = 0;

            int idwErrorCode = 0;

            //
            string sName = "";
            string sPassword = "";
            int iPrivilege = 0;
            bool bEnabled = false;
            Dictionary<string, string> dRecord = new Dictionary<string, string>();
            //

            var bll = new BaseBll<kqRecord>();
            var l = new List<kqRecord>();
            var RList = bll.All;
            Int64 sort = Convert.ToInt64(RList.Max(r => r.Sort));
            int iValue = 0;
#if debug
            var temp = axCZKEM.GetDeviceStatus(iMachineNumber, 6, ref iValue);
#endif
#if original
            axCZKEM.GetDeviceStatus(iMachineNumber, 6, ref iValue);
#endif

            var recordList = new List<kqRecord>();
            var kqRecordCount = 4000;
            var filepath = string.Empty;
            var recordMachineModel = new BaseBll<kqPort>().All.Where(p => p.ID == id).FirstOrDefault();
            if (recordMachineModel != null)
            {
                kqRecordCount = recordMachineModel.MaxRecord.HasValue ? recordMachineModel.MaxRecord.Value : kqRecordCount;
                //filepath = recordMachineModel.BackUpPath + "/UploadFile/kqRecord/";
            }
            axCZKEM.EnableDevice(iMachineNumber, false);//disable the device
            if (axCZKEM.ReadGeneralLogData(iMachineNumber))//read all the attendance records to the memory
            {
                if (iValue < kqRecordCount)
                {
                    axCZKEM.EnableDevice(iMachineNumber, true);//enable the device  当记录数小于最大记录数时，才马上启用，否则，要备份完之后才启用。读取完数据之后就启用考勤机
                }
                else
                {
                    axCZKEM.ReadAllUserID(iMachineNumber);
                    while (axCZKEM.SSR_GetAllUserInfo(iMachineNumber, out sdwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))
                    {
                        int index = sName.IndexOf("\0");
                        if (index != -1)
                        {
                            sName = sName.Substring(0, index);
                        }

                        dRecord.Add(sdwEnrollNumber, sName);
                    }
                }
                while (axCZKEM.SSR_GetGeneralLogData(iMachineNumber, out sdwEnrollNumber, out idwVerifyMode,
                           out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                {

                    //lvLogs.Items.Add(iGLCount.ToString());
                    //lvLogs.Items[iIndex].SubItems.Add(sdwEnrollNumber);//modify by Darcy on Nov.26 2009
                    //lvLogs.Items[iIndex].SubItems.Add(idwVerifyMode.ToString());
                    //lvLogs.Items[iIndex].SubItems.Add(idwInOutMode.ToString());
                    //lvLogs.Items[iIndex].SubItems.Add(idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString());
                    //lvLogs.Items[iIndex].SubItems.Add(idwWorkcode.ToString());

                    DateTime RTime = Convert.ToDateTime(idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString());
                    String CId = sdwEnrollNumber;
                    if (RList.Where(r => r.CId == CId && r.RecordTime == RTime && r.PId == id).FirstOrDefault() == null)//考勤机的数据是还没有添加进数据库的话才添加进去
                    {
                        kqRecord model = new kqRecord();
                        model.PId = id;
                        model.RecordTime = RTime;
                        model.CId = CId;
                        model.ID = Guid.NewGuid();
                        model.RegTime = DateTime.Now;
                        model.Sort = sort;
                        sort++;
                        l.Add(model);
                    }
                    if (iValue >= kqRecordCount)
                    {
#if debug
                        if (dRecord.ContainsKey(CId))
                        {
                            recordList.Add(new kqRecord() { CId = CId, RecordTime = RTime, userName = dRecord[CId] });
                        }
#endif
#if original
                        recordList.Add(new kqRecord() { CId = CId, RecordTime = RTime, userName = dRecord[CId] });
#endif
                    }

                }
                if (l.Count() > 0)
                {
                    foreach (var m in l)
                    {
                        RList.AddObject(m);
                    }
                    bll.UpdateChanges();
                    msg = "true";
                    if (clearData)
                        axCZKEM.ClearGLog(iMachineNumber);  //这句会删掉考勤机所有数据
                }
                else
                {
                    msg = "截止上次读取数据为止没有新的打卡记录或数据读取失败!";
                }
                if (recordList.Count > 0)
                {
                    var ds = ToDataSet(recordList);
                    FileObj.FolderCreate(Server.MapPath("/UploadFile/kqRecord/") + id);
                    var fileName = DateTime.Now.ToString("yyyy-MM-ddHHmmss") + ".xls";
                    var oldFileName = fileName;
                    fileName = Server.MapPath("/UploadFile/kqRecord/") + id + "/" + fileName;
                    //new ExcleHelper().DataSetToExcle(ds, fileName);
                    ExportExcel.LeadingOut(ds.Tables[0], fileName);
                    var mailModel = new wmfMail();
                    mailModel.MailContent = "考勤机数据已经达到要清空的标准，系统已经做好备份，并清空考勤机数据。点击这里下载备份文件。<a href=\"javascript:void(0);\" onclick=\"DownloadFile('/KQ','/UploadFile/kqRecord/" + id + "/" + oldFileName + "','" + oldFileName + "')\">" + oldFileName + "</a>";
                    new SysEmailController().SentSystemInfo(mailModel);
                    axCZKEM.ClearGLog(iMachineNumber);//清空考勤记录
                    axCZKEM.EnableDevice(iMachineNumber, true);//启用考勤机
                }
            }
            else
            {
                axCZKEM.GetLastError(ref idwErrorCode);

                if (idwErrorCode != 0)
                {
                    msg = "从设备读取数据失败，错误代码:" + idwErrorCode.ToString();
                }
                else
                {
                    msg = "设备中没有数据！";
                }
                var mailModel = new wmfMail();
                mailModel.MailContent = "考勤机数据已经超过要清空的标准，系统无法进行进一步操作，请您做好备份并清空考勤机数据。";
                new SysEmailController().SentSystemInfo(mailModel);
            }
            //axCZKEM.EnableDevice(iMachineNumber, true);//enable the device            
            return msg;
        }

        /// <summary>
        /// 获取考勤数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public string GetData(string id, kqPort t)
        {
            string msg = "";//返回消息


            if (!string.IsNullOrEmpty(id))
            {
                t = new BaseBll<kqPort>().GetModel(id);
            }

            if (t.CheckedId != "")
            {
                t = new BaseBll<kqPort>().GetModel(t.CheckedId);
            }

            if (t == null)
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfVillage>("未找到考勤机记录"), "") });
            }

            //没有取到考勤机IP
            if (t.PortIP == null || t.PortIP.Equals(""))
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfVillage>("考勤机IP为空"), "") });

            //中控考勤机OP1000默认端口号
            if (t.PortNumber == null)
                t.PortNumber = 4370;

            zkemkeeper.CZKEMClass axCZKEM = new zkemkeeper.CZKEMClass();

            string connectMsg = ConnectKQ(t.PortIP, Convert.ToInt32(t.PortNumber), axCZKEM);
            if (connectMsg.Equals("true"))
            {
                msg = GetGeneralLogData(t.ID, false, axCZKEM);
                //关闭连接
                DisConnectKQ(axCZKEM);
            }
            else
            {
                msg = connectMsg;
            }
            return msg;
        }

        /// <summary>
        /// 获取数据并清除考勤机数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetDataAndClear(string id, kqPort t)
        {
            string msg = "";//返回消息

            if (!string.IsNullOrEmpty(id))
            {
                t = new BaseBll<kqPort>().GetModel(id);
            }

            if (t.CheckedId != "")
            {
                t = new BaseBll<kqPort>().GetModel(t.CheckedId);
            }
            var model = t;

            if (model == null)
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfVillage>("未找到考勤机记录"), "") });
            }

            //没有取到考勤机IP
            if (model.PortIP == null || model.PortIP.Equals(""))
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfVillage>("考勤机IP为空"), "") });

            //中控考勤机OP1000默认端口号
            if (model.PortNumber == null)
                model.PortNumber = 4370;
            zkemkeeper.CZKEMClass axCZKEM = new zkemkeeper.CZKEMClass();
            string connectMsg = ConnectKQ(model.PortIP, Convert.ToInt32(model.PortNumber), axCZKEM);
            if (connectMsg.Equals("true"))
            {
                msg = GetGeneralLogData(model.ID, true, axCZKEM);
                //关闭连接
                DisConnectKQ(axCZKEM);
            }
            else
            {
                msg = connectMsg;
            }
            return msg;
        }

        /// <summary>
        /// 查看考勤记录  测试用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult KKQ(string id)
        {
            var model = new KRVModel();
            model.formCid = id;
            var kqr = model.All.Where(m => m.CId == id);
            if (kqr.Count() == 0)
                model.errString = "穿越失败！没找到您的考勤记录！";

            return View(model);
        }

        /// <summary>
        /// 我的考勤查看
        /// </summary>
        /// <returns></returns>
        public new ActionResult View(Guid? UserId)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.我的考勤, MorSun.Common.Privelege.操作.查看))
            {
                var model = new KRVModel();
                if (UserId != null && UserId != Guid.Empty)
                {
                    model.UserId = UserId;
                    model.isManagerPage = true;
                }
                else
                {
                    model.UserId = UserID;
                    model.isManagerPage = false;
                }

                //插入操作日志
                InsertLog(null, "kqRecord", "查看我的考勤", "", "");
                return View(model);
            }
            else
            {
                return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
            }
        }

        #region 矿石代码
        /// <summary>
        /// 定时读取
        /// </summary>
        /// <returns></returns>
        public string dsdq()
        {
            //MorSunScheduler.Instance.Clear();
            CheckingTrigger t = new CheckingTrigger();
            t.Run();
            CheckingTrigger2 t2 = new CheckingTrigger2();
            t2.Run();
            //SimpleTriggerExample t3 = new SimpleTriggerExample();
            //t3.Run();

            return "true";
        }

        /// <summary>
        /// 是否开启状态
        /// </summary>
        /// <returns></returns>
        public string IsStart()
        {
            return MorSunScheduler.Instance.IsStart().ToString();
        }

        /// <summary>
        /// 清除矿石任务
        /// </summary>
        /// <returns></returns>
        public string Clear()
        {
            MorSunScheduler.Instance.Clear();
            return "true";
        }

        /// <summary>
        /// 停止矿石
        /// </summary>
        /// <returns></returns>
        public string Stop()
        {
            MorSunScheduler.Instance.Stop(false);
            return "true";
        }

        /// <summary>
        /// 开启矿石
        /// </summary>
        /// <returns></returns>
        public string Start()
        {
            MorSunScheduler.Instance.Start();
            return "true";
        }

        /// <summary>
        /// 全部继续
        /// </summary>
        /// <returns></returns>
        public string ResumeAll()
        {
            MorSunScheduler.Instance.ResumeAll();
            return "true";
        }

        /// <summary>
        /// 停止某项工作
        /// </summary>
        /// <returns></returns>
        public string StopJob(string name, string group)
        {
            MorSunScheduler.Instance.StopJob(name, group);
            return "true";
        }
        #endregion

        /// <summary>
        /// 编辑备注、经理备注
        /// </summary>
        /// <param name="t"></param>
        /// <param name="userIdStr"></param>
        /// <returns></returns>
        public string EditKqClassPlan(kqClassPlan t, Guid? userIdStr)
        {
            var userId = UserID;
            string krStr = string.Empty;
            if (userIdStr != null && userIdStr != Guid.Empty)
            {
                userId = Guid.Parse(userIdStr.ToString());
            }
            //是否开启考勤备注权限
            var remarkPrv = bool.Parse(webConfigHelp.GetWebConfigValue("KQRemark"));
            if (remarkPrv)
            {
                if (t.PlanDate == null)
                {
                    return XmlHelper.GetKeyNameValidation<kqClassPlan>("操作失败，请重新加载页面");
                }
                //if (t.PlanDate.ToString("yyyy-MM") != YearMonth.ToString("yyyy-MM"))
                //{
                //    return XmlHelper.GetKeyNameValidation<kqClassPlan>("该日期不在当前月份内" + t.PlanDate + "  " + YearMonth);
                //}
                if (!string.IsNullOrEmpty(t.PersonnelRemark) && t.PersonnelRemark.Length > 50)
                {
                    return XmlHelper.GetKeyNameValidation<kqClassPlan>("备注内容已经超过50个字符");
                }
                if (!string.IsNullOrEmpty(t.ManagerRemark) && t.ManagerRemark.Length > 50)
                {
                    return XmlHelper.GetKeyNameValidation<kqClassPlan>("经理备注内容已经超过50个字符");
                }

                var kqClassBll = new BaseBll<kqClassPlan>();
                var kqClass = kqClassBll.All.Where(p => p.UserId == userId && p.PlanDate == t.PlanDate).FirstOrDefault();
                if (kqClass != null)
                {
                    kqClass.PersonnelRemark = HttpUtility.HtmlEncode(t.PersonnelRemark);
                    var ManagerRemark_Item = MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.我的考勤, MorSun.Common.Privelege.操作.添加);
                    if (ManagerRemark_Item)
                    {
                        kqClass.ManagerRemark = HttpUtility.HtmlEncode(t.ManagerRemark);
                    }
                    kqClassBll.Update(kqClass);
                    //插入操作日志
                    InsertLog(null, "kqRecord", "我的考勤备注", "", "");
                    return "true";
                }
                return XmlHelper.GetKeyNameValidation<kqClassPlan>("记录不存在，更新失败");
            }
            else
            {
                return XmlHelper.GetKeyNameValidation<kqClassPlan>("没有权限");
            }
        }

        /// <summary>
        /// 读取用户备注信息
        /// </summary>
        /// <param name="t"></param>
        /// <param name="userIdStr"></param>
        /// <returns></returns>
        public string GetKqClassPlan(kqClassPlan t, Guid? userIdStr)
        {
            var userId = UserID;
            string krStr = string.Empty;
            if (userIdStr != null && userIdStr != Guid.Empty)
            {
                userId = Guid.Parse(userIdStr.ToString());
            }
            var kqClassBll = new BaseBll<kqClassPlan>();
            var kqClass = kqClassBll.All.Where(p => p.UserId == userId && p.PlanDate == t.PlanDate).FirstOrDefault();
            // var cdStr = string.Empty;
            if (kqClass != null)
            {
                krStr = "{ \"PersonnelRemark\": \"" + HttpUtility.HtmlDecode(kqClass.PersonnelRemark) + "\",\"ManagerRemark\": \"" + HttpUtility.HtmlDecode(kqClass.ManagerRemark) + "\",\"Type\": \"remark\"}";
            }

            DateTime begintime = DateTime.Parse(t.PlanDate.ToString("yyyy-MM-dd"));
            DateTime endtime = begintime.AddDays(1).AddSeconds(-1); ;
            var userInfoBll = new wmfUserInfoBll();
            var userInfo = userInfoBll.GetModel(userId);
            if (userInfo != null)
            {
                if (!string.IsNullOrEmpty(userInfo.CheckNumber))
                {
                    //根据时间段获取考勤信息-
                    IQueryable<kqRecord> krList = null;
                    //员工有考勤机ID，取出相应的考勤记录
                    if (userInfo.RollMachine != null && userInfo.RollMachine != Guid.Empty)
                    {
                        krList = new KRVModel().All.Where(r => r.CId == userInfo.CheckNumber && r.kqPort.ID == userInfo.RollMachine && r.RecordTime >= begintime && r.RecordTime <= endtime).OrderBy(r => r.RecordTime);
                    }
                    //员工没有考勤机ID，则取出全部。
                    else
                    {
                        krList = new KRVModel().All.Where(r => r.CId == userInfo.CheckNumber && r.RecordTime >= begintime && r.RecordTime <= endtime).OrderBy(r => r.RecordTime);
                    }

                    var krCount = krList.Count();
                    if (kqClass != null && krCount > 0)
                    {
                        krStr += ",";
                    }
                    var i = 1;
                    foreach (var item in krList)
                    {
                        krStr += "{ \"Title\": \"" + item.RecordTime + "\", \"Type\": \"kq\" }";
                        if (i != krCount)
                        {
                            krStr += ",";
                        }
                        i++;
                    }
                }
            }

            return "[" + krStr + "]";
        }


        public DataSet ToDataSet(List<kqRecord> list)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("Table");
            dt.Columns.Add(new DataColumn("登记号", typeof(string)));//姓名
            dt.Columns.Add(new DataColumn("姓名", typeof(string)));
            dt.Columns.Add(new DataColumn("出勤时间", typeof(DateTime)));

            for (int i = 0; i < list.Count; i++)
            {
                DataRow dr = dt.NewRow();
                dr["登记号"] = list[i].CId;
                dr["姓名"] = list[i].userName;
                dr["出勤时间"] = list[i].RecordTime; ;
                dt.Rows.Add(dr);
            }
            ds.Tables.Add(dt);

            return ds;
        }

        public ActionResult DataFiles(kqPort t)
        {
            return View(t);
        }

        /// <summary>
        /// 直接删除
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public override string Delete(kqPort t)
        {
            string path = Server.MapPath("/");
            if (t.fileName == null)
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<kqPort>("文件件不存在"), "") });
            }
            string fpath = path + t.fileName;
            FileObj.FileDel(fpath);
            return "true";
        }

        //用户密码解密
        public virtual string ajaxEProject_1(Guid u, string p, string q, int tid)
        {
            //用户ID 用户操作密码
            var userid = u;
            var OperatePassword = p;
            if (userid != null && userid != Guid.Empty)
            {
                var userinfoBll = new wmfUserInfoBll();
                var userinfo = userinfoBll.GetModel(userid);
                if (userinfo != null)
                {
                    //操作密码比较
                    var opassword = HOHO18.Common.DEncrypt.DESEncrypt.Decrypt(userinfo.OperatePassword);
                    if (opassword == OperatePassword)
                    {
                        if (!string.IsNullOrEmpty(q))
                        {
                            //调换班，更新班次
                            if (q == "kqwfTransfer")
                            {

                                BaseBll<kqwfTransfer> kqwftransferbll = new BaseBll<kqwfTransfer>();
                                //调换申请信息
                                var kqwftransfer = kqwftransferbll.All.Where(r => r.ID == tid).FirstOrDefault();
                                if (kqwftransfer != null)
                                {
                                    //调班类别
                                    var TransferRef = kqwftransfer.TransferRef.ToString();

                                    //调换人ID  
                                    var TransferUserId = kqwftransfer.TransferUserId;
                                    //调班人调整前时间
                                    var TransferTimeBefore = DateTime.Parse(kqwftransfer.TransferTimeBefore.ToString("yyyy-MM-dd"));
                                    //调班人调整后时间
                                    var TransferTimeAfter = DateTime.Parse(kqwftransfer.TransferTimeAfter.ToString("yyyy-MM-dd"));

                                    var userClasspLanSC = new MorSun.Controllers.UserDefaultScheduleTemplatesController();

                                    //如果调整前 不存在班次则通过排版模版生成班次
                                    userClasspLanSC.AutoGenerateScheduleTableByUserID(TransferTimeBefore, kqwftransfer.UserId.ToAs<Guid>(), false);

                                    //如果调整后 不存在班次则通过排版模版生成班次
                                    userClasspLanSC.AutoGenerateScheduleTableByUserID(TransferTimeAfter, kqwftransfer.UserId.ToAs<Guid>(), false);

                                    //读取所有班次
                                    var kqclassplanbll = new BaseBll<kqClassPlan>();
                                    var kqclassplanList = kqclassplanbll.All.Where(k => k.FlagTrashed == false);

                                    if (TransferRef == MorSun.Common.类别.Reference.调换班_调班)
                                    {
                                        var kqclassplanBefore = kqclassplanList.Where(r => r.PlanDate == TransferTimeBefore && r.aspnet_Users.wmfUserInfo.AutoGeneticId == TransferUserId).FirstOrDefault();

                                        var kqclassplanAfter = kqclassplanList.Where(r => r.PlanDate == TransferTimeAfter && r.aspnet_Users.wmfUserInfo.AutoGeneticId == TransferUserId).FirstOrDefault();

                                        if (kqclassplanBefore != null && kqclassplanAfter != null)
                                        {
                                            //调整前班次ID
                                            var bCSId = kqclassplanBefore.CSId;

                                            //调整前是否值班组长(是的话第二天上班时间提前到8:25)
                                            var bIsShiftForeman = kqclassplanBefore.IsShiftForeman;

                                            //调整后班次ID
                                            var aCSId = kqclassplanAfter.CSId;

                                            //调整后是否值班组长(是的话第二天上班时间提前到8:25)
                                            var aIsShiftForeman = kqclassplanAfter.IsShiftForeman;

                                            //进行对调
                                            kqclassplanBefore.CSId = aCSId;
                                            kqclassplanBefore.IsShiftForeman = aIsShiftForeman;

                                            //进行对调
                                            kqclassplanAfter.CSId = bCSId;
                                            kqclassplanAfter.IsShiftForeman = bIsShiftForeman;

                                            //更新
                                            kqclassplanbll.Update(kqclassplanBefore);

                                            //更新
                                            //kqclassplanbll.Update(kqclassplanAfter);

                                            //context.Response.Write("1");
                                            //context.Response.End();
                                        }
                                        else if (kqclassplanBefore != null && kqclassplanAfter == null)
                                        {


                                        }
                                    }
                                    else if (TransferRef == MorSun.Common.类别.Reference.调换班_换班)
                                    {
                                        //被调班人ID
                                        var ByTransferAfterUserId = kqwftransfer.ByTransferAfterUserId;
                                        //被调班人调整前时间
                                        var ByTransferTimeBefore = DateTime.Parse(kqwftransfer.ByTransferTimeBefore.ToString("yyyy-MM-dd"));
                                        //被调班人调整后时间
                                        var ByTransferTimeAfter = DateTime.Parse(kqwftransfer.ByTransferTimeAfter.ToString("yyyy-MM-dd"));

                                        var kqclassplanBefore = kqclassplanList.Where(r => r.PlanDate == TransferTimeBefore && r.aspnet_Users.wmfUserInfo.AutoGeneticId == TransferUserId).FirstOrDefault();
                                        var kqclassplanAfter = kqclassplanList.Where(r => r.PlanDate == TransferTimeAfter && r.aspnet_Users.wmfUserInfo.AutoGeneticId == TransferUserId).FirstOrDefault();

                                        var BykqclassplanBefore = kqclassplanList.Where(r => r.PlanDate == TransferTimeBefore && r.aspnet_Users.wmfUserInfo.AutoGeneticId == ByTransferAfterUserId).FirstOrDefault();
                                        var BykqclassplanAfter = kqclassplanList.Where(r => r.PlanDate == TransferTimeAfter && r.aspnet_Users.wmfUserInfo.AutoGeneticId == ByTransferAfterUserId).FirstOrDefault();


                                        if (kqclassplanBefore != null && kqclassplanAfter != null && BykqclassplanAfter != null && BykqclassplanBefore != null)
                                        {
                                            //调整前班次ID
                                            var bCSId = kqclassplanBefore.CSId;

                                            //调整前是否值班组长(是的话第二天上班时间提前到8:25)
                                            var bIsShiftForeman = kqclassplanBefore.IsShiftForeman;

                                            //调整后班次ID
                                            var aCSId = kqclassplanAfter.CSId;

                                            //调整后是否值班组长(是的话第二天上班时间提前到8:25)
                                            var aIsShiftForeman = kqclassplanAfter.IsShiftForeman;

                                            //被 调整前班次ID
                                            var BybCSId = BykqclassplanBefore.CSId;

                                            //被 调整前是否值班组长(是的话第二天上班时间提前到8:25)
                                            var BybIsShiftForeman = BykqclassplanBefore.IsShiftForeman;

                                            //被 调整后班次ID
                                            var ByaCSId = BykqclassplanAfter.CSId;

                                            //被 调整后是否值班组长(是的话第二天上班时间提前到8:25)
                                            var ByaIsShiftForeman = BykqclassplanAfter.IsShiftForeman;

                                            //进行对调
                                            kqclassplanBefore.CSId = BybCSId;
                                            kqclassplanBefore.IsShiftForeman = BybIsShiftForeman;

                                            //进行对调
                                            kqclassplanAfter.CSId = ByaCSId;
                                            kqclassplanAfter.IsShiftForeman = ByaIsShiftForeman;

                                            //进行对调
                                            BykqclassplanBefore.CSId = bCSId;
                                            BykqclassplanBefore.IsShiftForeman = bIsShiftForeman;

                                            //进行对调
                                            BykqclassplanAfter.CSId = aCSId;
                                            BykqclassplanAfter.IsShiftForeman = aIsShiftForeman;


                                            //更新
                                            kqclassplanbll.Update(kqclassplanBefore);

                                            ////更新
                                            //kqclassplanbll.Update(kqclassplanAfter);

                                            ////更新
                                            //kqclassplanbll.Update(BykqclassplanBefore);

                                            ////更新
                                            //kqclassplanbll.Update(BykqclassplanAfter);
                                        }
                                    }
                                }

                            }
                            //请年假或学习假 更新
                            else if (q == "kqwfLeave")
                            {

                                var kqleavebll = new BaseBll<kqwfLeave>();
                                //调换信息信息
                                var kqwfleave = kqleavebll.All.Where(r => r.ID == tid).FirstOrDefault();

                                if (kqwfleave != null)
                                {
                                    //请假用户信息
                                    var leaveUserInfo = kqwfleave.aspnet_Users.wmfUserInfo;
                                    //请假小时数
                                    var leavenum = (double)kqwfleave.LeaveDays;
                                    //请假类型
                                    var leavetype = kqwfleave.LeaveRef.ToString();
                                    //请假小时数
                                    if (userinfo.DatesEmployed != null)
                                    {
                                        //年假
                                        if (leavetype == MorSun.Common.类别.Reference.请假_年假)
                                        {
                                            //年假
                                            var njLeaveNum = (double)(leaveUserInfo.WinterVacation != null ? leaveUserInfo.WinterVacation : 0) * 7;
                                            if (leaveUserInfo.WinterVacation == null)
                                            {
                                                //计算年假
                                                njLeaveNum = MorSun.Controllers.UserController.GetYearLeave((DateTime)leaveUserInfo.DatesEmployed) * 7;
                                            }
                                            if (njLeaveNum >= leavenum)
                                            {
                                                njLeaveNum = System.NumHelp.Round((njLeaveNum - leavenum) / 7, 1);
                                                leaveUserInfo.WinterVacation = (decimal)njLeaveNum;

                                                //更新
                                                userinfoBll.Update(leaveUserInfo);
                                            }
                                        }
                                        //学习假
                                        else if (leavetype == MorSun.Common.类别.Reference.请假_学习假)
                                        {
                                            //学习假
                                            var xxLeaveNum = (double)(leaveUserInfo.StudyLeave != null ? leaveUserInfo.StudyLeave : 0) * 7;
                                            if (leaveUserInfo.StudyLeave == null)
                                            {
                                                //计算学习假
                                                xxLeaveNum = MorSun.Controllers.UserController.GetStudentLeave((DateTime)leaveUserInfo.DatesEmployed) * 7;
                                            }
                                            if (xxLeaveNum >= leavenum)
                                            {
                                                xxLeaveNum = System.NumHelp.Round((xxLeaveNum - leavenum) / 7, 1);
                                                leaveUserInfo.StudyLeave = (decimal)xxLeaveNum;

                                                //更新
                                                userinfoBll.Update(leaveUserInfo);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return "1";
                    }
                }
            }
            return "0";
        }

        //请假时间计算，通过关联班次表
        public virtual string ajaxEProject_2(Guid UserID, DateTime BeginTime, DateTime EndTime)
        {
            var hours = 0.0;
            //请假时间计算，通过关联班次表

            var userid = UserID;
            //var zcb = Guid.Parse(MorSun.Common.类别.Reference.正常班);

            var begintime = DateTime.Parse(BeginTime.ToString("yyyy-MM-dd"));
            var endtime = DateTime.Parse(EndTime.ToString("yyyy-MM-dd"));
            var sjc = (endtime - begintime).Days;

            var userClasspLanSC = new MorSun.Controllers.UserDefaultScheduleTemplatesController();

            //如果请假前 不存在班次则通过排版模版生成班次
            userClasspLanSC.AutoGenerateScheduleTableByUserID(begintime, userid, false);

            //如果请假后 不存在班次则通过排版模版生成班次
            userClasspLanSC.AutoGenerateScheduleTableByUserID(endtime, userid, false);

            //排班表
            var kqClassPlanbll = new BaseBll<kqClassPlan>();
            //获取该用户指定时间段内的班次
            IQueryable<kqClassPlan> userkqClassPlanList = null;
            if (sjc >= 1)
            {
                userkqClassPlanList = kqClassPlanbll.All.Where(p => p.UserId == userid && p.PlanDate >= begintime && p.PlanDate <= endtime).OrderBy(p => p.Sort);
            }
            else
            {
                userkqClassPlanList = kqClassPlanbll.All.Where(p => p.UserId == userid && p.PlanDate == begintime).OrderBy(p => p.Sort);
            }

            //排班表
            if (userkqClassPlanList != null && userkqClassPlanList.Count() > 0)
            {
                //班次类型对应表
                var kqClassesRelationbll = new BaseBll<kqClassesRelation>();
                var kqClassesRelationList = kqClassesRelationbll.All.Where(k => k.FlagTrashed == false);

                var kqClassesSequencebll = new BaseBll<kqClassesSequence>();
                var kqClassesSequenceList = kqClassesSequencebll.All.Where(k => k.FlagTrashed == false);
                foreach (var userkqClassPlan in userkqClassPlanList)
                {
                    //班次时间
                    var planDate = userkqClassPlan.PlanDate;

                    //读取班次，只取正常班
                    //var userkqClassesSequenceList = kqClassesSequenceList.Where(p => p.ID == userkqClassPlan.CSId && p.CSRef == zcb).OrderBy(p => p.Sort);
                    var userkqClassesSequenceList = kqClassesSequenceList.Where(p => p.ID == userkqClassPlan.CSId).OrderBy(p => p.Sort);

                    //读取班次具体安排
                    if (userkqClassesSequenceList != null && userkqClassesSequenceList.Count() > 0)
                    {
                        foreach (var userkqClassesSequence in userkqClassesSequenceList)
                        {
                            //班次类型对应表数据
                            var userkqClassesRelationList = kqClassesRelationList.Where(p => p.CSId == userkqClassPlan.CSId).OrderBy(p => p.Sort);

                            if (userkqClassesRelationList != null && userkqClassesRelationList.Count() > 0)
                            {
                                //用户每天班次的具体安排时间
                                foreach (var userkqClassesRelation in userkqClassesRelationList)
                                {
                                    //具体班次类型
                                    var kqclassesref = userkqClassesRelation.kqClassesRef;

                                    //班次类型ID(空值为休息日)
                                    if (kqclassesref != null)
                                    {
                                        //班次开始时间
                                        var classesRefstartTime = DateTime.Parse(planDate.ToString("yyyy-MM-dd") + " " + kqclassesref.StartTime.ToString("HH:mm"));

                                        //班次结束时间
                                        var classesRefEndTime = DateTime.Parse(planDate.ToString("yyyy-MM-dd") + " " + kqclassesref.EndTime.ToString("HH:mm"));
                                        if (BeginTime <= classesRefEndTime && EndTime >= classesRefstartTime)
                                        {
                                            if (BeginTime >= classesRefstartTime)
                                            {
                                                classesRefstartTime = BeginTime;
                                            }

                                            if (EndTime <= classesRefEndTime)
                                            {
                                                classesRefEndTime = EndTime;
                                            }

                                            TimeSpan d1 = new TimeSpan(classesRefstartTime.Ticks);
                                            TimeSpan d2 = new TimeSpan(classesRefEndTime.Ticks);
                                            TimeSpan dd = d1.Subtract(d2).Duration();
                                            hours += dd.TotalMinutes;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                return "不存在排班表，请联系管理员！";
            }
            //返回小时数
            hours = hours / 60;
            return hours.ToString("f2");
        }

        //查询今天是否有值班(调班申请表中使用)
        public virtual string ajaxEProject_3(Guid u, DateTime t1)
        {
            var userid = u;
            var ttime = t1;
            var kqclassplanbll = new BaseBll<kqClassPlan>();
            var zcbcsref = Guid.Parse(MorSun.Common.类别.Reference.正常班);
            var kqclass = kqclassplanbll.All.Where(p => p.UserId == userid && p.PlanDate == ttime && p.kqClassesSequence.CSRef != zcbcsref).FirstOrDefault();
            if (kqclass != null)
            {
                return "1{error}" + kqclass.kqClassesSequence.CSRef;
            }
            else
            {
                return "0{error}您 " + ttime.ToString("yyyy-MM-dd") + " 没有值班信息，请重新选择调班时间！";
            }
        }

        //调休申请计算时间
        public virtual string ajaxEProject_4(Guid u, DateTime t1, DateTime t2)
        {
            var hours = 0.0;
            //请假时间计算，通过关联班次表

            var userid = u;
            //var zcb = Guid.Parse(MorSun.Common.类别.Reference.正常班);

            var begintime = DateTime.Parse(t1.ToString("yyyy-MM-dd"));
            var endtime = DateTime.Parse(t2.ToString("yyyy-MM-dd"));
            var sjc = (endtime - begintime).Days;


            var userClasspLanSC = new MorSun.Controllers.UserDefaultScheduleTemplatesController();

            //如果请假前 不存在班次则通过排版模版生成班次
            userClasspLanSC.AutoGenerateScheduleTableByUserID(begintime, userid, false);

            //如果请假后 不存在班次则通过排版模版生成班次
            userClasspLanSC.AutoGenerateScheduleTableByUserID(endtime, userid, false);

            //排班表
            var kqClassPlanbll = new BaseBll<kqClassPlan>();

            //获取该用户指定时间段内的班次
            IQueryable<kqClassPlan> userkqClassPlanList = null;
            if (sjc >= 1)
            {
                userkqClassPlanList = kqClassPlanbll.All.Where(p => p.UserId == userid && p.PlanDate >= begintime && p.PlanDate <= endtime).OrderBy(p => p.Sort);
            }
            else
            {
                userkqClassPlanList = kqClassPlanbll.All.Where(p => p.UserId == userid && p.PlanDate == begintime).OrderBy(p => p.Sort);
            }

            //排班表
            if (userkqClassPlanList != null && userkqClassPlanList.Count() > 0)
            {
                //班次类型对应表
                var kqClassesRelationbll = new BaseBll<kqClassesRelation>();
                var kqClassesRelationList = kqClassesRelationbll.All.Where(k => k.FlagTrashed == false);

                var kqClassesSequencebll = new BaseBll<kqClassesSequence>();
                var kqClassesSequenceList = kqClassesSequencebll.All.Where(k => k.FlagTrashed == false);
                foreach (var userkqClassPlan in userkqClassPlanList)
                {
                    //班次时间
                    var planDate = userkqClassPlan.PlanDate;

                    //读取班次，只取正常班
                    var userkqClassesSequenceList = kqClassesSequenceList.Where(p => p.ID == userkqClassPlan.CSId).OrderBy(p => p.Sort);

                    //读取班次具体安排
                    if (userkqClassesSequenceList != null && userkqClassesSequenceList.Count() > 0)
                    {
                        foreach (var userkqClassesSequence in userkqClassesSequenceList)
                        {
                            //班次类型对应表数据
                            var userkqClassesRelationList = kqClassesRelationList.Where(p => p.CSId == userkqClassPlan.CSId).OrderBy(p => p.Sort);

                            if (userkqClassesRelationList != null && userkqClassesRelationList.Count() > 0)
                            {
                                //用户每天班次的具体安排时间
                                foreach (var userkqClassesRelation in userkqClassesRelationList)
                                {
                                    //具体班次类型
                                    var kqclassesref = userkqClassesRelation.kqClassesRef;

                                    //班次类型ID(空值为休息日)
                                    if (kqclassesref != null)
                                    {
                                        //班次开始时间
                                        var classesRefstartTime = DateTime.Parse(planDate.ToString("yyyy-MM-dd") + " " + kqclassesref.StartTime.ToString("HH:mm"));

                                        //班次结束时间
                                        var classesRefEndTime = DateTime.Parse(planDate.ToString("yyyy-MM-dd") + " " + kqclassesref.EndTime.ToString("HH:mm"));
                                        if (t1 <= classesRefEndTime && t2 >= classesRefstartTime)
                                        {
                                            if (t1 >= classesRefstartTime)
                                            {
                                                classesRefstartTime = t1;
                                            }

                                            if (t2 <= classesRefEndTime)
                                            {
                                                classesRefEndTime = t2;
                                            }

                                            TimeSpan d1 = new TimeSpan(classesRefstartTime.Ticks);
                                            TimeSpan d2 = new TimeSpan(classesRefEndTime.Ticks);
                                            TimeSpan dd = d1.Subtract(d2).Duration();
                                            hours += dd.TotalMinutes;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                return "不存在排班表，请联系管理员！";
            }
            //返回小时数
            hours = hours / 60;
            return hours.ToString("f2");
        }

        //年假计算
        public virtual string ajaxEProject_5(Guid u)
        {
            //用户ID 用户操作密码
            var userid = u;
            if (userid != null && userid != Guid.Empty)
            {
                var userinfoBll = new wmfUserInfoBll();
                var userinfo = userinfoBll.GetModel(userid);
                if (userinfo != null)
                {
                    return (userinfo.WinterVacation != null ? (userinfo.WinterVacation * 7).ToString("f1") : "0") + "," + (userinfo.StudyLeave != null ? (userinfo.StudyLeave * 7).ToString("f1") : "0");
                }
            }
            return "0";
        }
    }
}
