using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web.Mvc;
using HOHO18.Common;

namespace MorSun.Model
{    
    public class bmQAJson
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ID
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long AutoGrenteId
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? ParentId
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? UserId
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string WeiXinId
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? QARef
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string QAContent
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MsgId
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? MsgType
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MediaId
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PicUrl
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