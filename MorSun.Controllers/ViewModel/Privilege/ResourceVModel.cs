using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;

namespace MorSun.Controllers.ViewModel
{
    /// <summary>
    /// 资源
    /// </summary>
    public class ResourceVModel : BaseVModel<wmfResource>
    {
        ///// <summary>
        ///// 被选中的编号
        ///// </summary>
        //public virtual string CheckedId { get; set; }

        public override IQueryable<wmfResource> List
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
                if (sIsSort != null && sIsSort.Value == true)
                {
                    if (sParentId != null)
                        l = l.Where(p => p.ParentId == sParentId);
                    else
                        l = l.Where(p => p.ParentId == null);
                }
                return l.OrderBy(p => p.Sort).ThenBy(p => p.ResourceCNName);
            }
        }

        /// <summary>
        /// 获取跟目录
        /// </summary>
        public virtual IQueryable<wmfResource> Roots
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
                return l.OrderBy(p => p.Sort).ThenBy(p => p.ResourceCNName);
            }
        }


        public Guid? sParentId { get; set; }

        public bool? sIsSort { get; set; }
    }
}
