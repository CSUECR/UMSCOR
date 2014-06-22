using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using MorSun.Controllers.ViewModel;
using System.Web.Mvc;
using HOHO18.Common.Model;
using MorSun.Bll;

namespace MorSun.Controllers.CommonController
{
    public class UserDutyController : BaseController<kqClassPlan>
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
                            var list = new kqClassPlanVModel().All;
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
            var classBll = new BaseBll<KQDELETESCHEDULE>();
            var dateStr = model.PlanDate.Value.ToShortDateString();
            var dateFrom = DateHelp.ToDateFrom(dateStr);
            var dateTo = DateHelp.ToDateTo(dateStr);
            var classModel = new kqDeleteScheduleVModel().All.Where(p => p.PLANDATE >= dateFrom && p.PLANDATE <= dateTo).FirstOrDefault();
            if (classModel != null)
            {
                model.CSId = classModel.CSID;
            }
            else
            {
                model.CSId = null;
            }
            bll.Update(model);
            return "true";
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
        
    }
}
