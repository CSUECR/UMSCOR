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
                if (MenuName != null)
                {
                    l = l.Where(p => p.MenuName == MenuName);
                }
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

                return from q in l orderby q.Sort ascending select q;
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
    }
}
