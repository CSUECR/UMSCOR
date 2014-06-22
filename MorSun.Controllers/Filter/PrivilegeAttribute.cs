using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using MorSun.Model;
using MorSun.Bll;
using System.Web.Security;
using System.Collections;
using HOHO18.Common;

namespace MorSun.Controllers.Filter
{
    public class PrivilegeAttribute : FilterAttribute, IAuthorizationFilter
    {

        #region IAuthorizationFilter 成员

        public string resourceId { get; set; }
        public string operationId { get; set; }
        public string privilegeValue { get; set; }

        void IAuthorizationFilter.OnAuthorization(AuthorizationContext filterContext)
        {
            MembershipUser u = Membership.GetUser();
            if (u != null)
            {

                //throw new Exception("这个方法要改！");
                if (System.Web.HttpContext.Current.Session["HaveSessionPrivilege"] == null)
                {
                    MorSun.Controllers.BasisController.setSessionPrivileges();
                }
                if (String.Compare("无权限", System.Web.HttpContext.Current.Session["HaveSessionPrivilege"] as string) == 0)
                    throw new Exception("您没有权限支持当前操作，如有需要，请联系管理员！");
                else
                {
                    if (System.Web.HttpContext.Current.Session["SessionPrivilege"] == null)
                        throw new Exception("您没有权限支持当前操作，如有需要，请联系管理员！");
                    else
                    {
                        List<wmfSessionPrivilege> sessionPrivilegeList = System.Web.HttpContext.Current.Session["SessionPrivilege"] as List<wmfSessionPrivilege>;
                        wmfSessionPrivilege sp = null;
                        if (String.IsNullOrEmpty(privilegeValue))
                            sp = sessionPrivilegeList.Where(p => p.operationId == operationId.ToString() && p.resourceId == resourceId.ToString()).FirstOrDefault();
                        else
                            sp = sessionPrivilegeList.Where(p => p.operationId == operationId.ToString() && p.resourceId == resourceId.ToString() && (!String.IsNullOrEmpty(privilegeValue) && p.privilegeValuesArray.Contains(privilegeValue))).FirstOrDefault();
                        if (sp == null)
                        {
                            throw new Exception("您没有权限支持当前操作，如有需要，请联系管理员！");
                        }
                    }
                }
            }
            else
            {
                throw new Exception("请先登录！");
            }
        }

        #endregion
    }
}
