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
            var invalidCommondService = new InvalidCommondService();
            if (model == null)
            { //传过来是空值时，返回系统资源分配中
                return invalidCommondService.InvalidCommondResponse(requestMessage);
            }
            if (model.MsgType == Guid.Parse(Reference.微信消息类别_声音))
            {
                var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageVoice>(requestMessage);
                responseMessage.Voice.MediaId = model.MediaId;
                return responseMessage;
            }
            if (model.MsgType == Guid.Parse(Reference.微信消息类别_图片))
            {
                var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageImage>(requestMessage);
                responseMessage.Image.MediaId = model.MediaId;
                return responseMessage;
            }
            if (model.MsgType == Guid.Parse(Reference.微信消息类别_文本))
            {
                var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
                responseMessage.Content = model.QAContent;
                return responseMessage;
            }
            return invalidCommondService.InvalidCommondResponse(requestMessage); 
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
            var bll = new BaseBll<bmQA>();
            var model = new bmQA();
            var qaATId = 0;
            if (requestMessage.Content.ToLower().StartsWith(CFG.查看问题))
                qaATId = Convert.ToInt32(requestMessage.Content.Substring(2).Trim());
            if (qaATId != 0)
                model = bll.All.FirstOrDefault(p => p.AutoGrenteId == qaATId);
            return QAResponse(requestMessage, model);
        }     
        #endregion
    }
}