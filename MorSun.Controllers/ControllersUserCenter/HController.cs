using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MorSun.Bll;
using MorSun.Model;
using System.Web.Routing;
using MorSun.Common.类别;
using MorSun.Common.配置;
using HOHO18.Common.SSO;
using HOHO18.Common.WEB;


namespace MorSun.Controllers
{
    public class HController : Controller
    {
        public ActionResult I(string id, string returnUrl)
        {
            if (!String.IsNullOrEmpty(id))
                id = id.ToLower().Trim();
            LogHelper.Write("跳转URL" + returnUrl, LogHelper.LogMessageType.Debug);
            var refuseUrl = "RefuseUrl".GX().Split(',');
            LogHelper.Write("ServiceDomain".GHU(), LogHelper.LogMessageType.Info);
            if (refuseUrl.Contains("ServiceDomain".GHU()))
                return Redirect("http://" + "ServiceDomain".GX());
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
            else if(!String.IsNullOrEmpty(id))
            {
                Cookie_login = new HttpCookie("HIC");
                Cookie_login["HIC"] = id;                
            }
            else if(Cookie_login == null)
            {
                //无值时设置为bungma
                Cookie_login = new HttpCookie("HIC");
                Cookie_login["HIC"] = CFG.默认推广代码;
            }

            //如果用户已经登录，则设置cookei为当前用户，防止用户未退出但被人推广
            if(User != null && User.Identity.IsAuthenticated)
            {
                var uinfo = MorSun.Controllers.BasisController.CurrentAspNetUser.wmfUserInfo;
                if(!Cookie_login["HIC"].ToString().Eql(uinfo.HamInviteCode))
                {
                    Cookie_login["HIC"] = uinfo.HamInviteCode;
                }
            }

            //对修改 及 新创建的cookie进行重新管理
            Cookie_login.Path = "/";
            Cookie_login.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(Cookie_login);
            ViewBag.Title = "ServiceName".GX();
            if (!String.IsNullOrEmpty(returnUrl) && returnUrl.StartsWith("ServiceDomain".GHU()))
                return Redirect(returnUrl);
            var indexModel = new IndexModel();
            var newtz = Guid.Parse(Reference.新闻类别_通知);
            var inc = String.IsNullOrEmpty("IndexNoticeCount".GX()) ? "5" : "IndexNoticeCount".GX();
            int takeCount = Convert.ToInt32(inc);
            if (takeCount < 5) takeCount = 5;
            indexModel.nList = new BaseBll<bmNew>().All.Where(p => p.NewRef == newtz).OrderBy(p => p.Sort).Take(takeCount);
            return View(indexModel);
        }

        /// <summary>
        /// 根据传入的用户加密微信ID，登录用户并且生成分享链接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult WXShareLink(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                var DCWeiXinId = SecurityHelper.Decrypt(id);
                var BMU = GetUserByWeiXinId(DCWeiXinId);
                if(BMU != null)
                {                    
                    LoginFunction(BMU.aspnet_Users1);
                }    
                else
                {
                    LoginFunction(null);
                }
            }            
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

        /// <summary>
        /// 邮件通往入口
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EL(string id)
        {
            var bll = new BaseBll<wmfEncryptRecord>();
            var effectiveHour = 0 - CFG.有效时间.ToAs<int>();
            var timeBefore = DateTime.Now.AddHours(effectiveHour);
            var model = bll.All.Where(p => p.EncryptCode == id && p.EncryptTime >= timeBefore).OrderByDescending(p => p.RegTime).FirstOrDefault();
            //var au = CFG.账号激活路径;
            //用户激活不能限制时间
            if(model == null)
            {
                var actModel = bll.All.Where(p => p.EncryptCode == id && p.EncryptUrl.ToLower() == "/account/activeuser").OrderByDescending(p => p.RegTime).FirstOrDefault();
                if (actModel != null)
                    model = actModel;
            }            
            if(model != null)
            { 
                switch (model.EncryptUrl.ToLower())
                {
                    case "/account/activeuser": return activeUser(model.UserNameString);
                    case "/account/ecpw": return RedirectToAction("ECPW", "Account", new { id = id});
                    default: return RedirectToAction("I", "H");
                }
            }
            return RedirectToAction("I", "H");            
        }
        public IFormsAuthenticationService FormsService { get; set; }
        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }   
            base.Initialize(requestContext);
        }

        /// <summary>
        /// 激活用户接口
        /// </summary>
        /// <param name="userNameString"></param>
        /// <returns></returns>
        private ActionResult activeUser(string userNameString)
        {
            var bll = new BaseBll<wmfUserInfo>();
            var user = bll.All.Where(p => p.UserNameString == userNameString).FirstOrDefault();
            if(user != null && user.FlagActive == false)
            {
                FormsService.SignIn(user.aspnet_Users.UserName, false);
                LoginFunction(user.aspnet_Users); 
                //更新用户前就登录
                user.FlagActive = true;
                //激活后，互相赠送马币
                var addMBR = new AddMBRModel();
                addMBR.UIds.Add(user.ID);
                if (user.InviteUser != null)
                    addMBR.UIds.Add(user.InviteUser.Value);

                addMBR.SR = Guid.Parse(Reference.马币来源_赠送);
                addMBR.MBR = Guid.Parse(Reference.马币类别_邦币);
                addMBR.MBN = 1000;
                new BasisController().AddUMBR(addMBR, false);
                //互相赠送马币结束
                bll.Update(user);  
            }
            return RedirectToAction("I", "H");
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
 
        /// <summary>
        /// 获取绑定用户
        /// </summary>
        /// <param name="userWeiXinId"></param>
        /// <returns></returns>
        private bmUserWeixin GetUserByWeiXinId(string userWeiXinId)
        {
            if (!String.IsNullOrEmpty(userWeiXinId))
                return new BaseBll<bmUserWeixin>().All.Where(p => p.WeiXinId == userWeiXinId).FirstOrDefault();
            else
                return null;
        }        
    }
}
