
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using MorSun.Controllers.ViewModel;
using HOHO18.Common.Model;
using MorSun.Bll;
using HOHO18.Common;

namespace MorSun.Controllers
{
    public class UserDefaultScheduleTemplatesController : BaseController<kqDUCPT>
    {
        protected override string ResourceId
        {
            get { return MorSun.Common.Privelege.资源.默认排班模板; }
        }
        protected override string OnPreCreateCK(kqDUCPT t)
        {
            var ret = string.Empty;
            List<RuleViolation> errs = new List<RuleViolation>();
            var l = new kqDUCPTVModel().List;
            var MohtnIndates = string.Empty;
            var referList = new ReferListVModel().List;
            var modelList = l.Where(p => p.DeptId == t.DeptId);
            if (t.UserId != null && t.UserId != Guid.Empty)
            {
                modelList = modelList.Where(p => p.UserId == t.UserId);
            }
            else
            {
                modelList = modelList.Where(p => p.UserId == null);
            }
            if (modelList != null)
            {
                foreach (var item in modelList)
                {
                    MohtnIndates += item.MohtnIndate;
                }
                var tempString = string.Empty;
                if (string.IsNullOrEmpty(t.MohtnIndate))
                {
                    errs.Add(new RuleViolation(XmlHelper.GetKeyNameValidation<kqDUCPT>("请选择有效月份"), "MohtnIndates"));
                }
                else
                {
                    var strings = t.MohtnIndate.Split(',');
                    for (int i = 0; i < strings.Length - 1; i++)
                    {
                        if (MohtnIndates.Contains(strings[i]))
                        {
                            var ID = Guid.Parse(strings[i]);
                            tempString += referList.Where(p=>p.ID==ID).FirstOrDefault().ItemInfo+",";
                        }
                    } 
                    if (tempString != string.Empty)
                    {
                        tempString = tempString.TrimEnd(',');
                        errs.Add(new RuleViolation(string.Format(XmlHelper.GetKeyNameValidation<kqDUCPT>("该月份已存在"), tempString), "MohtnIndates"));
                    }
                }
            }
            if (errs.Count > 0)
            {
                return getErrListJson(errs.AsEnumerable());
            }
            ret = "true";
            return ret;
        }

        protected override string OnEditCK(kqDUCPT t)
        {
            var ret = string.Empty;
            List<RuleViolation> errs = new List<RuleViolation>();
            var l = new kqDUCPTVModel().List;
            var MohtnIndates = string.Empty;
            var referList = new ReferListVModel().List;
            var modelList = l.Where(p => p.DeptId == t.DeptId && p.ID != t.ID);
            if (!ParameterHelper.IsNullOrEmpty(t.UserId))
            {
                modelList = modelList.Where(p => p.UserId == t.UserId);
            }
            else
            {
                modelList = modelList.Where(p => p.UserId == null);
            }
            if (modelList != null)
            {
                foreach (var item in modelList)
                {
                    MohtnIndates += item.MohtnIndate;
                }
                var tempString = string.Empty;
                if (string.IsNullOrEmpty(t.MohtnIndate))
                {
                    errs.Add(new RuleViolation(XmlHelper.GetKeyNameValidation<kqDUCPT>("请选择有效月份"), "MohtnIndates"));
                }
                else
                {
                    var strings = t.MohtnIndate.Split(',');
                    for (int i = 0; i < strings.Length - 1; i++)
                    {
                        if (MohtnIndates.Contains(strings[i]))
                        {
                            var ID = Guid.Parse(strings[i]);
                            tempString += referList.Where(p=>p.ID==ID).FirstOrDefault().ItemInfo+",";
                        }
                    } 
                    if (tempString != string.Empty)
                    {
                        tempString = tempString.TrimEnd(',');
                        errs.Add(new RuleViolation(string.Format(XmlHelper.GetKeyNameValidation<kqDUCPT>("该月份已存在"), tempString), "MohtnIndates"));
                    }
                }
            }
            if (errs.Count > 0)
            {
                return getErrListJson(errs.AsEnumerable());
            }
            ret = "true";
            return ret;
        }

        #region 生成排班模板

