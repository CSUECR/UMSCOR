using System;
using System.Configuration;
using System.Text;
using System.Data;

namespace HOHO18.Common
{
    /// <summary>
    /// 读取Session值
    /// </summary>
    public class SessionHelper
    {

        #region

        /// <summary>
        /// 当前语言
        /// </summary>
        /// <returns></returns>
        public static string GetSessionLanguages()
        {
            
            //系统默认语言
            var DefaultLanguage = "zh_cn";
            try
            {
                if (System.Web.HttpContext.Current.Session["Language"] != null)
                {
                    //转换为小写
                    DefaultLanguage = System.Web.HttpContext.Current.Session["Language"].ToString().ToLower();
                }
                else
                {
                    //转换为小写
                    DefaultLanguage = "DefaultLanguage".GX().ToLower();

                    //保存到Session中
                    System.Web.HttpContext.Current.Session["Language"] = DefaultLanguage;
                }
            }
            catch
            {
            }
            return DefaultLanguage;
        }
        #endregion


    }
}
