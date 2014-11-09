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
using MorSun.Common.常量集;
using HOHO18.Common;
using HOHO18.Common.WEB;

namespace MorSun.WX.ZYB.Service
{
    public class AnswerService
    {

        #region 请求开始处理
        private void RQStart(RequestMessageText requestMessage, Guid rqid, CommonService commonService)
        {
            var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString();            
            Guid mid = commonService.GetMsgIdCache(msgid);
            if (mid == Guid.Empty)
            {
                LogHelper.Write((msgid + " 设置消息缓存，防止操作并发 " + rqid), LogHelper.LogMessageType.Debug);
                //设置用户消息缓存
                commonService.SetMsgIdCache(msgid, rqid);
            }
        }
        #endregion

        #region 答题返回数据处理
        /// <summary>
        /// 提问返回数据处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private ResponseMessageNews AnswerResponse<T>(T requestMessage, bmQA model)
            where T : RequestMessageBase
        {
            LogHelper.Write("返回待答问题", LogHelper.LogMessageType.Debug);
            if(model == null)
            { //传过来是空值时，返回系统资源分配中
                return NonDistributionResponse(requestMessage); 
            }               
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage); //CreateResponseMessage<ResponseMessageNews>();
            var comonservice = new CommonService();
            responseMessage.Articles.Add(new Article()
            {
                Title = ("问题编号：" + model.AutoGrenteId + " ") + ((model.MaBiNum == 0 || model.MaBiNum == null) ? "免费提问" : ("消耗" + (model.MaBiNum == null ? "0" : model.MaBiNum.ToString("f0") + comonservice.GetReferenceValue(model.MaBiRef)))),
                Description = ((model.MaBiNum == 0 || model.MaBiNum == null) ? "免费提问" : ("消耗" + (model.MaBiNum == null ? "0" : model.MaBiNum.ToString() + comonservice.GetReferenceValue(model.MaBiRef)))) + (" 问题编号：" + model.AutoGrenteId),
                PicUrl = model.PicUrl,
                Url = model.PicUrl
            });
            responseMessage.Articles.Add(new Article()
            {//眼睛图片
                Title = "放弃本题请发送:" + " " + CFG.放弃本题,
                Description = "放弃本题请发送:" + " " + CFG.放弃本题,
                PicUrl = "",
                Url = ""
            });//再增加 加码 求解题思路            
            responseMessage.Articles.Add(new Article()
            {//美元图片
                Title = "这不是一个问题请发送：" + " " + CFG.不是问题,
                Description = "这不是一个问题请发送：" + " " + CFG.不是问题,
                PicUrl = "",
                Url = ""
            });
            responseMessage.Articles.Add(new Article()
            {//问号图片
                Title = "本题提问时间:" + (model.RegTime == null ? "" : (model.RegTime.ToShortDateString() + " " + model.RegTime.Value.ToShortTimeString())),
                Description = "本题提问时间:" + (model.RegTime == null ? "" : (model.RegTime.ToShortDateString() + " " + model.RegTime.Value.ToShortTimeString())),
                PicUrl = "",
                Url = ""
            });
            responseMessage.Articles.Add(new Article()
            {//问号图片
                Title = "本题获取时间:" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(),
                Description = "本题获取时间:" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(),
                PicUrl = "",
                Url = ""
            });
            responseMessage.Articles.Add(new Article()
            {//问号图片
                Title = "当前未答题数:   " + model.DJDCount,
                Description = "当前未答题数： " + model.DJDCount,
                PicUrl = "",
                Url = ""
            });
            responseMessage.Articles.Add(new Article()
            {//美元图片
                Title = "退出答题请发送：" + " " + CFG.退出答题,
                Description = "退出答题请发送：" + " " + CFG.退出答题,
                PicUrl = "",
                Url = ""
            });
            return responseMessage;
        }

