using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web.Mvc;
using HOHO18.Common;

namespace MorSun.Model
{
    public class bmQADistributionJson
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ID
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid QAId
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid UserId
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string WeiXinId
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? DistributionTime
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? OperateTime
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid Result
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? Sort
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid RegUser
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? RegTime
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ModTime
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool FlagTrashed
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool FlagDeleted
        { get; set; }
    }

}