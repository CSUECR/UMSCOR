using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using MorSun.Model;
using System.Web.Mvc;
using MorSun.Bll;

namespace MorSun.Controllers.ViewModel
{
    //[Bind]
    public class PositionVModel : BaseVModel<wmfPosition>
    {

        /// <summary>
        /// 获取跟目录
        /// </summary>
        public override IQueryable<wmfPosition> List
        {
            get
            {
                var l = All;
                if (!String.IsNullOrEmpty(PositionName))
                {
                    l = l.Where(p => p.PositionName == PositionName);
                }
                if (Dep != null)
                {
                    l = l.Where(p => p.DeptId == Dep);
                }
                //已经被，回收站
                if (FlagTrashed == "1")
                {
                    l = l.Where(p => p.FlagTrashed == true);
                }
                if (FlagTrashed == "0")
                {
                    l = l.Where(p => p.FlagTrashed == false);
                }
                //已经被删除到回收站的部门下的岗位不要取出来
                l = l.Where(p => p.wmfDept.FlagTrashed == false);
                return l.OrderBy(p => p.wmfDept.Sort).ThenBy(p => p.Sort);
            }
        }

        public Guid? Dep { get; set; }

        public String PositionName { get; set; }

        /// <summary>
        /// 被选中的编号
        /// </summary>
        public virtual string CheckedId { get; set; }

        public virtual string FlagTrashed { get; set; }

        public virtual string SysMenu { get; set; }
    }
}
