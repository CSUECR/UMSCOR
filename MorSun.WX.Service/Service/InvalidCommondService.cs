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
        public ResponseMessageNews GetInvalidCommondResponseMessage(RequestMessageText requestMessage)
        {
            //错误指令处理
            var responseMessage = InvalidCommondResponse(requestMessage);
            return responseMessage;
        }

        /// <summary>
        /// 错误指令处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private ResponseMessageNews InvalidCommondResponse(RequestMessageText requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage); 
            
            responseMessage.Articles.Add(new Article()
            {//眼睛图片
                Title = "邦马网无法与您的指令对接",
                Description = "邦马网无法与您的指令对接",
                PicUrl = "",
                Url = CFG.网站域名 + "/QA/Q/"
            });//再增加 加码 求解题思路            
            //responseMessage.Articles.Add(new Article()
            //{//美元图片
            //    Title = "加马币",
            //    Description = "加马币",
            //    PicUrl = "",
            //    Url = CFG.网站域名 + "/QA/Q/"
            //});
            //responseMessage.Articles.Add(new Article()
            //{//问号图片
            //    Title = "求思路",
            //    Description = "求思路",
            //    PicUrl = "",
            //    Url = CFG.网站域名 + "/QA/Q/"
            //});            

            //判断用户是否绑定，未绑定显示注册账号并绑定，已经绑定显示分享链接
            var userWeiXin = new UserMaBiService().GetUserByWeiXinId(requestMessage.FromUserName);
            if (userWeiXin == null)
            {
                responseMessage.Articles.Add(new Article()
                {//问号图片
                    Title = "注册账号并绑定",
                    Description = "注册账号并绑定",
                    PicUrl = "",
                    Url = CFG.网站域名 + "/Account/Register"
                });
            }
            else
            {
                responseMessage.Articles.Add(new Article()
                {//问号图片
                    Title = "分享给朋友",
                    Description = "分享给朋友",
                    PicUrl = "",
                    Url = CFG.网站域名 + "/Home/WXShareLink/" + SecurityHelper.Encrypt(requestMessage.FromUserName)
                });
            }

            return responseMessage;
        } 
    }
}