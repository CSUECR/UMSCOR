using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using MorSun.Controllers.ViewModel;
using HOHO18.Common.Model;

namespace MorSun.Controllers
{
    public class UserHolidayController : BaseController<kqHoliday>
    {
        protected override string ResourceId
        {
            get { return MorSun.Common.Privelege.资源.节假日管理; }
        }

        protected override string OnPreCreateCK(kqHoliday t)
        {
            var ret = string.Empty;
            var l = new kqHolidayVModel().All;
            var model = l.Where(p=>p.StartTime==t.StartTime&&p.EndTime==t.EndTime).FirstOrDefault();
            if (model != null)
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<kqHoliday>("节假日已存在"), "") });
            }
            ret = "true";
            return ret;
        }

        protected override string OnEditCK(kqHoliday t)
        {
            var ret = string.Empty;
            var l = new kqHolidayVModel().All;
            var model = l.Where(p => p.StartTime == t.StartTime && p.EndTime == t.EndTime&&p.ID!=t.ID).FirstOrDefault();
            if (model != null)
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<kqHoliday>("节假日已存在"), "") });
            }
            ret = "true";
            return ret;
        }
    }
}
