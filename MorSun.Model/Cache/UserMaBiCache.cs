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
    /// 用户马币缓存
    /// </summary>
    public class UserMaBiCache 
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 微信用户ID
        /// </summary>
        public string WeiXinId { get; set; }

        /// <summary>
        /// 当前问题ID
        /// </summary>
        public UserMaBi UMB { get; set; }        
    }
}
