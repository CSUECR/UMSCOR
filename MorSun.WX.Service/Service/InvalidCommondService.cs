using System;
using System.Linq;
using System.Collections.Generic;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.GoogleMap;
using Senparc.Weixin.MP.Helpers;
using MorSun.Bll;
using MorSun.Model;
using MorSun.Common.类别;
using MorSun.Common.配置;
using HOHO18.Common.SSO;

namespace MorSun.WX.ZYB.Service
{
    public class InvalidCommondService
    {
        public ResponseMessageNews GetInvalidCommondResponseMessage<T>(T requestMessage)
            where T : RequestMessageBase
        {
            //错误指令处理
            return InvalidCommondResponse(requestMessage);            
        }

        /// <summary>
        /// 错误指令处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private ResponseMessageNews InvalidCommondResponse<T>(T requestMessage)
            where T : RequestMessageBase
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage); 
            
            responseMessage.Articles.Add(new Article()
            {//眼睛图片
                Title = "邦马网无法与您的指令对接",
                Description = "邦马网无法与您的指令对接",
                PicUrl = "",
                Url = ""
            });
            responseMessage.Articles.Add(new Article()
            {//眼睛图片
                Title = "查看指令帮助文档",
                Description = "查看指令帮助文档",
                PicUrl = "",
                Url = CFG.网站域名 + "CommondHelp".GX()
            });

            //判断用户是否绑定，未绑定显示注册账号并绑定，已经绑定显示分享链接
            //new CommonService().RegOrShare<RequestMessageText>(requestMessage, responseMessage);

            return responseMessage;
        } 
    }
}