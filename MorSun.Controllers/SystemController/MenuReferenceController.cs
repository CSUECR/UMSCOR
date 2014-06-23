using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using HOHO18.Common;
using MorSun.Bll;
using System.Collections;
using System.Web.Mvc;
using MorSun.Controllers.ViewModel;

namespace MorSun.Controllers.CommonController
{
    public class MenuReferenceController : BaseController<wmfReference>
    {
        protected override string ResourceId
        {
            get { return MorSun.Common.Privelege.资源.头部菜单; }
        }


        public override ActionResult Index()
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.查看))
            {
                ViewBag.CanDoSth = CanDoSth;
                var vModel = new ReferenceVModel();
                return View(vModel);
            }
            else
            {
                return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
            }
        }

        

        protected override string OnPreCreateCK(wmfReference t)
        {
            //显示名称
            if (string.IsNullOrEmpty(t.ItemValue))
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("显示名称不能为空"), "ItemValue") });
            }
            //图标
            if (string.IsNullOrEmpty(t.Icon))
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("图标不能为空"), "Icon") });
            }

            string ret = "true";
            var Refer = Bll.All.FirstOrDefault(r => r.ItemInfo == t.ItemInfo && r.RefGroupId == t.RefGroupId);
            if (Refer != null)
            {
                //该类别已经存在，请重新输入！
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("Name已存在"), "ItemInfo") });
            }

            return ret;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override string Edit(wmfReference t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.修改))
            {
                //显示名称
                if (string.IsNullOrEmpty(t.ItemValue))
                {
                    return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("显示名称不能为空"), "ItemValue") });
                }
                //图标
                if (string.IsNullOrEmpty(t.Icon))
                {
                    return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("图标不能为空"), "Icon") });
                }

                string ret = "true";
                var Refer = Bll.All.FirstOrDefault(r => r.ItemInfo == t.ItemInfo && r.RefGroupId == t.RefGroupId);
                if (Refer != null && t.ID != Refer.ID)
                {
                    //该类别已经存在，请重新输入！
                    return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("Name已存在"), "ItemInfo") });
                }
                return base.Edit(t);
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
            }
        }

        public override ActionResult Recycle()
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.回收站))
            {
                var vModel = new ReferenceVModel();
                return View(vModel);
            }
            else
            {
                return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
            }
        }
    }
}
