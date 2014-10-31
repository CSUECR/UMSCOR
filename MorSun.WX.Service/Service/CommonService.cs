using System;
using System.Linq;
using System.Collections.Generic;
using Senparc.Weixin.MP.Entities;

using Senparc.Weixin.MP.Helpers;
using MorSun.Bll;
using MorSun.Model;
using MorSun.Common.配置;
using HOHO18.Common.SSO;
using HOHO18.Common;

namespace MorSun.WX.ZYB.Service
{
    public class CommonService
    { 
        /// <summary>
        /// 通过id获取其itemValue
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetReferenceValue(Guid? guid)
        {
            var resultValue = string.Empty;
            if (guid != null && guid != Guid.Empty)
            {
                var referenceModel = new BaseBll<wmfReference>().GetModel(guid);
                if (referenceModel != null)
                {
                    resultValue = referenceModel.ItemValue;
                }
            }
            return resultValue;
        }

        /// <summary>
        /// 绑定与分享链接
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="responseMessage"></param>
        public void RegOrShare<T>(T requestMessage, ResponseMessageNews responseMessage)
            where T : RequestMessageBase
        {
            var userWeiXin = GetUserByWeiXinId(requestMessage.FromUserName);
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
        }

        public void NonObject<T>(T requestMessage, ResponseMessageNews responseMessage,string title,string description,string picurl,string url)
            where T : RequestMessageBase
        {
            responseMessage.Articles.Add(new Article()
            {//问号图片
                Title = title,
                Description = description,
                PicUrl = picurl,
                Url = CFG.网站域名 + url
            });
        }

        /// <summary>
        /// 根据消息ID获取问答的ID值
        /// </summary>
        /// <param name="msgid"></param>
        /// <returns></returns>
        public Guid GetMsgIdCache(string msgid)
        {
            Guid gqaid = Guid.Empty;
            //从缓存中读取
            var qaid = CacheAccess.GetFromCache(msgid);
            if (qaid != null)
                gqaid = Guid.Parse(qaid.ToString());
            return gqaid;
        }

        /// <summary>
        /// 设置消息ID的问答ID
        /// </summary>
        /// <param name="msgid"></param>
        /// <param name="qaid"></param>
        public void SetMsgIdCache(string msgid, Guid qaid)
        {
            //保存到缓存中
            CacheAccess.SaveToCacheByTime(msgid, qaid, 20);
        }

        /// <summary>
        /// 获取绑定用户
        /// </summary>
        /// <param name="userWeiXinId"></param>
        /// <returns></returns>
        public bmUserWeixin GetUserByWeiXinId(string userWeiXinId)
        {
            if (!String.IsNullOrEmpty(userWeiXinId))
                return new BaseBll<bmUserWeixin>().All.Where(p => p.WeiXinId == userWeiXinId).FirstOrDefault();
            else
                return null;
        }
    }
}