        /// <summary>
        /// 答题资源未分配
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private ResponseMessageNews NonDistributionResponse<T>(T requestMessage)
            where T : RequestMessageBase
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage);             
            responseMessage.Articles.Add(new Article()
            {
                Title = "正在为您分配答题资源，请稍候再尝试发送： " + CFG.开始答题 + " 开始答题",
                Description = "正在为您分配答题资源，请稍候再尝试发送： " + CFG.开始答题 + " 开始答题",
                PicUrl = "",
                Url = ""
            });
            
            responseMessage.Articles.Add(new Article()
            {//问号图片
                Title = "查看分配规则",
                Description = "查看分配规则",
                PicUrl = "",
                Url = CFG.网站域名 + "DistributionRule".GX()
            });

            //当前答题缓存数据
            var model = UserQAService.GetOlineQAUserCache();
            if(model != null)
            {
                responseMessage.Articles.Add(new Article()
                {//问号图片
                    Title = ("当前未答数(收费：" + model.MaBiQACount + " 免费：" + model.NonMaBiQACount + ")"),
                    Description = "查看分配规则",
                    PicUrl = "",
                    Url = CFG.网站域名 + "DistributionRule".GX()
                });
                responseMessage.Articles.Add(new Article()
                {//问号图片
                    Title = ("当前在线人数(认证：" + model.CertificationUser.Count() + " 未认证：" + model.NonCertificationQAUser.Count() + ")"),
                    Description = ("当前在线人数"),
                    PicUrl = "",
                    Url = CFG.网站域名 + "DistributionRule".GX()
                });                
            }

            return responseMessage;
        }

        /// <summary>
        /// 拒绝答题服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private ResponseMessageNews RefusedAnswerResponse<T>(T requestMessage)
            where T : RequestMessageBase
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage);
            responseMessage.Articles.Add(new Article()
            {
                Title = "系统拒绝了您的答题请求",
                Description = "系统拒绝了您的答题请求",
                PicUrl = "",
                Url = ""
            });

            responseMessage.Articles.Add(new Article()
            {//问号图片
                Title = "查看拒绝答题请求规则",
                Description = "查看拒绝答题请求规则",
                PicUrl = "",
                Url = CFG.网站域名 + "RefusedAnswer".GX()
            });
            return responseMessage;
        } 
        #endregion

        #region 需要返回的问题
        /// <summary>
        /// 包装当前答题 设置待答题数量
        /// </summary>
        /// <param name="requestMessage"></param>
        private bmQA PackCurrentQA<T>(T requestMessage, UserQACache model)
            where T : RequestMessageBase
        {
            LogHelper.Write("包装当前答题开始", LogHelper.LogMessageType.Debug);
            if (model == null)
                return null;
            else
            {
                LogHelper.Write("答题缓存不为空时的包装当前答题", LogHelper.LogMessageType.Debug);
                if (model.CurrentQA == null)
                    return null;
                else if (model.CurrentQA != null && model.WaitQA != null)
                {
                    //有已回答列表的方法
                    //if (model.AlreadyQA != null)
                    //{ model.CurrentQA.DJDCount = model.WaitQA.Count() - model.AlreadyQA.Count(); }
                    //else
                    //{
                    //model.CurrentQA.DJDCount = model.WaitQA.Count(); 
                    //} 
                    model.CurrentQA.DJDCount = model.WaitQA.Count(); 
                }
                LogHelper.Write("准备返回", LogHelper.LogMessageType.Debug);
                //是不是为空由下一步返回的代码再判断
                return model.CurrentQA;
            }
        }

        /// <summary>
        /// 答题缓存更新操作，调用前提是对当前问题操作过了
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestMessage"></param>
        /// <param name="rqid"></param>
        /// <param name="model"></param>
        /// <param name="commonService"></param>
        private UserQACache RefreshQACache<T>(T requestMessage, Guid rqid, UserQACache model, CommonService commonService)
            where T : RequestMessageBase
        {            
            var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString();
            var qakey = CFG.用户待答题缓存键前缀 + model.WeiXinId;
            var cid = model.CurrentQA == null ? Guid.Empty : model.CurrentQA.ID;
            //当前答题为空
            LogHelper.Write((rqid + "刷新缓存操作开始" + cid), LogHelper.LogMessageType.Debug);
            //有已回答列表的处理
            //if (commonService.GetMsgIdCache(msgid) == rqid)
            //{
            //    if(model.CurrentQA != null)
            //    {//将操作前的当前问题加入到已答题列表
            //        LogHelper.Write("问题添加进已答问题", LogHelper.LogMessageType.Debug);
            //        if (model.AlreadyQA == null)
            //            model.AlreadyQA = new List<bmQA>();
            //        model.AlreadyQA.Add(model.CurrentQA);
            //        LogHelper.Write("已答问题添加了已处理的问题", LogHelper.LogMessageType.Debug);
            //    }
            //    LogHelper.Write(("已答题是否为空" + (model.AlreadyQA == null).ToString() + " " + (model.AlreadyQA.Count() == 0).ToString()), LogHelper.LogMessageType.Debug);    
            //    if (model.AlreadyQA == null || model.AlreadyQA.Count() == 0)
            //    {
            //        LogHelper.Write("已答题为空时，设置当前答题", LogHelper.LogMessageType.Debug);
            //        model.CurrentQA = model.WaitQA.OrderBy(p => p.RegTime).FirstOrDefault(); 
            //    }                   
            //    else
            //    {
            //        //已答题数量与待答题数量一致时
            //        if (model.WaitQA.Count() == model.AlreadyQA.Count())
            //        {   //再初始化缓存
            //            LogHelper.Write("待答题都回答完后，初始化答题缓存", LogHelper.LogMessageType.Debug);
            //            model = UserQAService.InitUserQACache(qakey, false);
            //            //设置当前答题的代码放到设置缓存方法去
            //        }
            //        else
            //        {
            //            LogHelper.Write("待答题未回答完时，设置当前答题", LogHelper.LogMessageType.Debug);
            //            //已答题有数据时，排除掉已答题后再取值                        
            //            model.CurrentQA = model.WaitQA.Except(model.AlreadyQA).OrderBy(p => p.RegTime).FirstOrDefault();
            //            LogHelper.Write("完成设置当前答题", LogHelper.LogMessageType.Debug);
            //        }
            //    }
            if (commonService.GetMsgIdCache(msgid) == rqid)
            {
                if (model.CurrentQA != null)
                {//将操作前的当前问题加入到已答题列表
                    LogHelper.Write("待答问题移除当前答题", LogHelper.LogMessageType.Debug);                    
                    model.WaitQA.Remove(model.CurrentQA);
                    LogHelper.Write("完成待答问题移除当前答题", LogHelper.LogMessageType.Debug);
                }                
                
                //已答题数量与待答题数量一致时
                if (model.WaitQA.Count() == 0)
                {   //再初始化缓存
                    LogHelper.Write("待答题都回答完后，初始化答题缓存", LogHelper.LogMessageType.Debug);
                    model = UserQAService.InitUserQACache(qakey, false);
                    //设置当前答题的代码放到设置缓存方法去
                }
                else
                {
                    LogHelper.Write("待答题未回答完时，设置当前答题", LogHelper.LogMessageType.Debug);
                    //已答题有数据时，排除掉已答题后再取值                        
                    model.CurrentQA = model.WaitQA.FirstOrDefault();
                    LogHelper.Write("完成设置当前答题", LogHelper.LogMessageType.Debug);
                }
                
                //到这里，不管当前答题是否为空都要重新设置缓存
                LogHelper.Write("设置当前答题缓存", LogHelper.LogMessageType.Debug);
                UserQAService.SetUserQACache(qakey, model);
                return model;
            }
            else
            {
                LogHelper.Write("刷新缓存中非主线程直接取答题", LogHelper.LogMessageType.Debug);
                int i = 0;
                //为了取自增长ID
                do
                {                    
                    System.Threading.Thread.Sleep(500);
                    i++;
                    model = UserQAService.GetUserQACache(qakey);
                } while ((model.CurrentQA.ID != cid) || i > 20);
                return model;
            }
        }

        #endregion
               
        #region 用户开始答题 输入答题命令
        /// <summary>
        /// 用户输入答题命令处理  获取题目前的操作，判断缓存是否存在该用户
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public ResponseMessageNews StartAnswerResponseMessage(RequestMessageText requestMessage)
        {
            var commonService = new CommonService();
            //未绑定的用户录入答题命令的处理
            var userWeiXin = commonService.GetZYBUserByWeiXinId(requestMessage.FromUserName);
            if(userWeiXin == null)
            {
                return new UnboundService().GetUnboundResponseMessage(requestMessage);
            }
            else
            { 
                //连续答退的用户处理
                var dt = DateTime.Now.AddHours(0 - Convert.ToInt32(CFG.连续答退时间间隔));
                var userOnlineCount = new BaseBll<bmOnlineQAUser>().All.Where(p => p.AQEndTime >= dt && p.FlagTrashed == false).Count();
                if (userOnlineCount >= 5)
                    return RefusedAnswerResponse(requestMessage);
                //连续答退的用户处理结束

                //已经绑定的用户处理       
                var onlineuserCache = UserQAService.GetOlineQAUserCache();
                //处理并发而生成的操作唯一ID
                var rqid = Guid.NewGuid();
                RQStart(requestMessage, rqid, commonService);
                if (onlineuserCache == null)
                {   //缓存未设置的情况
                    //将用户添加或更新进数据库，由统一方法设置缓存
                    LogHelper.Write("添加用户到在线答题", LogHelper.LogMessageType.Debug);
                    UserQAService.AddOrUpdateOnlineQAUser(requestMessage, userWeiXin, rqid);
                    //返回答题资源分配中，稍候再发送答题命令
                    return NonDistributionResponse(requestMessage);
                }
                else
                { 
                    //更新用户活跃时间 将用户添加或更新进数据库，由统一方法设置缓存
                    UserQAService.AddOrUpdateOnlineQAUser(requestMessage, userWeiXin, rqid);
                    LogHelper.Write("更新用户活跃时间", LogHelper.LogMessageType.Debug);
                    if (userWeiXin.aspnet_Users1.wmfUserInfo != null && userWeiXin.aspnet_Users1.wmfUserInfo.CertificationLevel != null && ConstList.DTCertificationLevel.Contains(userWeiXin.aspnet_Users1.wmfUserInfo.CertificationLevel))
                    {//认证用户处理
                        if(onlineuserCache.CertificationUser != null && onlineuserCache.CertificationUser.FirstOrDefault(p => p.WeiXinId == userWeiXin.WeiXinId) != null)
                        {
                            //在线认证用户缓存存在该用户的处理方式
                            //不管认证与未认证的用户，答题方法是一样的，只是在分配答题时，系统根据认证与未认证用户进行答题分配，分配好后，都是一样的从数据库中取数据答题
                            LogHelper.Write("进入认证答题", LogHelper.LogMessageType.Debug);
                            //在线缓存存在当前答题用户，进去下一步获取题目操作
                            return GetAnswerResponse(requestMessage, rqid);
                        }
                        else
                        {
                            //认证用户未进缓存
                        
                            //返回答题资源分配中，稍候再发送答题命令
                            return NonDistributionResponse(requestMessage);
                        }
                    }
                    else
                    {//不管什么原因的非认证用户处理
                        if(onlineuserCache.NonCertificationQAUser != null && onlineuserCache.NonCertificationQAUser.FirstOrDefault(p => p.WeiXinId == userWeiXin.WeiXinId) != null)
                        {
                            //在线未认证用户缓存存在该用户的处理方式
                            LogHelper.Write("进入非认证答题", LogHelper.LogMessageType.Debug);
                            //在线缓存存在当前答题用户，进去下一步获取题目操作
                            return GetAnswerResponse(requestMessage, rqid);
                        }
                        else
                        {
                            //未认证用用户未进缓存
                            
                            //返回答题资源分配中，稍候再发送答题命令
                            return NonDistributionResponse(requestMessage);
                        }
                    }
                    
                }
            }
        }

        /// <summary>
        /// 在线用户缓存存在当前用户时，取当前用户的答题缓存，为空时设置答题缓存。
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private ResponseMessageNews GetAnswerResponse(RequestMessageText requestMessage, Guid rqid)
        {
            LogHelper.Write("进入GetAnswerResponse方法", LogHelper.LogMessageType.Debug);
            var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString();            
            var commonService = new CommonService(); 
            //RQStart(requestMessage, rqid, commonService);
            // 用户的答题缓存都由用户在答题是设置
            //从缓存中获取后，待答题数量为0的处理
            var qakey = CFG.用户待答题缓存键前缀 + requestMessage.FromUserName;
            LogHelper.Write(("答题KEY " + qakey), LogHelper.LogMessageType.Debug);
            var model = new UserQACache();
            try
            {
                LogHelper.Write(("答题KEY " + qakey + " 准备获取缓存"), LogHelper.LogMessageType.Debug);
                model = UserQAService.GetUserQACache(qakey);
            }
            catch
            {
                LogHelper.Write(("答题KEY " + qakey + " 获取缓存异常"), LogHelper.LogMessageType.Debug);
            }
            
            if(model == null)
            {
                LogHelper.Write("答题缓存为空时初始化答题缓存", LogHelper.LogMessageType.Debug);
                //无缓存或待答题数量为0，先取数据，如果数据库还没有待答题，则返回答题资源分配中
                //设置缓存微信并发时要处理
                if (commonService.GetMsgIdCache(msgid) == rqid)
                    model = UserQAService.InitUserQACache(qakey, true);                    
            }
            else if(model != null && (model.WaitQA == null || model.WaitQA.Count() == 0))
            {
                LogHelper.Write("待答题数量为0或为空时初始化缓存", LogHelper.LogMessageType.Debug);
                if (commonService.GetMsgIdCache(msgid) == rqid)
                    model = UserQAService.InitUserQACache(qakey, true);                
            }

            if (commonService.GetMsgIdCache(msgid) != rqid)
            {
                int i = 0;
                LogHelper.Write("答题缓存为空时非主线程等待获取缓存", LogHelper.LogMessageType.Debug);
                do
                {
                    System.Threading.Thread.Sleep(500);//其他访问等1秒
                    i++;
                    model = UserQAService.GetUserQACache(qakey);
                } while (model == null || i > 6);                
            }
            //还是为空，返回答题资源分配中
            if (model == null)
            {//返回答题资源分配中
                LogHelper.Write("答题缓存初始化后还是为空", LogHelper.LogMessageType.Debug);
                return NonDistributionResponse(requestMessage);
            }
            else if (model != null && (model.WaitQA == null || model.WaitQA.Count() == 0))
            {
                LogHelper.Write("答题缓存的待答题初始化后还是为空", LogHelper.LogMessageType.Debug);
                return NonDistributionResponse(requestMessage);
            }

            //从缓存中获取后，待答题数量与已答题数量一致时的处理
            //这种情况下，用户答题后系统要设置，首先，当前答题为空，其次待答题数量与已答题数量一致
            //经分析，用户输入dt命令时一般不会出现待答题与已答题数量一致的情况，
            LogHelper.Write("取到缓存，准备返回答题", LogHelper.LogMessageType.Debug);
            //从缓存中获取后，有可答题时的处理            
            return AnswerResponse(requestMessage, PackCurrentQA(requestMessage, model));            
        }        
        #endregion

        #region 用户操作问题  放弃 不是问题 回答
        /// <summary>
        /// 放弃问题
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public ResponseMessageNews OperateQuestionResponseMessage(RequestMessageText requestMessage, string operate)
        {
            var commonService = new CommonService();
            //未绑定的用户录入放弃本题的处理
            var userWeiXin = commonService.GetZYBUserByWeiXinId(requestMessage.FromUserName);
            if (userWeiXin == null)
            {
                return new UnboundService().GetUnboundResponseMessage(requestMessage);
            }
            else
            {
                //已经绑定的用户处理      
                var onlineuserCache = UserQAService.GetOlineQAUserCache();
                //处理并发而生成的操作唯一ID
                var rqid = Guid.NewGuid();
                RQStart(requestMessage, rqid, commonService);
                var ics = new InvalidCommondService();
                if (onlineuserCache == null)
                {
                    LogHelper.Write("操作问题，无在线用户缓存", LogHelper.LogMessageType.Debug);
                    //不是在线答题用户，直接返回无效命令 
                    return ics.GetInvalidCommondResponseMessage(requestMessage);
                }
                else
                {
                    //在线用户是否存在该用户
                    if (onlineuserCache.CertificationUser.FirstOrDefault(p => p.WeiXinId == requestMessage.FromUserName) == null
                        && onlineuserCache.NonCertificationQAUser.FirstOrDefault(p => p.WeiXinId == requestMessage.FromUserName) == null)
                    {
                        LogHelper.Write("操作问题，当前用户不在缓存里", LogHelper.LogMessageType.Debug);
                        //不是在线答题用户，直接返回无效命令 
                        return ics.GetInvalidCommondResponseMessage(requestMessage);
                    }

                    var qakey = CFG.用户待答题缓存键前缀 + requestMessage.FromUserName;
                    var model = UserQAService.GetUserQACache(qakey);
                    if (model == null || model.CurrentQA == null)
                    {
                        LogHelper.Write("操作问题，当前用户答题缓存为空或无当前答题", LogHelper.LogMessageType.Debug);
                        //用户答题缓存为空，
                        return ics.GetInvalidCommondResponseMessage(requestMessage);
                    }

                    //自行判断用户是否超期未操作  不用这个的原因是，可能会与定时更新缓存操作冲突
                    //var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString(); 
                    //var nondismn = 0 - Convert.ToInt32(CFG.疑似退出时间);
                    //var nondisdt = DateTime.Now.AddMinutes(nondismn);
                    //var state = Guid.Parse(Reference.在线状态_在线);
                    //var bll = new BaseBll<bmOnlineQAUser>();
                    //var curUserState = bll.All.FirstOrDefault(p => p.WeiXinId ==  requestMessage.FromUserName && p.State == state);
                    //if(curUserState == null || curUserState.ActiveTime < nondisdt)
                    //{
                    //    if (commonService.GetMsgIdCache(msgid) == rqid)
                    //    {
                    //        if(curUserState != null && curUserState.ActiveTime < nondisdt)
                    //        {
                    //            curUserState.AQEndTime = DateTime.Now;
                    //            curUserState.State = Guid.Parse(Reference.在线状态_退出);
                    //            bll.Update(curUserState);
                    //        }
                    //        CacheAccess.RemoveCache(CFG.用户待答题缓存键前缀 + requestMessage.FromUserName);
                    //    }
                    //}
                    //else
                    //{
                    //    //更新用户活跃时间 将用户添加或更新进数据库，由统一方法设置缓存
                    //    UserQAService.AddOrUpdateOnlineQAUser(requestMessage, userWeiXin, rqid);
                    //}

                    //更新用户活跃时间 将用户添加或更新进数据库，由统一方法设置缓存
                    UserQAService.AddOrUpdateOnlineQAUser(requestMessage, userWeiXin, rqid);
                    LogHelper.Write("操作问题，更新用户活跃时间", LogHelper.LogMessageType.Debug);
                    switch(operate)
                    {
                        case CFG.放弃本题: return GiveUpQuestionResponse(requestMessage, rqid, model, qakey);
                        case CFG.不是问题: return NotQuestionResponse(requestMessage, rqid, model, qakey);
                    }
                    LogHelper.Write("操作问题，非答题操作命令", LogHelper.LogMessageType.Debug);
                    return ics.GetInvalidCommondResponseMessage(requestMessage);
                }
            }
        }

        /// <summary>
        /// 放弃问题的处理方法
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="rqid"></param>
        /// <param name="model"></param>
        /// <param name="qakey"></param>
        /// <returns></returns>
        private ResponseMessageNews GiveUpQuestionResponse(RequestMessageText requestMessage, Guid rqid, UserQACache model, string qakey)
        {
            LogHelper.Write("放弃问题，进入放弃业务", LogHelper.LogMessageType.Debug);
            var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString();
            var commonService = new CommonService();
            //RQStart(requestMessage, rqid, commonService);
            var curentQAId = model.CurrentQA.ID;//为了比较一下，缓存里的当前问题是否已经被替换
            //经过以上的判断，这边的model必须有值
            //先判断，生成数据库对象，在保存时还要再判断，因为有可能两条以上进去了。
            LogHelper.Write((commonService.GetMsgIdCache(msgid) + " " + rqid + " " + (UserQAService.GetUserQACache(qakey) != null ).ToString()), LogHelper.LogMessageType.Debug);
            if (commonService.GetMsgIdCache(msgid) == rqid && UserQAService.GetUserQACache(qakey) != null)
            {//随时判断缓存有没有被定时器清空
                //问题分配记录
                var dsbll = new BaseBll<bmQADistribution>();
                var dsmodel = dsbll.All.FirstOrDefault(p => p.QAId == model.CurrentQA.ID && p.WeiXinId == requestMessage.FromUserName);
                LogHelper.Write(("取当前分配记录" + (dsmodel != null).ToString()), LogHelper.LogMessageType.Debug);
                if(dsmodel != null)
                { 
                    dsmodel.ModTime = DateTime.Now;
                    dsmodel.Result = Guid.Parse(Reference.分配答题操作_未处理);
                    //问题按是否收费区分回收
                    if (model.CurrentQA.MaBiNum > 0)
                    {
                        //收费问题的回收
                        dsmodel.WeiXinId = CFG.默认收费问题微信号;
                    }
                    else
                    {
                        //免费问题的回收                  
                        dsmodel.WeiXinId = CFG.默认免费问题微信号;
                    }
                    //放弃问题记录
                    var bll = new BaseBll<bmQA>();
                    var qamodel = new bmQA();
                    GenerateGiveUpQuestionModel(requestMessage, msgid, qamodel, model.CurrentQA.ID);
                    LogHelper.Write("放弃问题，主线程更新数据库前", LogHelper.LogMessageType.Debug);
                    LogHelper.Write((rqid + " " + (UserQAService.GetUserQACache(qakey) != null).ToString()), LogHelper.LogMessageType.Debug);
                    //更新到数据库
                    if (commonService.GetMsgIdCache(msgid) == rqid && UserQAService.GetUserQACache(qakey) != null)
                    {//添加前确认缓存是否被清空
                        bll.Insert(qamodel, false);
                        dsbll.Update(dsmodel);
                        //更新缓存操作
                        model = RefreshQACache(requestMessage, rqid, model, commonService);
                        LogHelper.Write("放弃问题，完成缓存刷新操作", LogHelper.LogMessageType.Debug);
                    }                    
                }//dsmodel != null     
            }//放弃答题业务结束
            else
            {
                LogHelper.Write("放弃问题，非主线程取缓存问题", LogHelper.LogMessageType.Debug);
                int i = 0;
                //为了取自增长ID
                do
                {
                    System.Threading.Thread.Sleep(500);
                    i++;
                    model = UserQAService.GetUserQACache(qakey);
                } while ((model.CurrentQA.ID != curentQAId) || i > 20);                
            }
            LogHelper.Write("答题缓存刷新了后，准备返回答题", LogHelper.LogMessageType.Debug);
            return AnswerResponse(requestMessage, PackCurrentQA(requestMessage, model));     
        }        

        /// <summary>
        /// 生成放弃本题模型
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="msgid"></param>
        /// <param name="model"></param>
        private void GenerateGiveUpQuestionModel(RequestMessageText requestMessage, string msgid, bmQA model, Guid parentId)            
        {
            model.ID = Guid.NewGuid();

            model.ParentId = parentId;
            model.WeiXinId = requestMessage.FromUserName;
            model.QARef = Guid.Parse(Reference.问答类别_放弃);
            model.MsgId = msgid;
            model.MsgType = Guid.Parse(Reference.微信消息类别_文本);
            model.QAContent = requestMessage.Content;//将指令保存数据库
            //model.MediaId = requestMessage.MediaId;
            //model.PicUrl = requestMessage.PicUrl;

            model.RegTime = DateTime.Now;
            model.ModTime = DateTime.Now;
            model.FlagTrashed = false;
            model.FlagDeleted = false;
        }

        /// <summary>
        /// 不是问题
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="rqid"></param>
        /// <param name="model"></param>
        /// <param name="qakey"></param>
        /// <returns></returns>
        private ResponseMessageNews NotQuestionResponse(RequestMessageText requestMessage, Guid rqid, UserQACache model, string qakey)
        {
            LogHelper.Write("不是问题，进入不是问题业务", LogHelper.LogMessageType.Debug);
            var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString();
            var commonService = new CommonService();
            //RQStart(requestMessage, rqid, commonService);
            var curentQAId = model.CurrentQA.ID;//为了比较一下，缓存里的当前问题是否已经被替换
            //经过以上的判断，这边的model必须有值
            //先判断，生成数据库对象，在保存时还要再判断，因为有可能两条以上进去了。
            LogHelper.Write((commonService.GetMsgIdCache(msgid) + " " + rqid + " " + (UserQAService.GetUserQACache(qakey) != null).ToString()), LogHelper.LogMessageType.Debug);
            if (commonService.GetMsgIdCache(msgid) == rqid && UserQAService.GetUserQACache(qakey) != null)
            {//随时判断缓存有没有被定时器清空                
                //放弃问题记录
                //var bll = new BaseBll<bmQA>();
                //var qamodel = new bmQA();
                //GenerateNotQuestionModel(requestMessage, msgid, qamodel, model.CurrentQA.ID);
                //LogHelper.Write("放弃问题，主线程更新数据库前", LogHelper.LogMessageType.Debug);
                //LogHelper.Write((rqid + " " + (UserQAService.GetUserQACache(qakey) != null).ToString()), LogHelper.LogMessageType.Debug);
                ////更新到数据库
                //if (commonService.GetMsgIdCache(msgid) == rqid && UserQAService.GetUserQACache(qakey) != null)
                //{//添加前确认缓存是否被清空
                //    bll.Insert(qamodel);                        
                //    //更新缓存操作
                //    model = RefreshQACache(requestMessage, rqid, model, commonService);
                //    LogHelper.Write("放弃问题，完成缓存刷新操作", LogHelper.LogMessageType.Debug);
                //}

                var dsbll = new BaseBll<bmQADistribution>();
                var dsmodel = dsbll.All.FirstOrDefault(p => p.QAId == model.CurrentQA.ID && p.WeiXinId == requestMessage.FromUserName);
                LogHelper.Write(("取当前分配记录" + (dsmodel != null).ToString()), LogHelper.LogMessageType.Debug);
                if (dsmodel != null)
                {
                    dsmodel.ModTime = DateTime.Now;
                    dsmodel.Result = Guid.Parse(Reference.分配答题操作_不是问题);
                    dsmodel.OperateTime = DateTime.Now;
                    //放弃问题记录
                    var bll = new BaseBll<bmQA>();
                    var qamodel = new bmQA();
                    GenerateNotQuestionModel(requestMessage, msgid, qamodel, model.CurrentQA.ID);
                    LogHelper.Write("不是问题，主线程更新数据库前", LogHelper.LogMessageType.Debug);
                    LogHelper.Write((rqid + " " + (UserQAService.GetUserQACache(qakey) != null).ToString()), LogHelper.LogMessageType.Debug);
                    //更新到数据库
                    if (commonService.GetMsgIdCache(msgid) == rqid && UserQAService.GetUserQACache(qakey) != null)
                    {//添加前确认缓存是否被清空
                        bll.Insert(qamodel, false);
                        dsbll.Update(dsmodel);
                        //更新缓存操作
                        model = RefreshQACache(requestMessage, rqid, model, commonService);
                        LogHelper.Write("放弃问题，完成缓存刷新操作", LogHelper.LogMessageType.Debug);
                    }
                }//dsmodel != null  
            }//放弃答题业务结束
            else
            {
                LogHelper.Write("放弃问题，非主线程取缓存问题", LogHelper.LogMessageType.Debug);
                int i = 0;
                //为了取自增长ID
                do
                {
                    System.Threading.Thread.Sleep(500);
                    i++;
                    model = UserQAService.GetUserQACache(qakey);
                } while ((model.CurrentQA.ID != curentQAId) || i > 20);
            }
            LogHelper.Write("答题缓存刷新了后，准备返回答题", LogHelper.LogMessageType.Debug);
            return AnswerResponse(requestMessage, PackCurrentQA(requestMessage, model));
        }

        /// <summary>
        /// 生成不是问题对象
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="msgid"></param>
        /// <param name="model"></param>
        /// <param name="parentId"></param>
        private void GenerateNotQuestionModel(RequestMessageText requestMessage, string msgid, bmQA model, Guid parentId)
        {
            model.ID = Guid.NewGuid();

            model.ParentId = parentId;
            model.WeiXinId = requestMessage.FromUserName;
            model.QARef = Guid.Parse(Reference.问答类别_不是问题);
            model.MsgId = msgid;
            model.MsgType = Guid.Parse(Reference.微信消息类别_文本);
            model.QAContent = requestMessage.Content;//将指令保存数据库
            //model.MediaId = requestMessage.MediaId;
            //model.PicUrl = requestMessage.PicUrl;

            model.RegTime = DateTime.Now;
            model.ModTime = DateTime.Now;
            model.FlagTrashed = false;
            model.FlagDeleted = false;
        }

        #endregion

        

        #region 答题处理 未实现
        /// <summary>
        /// 用户提问处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public ResponseMessageNews GetSubmitAnswerResponseMessage(RequestMessageImage requestMessage)
        {
            //用户提交问题处理
            return SubmitAnswerResponse(requestMessage);
        }

        /// <summary>
        /// 提问返回数据处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private ResponseMessageNews SubmitAnswerResponse(RequestMessageImage requestMessage)
        {
            return AnswerResponse<RequestMessageImage>(requestMessage, SubmitQuestion(requestMessage));
        }

        /// <summary>
        /// 用户拍照提交问题
        /// </summary>
        /// <param name="requestMessage"></param>
        private bmQA SubmitQuestion(RequestMessageImage requestMessage)
        {
            var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString();
            var qaid = Guid.NewGuid();
            var bll = new BaseBll<bmQA>();

            var model = new bmQA();
            var commonService = new CommonService();
            Guid mid = commonService.GetMsgIdCache(msgid);
            if (mid == Guid.Empty)
            {//已经添加的问题答案，不再保存进系统
                commonService.SetMsgIdCache(msgid, qaid);

                //将图片信息保存进数据库            
                model.ID = qaid;

                model.WeiXinId = requestMessage.FromUserName;
                model.QARef = Guid.Parse(Reference.问答类别_问题);
                model.MsgId = msgid;
                model.MsgType = Guid.Parse(Reference.微信消息类别_图片);
                model.MediaId = requestMessage.MediaId;
                model.PicUrl = requestMessage.PicUrl;

                model.RegTime = DateTime.Now;
                model.ModTime = DateTime.Now;
                model.FlagTrashed = false;
                model.FlagDeleted = false;

                //问题消耗马币和分配答题用户处理
                var userMaBi = new UserMaBiService().GetUserCurrentMaBi(requestMessage.FromUserName);
                //消耗马币
                if (userMaBi != null)
                {
                    var defMaBi = Convert.ToDecimal(CFG.提问默认收费马币值);
                    if (userMaBi.UMB.BBi >= defMaBi || userMaBi.UMB.MaBi >= defMaBi)
                    {
                        if (userMaBi.UMB.BBi >= defMaBi)
                        {
                            //消耗邦币处理
                            model.MaBiRef = Guid.Parse(Reference.马币类别_邦币);
                            model.MaBiNum = defMaBi;

                            //添加马币消费记录
                            var addMBR = new AddMBRModel();
                            addMBR.UIds.Add(userMaBi.UserId);

                            addMBR.QAId = model.ID;
                            addMBR.SR = Guid.Parse(Reference.马币来源_扣取);
                            addMBR.MBR = Guid.Parse(Reference.马币类别_邦币);
                            addMBR.MBN = 0 - defMaBi;
                            new UserMaBiService().AddUMBRByQA(addMBR, false);
                        }
                        else if (userMaBi.UMB.MaBi >= defMaBi)
                        {
                            //消耗马币处理
                            model.MaBiRef = Guid.Parse(Reference.马币类别_马币);
                            model.MaBiNum = defMaBi;

                            //添加马币消费记录
                            var addMBR = new AddMBRModel();
                            addMBR.UIds.Add(userMaBi.UserId);

                            addMBR.QAId = model.ID;
                            addMBR.SR = Guid.Parse(Reference.马币来源_扣取);
                            addMBR.MBR = Guid.Parse(Reference.马币类别_马币);
                            addMBR.MBN = 0 - defMaBi;
                            new UserMaBiService().AddUMBRByQA(addMBR, false);
                        }
                    }
                    else
                    {
                        //马币与邦币不足时的处理  都不足时不作任何处理
                    }
                }
                else
                {
                    //未绑定的微信号  也不作任何处理
                }

                //问题分配处理
                var qadbll = new BaseBll<bmQADistribution>();
                var qaModel = new bmQADistribution();

                qaModel.ID = Guid.NewGuid();
                qaModel.QAId = model.ID;
                qaModel.DistributionTime = DateTime.Now;
                qaModel.RegTime = DateTime.Now;
                if (model.MaBiNum > 0)
                {
                    //收费问题的分配
                    var bmOU = new UserQADistributionService().GetQADistribution(Guid.Parse(Reference.认证类别_认证邦主));
                    qaModel.WeiXinId = bmOU == null ? CFG.默认收费问题微信号 : bmOU.WeiXinId;
                }
                else
                {
                    //免费问题的分配
                    var bmOU = new UserQADistributionService().GetQADistribution(Guid.Parse(Reference.认证类别_未认证));
                    qaModel.WeiXinId = bmOU == null ? CFG.默认免费问题微信号 : bmOU.WeiXinId;
                }
                //判断缓存里保存的问答ID是否是当前的对象ID    
                if (commonService.GetMsgIdCache(msgid) == model.ID)
                {
                    qadbll.Insert(qaModel, false);
                    bll.Insert(model);
                }
            }

            //增加数据获取限制，如果等了7秒还未取到值，则不再取对象
            int i = 0;
            //为了取自增长ID
            do
            {
                if (commonService.GetMsgIdCache(msgid) != model.ID)
                {
                    System.Threading.Thread.Sleep(500);
                    i++;
                }
                model = bll.All.Where(p => p.MsgId == msgid).FirstOrDefault();
            } while (model.AutoGrenteId == 0 || i > 20);
            return model;
        }
        #endregion
    }
}