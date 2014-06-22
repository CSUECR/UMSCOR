using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Data.Objects;
using MorSun.Model;
using System.Web.Mvc;
using MorSun.Bll;

namespace MorSun.Controllers.ViewModel
{
    //[Bind]
    public class DeptVModel : BaseVModel<wmfDept>
    {

        /// <summary>
        /// 获取跟目录
        /// </summary>
        public virtual IQueryable<wmfDept> Roots
        {
            get
            {
                var l = base.All;

                l = l.Where(dep => dep.ParentId == Guid.Empty || dep.ParentId == null);
                if (FlagTrashed == "1")
                {
                    l = l.Where(p => p.FlagTrashed == true);
                }
                if (FlagTrashed == "0")
                {
                    l = l.Where(p => p.FlagTrashed == false);
                }

                return from q in l orderby q.Sort select q;
            }
        }


        public override IQueryable<wmfDept> List
        {
            get
            {
                var l = base.All;
                if (FlagTrashed == "1")
                {
                    l = l.Where(p => p.FlagTrashed == true);
                }
                if (FlagTrashed == "0")
                {
                    l = l.Where(p => p.FlagTrashed == false);
                }

                return from q in l orderby q.Sort select q;
            }
        }


        public override IQueryable<wmfDept> All
        {
            get
            {
                return base.All.Where(p => p.FlagTrashed == false).OrderBy(q => new { q.DeptName });
            }
        }

        public string GetDeptName(Guid? deptId)
        {
            var model = base.Dao.GetModel(deptId);
            return model == null ? "" : model.DeptName;
        }

        public string GetDeptNameByUserId(Guid? userId)
        {
            var user = new BaseBll<aspnet_Users>().GetModel(userId);
            if (user == null)
                return "";
            else
            {
                return user.wmfUserDeptPositions.First().wmfDept.DeptName;
            }
        }


        public wmfDept GetDeptByUserId(Guid? userId)
        {
            var user = new BaseBll<aspnet_Users>().GetModel(userId);
            if (user == null)
                return null;
            else
            {
                return user.wmfUserDeptPositions.First().wmfDept;
            }
        }

        public string GetDeptIdOfCurrentUser()
        {
            var dept = this.GetDeptByUserId(UserID);
            return dept == null ? "" : dept.ID.ToString();
        }



       
    }
}
