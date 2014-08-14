using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using MorSun.Bll;

namespace MorSun.Controllers.ViewModel
{
    public class RefGroupVModel:BaseVModel<wmfRefGroup>
    {
        /// <summary>
        /// 被选中的编号
        /// </summary>
        //public virtual Guid? CheckedId { get; set; }

        /// <summary>
        /// 获取跟目录
        /// </summary>
        public virtual IQueryable<wmfRefGroup> Roots
        {
            get
            {
                var l = base.All;

                if (FlagTrashed == "0")
                {//回收站不能只取根节点
                    l = l.Where(p => p.ParentId == Guid.Empty || p.ParentId == null);
                }
                if (String.IsNullOrEmpty(FlagTrashed) || (!FlagTrashed.Eql("0") && !FlagTrashed.Eql("1")))
                    FlagTrashed = "0";
                if (FlagTrashed == "1")
                {
                    l = l.Where(p => p.FlagTrashed == true);
                }
                if (FlagTrashed == "0")
                {
                    l = l.Where(p => p.FlagTrashed == false);
                }

                return l.OrderBy(p => p.Sort).ThenBy(p => p.RefGroupName);
            }
        }

        public override IQueryable<wmfRefGroup> List
        {
            get
            {
                var l = All;
                if (String.IsNullOrEmpty(FlagTrashed))
                    FlagTrashed = "0";
                if (FlagTrashed == "1")
                {
                    l = l.Where(p => p.FlagTrashed == true);
                }
                if (FlagTrashed == "0")
                {
                    l = l.Where(p => p.FlagTrashed == false);
                }
                if(sIsSort != null && sIsSort.Value == true)
                {
                    if (sParentId != null)
                        l = l.Where(p => p.ParentId == sParentId);
                    else
                        l = l.Where(p => p.ParentId == null);
                }
                return l.OrderBy(p => p.Sort).ThenBy(p => p.RefGroupName);
            }
        }

        /// <summary>
        /// 获取被选中的类型组
        /// </summary>
        //public virtual wmfRefGroup RefGroup
        //{
        //    get
        //    {
        //        var refGroup = All.FirstOrDefault(r => r.ID == CheckedId);
        //        return refGroup ?? First;
        //    }
        //}

       private BaseBll<wmfReference> referBll;
        /// <summary>
        /// 类型
        /// </summary>
        public virtual BaseBll<wmfReference> ReferBll
        {
            get
            {
                referBll = referBll.Load();
                return referBll;
            }
            set { referBll = value; }
        }

        /// <summary>
        /// 类型
        /// </summary>
        public virtual IQueryable<wmfReference> Refer
        {
            get
            {
                return ReferBll.All;
            }
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

        public string sRefGroupName { get; set; }

        public Guid? sParentId { get; set; }

        public bool? sIsSort { get; set; }
    }
}
