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
using MorSun.Common.类别;
using HOHO18.Common.WEB;
using MorSun.Common.Privelege;
using MorSun.Common.配置;
using HOHO18.Common.Helper;


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
        public ActionResult I()
        {
            var cu = CurrentAspNetUser.wmfUserInfo;
            var UInfo = new UInfo();
            UInfo.NickName = cu.NickName;
            var uw = GetUserBound();
            if(uw == null || String.IsNullOrEmpty(uw.WeiXinId))
            {
                UInfo.IsBoundZYB = false;
                //缓存存储
                UInfo.BoundCode = CFG.微信绑定前缀 + " " + GetUserBoundCache().BoundCode.ToString();
            }
            else
            {
                UInfo.IsBoundZYB = true;
            }
            return View(UInfo);
        }

        /// <summary>
        /// 改个人信息
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
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
                    LogHelper.Write(model.aspnet_Users.UserName + "IP;" + Request.UserHostAddress + "修改呢称" + model.NickName + ";" + uinfo.NickName, LogHelper.LogMessageType.Info);
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

        /// <summary>
        /// 改密码
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
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
                        //user.UserPassword = model.NewPassword.EP(user.ID.ToString());
                        //ubll.Update(user);
                        LogHelper.Write(user.aspnet_Users.UserName + "IP;" + Request.UserHostAddress + "修改密码", LogHelper.LogMessageType.Info);
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

        /// <summary>
        /// 改密保
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
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
                    //TryUpdateModel(user2);
                    var ep = user2.ID.ToString();
                    //在这个位置要加密存储，否则用户无法使用
                    if (!String.IsNullOrEmpty(model.Question1))
                        user2.Question1 = model.Question1.EP(ep);
                    if (!String.IsNullOrEmpty(model.Question2))
                        user2.Question2 = model.Question2.EP(ep);
                    if (!String.IsNullOrEmpty(model.Question3))
                        user2.Question3 = model.Question3.EP(ep);

                    if (!String.IsNullOrEmpty(model.Answer1))
                        user2.Answer1 = model.Answer1.EP(ep);
                    if (!String.IsNullOrEmpty(model.Answer2))
                        user2.Answer2 = model.Answer2.EP(ep);
                    if (!String.IsNullOrEmpty(model.Answer3))
                        user2.Answer3 = model.Answer3.EP(ep);
                    ubll.Update(user2);
                    LogHelper.Write(user.aspnet_Users.UserName + "IP;" + Request.UserHostAddress + "修改密保", LogHelper.LogMessageType.Info);
                }                   
                //封装返回的数据
                fillOperationResult(Url.Action("Info", "Member"), oper, "修改成功");
                return Json(oper, JsonRequestBehavior.AllowGet);                
            }            
            oper.AppendData = ModelState.GE();
            return Json(oper, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult Recharge(string returnUrl)
        {
            var recharge = new Recharge();
            var curUser = CurrentAspNetUser;
            var yxKM = Guid.Parse(Reference.卡密有效性_有效);
            var ycKM = Guid.Parse(Reference.卡密充值_已充值);
            recharge.rList = new BaseBll<bmRecharge>().All.Where(p => p.UserId == curUser.UserId && p.Effective == yxKM && p.Recharge == ycKM).OrderByDescending(p => p.RegTime).Take(5);

            return View(recharge);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Recharge(Recharge recharge, string returnUrl)
        {
            var oper = new OperationResult(OperationResultType.Error, "提交失败");

            var curUser = CurrentAspNetUser;
            var startTime = DateTime.Now.AddMinutes(-5);
            var rbll = new BaseBll<bmRecharge>();
            var rFive = rbll.All.Where(p => p.aspnet_Users.UserId == UserID && p.RegTime >= startTime);
            //防攻击判断
            if (rFive.Count() >= 5)
            {
                "KaMe".AE("五分钟内最多只能输入5次卡密", ModelState);
            }
            //重复输入判断
            var repeatKaMe = rbll.All.FirstOrDefault(p => p.KaMe == recharge.KaMe);
            if(repeatKaMe != null)
            {
                "KaMe".AE("该卡密已经被使用", ModelState);
            }
            var sellKaMe = new BaseBll<bmSellKaMe>().All.FirstOrDefault(p => p.KaMe == recharge.KaMe);
            if(sellKaMe != null && sellKaMe.Recharge == Guid.Parse(Reference.卡密充值_已退款))
            {
                "KaMe".AE("该卡密已退款", ModelState);
            }
            if (ModelState.IsValid)
            {     
                var model = new bmRecharge();
                model.ID = Guid.NewGuid();
                model.KaMeUse = Guid.Parse(Reference.卡密用途_充值);
                model.UserId = curUser.UserId;
                model.KaMe = recharge.KaMe;
                model.Recharge = Guid.Parse(Reference.卡密充值_未充值);

                model.RegTime = DateTime.Now;
                model.ModTime = DateTime.Now;
                model.RegUser = curUser.UserId;
                model.FlagTrashed = false;
                model.FlagDeleted = false;
                rbll.Insert(model);
                LogHelper.Write(curUser.UserName + "IP;" + Request.UserHostAddress + "提交卡密", LogHelper.LogMessageType.Info);
                //封装返回的数据
                fillOperationResult(Url.Action("Recharge", "Member"), oper, "提交成功");
                return Json(oper, JsonRequestBehavior.AllowGet);
            }
            oper.AppendData = ModelState.GE();
            return Json(oper, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 取现
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult Take(string returnUrl)
        {            
            var take = new Take();
            var curUser = CurrentAspNetUser;
            take.tList = new BaseBll<bmTakeNow>().All.Where(p => p.UserId == curUser.UserId).OrderByDescending(p => p.RegTime).Take(5);
            var TWStartT = Convert.ToDateTime(TimeFormatHelper.ChinaWeekFirstDay());
            var TWEndT = Convert.ToDateTime(TimeFormatHelper.ChinaWeekLastDay()).AddDays(1).AddSeconds(-1);
            take.thisWeakTake = new BaseBll<bmTakeNow>().All.Where(p => p.UserId == curUser.UserId && p.RegTime >= TWStartT && p.RegTime <= TWEndT);
            ViewBag.RS = 资源.取现;
            return View(take);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Take(Take take, string returnUrl)
        {
            var oper = new OperationResult(OperationResultType.Error, "提交失败");
            var tbll = new BaseBll<bmTakeNow>();
            var curUser = CurrentAspNetUser;
            var curUserMB = CurrentUserMabi.NMB;
            
            var curUserTake = tbll.All.Where(p => p.UserId == curUser.UserId && p.Effective == null && p.TakeRef == null).Sum(p => p.MaBiNum);
            if (curUserTake == null) curUserTake = 0;
            if((curUserTake + take.MaBiNum) > curUserMB)
            {
                "MaBiNum".AE("已经超出取现范围", ModelState);
            }

            if(take.MaBiNum % 5000 != 0)
            {
                "MaBiNum".AE("币值必须为5000的整数倍", ModelState);
            }

            if(take.MaBiNum <= 0)
            {
                "MaBiNum".AE("币值必须大于0", ModelState);
            }

            if(!资源.取现.HP(操作.添加))
            {
                "MaBiNum".AE("您还未认证，无权限操作", ModelState);
            }
         
            if (ModelState.IsValid)
            {
                var model = new bmTakeNow();
                model.ID = Guid.NewGuid();                
                model.UserId = curUser.UserId;
                //要取绝对值，不能让用户填写负数
                model.MaBiNum = Math.Abs(take.MaBiNum);
                model.UserRemark = take.UserRemark;

                model.RegTime = DateTime.Now;
                model.ModTime = DateTime.Now;
                model.RegUser = curUser.UserId;
                model.FlagTrashed = false;
                model.FlagDeleted = false;
                tbll.Insert(model);
                LogHelper.Write(curUser.UserName + "IP;" + Request.UserHostAddress + "取现", LogHelper.LogMessageType.Info);
                //封装返回的数据
                fillOperationResult(Url.Action("Take", "Member"), oper, "提交成功");
                return Json(oper, JsonRequestBehavior.AllowGet);
            }
            oper.AppendData = ModelState.GE();
            return Json(oper, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取用户最近的5条马币结算记录
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult MaBiSettle(string returnUrl)
        {
            var take = new MaBiSettle();

            //币种
            var mabi = Guid.Parse(Reference.马币类别_马币);
            var bbi = Guid.Parse(Reference.马币类别_邦币);
            var banbi = Guid.Parse(Reference.马币类别_绑币);
            var curUser = CurrentAspNetUser;
            take.mabiList = new BaseBll<bmUserMaBiSettleRecord>().All.Where(p => p.UserId == curUser.UserId).OrderByDescending(p => p.RegTime).Take(5);
            take.bbiList = new BaseBll<bmUserMaBiSettleRecord>().All.Where(p => p.UserId == curUser.UserId).OrderByDescending(p => p.RegTime).Take(5);
            take.banbiList = new BaseBll<bmUserMaBiSettleRecord>().All.Where(p => p.UserId == curUser.UserId).OrderByDescending(p => p.RegTime).Take(5);
            return View(take);
        }

        /// <summary>
        /// 显示用户的答题缓存
        /// </summary>
        /// <returns></returns>
        public ActionResult CQ()
        {
            var uw = new BaseBll<bmUserWeixin>().All.FirstOrDefault(p => p.UserId == UserID);
            var model = new UserQACache();
            if(uw != null)
            {
                try { 
                    //从缓存中读取  不在这边设置缓存是为了防止并发
                    model = CacheAccess.GetFromCache(CFG.用户待答题缓存键前缀 + uw.WeiXinId) as UserQACache;
                }
                catch { }
            }
            return View(model);
        }
    }
}
