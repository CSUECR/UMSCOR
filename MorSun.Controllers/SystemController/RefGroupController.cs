using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using HOHO18.Common;
using MorSun.Bll;
using System.Web.Mvc;
using MorSun.Controllers.ViewModel;
using System.Xml;

namespace MorSun.Controllers.CommonController
{
    public class RefGroupController : BaseController<wmfRefGroup>
    {
        protected override string ResourceId
        {
            get { return MorSun.Common.Privelege.资源.类别组; }
        }

        private BaseBll<wmfRefGroup> _depBll;

        public BaseBll<wmfRefGroup> RefGroupBll
        {
            get
            {
                _depBll = _depBll.Load();
                return _depBll;
            }
            set { _depBll = value; }
        }

        public override string Create(wmfRefGroup t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.添加))
            {
                string msg = "";
                string[] refGroupNames = ((t.RefGroupName == null) ? (t.RefGroupName = " ").Split(',') : t.RefGroupName.Split(','));
                for (int i = 0; i < refGroupNames.Length; i++)
                {
                    if (refGroupNames.Length == 1)
                    {
                        t.RefGroupName = refGroupNames[0];
                        msg = OnPreCreateCK(t);
                        if (msg == "true")
                        {
                            return base.Create(t);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(refGroupNames[i]))
                        {
                            t.RefGroupName = refGroupNames[i];
                            msg = OnPreCreateCK(t);
                            if (msg == "true")
                            {
                                var newRefGroup = new wmfRefGroup();
                                newRefGroup.RefGroupName = refGroupNames[i];
                                NCreate(newRefGroup);
                            }
                        }
                        msg = "true";
                    }
                }

                return msg;
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
            }
        }

        public string TreeTableMove(string id, string pid)
        {
            var p1 = Guid.Parse(id.Replace("node-", ""));
            //父ID
            var p2 = Guid.Parse(pid.Replace("node-", ""));

            //不能将自己当做父节点
            if (p1 == p2)
            {
                //移动失败，部门A不能移动到部门A下！
                //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfRefGroup>("自己的类别组不能移到自己的类别组"), "") });
            }

            ///判断ID与父级ID相同
            if (SearchDep(p1, p2))
            {
                //上级部门不能往自己的下级部门移动！
                //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfRefGroup>("上级类别组不能移到下级类别组"), "") });
            }
            else
            {
                var ss = RefGroupBll.GetModel(p1);
                ss.ParentId = p2;
                RefGroupBll.Update(ss);
                return "true";
            }
        }

        public bool SearchDep(Guid p1, Guid p2)
        {
            var dept = RefGroupBll.All.FirstOrDefault(r => r.ID == p2);
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


        protected override string OnPreCreateCK(wmfRefGroup t)
        {
            string ret = "true";
            var ReferGrop = Bll.All.FirstOrDefault(r => r.RefGroupName == t.RefGroupName);
            if (ReferGrop != null)
            {
                //该类别组已经存在，请重新输入！
                //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfRefGroup>("类别组已存在"), "") });
            }

            return ret;
        }

        protected override string OnEditCK(wmfRefGroup t)
        {
            string ret = "true";
            var ReferGrop = Bll.All.FirstOrDefault(r => r.RefGroupName == t.RefGroupName);
            if (ReferGrop != null && ReferGrop.ID != t.ID)
            {
                //该类别组已经存在，请重新输入！
                //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfRefGroup>("类别组已存在"), "") });
            }
            var p1 = t.ID;
            //父ID
            var p2 = t.ParentId.ToAs<Guid>();

            //不能将自己当做父节点
            if (p1 == p2)
            {
                //移动失败，类别组A不能移动到类别组A下！
                //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfRefGroup>("自己的类别组不能移到自己的类别组"), "") });
            }

            ///判断ID与父级ID相同
            if (SearchDep(p1, p2))
            {
                //上级类别组不能往自己的下级类别组移动！
                //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfRefGroup>("上级类别组不能移到下级类别组"), "") });
            }
            return ret;
        }

        protected override string OnDelCk(wmfRefGroup t)
        {
            string ret = "true";
            var BaseBll = new BaseBll<wmfReference>();
            var model = BaseBll.All.FirstOrDefault(r => r.RefGroupId == t.ID);
            if (model != null)
            {
                //该类别组类别存在，请重新输入！
                //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfRefGroup>("请先删除类别组下的类别"), "") });
            }
            return ret;
        }

        public virtual ActionResult Left(RefGroupVModel t)
        {
            return View(t);
        }
        public virtual ActionResult TaskLeft(RefGroupVModel t)
        {
            return View(t);
        }

    }
}
