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
            var qadisbll = new BaseBll<bmQADistribution>();
            var qastate = Guid.Parse(Reference.分配答题操作_待解答);
            //默认用户未解决的问题修改为待解答，以超过配置的时间为准
            var acQADmn = 0 - Convert.ToInt32(CFG.未处理问题激活时间);
            var acQADdt = DateTime.Now.AddMinutes(acQADmn);
            var nonHandleRef = Guid.Parse(Reference.分配答题操作_未处理);
            var nonACQAD = qadisbll.All.Where(p => p.ModTime < acQADdt && ConstList.DefaultDISUser.Contains(p.WeiXinId) && p.Result == nonHandleRef);
            foreach(var item in nonACQAD)
            {
                item.Result = qastate;
                item.ModTime = DateTime.Now;
            }
            //测试看这边不更新数据库，以下代码能执行不。
            //未解决的问题修改为待解答 结束
            //不活跃用户处理
            var logoutMN = 0 - Convert.ToInt32(CFG.强制退出时间);
            var dt = DateTime.Now.AddMinutes(logoutMN);

            //待解答的问题数量
            var djdqadis = qadisbll.All.Where(p => p.Result == qastate);
            var mabiqaCount = djdqadis.Where(p => p.bmQA.MaBiNum > 0).Count();
            var nonmabiqaCount = djdqadis.Where(p => p.bmQA.MaBiNum == null || p.bmQA.MaBiNum <= 0).Count();

            var state = Guid.Parse(Reference.在线状态_在线);
            //用户待答题保有量
            var qaWaitCount = Convert.ToInt32(CFG.用户待答题保有量);

            //这边要过滤掉不活跃的在线用户,超过5分钟不活跃的，则不分配题目给他
            var nondismn = 0 - Convert.ToInt32(CFG.疑似退出时间);
            var nondisdt = DateTime.Now.AddMinutes(nondismn);

            //认证在线用户处理
            //活跃在线的认证答题用户,排除掉默认分配用户，省的多余操作
            var certificationUsers = bll.All.Where(p => p.WeiXinId !=  CFG.默认收费问题微信号 && p.State == state && p.ActiveTime >= nondisdt && ConstList.DTCertificationLevel.Contains(p.CertificationLevel)).OrderByDescending(p => p.ActiveNum);
            if(mabiqaCount > 0)
            {
                int selectCount = mabiqaCount / qaWaitCount;
                if (selectCount == 0)
                    selectCount = 1;
                certificationUsers = certificationUsers.Take(selectCount).OrderByDescending(p => p.ActiveNum);
            }

            //取不活跃用户
            var noActiveCU = bll.All.Where(p => p.State == state && p.ActiveTime < nondisdt && ConstList.DTCertificationLevel.Contains(p.CertificationLevel));

            if(certificationUsers.Count() > 0)
            {//存在在线用户时                
                if(noActiveCU.Count() > 0)
                {//存在不活跃的用户，将不活跃用户的答题分配记录标识为放弃，并将答题分配给活跃用户
                    var noActiveCUWIDS = noActiveCU.Select(p => p.WeiXinId);
                    var noActiveQAD = qadisbll.All.Where(p => p.Result == qastate && noActiveCUWIDS.Contains(p.WeiXinId)).OrderBy(p => p.bmQA.RegTime);
                    if(noActiveQAD.Count() > 0)
                    {//待答题数据重新分配
                        var ouCount = certificationUsers.Count();
                        var i = 0;
                        foreach(var item in noActiveQAD)
                        {
                            //对答题进行分配
                            i++;
                            var j = i % ouCount;
                            if (j == 0)
                                j = ouCount;
                            var disOU = certificationUsers.Skip(j - 1).Take(1).FirstOrDefault();
                            item.WeiXinId = disOU.WeiXinId;
                            item.ModTime = DateTime.Now;
                        }
                    }
                }
                //默认用户的待答题记录分配                
                var defCUQAD = qadisbll.All.Where(p => p.Result == qastate && p.WeiXinId == CFG.默认收费问题微信号).OrderBy(p => p.bmQA.RegTime);
                if(defCUQAD.Count() > 0)
                {//系统默认接收用户的待答题数据分配给当前在线答题人员
                    var ouCount = certificationUsers.Count();
                    var i = 0;
                    foreach (var item in defCUQAD)
                    {
                        //对答题进行分配
                        i++;
                        var j = i % ouCount;
                        if (j == 0)
                            j = ouCount;
                        var disOU = certificationUsers.Skip(j - 1).Take(1).FirstOrDefault();
                        item.WeiXinId = disOU.WeiXinId;
                        item.ModTime = DateTime.Now;
                    }
                }
            }
            else
            {//无在线活跃用户，系统将答题回收给默认用户
                 //var noActiveCU = bll.All.Where(p => p.State == state && p.ActiveTime < nondisdt && ConstList.DTCertificationLevel.Contains(p.CertificationLevel));
                 if (noActiveCU.Count() > 0)
                 {//存在不活跃的用户，将不活跃用户的答题分配记录标识为放弃，并将答题分配给活跃用户
                     var noActiveCUWIDS = noActiveCU.Select(p => p.WeiXinId);
                     var noActiveQAD = qadisbll.All.Where(p => p.Result == qastate && noActiveCUWIDS.Contains(p.WeiXinId)).OrderBy(p => p.bmQA.RegTime);
                     if (noActiveQAD.Count() > 0)
                     { //将答题回收给默认答题用户
                         foreach(var item in noActiveQAD)
                         {
                             item.WeiXinId = CFG.默认收费问题微信号;
                             item.ModTime = DateTime.Now;
                         }
                     }
                 }
            }
            
            //未认证的用户处理
            var noncertificationUsers = bll.All.Where(p => p.WeiXinId != CFG.默认免费问题微信号 && p.State == state && (p.CertificationLevel == null || !ConstList.DTCertificationLevel.Contains(p.CertificationLevel))).OrderByDescending(p => p.ActiveNum);
            if(nonmabiqaCount > 0)
            {
                int selectCount = nonmabiqaCount / qaWaitCount;
                if (selectCount == 0)
                    selectCount = 1;
                noncertificationUsers = noncertificationUsers.Take(selectCount).OrderByDescending(p => p.ActiveNum);
            }
            //取不活跃用户
            var noActiveU = bll.All.Where(p => p.State == state && p.ActiveTime < nondisdt && (p.CertificationLevel == null || !ConstList.DTCertificationLevel.Contains(p.CertificationLevel)));
            if(noncertificationUsers.Count() >0)
            {                
                if (noActiveU.Count() > 0)
                {//存在不活跃的用户，将不活跃用户的答题分配记录标识为放弃，并将答题分配给活跃用户
                    var noActiveUWIDS = noActiveU.Select(p => p.WeiXinId);
                    var noActiveQAD = qadisbll.All.Where(p => p.Result == qastate && noActiveUWIDS.Contains(p.WeiXinId)).OrderBy(p => p.bmQA.RegTime);
                    if (noActiveQAD.Count() > 0)
                    {//待答题数据重新分配
                        var ouCount = noncertificationUsers.Count();
                        var i = 0;
                        foreach (var item in noActiveQAD)
                        {
                            //对答题进行分配
                            i++;
                            var j = i % ouCount;
                            if (j == 0)
                                j = ouCount;
                            var disOU = noncertificationUsers.Skip(j - 1).Take(1).FirstOrDefault();
                            item.WeiXinId = disOU.WeiXinId;
                            item.ModTime = DateTime.Now;
                        }
                    }
                }
                //默认用户的待答题记录分配                
                var defUQAD = qadisbll.All.Where(p => p.Result == qastate && p.WeiXinId == CFG.默认免费问题微信号).OrderBy(p => p.bmQA.RegTime);
                if (defUQAD.Count() > 0)
                {//系统默认接收用户的待答题数据分配给当前在线答题人员
                    var ouCount = noncertificationUsers.Count();
                    var i = 0;
                    foreach (var item in defUQAD)
                    {
                        //对答题进行分配
                        i++;
                        var j = i % ouCount;
                        if (j == 0)
                            j = ouCount;
                        var disOU = noncertificationUsers.Skip(j - 1).Take(1).FirstOrDefault();
                        item.WeiXinId = disOU.WeiXinId;
                        item.ModTime = DateTime.Now;
                    }
                }
            }
            else
            {
                //var noActiveU = bll.All.Where(p => p.State == state && p.ActiveTime < nondisdt && (p.CertificationLevel == null || !ConstList.DTCertificationLevel.Contains(p.CertificationLevel)));
                if (noActiveU.Count() > 0)
                {//存在不活跃的用户，将不活跃用户的答题分配记录标识为放弃，并将答题分配给活跃用户
                    var noActiveUWIDS = noActiveU.Select(p => p.WeiXinId);
                    var noActiveQAD = qadisbll.All.Where(p => p.Result == qastate && noActiveUWIDS.Contains(p.WeiXinId)).OrderBy(p => p.bmQA.RegTime);
                    if (noActiveQAD.Count() > 0)
                    { //将答题回收给默认答题用户
                        foreach (var item in noActiveQAD)
                        {
                            item.WeiXinId = CFG.默认免费问题微信号;
                            item.ModTime = DateTime.Now;
                        }
                    }
                }
            }
            //强制退出不活跃用户  并将不活跃用户的答题缓存清空  
            var tqState = Guid.Parse(Reference.在线状态_退出);
            foreach(var item in noActiveCU)
            {
                //不修改修改时间 
                item.AQEndTime = DateTime.Now;
                item.State = tqState;                
                CacheAccess.RemoveCache(CFG.用户待答题缓存键前缀 + item.WeiXinId);
            }          
            foreach(var item in noActiveU)
            {
                item.AQEndTime = DateTime.Now;
                item.State = tqState;
                CacheAccess.RemoveCache(CFG.用户待答题缓存键前缀 + item.WeiXinId);
            }
            //统一更新进数据库
            bll.UpdateChanges();

            //不活跃用户处理结束

            //生成缓存对象
            var model = new OnlineQAUserCache();
            model.RefreshTime = DateTime.Now;           
            
            //收费待答题数量
            model.MaBiQACount = mabiqaCount;
            //免费待答题数量
            model.NonMaBiQACount = nonmabiqaCount;            
            model.CertificationUser = bll.All.Where(p => p.State == state && ConstList.DTCertificationLevel.Contains(p.CertificationLevel)).OrderByDescending(p => p.ActiveNum);
            model.NonCertificationQAUser = bll.All.Where(p => p.State == state && (p.CertificationLevel == null || !ConstList.DTCertificationLevel.Contains(p.CertificationLevel))).OrderByDescending(p => p.ActiveNum);
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
    }
}