        #region 自动通过用户GUID生成排班表
        /// <summary>
        /// 自动通过用户GUID生成排班表
        /// </summary>
        /// <param name="date">排班日期</param>
        /// <param name="userId">用户ID</param>
        /// <param name="isGenerate">强制重新生成排班表</param>
        /// <returns></returns>
        public string AutoGenerateScheduleTableByUserID(DateTime date, Guid userId, bool isGenerate)
        {
            var ret = string.Empty;
            ret=IsGenerateSchudle(date, userId);//该月有排班
            if (isGenerate||ret != "true")
            {
                ret = GenerateScheduleTableByUserID(date, userId, false,isGenerate);
            }
            return ret;
        }
        #endregion

        #region 自动通过部门GUID生成排班表
        /// <summary>
        /// 自动通过部门GUID生成排班表
        /// </summary>
        /// <param name="date">排班日期</param>
        /// <param name="deptId">部门ID</param>
        /// <returns></returns>
        public string AutoGenerateScheduleTableByDeptID(DateTime date, Guid deptId,bool isGenerate)
        {
            var ret = string.Empty;
            ret = GenerateScheduleTableByDeptID(date, deptId, false, isGenerate);
            return ret;
        } 
        #endregion

        #region 通过用户GUID重新生成排班表
        /// <summary>
        /// 通过用户GUID重新生成排班表
        /// </summary>
        /// <param name="date">排班日期</param>
        /// <param name="userId">部门ID</param>
        /// <returns></returns>
        public string GenerateScheduleTableByUserID(DateTime date, Guid userId,bool isGenerate)
        {
            var ret = string.Empty;
            ret = GenerateScheduleTableByUserID(date, userId, true,isGenerate);
            return ret;
        } 
        #endregion

        #region 通过部门GUID重新生成排班表
        /// <summary>
        /// 通过部门GUID重新生成排班表
        /// </summary>
        /// <param name="date">排班日期</param>
        /// <param name="deptId">部门ID</param>
        /// <returns></returns>
        public string GenerateScheduleTableByDeptID(DateTime date, Guid deptId,bool isGenerate)
        {
            var ret = string.Empty;
            ret = GenerateScheduleTableByDeptID(date, deptId, true, isGenerate);
            return ret;
        } 
        #endregion

        #region 通过用户ID生成排班表
        /// <summary>
        /// 通过用户ID生成排班表
        /// </summary>
        /// <param name="date">排班日期</param>
        /// <param name="userId">用户ID</param>
        /// <param name="bReSchedule">是否重新生成排班表</param>
        /// <returns></returns>
        private string GenerateScheduleTableByUserID(DateTime date, Guid userId, bool bReSchedule,bool isReGenerate=false)
        {
            var ret = string.Empty;
            //通过用户GUID获取部门GUID
            var deptId = GetDeptIdByUserId(userId);
            ret = GenerateScheduleTable(date, deptId, userId, bReSchedule, isReGenerate);
            return ret;
        } 
        #endregion

        #region 通过部门ID生成排班表
        /// <summary>
        /// 通过部门ID生成排班表
        /// </summary>
        /// <param name="date">排班日期</param>
        /// <param name="deptId">部门ID</param>
        /// <param name="bReSchedule">是否重新生成排班表</param>
        /// <returns></returns>
        private string GenerateScheduleTableByDeptID(DateTime date, Guid deptId, bool bReSchedule,bool isGenerate)
        {
            var ret = string.Empty;
            var userList = GetUserByDeptId(deptId);
            for (int i = 0; i < userList.Count; i++)
            {
                ret = GenerateScheduleTable(date, userList[i].deptId, userList[i].ID, bReSchedule,isGenerate);
            }          
            return ret;
        } 
        #endregion

