using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;

namespace MorSun.Controllers.ViewModel
{
    public class NavigationLinkVModel : BaseVModel<wmfNavigationLink>
    {
        public override IQueryable<wmfNavigationLink> List
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
                if (MenuName != null)
                {
                    l = l.Where(p => p.MenuName == MenuName);
                }                
                if (RefId != null && RefId != Guid.Empty)
                {
                    l = l.Where(p => p.RefId == RefId);
                }

                return l.OrderBy(p => p.Sort).ThenBy(p => p.MenuName);
            }
        }

        /// <summary>
        /// 获取跟目录
        /// </summary>
        public virtual IQueryable<wmfNavigationLink> Roots
        {
            get
            {
                var l = base.All;
                if (RefId != null)
                {
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
                    if (RefId != null && RefId != Guid.Empty)
                    {
                        l = l.Where(p => p.RefId == RefId);
                    }
                }
                else if (RefId == null && FlagTrashed == "1")
                {
                    l = l.Where(p => p.FlagTrashed == true);
                }
                else
                    l = l.Take(0);
                return l.OrderBy(p => p.Sort).ThenBy(p => p.MenuName);
            }
        }

        public virtual Guid? RefId { get; set; }
        /// <summary>
        /// 被选中的编号
        /// </summary>
        public virtual string CheckedId { get; set; }

        public virtual string MenuName { get; set; }

        public virtual string FlagTrashed { get; set; }

        /// <summary>
        /// 被选中的资源编号
        /// </summary>
        public virtual Guid[] RIds { get; set; }

        public Guid? sParentId { get; set; }

        public bool? sIsSort { get; set; }
    }
}
