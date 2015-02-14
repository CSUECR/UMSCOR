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
        #region 用户并发访问限制处理
        /// <summary>
        /// 限制并发请求返回内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public IResponseMessageBase ConcurrentResponse<T>(T requestMessage)
            where T : RequestMessageBase
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage);

            responseMessage.Articles.Add(new Article()
            {//眼睛图片
                Title = "系统拒绝响应您的胡乱答题请求",
                Description = "请您间隔" + CFG.用户连续请求时间间隔 + "秒来一发\r\n答题时连续发送多张图片，邦马网就判定您在乱答题",
                PicUrl = CFG.网站域名 + "/images/zyb/cancel.png",
                Url = CFG.网站域名
            });

            return responseMessage;
        }
        #endregion

        #region 请求开始处理
        public void RQStart<T>(T requestMessage, Guid rqid, CommonService commonService)
            where T : RequestMessageBase
        {
            var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString();            
            Guid mid = commonService.GetMsgIdCache(msgid);
            if (mid == Guid.Empty)
            {
                LogHelper.Write((msgid + " 设置用户单个请求消息缓存，防止微信发送单请求并发 " + rqid), LogHelper.LogMessageType.Debug);
                //设置用户消息缓存
                commonService.SetMsgIdCache(msgid, rqid);
            }
        }

        private void RQLimit<T>(T requestMessage, CommonService commonService)
            where T : RequestMessageBase
        {                        
            var rqLimKey = CFG.限制用户并发缓存键前缀 + requestMessage.FromUserName;
            
            if (String.IsNullOrEmpty(commonService.GetUserRQLimCache(rqLimKey)))
            {
                var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString();
                LogHelper.Write((msgid + " 设置用户请求缓存，防止用户并发操作"), LogHelper.LogMessageType.Debug);
                //设置用户消息缓存
                commonService.SetUserRQLimCache(rqLimKey, msgid);
            }
        }

        /// <summary>
        /// 判断是否为限制时间内的请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public bool nonConcurrentRQ<T>(T requestMessage)
            where T : RequestMessageBase
        {
            var commonService = new CommonService();
            RQLimit(requestMessage, commonService);
            var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString();
            var rqLimKey = CFG.限制用户并发缓存键前缀 + requestMessage.FromUserName;
            LogHelper.Write((commonService.GetUserRQLimCache(rqLimKey) + " 取当前用户的防并发请求缓存"), LogHelper.LogMessageType.Debug);
            if (msgid == commonService.GetUserRQLimCache(rqLimKey))
                return true;
            else
                return false;

        }
        #endregion

        #region 答题返回数据处理
        /// <summary>
        /// 提问返回数据处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private IResponseMessageBase AnswerResponse<T>(T requestMessage, bmQAView model)
            where T : RequestMessageBase
        {
            LogHelper.Write("返回待答问题", LogHelper.LogMessageType.Debug);
            if(model == null)
            { //传过来是空值时，返回系统资源分配中
                LogHelper.Write("传过来的答题为空", LogHelper.LogMessageType.Debug);
                return NonDistributionResponse(requestMessage); 
            }     
            if(model.MsgType == Guid.Parse(Reference.微信消息类别_声音))
            {
                var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageVoice>(requestMessage);
                responseMessage.Voice.MediaId = model.MediaId;
                return responseMessage;
            }
            else if (model.MsgType == Guid.Parse(Reference.微信消息类别_图片))
            {
                var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageImage>(requestMessage);
                responseMessage.Image.MediaId = model.MediaId;
                return responseMessage;
            }
            else if (model.MsgType == Guid.Parse(Reference.微信消息类别_文本))
            {
                var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
                responseMessage.Content = model.QAContent;
                return responseMessage;
            }
            else
            {
                LogHelper.Write("无法识别的答题格式，返回再分配", LogHelper.LogMessageType.Debug);
                return NonDistributionResponse(requestMessage);
            }
            //else
            //{ 
            //    var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage); //CreateResponseMessage<ResponseMessageNews>();
            //    var comonservice = new CommonService();

            //    if (model.MsgType == Guid.Parse(Reference.微信消息类别_图片))
            //    {
            //        responseMessage.Articles.Add(new Article()
            //        {
            //            Title = ("问题编号：" + model.AutoGrenteId + " " + ((model.MBNum == 0 || model.MBNum == null) && (model.BBNum == 0 || model.BBNum == null) ? "免费提问" : ("消耗" + ((model.MBNum == 0 || model.MBNum == null) ? "" : (Math.Abs(model.MBNum).ToString("f0") + "马币")) + ((model.BBNum == 0 || model.BBNum == null) ? "" : (Math.Abs(model.BBNum).ToString("f0") + "邦币"))))),
            //            Description = "提问时间:" + (model.RegTime == null ? "" : (model.RegTime.ToShortDateString() + " " + model.RegTime.Value.ToShortTimeString()))
            //            + "\r\n获取时间:" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()
            //            + "\r\n当前未答题数： " + model.DJDCount
            //            ,
            //            PicUrl = model.PicUrl,
            //            Url = CFG.网站域名 + CFG.问题查看路径 + "/" + model.ID.ToString() //model.PicUrl
            //        });
            //    }

            //    if (model.MsgType == Guid.Parse(Reference.微信消息类别_文本))
            //    {
            //        responseMessage.Articles.Add(new Article()
            //        {
            //            Title = ("问题编号：" + model.AutoGrenteId + " " + ((model.MBNum == 0 || model.MBNum == null) && (model.BBNum == 0 || model.BBNum == null) ? "免费提问" : ("消耗" + ((model.MBNum == 0 || model.MBNum == null) ? "" : (Math.Abs(model.MBNum).ToString("f0") + "马币")) + ((model.BBNum == 0 || model.BBNum == null) ? "" : (Math.Abs(model.BBNum).ToString("f0") + "邦币"))))),
            //            Description = model.QAContent
            //            +"\r\n"
            //            +"\r\n提问时间:" + (model.RegTime == null ? "" : (model.RegTime.ToShortDateString() + " " + model.RegTime.Value.ToShortTimeString()))
            //            + "\r\n获取时间:" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()
            //            + "\r\n当前未答题数： " + model.DJDCount
            //            ,
            //            PicUrl = CFG.网站域名 + "/images/zyb/textQ.png",
            //            Url = CFG.网站域名 + CFG.问题查看路径 + "/" + model.ID.ToString()
            //        });
            //    }   
            //    return responseMessage;
            //}
        }

        /// <summary>
        /// 答题资源未分配
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public IResponseMessageBase NonDistributionResponse<T>(T requestMessage)
            where T : RequestMessageBase
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage); 
            var s = "";
            var model = UserQAService.GetOlineQAUserCache();
            if(model != null)
            {
                s += "\r\n" + ("当前未答数(收费：" + model.MaBiQACount + " 免费：" + model.NonMaBiQACount + ")");
                s += "\r\n" + ("当前在线人数(认证：" + model.CertificationUser.Count() + " 未认证：" + model.NonCertificationQAUser.Count() + ")");
            }
            responseMessage.Articles.Add(new Article()
            {
                Title = "正在为您分配答题资源，请稍候再尝试发送： " + CFG.开始答题,
                Description = "放弃本题请发送:" + " " + CFG.放弃本题 +
                "\r\n" + "这不是一个问题请发送：" + " " + CFG.不是问题 +
                "\r\n" + "文字答题请发送文字答案，想稳一点的发送：" + " " + CFG.回答问题 + " 答案" + 
                "\r\n" + "图片答题请发送图片答案" +
                "\r\n" + "语音答题请发送语音答案" +
                "\r\n" + "查看当前问题详细信息请发送：" + CFG.详细信息 +
                "\r\n" + "退出答题请发送：" + " " + CFG.退出答题 +
                s,
                PicUrl = CFG.网站域名 + "/images/zyb/traffic.png",
                Url = ""
            }); 
            return responseMessage;
        }

        /// <summary>
        /// 拒绝答题服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private IResponseMessageBase RefusedAnswerResponse<T>(T requestMessage)
            where T : RequestMessageBase
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage);
            responseMessage.Articles.Add(new Article()
            {
                Title = "系统拒绝了您的答题请求",
                Description = "一小时内答退次数超过5次，邦马网就不会分配新题目给您",
                PicUrl = CFG.网站域名 + "/images/zyb/cancel.png",
                Url = CFG.网站域名
            });
            return responseMessage;
        }

        /// <summary>
        /// 退出答题返回内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private IResponseMessageBase LogOutAnswerResponse<T>(T requestMessage)
            where T : RequestMessageBase
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage);
            var s = "";
            var model = UserQAService.GetOlineQAUserCache();
            if (model != null)
            {
                s += "\r\n" + ("当前未答数(收费：" + model.MaBiQACount + " 免费：" + model.NonMaBiQACount + ")");
                s += "\r\n" + ("当前在线人数(认证：" + model.CertificationUser.Count() + " 未认证：" + model.NonCertificationQAUser.Count() + ")");
            }
            responseMessage.Articles.Add(new Article()
            {
                Title = "您已退出答题",
                Description = "系统正在回收答题资源" + s,
                PicUrl = CFG.网站域名 + "/images/zyb/home.png",
                Url = CFG.网站域名
            });

            ////当前答题缓存数据
            //var model = UserQAService.GetOlineQAUserCache();
            //if (model != null)
            //{
            //    responseMessage.Articles.Add(new Article()
            //    {//问号图片
            //        Title = ("当前未答数(收费：" + model.MaBiQACount + " 免费：" + model.NonMaBiQACount + ")"),
            //        Description = "查看分配规则",
            //        PicUrl = "",
            //        Url = CFG.网站域名
            //    });
            //    responseMessage.Articles.Add(new Article()
            //    {//问号图片
            //        Title = ("当前在线人数(认证：" + model.CertificationUser.Count() + " 未认证：" + model.NonCertificationQAUser.Count() + ")"),
            //        Description = ("当前在线人数"),
            //        PicUrl = "",
            //        Url = CFG.网站域名
            //    });
            //}

            return responseMessage;
        }
        #endregion

        #region 需要返回的问题
        /// <summary>
        /// 包装当前答题 设置待答题数量
        /// </summary>
        /// <param name="requestMessage"></param>
        public bmQAView PackCurrentQA<T>(T requestMessage, UserQACache model)
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
        public IResponseMessageBase StartAnswerResponseMessage(RequestMessageText requestMessage)
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
                    LogHelper.Write("答题用户缓存为空", LogHelper.LogMessageType.Debug);
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
                            LogHelper.Write("认证用户未进答题缓存", LogHelper.LogMessageType.Debug);
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
                            LogHelper.Write("未认证用户未进答题缓存", LogHelper.LogMessageType.Debug);
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
        private IResponseMessageBase GetAnswerResponse(RequestMessageText requestMessage, Guid rqid)
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

        #region 用户操作问题  放弃 不是问题 文本答题 退出答题
        /// <summary>
        /// 放弃问题
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public IResponseMessageBase OperateQuestionResponseMessage(RequestMessageText requestMessage, string operate, bool ignoreJudgeCurUQAC = false)
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

                    
                    //退出答题不用判断用户当前答题是否为空
                    var qakey = CFG.用户待答题缓存键前缀 + requestMessage.FromUserName;
                    var model = UserQAService.GetUserQACache(qakey);
                    if (!ignoreJudgeCurUQAC)
                    { 
                        if (model == null || model.CurrentQA == null)
                        {
                            LogHelper.Write("操作问题，当前用户答题缓存为空或无当前答题", LogHelper.LogMessageType.Debug);
                            //用户答题缓存为空，
                            return ics.GetInvalidCommondResponseMessage(requestMessage);
                        }
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
                    if (!ignoreJudgeCurUQAC)
                    {//退出答题不需要更新当前用户活跃时间，他需要的操作是，将当前用户的活跃时间提前到让缓存足以回收。
                        UserQAService.AddOrUpdateOnlineQAUser(requestMessage, userWeiXin, rqid);
                        LogHelper.Write("操作问题，更新用户活跃时间", LogHelper.LogMessageType.Debug);
                    }
                    switch(operate)
                    {
                        case CFG.放弃本题: return GiveUpQuestionResponse(requestMessage, rqid, model, qakey);
                        case CFG.不是问题: return NotQuestionResponse(requestMessage, rqid, model, qakey);
                        case CFG.回答问题: return TextAnswerQuestionResponse(requestMessage, rqid, model, qakey);
                        case CFG.退出答题: return ExitQuestionResponse(requestMessage, rqid);
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
        private IResponseMessageBase GiveUpQuestionResponse(RequestMessageText requestMessage, Guid rqid, UserQACache model, string qakey)
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
                    if (model.CurrentQA.MBNum > 0 || model.CurrentQA.BBNum > 0)
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
            model.WeiXinAPP = Guid.Parse(CFG.邦马网_当前微信应用);
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
        private IResponseMessageBase NotQuestionResponse(RequestMessageText requestMessage, Guid rqid, UserQACache model, string qakey)
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
                        LogHelper.Write("不是问题，完成缓存刷新操作", LogHelper.LogMessageType.Debug);
                    }
                }//dsmodel != null  
            }//放弃答题业务结束
            else
            {
                LogHelper.Write("不是问题，非主线程取缓存问题", LogHelper.LogMessageType.Debug);
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
            model.WeiXinAPP = Guid.Parse(CFG.邦马网_当前微信应用);
            //model.MediaId = requestMessage.MediaId;
            //model.PicUrl = requestMessage.PicUrl;

            model.RegTime = DateTime.Now;
            model.ModTime = DateTime.Now;
            model.FlagTrashed = false;
            model.FlagDeleted = false;
        }

        /// <summary>
        /// 退出答题
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="rqid"></param>
        /// <param name="model"></param>
        /// <param name="qakey"></param>
        /// <returns></returns>
        private IResponseMessageBase ExitQuestionResponse(RequestMessageText requestMessage, Guid rqid)
        {
            LogHelper.Write("退出答题，进入退出答题业务", LogHelper.LogMessageType.Debug);
            var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString();
            var commonService = new CommonService();
            //RQStart(requestMessage, rqid, commonService);
            //var curentQAId = model.CurrentQA.ID;//为了比较一下，缓存里的当前问题是否已经被替换
            //经过以上的判断，这边的model必须有值
            //先判断，生成数据库对象，在保存时还要再判断，因为有可能两条以上进去了。
            LogHelper.Write((commonService.GetMsgIdCache(msgid) + " " + rqid), LogHelper.LogMessageType.Debug);
            if (commonService.GetMsgIdCache(msgid) == rqid)
            {  
                var state = Guid.Parse(Reference.在线状态_在线);
                var bll = new BaseBll<bmOnlineQAUser>();
                var curUserOnlineState = bll.All.Where(p => p.State == state && p.WeiXinId == requestMessage.FromUserName);                  
                if(curUserOnlineState.Count() > 0)
                {
                    LogHelper.Write("用户在线答题表存在当前用户", LogHelper.LogMessageType.Debug);  
                    var logoutMN = 0 - Convert.ToInt32(CFG.强制退出时间);
                    var dt = DateTime.Now.AddMinutes(2 * logoutMN);
                    foreach(var item in curUserOnlineState)
                    {
                        item.ActiveTime = dt;
                    }
                    bll.UpdateChanges();
                    LogHelper.Write("完成更新用户在线答题记录", LogHelper.LogMessageType.Debug);  
                }
            }//放弃答题业务结束
            else
            {                
                System.Threading.Thread.Sleep(1000);                   
            }
            LogHelper.Write("当前用户退出答题的活跃时间理新后，返回退出答题信息", LogHelper.LogMessageType.Debug);
            return LogOutAnswerResponse(requestMessage);
        }        

        /// <summary>
        /// 文字回答问题
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="rqid"></param>
        /// <param name="model"></param>
        /// <param name="qakey"></param>
        /// <returns></returns>
        private IResponseMessageBase TextAnswerQuestionResponse(RequestMessageText requestMessage, Guid rqid, UserQACache model, string qakey)
        {
            LogHelper.Write("文字回答问题，进入文字回答问题业务", LogHelper.LogMessageType.Debug);
            var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString();
            var commonService = new CommonService();
            //RQStart(requestMessage, rqid, commonService);
            var curentQAId = model.CurrentQA.ID;//为了比较一下，缓存里的当前问题是否已经被替换
            //经过以上的判断，这边的model必须有值
            //先判断，生成数据库对象，在保存时还要再判断，因为有可能两条以上进去了。
            LogHelper.Write((commonService.GetMsgIdCache(msgid) + " " + rqid + " " + (UserQAService.GetUserQACache(qakey) != null).ToString()), LogHelper.LogMessageType.Debug);
            if (commonService.GetMsgIdCache(msgid) == rqid && UserQAService.GetUserQACache(qakey) != null)
            {//随时判断缓存有没有被定时器清空
                var dsbll = new BaseBll<bmQADistribution>();
                var dsmodel = dsbll.All.FirstOrDefault(p => p.QAId == model.CurrentQA.ID && p.WeiXinId == requestMessage.FromUserName);
                LogHelper.Write(("取当前分配记录" + (dsmodel != null).ToString()), LogHelper.LogMessageType.Debug);
                if (dsmodel != null)
                {
                    dsmodel.ModTime = DateTime.Now;
                    dsmodel.Result = Guid.Parse(Reference.分配答题操作_已解答);
                    dsmodel.OperateTime = DateTime.Now;
                    //放弃问题记录
                    var bll = new BaseBll<bmQA>();
                    var qamodel = new bmQA();
                    GenerateTextAnswerQuestionModel(requestMessage, msgid, qamodel, model.CurrentQA.ID);
                    LogHelper.Write("文字回答问题，主线程更新数据库前", LogHelper.LogMessageType.Debug);
                    LogHelper.Write((rqid + " " + (UserQAService.GetUserQACache(qakey) != null).ToString()), LogHelper.LogMessageType.Debug);
                    //更新到数据库
                    if (commonService.GetMsgIdCache(msgid) == rqid && UserQAService.GetUserQACache(qakey) != null)
                    {//添加前确认缓存是否被清空
                        bll.Insert(qamodel, false);
                        dsbll.Update(dsmodel);
                        //更新缓存操作
                        model = RefreshQACache(requestMessage, rqid, model, commonService);
                        LogHelper.Write("文字回答问题，完成缓存刷新操作", LogHelper.LogMessageType.Debug);
                    }
                }//dsmodel != null  
            }//放弃答题业务结束
            else
            {
                LogHelper.Write("文字回答问题，非主线程取缓存问题", LogHelper.LogMessageType.Debug);
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
        /// 生成文本答题对象
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="msgid"></param>
        /// <param name="model"></param>
        /// <param name="parentId"></param>
        private void GenerateTextAnswerQuestionModel(RequestMessageText requestMessage, string msgid, bmQA model, Guid parentId)
        {
            model.ID = Guid.NewGuid();

            model.ParentId = parentId;
            model.WeiXinId = requestMessage.FromUserName;
            model.QARef = Guid.Parse(Reference.问答类别_答案);
            model.MsgId = msgid;
            model.MsgType = Guid.Parse(Reference.微信消息类别_文本);
            //一般文本回答问题与强制文本回答问题
            if (requestMessage.Content.Substring(0,2).ToLower().StartsWith(CFG.回答问题))
                model.QAContent = requestMessage.Content.Substring(2).Trim();//将指令保存数据库
            else
                model.QAContent = requestMessage.Content;
            model.WeiXinAPP = Guid.Parse(CFG.邦马网_当前微信应用);
            //model.MediaId = requestMessage.MediaId;
            //model.PicUrl = requestMessage.PicUrl;

            model.RegTime = DateTime.Now;
            model.ModTime = DateTime.Now;
            model.FlagTrashed = false;
            model.FlagDeleted = false;
        }
        #endregion        

        #region 图片答题处理        
        /// <summary>
        /// 回答问题处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public IResponseMessageBase AnswerQuestionResponseMessage(RequestMessageImage requestMessage)
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
                    LogHelper.Write("图片回答问题，无在线用户缓存", LogHelper.LogMessageType.Debug);
                    //不是在线答题用户，直接返回无效命令 
                    return ics.GetInvalidCommondResponseMessage(requestMessage);
                }
                else
                {
                    //在线用户是否存在该用户
                    if (onlineuserCache.CertificationUser.FirstOrDefault(p => p.WeiXinId == requestMessage.FromUserName) == null
                        && onlineuserCache.NonCertificationQAUser.FirstOrDefault(p => p.WeiXinId == requestMessage.FromUserName) == null)
                    {
                        LogHelper.Write("图片回答问题，当前用户不在缓存里", LogHelper.LogMessageType.Debug);
                        //不是在线答题用户，直接返回无效命令 
                        return ics.GetInvalidCommondResponseMessage(requestMessage);
                    }

                    var qakey = CFG.用户待答题缓存键前缀 + requestMessage.FromUserName;
                    var model = UserQAService.GetUserQACache(qakey);
                    if (model == null || model.CurrentQA == null)
                    {
                        LogHelper.Write("图片回答问题，当前用户答题缓存为空或无当前答题", LogHelper.LogMessageType.Debug);
                        //用户答题缓存为空，
                        return ics.GetInvalidCommondResponseMessage(requestMessage);
                    }

                    //更新用户活跃时间 将用户添加或更新进数据库，由统一方法设置缓存
                    UserQAService.AddOrUpdateOnlineQAUser(requestMessage, userWeiXin, rqid);
                    LogHelper.Write("图片回答问题，更新用户活跃时间", LogHelper.LogMessageType.Debug);
                    return AnswerQuestionResponse(requestMessage, rqid, model, qakey);                                         
                }
            }
        }

        private IResponseMessageBase AnswerQuestionResponse(RequestMessageImage requestMessage, Guid rqid, UserQACache model, string qakey)
        {
            LogHelper.Write("图片回答问题，进入图片回答问题业务", LogHelper.LogMessageType.Debug);
            var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString();
            var commonService = new CommonService();
            //RQStart(requestMessage, rqid, commonService);
            var curentQAId = model.CurrentQA.ID;//为了比较一下，缓存里的当前问题是否已经被替换
            //经过以上的判断，这边的model必须有值
            //先判断，生成数据库对象，在保存时还要再判断，因为有可能两条以上进去了。
            LogHelper.Write((commonService.GetMsgIdCache(msgid) + " " + rqid + " " + (UserQAService.GetUserQACache(qakey) != null).ToString()), LogHelper.LogMessageType.Debug);
            if (commonService.GetMsgIdCache(msgid) == rqid && UserQAService.GetUserQACache(qakey) != null)
            {//随时判断缓存有没有被定时器清空
                //以下代码是将答案保存进数据库，并刷新缓存操作 也就是回答一次就添加一条记录进数据库
                var dsbll = new BaseBll<bmQADistribution>();
                var dsmodel = dsbll.All.FirstOrDefault(p => p.QAId == model.CurrentQA.ID && p.WeiXinId == requestMessage.FromUserName);
                LogHelper.Write(("取当前分配记录" + (dsmodel != null).ToString()), LogHelper.LogMessageType.Debug);
                if (dsmodel != null)
                {
                    dsmodel.ModTime = DateTime.Now;
                    dsmodel.Result = Guid.Parse(Reference.分配答题操作_已解答);
                    dsmodel.OperateTime = DateTime.Now;
                    //放弃问题记录
                    var bll = new BaseBll<bmQA>();
                    var qamodel = new bmQA();
                    GenerateAnswerQuestionModel(requestMessage, msgid, qamodel, model.CurrentQA.ID);
                    LogHelper.Write("图片回答问题，主线程更新数据库前", LogHelper.LogMessageType.Debug);
                    LogHelper.Write((rqid + " " + (UserQAService.GetUserQACache(qakey) != null).ToString()), LogHelper.LogMessageType.Debug);
                    //更新到数据库
                    if (commonService.GetMsgIdCache(msgid) == rqid && UserQAService.GetUserQACache(qakey) != null)
                    {//添加前确认缓存是否被清空
                        bll.Insert(qamodel, false);
                        dsbll.Update(dsmodel);
                        //更新缓存操作
                        model = RefreshQACache(requestMessage, rqid, model, commonService);
                        LogHelper.Write("图片回答问题，完成缓存刷新操作", LogHelper.LogMessageType.Debug);
                    }
                }//dsmodel != null  
            }//放弃答题业务结束
            else
            {
                LogHelper.Write("图片回答问题，非主线程取缓存问题", LogHelper.LogMessageType.Debug);
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
        /// 生成图片答题对象
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="msgid"></param>
        /// <param name="model"></param>
        /// <param name="parentId"></param>
        private void GenerateAnswerQuestionModel(RequestMessageImage requestMessage, string msgid, bmQA model, Guid parentId)
        {
            model.ID = Guid.NewGuid();

            model.ParentId = parentId;
            model.WeiXinId = requestMessage.FromUserName;
            model.QARef = Guid.Parse(Reference.问答类别_答案);
            model.MsgId = msgid;
            model.MsgType = Guid.Parse(Reference.微信消息类别_图片);
            model.MediaId = requestMessage.MediaId;
            model.PicUrl = requestMessage.PicUrl;
            model.WeiXinAPP = Guid.Parse(CFG.邦马网_当前微信应用);

            model.RegTime = DateTime.Now;
            model.ModTime = DateTime.Now;
            model.FlagTrashed = false;
            model.FlagDeleted = false;
        }
       
        #endregion

        #region 声音答题处理
        /// <summary>
        /// 回答问题处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public IResponseMessageBase AnswerQuestionVoiceResponseMessage(RequestMessageVoice requestMessage)
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
                    LogHelper.Write("声音回答问题，无在线用户缓存", LogHelper.LogMessageType.Debug);
                    //不是在线答题用户，直接返回无效命令 
                    return ics.GetInvalidCommondResponseMessage(requestMessage);
                }
                else
                {
                    //在线用户是否存在该用户
                    if (onlineuserCache.CertificationUser.FirstOrDefault(p => p.WeiXinId == requestMessage.FromUserName) == null
                        && onlineuserCache.NonCertificationQAUser.FirstOrDefault(p => p.WeiXinId == requestMessage.FromUserName) == null)
                    {
                        LogHelper.Write("声音回答问题，当前用户不在缓存里", LogHelper.LogMessageType.Debug);
                        //不是在线答题用户，直接返回无效命令 
                        return ics.GetInvalidCommondResponseMessage(requestMessage);
                    }

                    var qakey = CFG.用户待答题缓存键前缀 + requestMessage.FromUserName;
                    var model = UserQAService.GetUserQACache(qakey);
                    if (model == null || model.CurrentQA == null)
                    {
                        LogHelper.Write("声音回答问题，当前用户答题缓存为空或无当前答题", LogHelper.LogMessageType.Debug);
                        //用户答题缓存为空，
                        return ics.GetInvalidCommondResponseMessage(requestMessage);
                    }

                    //更新用户活跃时间 将用户添加或更新进数据库，由统一方法设置缓存
                    UserQAService.AddOrUpdateOnlineQAUser(requestMessage, userWeiXin, rqid);
                    LogHelper.Write("图片回答问题，更新用户活跃时间", LogHelper.LogMessageType.Debug);
                    return AnswerQuestionVoiceResponse(requestMessage, rqid, model, qakey);
                }
            }
        }

        private IResponseMessageBase AnswerQuestionVoiceResponse(RequestMessageVoice requestMessage, Guid rqid, UserQACache model, string qakey)
        {
            LogHelper.Write("声音回答问题，进入声音回答问题业务", LogHelper.LogMessageType.Debug);
            var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString();
            var commonService = new CommonService();
            //RQStart(requestMessage, rqid, commonService);
            var curentQAId = model.CurrentQA.ID;//为了比较一下，缓存里的当前问题是否已经被替换
            //经过以上的判断，这边的model必须有值
            //先判断，生成数据库对象，在保存时还要再判断，因为有可能两条以上进去了。
            LogHelper.Write((commonService.GetMsgIdCache(msgid) + " " + rqid + " " + (UserQAService.GetUserQACache(qakey) != null).ToString()), LogHelper.LogMessageType.Debug);
            if (commonService.GetMsgIdCache(msgid) == rqid && UserQAService.GetUserQACache(qakey) != null)
            {//随时判断缓存有没有被定时器清空
                //以下代码是将答案保存进数据库，并刷新缓存操作 也就是回答一次就添加一条记录进数据库
                var dsbll = new BaseBll<bmQADistribution>();
                var dsmodel = dsbll.All.FirstOrDefault(p => p.QAId == model.CurrentQA.ID && p.WeiXinId == requestMessage.FromUserName);
                LogHelper.Write(("取当前分配记录" + (dsmodel != null).ToString()), LogHelper.LogMessageType.Debug);
                if (dsmodel != null)
                {
                    dsmodel.ModTime = DateTime.Now;
                    dsmodel.Result = Guid.Parse(Reference.分配答题操作_已解答);
                    dsmodel.OperateTime = DateTime.Now;
                    //放弃问题记录
                    var bll = new BaseBll<bmQA>();
                    var qamodel = new bmQA();
                    GenerateAnswerQuestionVoiceModel(requestMessage, msgid, qamodel, model.CurrentQA.ID);
                    LogHelper.Write("声音回答问题，主线程更新数据库前", LogHelper.LogMessageType.Debug);
                    LogHelper.Write((rqid + " " + (UserQAService.GetUserQACache(qakey) != null).ToString()), LogHelper.LogMessageType.Debug);
                    //更新到数据库
                    if (commonService.GetMsgIdCache(msgid) == rqid && UserQAService.GetUserQACache(qakey) != null)
                    {//添加前确认缓存是否被清空
                        bll.Insert(qamodel, false);
                        dsbll.Update(dsmodel);
                        //更新缓存操作
                        model = RefreshQACache(requestMessage, rqid, model, commonService);
                        LogHelper.Write("声音回答问题，完成缓存刷新操作", LogHelper.LogMessageType.Debug);
                    }
                }//dsmodel != null  
            }//放弃答题业务结束
            else
            {
                LogHelper.Write("声音回答问题，非主线程取缓存问题", LogHelper.LogMessageType.Debug);
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
        /// 生成图片答题对象
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="msgid"></param>
        /// <param name="model"></param>
        /// <param name="parentId"></param>
        private void GenerateAnswerQuestionVoiceModel(RequestMessageVoice requestMessage, string msgid, bmQA model, Guid parentId)
        {
            model.ID = Guid.NewGuid();

            model.ParentId = parentId;
            model.WeiXinId = requestMessage.FromUserName;
            model.QARef = Guid.Parse(Reference.问答类别_答案);
            model.MsgId = msgid;
            model.MsgType = Guid.Parse(Reference.微信消息类别_声音);
            model.MediaId = requestMessage.MediaId;            
            model.WeiXinAPP = Guid.Parse(CFG.邦马网_当前微信应用);

            model.RegTime = DateTime.Now;
            model.ModTime = DateTime.Now;
            model.FlagTrashed = false;
            model.FlagDeleted = false;
        }

        #endregion
    }
}