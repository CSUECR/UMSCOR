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
            LogHelper.Write(Request.RawUrl, LogHelper.LogMessageType.Debug);
            var model = new BMQAViewVModel();
            if(id != null)
            {                 
                //需要取出顶级问题//业务原因，现在只搞了两级以后要不要扩展再说
                var bll = new BaseBll<bmQAView>();
                var refQId = Guid.Parse(Reference.问答类别_问题);
                var qavmodel = bll.All.FirstOrDefault(p => p.ID == id && p.QARef == refQId);
                if (qavmodel != null && qavmodel.ParentId != null)
                    qavmodel = bll.All.FirstOrDefault(p => p.ID == qavmodel.ParentId && p.QARef == refQId);

                if(qavmodel == null)
                    return RedirectToAction("I", "H");

                model.sId = qavmodel.ID;
                model.urlId = id;
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
                if (qCount == 0)
                    qCount = 1;
            }
            if(!id.HasValue || id==Guid.Empty)
            {
                "qCount".AE("参数错误", ModelState);
                s += "参数错误";
            }
            else 
            { 
                //var qa = new BaseBll<bmQA>().GetModel(id);
                var bmqaBll = new BaseBll<bmQAView>();
                var qaView = bmqaBll.GetModel(id);
                if (qaView == null)
                {
                    "qCount".AE("参数错误", ModelState);
                    s += "参数错误";
                }
                var qauser = new BaseBll<bmUserWeixin>().All.FirstOrDefault(p => p.WeiXinId == qaView.WeiXinId);
                if(qauser == null)
                {
                    "qCount".AE("未绑定", ModelState);
                    s += " 提问用户未绑定邦马网";
                }
                else if(qauser.UserId != UserID)
                {
                    "qCount".AE("不是您的", ModelState);
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
                    "qCount".AE("已被解答", ModelState);
                    s += " 该问题已经被解答";
                }
                //邦马币余额不足
                var numbbll = new BaseBll<bmNewUserMB>();                
                var UserBMB = numbbll.All.FirstOrDefault(p => p.UserId == UserID);
                tempMB = UserBMB.NMB.Value;
                tempBB = UserBMB.NBB.Value;
                if ((tempMB + tempBB) < qCount * defXFMB)
                {
                    "qCount".AE("余额不足", ModelState);
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OB(AddObjection t, string returnUrl)
        {
            var s = "";
            var tempMB = Convert.ToDecimal(0);
            var tempBB = Convert.ToDecimal(0);
            var defXFMB = Convert.ToDecimal(CFG.提交异议扣取压金值);
            
            t.ErrorNum = Math.Abs(t.ErrorNum);
            if (t.ErrorNum == 0)
                t.ErrorNum = 1;            
            if (t.QAId == Guid.Empty)
            {
                "ErrorNum".AE("参数错误", ModelState);
                s += "参数错误";
            }
            else
            {
                //var qa = new BaseBll<bmQA>().GetModel(id);
                var bmqaBll = new BaseBll<bmQAView>();
                var qaView = bmqaBll.GetModel(t.QAId);
                if (qaView == null)
                {
                    "ErrorNum".AE("参数错误", ModelState);
                    s += "参数错误";
                }
                else
                {
                    if((Math.Abs(qaView.MBNum) + Math.Abs(qaView.BBNum)) == 0)
                    {
                        "ErrorNum".AE("该问题是免费提问，您不能提交异议", ModelState);
                        s += "该问题是免费提问，您不能提交异议";
                    }
                    //有特殊情况，一次性提交多道问题时平均到每道题消耗的邦马币值小于500或大于500的情况。所以在这里不能限制。
                    //if ((Math.Abs(qaView.MBNum) + Math.Abs(qaView.BBNum)) < t.ErrorNum * defXFMB)
                    //{
                    //    "ErrorNum".AE("您提交异议所扣取的压金已经超过提问时消费的金额，请您减少错题数量，否则多扣的压金不会归还到您账户", ModelState);
                    //    s += "您提交异议所扣取的压金已经超过提问时消费的金额，请您减少错题数量，否则多扣的压金不会归还到您账户";
                    //}
                }
                var qauser = new BaseBll<bmUserWeixin>().All.FirstOrDefault(p => p.WeiXinId == qaView.WeiXinId);
                if (qauser == null)
                {
                    "ErrorNum".AE("提问用户未绑定邦马网", ModelState);
                    s += "提问用户未绑定邦马网";
                }
                else if (qauser.UserId != UserID)
                {
                    "ErrorNum".AE("不是您的问题你别动", ModelState);
                    s += " 不是您的问题你别动";
                }

                LogHelper.Write(qaView.MBNum.ToString() + qaView.BBNum.ToString(), LogHelper.LogMessageType.Debug);
                if (t.ErrorNum > 50)
                {//超过25000马币就不让再增加
                    "ErrorNum".AE("问题总数量不能超过50", ModelState);
                    s += "问题总数量不能超过50";
                }
                //已经被回答了则不再增加马币
                var refAId = Guid.Parse(Reference.问答类别_答案);
                var refBSId = Guid.Parse(Reference.问答类别_不是问题);
                var qada = bmqaBll.All.FirstOrDefault(p => p.ParentId == t.QAId && (p.QARef == refAId || p.QARef == refBSId));
                if (qada == null)
                {
                    "ErrorNum".AE("该问题未解答", ModelState);
                    s += " 该问题未解答";
                }
                else
                { 
                    //从问题解答开始到现在已经超过72个小时
                    var yyjg = Convert.ToInt32(CFG.用户提交异议有效时间间隔);                    
                    if (qada.RegTime.Value.AddHours(yyjg) < DateTime.Now)
                    {
                        "ErrorNum".AE("超期提交无效", ModelState);
                        s += "该问题解答时间到现在已超过" + yyjg + "小时，不能再提交异议";
                    }
                }
                var ob = new BaseBll<bmObjection>().All.FirstOrDefault(p => p.QAId == t.QAId);
                if(ob != null)
                {
                    "ErrorNum".AE("该问题已经提交了一次异议", ModelState);
                    s += " 该问题已经提交了一次异议";
                }
                //邦马币余额不足
                var numbbll = new BaseBll<bmNewUserMB>();
                var UserBMB = numbbll.All.FirstOrDefault(p => p.UserId == UserID);
                tempMB = UserBMB.NMB.Value;
                tempBB = UserBMB.NBB.Value;
                if ((tempMB + tempBB) < t.ErrorNum * defXFMB)
                {
                    "ErrorNum".AE("您的邦马币余额不足", ModelState);
                    s += " 您的邦马币余额不足";
                }
            }
            
            var oper = new OperationResult(OperationResultType.Error, "提交失败：" + s);
            if (ModelState.IsValid)
            {
                //添加异议
                var obBll = new BaseBll<bmObjection>();
                var model = new bmObjection();
                TryUpdateModel(model);
                model.ID = Guid.NewGuid();
                model.UserId = UserID;
                model.SubmitTime = DateTime.Now;

                model.IsSettle = false;
                model.RegUser = UserID;
                model.RegTime = DateTime.Now;
                model.ModTime = DateTime.Now;
                model.FlagTrashed = false;
                model.FlagDeleted = false;
                obBll.Insert(model);

                //马币扣取
                var bmumbBll = new BaseBll<bmUserMaBiRecord>();
                for (var i = 0; i < t.ErrorNum; i++)
                {
                    var umbrModel = new bmUserMaBiRecord();
                    umbrModel.SourceRef = Guid.Parse(Reference.马币来源_扣取);

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
                    umbrModel.QAId = t.QAId;
                    umbrModel.OBId = model.ID;

                    umbrModel.IsSettle = false;
                    umbrModel.RegTime = DateTime.Now;
                    umbrModel.ModTime = DateTime.Now;
                    umbrModel.FlagTrashed = false;
                    umbrModel.FlagDeleted = false;

                    umbrModel.ID = Guid.NewGuid();
                    umbrModel.UserId = UserID;
                    umbrModel.RegUser = UserID;

                    bmumbBll.Insert(umbrModel, false);
                }
                bmumbBll.UpdateChanges();
                //封装返回的数据
                fillOperationResult(returnUrl, oper, "提交成功");
                return Json(oper, JsonRequestBehavior.AllowGet);
            }


            oper.AppendData = ModelState.GE();
            return Json(oper, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 追问
        /// </summary>
        /// <param name="id"></param>
        /// <param name="AddQ"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddQ(Guid? id, string AddQ, string returnUrl)
        {
            var s = "";
            var bmqaBll = new BaseBll<bmQAView>();
            var qaView = bmqaBll.GetModel(id);
            if (!id.HasValue || id == Guid.Empty)
            {
                "AddQ".AE("参数错误", ModelState);
                s += "参数错误";
            }
            else
            {
                //var qa = new BaseBll<bmQA>().GetModel(id);       
                if (String.IsNullOrEmpty(AddQ))
                {
                    "AddQ".AE("追问无值", ModelState);
                    s += "请输入追问内容";
                }

                if (qaView == null)
                {
                    "AddQ".AE("参数错误", ModelState);
                    s += "参数错误";
                }
                var qauser = new BaseBll<bmUserWeixin>().All.FirstOrDefault(p => p.WeiXinId == qaView.WeiXinId);
                if (qauser == null)
                {
                    "AddQ".AE("未绑定", ModelState);
                    s += " 提问用户未绑定邦马网";
                }
                else if (qauser.UserId != UserID)
                {
                    "AddQ".AE("不是您的", ModelState);
                    s += " 不是您的问题您不能追问";
                }

                //已经被回答了则不再增加马币
                var refAId = Guid.Parse(Reference.问答类别_答案);
                var refBSId = Guid.Parse(Reference.问答类别_不是问题);
                var qada = bmqaBll.All.FirstOrDefault(p => p.ParentId == id && (p.QARef == refAId || p.QARef == refBSId));
                if (qada == null)
                {
                    "AddQ".AE("未解答", ModelState);
                    s += " 该问题未解答";
                }                
            }
            var oper = new OperationResult(OperationResultType.Error, "提交失败：" + s);
            if (ModelState.IsValid)
            {
                var bll = new BaseBll<bmQA>();
                var model = new bmQA();

                model.ID = Guid.NewGuid();

                model.WeiXinId = qaView.WeiXinId;
                model.QARef = Guid.Parse(Reference.问答类别_问题);
                model.MsgId = qaView.MsgId + DateTime.Now.ToString();
                model.MsgType = Guid.Parse(Reference.微信消息类别_文本);
                model.QAContent = AddQ;
                model.WeiXinAPP = qaView.WeiXinAPP;
                model.ParentId = qaView.ID;

                model.RegTime = DateTime.Now;
                model.ModTime = DateTime.Now;
                model.FlagTrashed = false;
                model.FlagDeleted = false;
                bll.Insert(model);
                //封装返回的数据
                fillOperationResult(returnUrl, oper, "提交成功");
                return Json(oper, JsonRequestBehavior.AllowGet);
            }
            oper.AppendData = ModelState.GE();
            return Json(oper, JsonRequestBehavior.AllowGet);
        }
    }
}
