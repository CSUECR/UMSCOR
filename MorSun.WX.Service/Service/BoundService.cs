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
using HOHO18.Common;

namespace MorSun.WX.ZYB.Service
{
    public class BoundService
    {
        /// <summary>
        /// 绑定返回指令处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private IResponseMessageBase BoundResponse(RequestMessageText requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage);

            responseMessage.Articles.Add(new Article()
            {//眼睛图片
                Title = "您的账号已经绑定邦马网",
                Description = "绑定时间" + DateTime.Now + "\r\n提问请直接发送拍照的问题图片\r\n答题可发送答题命令：dt",
                PicUrl = CFG.网站域名 + "/images/zyb/reg.jpg",
                Url = CFG.网站域名
            });

            //判断用户是否绑定，未绑定显示注册账号并绑定，已经绑定显示分享链接
            //new CommonService().RegOrShare<RequestMessageText>(requestMessage, responseMessage);

            return responseMessage;
        }

        /// <summary>
        /// 绑定指令处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public IResponseMessageBase UserBoundResponseMessage(RequestMessageText requestMessage)
        {
            //微信并发处理
            var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString();
            var commonService = new CommonService();
            var rqid = Guid.NewGuid();

            var bll = new BaseBll<bmUserWeixin>();
            var model = new bmUserWeixin();            
            Guid mid = commonService.GetMsgIdCache(msgid);
            if (mid == Guid.Empty)
            {
                commonService.SetMsgIdCache(msgid, rqid);
            }// mid
            //绑定指令处理 以下都是
            var text = requestMessage.Content;
            var boundCode = 0;
            if (text.Contains(" "))
            {//取绑定码
                try
                {
                    var commond = text.Substring(0, text.IndexOf(" "));
                    var numValue = text.Substring(commond.Length + 1, text.Length - commond.Length - 1).Replace(" ", "");
                    boundCode = Convert.ToInt32(numValue);
                }
                catch
                {
                    return new InvalidCommondService().GetInvalidCommondResponseMessage(requestMessage as RequestMessageText);
                }
            }
            else
            {
                return new InvalidCommondService().GetInvalidCommondResponseMessage(requestMessage as RequestMessageText);
            }

            var ubcc = GetUserBoundCodeCache(boundCode);
            if (boundCode == 0 || ubcc == null)
                return new InvalidCommondService().GetInvalidCommondResponseMessage(requestMessage as RequestMessageText);
            else
            {//以上判断的是命令是否出错，和缓存是否有指令
                //已经绑定的用户不再操作绑定
                var curWeiXinAPP = Guid.Parse(CFG.邦马网_当前微信应用);                     
                if (commonService.GetZYBUserByWeiXinId(requestMessage.FromUserName) != null)
                {//用户重复发送绑定的情况
                    return BoundResponse(requestMessage);
                }
                else if (commonService.GetZybUserByUserId(ubcc.UserId) != null)
                {
                    return commonService.CustomResponse(requestMessage, "该用户已经被绑定", "该用户已经被绑定", "", CFG.网站域名);
                }
                else
                {
                    //判断缓存里保存的问答ID是否是当前的对象ID    
                    if (commonService.GetMsgIdCache(msgid) == rqid)
                    {
                        model.ID = Guid.NewGuid();
                        model.UserId = ubcc.UserId;
                        model.WeiXinId = requestMessage.FromUserName;
                        model.WeiXinAPP = curWeiXinAPP;

                        model.RegTime = DateTime.Now;
                        model.ModTime = DateTime.Now;
                        model.FlagTrashed = false;
                        model.FlagDeleted = false;
                        bll.Insert(model);
                        //释放资源
                        CacheAccess.RemoveCache(CFG.微信绑定前缀 + boundCode);
                        CacheAccess.RemoveCache(CFG.微信绑定前缀 + model.UserId.ToString());

                        //绑定微信互相赠送邦马币                        
                        var addMBR = new AddMBRModel();
                        var user = new BaseBll<wmfUserInfo>().All.Where(p => p.ID == ubcc.UserId).FirstOrDefault();
                        addMBR.UIds.Add(user.ID);
                        if (user.InviteUser != null)
                            addMBR.UIds.Add(user.InviteUser.Value);

                        addMBR.SR = Guid.Parse(Reference.马币来源_赠送);
                        addMBR.MBR = Guid.Parse(Reference.马币类别_邦币);
                        addMBR.MBN = 1000;
                        commonService.AddUMBR(addMBR,ubcc.UserId, true);
                    }
                }
            }
            
            
            //增加数据获取限制，如果等了7秒还未取到值，则不再取对象

            int i = 0;
            //为了取自增长ID
            do
            {
                if (commonService.GetMsgIdCache(msgid) != rqid)
                {
                    System.Threading.Thread.Sleep(500);
                    i++;
                }
                model = commonService.GetZYBUserByWeiXinId(requestMessage.FromUserName);
            } while (model == null || i > 20);
            //执行后还是为空
            if (model == null)
                return new InvalidCommondService().GetInvalidCommondResponseMessage(requestMessage as RequestMessageText);
            return BoundResponse(requestMessage);
        }
        

        /// <summary>
        /// 根据绑定代码取要绑定的用户
        /// </summary>
        /// <param name="boundCode"></param>
        /// <returns></returns>
        private UserBoundCodeCache GetUserBoundCodeCache(int boundCode)
        {
            var key = CFG.微信绑定前缀 + boundCode.ToString();
            return CacheAccess.GetFromCache(key) as UserBoundCodeCache;
        }
    }
}