using System;
using System.Web;
using System.Web.Caching;

namespace HOHO18.Common.SSO
{    public static class SsoConst
    {
        public const string AppLoginPageName = "/Account/Login";
        public const string AppLogoutPageName = "/Account/LogOff";
        public const string SsoTokenName = "Token";
        public const string ReturnUrlParameterName = "ReturnUrl";
        public const string TokenCookieName = "SsoToken";
        public const string SsoLoginPageName = "/Account/Login";
        public const string SsoLogoutPageName = "/Account/LogOff";
    }
}
