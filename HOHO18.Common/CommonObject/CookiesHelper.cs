using System;
using System.Configuration;
using System.Text;
using System.Data;
using System.Web;

namespace HOHO18.Common
{
    /// <summary>
    /// ��ȡCookiesֵ
    /// </summary>
    public class CookiesHelper
    {

        #region
        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetThemsStyleCookies_Skincss(System.Web.HttpRequest request, string styleName)
        {
            //ת����Сд
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
        /// ��ȡ�Ƿ�Ϊ�����ڻ��Ƕര��
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
        /// Cookies��ֵ
        /// </summary>
        /// <param name="strName">����</param>
        /// <param name="strValue">��ֵ</param>
        /// <param name="strDay">��Ч����</param>
        /// <returns></returns>
        public static bool setCookie(string strName, string strValue, int strDay)
        {
            try
            {
                HttpCookie Cookie = new HttpCookie(strName);
                //Cookie.Domain = ".xxx.com";//��Ҫ���������ʵ�ʱ��,��cookieָ����������,��ʽΪ.xxx.com
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
        /// ��ȡCookies
        /// </summary>
        /// <param name="strName">����</param>
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
        /// ɾ��Cookies
        /// </summary>
        /// <param name="strName">����</param>
        /// <returns></returns>
        public static bool delCookie(string strName)
        {
            try
            {
                HttpCookie Cookie = new HttpCookie(strName);
                //Cookie.Domain = ".xxx.com";//��Ҫ���������ʵ�ʱ��,��cookieָ����������,��ʽΪ.xxx.com
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
