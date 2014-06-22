using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MorSun.Model;
using MorSun.Controllers.ViewModel;
using HOHO18.Common.Model;

namespace MorSun.Controllers.CommonController
{
    public class UserClassSequenceController : BaseController<kqClassesSequence>
    {
        protected override string ResourceId
        {
            get { return MorSun.Common.Privelege.资源.班次; }
        }

        public override ActionResult Add(kqClassesSequence t)
        {
            var pindex = t.PIndex;
            var preUrl = t.PreUrl;
            t.PreUrl = "/UserClassSequence/" + t.PreUrl;
            if (t.PreUrl != null && t.PreUrl.IndexOf("PreUrl") != -1)
            {
                var i = t.PreUrl.IndexOf("PreUrl");
                t.PreUrl = t.PreUrl.Substring(i + 7);
            }
            return View(t);
        }

        protected override string OnPreCreateCK(kqClassesSequence t)
        {
            var ret = string.Empty;
            var l = new kqClassesSequenceVModel().List;
            var model = l.Where(p => p.CSName == t.CSName).FirstOrDefault();
            if (model != null)
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<kqClassesSequence>("班次已存在"), "") });
            }
            ret = "true";
            return ret;
        }

        protected override string OnEditCK(kqClassesSequence t)
        {
            var ret = string.Empty;
            var l = new kqClassesSequenceVModel().List;
            var model = l.Where(p => p.CSName == t.CSName && p.ID != t.ID).FirstOrDefault();
            if (model != null)
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<kqClassesSequence>("班次已存在"), "") });
            }
            ret = "true";
            return ret;
        }
        protected override string OnBatchDelCk(kqClassesSequence t)
        {
            var ret = string.Empty;

            var l = new kqClassesRelationVModel().List;
            var model=l.Where(p => p.CSId==t.ID).FirstOrDefault();
            if (model != null)
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<kqClassesSequence>("班次已被使用"), "") });
            }
            ret = "true";
            return ret;
        }
    }
}
