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
    public class ResourcesVModel : BaseVModel<wmfResource>
    {

        /// <summary>
        /// 被选中的编号
        /// </summary>
        public virtual string CheckedId { get; set; }

        public override IQueryable<wmfResource> List
        {
            get
            {
                return All.OrderBy(t => t.Sort);
                // var l = All;
                //if (!string.IsNullOrEmpty(ResourcesCNName))
                //{
                //    l = l.Where(r => r.ResourcesCNName.Contains(ResourcesCNName));
                //}
                //if (ResourcesID != null && ResourcesID != Guid.Empty)
                //{
                //    l = l.Where(r => r.ParentId == ResourcesID);
                //}
                //if (!string.IsNullOrEmpty(ResourcesName))
                //{
                //    l = l.Where(r => r.ResourcesName.Contains(ResourcesName));
                //}
                // return from q in l orderby q.Sort ascending select q;
            }
        }

        /// <summary>
        /// 获取跟目录
        /// </summary>
        public virtual IQueryable<wmfResource> Roots
        {
            get
            {
                var l = All;

                l = l.Where(dep => dep.ParentId == Guid.Empty || dep.ParentId == null);
                //if (FlagTrashed == "1")
                //{
                //    l = l.Where(p => p.FlagTrashed == true);
                //}
                //if (FlagTrashed == "0")
                //{
                //    l = l.Where(p => p.FlagTrashed == false);
                //}

                return l;
            }
        }
    }
}
