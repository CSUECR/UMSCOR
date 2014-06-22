using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using System.Web.Mvc;
using MorSun.Controllers.ViewModel;
using System.Collections;
using MorSun.Bll;
using HOHO18.Common.Model;

namespace MorSun.Controllers
{
    public class UserScheduleSetController : BaseController<kqCPTC>
    {
        protected override string ResourceId
        {
            get { return MorSun.Common.Privelege.资源.设置排班; }
        }

        public override ActionResult Add(kqCPTC t)
        {
            var pindex = t.PIndex;
            var preUrl = t.PreUrl;
            t.PreUrl = "/UserScheduleSet/" + t.PreUrl;
            if (t.PreUrl != null && t.PreUrl.IndexOf("PreUrl") != -1)
            {
                var i = t.PreUrl.IndexOf("PreUrl");
                t.PreUrl = t.PreUrl.Substring(i + 7);
            }
            return View(t);
        }

        public ActionResult ScheduleList(kqCPTCSVModel t)
        {
            if (!string.IsNullOrEmpty(t.CPTId))
            {
                var ID = Guid.Parse(t.CPTId);
                var l = new kqCalssPlanTemplateVModel().All;
                var model = l.Where(p => p.ID == ID).FirstOrDefault();
                if (model != null)
                {
                    t.Period = model.Period.Value.ToString("F0");
                    t.IsWeekPeriod = model.IsWeekPeriod;
                }
            }
            return View(t);
        }

        public ActionResult IndexList(kqCPTCSVModel t)
        {
            return View(t);
        }


        /// <summary>
        /// 对数据进行重新排列
        /// </summary>
        /// <param name="CheckedId"></param>
        /// <returns></returns>
        public override string GetSortableList(kqCPTC t)
        {
            string msg = "排序失败";
            string[] ids = GetCheckId(t).Split(',');
            var referList = new List<kqCPTC>();
            var baseRef = new BaseBll<kqCPTC>();
            var refergroupids = new ArrayList();
            for (int i = 0; i < ids.Length - 1; i++)
            {
                if (!string.IsNullOrEmpty(ids[i]))
                {
                    var refer = new kqCPTC();
                    refer = baseRef.GetModel(Guid.Parse(ids[i]));
                    referList.Add(refer);
                }
            }
            int k = 0;
            for (int m = 0; m < referList.Count; m++)
            {
                referList[m].Sort = k + 1;
                k++;
            }
            Bll.UpdateChanges();
            msg = "true";
            return msg;
        }

        public static string GetRefer()
        {
            StringBuilder str = new StringBuilder();
            var deptList = new kqCalssPlanTemplateVModel().All;
            str.Append("{id: '1', pId: 0, name: '" + XmlHelper.GetPagesString<kqCalssPlanTemplate>("排班模板树结点Title") + "',open:true },");
            foreach (var item in deptList)
            {
                str.Append("{");
                str.AppendFormat("id:\"{0}\",pId:'1',name:\"{1}\",isDept:true", item.ID, item.TempName);
                str.Append("}");
                str.Append(",");
            }
            return "var zNodes =[" + str.ToString().TrimEnd(',') + "]";
        }

        protected override string OnPreCreateCK(kqCPTC t)
        {
            if (t.Period <= t.SumNum)
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<kqCPTC>("班次天数不能超过周期天数"), "") });
            }
            return "true";
        }
    }
}
