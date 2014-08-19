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
using MorSun.Common.Privelege;

namespace MorSun.Controllers.SystemController
{
    public class MenuReferenceController : BaseController<wmfReference>
    {
        protected override string ResourceId
        {
            get { return 资源.头部菜单; }
        }
        

        protected override string OnAddCK(wmfReference t)
        {
            //显示名称
            if (string.IsNullOrEmpty(t.ItemValue))
            {
                //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("显示名称不能为空"), "ItemValue") });
            }
            //图标
            if (string.IsNullOrEmpty(t.Icon))
            {
                //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("图标不能为空"), "Icon") });
            }

            string ret = "true";
            var Refer = Bll.All.FirstOrDefault(r => r.ItemInfo == t.ItemInfo && r.RefGroupId == t.RefGroupId);
            if (Refer != null)
            {
                //该类别已经存在，请重新输入！
                //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("Name已存在"), "ItemInfo") });
            }

            return ret;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        //public override string Update(wmfReference t)
        //{
        //    if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.修改))
        //    {
        //        //显示名称
        //        if (string.IsNullOrEmpty(t.ItemValue))
        //        {
        //            //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("显示名称不能为空"), "ItemValue") });
        //        }
        //        //图标
        //        if (string.IsNullOrEmpty(t.Icon))
        //        {
        //            //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("图标不能为空"), "Icon") });
        //        }

        //        string ret = "true";
        //        var Refer = Bll.All.FirstOrDefault(r => r.ItemInfo == t.ItemInfo && r.RefGroupId == t.RefGroupId);
        //        if (Refer != null && t.ID != Refer.ID)
        //        {
        //            //该类别已经存在，请重新输入！
        //            //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("Name已存在"), "ItemInfo") });
        //        }
        //        return base.Update(t);
        //    }
        //    else
        //    {
        //        //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
        //    }
        //}        
    }
}
