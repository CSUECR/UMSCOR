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
using System.Web.Configuration;
using System.Configuration;
namespace MorSun.Controllers.SystemController
{    
    [HandleError]
    [Authorize]
    public class RoleController : BaseController<aspnet_Roles>
    {
        protected override string ResourceId
        {
            get { return ��Դ.��ɫ����; }
        }                

        /// <summary>
        /// ��ɫ����
        /// </summary>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public ActionResult Manage(RoleVModel vModel)
        {      
            if (ResourceId.HP(����.�鿴))
            {
                return View(vModel);
            }
            else
            {
                "".AE("��Ȩ��", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "��Ȩ��");
                oper.AppendData = ModelState.GE();
                return Json(oper, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// ���ӽ�ɫ
        /// </summary>
        /// <param name="t"></param>
        /// <param name="returnUrl"></param>
        /// <param name="ck"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult Create(aspnet_Roles t, string returnUrl)
        {
            if (ResourceId.HP(����.����))
            {
                var oper = new OperationResult(OperationResultType.Error, "����ʧ��");
                OnAddCK(t);
                if (ModelState.IsValid)
                {
                    Roles.CreateRole(t.RoleName);
                    //�����������ӵķ���,��Ȼ���ӽ�ɫʱ���������Ӳ���ȥ��
                    //var model = Bll.All.Where(m => m.LoweredRoleName == t.RoleName).FirstOrDefault();
                    //model.Sort = t.Sort;
                    //Bll.Update(model);
                    fillOperationResult(returnUrl, oper, "���ӳɹ�");
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
                "".AE("��Ȩ��", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "��Ȩ��");
                oper.AppendData = ModelState.GE();
                return Json(oper, JsonRequestBehavior.AllowGet);
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
        /// �ж����ݿ����Ƿ���ڸý�ɫ
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected override string OnAddCK(aspnet_Roles t)
        {
            //�ظ�
            if (Bll.All.Count(role => role.RoleName == t.RoleName) > 0)
            {
                "RoleName".AE("��ɫ�Ѵ���", ModelState);                
            }

            return "";
        }
        /// <summary>
        /// �ж����ݿ��Ƿ���ڷ��޸Ľ�ɫ�Ľ�ɫ����
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected override string OnEditCK(aspnet_Roles t)
        {
            //�ظ�
            if (Bll.All.Count(role => role.RoleName == t.RoleName && role.RoleId != t.RoleId) > 0)
            {
                "RoleName".AE("��ɫ�Ѵ���", ModelState);
            }

            return "";
        }

        //�༭��ɫ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult Update(aspnet_Roles t, string returnUrl)
        {
            if (ResourceId.HP(����.�޸�))
            {
                var oper = new OperationResult(OperationResultType.Error, "�޸�ʧ��");
                var model = Bll.GetModel(t);
                if (model == null)
                {
                    "".AE("�޸�ʧ��", ModelState);
                }
                OnEditCK(t);
                if (ModelState.IsValid)
                {
                    TryUpdateModel<aspnet_Roles>(model);
                    model.LoweredRoleName = t.RoleName.ToLower();
                    Bll.Update(model);
                    fillOperationResult(returnUrl, oper, "�޸ĳɹ�");
                }
                else
                {
                    "".AE("�޸�ʧ��", ModelState);
                    oper.AppendData = ModelState.GE();
                }
                return Json(oper, JsonRequestBehavior.AllowGet);
            }
            else
            {
                "".AE("��Ȩ��", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "��Ȩ��");
                oper.AppendData = ModelState.GE();
                return Json(oper, JsonRequestBehavior.AllowGet);
            }            
        }

        //ɾ��ǰ��֤
        protected override string OnDelCk(aspnet_Roles t)
        {
            var roles = dotNetRoles.GetUsersInRole(t.RoleName);
            if (roles.Count() != 0)
            {
                //�й���Աʹ�øý�ɫ!
                "RoleName".AE("�ý�ɫ�����û�ʹ��", ModelState);                
            }
            //ɾ���ý�ɫ������Ȩ��
            var privRoleBll = new BaseBll<wmfPrivilegeInRole>();
            var privRole = privRoleBll.All.Where(r => r.RoleId == t.RoleId);
            PirBll.Delete(privRole);
            return "";
        }

        /// <summary>
        /// ����Ȩ��
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Manage(RoleVModel vmodel, string returnUrl)
        {
            if (ResourceId.HP(����.�޸�))
            {
                var oper = new OperationResult(OperationResultType.Error, "�޸�ʧ��");
                //�����Ľ�ɫ
                var role = vmodel.CheckedRole;
                if(role != null)
                { 
                    //ɾ���ý�ɫ�����в���
                    var delPirs = role.wmfPrivilegeInRoles.Where(pir => pir.RoleId == vmodel.RoleId);
                    PirBll.Delete(delPirs);

                    if (vmodel.PrivId != null && vmodel.PrivId.Count() != 0)
                    {
                        //��ѡ�еĲ�������
                        var privs = vmodel.Privileges.Where(p => vmodel.PrivId.Contains(p.ID));
                        if (privs.Count() != 0)
                        {
                            //�����µ�����
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
                    //ִ�и���
                    PirBll.UpdateChanges();
                    fillOperationResult(returnUrl, oper, "�޸ĳɹ�");                    
                }
                else
                {
                    "".AE("�޸�ʧ��", ModelState);
                    oper.AppendData = ModelState.GE();
                }
                return Json(oper, JsonRequestBehavior.AllowGet);
            }
            else
            {
                "".AE("��Ȩ��", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "��Ȩ��");
                oper.AppendData = ModelState.GE();
                return Json(oper, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RoleToPrivilege(string ParentResourceID = "9dd4aec9-fa03-46ec-8be6-6d157df3e221")
        {
            if (ResourceId.HP(����.�޸�))
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
                "".AE("��Ȩ��", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "��Ȩ��");
                oper.AppendData = ModelState.GE();
                return Json(oper, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleToPrivilege(string[] Roles, string[] Privileges, string returnUrl, bool deletePrivButNotAdd = false)
        {
            if (ResourceId.HP(����.�޸�))
            {
                var oper = new OperationResult(OperationResultType.Error, "�޸�ʧ��");
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
                                //��ɾ��
                                foreach (var privilege in Privileges)
                                {
                                    var privilegeGuid = Guid.Empty;
                                    if (Guid.TryParse(privilege, out privilegeGuid))
                                    {
                                        var privilegeModel = privilegeBll.GetModel(privilegeGuid);
                                        var officePrivs = roleModel.wmfPrivilegeInRoles.Where(u => u.wmfPrivilege.wmfResource.ID == privilegeModel.ResourceId);
                                        PirBll.Delete(officePrivs);
                                    }
                                }
                                //ֻɾ��Ȩ�޲�����Ȩ�ޡ�
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
                    fillOperationResult(returnUrl, oper, "�޸ĳɹ�");
                }                
                else
                {
                    "".AE("�޸�ʧ��", ModelState);
                    oper.AppendData = ModelState.GE();
                }
                return Json(oper, JsonRequestBehavior.AllowGet);
            }
            else
            {
                "".AE("��Ȩ��", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "��Ȩ��");
                oper.AppendData = ModelState.GE();
                return Json(oper, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// webconfig���ܽ���
        /// </summary>
        /// <returns></returns>
        public string ENCWeb()
        {
            var provider = "RSAProtectedConfigurationProvider";
            var section = "connectionStrings";
            var section1 = "quartz";
            var section2 = "log4net";
            Configuration confg = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
            ConfigurationSection configSect = confg.GetSection(section);
            if (configSect != null)
            {
                configSect.SectionInformation.ProtectSection(provider);
                confg.Save();
            }

            ConfigurationSection configSect1 = confg.GetSection(section1);
            if (configSect1 != null)
            {
                configSect1.SectionInformation.ProtectSection(provider);
                confg.Save();
            }

            ConfigurationSection configSect2 = confg.GetSection(section2);
            if (configSect2 != null)
            {
                configSect2.SectionInformation.ProtectSection(provider);
                confg.Save();
            }
            return "";
        }

        public string DECWeb()
        {
            var provider = "RSAProtectedConfigurationProvider";
            var section = "connectionStrings";
            var section1 = "quartz";
            var section2 = "log4net";
            Configuration config = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
            ConfigurationSection configSect = config.GetSection(section);
            if (configSect.SectionInformation.IsProtected)
            {
                configSect.SectionInformation.UnprotectSection();
                config.Save();
            }

            ConfigurationSection configSect1 = config.GetSection(section1);
            if (configSect1.SectionInformation.IsProtected)
            {
                configSect1.SectionInformation.UnprotectSection();
                config.Save();
            }

            ConfigurationSection configSect2 = config.GetSection(section2);
            if (configSect2.SectionInformation.IsProtected)
            {
                configSect2.SectionInformation.UnprotectSection();
                config.Save();
            }
            return "";
        }

    }
}