        #region 生成排班
        /// <summary>
        /// 生成排班
        /// </summary>
        /// <param name="date">排班时间</param>
        /// <param name="deptId">部门ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="bReSchedule">是否重新排班</param>
        /// <returns></returns>
        private string GenerateScheduleTable(DateTime? date, Guid? deptId, Guid? userId, bool bReSchedule,bool isReGenerate)
        {
            var ret = string.Empty;
            var dates = GenerateDays(date.Value);//当月的日期
            var tempDates = dates[0];
            //默认排版模板的id集合
            var templateIDList = new List<Guid>();//默认排班模板
            //排版模板的id集合
            var templateList = GetDafautTemplates(ref templateIDList, date, deptId, userId);//排班模板

            if (templateList.Count>0)
            {
                var holidayList = new List<DateTime>();
                var classList = new List<Guid>();
                var weekFlag = IsWeekPeriod(templateList[0]);
                //是否取消节假日
                var flag = IsRejectHoliday(templateIDList[0]);
                if (flag)
                {
                    classList = RemoveHoliday(ref dates, ref holidayList, weekFlag);
                }
                var lists = GetTemplate(templateList[0]);
                var period = lists.Count;//周期
                var incrementNum = 0;
                if (weekFlag)//是否周期为周
                {
                    incrementNum = dates[0] != DateTime.MinValue ? GetWeek(dates[0]) - 1 : GetWeek(tempDates) - 1;
                }
                if (!bReSchedule && !weekFlag)//不重新排班
                {
                    var iPreClass = GetPreMonthClass(date.Value, period, weekFlag);//上个月排到第几班次
                    incrementNum = iPreClass;
                }
                GenerateCommonClassPlan(dates, lists, incrementNum, period, userId, isReGenerate);
                GenerateHolidayClassPlan(holidayList, classList, userId, isReGenerate);
                ret = "true";
            }
            else
            {
                var url = string.Empty;
                if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.添加))
                {
                    ret = XmlHelper.GetKeyNameValidation<kqDUCPT>("默认排班模板不存在");
                    url = XmlHelper.GetKeyNameValidation<kqDUCPT>("默认排班模板不存在url");
                }
                else
                {
                    ret = "无权限";
                    url = "";
                }               
                ret = "{ \"ErrorMessage\": \"" + ret + "\", \"URL\": \"" + url + "\", \"Type\": \"error\" }";
            }
            return ret;
        }
        #endregion

        #region 正常排班
        /// <summary>
        /// 正常排班
        /// </summary>
        /// <param name="dateList">排班时间列表</param>
        /// <param name="classList">班次列表</param>
        /// <param name="incrementNum">跳过班次</param>
        /// <param name="period">班次周期</param>
        /// <param name="userId">用户ID</param>
        public void GenerateCommonClassPlan(List<DateTime> dateList, List<Guid> classList, int incrementNum, int period, Guid? userId, bool isReGenerate = false)
        {
            for (int i = 0; i < dateList.Count; i++)//正常排班
            {
                var date1 = dateList[i];
                if (dateList[i] != DateTime.MinValue)
                {
                    GenerateClassPlan(date1, userId.Value, classList[(i + incrementNum) % period], isReGenerate);
                }
            }
        } 
        #endregion

        #region 节假日排班
        /// <summary>
        /// 节假日排班
        /// </summary>
        /// <param name="dateList">排班时间列表</param>
        /// <param name="classList">班次列表</param>
        /// <param name="userId">用户ID</param>
        public void GenerateHolidayClassPlan(List<DateTime> dateList, List<Guid> classList, Guid? userId, bool isReGenerate = false)
        {
            for (int i = 0; i < dateList.Count; i++)
            {
                var date1 = dateList[i];
                if (!ParameterHelper.IsNullOrEmpty(classList[i]))
                {
                    GenerateClassPlan(date1, userId.Value, classList[i], isReGenerate);
                }
            }
        } 
        #endregion

        #region 根据时间生成时间列表
        /// <summary>
        /// 根据时间生成时间列表
        /// </summary>
        /// <param name="date">时间</param>
        /// <returns></returns>
        public List<DateTime> GenerateDays(DateTime date)
        {
            var lists = new List<DateTime>();
            var year = date.Year;
            var month = date.Month;
            var day = date.Day;
            var iWeek = Convert.ToInt32(date.DayOfWeek);
            var iDays = DateTime.DaysInMonth(year, month);
            var num = 1;
            for (int i = day; i <= iDays; i++)
            {
                if (i == day)
                {
                    lists.Add(date);
                }
                else
                {
                    lists.Add(date.AddDays(num));
                    num++;
                }
            }
            return lists;
        } 
        #endregion

        #region 上个月排到哪个班次
        /// <summary>
        /// 上个月排到哪个班次
        /// </summary>
        /// <param name="date">时间</param>
        /// <param name="period">周期</param>
        /// <returns></returns>
        public int GetPreMonthClass(DateTime date, int period, bool weekFlag)
        {
            var ret = 0;
            var preDates = GenerateDays(date.AddMonths(-1));//当月的日期
            var holidayList = new List<DateTime>();
            RemoveHoliday(ref preDates, ref holidayList, weekFlag);
            ret = preDates.Count % period;
            return ret;
        } 
        #endregion

        #region 判断是否是节假日
        /// <summary>
        /// 判断是否是节假日
        /// </summary>
        /// <param name="date">时间</param>
        /// <param name="holiday">节假日时间</param>
        /// <param name="weekFlag">是否周期为周</param>
        /// <returns></returns>
        public List<Guid> RemoveHoliday(ref List<DateTime> date, ref List<DateTime> holiday, bool weekFlag)
        {
            var l = new kqHolidayVModel().List;
            var holidayList = new List<DateTime>();
            var classList = new List<Guid>();
            var dateFrom = DateHelp.ToDateFrom(date[0].AddMonths(-1).MinDay().ToShortDateString());
            var dateTo = DateHelp.ToDateTo(date[0].AddMonths(1).MaxDay().ToShortDateString());
            for (int i = 0; i < date.Count; i++)
            {
                var model = l.Where(p => p.StartTime >= dateFrom && p.EndTime <= dateTo);
                if (model.FirstOrDefault() != null)
                {
                    foreach (var item in model)
                    {
                        var startTime = item.StartTime;
                        var endTime = item.EndTime;
                        if (startTime <= date[i] && endTime >= date[i])
                        {
                            holidayList.Add(date[i]);
                            date[i] = DateTime.MinValue;
                            classList.Add(item.CSId.HasValue ? item.CSId.Value : Guid.Empty);
                        }
                    }
                }
            }
            if (!weekFlag)
            {
                date = date.Except(holidayList).ToList();
            }
            holiday = holidayList;
            return classList;
        } 
        #endregion

        #region 通过时间，部门、人员查找排班模板
        //通过时间，部门、人员查找排班模板
        public List<Guid> GetDafautTemplates(ref List<Guid> templateIDList, DateTime? date, Guid? deptId, Guid? userId)
        {
            //结果
            var ret = new List<Guid>();
            //格式化 月份
            var month = string.Empty;
            if (date != null)
            {
                month = date.Value.Month.ToString() + "月";
            }
            //默认排班模板的VModel

            var l = new kqDUCPTVModel().List;
            //排除标记删除的集合  2013/6/19 lj
            l = l.Where(u => u.FlagDeleted != true && u.FlagTrashed != true);
            //通过月份名称，获取该月份对应的guid
            var ID = GetReferGuidByName(month);
            //部门id为空则直接返回
            if (ParameterHelper.IsNullOrEmpty(deptId))
            {
                return ret;
            }
            //通过默认排版 模板，部门，userid，月份id获取对应的默认排版模板；
            var models = GetModels(l,userId,deptId.Value,ID);

            if (models.FirstOrDefault() != null)
            {
                ret.Add(models.FirstOrDefault().CPTId.Value);
                templateIDList.Add(models.FirstOrDefault().ID);
            }
            else
            {
                var parentId = GetParentDeptId(deptId.Value);
                ret = GetDafautTemplates(ref templateIDList, date, parentId, null);
            }
            return ret;
        }
       /// <summary>
       /// 
       /// </summary>
       /// <param name="list"></param>
       /// <param name="userId"></param>
       /// <param name="deptId"></param>
       /// <param name="ID"></param>
       /// <returns></returns>
        public IQueryable<kqDUCPT> GetModels(IQueryable<kqDUCPT> list, Guid? userId, Guid deptId,string ID)
        {
            //set ret default value
            var ret = default(IQueryable<kqDUCPT>);
            //通过部门id、当前用户id、当前月份，筛选默认排版的模板
            ret = list.Where(p => p.DeptId == deptId && p.UserId == userId && p.MohtnIndate.Contains(ID));
            if (ret.FirstOrDefault() == null)
            {
                ret = list.Where(p => p.DeptId == deptId && p.UserId == null && p.MohtnIndate.Contains(ID));
            }
            return ret;
        }

        #endregion

        #region 通过排班模板获取班次
        //通过排班模板获取班次
        public List<Guid> GetTemplate(Guid ID)
        {
            var ret = new List<Guid>();
            var l = new kqCPTCSVModel().All.OrderBy(p => p.Sort);
            var models = l.Where(p => p.CPTId == ID);
            if (models.FirstOrDefault() == null)
            {
                return ret;
            }
            foreach (var item in models)
            {
                ret.Add(item.CSId);
            }
            return ret;
        } 
        #endregion

        #region 通过班次获取班次类型
        //通过班次获取班次类型
        public List<Guid> GetClassesRef(Guid ID, ref string guidType)
        {
            var ret = new List<Guid>();
            var l = new kqClassesRelationVModel().List;
            var models = l.Where(p => p.CSId == ID);
            if (models.FirstOrDefault() == null)
            {
                return ret;
            }
            foreach (var item in models)
            {
                if (item.CRId.HasValue)
                {
                    ret.Add(item.CRId.Value);
                }
                else
                {
                    guidType = "CSId";
                    ret.Add(item.CSId);
                }
            }

            return ret;
        } 
        #endregion

        #region 生成排班计划
        //生成排班计划
        public string GenerateClassPlan(DateTime date, Guid userId, Guid CSId, bool isReGenerate=false)
        {
            var ret = string.Empty;
            var Bll = new BaseBll<kqClassPlan>();
            var classBll = new BaseBll<KQDELETESCHEDULE>();
            var oldModel=new kqClassPlan();
            if (!HaveClassRecord(date, userId, CSId,ref oldModel,isReGenerate)||isReGenerate)
            {
                var model = new kqClassPlan();
                if (oldModel != null) { model.ManagerRemark = oldModel.ManagerRemark; model.PersonnelRemark = oldModel.PersonnelRemark; }
                model.ID = Guid.NewGuid();
                model.UserId = userId;
                model.CSId = CSId;
                model.PlanDate = date;
                model.RegTime = DateTime.Now;
                Bll.Insert(model);
            }
            var classModel = new KQDELETESCHEDULE();
            classModel.ID = Guid.NewGuid();
            classModel.RegUser = UserID;
            classModel.RegTime = DateTime.Now;
            classModel.PLANDATE = date;
            classModel.CSID = CSId;
            if (!HasDeletedSchedule(date))
            {
                classBll.Insert(classModel);
            }
            return ret;
        }

        private bool HasDeletedSchedule(DateTime date)
        {
            var ret = false;
            var dateStr = date.ToShortDateString();
            var dateFrom = DateHelp.ToDateFrom(dateStr);
            var dateTo = DateHelp.ToDateTo(dateStr);
            var model = new kqDeleteScheduleVModel().All.Where(p=>p.PLANDATE>=dateFrom&&p.PLANDATE<=dateTo).FirstOrDefault();
            if (model != null)
            {
                ret = true;
            }
            return ret;
        }

        public string GenerateClassPlanALL(kqClassPlan t)
        {
            var ret = string.Empty;
            if (!string.IsNullOrEmpty(t.CheckedId))
            {
                var userIds = t.CheckedId.Split(',');
                var Bll = new BaseBll<kqClassPlan>();
                var l = Bll.All;
                for (int i = 0; i < userIds.Length; i++)
                {
                    if (userIds[i] != "")
                    {
                        var userID = Guid.Parse(userIds[i]);
                        var list = new kqClassPlanVModel().All;
                        var model = list.Where(p => p.UserId == userID && p.PlanDate == t.PlanDate).FirstOrDefault();
                        if (model != null)
                        {
                            l.DeleteObject(model);
                        }
                        GenerateClassPlan(t.PlanDate.Value, userID, t.CSId.Value);
                    }
                }
            }
            return ret;
        }
        #endregion

        #region 已存在排班计划
        //已存在排班计划
        public bool HaveClassRecord(DateTime date, Guid userId, Guid csId, ref kqClassPlan oldModel, bool isReGenerate = false)
        {
            var ret = false;
            var l = new kqClassPlanVModel().All;
            var model = l.Where(p => p.UserId == userId && p.PlanDate == date);
            foreach (var item in model)
            {
                if (!string.IsNullOrEmpty(item.ManagerRemark) || !string.IsNullOrEmpty(item.PersonnelRemark))
                {
                    oldModel = item;
                }
                if (isReGenerate == true)
                {
                    ClearClassRecord(date, userId, item.CSId);
                }
                ret = true;
            }
            return ret;
        }
        public void ClearClassRecord(DateTime date, Guid userId, Guid? csId)
        {
            var Bll = new BaseBll<kqClassPlan>();
            var list = Bll.All;
            var l = new kqClassPlanVModel().All;
            var marksList = new List<kqClassPlan>();
            var dateFrom = date.MinDay().ToShortDateString().ToDateFrom();
            var dateTo = date.MaxDay().ToShortDateString().ToDateTo();
            var model = l.Where(p => p.UserId == userId && p.PlanDate == date && (p.CSId == csId||p.CSId==null));
            foreach (var item in model)
            {
                list.DeleteObject(item);
            }
        } 
        #endregion

        #region 是否取消节假日
        //是否取消节假日
        public bool IsRejectHoliday(Guid ID)
        {
            var ret = false;
            var templateModel = new kqDUCPTVModel().All.Where(p => p.ID == ID).FirstOrDefault();
            if (templateModel != null)
            {
                ret = templateModel.IsRejectHoliday;
            }
            return ret;
        } 
        #endregion

        #region 计算时间为星期几
        //计算时间为星期几
        public int GetWeek(DateTime date)
        {
            var ret = 0;
            var iWeek = date.DayOfWeek;
            ret = Convert.ToInt16(iWeek);
            ret = ret == 0 ? 7 : ret;
            return ret;
        } 
        #endregion

        #region 周期是否是周
        //周期是否是周
        public bool IsWeekPeriod(Guid ID)
        {
            var ret = false;
            var l = new kqCalssPlanTemplateVModel().All;
            var model = l.Where(p => p.ID == ID).FirstOrDefault();
            if (model != null)
            {
                ret = model.IsWeekPeriod;
            }
            return ret;
        } 
        #endregion

        #region 这个月该用户是否已经排班
        //这个月该用户是否已经排班
        public string IsGenerateSchudle(DateTime date, Guid userId)
        {
            var ret = string.Empty;
            var year = date.Year;
            var month = date.Month;
            var iday = DateTime.DaysInMonth(year, month);
            var dateFrom = DateHelp.ToDateFrom(year + "-" + month + "-01");
            var dateTo = DateHelp.ToDateTo(year + "-" + month + "-" + iday);
            var l = new kqClassPlanVModel().All;
            var model = l.Where(p => p.UserId == userId && p.PlanDate >= dateFrom && p.PlanDate <= dateTo);
            if (model != null&&model.Count()>=iday)
            {
                ret = "true";
            }
            else
            {
                var url = string.Empty;
                if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.添加))
                {
                    ret = XmlHelper.GetKeyNameValidation<kqDUCPT>("该月未排班不存在");
                    url = XmlHelper.GetKeyNameValidation<kqDUCPT>("该月未排班不存在url");

                }
                else
                {
                    ret = "无权限";
                     url = "";
                }      
                ret = "{ \"ErrorMessage\": \"" + ret + "\", \"URL\": \"" + url + "\", \"Type\": \"error\" }";
            }
            return ret;
        } 
        #endregion

        public string GetUserKQJson(string yearMonth)
        {
            var ret = string.Empty;
            string krStr = string.Empty;
            var date =DateTime.Parse(yearMonth);
            var startTime = date.MinDay().ToShortDateString().ToDateFrom();
            var endTime = date.MaxDay().ToShortDateString().ToDateTo();
            var l = new kqClassPlanVModel().All;
            var ID = Guid.Parse(MorSun.Common.类别.Reference.周末值班);
            var ID1 = Guid.Parse(MorSun.Common.类别.Reference.值班);
            var ID2 = Guid.Parse(MorSun.Common.类别.Reference.节假日值班);
            var models = l.Where(p => p.PlanDate >= startTime && p.PlanDate <= endTime &&(p.kqClassesSequence.CSRef == ID||p.kqClassesSequence.CSRef==ID1||p.kqClassesSequence.CSRef==ID2));
            var i = 1;
            foreach (var item in models)
            {
                var content = item.kqClassesSequence.wmfReference != null ? item.kqClassesSequence.wmfReference.ItemInfo : item.kqClassesSequence.CSName;
                var userName=item.aspnet_Users == null ? "" : item.aspnet_Users.wmfUserInfo == null ? "" : item.aspnet_Users.wmfUserInfo.TrueName;
                krStr += "{ \"container\": \"#jMonthCalendar\", \"head\": \"#CalendarHead\", \"body\": \"#CalendarBody\", \"EventID\": " + i + ", \"StartDateTime\": \"" + item.PlanDate.ToString("yyyy-MM-dd") + "\", \"Title\": \"" + content + ":" + userName + "\", \"URL\": \"" + item.ID + "," + item.CSId + "," + item.UserId + "\", \"CssClass\": \"\" }";
                if (i != models.Count())
                {
                    krStr += ",";
                }
                i++;
            }
            ret = "[" + krStr + "]";
            return ret;
        }

        #endregion

    }
}
