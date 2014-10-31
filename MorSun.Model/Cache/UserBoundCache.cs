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
    /// 这边的缓存Key是 bd + 用户ID 缓存时间为2分钟
    /// </summary>
    public class UserBoundCache 
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 绑定代码
        /// </summary>
        public int BoundCode { get; set; }        
    }
}
