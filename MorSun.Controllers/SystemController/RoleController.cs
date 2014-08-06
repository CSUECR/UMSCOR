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

namespace MorSun.Controllers.SystemController
{
    //[Authorize(Roles = "ϵͳ����Ա")]
    [HandleError]
    public class RoleController : BaseController<aspnet_Roles>
    {
        protected override string ResourceId
        {
            get { return MorSun.Common.Privelege.��Դ.��ɫ����; }
        }


        public ActionResult RoleManage(RoleVModel vModel)
        {
            ViewBag.CanDoSth = this.CanDoSth;
            return View(vModel);
        }

        protected override aspnet_Roles NInsert(aspnet_Roles t)
        {
            Roles.CreateRole(t.RoleName);
            //�����������ӵķ���,��Ȼ��ӽ�ɫʱ��������Ӳ���ȥ��
            var model = Bll.All.Where(m => m.LoweredRoleName == t.RoleName).FirstOrDefault();
            model.Sort = t.Sort;
            Bll.Update(model);
            return t;
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
        protected override string OnPreCreateCK(aspnet_Roles t)
        {
            //�ظ�
            if (Bll.All.Count(role => role.RoleName == t.RoleName) > 0)
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Roles>("��ɫ�Ѵ���"), "RoleName") });
            }

            return "true";
        }

        //�༭��ɫ
        public override string Edit(aspnet_Roles t)
        {
            var roles = new BaseBll<aspnet_Roles>().GetModel(t);
            TryUpdateModel<aspnet_Roles>(roles);
            roles.LoweredRoleName = t.RoleName.ToLower();

            return base.Edit(roles);
        }


        public override ActionResult GetEdit(string id, aspnet_Roles t)
        {
            var guid = Guid.Empty;
            aspnet_Roles model = null;
            if (Guid.TryParse(id, out guid))
            {
                model = Bll.All.FirstOrDefault(u => u.RoleId == guid);
            }
            var js = JsHelper.Json(model);

            return Json("[" + js + "]");
        }

        //ɾ��ǰ��֤
        protected override string OnDelCk(aspnet_Roles t)
        {
            var roles = dotNetRoles.GetUsersInRole(t.RoleName);
            if (roles.Count() != 0)
            {
                //�й���Աʹ�øý�ɫ!
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Roles>("�й���Աʹ�øý�ɫ"), "") });
            }

            //ɾ���ý�ɫ������Ȩ��
            var privRoleBll = new BaseBll<wmfPrivilegeInRole>();
            var privRole = privRoleBll.All.Where(r => r.RoleId == t.RoleId);
            PirBll.Delete(privRole);
            return "true";
        }

        /// <summary>
        /// ����Ȩ��
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        public virtual string SavePriv(RoleVModel vmodel)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.����.����))
            {
                //�����Ľ�ɫ
                var role = vmodel.CheckedRole;
                //ɾ���ý�ɫ�����в���
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

                        //��ѡ�еĲ�������
                        var privs = vmodel.Privs.Where(p => privArrays.Contains(p.ID));
                        if (privs.Count() != 0)
                        {
                            //����µ�����
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
                //ִ�и���
                PirBll.UpdateChanges();


                //var treeStr = "";
                //var privilegeCheckRoleList = vmodel.CheckedRole.wmfPrivilegeInRoles;
                //var resourceList = vmodel.Resrcs;
                //foreach (var resrc in resourceList)
                //{
                //    //�����Ƿ�ȫ��ѡ��
                //    var ischildChecked = false;

                //    //Ȩ��
                //    var privList = resrc.wmfPrivileges.OrderBy(t => t.wmfOperation.Sort);
                //    foreach (var priv in privList)
                //    {
                //        var flag = privilegeCheckRoleList.Any(pir => pir.PrivilegeId == priv.ID);
                //        treeStr += "{ id: \"" + priv.ID + "\", pId: \"" + resrc.ID + "\", name: \"" + priv.PrivilegeCNName + "\"" + (flag ? ",checked:true" : "") + " },";
                //        if (flag)
                //        {
                //            ischildChecked = true;
                //        }
                //    }
                //    var reList = resourceList.Where(p => p.ParentId == resrc.ID);
                //    if (privList.Count() == 0 || reList.Count() != 0)
                //    {
                //        ischildChecked = vmodel.GetChecked(resourceList, privilegeCheckRoleList, resrc.ID);
                //    }

                //    treeStr += "{ id: \"" + resrc.ID + "\", pId: \"" + resrc.ParentId + "\", name: \"" + resrc.ResourcesCNName + "\"" + (ischildChecked ? ",checked:true" : "") + " },";
                //}

                //if (!string.IsNullOrEmpty(treeStr))
                //{
                //    treeStr = treeStr.Substring(0, treeStr.Length - 1);
                //}

                //treeStr = "var zNodes = [" + treeStr + "];";

                ////�ļ����·��
                //var path = Server.MapPath("/UploadFile/Role/" + vmodel.CheckedId + ".js");
                ////�ж�ָ�����ļ��Ƿ����
                //if (System.IO.File.Exists(path))
                //{
                //    //ɾ���ļ�
                //    HOHO18.Common.FileObj.FileDel(path);
                //}

                ////��������
                //HOHO18.Common.FileObj.WriteFile(path, treeStr);
                return "true";
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"), "") });
            }
        }


        //����Ȩ�޷�Χ
        public virtual string SavePrivilegeRange(wmfPrivilegeInRole model)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.����.����))
            {
                var privilegeInRole = PirBll.GetModel(model.ID);
                TryUpdateModel<wmfPrivilegeInRole>(privilegeInRole);
                //ִ�и���
                PirBll.Update(privilegeInRole);
                return "true";
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"), "") });
            }
        }

        public ActionResult RoleToPrivilege(string ParentResourceID = "9dd4aec9-fa03-46ec-8be6-6d157df3e221")
        {
            var roleBll = new BaseBll<aspnet_Roles>();
            var resourceBll = new BaseBll<wmfResource>();

            var ParentResourceGuid = Guid.Parse(ParentResourceID);
            ViewBag.ParentResourceID = ParentResourceGuid;
            ViewBag.ParentResoures = resourceBll.All.Where(u => u.wmfResource1.Any() && u.wmfResource1.FirstOrDefault().wmfPrivileges.Any()).OrderByDescending(u=>u.RegTime);
            ViewBag.Roles = roleBll.All.OrderBy(u => u.RoleName);
            ViewBag.Resources = resourceBll.All.First(u => u.ID == ParentResourceGuid).wmfResource1.OrderByDescending(u => u.RegTime);
            return View();
        }
        [HttpPost]
        public ActionResult RoleToPrivilege(string[] Roles, string[] Privileges, bool deletePrivButNotAdd = false)
        {
            if (Roles != null && Roles.Length > 0 && Privileges != null && Privileges.Length > 0)
            {
                var officeId = Guid.Parse("9dd4aec9-fa03-46ec-8be6-6d157df3e221");
                var roleBll = new BaseBll<aspnet_Roles>();
                var privilegeBll = new BaseBll<wmfPrivilege>();
                foreach (var role in Roles)
                {
                    var roleGuid = Guid.Empty;
                    if (Guid.TryParse(role, out roleGuid))
                    {
                        var roleModel = roleBll.All.FirstOrDefault(u => u.RoleId == roleGuid);

                        if (Privileges.Count() > 0)
                        {
                            //��ɾ��
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
                            //ֻɾ��Ȩ�޲����Ȩ�ޡ�
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
            }
            return Content("�ɹ�");
        }

    }
}
