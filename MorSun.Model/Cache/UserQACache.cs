using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HOHO18.Common;
using System.Data.Linq;
using System.ComponentModel.DataAnnotations;

namespace MorSun.Model
{    
    /// <summary>
    /// 考虑到并发问题，这边一个在线答题用户一个缓存。不管用户放弃还是答题，已经处理过的问题都进AlreadyQA
    /// 注意缓存的设置，有问题和分配项两个都要设置
    /// </summary>
    public class UserQACache 
    {
        /// <summary>
        /// 微信用户ID
        /// </summary>
        public string WeiXinId { get; set; }

        /// <summary>
        /// 当前回答问题
        /// </summary>
        public bmQAView CurrentQA { get; set; }

        /// <summary>
        /// 待答问题
        /// </summary>
        public List<bmQAView> WaitQA { get; set; }

        /// <summary>
        /// 已答问题
        /// </summary>
        //public List<bmQA> AlreadyQA { get; set; }

        ///// <summary>
        ///// 当前分配项
        ///// </summary>
        //public bmQADistribution CurrentQADis { get; set; }

        ///// <summary>
        ///// 待处理的分配项
        ///// </summary>
        //public IQueryable<bmQADistribution> WaitQADis { get; set; }

        ///// <summary>
        ///// 已处理的分配项
        ///// </summary>
        //public IQueryable<bmQADistribution> AlreadyQADis { get; set; }
    }
}
