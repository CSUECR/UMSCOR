using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using MorSun.Model;
using System.Web.Mvc;
using MorSun.Bll;
using System.Web;
using System.Data.Objects.DataClasses;

namespace MorSun.Controllers.ViewModel
{
    public class RoleVModel : BaseVModel<aspnet_Roles>
    {
        public override IQueryable<aspnet_Roles> List
        {
            get
            {
                var roleList = All;
                //如果不是系统管理员，排除掉系统管理员的角色
                if (!MorSun.Common.Privelege.资源.操作范围.HP(MorSun.Common.Privelege.操作.系统管理员))
                {
                    var zy = Guid.Parse(MorSun.Common.Privelege.资源.操作范围);
                    var cz = Guid.Parse(MorSun.Common.Privelege.操作.系统管理员);
                    var xtgly = roleList.Where(r => r.wmfPrivilegeInRoles.Count(p => p.wmfPrivilege.ResourceId == zy && p.wmfPrivilege.OperationId == cz) > 0);
                    roleList = roleList.Except(xtgly);
                }
                return roleList.OrderBy(t => t.Sort);
            }
        }

        /// <summary>
        /// 被选中的编号
        /// </summary>
        //public virtual Guid? CheckedId { get; set; }

        public virtual Guid? RoleId { get; set; }
        /// <summary>
        /// 获取被选中的角色
        /// </summary>
        public virtual aspnet_Roles CheckedRole
        {
            get
            {
                var role = All.FirstOrDefault(r => r.RoleId == RoleId);
                return role ?? First;
            }
        }


        public virtual IQueryable<wmfPrivilege> Privileges
        {
            get
            {
                return new PrivilegeVModel().All;
            }
        }

        /// <summary>
        /// 被选中的权限编号
        /// </summary>

        public virtual Guid[] PrivId { get; set; }

        //Guid[] _PrivId;

        //public virtual Guid[] PrivId
        //{
        //    get
        //    {
        //        _PrivId = _PrivId.Load(() =>
        //        {
        //            var values = HttpContext.Current.Request.Params.GetValues("PrivId");
        //            return values == null ?
        //                new Guid[0] :
        //                values.Select(v => Guid.Parse(v)).ToArray();
        //        });
        //        return _PrivId;
        //    }
        //    set { _PrivId = value; }
        //}

        //BaseBll<wmfPrivilege> privBll;

        //public virtual BaseBll<wmfPrivilege> PrivBll
        //{
        //    get
        //    {
        //        privBll = privBll.Load();
        //        return privBll;
        //    }
        //    set { privBll = value; }
        //}

        ///// <summary>
        ///// 全部权限
        ///// </summary>
        //public virtual IQueryable<wmfPrivilege> Privs
        //{
        //    get
        //    {
        //        var l = PrivBll.All.Where(p => p.FlagTrashed==false);
        //        return from q in l orderby q.Sort ascending select q;
        //    }
        //}

        //BaseBll<wmfResource> resrcBll;
        ///// <summary>
        ///// 资源
        ///// </summary>
        //public virtual BaseBll<wmfResource> ResrcBll
        //{
        //    get
        //    {
        //        resrcBll = resrcBll.Load();
        //        return resrcBll;
        //    }
        //    set { resrcBll = value; }
        //}

        ///// <summary>
        ///// 资源
        ///// </summary>
        //public virtual IQueryable<wmfResource> Resrcs
        //{
        //    get
        //    {
        //        //资源信息
        //        var zyRef = Guid.Parse(MorSun.Common.类别.Reference.资源类别_资源);
        //        return ResrcBll.All.Where(t => t.RefId == zyRef).OrderBy(t => t.Sort);
        //    }
        //}

        //判断是否有子集选中
        //public bool GetChecked(IQueryable<wmfResource> wmfResourceList, EntityCollection<wmfPrivilegeInRole> wmfPrivilegeInRolesList, Guid resourceid)
        //{
        //    var ischildChecked = false;
        //    var parentList = wmfResourceList.Where(p => p.ParentId == resourceid);
        //    if (parentList != null && parentList.Count() > 0)
        //    {
        //        foreach (var resrc in parentList)
        //        {
        //            //权限
        //            var privList = resrc.wmfPrivileges.OrderBy(t => t.wmfOperation.Sort);
        //            foreach (var priv in privList)
        //            {
        //                var flag = wmfPrivilegeInRolesList.Any(pir => pir.PrivilegeId == priv.ID);
        //                if (flag)
        //                {
        //                    ischildChecked = true;
        //                }
        //            }
        //        }
        //    }
        //    return ischildChecked;
        //}
    }
}
