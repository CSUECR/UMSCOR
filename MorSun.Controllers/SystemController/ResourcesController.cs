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
    [HandleError]
    public class ResourcesController : BaseController<wmfResource>
    {
        protected override string ResourceId
        {
            get { return MorSun.Common.Privelege.资源.资源管理; }
        }
        private BaseBll<wmfResource> _resourceBll;

        public BaseBll<wmfResource> ResourceBll
        {
            get
            {
                _resourceBll = _resourceBll.Load();
                return _resourceBll;
            }
            set { _resourceBll = value; }
        }

        //删除前验证
        protected override string OnDelCk(wmfResource t)
        {
            var resource = ResourceBll.All.FirstOrDefault(r => r.ParentId == t.ID);
            var privilegeBll = new BaseBll<wmfPrivilege>();
            //var privilege = privilegeBll.All.Where(r => r.ResourcesId == t.ID).FirstOrDefault();

            if (resource != null)
            {
                //资源存在下级目录，请先删除下级目录!
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfResource>("资源存在下级目录"), "") });
            }

            #region 删除角色权限及权限和资源
            //13.12.17新增代码，删除权限时很麻烦，要先把各个角色里面的权限删除掉，再删除权限然后再删除资源。
            //如果不存在下级目录，先删除掉角色权限，再删除权限，然后继续。
            //取出该资源的所有权限ID
            var rids = t.ID;            
            var privileges = privilegeBll.All.Where(p => p.ResourcesId != null && rids == p.ResourcesId);
            var pid = privileges.Select(p => p.ID);
            //先删除角色权限表里面的权限
            var pirBll = new BaseBll<wmfPrivilegeInRole>();
            var pirs = pirBll.All.Where(p => pid.Contains(p.PrivilegeId));
            foreach (var pir in pirs)
            {
                pirBll.Delete(pir, false);
            }
            //再删除权限
            foreach (var p in privileges)
            {
                privilegeBll.Delete(p, false);
            }
            #endregion
            return "true";
        }



        /// <summary>
        /// 移动记录
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="pid">目标ID</param>
        /// <returns></returns>
        // [AcceptVerbs(HttpVerbs.Post)]
        public string TreeTableMove(string id, string pid)
        {
            var p1 = Guid.Parse(id.Replace("node-", ""));
            //父ID
            var p2 = Guid.Parse(pid.Replace("node-", ""));

            //不能将自己当做父节点
            if (p1 == p2)
            {
                //移动失败，资源A不能移动到资源A下！
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfResource>("自己的资源不能移到自己的资源目录下"), "") });
            }

            ///判断ID与父级ID相同
            if (SearchDep(p1, p2))
            {
                //上级资源不能往自己的下级资源移动！
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfResource>("上级资源不能移到下级资源目录"), "") });
            }
            else
            {
                var ss = ResourceBll.GetModel(p1);
                ss.ParentId = p2;
                ResourceBll.Update(ss);
                return "true";
            }
        }



        public bool SearchDep(Guid p1, Guid p2)
        {
            var dept = ResourceBll.All.FirstOrDefault(r => r.ID == p2);
            if (dept != null)
            {
                Guid parentId = dept.ParentId.ToAs<Guid>();
                if (parentId == p1)
                {
                    return true;
                }
                else
                {
                    return SearchDep(p1, parentId);
                }
            }
            else
            {
                return false;
            }
        }       


        //创建前验证
        protected override string OnPreCreateCK(wmfResource t)
        {
            var resource = ResourceBll.All.FirstOrDefault(r => r.ResourcesCNName == t.ResourcesCNName && r.RefId == t.RefId);
            if (resource != null)
            {
                //该资源名称已经存在，请重新输入！
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfResource>("资源名称已存在"), "") });
            }
            return "true";
        }

        //编辑前验证
        protected override string OnEditCK(wmfResource t)
        {
            string ret = "true";
            //大于2条说明已经存在
            var resource = ResourceBll.All.FirstOrDefault(r => r.ResourcesCNName == t.ResourcesCNName && r.RefId == t.RefId);
            if (resource != null && resource.ID != t.ID)
            {
                //该资源名称已经存在，请重新输入！
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfResource>("资源名称已存在"), "") });
            }
            var p1 = t.ID;
            //父ID
            var p2 = t.ParentId.ToAs<Guid>();

            //不能将自己当做父节点
            if (p1 == p2)
            {
                //移动失败，资源A不能移动到资源A下！
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfResource>("自己的资源不能移到自己的资源目录下"), "") });
            }

            ///判断ID与父级ID相同
            if (SearchDep(p1, p2))
            {
                //上级资源不能往自己的下级资源移动！
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfResource>("上级资源不能移到下级资源目录"), "") });
            }

            return ret;
        }

        
            



    }
}

