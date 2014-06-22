using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using MorSun.Controllers.ViewModel;
using HOHO18.Common.Model;
using System.Web.Mvc;
using MorSun.Bll;

namespace MorSun.Controllers
{
    public class UserScheduleController : BaseController<kqCalssPlanTemplate>
    {
        protected override string ResourceId
        {
            get { return MorSun.Common.Privelege.资源.排班模板; }
        }

        public override ActionResult Add(kqCalssPlanTemplate t)
        {
            var pindex = t.PIndex;
            var preUrl = t.PreUrl;
            t.PreUrl = "/UserSchedule/" + t.PreUrl;
            if (t.PreUrl != null && t.PreUrl.IndexOf("PreUrl") != -1)
            {
                var i = t.PreUrl.IndexOf("PreUrl");
                t.PreUrl = t.PreUrl.Substring(i + 7);
            }
            return View(t);
        }

        public override string Create(kqCalssPlanTemplate t)
        {
            var ret = string.Empty;
            ret=base.Create(t);
            if (ret == "true")
            {
                var num = t.Period;
                var num1 = t.PeriodNum1;
                var num2 = t.PeriodNum2;
                for (int i = 0; i < num1; i++)
                {
                    if (t.CSId != null)
                    {
                        AddkqCPTCS(t.CSId, t.ID, i + 1);
                    }

                }
                for (int i = 0; i < num - num1; i++)
                {
                    var sort = int.Parse(num1.Value.ToString());
                    if (t.CSId2 != null)
                    {
                        AddkqCPTCS(t.CSId2, t.ID, sort + i + 1);
                    }

                }
            }
            if (ret == "true")
            {
                ret = "true";
            }
            return ret; 
        }

        public void AddkqCPTCS(Guid CSId, Guid CPTId,int sort)
        {
            var model = new kqCPTC();
            var Bll = new BaseBll<kqCPTC>();
            model.ID = Guid.NewGuid();
            model.Sort = sort;
            model.RegTime = DateTime.Now;
            model.CSId = CSId;
            model.CPTId = CPTId;
            Bll.Insert(model);
        }

        protected override string OnPreCreateCK(kqCalssPlanTemplate t)
        {
            var ret = string.Empty;
            List<RuleViolation> errs=new List<RuleViolation>();
            if (!ModelStateValidate.IsNum(t.PeriodNum1.ToString()))
                errs.Add(new RuleViolation(XmlHelper.GetKeyNameValidation<kqCalssPlanTemplate>("上班天数只能输入数字"), "PeriodNum1"));
            if (!ModelStateValidate.IsNum(t.PeriodNum2.ToString()))
                errs.Add(  new RuleViolation(XmlHelper.GetKeyNameValidation<kqCalssPlanTemplate>("休息天数只能输入数字"), "PeriodNum2"));
            if (t.PeriodNum1.HasValue)
            {
                if (t.PeriodNum1.Value > t.Period)
                {
                    errs.Add( new RuleViolation(XmlHelper.GetKeyNameValidation<kqCalssPlanTemplate>("上班天数小于等于周期"), "PeriodNum1"));
                }
            }
            if (t.PeriodNum2.HasValue)
            {
                if (t.PeriodNum2.Value > t.Period)
                {
                    errs.Add(new RuleViolation(XmlHelper.GetKeyNameValidation<kqCalssPlanTemplate>("休息天数小于等于周期"), "PeriodNum1"));
                }
            }
            if (t.PeriodNum1.HasValue && t.PeriodNum2.HasValue && t.Period.Value != t.PeriodNum1.Value + t.PeriodNum2.Value)
            {
                errs.Add(new RuleViolation(XmlHelper.GetKeyNameValidation<kqCalssPlanTemplate>("上班天数+休息天数=周期"), "PeriodNum1,PeriodNum2"));
            }
            var l = new kqCalssPlanTemplateVModel().All;
            var model = l.Where(p => p.TempName == t.TempName).FirstOrDefault();
            if (model != null)
            {
                errs.Add(new RuleViolation(XmlHelper.GetKeyNameValidation<kqCalssPlanTemplate>("排班模板已存在"), ""));
            }
            if (errs.Count > 0)
            {
                return getErrListJson(errs.AsEnumerable());
            }
            ret = "true";
            return ret;
        }

        protected override string OnEditCK(kqCalssPlanTemplate t)
        {
            var ret = string.Empty;
            var l = new kqCalssPlanTemplateVModel().All;
            var model = l.Where(p => p.TempName == t.TempName && p.ID != t.ID).FirstOrDefault();
            if (model != null)
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<kqCalssPlanTemplate>("排班模板已存在"), "") });
            }
            ret = "true";
            return ret;
        }
        protected override string OnBatchDelCk(kqCalssPlanTemplate t)
        {
            var ret = string.Empty;

            var l = new kqDUCPTVModel().All;
            var model = l.Where(p => p.CPTId == t.ID).FirstOrDefault();
            if (model != null)
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<kqCalssPlanTemplate>("排班模板已被使用"), "") });
            }
            ret = "true";
            return ret;
        }
    }
}
