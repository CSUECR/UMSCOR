using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using dotNetRoles = System.Web.Security.Roles;
using System.Web.Routing;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;

using HOHO18.Common;
using MorSun.Model;
using HOHO18.Common.Web;
using MorSun.Bll;

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

        /// <summary>
        /// 检验商家昵称是否已存在  //JS验证的不能增加域判断
        /// </summary>
        /// <param name="sellerNick">昵称</param>
        /// <returns>bool</returns>
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public JsonResult CheckUserName(string UserName)
        {
            bool isValidate = false;
            if (Membership.GetUser(UserName) == null)
            {
                isValidate = true;
            }
            return Json(isValidate, JsonRequestBehavior.AllowGet);

            //返回多个验证错误消息方法
            //if (IsUniqueName(userName) && IsForbiddenName(userName))
            //{
            //    return Json(true, JsonRequestBehavior.AllowGet);
            //}
            //else if (!IsUniqueName(userName))
            //{
            //    return Json("用户名不唯一！", JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    return Json("用户名不包含违禁词！", JsonRequestBehavior.AllowGet);
            //}
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
        
        /// <summary>
        /// 用户锁定验证
        /// </summary>
        /// <param name="model"></param>
        private void validateLockedUser(LoginModel model)
        {
            var user = Membership.GetUser(model.UserName);

            if (user != null)
            {
                if ("UnlockingFlag".GetXmlConfig() == "true")
                {
                    var lockedDate = user.LastLockoutDate;
                    var days = "UnlockingDay".GetXmlConfig();
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
                        "UserName".AE("用户已被锁定，" + days + "天后自动解锁或请联系管理员解锁", ModelState);
                    }
                }

                if (user.IsLockedOut)
                {
                    "UserName".AE("用户已被锁定，请联系管理员解锁", ModelState);
                }
            }
            else if (user == null)
            {
                "UserName".AE("提供的用户名或密码不正确", ModelState);
            }
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
            System.Web.HttpContext.Current.Session.Abandon();
            if (Request.IsAuthenticated)
                FormsService.SignOut();
            var oper = new OperationResult(OperationResultType.Error, "登录失败");
            validateVerifyCode(model.Verifycode, model.VerifycodeRandom, "LoginVerificationCode");
            if (ModelState.IsValid)
            {
                validateLockedUser(model);
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    //封装返回的数据
                    fillOperationResult(returnUrl, oper, "登录成功");                    
                    return Json(oper);
                }
                else
                {
                    "UserName".AE("提供的用户名或密码不正确", ModelState); 
                }
            }
            oper.AppendData = ModelState.GE();
            return Json(oper);
        }

        

        //
        // POST: /Account/LogOff

        
        public ActionResult LogOff()
        {
            FormsService.SignOut();            
            System.Web.HttpContext.Current.Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        //激活账号
        [AllowAnonymous]
        public ActionResult ActiveUser()
        {
            return View();
        }

        //电子邮件修改密码
        [AllowAnonymous]
        public ActionResult ECPW()
        {
            return View();
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register(string id)
        {
            RegisterModel model = new RegisterModel();
            model.BeInviteCode = id;
            return View(model);
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model, string returnUrl)
        {
            var oper = new OperationResult(OperationResultType.Error, "注册失败");
            validateVerifyCode(model.Verifycode, model.VerifycodeRandom, "RegVerificationCode");

            if ("Register".GetXmlConfig() != "true")
            {
                //注册已经关闭，不允许注册
                "UserName".AE("用户注册已经关闭", ModelState);                
            }           

            if (Membership.GetUser(model.UserName) != null)
            {
                //该用户名已经存在，请重新输入！
                "UserName".AE("该用户已存在", ModelState); 
            }

            //电子邮件即用户名
            model.Email = model.UserName;
            if (ModelState.IsValid)
            {
                // 尝试注册用户
                try
                {
                    //WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    //WebSecurity.Login(model.UserName, model.Password);

                    var createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);
                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        //查询出新注册的用户信息
                        var user = Membership.GetUser(model.UserName);
                        var userinfobll = new BaseBll<wmfUserInfo>();
                        wmfUserInfo userinfoModel = new wmfUserInfo();

                        userinfoModel.ID = user.ProviderUserKey.ToAs<Guid>();
                        userinfoModel.UserPassword = model.Password.Encrypt(userinfoModel.ID.ToString());
                        userinfoModel.OperatePassword = model.Password.Encrypt(userinfoModel.ID.ToString());
                        //密码串 不用
                        //userinfoModel.ValidateCode = Guid.NewGuid().ToString().Encrypt(userinfoModel.ID.ToString());
                        userinfoModel.NickName = String.IsNullOrEmpty(model.NickName) ? "DefaultNickName".GetXmlConfig() : model.NickName;
                        //邀请码
                        userinfoModel.InviteCode = Guid.NewGuid().ToString().Encrypt(userinfoModel.ID.ToString());
                        //被邀请码
                        userinfoModel.BeInviteCode = model.BeInviteCode;
                        //被邀请人
                        var inviteUser = userinfobll.All.FirstOrDefault(p => p.InviteCode == model.BeInviteCode);
                        if (inviteUser != null)
                            userinfoModel.InviteUser = inviteUser.ID;
                        //用户串和密码串
                        userinfoModel.UserNameString = Guid.NewGuid().ToString().Encrypt(userinfoModel.ID.ToString());
                        userinfoModel.PassWordString = Guid.NewGuid().ToString().Encrypt(userinfoModel.ID.ToString());

                        userinfoModel.FlagWorker = false;                        
                        userinfoModel.RegTime = DateTime.Now;
                        userinfoModel.FlagTrashed = false;
                        userinfoModel.FlagDeleted = false;
                        if ("AccountActive".GetXmlConfig() == "true")
                        { userinfoModel.FlagActive = false; }
                        else
                        {
                            userinfoModel.FlagActive = true;
                        }

                        //保存用户信息到 wmfuserinfo 表中
                        userinfobll.Insert(userinfoModel);

                        //设置默认角色
                        var RoleName = "RoleName".GetXmlConfig();
                        if (!string.IsNullOrEmpty(RoleName))
                        {
                            //添加角色  被注释的无效
                            //dotNetRoles.AddUserToRole(model.UserName, RoleName);
                            var constr = @"Insert Into aspnet_UsersInRoles ([UserId],[RoleId])  VALUES ('" + userinfoModel.ID + "','" + RoleName + "')";                            
                            userinfobll.Db.ExecuteStoreCommand(constr);
                        }

                        //发送激活邮件
                        if ("AccountActive".GetXmlConfig() == "true")
                        {
                            string fromEmail = "ServiceMail".GetXmlConfig();                            
                            string fromEmailPassword = "ServiceMailPassword".GetXmlConfig().Decrypt();

                            string body = new WebClient().GetHtml("ServiceDomain".GetXmlConfig() + "/Home/ActiveAccountEmail").Replace("[==NickName==]", userinfoModel.NickName).Replace("[==UserCode==]", userinfoModel.UserNameString);
                            //创建邮件对象并发送
                            var mail = new SendMail(model.Email, fromEmail, body, "激活账号", fromEmailPassword, "ServiceMailName".GetXmlConfig(), userinfoModel.NickName);
                            mail.Send("smtp.",587);
                        }
                        //激活后才能登录
                        //FormsService.SignIn(model.UserName, false);

                        //封装返回的数据
                        fillOperationResult(returnUrl, oper, "注册成功");
                        return Json(oper);
                    }
                }
                catch (MembershipCreateUserException e)
                {
                    "".AE(ErrorCodeToString(e.StatusCode),ModelState);
                }
            }

            oper.AppendData = ModelState.GE();
            return Json(oper);
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
