﻿using System;
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
        public virtual Guid? CheckedId { get; set; }

        public virtual Guid? ProjectId { get; set; }

        /// <summary>
        /// 获取跟目录
        /// </summary>
        public virtual IQueryable<wmfRefGroup> Roots
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

                return from q in l orderby q.RefGroupName descending select q;
            }
        }

        public override IQueryable<wmfRefGroup> List
        {
            get
            {
                return All;
            }
        }

        /// <summary>
        /// 获取被选中的类型组
        /// </summary>
        public virtual wmfRefGroup RefGroup
        {
            get
            {
                var refGroup = All.FirstOrDefault(r => r.ID == CheckedId);
                return refGroup ?? First;
            }
        }

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

        public string refGroupName { get; set; }
    }
}
