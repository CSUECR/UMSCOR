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
using MorSun.Common.类别;
using HOHO18.Common.WEB;

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
            var userWeiXin = GetZYBUserByWeiXinId(requestMessage.FromUserName);
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
                    Title = "分享给小伙伴们",
                    Description = "分享给小伙伴们",
                    PicUrl = "",
                    Url = CFG.网站域名
                });
            }
        }

        /// <summary>
        /// 未找到对象的返回内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestMessage"></param>
        /// <param name="responseMessage"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="picurl"></param>
        /// <param name="url"></param>
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
        /// 设置消息ID的头一次请求ID
        /// </summary>
        /// <param name="msgid"></param>
        /// <param name="qaid"></param>
        public void SetMsgIdCache(string msgid, Guid qaid)
        {
            //保存到缓存中
            CacheAccess.AddToCacheByTime(msgid, qaid, 10);
        }

        public string GetUserRQLimCache(string userRQKey)
        {
            var ms = CacheAccess.GetFromCache(userRQKey);
            if (ms == null)
                return "";
            return ms.ToString();
        }

        /// <summary>
        /// 设置用户并发请求时间 间隔缓存
        /// </summary>
        /// <param name="userRQKey"></param>
        /// <param name="msgid"></param>
        public void SetUserRQLimCache(string userRQKey, string msgid)
        {
            var t = Convert.ToInt32(CFG.用户连续请求时间间隔);
            //保存到缓存中
            CacheAccess.AddToCacheByTime(userRQKey, msgid, t);
            LogHelper.Write((msgid + " 添加缓存到 " + userRQKey), LogHelper.LogMessageType.Debug);
        }

        /// <summary>
        /// 获取作业邦绑定用户
        /// </summary>
        /// <param name="userWeiXinId"></param>
        /// <returns></returns>
        public bmUserWeixin GetZYBUserByWeiXinId(string userWeiXinId)
        {
            var wxyy = Guid.Parse(CFG.邦马网_当前微信应用);
            if (!String.IsNullOrEmpty(userWeiXinId))
                return new BaseBll<bmUserWeixin>().All.Where(p => p.WeiXinId == userWeiXinId && p.WeiXinAPP == wxyy).FirstOrDefault();
            else
                return null;
        }

        /// <summary>
        /// 根据用户ID取作业邦绑定用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bmUserWeixin GetZybUserByUserId(Guid userId)
        {
            var wxyy = Guid.Parse(CFG.邦马网_当前微信应用);            
            return new BaseBll<bmUserWeixin>().All.Where(p => p.UserId == userId && p.WeiXinAPP == wxyy).FirstOrDefault();            
        }

        /// <summary>
        /// 返回任意内容
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        /// <param name="picurl"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public ResponseMessageNews CustomResponse(RequestMessageText requestMessage,string title,string desc,string picurl,string url)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage);

            responseMessage.Articles.Add(new Article()
            {//眼睛图片
                Title = title,
                Description = desc,
                PicUrl = picurl,
                Url = url
            });
            return responseMessage;
        }

        /// <summary>
        /// 赠送邦马币
        /// </summary>
        /// <param name="addMBR"></param>
        /// <param name="updateChange"></param>
        public void AddUMBR(AddMBRModel addMBR, Guid? uid, bool updateChange = true)
        {
            var rbll = new BaseBll<bmUserMaBiRecord>();
            //检测用户是否存在
            var users = new BaseBll<aspnet_Users>().All.Where(p => addMBR.UIds.Contains(p.UserId));//找得到userId 就添加
            foreach (var u in users)
            {
                var model = new bmUserMaBiRecord();
                model.SourceRef = addMBR.SR;
                model.MaBiRef = addMBR.MBR;
                model.MaBiNum = addMBR.MBN;
                model.IsSettle = false;

                model.RegTime = DateTime.Now;
                model.ModTime = DateTime.Now;
                model.FlagTrashed = false;
                model.FlagDeleted = false;

                model.ID = Guid.NewGuid();
                model.UserId = u.UserId;
                if (uid != null)
                    model.RegUser = uid;
                else
                    model.RegUser = u.UserId;
                rbll.Insert(model, false);
            }
            if (updateChange)
                rbll.UpdateChanges();
        }
    }
}