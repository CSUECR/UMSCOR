using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using System.Web.Mvc;
using HOHO18.Common.Model;
using MorSun.Controllers.ViewModel;

namespace MorSun.Controllers
{
    public class UserClassesRefController : BaseController<kqClassesRef>
    {
       protected override string ResourceId
       {
           get { return MorSun.Common.Privelege.资源.班次类型管理; }
       }

       public override ActionResult Add(kqClassesRef t)
       {
           var pindex = t.PIndex;
           var preUrl = t.PreUrl;
           t.PreUrl = "/UserClassesRef/" + t.PreUrl;
           if (t.PreUrl != null && t.PreUrl.IndexOf("PreUrl") != -1)
           {
               var i = t.PreUrl.IndexOf("PreUrl");
               t.PreUrl = t.PreUrl.Substring(i + 7);
           }
           return View(t);
       }

       protected override string OnPreCreateCK(kqClassesRef t)
       {
           var ret = string.Empty;
           var l = new kqClassesRefVModel().List;
           var model = l.Where(p => p.ClassesName==t.ClassesName).FirstOrDefault();
           if (model != null)
           {
               return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<kqClassesRef>("班次类型已存在"), "") });
           }
           ret = "true";
           return ret;
       }

       protected override string OnEditCK(kqClassesRef t)
       {
           var ret = string.Empty;
           var l = new kqClassesRefVModel().List;
           var model = l.Where(p => p.ClassesName==t.ClassesName&& p.ID != t.ID).FirstOrDefault();
           if (model != null)
           {
               return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<kqClassesRef>("班次类型已存在"), "") });
           }
           ret = "true";
           return ret;
       }
       protected override string OnBatchDelCk(kqClassesRef t)
       {
           var ret = string.Empty;

           var l = new kqClassesRelationVModel().List;
           var model = l.Where(p => p.CRId == t.ID).FirstOrDefault();
           if (model != null)
           {
               return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<kqClassesRef>("班次类型已被使用"), "") });
           }
           ret = "true";
           return ret;
       }
    }
}
