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
    public class UnboundService
    {
        public ResponseMessageNews GetUnboundResponseMessage<T>(T requestMessage)
            where T : RequestMessageBase
        {
            //错误指令处理
            return UnboundResponse(requestMessage);            
        }

        /// <summary>
        /// 错误指令处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private ResponseMessageNews UnboundResponse<T>(T requestMessage)
            where T : RequestMessageBase
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage); 
            
            responseMessage.Articles.Add(new Article()
            {//眼睛图片
                Title = "您的账号未绑定邦马网，无法进行下一步操作",
                Description = "您的账号未绑定邦马网，无法进行下一步操作",
                PicUrl = "",
                Url = CFG.网站域名 + "/Account/Register"
            });    

            //判断用户是否绑定，未绑定显示注册账号并绑定，已经绑定显示分享链接
            new CommonService().RegOrShare(requestMessage, responseMessage);

            return responseMessage;
        }

        
    }
}