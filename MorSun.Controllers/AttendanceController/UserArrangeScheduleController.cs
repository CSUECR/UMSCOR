using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using System.Web.Mvc;
using MorSun.Controllers.ViewModel;
using MorSun.Bll;
using HOHO18.Common.Model;

namespace MorSun.Controllers.CommonController
{
    public class UserArrangeScheduleController : BaseController<kqClassPlan>
    {
        protected override string ResourceId
        {
            get { return MorSun.Common.Privelege.资源.安排值班; }
        }

        public override ActionResult Update(kqClassPlan t)
        {
            t = new kqClassPlanVModel().All.Where(r => r.ID == t.ID).FirstOrDefault();

            return View(t);
        }

        public override string Create(kqClassPlan t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.添加))
            {
                string msg = "";

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
                            var list = new kqClassPlanVModel().List;
                            var newModel = new kqClassPlan();
                            var model = list.Where(p => p.UserId == userID && p.PlanDate == t.PlanDate);
                            foreach (var item in model)
                            {
                                if (!string.IsNullOrEmpty(item.PersonnelRemark))
                                {
                                    newModel.PersonnelRemark = item.PersonnelRemark;
                                }
                                if (!string.IsNullOrEmpty(item.ManagerRemark))
                                {
                                    newModel.ManagerRemark = item.ManagerRemark;
                                }
                                l.DeleteObject(item);
                            }
                            newModel.ID = Guid.NewGuid();
                            newModel.UserId = userID;
                            newModel.CSId = t.CSId;
                            newModel.PlanDate = t.PlanDate;
                            newModel.RegTime = DateTime.Now;
                            l.AddObject(newModel);
                        }
                    }
                    Bll.UpdateChanges();
                }
                else
                {
                    return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<kqClassPlan>("请选择人员"), "") });
                }
                msg = "true";
                return msg;
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
            }
        }
        public override string Delete(kqClassPlan t)
        {
            var bll = new BaseBll<kqClassPlan>();
            var model = bll.GetModel(t);
            model.CSId = null;
            bll.Update(model);
            return "true";
        }

        public string AutoGenerateScheduleTableByUserID(DateTime date, Guid userId, bool isGenerate)
        {
           return new UserDefaultScheduleTemplatesController().AutoGenerateScheduleTableByUserID(date,userId,isGenerate);
        }

        public override ActionResult Index()
        {
            var vModel = new kqClassPlanVModel();
            FillModel(vModel);
            var right = (vModel.right == "false");

            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.查看) || right)
            {
                return View(vModel);
            }
            else
            {
                return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
            }

        }

        public string GetUserKQJson(string yearMonth,Guid userId,string right)
        {
            var ret = string.Empty;
            string krStr = string.Empty;
            var date = DateTime.Parse(yearMonth);
            var startTime = date.MinDay().ToShortDateString().ToDateFrom();
            var endTime = date.MaxDay().ToShortDateString().ToDateTo();
            var l = new kqClassPlanVModel().List;
            var ID = Guid.Parse(MorSun.Common.类别.Reference.正常班);
            var models = l.Where(p => p.PlanDate >= startTime && p.PlanDate <= endTime&&p.UserId==userId);
            var i = 1;
            foreach (var item in models)
            {
                var content = item.CSId == null ? "" : item.kqClassesSequence.wmfReference != null ? item.kqClassesSequence.wmfReference.ItemInfo : item.kqClassesSequence.CSName;
                krStr += "{ \"container\": \"#jMonthCalendar\", \"head\": \"#CalendarHead\", \"body\": \"#CalendarBody\", \"EventID\": " + i + ", \"StartDateTime\": \"" + item.PlanDate.ToString("yyyy-MM-dd") + "\", \"Title\": \"" + content + "\", \"URL\": \"" + item.ID + "," + item.CSId + "," + item.UserId + "\", \"CssClass\": \"\" }";
                if (i != models.Count())
                {
                    krStr += ",";
                }
                i++;
            }
            ret = "[" + krStr + "]";
            return ret;
        }
    }
}
