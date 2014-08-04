using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MorSun.Bll;
using MorSun.Model;
using System.Web.Routing;
using System.Web.Configuration;
using System.Configuration;

namespace MorSun.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {            
            ViewBag.Title = "悟空打码";
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
            var effectiveHour = 0 - "EncryptTime".GetXmlConfig().ToAs<int>();
            var timeBefore = DateTime.Now.AddHours(effectiveHour);
            var model = bll.All.Where(p => p.EncryptCode == id && p.EncryptTime >= timeBefore).OrderByDescending(p => p.RegTime).FirstOrDefault();
            //var au = "ActiveUserUrl".GetXmlConfig();
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
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// webconfig加密解密
        /// </summary>
        /// <returns></returns>
        public string ENCWeb()
        {
            var provider = "RSAProtectedConfigurationProvider";
            var section = "connectionStrings";
            var section1 = "quartz";
            var section2 = "log4net";
            Configuration confg = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
            ConfigurationSection configSect = confg.GetSection(section);            
            if (configSect != null)
            {
                configSect.SectionInformation.ProtectSection(provider);
                confg.Save();
            }

            ConfigurationSection configSect1 = confg.GetSection(section1);
            if (configSect1 != null)
            {
                configSect1.SectionInformation.ProtectSection(provider);
                confg.Save();
            }

            ConfigurationSection configSect2 = confg.GetSection(section2);
            if (configSect2 != null)
            {
                configSect2.SectionInformation.ProtectSection(provider);
                confg.Save();
            }
            return "";
        }

        public string DECWeb()
        {
            var provider = "RSAProtectedConfigurationProvider";
            var section = "connectionStrings";
            var section1 = "quartz";
            var section2 = "log4net";
            Configuration config = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
            ConfigurationSection configSect = config.GetSection(section);
            if (configSect.SectionInformation.IsProtected)
            {
                configSect.SectionInformation.UnprotectSection();
                config.Save();
            }

            ConfigurationSection configSect1 = config.GetSection(section1);
            if (configSect1.SectionInformation.IsProtected)
            {
                configSect1.SectionInformation.UnprotectSection();
                config.Save();
            }

            ConfigurationSection configSect2 = config.GetSection(section2);
            if (configSect2.SectionInformation.IsProtected)
            {
                configSect2.SectionInformation.UnprotectSection();
                config.Save();
            }
            return "";
        }
    }
}
