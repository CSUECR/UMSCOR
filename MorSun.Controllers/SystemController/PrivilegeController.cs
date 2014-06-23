using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using MorSun.Bll;
using MorSun.Model;
using MorSun.Controllers.Filter;
using System.Data.Objects;
using MorSun.Common;
using System.Text;
using dotNetRoles = System.Web.Security.Roles;
using HOHO18.Common;
using MorSun.Controllers.ViewModel;

namespace MorSun.Controllers.CommonController
{
    /// <summary>
    /// 资源
    /// </summary>
    public class PrivilegeController : BaseController<wmfPrivilege>
    {
        protected override string ResourceId
        {
            get { return MorSun.Common.Privelege.资源.权限; }
        }

        private BaseBll<wmfPrivilege> _privilegeBll;

        public BaseBll<wmfPrivilege> PrivilegeBll
        {
            get
            {
                _privilegeBll = _privilegeBll.Load();
                return _privilegeBll;
            }
            set { _privilegeBll = value; }
        }

        //编辑前验证
        protected override string OnEditCK(wmfPrivilege t)
        {
            var privilege = PrivilegeBll.All.FirstOrDefault(r => r.PrivilegeCNName == t.PrivilegeCNName && r.ResourcesId==t.ResourcesId);
            if (privilege != null && privilege.ID != t.ID)
            {
                //该权限名称已经存在，请重新输入！
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfPrivilege>("权限名称已经存在"), "") });
            }

            privilege = PrivilegeBll.All.FirstOrDefault(r => r.ResourcesId == t.ResourcesId && r.OperationId == t.OperationId);
            if (privilege != null && privilege.ID != t.ID)
            {
                //资源和操作相同的权限已经存在！
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfPrivilege>("资源和操作相同的权限已经存在"), "") });
            }
            return "true";
        }

        //创建前验证
        protected override string OnPreCreateCK(wmfPrivilege t)
        {
            var privilege = PrivilegeBll.All.FirstOrDefault(r => r.PrivilegeCNName == t.PrivilegeCNName && r.ResourcesId == t.ResourcesId);
            if (privilege != null)
            {
                //该权限名称已经存在，请重新输入！
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfPrivilege>("权限名称已经存在"), "") });
            }

            privilege = PrivilegeBll.All.FirstOrDefault(r => r.ResourcesId == t.ResourcesId && r.OperationId == t.OperationId);
            if (privilege != null)
            {
                //资源和操作相同的权限已经存在！
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfPrivilege>("资源和操作相同的权限已经存在"), "") });
            }

            return "true";
        }

        
        //删除前验证
        protected override string OnDelCk(wmfPrivilege t)
        {
            var privilegeInRoleBll = new BaseBll<wmfPrivilegeInRole>();
            var privilegeInRole = privilegeInRoleBll.All.Where(r => r.PrivilegeId == t.ID).FirstOrDefault();
            if (privilegeInRole != null)
            {
                //权限在角色中使用!
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfPrivilege>("权限在角色中使用"), "") });
            }
            return "true";
        }

       
    }
}
