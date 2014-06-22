using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MorSun.Controllers.ViewModel;
using MorSun.Model;
using HOHO18.Common.Model;
using MorSun.Bll;

namespace MorSun.Controllers
{
    public class UserVisaController : BaseController<aspnet_Users>
    {
        protected override string ResourceId
        {
            get { return MorSun.Common.Privelege.资源.免签人员管理; }
        }

        public override string Create(aspnet_Users t)
        {
            var ret = string.Empty;
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.添加))
            {
                string[] ids = ((t.CheckedId == null) ? (t.CheckedId = "").Split(',') : t.CheckedId.Split(','));
                if (ids[0] == "")
                {
                    return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Users>("免签人员不能为空"), "selectfriend") });
                }
                var newBll = new BaseBll<wmfUserInfo>();
                 var userList = newBll.All;
                for (int i = 0; i < ids.Length; i++)
                {

                    if (!string.IsNullOrEmpty(ids[i]))
                    {
                        var ID = Guid.Parse(ids[i]);
                        var model = userList.Where(p=>p.ID==ID).FirstOrDefault();
                        if (model != null)
                        {
                            model.IsNoCheck = true;
                        }
                    }
                }
                newBll.UpdateChanges();
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
            }
            ret = "true";
            return ret;
        }

        public override string BatchDelete(aspnet_Users t)
        {
            var ret = string.Empty;
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.彻底删除))
            {
                string[] ids = ((t.CheckedId == null) ? (t.CheckedId = "").Split(',') : t.CheckedId.Split(','));
                if (ids[0] == "")
                {
                    return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "请选择要设置的项"), "") });
                }
                var newBll = new BaseBll<wmfUserInfo>();
                var userList = newBll.All;
                for (int i = 0; i < ids.Length; i++)
                {

                    if (!string.IsNullOrEmpty(ids[i]))
                    {
                        var ID = Guid.Parse(ids[i]);
                        var model = userList.Where(p => p.ID == ID).FirstOrDefault();
                        if (model != null)
                        {
                            model.IsNoCheck = false;
                        }
                    }
                }
                newBll.UpdateChanges();                
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
            }
            ret = "true";
            return ret;
        }
    }
}
