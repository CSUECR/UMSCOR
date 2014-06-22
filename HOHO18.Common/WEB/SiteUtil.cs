using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace HOHO18.Common.Web
{
    public class SiteUtil
    {        
        public static string GetSiteUrl()
        {
            string path = HttpContext.Current.Request.ApplicationPath;
            if (path.EndsWith("/") && path.Length == 1)
            {
                return GetHostUrl();
            }
            else
            {
                return GetHostUrl() + path;
            }
        }

        public static string GetHostUrl()
        {
            return string.Format("{0}://{1}:{2}",
                HttpContext.Current.Request.Url.Scheme,
                HttpContext.Current.Request.Url.Host,
                HttpContext.Current.Request.Url.Port);
        }
    }
}
