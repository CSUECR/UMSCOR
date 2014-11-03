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

namespace MorSun.Controllers.SystemController
{
    [HandleError]
    [Authorize]
    public class MMController : BaseController<aspnet_Users>
    {
        protected override string ResourceId
        {
            get { return 资源.用户; }
        }


        public ActionResult CL(Guid userId, string returnUrl)
        {
            if (ResourceId.HP(操作.修改))
            {
                ViewBag.RS = ResourceId;
                ViewBag.ReturnUrl = returnUrl;
                var bll = new BaseBll<wmfUserInfo>();
                var uinfo = bll.GetModel(userId);
                var model = new UserCL();
                model.UserId = userId;
                model.UserName = uinfo == null ? "" : uinfo.aspnet_Users.UserName;
                model.NickName = uinfo == null ? "" : uinfo.NickName;
                model.CLevel = uinfo == null ? null : uinfo.CertificationLevel;
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
        public ActionResult CL(UserCL uc, string returnUrl)
        {
            if (ResourceId.HP(操作.修改))
            {
                var oper = new OperationResult(OperationResultType.Error, "认证失败");    
                ViewBag.ReturnUrl = returnUrl;
                var bll = new BaseBll<wmfUserInfo>();
                var model = bll.GetModel(uc.UserId);
                if (model == null)
                {
                    "UserId".AE("认证失败", ModelState);
                }
                model.CertificationLevel = uc.CLevel;
                if (ModelState.IsValid)
                {
                    bll.Update(model);
                    fillOperationResult(returnUrl, oper, "修改成功");
                }
                else
                {
                    "".AE("修改失败", ModelState);
                    oper.AppendData = ModelState.GE();
                }
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
