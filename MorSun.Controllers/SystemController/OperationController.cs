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
using System.Collections;

namespace MorSun.Controllers.SystemController
{
    /// <summary>
    /// 操作
    /// </summary>
    public class OperationController : BaseController<wmfOperation>
    {
        protected override string ResourceId
        {
            get { return MorSun.Common.Privelege.资源.操作; }
        }

        private BaseBll<wmfOperation> _operationBll;

        public BaseBll<wmfOperation> OperationBll
        {
            get
            {
                _operationBll = _operationBll.Load();
                return _operationBll;
            }
            set { _operationBll = value; }
        }

        //编辑前验证
        protected override string OnEditCK(wmfOperation t)
        {
            var resource = OperationBll.All.FirstOrDefault(r => r.OperationCNName == t.OperationCNName);
            if (resource != null && resource.ID != t.ID)
            {
                //该操作名称已经存在，请重新输入！
                //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfOperation>("操作名称已经存在"), "") });
            }
            return "true";
        }

        //创建前验证
        protected override string OnPreCreateCK(wmfOperation t)
        {
            var resource = OperationBll.All.FirstOrDefault(r => r.OperationCNName == t.OperationCNName);
            if (resource != null)
            {
                //该资源名称已经存在，请重新输入！
                //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfOperation>("操作名称已经存在"), "") });
            }
            return "true";
        }
        

        //删除前验证
        protected override string OnDelCk(wmfOperation t)
        {
            //权限
            var privilegeBll = new BaseBll<wmfPrivilege>();
            #region 遇到操作不能删除的情况这边整
            var privilege = privilegeBll.All.Where(r => r.OperationId == t.ID).FirstOrDefault();
            if (privilege != null)
            {
                //操作在权限中使用!
                //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfOperation>("操作在权限中使用"), "") });
            }
            //删除后用这个查询看有没有删除干净，不然系统出错select * from wmfPrivilege where OperationId IS NULL
            #endregion
            return "true";
        }        

    }
}
