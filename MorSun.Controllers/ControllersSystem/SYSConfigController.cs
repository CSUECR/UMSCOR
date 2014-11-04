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
using MorSun.Common.认证级别;
using MorSun.Common.类别;

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

        /// <summary>
        /// 从数据库中取用户，并生成缓存类
        /// </summary>
        /// <returns></returns>
        private static OnlineQAUserCache GenerateQAUserCache()
        {
            var bll = new BaseBll<bmOnlineQAUser>();
            var model = new OnlineQAUserCache();
            model.RefreshTime = DateTime.Now;
            var state = Guid.Parse(Reference.在线状态_在线);
            model.CertificationUser = bll.All.Where(p => p.State == state && CertificationLevel.DTCertificationLevel.Contains(p.CertificationLevel)).OrderByDescending(p => p.ActiveNum);
            model.CertificationUser = bll.All.Where(p => p.State == state && (p.CertificationLevel == null || !CertificationLevel.DTCertificationLevel.Contains(p.CertificationLevel))).OrderByDescending(p => p.ActiveNum);
            return model;
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
    }
}
