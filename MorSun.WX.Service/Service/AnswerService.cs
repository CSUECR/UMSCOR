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
using MorSun.Common.认证级别;

namespace MorSun.WX.ZYB.Service
{
    public class AnswerService
    {
        #region 答题返回数据处理
        /// <summary>
        /// 提问返回数据处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private ResponseMessageNews AnswerResponse<T>(T requestMessage, bmQA model)
            where T : RequestMessageBase
        {
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
                Title = "本题获取时间:" + DateTime.Now.ToShortTimeString(),
                Description = "本题获取时间:" + DateTime.Now.ToShortTimeString(),
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

        private ResponseMessageNews NonDistributionResponse<T>(T requestMessage)
            where T : RequestMessageBase
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage);             
            responseMessage.Articles.Add(new Article()
            {
                Title = "正在为您分配答题资源，请稍候再尝试",
                Description = "正在为您分配答题资源，请稍候再尝试",
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
            return responseMessage;
        }

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

        #region 答题处理
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

        #region 用户开始答题
        /// <summary>
        /// 用户输入答题命令处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public ResponseMessageNews GetAnswerResponseMessage(RequestMessageText requestMessage)
        {
            //未绑定的用户录入答题命令的处理
            var userWeiXin = new CommonService().GetZYBUserByWeiXinId(requestMessage.FromUserName);
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
                //判断用户是否认证，以及认证与未认证的用户处理
                var commonService = new CommonService();
                var onlineuserCache = UserQAService.GetOlineQAUserCache();
                if (onlineuserCache == null)
                {   //缓存未设置的情况
                    //将用户添加或更新进数据库，由统一方法设置缓存
                    UserQAService.AddOrUpdateOnlineQAUser(requestMessage, userWeiXin);
                    //返回答题资源分配中，稍候再发送答题命令
                    return NonDistributionResponse(requestMessage);
                }
                else
                { 
                    //更新用户活跃时间 将用户添加或更新进数据库，由统一方法设置缓存
                    UserQAService.AddOrUpdateOnlineQAUser(requestMessage, userWeiXin);
                
                    if (userWeiXin.aspnet_Users1.wmfUserInfo != null && userWeiXin.aspnet_Users1.wmfUserInfo.CertificationLevel != null && CertificationLevel.DTCertificationLevel.Contains(userWeiXin.aspnet_Users1.wmfUserInfo.CertificationLevel))
                    {//认证用户处理
                        if(onlineuserCache.CertificationUser != null && onlineuserCache.CertificationUser.FirstOrDefault(p => p.WeiXinId == userWeiXin.WeiXinId) != null)
                        {
                            //在线认证用户缓存存在该用户的处理方式
                            //不管认证与未认证的用户，答题方法是一样的，只是在分配答题时，系统根据认证与未认证用户进行答题分配，分配好后，都是一样的从数据库中取数据答题
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
                        }
                        else
                        {
                            //未认证用用户未进缓存
                            
                            //返回答题资源分配中，稍候再发送答题命令
                            return NonDistributionResponse(requestMessage);
                        }
                    }

                    return GetAnswerResponse(requestMessage);
                }
            }
        }

        /// <summary>
        /// 取问题返回数据处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private ResponseMessageNews GetAnswerResponse(RequestMessageText requestMessage)
        {

            var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString();
            var rqid = Guid.NewGuid();
            var commonService = new CommonService();
            Guid mid = commonService.GetMsgIdCache(msgid);
            if(mid == Guid.Empty)
            {
                //设置用户消息缓存
                commonService.SetMsgIdCache(msgid, rqid);
            }
            // 用户的答题缓存都由用户在答题是设置
            //从缓存中获取后，待答题数量为0的处理
            var qakey = "dt" + requestMessage.FromUserName;
            var model = UserQAService.GetUserQACache(qakey);
            if(model == null || model.WaitQA.Count() == 0)
            {
                //无缓存或待答题数量为0，先取数据，如果数据库还没有待答题，则返回答题资源分配中
                //设置缓存微信并发时要处理
                if (commonService.GetMsgIdCache(msgid) == rqid)
                    model = UserQAService.InitUserQACache(qakey);
                else
                    System.Threading.Thread.Sleep(1000);//其他访问等1秒

            }
            //还是为空，返回答题资源分配中
            if(model == null || model.WaitQA.Count() == 0)
            {//返回答题资源分配中
                return NonDistributionResponse(requestMessage);
            }

            //从缓存中获取后，待答题数量与已答题数量一致时的处理
            //这种情况下，用户答题后系统要设置，首先，当前答题为空，其次待答题数量与已答题数量一致
            //经分析，用户输入dt命令时一般不会出现待答题与已答题数量一致的情况，

            //从缓存中获取后，有可答题时的处理            
            return AnswerResponse(requestMessage, GetAnswer(requestMessage, model, rqid));            
        }

        /// <summary>
        /// 认证与未认证用户取问题统一方法
        /// </summary>
        /// <param name="requestMessage"></param>
        private bmQA GetAnswer<T>(T requestMessage,UserQACache model,Guid rqid)
            where T : RequestMessageBase
        {
            var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString();
            var commonService = new CommonService();
            //var bll = new BaseBll<bmQA>();
            //var model = bll.All.FirstOrDefault();
            //调用说明，这个方法，在用户输入dt命令，或答题后，返回下一答题的方法，
            //这个方法只返回题目，不做其他处理。当前答题为空时，则去待答题取一条附值
            if(model.CurrentQA == null)
            {
                //当前答题为空
                if (commonService.GetMsgIdCache(msgid) == rqid)
                {
                    if (model.AlreadyQA.Count() == 0)
                        model.CurrentQA = model.WaitQA.OrderBy(p => p.RegTime).FirstOrDefault();
                    else
                    {//已答题有数据时，排除掉已答题后再取值
                        model.CurrentQA = model.WaitQA.Except(model.AlreadyQA).OrderBy(p => p.RegTime).FirstOrDefault();
                    }
                }
                else
                    System.Threading.Thread.Sleep(1000);//其他访问等1秒               
            }
            //是不是为空由下一步返回的代码再判断
            return model.CurrentQA;
        }

        #endregion
    }
}