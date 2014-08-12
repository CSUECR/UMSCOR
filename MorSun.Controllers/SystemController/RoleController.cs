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
using System.Web.Security;
using HOHO18.Common.ExHelp;
using MorSun.Common.Privelege;

namespace MorSun.Controllers.SystemController
{    
    [HandleError]
    [Authorize]
    public class RoleController : BaseController<aspnet_Roles>
    {
        protected override string ResourceId
        {
            get { return 资源.角色配置; }
        }


        public ActionResult RoleManage(RoleVModel vModel)
        {      
            if (ResourceId.HP(操作.查看))
            {
                return View(vModel);
            }
            else
            {
                "".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper);
            }
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="t"></param>
        /// <param name="returnUrl"></param>
        /// <param name="ck"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult Add(aspnet_Roles t, string returnUrl)
        {
            if (ResourceId.HP(操作.添加))
            {
                var oper = new OperationResult(OperationResultType.Error, "添加失败");
                OnAddCK(t);
                if (ModelState.IsValid)
                {
                    Roles.CreateRole(t.RoleName);
                    //加了排序增加的方法,不然添加角色时，排序添加不进去。
                    var model = Bll.All.Where(m => m.LoweredRoleName == t.RoleName).FirstOrDefault();
                    model.Sort = t.Sort;
                    Bll.Update(model);
                    fillOperationResult(returnUrl, oper, "添加成功");
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
                "".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper);
            }
            
        }

        private BaseBll<wmfPrivilegeInRole> pirBll;

        public virtual BaseBll<wmfPrivilegeInRole> PirBll
        {
            get
            {
                pirBll = pirBll.Load();
                return pirBll;
            }
            set { pirBll = value; }
        }

        private BaseBll<wmfPrivilege> privBll;

        public virtual BaseBll<wmfPrivilege> PrivBll
        {
            get
            {
                privBll = privBll.Load();
                return privBll;
            }
            set { privBll = value; }
        }

        /// <summary>
        /// 判定数据库中是否存在该角色
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected override string OnAddCK(aspnet_Roles t)
        {
            //重复
            if (Bll.All.Count(role => role.RoleName == t.RoleName) > 0)
            {
                "RoleName".AE("角色已存在", ModelState);                
            }

            return "";
        }

        //编辑角色
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult Update(aspnet_Roles t, string returnUrl)
        {
            if (ResourceId.HP(操作.修改))
            {
                var oper = new OperationResult(OperationResultType.Error, "修改失败");
                var model = Bll.GetModel(t);
                if (model == null)
                {
                    "".AE("修改失败", ModelState);
                }
                if (ModelState.IsValid)
                {
                    TryUpdateModel<aspnet_Roles>(model);
                    model.LoweredRoleName = t.RoleName.ToLower();
                    Bll.Update(model);
                    fillOperationResult(returnUrl, oper, "修改成功");
                }
                else
                {
                    "".AE("修改失败", ModelState);
                    oper.AppendData = ModelState.GE();
                }
                return Json(oper);
            }
            else
            {
                "".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper);
            }            
        }

        //删除前验证
        protected override string OnDelCk(aspnet_Roles t)
        {
            var roles = dotNetRoles.GetUsersInRole(t.RoleName);
            if (roles.Count() != 0)
            {
                //有管理员使用该角色!
                "RoleName".AE("该角色还有用户使用", ModelState);                
            }
            //删除该角色的所有权限
            var privRoleBll = new BaseBll<wmfPrivilegeInRole>();
            var privRole = privRoleBll.All.Where(r => r.RoleId == t.RoleId);
            PirBll.Delete(privRole);
            return "";
        }

