﻿using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Web.Caching;
using MorSun.Bll;
using MorSun.Model;
using HOHO18.Common;
using MorSun.Common.配置;
using Senparc.Weixin.MP.Entities;
using MorSun.Common.类别;

namespace MorSun.WX.ZYB.Service
{
    /// <summary>
    /// 缓存类，缓存在线答题用户的题目(每个用户一个缓存)。用户回答问题时， 系统知道他回答的是哪个问题，以及缓存用户待答题和已答题数据。
    /// </summary>
    public static class UserQAService
    {
        private static string xmlSystemName = "XmlSystemName".GW();
        /// <summary>
        /// 用户答题缓存
        /// </summary>
        /// <param name="uid">传入的参数是 dt + uid uid是微信ID</param>
        /// <returns></returns>
        public static UserQACache GetUserQACache(string uid)
        {            
            //获取路径
            string path = System.Web.HttpContext.Current.Server.MapPath(xmlSystemName);

            //从缓存中读取
            var model = CacheAccess.GetFromCache(uid) as UserQACache;

            if (model == null)
            {
                CacheDependency fileDependency = new CacheDependency(path);

                var qaCache = new UserQACache();
                qaCache.WeiXinId = uid.Substring(2);
                //保存到缓存中
                CacheAccess.SaveToCacheByDependency(uid, qaCache, fileDependency);
                model = qaCache;
            }
            return model;
        }

        /// <summary>
        /// 设置用户答题缓存 当缓存的答题都完成时，要再设置缓存。每次答题也会触发设置缓存。
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="qaCache"></param>
        public static void SetUserQACache(string uid, UserQACache qaCache)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath(xmlSystemName);
            CacheDependency fileDependency = new CacheDependency(path);
            
            //保存到缓存中
            CacheAccess.SaveToCacheByDependency(uid, qaCache, fileDependency);
        }

        /// <summary>
        /// 获取在线答题用户缓存，这边只获取，为空时直接返回空。
        /// </summary>
        /// <param name="olineQAUserKey"></param>
        /// <returns></returns>
        public static OnlineQAUserCache GetOlineQAUserCache()
        {
            //获取路径
            string path = System.Web.HttpContext.Current.Server.MapPath(xmlSystemName);

            //从缓存中读取
            var model = CacheAccess.GetFromCache(CFG.在线答题用户缓存键) as OnlineQAUserCache;

            if (model == null)
            {
                //这边只获取缓存，不设置，设置缓存手动去设置，防止并发情况发生
                return null;
            }
            return model;
        }

        /// <summary>
        /// 微信手动设置缓存
        /// </summary>
        /// <param name="olineQAUserKey"></param>
        /// <param name="qaCache"></param>
        public static void SetOlineQAUserCache(OnlineQAUserCache qaCache)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath(xmlSystemName);
            CacheDependency fileDependency = new CacheDependency(path);

            //保存到缓存中
            CacheAccess.SaveToCacheByDependency(CFG.在线答题用户缓存键, qaCache, fileDependency);
        }       

        /// <summary>
        /// 添加或更新用户 在线答题状态
        /// </summary>
        /// <param name="uwx"></param>
        /// <param name="requestMessage"></param>
        public static void AddOrUpdateOnlineQAUser(bmUserWeixin uwx, RequestMessageText requestMessage)
        {
            //判断在线答题用户是否存在该用户
            var bll = new BaseBll<bmOnlineQAUser>();
            var state = Guid.Parse(Reference.在线状态_在线);
            var oqau = bll.All.FirstOrDefault(p => p.WeiXinId == uwx.WeiXinId && p.State == state);

            var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString();
            var rqid = Guid.NewGuid();

            var commonService = new CommonService();
            Guid mid = commonService.GetMsgIdCache(msgid);
            if (mid == Guid.Empty)
            {
                //设置用户消息缓存
                commonService.SetMsgIdCache(msgid, rqid);
                if(oqau == null)
                {//用户不在线时                             
                    var model = new bmOnlineQAUser();                
                    model.ID = rqid;
                    model.UserId = uwx.UserId;
                    model.WeiXinId = uwx.WeiXinId;
                    model.AQStartTime = DateTime.Now;
                    model.State = state;
                    model.ActiveTime = DateTime.Now;
                    //认证级别
                    var uinfo = new BaseBll<wmfUserInfo>().GetModel(uwx.UserId);
                    model.CertificationLevel = uinfo == null ? null : uinfo.CertificationLevel;

                    if(commonService.GetMsgIdCache(msgid) == model.ID)
                    {
                        bll.Insert(model);
                    }
                }
                else
                {//用户已经在线时,更新活跃时间
                    oqau.ActiveTime = DateTime.Now;
                    if (commonService.GetMsgIdCache(msgid) == rqid)
                    {
                        bll.Update(oqau);
                    }
                }
            }
        }






    }
}