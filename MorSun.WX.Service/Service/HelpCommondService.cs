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
    public class HelpCommondService
    {
        public IResponseMessageBase GetHelpCommondResponseMessage<T>(T requestMessage)
            where T : RequestMessageBase
        {
            //错误指令处理
            return HelpCommondResponse(requestMessage);            
        }

        /// <summary>
        /// 错误指令处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private IResponseMessageBase HelpCommondResponse<T>(T requestMessage)
            where T : RequestMessageBase
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage); 
            
            responseMessage.Articles.Add(new Article()
            {//眼睛图片
                Title = "邦马网帮助指令",
                Description = "提问可直接发送文字问题、语音问题、图片问题\r\n答题可发送答题命令：dt\r\n微信绑定邦马网，请登录邦马网，在会员中心获取绑定代码并发送\r\n\r\n邦马官网：www.bungma.com\r\n官方指定淘宝专卖店：bungma.taobao.com",
                PicUrl = CFG.网站域名 + "/images/zyb/bigsmile.png",
                Url = CFG.网站域名
            });
            //responseMessage.Articles.Add(new Article()
            //{//眼睛图片
            //    Title = "查看指令帮助文档",
            //    Description = "查看指令帮助文档",
            //    PicUrl = "",
            //    Url = CFG.网站域名 + "CommondHelp".GX()
            //});

            //判断用户是否绑定，未绑定显示注册账号并绑定，已经绑定显示分享链接
            //new CommonService().RegOrShare<RequestMessageText>(requestMessage, responseMessage);

            return responseMessage;
        } 
    }
}