﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using MorSun.Bll;
using MorSun.Common.类别;

namespace MorSun.Controllers.ViewModel
{
    public class BMQAViewVModel:BaseVModel<bmQAView>
    {
        /// <summary>
        /// 被选中的编号
        /// </summary>
        //public virtual Guid? CheckedId { get; set; }

        /// <summary>
        /// 获取跟目录
        /// </summary>
        public virtual IQueryable<bmQAView> Roots
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

                return l.OrderBy(p => p.RegTime);
            }
        }

        public virtual IQueryable<bmQAView> Others
        {
            get
            {
                var refAId = Guid.Parse(Reference.问答类别_答案);
                var refBSId = Guid.Parse(Reference.问答类别_不是问题);
                var l = base.All.Where(p => p.ParentId == sParentId && p.QARef != refAId && p.QARef != refBSId);
                return l.OrderBy(p => p.RegTime);
            }
        }

        /// <summary>
        /// 答案
        /// </summary>
        public virtual bmQAView A
        {
            get
            {
                var refAId = Guid.Parse(Reference.问答类别_答案);
                var refBSId = Guid.Parse(Reference.问答类别_不是问题);
                return base.All.FirstOrDefault(p => p.ParentId == sParentId && (p.QARef == refAId || p.QARef == refBSId));
            }
        }


        /// <summary>
        /// 问题
        /// </summary>
        public virtual bmQAView Q
        {
            get
            {
                return base.All.FirstOrDefault(p => p.ID == sParentId);
            }
        }

        public virtual IQueryable<bmOBView> Objecs
        {
            get
            {
                return new BaseBll<bmOBView>().All.Where(p => p.QAId == sParentId);
            }
        }

        public override IQueryable<bmQAView> List
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
                return l.OrderBy(p => p.RegTime);
            }
        }
        

        public Guid? sParentId { get; set; }

        public bool? sIsSort { get; set; }
    }
}
