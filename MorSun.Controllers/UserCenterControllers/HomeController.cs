using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MorSun.Bll;
using MorSun.Model;
using System.Web.Routing;


namespace MorSun.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string bic, string returnUrl)
        {
            HttpCookie Cookie_login = Request.Cookies["BIC"];
            if (Cookie_login != null && !String.IsNullOrEmpty(Cookie_login["BIC"].ToString()))
            {
                if(String.IsNullOrEmpty(bic))
                    bic = Cookie_login["BIC"].ToString();
                else if(!Cookie_login["BIC"].ToString().Eql(bic))
                {
                    Cookie_login = new HttpCookie("BIC");
                    Cookie_login["BIC"] = bic;
                }
            }
            else if(!String.IsNullOrEmpty(bic))
            {
                Cookie_login = new HttpCookie("BIC");
                Cookie_login["BIC"] = bic;                
            }

            //如果用户已经登录，则设置cookei为当前用户，防止用户未退出但被人推广
            if(User != null && User.Identity.IsAuthenticated)
            {
                var uinfo = MorSun.Controllers.BasisController.CurrentAspNetUser.wmfUserInfo;
                if(!Cookie_login["BIC"].ToString().Eql(uinfo.HamInviteCode))
                {
                    Cookie_login["BIC"] = uinfo.HamInviteCode;
                }
            }

            //对修改 及 新创建的cookie进行重新管理
            Cookie_login.Path = "/";
            Cookie_login.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(Cookie_login);

            ViewBag.Title = "悟空打码";
            if (!String.IsNullOrEmpty(returnUrl) && !returnUrl.Substring(0,1).Eql("/"))
                return Redirect(returnUrl);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult ActiveAccountEmail()
        {
            return View();
        }

        public ActionResult AccountChangePassword()
        {
            return View();
        }

        public ActionResult EL(string id)
        {
            var bll = new BaseBll<wmfEncryptRecord>();
            var effectiveHour = 0 - "EncryptTime".GX().ToAs<int>();
            var timeBefore = DateTime.Now.AddHours(effectiveHour);
            var model = bll.All.Where(p => p.EncryptCode == id && p.EncryptTime >= timeBefore).OrderByDescending(p => p.RegTime).FirstOrDefault();
            //var au = "ActiveUserUrl".GX();
            if(model != null)
            { 
                switch (model.EncryptUrl.ToLower())
                {
                    case "/account/activeuser": return activeUser(model.UserNameString);
                    case "/account/ecpw": return RedirectToAction("ECPW", "Account", new { id = id});
                    default: return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");            
        }
        public IFormsAuthenticationService FormsService { get; set; }
        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }   
            base.Initialize(requestContext);
        }
        private ActionResult activeUser(string userNameString)
        {
            var bll = new BaseBll<wmfUserInfo>();
            var user = bll.All.Where(p => p.UserNameString == userNameString).FirstOrDefault();
            if(user != null && user.FlagActive == false)
            {
                user.FlagActive = true;
                bll.Update(user);                
                FormsService.SignIn(user.aspnet_Users.UserName,false);
                LoginFunction(user.aspnet_Users); 
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 登录后的通用设置方法
        /// </summary>
        /// <param name="user"></param>
        private void LoginFunction(aspnet_Users user)
        {
            if (user.wmfUserInfo != null && !String.IsNullOrEmpty(user.wmfUserInfo.HamInviteCode))
            {
                //用户登录都更换推广码,否则用之前的推广码。
                HttpCookie Cookie_login = Request.Cookies["BIC"];
                Cookie_login = new HttpCookie("BIC");
                Cookie_login["BIC"] = user.wmfUserInfo.HamInviteCode;
                //对修改 及 新创建的cookie进行重新管理
                Cookie_login.Path = "/";
                Cookie_login.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Add(Cookie_login);
            }
        } 
    }
}
