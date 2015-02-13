using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using HOHO18.Common;
using MorSun.Bll;
using System.Collections;
using System.Web.Mvc;
using MorSun.Controllers.ViewModel;
using MorSun.Common.Privelege;
using MorSun.WX.ZYB.Service;
using MorSun.Common.常量集;
using MorSun.Common.类别;
using MorSun.Common.配置;
using HOHO18.Common.WEB;
using System.Configuration;
using System.Web.Configuration;
using MorSun.Controllers.Quartz;


namespace MorSun.Controllers.SystemController
{
    [HandleError]
    [Authorize]
    public class SYSConfigController : BaseController<bmOnlineQAUser>
    {
        protected override string ResourceId
        {
            get { return 资源.系统参数配置; }
        }
        
        /// <summary>
        /// 用户认证
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult OnlineQAUser(string returnUrl)
        {
            if (ResourceId.HP(操作.查看))
            {
                ViewBag.RS = ResourceId;
                ViewBag.ReturnUrl = returnUrl;
                var model = UserQAService.GetOlineQAUserCache();
                if(model == null)
                {
                    model = GenerateQAUserCache();
                    UserQAService.SetOlineQAUserCache(model);
                    LogHelper.Write("完成手动更新用户缓存", LogHelper.LogMessageType.Debug);
                }                
                return View(model);
            }
            else
            {
                "".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper, JsonRequestBehavior.AllowGet);
                //return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
            }
        }        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetOnlineQAUser(string returnUrl)
        {
            if (ResourceId.HP(操作.修改))
            {
                var oper = new OperationResult(OperationResultType.Error, "设置失败");    
                //ViewBag.ReturnUrl = returnUrl;
                UserQAService.SetOlineQAUserCache(GenerateQAUserCache());
                fillOperationResult(returnUrl, oper, "修改成功");
                return Json(oper, JsonRequestBehavior.AllowGet);
            }
            else
            {
                "".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper, JsonRequestBehavior.AllowGet);
                //return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
            }
        }

        /// <summary>
        /// 用户答题缓存
        /// </summary>
        /// <param name="weixinId"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult UserQACache(string id, string returnUrl)
        {
            if (ResourceId.HP(操作.查看))
            {
                ViewBag.RS = ResourceId;
                ViewBag.ReturnUrl = returnUrl;
                var model = UserQAService.GetUserQACache(CFG.用户待答题缓存键前缀 + id);  
                if(model == null)
                    LogHelper.Write("用户答题缓存为空", LogHelper.LogMessageType.Error);
                return View(model);
            }
            else
            {
                "".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper, JsonRequestBehavior.AllowGet);
                //return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetAccessTokenCache(string returnUrl)
        {
            if (ResourceId.HP(操作.修改))
            {
                var oper = new OperationResult(OperationResultType.Error, "设置失败");
                //ViewBag.ReturnUrl = returnUrl;                
                SetWXTKCache();
                SetWXTICCache();
                fillOperationResult(returnUrl, oper, "修改成功");
                return Json(oper, JsonRequestBehavior.AllowGet);
            }
            else
            {
                "".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper, JsonRequestBehavior.AllowGet);
                //return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
            }
        }

        /// <summary>
        /// 查看AT缓存
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult AccessTokenCache(string returnUrl)
        {
            if (ResourceId.HP(操作.查看))
            {
                ViewBag.RS = ResourceId;
                ViewBag.ReturnUrl = returnUrl;
                var s = GetWXTKCache();

                s += "||||";
                s += GetWXTICCache();

                return Content(s);
            }
            else
            {
                "".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper, JsonRequestBehavior.AllowGet);
                //return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
            }
        }

        #region 矿石代码
        /// <summary>
        /// 定时读取
        /// </summary>
        /// <returns></returns>
        public string dsdq()
        {
            MorSunScheduler.Instance.Clear();
            //CheckingTrigger t = new CheckingTrigger();
            //t.Run();
            //CheckingTrigger2 t2 = new CheckingTrigger2();
            //t2.Run();
            //SimpleTriggerExample t3 = new SimpleTriggerExample();
            //t3.Run();
            CheckingTrigger4 t4 = new CheckingTrigger4();
            t4.Run();
            CheckingTrigger5 t5 = new CheckingTrigger5();
            t5.Run();
            return "true";
        }

        /// <summary>
        /// 是否开启状态
        /// </summary>
        /// <returns></returns>
        public string IsStart()
        {
            return MorSunScheduler.Instance.IsStart().ToString();
        }

        /// <summary>
        /// 清除矿石任务
        /// </summary>
        /// <returns></returns>
        public string Clear()
        {
            MorSunScheduler.Instance.Clear();
            return "true";
        }

        /// <summary>
        /// 停止矿石
        /// </summary>
        /// <returns></returns>
        public string Stop()
        {
            MorSunScheduler.Instance.Stop(false);
            return "true";
        }

        /// <summary>
        /// 开启矿石
        /// </summary>
        /// <returns></returns>
        public string Start()
        {
            MorSunScheduler.Instance.Start();
            return "true";
        }

        /// <summary>
        /// 全部继续
        /// </summary>
        /// <returns></returns>
        public string ResumeAll()
        {
            MorSunScheduler.Instance.ResumeAll();
            return "true";
        }

        /// <summary>
        /// 停止某项工作
        /// </summary>
        /// <returns></returns>
        public string StopJob(string name, string group)
        {
            MorSunScheduler.Instance.StopJob(name, group);
            return "true";
        }

        /// <summary>
        /// 触发某项工作
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public string TriggerJob(string name, string group)
        {
            MorSunScheduler.Instance.TrggerJob(name, group);
            return "true";
        }
        #endregion

        #region webconfig加解密
        /// <summary>
        /// webconfig加密解密
        /// </summary>
        /// <returns></returns>
        public string ENCWebVNIRWIfkdain()
        {
            if (!ResourceId.HP(操作.修改))
            {                
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return "无权限";
            }
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

        public string DECWebEIvniewqWNfdsV()
        {
            if (!ResourceId.HP(操作.修改))
            {
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return "无权限";
            }
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
        #endregion
    }
}
