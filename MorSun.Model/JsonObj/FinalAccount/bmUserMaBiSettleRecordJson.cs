using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web.Mvc;
using HOHO18.Common;

namespace MorSun.Model
{
    public class bmUserMaBiSettleRecordJson
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ID
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? UserId
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? MaBiRef
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? MaBiNum
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? SettleTime
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? Sort
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? RegUser
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