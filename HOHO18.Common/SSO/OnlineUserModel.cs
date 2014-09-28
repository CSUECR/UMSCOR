using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HOHO18.Common.SSO
{
    public class OnlineUserModel
    {
        /// <summary>
        /// 用户唯一ID，多个应用合并数据的唯一标识
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { set; get; }

        /// <summary>
        /// 最近登录时间
        /// </summary>
        public DateTime LastLoginTime { set; get; }

        /// <summary>
        /// 从哪个应用登录
        /// </summary>
        public string LoginAppName { set; get; }

        public override string ToString()
        {
            return string.Format("UserName:{0},LastLoginTime:{1},LoginAppName:{2}", UserName,
                                 LastLoginTime.ToString("yyyy-MM-dd HH:mm:ss:fff"), LoginAppName);
        }
    }
}
