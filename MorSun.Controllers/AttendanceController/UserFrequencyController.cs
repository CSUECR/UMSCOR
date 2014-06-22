using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using System.Web.Mvc;
using MorSun.Controllers.ViewModel;
using HOHO18.Common.Model;

namespace MorSun.Controllers
{
    public class UserFrequencyController : BaseController<kqClassesRelation>
    {
        protected override string ResourceId
        {
            get { return MorSun.Common.Privelege.资源.班次管理; }
        }

        public virtual ActionResult IndexList(kqClassesRelationVModel t)
        {
            return View(t);
        }

        protected override string OnPreCreateCK(kqClassesRelation t)
        {
            var ret = string.Empty;

            var l = new kqClassesRelationVModel().All;
            var model = l.Where(p=>p.CRId==t.CRId&&p.CSId==t.CSId).FirstOrDefault();
            if (model != null)
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<kqClassesRelation>("该班次已存在"), "") });
            }
            ret = "true";
            return ret;
        }

        protected override string OnEditCK(kqClassesRelation t)
        {
            var ret = string.Empty;
            var l = new kqClassesRelationVModel().All;
            var model = l.Where(p => p.CRId == t.CRId && p.CSId == t.CSId && p.ID != t.ID).FirstOrDefault();
            if (model != null)
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<kqClassesRelation>("班次类型已存在"), "") });
            }
            ret = "true";
            return ret;
        }
    }
}
