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
using MorSun.Common.类别;
using HOHO18.Common.WEB;
using HOHO18.Common.SSO;
using MorSun.Common.配置;

namespace MorSun.Controllers
{
    [Authorize]
    [HandleError]
    //[InitializeSimpleMembership]
    public class AccountController : BasisController
    {
        //图片获取测试 已经测试成功，留着，以后用。
        //[AllowAnonymous]
        //public void GetImg()
        //{
        //    //图片已经可以拿下来，文件格式什么的无所谓，跟MediaId关联即可。
        //    string url = "http://mmbiz.qpic.cn/mmbiz/fmwArEibATSJrvPO8NjAdE11a8hoXP94sTNGazUSZkMvA6JfnDclOFVQicrGibnnicpuYrl5r8vu8UicibyTRRicJ7Srw/0";
        //    string filepath = "c:\\pic123421";
        //    WebClient mywebclient = new WebClient();
        //    mywebclient.DownloadFile(url, filepath);           
        //}

        #region 基本方法
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        /// <summary>
        /// 检验用户是否已存在，注册时用，存在则出错
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <returns></returns>
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

        /// <summary>
        /// 检查用户是否存在 不存在出错
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public JsonResult CheckUserNameTrue(string UserName)
        {
            bool isValidate = false;
            if (Membership.GetUser(UserName) != null)
            {
                isValidate = true;
            }
            return Json(isValidate, JsonRequestBehavior.AllowGet);
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
                if ("UnlockingFlag".GX() == "true")
                {
                    var lockedDate = user.LastLockoutDate;
                    var days = "UnlockingDay".GX();
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
        #endregion

        #region 登录
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.OpenVerificationCode = "VerificationCode".GX().ToAs<bool>();
            return View();
        }

        [AllowAnonymous]
        public ActionResult AjaxLogin(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.OpenVerificationCode = "VerificationCode".GX().ToAs<bool>();
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
            validateLockedUser(model);
            //SSO也要用到，所以从if(AccountActive)里取出来。
            var user = new BaseBll<aspnet_Users>().All.FirstOrDefault(p => p.LoweredUserName == model.UserName.ToLower());
            //判断账号是否激活,时间长未激活的话，系统再发送激活邮件
            if ("AccountActive".GX() == "true")
            {                
                if (user != null && user.wmfUserInfo.FlagActive == false)
                { 
                    //检查该用户的激活邮件发送时间，如果超过48小时，系统自动重新发送一条。
                    var mrbll = new BaseBll<wmfMailRecord>();
                    var effectiveHour = 0 - CFG.有效时间.ToAs<int>();
                    var dt = DateTime.Now.AddHours(effectiveHour);//计算48前的时间
                    var mr = mrbll.All.FirstOrDefault(p => p.MailTo == user.UserName && p.RegTime >= dt);
                    if(mr == null)
                    {
                        SendActiveMail(mrbll,user.aspnet_Membership.Email,user.wmfUserInfo.NickName,user.wmfUserInfo.UserNameString);
                        "UserName".AE("系统已经重新发送了激活邮件，请激活后再登录", ModelState);
                    }
                    else
                    {
                        "UserName".AE("账号未激活,请激活后再登录", ModelState);
                    }
                }
            }
            if (ModelState.IsValid)
            {                
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    if(!model.UserName.Contains("youhong@bungma"))
                        LogHelper.Write(model.UserName + "登录", LogHelper.LogMessageType.Info);
                    LoginFunction(user);        

                    //生成登录子应用链接
                    //var apps = "AppsUrl".GX().Split(',');
                    //string SSOLink = "";
                    //var dt = DateTime.Now;
                    //var dts = dt.ToShortDateString() + " " + dt.ToShortTimeString();
                    //var tok = HttpUtility.UrlEncode(SecurityHelper.Encrypt(dts + ";" + user.UserId + model.UserName));
                    //foreach (var item in apps)
                    //{

                    //    SSOLink += item + SsoConst.AppLoginPageName + "?" +
                    //             SsoConst.SsoTokenName + "=" + tok;//只传ID和用户名到子站吧，其他的子站ajax从主站获取
                    //    SSOLink += ",";
                    //}
                    ////封装返回的数据
                    //fillOperationResult(returnUrl, SSOLink, oper, "登录成功");
                    fillOperationResult(returnUrl, oper, "登录成功");  
                    return Json(oper, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    LogHelper.Write(model.UserName + "被" + Request.UserHostAddress + "恶意登录", LogHelper.LogMessageType.Info);
                    "UserName".AE("提供的用户名或密码不正确", ModelState); 
                }
            }
            oper.AppendData = ModelState.GE();
            return Json(oper, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 发送激活邮件
        /// </summary>
        /// <param name="user"></param>
        /// <param name="mrbll"></param>
        private void SendActiveMail( BaseBll<wmfMailRecord> mrbll,string email,string nickName,string userNameString)
        {
            LogHelper.Write(email + "发送邮件", LogHelper.LogMessageType.Debug);
            string fromEmail = CFG.应用邮箱;
            string fromEmailPassword = CFG.邮箱密码.DP();
            int emailPort = String.IsNullOrEmpty(CFG.邮箱端口) ? 587 : CFG.邮箱端口.ToAs<int>();
            var code = GenerateEncryptCode(userNameString, CFG.账号激活路径, false);
            string body = new WebClient().GetHtml("ServiceDomain".GHU() + "/H/ActiveAccountEmail").Replace("[==NickName==]", nickName).Replace("[==UserCode==]", code);
            //创建邮件对象并发送
            var mail = new SendMail(email, fromEmail, body, "激活账号", fromEmailPassword, "ServiceMailName".GX(), nickName);
            var mailRecord = new wmfMailRecord().wmfMailRecord2(email, body, "激活账号", "ServiceMailName".GX(), nickName, Guid.Parse(Reference.电子邮件类别_账号注册));
            mrbll.Insert(mailRecord);
            mail.Send("smtp.", emailPort, email + "激活账号邮件发送失败！");
        }

        /// <summary>
        /// 登录后的通用设置方法
        /// </summary>
        /// <param name="user"></param>
        private void LoginFunction(aspnet_Users user)
        {
            //用户登录都更换推广码,否则用之前的推广码。
            HttpCookie Cookie_login = Request.Cookies["HIC"];
            Cookie_login = new HttpCookie("HIC");
            if (user != null && user.wmfUserInfo != null && !String.IsNullOrEmpty(user.wmfUserInfo.HamInviteCode))
            {                
                Cookie_login["HIC"] = user.wmfUserInfo.HamInviteCode;                
            }
            else
            {
                Cookie_login["HIC"] = CFG.默认推广代码;
            }
            //对修改 及 新创建的cookie进行重新管理
            Cookie_login.Path = "/";
            Cookie_login.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(Cookie_login);
        } 

        // POST: /Account/LogOff        
        public ActionResult LogOff()
        {
            FormsService.SignOut();            
            System.Web.HttpContext.Current.Session.Abandon();
            return RedirectToAction("I", "H");
        }
         ///<summary>
         ///通行证登录       先不开启使用，有子系统再说
         ///</summary>
         ///<returns></returns>
        //[AllowAnonymous]
        //public String AppLogin()
        //{
        //    string userCode = Request.QueryString[SsoConst.SsoTokenName];
        //    string userName = "";
        //    if (!string.IsNullOrEmpty(userCode))
        //    {
        //        try
        //        {
        //            userCode = SecurityHelper.Decrypt(userCode);
        //            //修改了内容，取用户名要区分开来
        //            //取时间戳
        //            var ind = userCode.IndexOf(';');
        //            DateTime dt = DateTime.Parse(userCode.Substring(0, ind));
        //            var uid = Guid.Parse(userCode.Substring(ind + 1, 36));
        //            userName = userCode.Substring(ind + 1 + 36, userCode.Length - ind - 36 - 1);
        //            //在这个位置增加用户。
        //            var user = new BaseBll<aspnet_Users>().All.Where(p => p.UserName == userName).FirstOrDefault();
        //            if (user != null)
        //                FormsService.SignIn(userName, true);
        //        }
        //        catch (Exception ex)
        //        {
        //            return "";
        //        }
        //    }
        //    return ";";
        //}

        /// <summary>
        /// 通行证退出
        /// </summary>
        /// <returns></returns>
        //[AllowAnonymous]
        //public String AppLogOff()
        //{
        //    FormsService.SignOut();
        //    System.Web.HttpContext.Current.Session.Abandon();
        //    return ";";
        //}
        #endregion

        #region 忘记密码
        [AllowAnonymous]
        public ActionResult ForgetPass(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgetPass(ForgetModel model, string returnUrl)
        {
            var oper = new OperationResult(OperationResultType.Error, "提交失败");
            validateVerifyCode(model.Verifycode, model.VerifycodeRandom, "ForgetPassCode");
            if (ModelState.IsValid)
            {
                if (Membership.GetUser(model.UserName) != null)
                {
                    //封装返回的数据
                    fillOperationResult(Url.Action("ConfirmQuestion", "Account", new { UserName = model.UserName, returnUrl = Url.Action("ForgetPass", "Account") }), oper, "提交成功");
                    return Json(oper, JsonRequestBehavior.AllowGet);
                }
                else
                    "UserName".AE("提供的用户名不正确", ModelState);
            }
            oper.AppendData = ModelState.GE();
            return Json(oper, JsonRequestBehavior.AllowGet);            
        }

        [AllowAnonymous]        
        public ActionResult ConfirmQuestion(ForgetModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            var userInfo = new ConfirmQuestionModel();            
            userInfo.uName = model.UserName;
            return View(userInfo);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmQuestion(ConfirmQuestionModel model, string returnUrl)
        {
            var oper = new OperationResult(OperationResultType.Error, "提交失败");
            //确认信息并发送邮件
            var user = new BaseBll<aspnet_Users>().All.FirstOrDefault(p => p.LoweredUserName == model.uName.ToLower());
            if (user == null || user.wmfUserInfo == null)
                "Question1".AE("用户账号异常", ModelState);
            else
            {
                if (string.IsNullOrEmpty(model.Question1)) model.Question1 = "";
                if (string.IsNullOrEmpty(model.Question2)) model.Question2 = "";
                if (string.IsNullOrEmpty(model.Question3)) model.Question3 = "";
                if (string.IsNullOrEmpty(model.Answer1)) model.Answer1 = "";
                if (string.IsNullOrEmpty(model.Answer2)) model.Answer2 = "";
                if (string.IsNullOrEmpty(model.Answer3)) model.Answer3 = "";

                //不能直接修改数据库对象，后面的代码有保存动作。
                var model2 = new ConfirmQuestionModel();
                model2.Question1 = user.wmfUserInfo.Question1;
                model2.Question2 = user.wmfUserInfo.Question2;
                model2.Question3 = user.wmfUserInfo.Question3;
                model2.Answer1 = user.wmfUserInfo.Answer1;
                model2.Answer2 = user.wmfUserInfo.Answer2;
                model2.Answer3 = user.wmfUserInfo.Answer3;

                if (string.IsNullOrEmpty(model2.Question1)) model2.Question1 = "".EP(user.UserId.ToString());
                if (string.IsNullOrEmpty(model2.Question2)) model2.Question2 = "".EP(user.UserId.ToString());
                if (string.IsNullOrEmpty(model2.Question3)) model2.Question3 = "".EP(user.UserId.ToString());
                if (string.IsNullOrEmpty(model2.Answer1)) model2.Answer1 = "".EP(user.UserId.ToString());
                if (string.IsNullOrEmpty(model2.Answer2)) model2.Answer2 = "".EP(user.UserId.ToString());
                if (string.IsNullOrEmpty(model2.Answer3)) model2.Answer3 = "".EP(user.UserId.ToString());
                if (!model.Question1.EP(user.UserId.ToString()).Eql(model2.Question1)
                    || !model.Answer1.EP(user.UserId.ToString()).Eql(model2.Answer1)
                    || !model.Question2.EP(user.UserId.ToString()).Eql(model2.Question2)
                    || !model.Answer2.EP(user.UserId.ToString()).Eql(model2.Answer2)
                    || !model.Question3.EP(user.UserId.ToString()).Eql(model2.Question3)
                    || !model.Answer3.EP(user.UserId.ToString()).Eql(model2.Answer3)
                    )
                    "Question1".AE("验证失败", ModelState);

                //多次提交判断                
                var effectiveHour = 0 - CFG.有效时间.ToAs<int>();
                var timeBefore = DateTime.Now.AddHours(effectiveHour);
                var uns = user.wmfUserInfo.UserNameString;
                var erCount = new BaseBll<wmfEncryptRecord>().All.Where(p => p.UserNameString == uns && p.EncryptTime >= timeBefore && p.EncryptUrl.ToLower() == "/account/ecpw").Count();
                if(erCount >= 5)
                    "Question1".AE(CFG.有效时间 + "小时内最多找回密码5次", ModelState);

                if(ModelState.IsValid)                
                {
                    //发送邮件并转发
                    string fromEmail = CFG.应用邮箱;
                    string fromEmailPassword = CFG.邮箱密码.DP();
                    int emailPort = String.IsNullOrEmpty(CFG.邮箱端口) ? 587 : CFG.邮箱端口.ToAs<int>();
                    var code = GenerateEncryptCode(user.wmfUserInfo.UserNameString, CFG.邮件改密路径, false);
                    string body = new WebClient().GetHtml("ServiceDomain".GHU() + "/H/AccountChangePassword").Replace("[==NickName==]", user.wmfUserInfo.NickName).Replace("[==UserCode==]", code);
                    //创建邮件对象并发送
                    var mail = new SendMail(user.UserName, fromEmail, body, "找回密码", fromEmailPassword, "ServiceMailName".GX(), user.wmfUserInfo.NickName);
                    var mailRecord = new wmfMailRecord().wmfMailRecord2(user.UserName, body, "找回密码", "ServiceMailName".GX(), user.wmfUserInfo.NickName, Guid.Parse(Reference.电子邮件类别_找回密码));
                    new BaseBll<wmfMailRecord>().Insert(mailRecord);
                    mail.Send("smtp.", emailPort, user.UserName + "找回密码邮件发送失败！");

                    //转发
                    fillOperationResult(Url.Action("SendPassEmail", "Account", new { returnUrl = Url.Action("ConfirmQuestion", "Account", new { UserName = user.UserName}) }), oper, "找回密码邮件发送成功");
                    return Json(oper, JsonRequestBehavior.AllowGet);
                }
            }
            oper.AppendData = ModelState.GE();
            return Json(oper, JsonRequestBehavior.AllowGet);      
        }

        [AllowAnonymous]
        public ActionResult SendPassEmail(ForgetModel model, string returnUrl)
        {            
            return View(model);
        }

        //电子邮件修改密码        
        [AllowAnonymous]
        public ActionResult ECPW(string id, string returnUrl)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("I", "H");
            var model = new ECPWModel();
            model.id = id;
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ECPW(ECPWModel model, string returnUrl)
        {
            var oper = new OperationResult(OperationResultType.Error, "提交失败");            
            if (string.IsNullOrEmpty(model.id))
                "OldPassword".AE("非法提交", ModelState);
            else
            {
                var bll = new BaseBll<wmfEncryptRecord>();
                var effectiveHour = 0 - CFG.有效时间.ToAs<int>();
                var timeBefore = DateTime.Now.AddHours(effectiveHour);
                var er = bll.All.Where(p => p.EncryptCode == model.id && p.EncryptTime >= timeBefore).OrderByDescending(p => p.RegTime).FirstOrDefault();
                if(er == null)
                    "OldPassword".AE("非法提交", ModelState);
                else
                { 
                    var ubll = new BaseBll<wmfUserInfo>();
                    var user = ubll.All.Where(p => p.UserNameString == er.UserNameString).FirstOrDefault();
                    if(user == null)
                        "OldPassword".AE("非法提交", ModelState);
                    else
                    {
                        //忘记密码是不知道以前的密码才用邮件修改的，下面的多此一举
                        //if(!MembershipService.ValidateUser(user.aspnet_Users.UserName, model.OldPassword))
                        //{
                        //    "OldPassword".AE("旧密码输入错误", ModelState);
                        //}
                        if(ModelState.IsValid)
                        {
                            MembershipUser muser = Membership.GetUser(user.aspnet_Users.UserName);
                            if (MembershipService.ChangePassword(user.aspnet_Users.UserName, muser.ResetPassword(), model.NewPassword))
                            {
                                //user.UserPassword = "";// model.NewPassword.EP(user.ID.ToString());
                                //ubll.Update(user);
                                //封装返回的数据
                                fillOperationResult(Url.Action("I", "H"), oper, "密码修改成功");
                                return Json(oper, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
            }
            var ers = ModelState.GE();
            var eper = ers.Where(p => p.Key == "").FirstOrDefault().ErrorMessages.Join();
            if (!String.IsNullOrEmpty(eper))
                oper = new OperationResult(OperationResultType.Error, eper);
            oper.AppendData = ers;
            return Json(oper, JsonRequestBehavior.AllowGet);
        }
        
        #endregion

        #region 注册
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register(string id)
        {
            if (!String.IsNullOrEmpty(id))
                id = id.ToLower().Trim();
            HttpCookie Cookie_login = Request.Cookies["HIC"];
            if (Cookie_login != null && !String.IsNullOrEmpty(Cookie_login["HIC"].ToString()))
            {
                if(String.IsNullOrEmpty(id))
                    id = Cookie_login["HIC"].ToString();
                else if(!Cookie_login["HIC"].ToString().Eql(id))
                {
                    Cookie_login = new HttpCookie("HIC");
                    Cookie_login["HIC"] = id;
                }
            }
            else if (!String.IsNullOrEmpty(id))
            {
                Cookie_login = new HttpCookie("HIC");
                Cookie_login["HIC"] = id;                
            }//为空时直接注册会有问题
            else if (Cookie_login == null)
            {
                //无值时设置为bungma
                Cookie_login = new HttpCookie("HIC");
                Cookie_login["HIC"] = CFG.默认推广代码;
            }
            //对修改 及 新创建的cookie进行重新管理
            Cookie_login.Path = "/";
            Cookie_login.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(Cookie_login);

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

            if ("Register".GX() != "true")
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
                    if (model.Uid == null)
                        model.Uid = Guid.NewGuid();
                    var createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email, model.Uid);
                    LogHelper.Write(model.UserName + "Membership添加用户成功", LogHelper.LogMessageType.Debug);
                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        LogHelper.Write(model.UserName + "准备添加UserInfo", LogHelper.LogMessageType.Debug);
                        //查询出新注册的用户信息
                        var user = Membership.GetUser(model.UserName);
                        var userinfobll = new BaseBll<wmfUserInfo>();
                        wmfUserInfo userinfoModel = new wmfUserInfo();

                        userinfoModel.ID = user.ProviderUserKey.ToAs<Guid>();
                        userinfoModel.UserPassword = ""; //model.Password.EP(userinfoModel.ID.ToString());
                        userinfoModel.OperatePassword = "";// model.Password.EP(userinfoModel.ID.ToString());
                        LogHelper.Write(model.UserName + "用户密码和操作密码加密", LogHelper.LogMessageType.Debug);
                        //密码串 不用
                        //userinfoModel.ValidateCode = Guid.NewGuid().ToString().EP(userinfoModel.ID.ToString());
                        userinfoModel.NickName = String.IsNullOrEmpty(model.NickName) ? "DefaultNickName".GX() : model.NickName;
                        //邀请码
                        userinfoModel.InviteCode = Guid.NewGuid().ToString().EP(userinfoModel.ID.ToString());
                        //邀请码明码
                        var hbc = model.UserName.Substring(model.UserName.LastIndexOf("."), model.UserName.Length - model.UserName.LastIndexOf(".")).Replace(".", "|");
                        userinfoModel.HamInviteCode = (model.UserName.Substring(0, model.UserName.LastIndexOf(".")) + hbc).ToLower().Trim();
                        //被邀请码
                        userinfoModel.BeInviteCode = model.BeInviteCode.ToLower().Trim();
                        //被邀请人
                        if (!string.IsNullOrEmpty(model.BeInviteCode))
                        {
                            //去掉复杂版推广码，一般没人用。
                            //var inviteUser = userinfobll.All.FirstOrDefault(p => p.InviteCode == model.BeInviteCode);
                            //if (inviteUser != null)
                            //    userinfoModel.InviteUser = inviteUser.ID;
                            //else
                            //{
                            var inviteUser = userinfobll.All.Where(p => p.HamInviteCode == model.BeInviteCode).FirstOrDefault();
                            LogHelper.Write(model.UserName + "取邀请人" + model.BeInviteCode, LogHelper.LogMessageType.Debug);
                            //如果邀请人为空，从cookie里取邀请人
                            if(inviteUser == null)
                            {
                                HttpCookie Cookie_login = Request.Cookies["HIC"];//创建Cookie
                                if (Cookie_login != null)
                                {
                                    var bic = Cookie_login["HIC"].ToString().ToLower().Trim();
                                    LogHelper.Write("bic " + bic, LogHelper.LogMessageType.Debug);
                                    inviteUser = userinfobll.All.FirstOrDefault(p => p.HamInviteCode == bic);
                                }
                            }                            
                            if (inviteUser != null)
                                userinfoModel.InviteUser = inviteUser.ID;
                            else
                            {
                                if (model.BeInviteCode.Contains("|"))
                                {
                                    var bc = model.BeInviteCode.Substring(model.BeInviteCode.LastIndexOf("|"), model.BeInviteCode.Length - model.BeInviteCode.LastIndexOf("|")).Replace("|", ".");
                                    model.BeInviteCode = (model.BeInviteCode.Substring(0, model.BeInviteCode.LastIndexOf("|")) + bc).ToLower().Trim();
                                    LogHelper.Write(model.UserName + model.BeInviteCode, LogHelper.LogMessageType.Debug);
                                    var aspnetUser = Membership.GetUser(model.BeInviteCode);
                                    if (aspnetUser != null)
                                        userinfoModel.InviteUser = aspnetUser.ProviderUserKey.ToAs<Guid>();
                                }
                            }
                            //}
                        }
                        //用户串和密码串
                        userinfoModel.UserNameString = Guid.NewGuid().ToString().EP(userinfoModel.ID.ToString());
                        userinfoModel.PassWordString = Guid.NewGuid().ToString().EP(userinfoModel.ID.ToString());

                        userinfoModel.FlagWorker = false;                        
                        userinfoModel.RegTime = DateTime.Now;
                        userinfoModel.FlagTrashed = false;
                        userinfoModel.FlagDeleted = false;
                        if ("AccountActive".GX() == "true")
                        { userinfoModel.FlagActive = false; }
                        else
                        {
                            userinfoModel.FlagActive = true;
                        }
                        //保存用户信息到 wmfuserinfo 表中
                        userinfobll.Insert(userinfoModel);
                        //设置默认角色
                        var RoleName = CFG.注册默认角色;//"RoleName".GX();
                        if (!string.IsNullOrEmpty(RoleName))
                        {
                            //添加角色  被注释的无效
                            //dotNetRoles.AddUserToRole(model.UserName, RoleName);
                            var constr = @"Insert Into aspnet_UsersInRoles ([UserId],[RoleId])  VALUES ('" + userinfoModel.ID + "','" + RoleName + "')";                            
                            userinfobll.Db.ExecuteStoreCommand(constr);
                            LogHelper.Write(model.UserName + "用户基本信息和角色添加成功", LogHelper.LogMessageType.Debug);
                        }
                        //发送激活邮件
                        if ("AccountActive".GX() == "true")
                        {
                            LogHelper.Write(model.UserName + "准备发送邮件", LogHelper.LogMessageType.Debug);
                            SendActiveMail(new BaseBll<wmfMailRecord>(), model.Email, userinfoModel.NickName, userinfoModel.UserNameString);
                        } 
                        else
                        { 
                            //激活后才能登录
                            FormsService.SignIn(model.UserName, false);
                        }  
                        //封装返回的数据
                        var rs = "注册成功";
                        if ("AccountActive".GX() == "true")
                        {
                            rs += ",请登录您的邮箱激活账号";
                        }
                        fillOperationResult(returnUrl, oper, rs);
                        return Json(oper, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (MembershipCreateUserException e)
                {
                    "".AE(ErrorCodeToString(e.StatusCode),ModelState);
                }
            }

            var er = ModelState.GE();
            var eper = er.Where(p => p.Key == "").FirstOrDefault().ErrorMessages.Join();
            if (!String.IsNullOrEmpty(eper))
                oper = new OperationResult(OperationResultType.Error, eper);
            oper.AppendData = er;
            return Json(oper, JsonRequestBehavior.AllowGet);
        }

        #endregion

        /// <summary>
        /// 统计用户马币
        /// </summary>
        /// <returns></returns>
        public string UPMaBi()
        {
            SettleMaBi();
            return "true";
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
                return RedirectToAction("I", "H");
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
