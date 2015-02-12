using System;
using System.Linq;
using System.Collections.Generic;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Helpers;
using MorSun.Bll;
using MorSun.Model;
using MorSun.Common.类别;
using MorSun.Common.配置;
using HOHO18.Common.SSO;
using HOHO18.Common.WEB;

namespace MorSun.WX.ZYB.Service
{
    public class MoreInfoService
    {        

        #region 答题获取的详细信息
        /// <summary>
        /// 提问返回数据处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private IResponseMessageBase AnswerResponse<T>(T requestMessage, bmQAView model)
            where T : RequestMessageBase
        {
            LogHelper.Write("返回待答问题", LogHelper.LogMessageType.Debug);
            if (model == null)
            { //传过来是空值时，返回系统资源分配中
                return new AnswerService().NonDistributionResponse(requestMessage);
            }            
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage); //CreateResponseMessage<ResponseMessageNews>();
            var comonservice = new CommonService();

            if (model.MsgType == Guid.Parse(Reference.微信消息类别_图片))
            {
                responseMessage.Articles.Add(new Article()
                {
                    Title = ("问题编号：" + model.AutoGrenteId + " " + ((model.MBNum == 0 || model.MBNum == null) && (model.BBNum == 0 || model.BBNum == null) ? "免费提问" : ("消耗" + ((model.MBNum == 0 || model.MBNum == null) ? "" : (Math.Abs(model.MBNum).ToString("f0") + "马币")) + ((model.BBNum == 0 || model.BBNum == null) ? "" : (Math.Abs(model.BBNum).ToString("f0") + "邦币"))))),
                    Description = "提问时间:" + (model.RegTime == null ? "" : (model.RegTime.ToShortDateString() + " " + model.RegTime.Value.ToShortTimeString()))
                    + "\r\n获取时间:" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()
                    + "\r\n当前未答题数： " + model.DJDCount
                    ,
                    PicUrl = model.PicUrl,
                    Url = CFG.网站域名 + CFG.问题查看路径 + "/" + model.ID.ToString() //model.PicUrl
                });
            }

            if (model.MsgType == Guid.Parse(Reference.微信消息类别_文本))
            {
                responseMessage.Articles.Add(new Article()
                {
                    Title = ("问题编号：" + model.AutoGrenteId + " " + ((model.MBNum == 0 || model.MBNum == null) && (model.BBNum == 0 || model.BBNum == null) ? "免费提问" : ("消耗" + ((model.MBNum == 0 || model.MBNum == null) ? "" : (Math.Abs(model.MBNum).ToString("f0") + "马币")) + ((model.BBNum == 0 || model.BBNum == null) ? "" : (Math.Abs(model.BBNum).ToString("f0") + "邦币"))))),
                    Description = model.QAContent
                    + "\r\n"
                    + "\r\n提问时间:" + (model.RegTime == null ? "" : (model.RegTime.ToShortDateString() + " " + model.RegTime.Value.ToShortTimeString()))
                    + "\r\n获取时间:" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()
                    + "\r\n当前未答题数： " + model.DJDCount
                    ,
                    PicUrl = CFG.网站域名 + "/images/zyb/textQ.png",
                    Url = CFG.网站域名 + CFG.问题查看路径 + "/" + model.ID.ToString()
                });
            }
            if (model.MsgType == Guid.Parse(Reference.微信消息类别_声音))
            {
                responseMessage.Articles.Add(new Article()
                {
                    Title = ("问题编号：" + model.AutoGrenteId + " " + ((model.MBNum == 0 || model.MBNum == null) && (model.BBNum == 0 || model.BBNum == null) ? "免费提问" : ("消耗" + ((model.MBNum == 0 || model.MBNum == null) ? "" : (Math.Abs(model.MBNum).ToString("f0") + "马币")) + ((model.BBNum == 0 || model.BBNum == null) ? "" : (Math.Abs(model.BBNum).ToString("f0") + "邦币"))))),
                    Description = "提问时间:" + (model.RegTime == null ? "" : (model.RegTime.ToShortDateString() + " " + model.RegTime.Value.ToShortTimeString()))
                    + "\r\n获取时间:" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()
                    + "\r\n当前未答题数： " + model.DJDCount
                    ,
                    PicUrl = CFG.网站域名 + "/images/zyb/voice.png",
                    Url = CFG.网站域名 + CFG.问题查看路径 + "/" + model.ID.ToString() //model.PicUrl
                });
            }
            return responseMessage;
            
        }
        #endregion        

        #region 答题用户取问题             
        /// <summary>
        /// 回答问题处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public IResponseMessageBase GetAnswerQuestionResponseMessage(RequestMessageText requestMessage)
        {
            var commonService = new CommonService();
            var answerService = new AnswerService();
            //未绑定的用户录入获取问题详细信息的处理
            var userWeiXin = commonService.GetZYBUserByWeiXinId(requestMessage.FromUserName);
            if (userWeiXin == null)
            {
                return new UnboundService().GetUnboundResponseMessage(requestMessage);
            }
            else
            {
                //已经绑定的用户处理      
                var onlineuserCache = UserQAService.GetOlineQAUserCache();
                //处理并发而生成的操作唯一ID
                var rqid = Guid.NewGuid();
                answerService.RQStart(requestMessage, rqid, commonService);
                var ics = new InvalidCommondService();
                if (onlineuserCache == null)
                {
                    LogHelper.Write("图片回答问题，无在线用户缓存", LogHelper.LogMessageType.Debug);
                    //不是在线答题用户，直接返回无效命令 
                    return ics.GetInvalidCommondResponseMessage(requestMessage);
                }
                else
                {
                    //在线用户是否存在该用户
                    if (onlineuserCache.CertificationUser.FirstOrDefault(p => p.WeiXinId == requestMessage.FromUserName) == null
                        && onlineuserCache.NonCertificationQAUser.FirstOrDefault(p => p.WeiXinId == requestMessage.FromUserName) == null)
                    {
                        LogHelper.Write("图片回答问题，当前用户不在缓存里", LogHelper.LogMessageType.Debug);
                        //不是在线答题用户，直接返回无效命令 
                        return ics.GetInvalidCommondResponseMessage(requestMessage);
                    }

                    var qakey = CFG.用户待答题缓存键前缀 + requestMessage.FromUserName;
                    var model = UserQAService.GetUserQACache(qakey);
                    if (model == null || model.CurrentQA == null)
                    {
                        LogHelper.Write("图片回答问题，当前用户答题缓存为空或无当前答题", LogHelper.LogMessageType.Debug);
                        //用户答题缓存为空，
                        return ics.GetInvalidCommondResponseMessage(requestMessage);
                    }

                    //更新用户活跃时间 将用户添加或更新进数据库，由统一方法设置缓存
                    UserQAService.AddOrUpdateOnlineQAUser(requestMessage, userWeiXin, rqid);
                    LogHelper.Write("图片回答问题，更新用户活跃时间", LogHelper.LogMessageType.Debug);
                    return AnswerResponse(requestMessage, answerService.PackCurrentQA(requestMessage, model));
                }
            }
        }
        #endregion


        
    }
}