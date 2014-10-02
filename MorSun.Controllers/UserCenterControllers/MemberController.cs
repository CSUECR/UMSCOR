using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MorSun.Bll;
using MorSun.Model;
using System.Web.Routing;
using HOHO18.Common;


namespace MorSun.Controllers
{
    [Authorize]
    [HandleError]
    public class MemberController : BasisController
    {       
        public ActionResult Index()
        {
            return View(new UInfo());
        }

        public ActionResult Info()
        {
            var cu = CurrentAspNetUser.wmfUserInfo;
            var UInfo = new UInfo();
            UInfo.NickName = cu.NickName;
            return View(UInfo);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Info(UInfo model, string returnUrl)
        {
            var oper = new OperationResult(OperationResultType.Error, "提交失败");
            if (ModelState.IsValid)
            {
                var ubll = new BaseBll<wmfUserInfo>();
                var uinfo = ubll.GetModel(UserID);
                TryUpdateModel(uinfo);
                ubll.Update(uinfo);
                //封装返回的数据
                fillOperationResult(Url.Action("Info", "Member"), oper, "修改成功");
                return Json(oper, JsonRequestBehavior.AllowGet);
            }
            oper.AppendData = ModelState.GE();
            return Json(oper, JsonRequestBehavior.AllowGet);
        }
    }
}
