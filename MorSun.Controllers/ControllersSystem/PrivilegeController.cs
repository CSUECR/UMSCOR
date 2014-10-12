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
using MorSun.Common.Privelege;

namespace MorSun.Controllers.SystemController
{
    /// <summary>
    /// 资源
    /// </summary>
    [HandleError]
    [Authorize]
    public class PrivilegeController : BaseController<wmfPrivilege>
    {
        protected override string ResourceId
        {
            get { return 资源.权限; }
        }

        //编辑前验证
        protected override string OnEditCK(wmfPrivilege t)
        {
            var privilege = Bll.All.FirstOrDefault(r => r.ResourceId == t.ResourceId && r.OperationId == t.OperationId);
            if (privilege != null && privilege.ID != t.ID)
            {
                //资源和操作相同的权限已经存在！
                "PrivilegeCNName".AE("已存在相同类型权限", ModelState);                  
            }
            return "";
        }

        //创建前验证
        protected override string OnAddCK(wmfPrivilege t)
        {
            var privilege = Bll.All.FirstOrDefault(r => r.ResourceId == t.ResourceId && r.OperationId == t.OperationId);
            if (privilege != null)
            {
                //资源和操作相同的权限已经存在！
                "PrivilegeCNName".AE("已存在相同类型权限", ModelState);                  
            }

            return "";
        }

        
        //删除前验证
        protected override string OnDelCk(wmfPrivilege t)
        {
            var privilegeInRoleBll = new BaseBll<wmfPrivilegeInRole>();
            var privilegeInRole = privilegeInRoleBll.All.Where(r => r.PrivilegeId == t.ID).FirstOrDefault();
            var s = "";
            if (privilegeInRole != null)
            {
                //权限在角色中使用!
                "PrivilegeCNName".AE("权限在角色中使用", ModelState);
                s += "权限在角色中使用";
            }
            return s;
        }

       
    }
}
