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
    /// 在线用户缓存 每5分钟更新一次，5分钟可配置
    /// </summary>
    public class OlineQAUserCache 
    {
        /// <summary>
        /// 缓存刷新时间
        /// </summary>
        public DateTime RefreshTime { get; set; }        

        /// <summary>
        /// 认证的答题用户
        /// </summary>
        public IQueryable<bmOnlineQAUser> CertificationUser { get; set; }

        /// <summary>
        /// 未认证的答题用户
        /// </summary>
        public IQueryable<bmOnlineQAUser> NonCertificationQAUser { get; set; }
    }
}
