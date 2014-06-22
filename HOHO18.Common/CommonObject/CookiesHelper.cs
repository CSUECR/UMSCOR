using System;
using System.Configuration;
using System.Text;
using System.Data;
using System.Web;

namespace HOHO18.Common
{
    /// <summary>
    /// 读取Cookies值
    /// </summary>
    public class CookiesHelper
    {

        #region
        /// <summary>
        /// 获取主题
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetThemsStyleCookies_Skincss(System.Web.HttpRequest request, string styleName)
        {
            //转换成小写
            styleName = styleName.ToLower();
            string themsStyle = "default";
            if (request.Cookies["ThemesSytle"] != null)
            {
                themsStyle = request.Cookies["ThemesSytle"].Value.ToString().Trim();
            }
            switch (styleName)
            {
                case "skin":
                    return "<link rel=\"stylesheet\" href=\"/Content/themes/" + themsStyle + "/skin.min.css\" type=\"text/css\"/>";
                case "morsuncommon":
                    return "<link rel=\"stylesheet\" href=\"/Content/themes/" + themsStyle + "/MorSun.Common.min.css\" type=\"text/css\"/>";
                case "jquerytreetable":
                    return "<link rel=\"stylesheet\" href=\"/Content/themes/" + themsStyle + "/treetable/jquery.treeTable.min.css\" type=\"text/css\"/>";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 获取是否为单窗口还是多窗口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool GetThemsWindowCookies(System.Web.HttpRequest request)
        {
            bool themsWindow = true;
            if (request.Cookies["ThemesWindow"] != null)
            {
                bool.TryParse(request.Cookies["ThemesWindow"].Value.ToString().Trim(), out themsWindow);
            }
            return themsWindow;
        }


        #endregion


        /// <summary>
        /// Cookies赋值
        /// </summary>
        /// <param name="strName">主键</param>
        /// <param name="strValue">键值</param>
        /// <param name="strDay">有效天数</param>
        /// <returns></returns>
        public static bool setCookie(string strName, string strValue, int strDay)
        {
            try
            {
                HttpCookie Cookie = new HttpCookie(strName);
                //Cookie.Domain = ".xxx.com";//当要跨域名访问的时候,给cookie指定域名即可,格式为.xxx.com
                Cookie.Expires = DateTime.Now.AddDays(strDay);
                Cookie.Value = strValue;
                System.Web.HttpContext.Current.Response.Cookies.Add(Cookie);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 读取Cookies
        /// </summary>
        /// <param name="strName">主键</param>
        /// <returns></returns>

        public static string getCookie(string strName)
        {
            HttpCookie Cookie = System.Web.HttpContext.Current.Request.Cookies[strName];
            if (Cookie != null)
            {
                return Cookie.Value.ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 删除Cookies
        /// </summary>
        /// <param name="strName">主键</param>
        /// <returns></returns>
        public static bool delCookie(string strName)
        {
            try
            {
                HttpCookie Cookie = new HttpCookie(strName);
                //Cookie.Domain = ".xxx.com";//当要跨域名访问的时候,给cookie指定域名即可,格式为.xxx.com
                Cookie.Expires = DateTime.Now.AddDays(-1);
                System.Web.HttpContext.Current.Response.Cookies.Add(Cookie);
                return true;
            }
            catch
            {
                return false;
            }
        }



    }
}
