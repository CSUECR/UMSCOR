using MorSun.Common.配置;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MorSun.Controllers.ViewModel;
using HOHO18.Common;
using MorSun.Bll;
using MorSun.Model;
using HOHO18.Common.WEB;
using MorSun.Common.类别;

namespace MorSun.Controllers
{    
    [HandleError]
    public class QAController : BasisController
    {
        public ActionResult Q(Guid? id)
        {
            LogHelper.Write(Request.RawUrl, LogHelper.LogMessageType.Info);
            var model = new BMQAViewVModel();
            if(id != null)
            { 
                model.sParentId = id;
                return View(model);
            }    
            else
            {
                return RedirectToAction("I", "H");
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MB(Guid? id, int? qCount,string returnUrl)
        {
            var s = "";
            var tempMB = Convert.ToDecimal(0);
            var tempBB = Convert.ToDecimal(0);
            var defXFMB = Convert.ToDecimal(CFG.提问默认收费马币值);
            if (!qCount.HasValue)
                qCount = 1;
            else
            {
                qCount = Math.Abs(qCount.Value);
            }
            if(!id.HasValue || id==Guid.Empty)
            {
                "id".AE("参数错误", ModelState);
                s += "参数错误";
            }
            else 
            { 
                //var qa = new BaseBll<bmQA>().GetModel(id);
                var bmqaBll = new BaseBll<bmQAView>();
                var qaView = bmqaBll.GetModel(id);
                if (qaView == null)
                {
                    "id".AE("参数错误", ModelState);
                    s += "参数错误";
                }
                var qauser = new BaseBll<bmUserWeixin>().All.FirstOrDefault(p => p.WeiXinId == qaView.WeiXinId);
                if(qauser == null)
                {
                    "id".AE("提问用户未绑定邦马网", ModelState);
                    s += " 提问用户未绑定邦马网";
                }
                else if(qauser.UserId != UserID)
                {
                    "id".AE("不是您的问题你别动", ModelState);
                    s += " 不是您的问题你别动";
                }

                LogHelper.Write(qaView.MBNum.ToString() + qaView.BBNum.ToString(), LogHelper.LogMessageType.Debug);
                if ((Math.Abs(qaView.MBNum) + Math.Abs(qaView.BBNum) + qCount * defXFMB)  >= 25000)
                {//超过25000马币就不让再增加
                    "qCount".AE("超过50", ModelState);
                    s += "问题总数量不能超过50";
                }
                //已经被回答了则不再增加马币
                var refAId = Guid.Parse(Reference.问答类别_答案);
                var refBSId = Guid.Parse(Reference.问答类别_不是问题);
                var qada = bmqaBll.All.FirstOrDefault(p => p.ParentId == id && (p.QARef == refAId || p.QARef == refBSId));
                if(qada != null)
                {
                    "id".AE("该问题已经被解答", ModelState);
                    s += " 该问题已经被解答";
                }
                //邦马币余额不足
                var numbbll = new BaseBll<bmNewUserMB>();                
                var UserBMB = numbbll.All.FirstOrDefault(p => p.UserId == UserID);
                tempMB = UserBMB.NMB.Value;
                tempBB = UserBMB.NBB.Value;
                if((tempMB + tempBB) < defXFMB)
                {
                    "id".AE("您的邦马币余额不足", ModelState);
                    s += " 您的邦马币余额不足";
                }
            }
            var oper = new OperationResult(OperationResultType.Error, "提交失败：" + s);
            if (ModelState.IsValid)
            {
                var bmumbBll = new BaseBll<bmUserMaBiRecord>();
                for(var i = 0; i<qCount; i++)
                { 
                    var umbrModel = new bmUserMaBiRecord();
                    umbrModel.SourceRef = Guid.Parse(Reference.马币来源_消费);
                
                    if (tempBB >= defXFMB)
                    {
                        umbrModel.MaBiRef = Guid.Parse(Reference.马币类别_邦币);
                        tempBB -= defXFMB;
                    }
                    else if (tempMB >= defXFMB)
                    {
                        umbrModel.MaBiRef = Guid.Parse(Reference.马币类别_马币);
                        tempMB -= defXFMB;
                    }
                    umbrModel.MaBiNum = 0 - defXFMB;
                    umbrModel.QAId = id;

                    umbrModel.IsSettle = false;
                    umbrModel.RegTime = DateTime.Now;
                    umbrModel.ModTime = DateTime.Now;
                    umbrModel.FlagTrashed = false;
                    umbrModel.FlagDeleted = false;

                    umbrModel.ID = Guid.NewGuid();
                    umbrModel.UserId = UserID;
                    umbrModel.RegUser = UserID;

                    bmumbBll.Insert(umbrModel,false);
                }
                bmumbBll.UpdateChanges();
                //封装返回的数据
                fillOperationResult(returnUrl, oper, "马币增加成功");
                return Json(oper, JsonRequestBehavior.AllowGet);
            }
            oper.AppendData = ModelState.GE();
            return Json(oper, JsonRequestBehavior.AllowGet);
        }
    }
}
