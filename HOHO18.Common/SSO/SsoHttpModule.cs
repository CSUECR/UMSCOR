using System;
using System.Web;
using System.Web.Security;

namespace HOHO18.Common.SSO
{
    public class SsoHttpModule : IHttpModule
    {
        public void Dispose()
        {
            //to do list...
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += BeginRequest;
            context.PostAuthenticateRequest += PostAuthenticateRequest;
        }

        void BeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpContext ctx = app.Context;
            var requestUrl = ctx.Request.Url.ToString().Trim().ToLower();

            if (requestUrl.Contains(SsoConst.AppLoginPageName.ToLower()))
            {
                ctx.Response.ContentType = "text/plain";
                ctx.Response.Write(";");

                string userName = ctx.Request.QueryString[SsoConst.SsoTokenName];
                if (!string.IsNullOrEmpty(userName))
                {
                    userName = SecurityHelper.Decrypt(userName);
                    //在这个位置增加用户。
                    FormsAuthentication.SetAuthCookie(userName, true);//登录子应用
                }

                ctx.Response.End();
            }

            if (requestUrl.Contains(SsoConst.AppLogoutPageName.ToLower()))
            {
                ctx.Response.ContentType = "text/plain";
                ctx.Response.Write(";");

                FormsAuthentication.SignOut();//退出子应用

                ctx.Response.End();
            }
        }

        private void PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpContext ctx = app.Context;
            var requestUrl = ctx.Request.Url.ToString().Trim().ToLower();

            //登录和退出页 不拦截
            if (requestUrl.Contains(SsoConst.AppLogoutPageName.ToLower()) || requestUrl.Contains(SsoConst.AppLoginPageName.ToLower()))
            {
                return;
            }

            if (ctx.User != null && !ctx.User.Identity.IsAuthenticated)
            {
                //未登录，跳转到sso login页面               
                ctx.Response.RedirectPermanent("SSOUrl".GX() + SsoConst.SsoLoginPageName + "?" + SsoConst.ReturnUrlParameterName + "=" + HttpUtility.UrlEncode(ctx.Request.Url.ToString()));
            }

        }
    }
}
