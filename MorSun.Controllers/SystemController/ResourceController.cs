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
    public class ResourceController : BaseController<wmfResource>
    {
        protected override string ResourceId
        {
            get { return 资源.资源管理; }
        }

        /// <summary>
        /// 可批量添加类别组
        /// </summary>
        /// <param name="t"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult Create(wmfResource t, string returnUrl)
        {
            if (ResourceId.HP(操作.添加))
            {
                var oper = new OperationResult(OperationResultType.Error, "添加失败");
                string[] Names = ((t.ResourceCNName == null) ? (t.ResourceCNName = " ").Split(',') : t.ResourceCNName.Split(','));
                for (int i = 0; i < Names.Length; i++)
                {
                    if (Names.Length == 1)
                    {
                        t.ResourceCNName = Names[0];
                        OnAddCK(t);
                        if (ModelState.IsValid)
                        {
                            //添加初始化字段
                            CreateInitObject(t);
                            var result = Bll.Insert(t, false);
                            if (result == null)
                            {
                                "ResourceCNName".AE(Names[0] + "添加失败", ModelState);
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(Names[i]))
                        {
                            var model = new wmfResource();
                            model.ResourceCNName = Names[i];
                            model.ParentId = t.ParentId;
                            //资源数据
                            model.Icon = t.Icon;
                            model.URL = t.URL;
                            OnAddCK(t);
                            if (ModelState.IsValid)
                            {
                                CreateInitObject(model);
                                var result = Bll.Insert(model, false);
                                if (result == null)
                                {
                                    "ResourceCNName".AE(Names[i] + "添加失败", ModelState);
                                }
                            }
                        }
                    }
                }
                if (ModelState.IsValid)
                {
                    fillOperationResult(returnUrl, oper, "添加成功");
                    Bll.UpdateChanges();
                    return Json(oper, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    oper.AppendData = ModelState.GE();
                    return Json(oper, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                "ResourceCNName".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper, JsonRequestBehavior.AllowGet);
            }
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
                var p1 = Guid.Parse(id);
                //父ID
                var p2 = Guid.Parse(pid);
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
                var model = Bll.GetModel(p1);
                var pmodel = Bll.GetModel(p2);
                if (model == null || pmodel == null)
                {
                    errms = "数据提交错误";
                    "ResourcesCNName".AE("数据提交错误", ModelState);
                }
                var oper = new OperationResult(OperationResultType.Error, "移动失败 " + errms);
                if (ModelState.IsValid)
                {                    
                    model.ParentId = p2;
                    Bll.Update(model);
                    fillOperationResult(returnUrl, oper, "移动成功");
                    return Json(oper, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    oper.AppendData = ModelState.GE();
                    return Json(oper, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                "ResourcesCNName".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper, JsonRequestBehavior.AllowGet);
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
            var resource = Bll.All.FirstOrDefault(r => r.ResourceCNName == t.ResourceCNName && r.RefId == t.RefId);
            if (resource != null)
            {
                //该资源名称已经存在，请重新输入！
                "ResourceCNName".AE("资源名称已存在",ModelState);
            }
            if (t.ParentId != null)
            {
                var pReferGrop = Bll.All.FirstOrDefault(r => r.ID == t.ParentId);
                if (pReferGrop == null)
                    "ParentId".AE("请正确选择父级资源", ModelState);
            }
            if(t.ResourceCNName.Length > 50)
            {
                "ResourceCNName".AE("资源名长度不可超过50", ModelState);
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
            var privileges = privilegeBll.All.Where(p => p.ResourceId != null && rids == p.ResourceId);
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
            var resource = Bll.All.FirstOrDefault(r => r.ResourceCNName == t.ResourceCNName && r.RefId == t.RefId);
            if (resource != null && resource.ID != t.ID)
            {
                //该资源名称已经存在，请重新输入！
                "ResourceCNName".AE("资源名称已存在",ModelState);
            }
            var p1 = t.ID;
            //父ID
            var p2 = t.ParentId.ToAs<Guid>();

            //不能将自己当做父节点
            if (p1 == p2)
            {
                //移动失败，资源A不能移动到资源A下！
                "ResourceCNName".AE("移动位置错误", ModelState);                
            }

            ///判断ID与父级ID相同
            if (SearchDep(p1, p2))
            {
                //上级资源不能往自己的下级资源移动！
                "ResourceCNName".AE("上级资源不能移到下级资源目录", ModelState);                  
            }

            if (t.ResourceCNName.Length > 50)
            {
                "ResourceCNName".AE("资源名长度不可超过50", ModelState);
            }
            return "";
        }

        public ActionResult GetP()
        {
            return View();
        }
    }
}