        /// <summary>
        /// 保存权限
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult SavePriv(RoleVModel vmodel, string returnUrl)
        {
            if (ResourceId.HP(操作.修改))
            {
                var oper = new OperationResult(OperationResultType.Error, "修改失败");
                //操作的角色
                var role = vmodel.CheckedRole;
                if(role != null)
                { 
                    //删除该角色的所有操作
                    var delPirs = role.wmfPrivilegeInRoles.Where(pir => pir.RoleId == vmodel.CheckedId);
                    PirBll.Delete(delPirs);

                    if (vmodel.PrivId != null && vmodel.PrivId.Count() != 0)
                    {
                        var privStr = vmodel.PrivId.Split(',');
                        if (privStr != null && privStr.Length != 0)
                        {
                            Guid[] privArrays = new Guid[privStr.Length - 1];
                            for (int i = 0; i < privStr.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(privStr[i]))
                                {
                                    privArrays[i] = Guid.Parse(privStr[i]);
                                }
                            }

                            //被选中的操作集合
                            var privs = vmodel.Privs.Where(p => privArrays.Contains(p.ID));
                            if (privs.Count() != 0)
                            {
                                //添加新的数据
                                foreach (var priv in privs)
                                {
                                    var newPir = new wmfPrivilegeInRole
                                    {
                                        wmfPrivilege = priv,
                                        aspnet_Roles = role,
                                    };
                                    PirBll.Insert(newPir, false);
                                }
                            }
                        }
                    }
                    //执行更新
                    PirBll.UpdateChanges();
                    fillOperationResult(returnUrl, oper, "修改成功");                    
                }
                else
                {
                    "".AE("修改失败", ModelState);
                    oper.AppendData = ModelState.GE();
                }
                return Json(oper);
            }
            else
            {
                "".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper);
            }
        }

        public ActionResult RoleToPrivilege(string ParentResourceID = "9dd4aec9-fa03-46ec-8be6-6d157df3e221")
        {
            if (ResourceId.HP(操作.修改))
            {
                var roleBll = new BaseBll<aspnet_Roles>();
                var resourceBll = new BaseBll<wmfResource>();

                var ParentResourceGuid = Guid.Parse(ParentResourceID);
                ViewBag.ParentResourceID = ParentResourceGuid;
                ViewBag.ParentResoures = resourceBll.All.Where(u => u.wmfResource1.Any() && u.wmfResource1.FirstOrDefault().wmfPrivileges.Any()).OrderByDescending(u => u.RegTime);
                ViewBag.Roles = roleBll.All.OrderBy(u => u.RoleName);
                ViewBag.Resources = resourceBll.All.First(u => u.ID == ParentResourceGuid).wmfResource1.OrderByDescending(u => u.RegTime);
                return View();
            }
            else
            {
                "".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleToPrivilege(string[] Roles, string[] Privileges, string returnUrl, bool deletePrivButNotAdd = false)
        {
            if (ResourceId.HP(操作.修改))
            {
                var oper = new OperationResult(OperationResultType.Error, "修改失败");
                if (Roles != null && Roles.Length > 0 && Privileges != null && Privileges.Length > 0)
                {
                    var officeId = Guid.Parse("9dd4aec9-fa03-46ec-8be6-6d157df3e221");                
                    var privilegeBll = new BaseBll<wmfPrivilege>();
                    foreach (var role in Roles)
                    {
                        var roleGuid = Guid.Empty;
                        if (Guid.TryParse(role, out roleGuid))
                        {
                            var roleModel = Bll.All.FirstOrDefault(u => u.RoleId == roleGuid);
                            if (Privileges.Count() > 0)
                            {
                                //先删除
                                foreach (var privilege in Privileges)
                                {
                                    var privilegeGuid = Guid.Empty;
                                    if (Guid.TryParse(privilege, out privilegeGuid))
                                    {
                                        var privilegeModel = privilegeBll.GetModel(privilegeGuid);
                                        var officePrivs = roleModel.wmfPrivilegeInRoles.Where(u => u.wmfPrivilege.wmfResource.ID == privilegeModel.ResourcesId);
                                        PirBll.Delete(officePrivs);
                                    }
                                }
                                //只删除权限不添加权限。
                                if (!deletePrivButNotAdd)
                                {
                                    foreach (var privilege in Privileges)
                                    {
                                        var privilegeGuid = Guid.Empty;
                                        if (Guid.TryParse(privilege, out privilegeGuid))
                                        {
                                            var newPri = new wmfPrivilegeInRole
                                            {
                                                PrivilegeId = privilegeGuid,
                                                RoleId = roleGuid,
                                                ID = Guid.NewGuid()
                                            };
                                            PirBll.Insert(newPri, false);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    pirBll.UpdateChanges();
                    fillOperationResult(returnUrl, oper, "修改成功");
                }                
                else
                {
                    "".AE("修改失败", ModelState);
                    oper.AppendData = ModelState.GE();
                }
                return Json(oper);
            }
            else
            {
                "".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper);
            }
        }

    }
}
