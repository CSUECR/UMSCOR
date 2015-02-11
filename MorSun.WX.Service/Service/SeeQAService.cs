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
    public class SeeQAService
    {        

        #region 提问返回数据处理
        /// <summary>
        /// 提问返回数据处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private IResponseMessageBase QAResponse<T>(T requestMessage, bmQA model)
            where T : RequestMessageBase
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage); //CreateResponseMessage<ResponseMessageNews>();
            var comonservice = new CommonService();
            if(model == null)
            {
                comonservice.NonObject(requestMessage, responseMessage, "未找到问题答案", "未找到问题答案", "", "");
            }
            else
            {
                if (model.MsgType == Guid.Parse(Reference.微信消息类别_图片))
                { 
                    responseMessage.Articles.Add(new Article()
                    {
                        Title = ("问题编号：" + model.AutoGrenteId + " "),// + ((model.MaBiNum == 0 || model.MaBiNum == null) ? "" : (model.MaBiNum == null ? "" : ("消耗" + model.MaBiNum.ToString("f0") + comonservice.GetReferenceValue(model.MaBiRef)))),
                        Description = "提问时间:" + DateTime.Now,
                        PicUrl = model.PicUrl,
                        Url = CFG.网站域名 + CFG.问题查看路径 + "/" + model.ID.ToString()
                    });
                }
                if(model.MsgType == Guid.Parse(Reference.微信消息类别_文本))
                {
                    responseMessage.Articles.Add(new Article()
                    {
                        Title = ("问题编号：" + model.AutoGrenteId + " "),
                        Description = "提问时间:" + DateTime.Now + "\r\n" + model.QAContent,
                        PicUrl = CFG.网站域名 + "/images/zyb/textQ.png",
                        Url = CFG.网站域名 + CFG.问题查看路径 + "/" + model.ID.ToString()
                    });
                }
                if (model.MsgType == Guid.Parse(Reference.微信消息类别_声音))
                {
                    responseMessage.Articles.Add(new Article()
                    {
                        Title = ("问题编号：" + model.AutoGrenteId + " "),
                        Description = "提问时间:" + DateTime.Now,
                        PicUrl = CFG.网站域名 + "/images/zyb/voice.png",
                        Url = CFG.网站域名 + CFG.问题查看路径 + "/" + model.ID.ToString()
                    });
                }
            }
            //判断用户是否绑定，未绑定显示注册账号并绑定，已经绑定显示分享链接
            //comonservice.RegOrShare(requestMessage, responseMessage);            
            return responseMessage;
        } 
        #endregion

        #region 用户取问题
        /// <summary>
        /// 按时间逆序获取用户提问的问题
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="skipNum"></param>
        /// <returns></returns>
        public IResponseMessageBase GetQAResponseMessage(RequestMessageText requestMessage)
        {
            //用户提交问题处理
            var skipNum = 1;
            var text = requestMessage.Content;            
            if (text.Contains(" "))
            {
                try 
                { 
                    var commond = text.Substring(0, text.IndexOf(" "));
                    var numValue = text.Substring(commond.Length + 1, text.Length - commond.Length - 1).Replace(" ","");
                    skipNum = Convert.ToInt32(String.IsNullOrEmpty(numValue) ? "1" : numValue);
                }
                catch
                {
                    return new InvalidCommondService().GetInvalidCommondResponseMessage(requestMessage as RequestMessageText);
                }
            }     
            return GetQuestionResponse(requestMessage, skipNum);
        }

        /// <summary>
        /// 取问题返回数据处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private IResponseMessageBase GetQuestionResponse(RequestMessageText requestMessage, int skipNum)
        {
            return QuestionResponse<RequestMessageText>(requestMessage, GetQuestion(requestMessage, skipNum));
        }

        /// <summary>
        /// 用户取问题
        /// </summary>
        /// <param name="requestMessage"></param>
        private bmQA GetQuestion(RequestMessageText requestMessage, int skipNum)
        {
            if (skipNum < 1)
                skipNum = 1;
            skipNum = skipNum - 1;
            var bll = new BaseBll<bmQA>();
            var qaRef = Guid.Parse(Reference.问答类别_问题);
            var curWeiXinAPP = Guid.Parse(CFG.邦马网_当前微信应用);
            var questionCount = bll.All.Where(p => p.WeiXinAPP != null && p.WeiXinAPP == curWeiXinAPP && p.WeiXinId == requestMessage.FromUserName && p.QARef == qaRef).Count();
            if (skipNum > questionCount)
                skipNum = questionCount - 1;
            var model = bll.All.Where(p => p.WeiXinAPP != null && p.WeiXinAPP == curWeiXinAPP && p.WeiXinId == requestMessage.FromUserName && p.QARef == qaRef).OrderByDescending(p => p.RegTime).Skip(skipNum).Take(1).FirstOrDefault();
            return model;
        }

        #endregion


        
    }
}