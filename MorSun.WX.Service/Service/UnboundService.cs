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
        public IResponseMessageBase GetUnboundResponseMessage<T>(T requestMessage)
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
        private IResponseMessageBase UnboundResponse<T>(T requestMessage)
            where T : RequestMessageBase
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage); 
            
            responseMessage.Articles.Add(new Article()
            {//眼睛图片
                Title = "您的账号未绑定邦马网，系统无法处理您的请求",
                Description = "微信绑定邦马网，请登录邦马网，在会员中心获取绑定代码并发送",
                PicUrl = CFG.网站域名 + "/images/zyb/reg.jpg",
                Url = CFG.网站域名
            });    

            //判断用户是否绑定，未绑定显示注册账号并绑定，已经绑定显示分享链接
            //new CommonService().RegOrShare(requestMessage, responseMessage);

            return responseMessage;
        }

        
    }
}