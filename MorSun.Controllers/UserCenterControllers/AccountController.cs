using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

using System.Web.Routing;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;

using HOHO18.Common;
using MorSun.Model;

namespace MorSun.Controllers
{
    [Authorize]
    [HandleError]
    //[InitializeSimpleMembership]
    public class AccountController : BasisController
    {
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.OpenVerificationCode = "VerificationCode".GetXmlConfig().ToAs<bool>();
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            System.Web.HttpContext.Current.Session.Abandon();
            if (Request.IsAuthenticated)
                FormsService.SignOut();            

            if (ModelState.IsValid)
            {
                var user = Membership.GetUser(model.UserName);

                if (user != null)
                {
                    if (HOHO18.Common.Web.XmlConfigHelp.GetWebConfigValue("UnlockingFlag") == "true")
                    {
                        var lockedDate = user.LastLockoutDate;
                        var days = HOHO18.Common.Web.XmlConfigHelp.GetWebConfigValue("UnlockingDay");
                        if (NumHelp.IsNum(days))
                        {
                            lockedDate = lockedDate.AddDays(double.Parse(days));
                        }
                        if (DateTime.Now > lockedDate && user.IsLockedOut)
                        {
                            user.UnlockUser();
                        }
                        else if (DateTime.Now <= lockedDate && user.IsLockedOut)
                        {                            
                            "UserName".AE(ModelState, "用户已被锁定，" + days + "天后自动解锁或者联系管理员");
                        }
                    }

                    if (user.IsLockedOut)
                    {                        
                        "UserName".AE(ModelState, "用户已被锁定，请联系管理员解锁");
                    }
                }
                else if (user == null)
                {                    
                    "UserName".AE(ModelState, "用户名不存在");
                }
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {                    
                    "UserName".AE(ModelState, "提供的用户名或密码不正确");
                }
                //return RedirectToLocal(returnUrl);
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单           
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult AjaxLogin(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.OpenVerificationCode = "VerificationCode".GetXmlConfig().ToAs<bool>();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxLogin(LoginModel model, string returnUrl)
        {
            var oper = new OperationResult(OperationResultType.Error, "登录失败");
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    //封装返回的数据
                    fillOperationResult(returnUrl, oper, "登录成功");
                    return Json(oper);
                }
                else
                {                    
                    "UserName".AE(ModelState, "提供的用户名或密码不正确");
                }
            }
            oper.AppendData = ModelState.GE();
            return Json(oper);
        }

        

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsService.SignOut();
            //clearSession();
            System.Web.HttpContext.Current.Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // 尝试注册用户
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        

        #region 帮助程序
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // 请参见 http://go.microsoft.com/fwlink/?LinkID=177550 以查看
            // 状态代码的完整列表。
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "用户名已存在。请输入其他用户名。";

                case MembershipCreateStatus.DuplicateEmail:
                    return "该电子邮件地址的用户名已存在。请输入其他电子邮件地址。";

                case MembershipCreateStatus.InvalidPassword:
                    return "提供的密码无效。请输入有效的密码值。";

                case MembershipCreateStatus.InvalidEmail:
                    return "提供的电子邮件地址无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidAnswer:
                    return "提供的密码取回答案无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidQuestion:
                    return "提供的密码取回问题无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidUserName:
                    return "提供的用户名无效。请检查该值并重试。";

                case MembershipCreateStatus.ProviderError:
                    return "身份验证提供程序返回了错误。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";

                case MembershipCreateStatus.UserRejected:
                    return "已取消用户创建请求。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";

                default:
                    return "发生未知错误。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";
            }
        }
        #endregion
    }
}
