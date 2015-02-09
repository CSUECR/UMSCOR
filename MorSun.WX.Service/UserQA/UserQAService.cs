using System;
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
using HOHO18.Common.WEB;

namespace MorSun.WX.ZYB.Service
{
    /// <summary>
    /// 缓存类，缓存在线答题用户的题目(每个用户一个缓存)。用户回答问题时， 系统知道他回答的是哪个问题，以及缓存用户待答题和已答题数据。
    /// </summary>
    public static class UserQAService
    {
        private static string xmlSystemName = "XmlSystemName".GW();

        #region 用户答题缓存
        /// <summary>
        /// 用户答题缓存
        /// </summary>
        /// <param name="uid">传入的参数是 dt + uid uid是微信ID</param>
        /// <returns></returns>
        public static UserQACache GetUserQACache(string uid)
        {
            LogHelper.Write(("获取用户答题缓存方法的uid " + uid), LogHelper.LogMessageType.Debug);
            //获取路径
            //string path = System.Web.HttpContext.Current.Server.MapPath(xmlSystemName);
            var model = new UserQACache();
            try { 
                //从缓存中读取  不在这边设置缓存是为了防止并发
                model = CacheAccess.GetFromCache(uid) as UserQACache;
            }
            catch
            {
                LogHelper.Write(("获取用户答题缓存方法异常"), LogHelper.LogMessageType.Debug);
            }
            LogHelper.Write(("完成获取用户答题缓存方法的uid " + uid), LogHelper.LogMessageType.Debug);
            if (model == null)
            {
                //这边只获取缓存，不设置，设置缓存手动去设置，防止并发情况发生
                LogHelper.Write(("用户答题缓存返回空"), LogHelper.LogMessageType.Debug);
                return null;
            }
            LogHelper.Write(("存在用户答题缓存并返回"), LogHelper.LogMessageType.Debug);
            return model;
        }

        /// <summary>
        /// 初始化用户答题缓存，在用户答题缓存为空，或用户待答题数量为0，或用户待答题数量与已答题数量一致时调用
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static UserQACache InitUserQACache(string uid,bool setCache)
        {     
            var model = new UserQACache();
            var weixinId = uid.Substring(2);
            model.WeiXinId = weixinId;
            //头一次取时，系统自动将待答问题和待处理的分配项放进缓存
            var djdRef = Guid.Parse(Reference.分配答题操作_待解答);
            //待回答的问题
            model.WaitQA = new BaseBll<bmQAView>().All.Where(p => p.DisWeiXinId == weixinId && p.Result == djdRef).ToList();
            LogHelper.Write("用户待答问题获取", LogHelper.LogMessageType.Debug);
            //待处理的分配项,每次答题都要再去取，还是直接根据问题ID和weixinid取分配项，不然对缓存的操作太麻烦
            //qaCache.WaitQADis = new BaseBll<bmQADistribution>().All.Where(p => p.WeiXinId == weixinId && p.Result == djdRef);
            //待答题有数据时，才设置当前答题，没有就不设置，当前答题是否为空，由返回方法决定返回什么
            if (model.WaitQA != null && model.WaitQA.Count() > 0) 
                model.CurrentQA = model.WaitQA.FirstOrDefault();//初始化时，取的就是第一个
            //保存到缓存中
            if(setCache)
            {
                LogHelper.Write("用户答题缓存设置", LogHelper.LogMessageType.Debug);
                SetUserQACache(uid, model);
            }            
            return model;            
        }

        /// <summary>
        /// 设置用户答题缓存 每次答题时触发设置缓存。
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
        #endregion

        #region 在线用户缓存
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
            LogHelper.Write("设置缓存" + path, LogHelper.LogMessageType.Debug);
            //保存到缓存中
            CacheAccess.SaveToCacheByDependency(CFG.在线答题用户缓存键, qaCache, fileDependency);
        }       

        /// <summary>
        /// 添加或更新用户 在线答题状态
        /// </summary>
        /// <param name="uwx"></param>
        /// <param name="requestMessage"></param>
        public static void AddOrUpdateOnlineQAUser<T>(T requestMessage, bmUserWeixin uwx, Guid rqid)
            where T : RequestMessageBase
        {            
            var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString();
            //var rqid = Guid.NewGuid();

            var commonService = new CommonService();
            //Guid mid = commonService.GetMsgIdCache(msgid);
            //if (mid == Guid.Empty)
            //{
            //    //设置用户消息缓存
            //    commonService.SetMsgIdCache(msgid, rqid);
            //}
            if (commonService.GetMsgIdCache(msgid) == rqid)
            {
                //判断在线答题用户是否存在该用户
                var bll = new BaseBll<bmOnlineQAUser>();
                var state = Guid.Parse(Reference.在线状态_在线);
                var curWeiXinAPP = Guid.Parse(CFG.邦马网_当前微信应用);
                var oqau = bll.All.Where(p => p.WeiXinAPP != null && p.WeiXinAPP == curWeiXinAPP && p.WeiXinId == uwx.WeiXinId && p.State == state && p.FlagTrashed == false).OrderByDescending(p => p.AQStartTime).FirstOrDefault();

                if (oqau == null)
                {//用户不在线时 
                    LogHelper.Write("添加用户到在线答题方法", LogHelper.LogMessageType.Debug);
                    var model = new bmOnlineQAUser();
                    model.ID = Guid.NewGuid();
                    model.UserId = uwx.UserId;
                    model.WeiXinId = uwx.WeiXinId;
                    model.AQStartTime = DateTime.Now;
                    model.State = state;
                    model.ActiveTime = DateTime.Now;
                    //认证级别
                    var uinfo = new BaseBll<wmfUserInfo>().GetModel(uwx.UserId);
                    model.CertificationLevel = uinfo == null ? null : uinfo.CertificationLevel;
                    model.WeiXinAPP = curWeiXinAPP;

                    model.RegTime = DateTime.Now;
                    model.ModTime = DateTime.Now;
                    model.FlagTrashed = false;
                    model.FlagDeleted = false;

                    bll.Insert(model);
                }
                else
                {//用户已经在线时,更新活跃时间
                    LogHelper.Write("更新用户活跃时间方法", LogHelper.LogMessageType.Debug);
                    oqau.ActiveTime = DateTime.Now;
                    oqau.ModTime = DateTime.Now;

                    //各种原因还出现两条记录时，
                    var allOqau = bll.All.Where(p => p.WeiXinAPP != null && p.WeiXinAPP == curWeiXinAPP && p.WeiXinId == uwx.WeiXinId && p.State == state && p.FlagTrashed == false);
                    if (allOqau.Count() > 1)
                    {
                        var currentOqau = bll.All.Where(p => p.ID == oqau.ID);
                        var otherOqau = allOqau.Except(currentOqau);
                        if (otherOqau.Count() > 0)
                        {
                            var tqState = Guid.Parse(Reference.在线状态_退出);
                            foreach (var item in otherOqau)
                            {
                                item.State = tqState;
                                item.AQEndTime = DateTime.Now;
                                item.FlagTrashed = true;
                            }
                        }
                    }
                    
                    bll.Update(oqau);                    
                }
            }//commonService.GetMsgIdCache(msgid) == rqid
            else
            {
                System.Threading.Thread.Sleep(1000);//其他访问等1秒
            }
        }
        #endregion
    }
}