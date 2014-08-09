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
    [HandleError]
    [Authorize]
    public class ResourcesController : BaseController<wmfResource>
    {
        protected override string ResourceId
        {
            get { return 资源.资源管理; }
        }  

        /// <summary>
        /// 移动记录
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="pid">目标ID</param>
        /// <returns></returns>
        // [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TreeTableMove(string id, string pid, string returnUrl)
        {
            if (ResourceId.HP(操作.修改))
            {
                var p1 = Guid.Parse(id.Replace("node-", ""));
                //父ID
                var p2 = Guid.Parse(pid.Replace("node-", ""));
                var errms = "";
                //不能将自己当做父节点
                if (p1 == p2)
                {
                    //移动失败，资源A不能移动到资源A下！
                    errms = "移动位置错误";
                    "ResourcesCNName".AE("移动位置错误", ModelState);
                }

                ///判断ID与父级ID相同
                if (SearchDep(p1, p2))
                {
                    //上级资源不能往自己的下级资源移动！
                    errms = "上级资源不能移到下级资源目录";
                    "ResourcesCNName".AE("上级资源不能移到下级资源目录", ModelState);
                }
                var oper = new OperationResult(OperationResultType.Error, "移动失败" + errms);
                if (ModelState.IsValid)
                {
                    var ss = Bll.GetModel(p1);
                    ss.ParentId = p2;
                    Bll.Update(ss);
                    fillOperationResult(returnUrl, oper, "移动成功");
                    return Json(oper);
                }
                else
                {
                    oper.AppendData = ModelState.GE();
                    return Json(oper);
                }
            }
            else
            {
                "ResourcesCNName".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper);
            }
        }

        public bool SearchDep(Guid p1, Guid p2)
        {
            var dept = Bll.All.FirstOrDefault(r => r.ID == p2);
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
        protected override string OnAddCK(wmfResource t)
        {
            var resource = Bll.All.FirstOrDefault(r => r.ResourcesCNName == t.ResourcesCNName && r.RefId == t.RefId);
            if (resource != null)
            {
                //该资源名称已经存在，请重新输入！
                "ResourcesCNName".AE("资源名称已存在",ModelState);
            }
            return "";
        }

        //删除前验证
        protected override string OnDelCk(wmfResource t)
        {
            var resource = Bll.All.FirstOrDefault(r => r.ParentId == t.ID);
            var privilegeBll = new BaseBll<wmfPrivilege>();
            //var privilege = privilegeBll.All.Where(r => r.ResourcesId == t.ID).FirstOrDefault();
            //全部直接删除，否则删除权限太麻烦
            //if (resource != null)
            //{
            //    //资源存在下级目录，请先删除下级目录!
            //    "ResourcesCNName".AE("资源存在下级目录", ModelState);                
            //}

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
            return "";
        }

        //编辑前验证
        protected override string OnEditCK(wmfResource t)
        {            
            //大于2条说明已经存在
            var resource = Bll.All.FirstOrDefault(r => r.ResourcesCNName == t.ResourcesCNName && r.RefId == t.RefId);
            if (resource != null && resource.ID != t.ID)
            {
                //该资源名称已经存在，请重新输入！
                "ResourcesCNName".AE("资源名称已存在",ModelState);
            }
            var p1 = t.ID;
            //父ID
            var p2 = t.ParentId.ToAs<Guid>();

            //不能将自己当做父节点
            if (p1 == p2)
            {
                //移动失败，资源A不能移动到资源A下！
                "ResourcesCNName".AE("移动位置错误", ModelState);                
            }

            ///判断ID与父级ID相同
            if (SearchDep(p1, p2))
            {
                //上级资源不能往自己的下级资源移动！
                "ResourcesCNName".AE("上级资源不能移到下级资源目录", ModelState);                  
            }
            return "";
        }
    }
}

