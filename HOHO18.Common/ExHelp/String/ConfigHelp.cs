using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web;
using HOHO18.Common;
using HOHO18.Common.Web;

namespace System
{
    /// <summary>
    /// 文本处理
    /// </summary>
    public static class ConfigHelper
    {
        
        #region 获取配置值
        /// <summary>
        /// 获取XMLConfig的值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GX(this string str)
        {
            return XmlConfigHelper.GetXmlConfig(str);
        }

        /// <summary>
        /// 根据语言类型获取XMLConfigMenu的按钮名称
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GXM(this string str)
        {
            return XmlConfigHelper.GetXmlConfigMenu(str);
        }


        public static string GW(this string str)
        {
            return WebConfigHelper.GetWebConfig(str);
        }

        public static string GHU(this String str)
        {
            var u = new StringBuilder();
            u.Append("http://");            
            if(HttpContext.Current.Request.Url.Port != 80)
            {                
                u.Append(HttpContext.Current.Request.Url.Authority);
            }
            else
            {
                u.Append(HttpContext.Current.Request.Url.Host);
            }            
            return u.ToString();
        }        
        #endregion

    }
}
