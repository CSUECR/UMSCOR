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
using HOHO18.Common;

namespace MorSun.WX.ZYB.Service
{
    public class WXLgionService
    {        

        #region 返回一键登录链接
        /// <summary>
        /// 提问返回数据处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private IResponseMessageBase WXLgionResponse<T>(T requestMessage, string id)
            where T : RequestMessageBase
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage); //CreateResponseMessage<ResponseMessageNews>();
            var comonservice = new CommonService();
            
            responseMessage.Articles.Add(new Article()
            {
                Title = "微信快捷登录隧道",
                Description = "请在一分钟内点击穿越，穿越后该隧道自动失效！",
                PicUrl = CFG.网站域名 + "/images/zyb/wxlogin.png",
                Url = CFG.网站域名 + CFG.邦马网_微信一键登录路径 + "/" + id //model.PicUrl
            });
            
            return responseMessage;
            
        }
        #endregion        

        #region 一键登录
        /// <summary>
        /// 一键登录
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public IResponseMessageBase WXLgionResponseMessage(RequestMessageText requestMessage)
        {
            var id = Guid.NewGuid();
            CacheAccess.InsertToCacheByTime(id.ToString(), requestMessage.FromUserName, 60);
            return WXLgionResponse(requestMessage, id.ToString());       
        }
        #endregion


        
    }
}