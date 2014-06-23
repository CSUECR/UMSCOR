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
using HOHO18.Common;
using MorSun.Common;

namespace MorSun.Controllers.CommonController
{
    public class DeptController : BaseController<wmfDept>
    {
        protected override string ResourceId
        {
            get { return MorSun.Common.Privelege.资源.部门; }
        }

        private BaseBll<wmfDept> _depBll;

        public BaseBll<wmfDept> DepBll
        {
            get
            {
                _depBll = _depBll.Load();
                return _depBll;
            }
            set { _depBll = value; }
        }

        //public override ActionResult Index()
        //{
        //    //for (int i = 3; i <= 98; i++)
        //    //{
        //    //    var t = new wmfDept();
        //    //    t.DeptName = i.ToString();
        //    //    t.ParentId = Guid.Parse("796d4378-d70d-44ad-8b0c-04c058596ba1");
        //    //    t.Tel = "123456";
        //    //    t.RegTime = DateTime.Now;
        //    //    Create(t);
        //    //}

        //    return base.Index();
        //}

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

            //取出所有部门
            var depAll = DepBll.All;
            var cDeptList = depAll.Where(t => t.ParentId == p2);
            var cDept = depAll.Where(t => t.ID == p1).FirstOrDefault();
            //不能将自己当做父节点
            if (p1 == p2)
            {
                //移动失败，部门A不能移动到部门A下！
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfDept>("自己的部门不能移到自己的部门"), "") });
            }

            //判断该 父节点下是否存在相同的部门名称
            var isDeptName = false;
            foreach (var item in cDeptList)
            {
                if (item.DeptName == cDept.DeptName)
                {
                    isDeptName = true;
                    break;
                }
            }

            if (isDeptName)
            {
                //该部门已经存在，请重新输入！
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfDept>("部门已存在"), "") });
            }


            ///判断ID与父级ID相同
            if (SearchDep(p1, p2))
            {
                //上级部门不能往自己的下级部门移动！
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfDept>("上级部门不能移到下级部门"), "") });
            }
            else
            {

                var t = DepBll.GetModel(p1);
                t.ParentId = p2;


                

                //更新
                DepBll.Update(t);
                return "true";
            }
        }


        public bool SearchDep(Guid p1, Guid p2)
        {
            var dept = DepBll.All.FirstOrDefault(r => r.ID == p2);
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



        protected override string OnPreCreateCK(wmfDept t)
        {
            //取出所有部门
            var depAll = DepBll.All;
            var dept = depAll.FirstOrDefault(r => r.DeptName == t.DeptName && r.ParentId == t.ParentId);
            if (dept != null)
            {
                //该部门已经存在，请重新输入！
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfDept>("部门已存在"), "") });
            }

            
            return "true";
        }

        protected override string OnEditCK(wmfDept t)
        {
            //取出所有部门
            var depAll = DepBll.All;
            string ret = "true";
            var dept = depAll.FirstOrDefault(r => r.DeptName == t.DeptName && r.ParentId == t.ParentId);
            if (dept != null && t.ID != dept.ID)
            {
                //该类别已经存在，请重新输入！
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfDept>("部门已存在"), "") });
            }
            var p1 = t.ID;
            //父ID
            var p2 = t.ParentId.ToAs<Guid>();

            //不能将自己当做父节点
            if (p1 == p2)
            {
                //移动失败，部门A不能移动到部门A下！
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfDept>("自己的部门不能移到自己的部门"), "") });
            }

            ///判断ID与父级ID相同
            if (SearchDep(p1, p2))
            {
                //上级部门不能往自己的下级部门移动！
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfDept>("上级部门不能移到下级部门"), "") });
            }            
            return ret;
        }


        


        protected override string OnDelCk(wmfDept t)
        {
            string ret = "true";
            var dept = Bll.All.FirstOrDefault(r => r.ParentId == t.ID);
            var userDeptPositionBll = new BaseBll<wmfUserDeptPosition>();
            var userDeptPosition = userDeptPositionBll.All.Where(r => r.DeptId == t.ID).FirstOrDefault();
            var positionBll = new BaseBll<wmfPosition>();
            var pisition = positionBll.All.Where(r => r.DeptId == t.ID).FirstOrDefault();
            if (userDeptPosition != null || dept != null || pisition != null)
            {
                //该部门存在下级部门或岗位有所属，请先进行删除或调整岗位或下级部门!
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfDept>("该部门存在下级部门或岗位有所属"), "") });
            }
            return ret;
        }


    }
}
