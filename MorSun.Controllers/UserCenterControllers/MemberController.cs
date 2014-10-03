using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MorSun.Bll;
using MorSun.Model;
using System.Web.Routing;
using HOHO18.Common;
using System.Web.Security;


namespace MorSun.Controllers
{
    [Authorize]
    [HandleError]
    public class MemberController : BasisController
    {
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }
        public ActionResult Index()
        {
            return View(new UInfo());
        }

        public ActionResult Info(string returnUrl)
        {
            var cu = CurrentAspNetUser.wmfUserInfo;
            var UInfo = new UInfo();
            UInfo.NickName = cu.NickName;
            return View(UInfo);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Info(UInfo uinfo, string returnUrl)
        {
            var oper = new OperationResult(OperationResultType.Error, "提交失败");
            if (ModelState.IsValid)
            {
                var ubll = new BaseBll<wmfUserInfo>();
                var model = ubll.GetModel(UserID);
                if(!uinfo.NickName.Eql(model.NickName))  //只有呢称时的临时方法，防止用户非法乱提交
                {
                    TryUpdateModel(model);
                    ubll.Update(model);
                }                
                //封装返回的数据
                fillOperationResult(Url.Action("Info", "Member"), oper, "修改成功");
                return Json(oper, JsonRequestBehavior.AllowGet);
            }
            oper.AppendData = ModelState.GE();
            return Json(oper, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult CPW(string returnUrl)
        {
            var model = new ChangePasswordModel();            
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CPW(ChangePasswordModel model, string returnUrl)
        {
            var oper = new OperationResult(OperationResultType.Error, "提交失败");

            if (!MembershipService.ValidateUser(CurrentAspNetUser.UserName, model.OldPassword))
            {
                "OldPassword".AE("旧密码输入错误", ModelState);
            }
            if (ModelState.IsValid)
            {
                if (!model.OldPassword.Eql(model.NewPassword))
                {
                    MembershipUser muser = Membership.GetUser(CurrentAspNetUser.UserName);
                    if (MembershipService.ChangePassword(CurrentAspNetUser.UserName, muser.ResetPassword(), model.NewPassword))
                    {
                    
                            var ubll = new BaseBll<wmfUserInfo>();
                            var user = ubll.All.Where(p => p.ID == CurrentAspNetUser.UserId).FirstOrDefault();
                            user.UserPassword = model.NewPassword.EP(user.ID.ToString());
                            ubll.Update(user);                   
                    }
                }
                //封装返回的数据
                fillOperationResult(Url.Action("CPW", "Member"), oper, "密码修改成功");
                return Json(oper, JsonRequestBehavior.AllowGet);
            }
            var er = ModelState.GE();            
            var eper = er.Where(p => p.Key == "").FirstOrDefault().ErrorMessages.Join();
            if(!String.IsNullOrEmpty(eper))
                oper = new OperationResult(OperationResultType.Error, eper);
            oper.AppendData = er;   
            return Json(oper, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CMB(string returnUrl)
        {
            var user = CurrentAspNetUser.wmfUserInfo;
            if(user != null && String.IsNullOrEmpty(user.Question1) && String.IsNullOrEmpty(user.Question2) && String.IsNullOrEmpty(user.Question3) && String.IsNullOrEmpty(user.Answer1) && String.IsNullOrEmpty(user.Answer2) && String.IsNullOrEmpty(user.Answer3))
            {
                var model = new ConfirmQuestionModel();
                return View(model);
            }
            return RedirectToAction("Info");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CMB(ConfirmQuestionModel model, string returnUrl)
        {
            var oper = new OperationResult(OperationResultType.Error, "提交失败");

            var user = CurrentAspNetUser.wmfUserInfo;
            if (user != null && !(String.IsNullOrEmpty(user.Question1) && String.IsNullOrEmpty(user.Question2) && String.IsNullOrEmpty(user.Question3) && String.IsNullOrEmpty(user.Answer1) && String.IsNullOrEmpty(user.Answer2) && String.IsNullOrEmpty(user.Answer3)))
            {
                "Question1".AE("密保不可再修改", ModelState);
            }

            if (ModelState.IsValid)
            {               
                var ubll = new BaseBll<wmfUserInfo>();
                var user2 = ubll.All.Where(p => p.ID == CurrentAspNetUser.UserId).FirstOrDefault();
                if (!(String.IsNullOrEmpty(model.Question1) && String.IsNullOrEmpty(model.Question2) && String.IsNullOrEmpty(model.Question3) && String.IsNullOrEmpty(model.Answer1) && String.IsNullOrEmpty(model.Answer2) && String.IsNullOrEmpty(model.Answer3)))
                {
                    TryUpdateModel(user2);
                    ubll.Update(user2); 
                }                   
                //封装返回的数据
                fillOperationResult(Url.Action("CPW", "Member"), oper, "修改成功");
                return Json(oper, JsonRequestBehavior.AllowGet);                
            }            
            oper.AppendData = ModelState.GE();
            return Json(oper, JsonRequestBehavior.AllowGet);
        }
    }
}
