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

namespace MorSun.Controllers.CommonController
{
    public class PositionController : BaseController<wmfPosition>
    {
        protected override string ResourceId
        {
            get {return MorSun.Common.Privelege.资源.岗位; }
        }

        private BaseBll<wmfPosition> _positionBll;

        public BaseBll<wmfPosition> PositionBll
        {
            get
            {
                _positionBll = _positionBll.Load();
                return _positionBll;
            }
            set { _positionBll = value; }
        }


        //直接删除
        protected override string OnDelCk(wmfPosition t)
        {
            var userDeptPositionBll = new BaseBll<wmfUserDeptPosition>();
            var userDeptPosition = userDeptPositionBll.All.Where(r => r.PostionId == t.ID).FirstOrDefault();
            if (userDeptPosition != null)
            {
                //该岗位有员工使用，不能删除！
                //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfPosition>("该岗位有员工使用"), "") });
            }
            return "true";
        }

        

        //创建前验证
        protected override string OnPreCreateCK(wmfPosition t)
        {
            var Position = PositionBll.All.FirstOrDefault(r => r.PositionName == t.PositionName && r.DeptId == t.DeptId && r.FlagTrashed == false);
            if (Position != null)
            {
                //该部门已经存在，请重新输入！
                //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfPosition>("岗位已存在"), "") });
            }
            return "true";
        }

        //编辑前验证
        protected override string OnEditCK(wmfPosition t)
        {
            var Position = PositionBll.All.FirstOrDefault(r => r.PositionName == t.PositionName && r.FlagTrashed == false);
            if (Position != null && Position.ID != t.ID)
            {
                //该操作名称已经存在，请重新输入！
                //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfPosition>("岗位已存在"), "") });
            }
            return "true";
        }
        
        /// <summary>
        /// 设置某个岗位为该部门的leader，目前用在工作流上面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string SetDeptLeader(Guid? id)
        {
            if (null == id || Guid.Empty == id) { }
                //return getErrJson(new RuleViolation("positionId为空"));

            var originalPosition = Bll.All.FirstOrDefault(u => u.ID == id);
            if (originalPosition == null) { }
                //return getErrJson(new RuleViolation("未找到对应的岗位实例"));

            //将岗位中的leader属性设置为1则为该部门的领导，主要用在工作流本部门领导设置那边；若leader为null，则为普通岗位
            if (originalPosition.Leader == null)
            {
                originalPosition.Leader = 1;                 
            }
            else
            {
                originalPosition.Leader = null;
            }
            //更新对象
            if (!TryUpdateModel(originalPosition))
            {
                return getErrJson(new RuleViolation("未能更新岗位对象"));
            }
            else
            {
                Bll.UpdateChanges();
                return "true";
            }

        }

    }
}
