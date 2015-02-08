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

namespace MorSun.WX.ZYB.Service
{
    public class QuestionService
    {        

        #region 提问返回数据处理
        /// <summary>
        /// 提问返回数据处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private IResponseMessageBase QuestionResponse<T>(T requestMessage, bmQA model)
            where T : RequestMessageBase
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage); //CreateResponseMessage<ResponseMessageNews>();
            var comonservice = new CommonService();
            if(model == null)
            {
                comonservice.NonObject(requestMessage, responseMessage, "您还未提问", "您还未提问", "", "");
            }
            else
            {
                if (model.MsgType == Guid.Parse(Reference.微信消息类别_图片))
                { 
                    responseMessage.Articles.Add(new Article()
                    {
                        Title = ("问题编号：" + model.AutoGrenteId + " "),// + ((model.MaBiNum == 0 || model.MaBiNum == null) ? "" : (model.MaBiNum == null ? "" : ("消耗" + model.MaBiNum.ToString("f0") + comonservice.GetReferenceValue(model.MaBiRef)))),
                        Description = "提问时间:" + DateTime.Now,
                        PicUrl = model.PicUrl,
                        Url = CFG.网站域名 + CFG.问题查看路径 + "/" + model.ID.ToString()
                    });
                }
                if(model.MsgType == Guid.Parse(Reference.微信消息类别_文本))
                {
                    responseMessage.Articles.Add(new Article()
                    {
                        Title = ("问题编号：" + model.AutoGrenteId + " "),
                        Description = "提问时间:" + DateTime.Now + "\r\n" + model.QAContent,
                        PicUrl = CFG.网站域名 + "/images/zyb/textQ.png",
                        Url = CFG.网站域名 + CFG.问题查看路径 + "/" + model.ID.ToString()
                    });
                }
                if (model.MsgType == Guid.Parse(Reference.微信消息类别_声音))
                {
                    responseMessage.Articles.Add(new Article()
                    {
                        Title = ("问题编号：" + model.AutoGrenteId + " "),
                        Description = "提问时间:" + DateTime.Now,
                        PicUrl = CFG.网站域名 + "/images/zyb/voice.png",
                        Url = CFG.网站域名 + CFG.问题查看路径 + "/" + model.ID.ToString()
                    });
                }
            }
            //判断用户是否绑定，未绑定显示注册账号并绑定，已经绑定显示分享链接
            //comonservice.RegOrShare(requestMessage, responseMessage);
            
            return responseMessage;
        } 
        #endregion

        #region 提问处理
        #region 图片提问
        /// <summary>
        /// 用户图片提问处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public IResponseMessageBase SubmitQuestionResponseMessage(RequestMessageImage requestMessage)
        {
            //用户提交问题处理
            return SubmitQuestionResponse(requestMessage);            
        }

        /// <summary>
        /// 图片提问返回数据处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private IResponseMessageBase SubmitQuestionResponse(RequestMessageImage requestMessage)
        {
            return QuestionResponse<RequestMessageImage>(requestMessage, SubmitQuestion(requestMessage));
        }

        /// <summary>
        /// 用户拍照提交问题 这边只做问题保存，快速，可并发，不做其他任何数据处理。
        /// </summary>
        /// <param name="requestMessage"></param>
        private bmQA SubmitQuestion(RequestMessageImage requestMessage)
        {
            var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString();
            var rqid = Guid.NewGuid();
            var bll = new BaseBll<bmQA>();

            var model = new bmQA();
            var commonService = new CommonService();
            Guid mid = commonService.GetMsgIdCache(msgid);
            if (mid == Guid.Empty)
            {//已经添加的问题答案，不再保存进系统
                commonService.SetMsgIdCache(msgid, rqid);

                //将图片信息保存进数据库            
                GenerateQuestionModel(requestMessage, msgid, model);

                //判断缓存里保存的问答ID是否是当前的对象ID    
                if (commonService.GetMsgIdCache(msgid) == rqid)
                {
                    //qadbll.Insert(qaModel, false);
                    try
                    {
                        bll.Insert(model);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Write("图片问题保存时出错" + ex.Message, LogHelper.LogMessageType.Error);
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
                model = bll.All.Where(p => p.MsgId == msgid).FirstOrDefault();
            } while (model.AutoGrenteId == 0 || i > 20);
            return model;
        }



        /// <summary>
        /// 生成问题模型
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="msgid"></param>
        /// <param name="model"></param>
        private void GenerateQuestionModel(RequestMessageImage requestMessage, string msgid, bmQA model)
        {
            model.ID = Guid.NewGuid();
            
            model.WeiXinId = requestMessage.FromUserName;
            model.QARef = Guid.Parse(Reference.问答类别_问题);
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

        #region 文本提问
        /// <summary>
        /// 用户文本提问处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public IResponseMessageBase SubmitTextQuestionResponseMessage(RequestMessageText requestMessage)
        {
            //用户提交问题处理
            return SubmitTextQuestionResponse(requestMessage);
        }

        /// <summary>
        /// 图片提问返回数据处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private IResponseMessageBase SubmitTextQuestionResponse(RequestMessageText requestMessage)
        {
            return QuestionResponse<RequestMessageText>(requestMessage, SubmitTextQuestion(requestMessage));
        }

        /// <summary>
        /// 用户拍照提交问题 这边只做问题保存，快速，可并发，不做其他任何数据处理。
        /// </summary>
        /// <param name="requestMessage"></param>
        private bmQA SubmitTextQuestion(RequestMessageText requestMessage)
        {
            var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString();
            var rqid = Guid.NewGuid();
            var bll = new BaseBll<bmQA>();

            var model = new bmQA();
            var commonService = new CommonService();
            Guid mid = commonService.GetMsgIdCache(msgid);
            if (mid == Guid.Empty)
            {//已经添加的问题答案，不再保存进系统
                commonService.SetMsgIdCache(msgid, rqid);

                //将图片信息保存进数据库            
                GenerateTextQuestionModel(requestMessage, msgid, model);

                //判断缓存里保存的问答ID是否是当前的对象ID    
                if (commonService.GetMsgIdCache(msgid) == rqid)
                {
                    //qadbll.Insert(qaModel, false);
                    try
                    {
                        bll.Insert(model);
                    }
                    catch(Exception ex)
                    {
                        LogHelper.Write("文本问题保存时出错" + ex.Message, LogHelper.LogMessageType.Error);
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
                model = bll.All.Where(p => p.MsgId == msgid).FirstOrDefault();
            } while (model.AutoGrenteId == 0 || i > 20);
            return model;
        }



        /// <summary>
        /// 生成问题模型
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="msgid"></param>
        /// <param name="model"></param>
        private void GenerateTextQuestionModel(RequestMessageText requestMessage, string msgid, bmQA model)
        {
            model.ID = Guid.NewGuid();

            model.WeiXinId = requestMessage.FromUserName;
            model.QARef = Guid.Parse(Reference.问答类别_问题);
            model.MsgId = msgid;
            model.MsgType = Guid.Parse(Reference.微信消息类别_文本);
            model.QAContent = requestMessage.Content;
            model.WeiXinAPP = Guid.Parse(CFG.邦马网_当前微信应用);

            model.RegTime = DateTime.Now;
            model.ModTime = DateTime.Now;
            model.FlagTrashed = false;
            model.FlagDeleted = false;
        }
        #endregion

        #region 语音提问
        /// <summary>
        /// 用户文本提问处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public IResponseMessageBase SubmitVoiceQuestionResponseMessage(RequestMessageVoice requestMessage)
        {
            //用户提交问题处理
            return SubmitVoiceQuestionResponse(requestMessage);
        }

        /// <summary>
        /// 图片提问返回数据处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private IResponseMessageBase SubmitVoiceQuestionResponse(RequestMessageVoice requestMessage)
        {
            return QuestionResponse<RequestMessageVoice>(requestMessage, SubmitVoiceQuestion(requestMessage));
        }

        /// <summary>
        /// 用户拍照提交问题 这边只做问题保存，快速，可并发，不做其他任何数据处理。
        /// </summary>
        /// <param name="requestMessage"></param>
        private bmQA SubmitVoiceQuestion(RequestMessageVoice requestMessage)
        {
            var msgid = requestMessage.MsgId == null ? "" : requestMessage.MsgId.ToString();
            var rqid = Guid.NewGuid();
            var bll = new BaseBll<bmQA>();

            var model = new bmQA();
            var commonService = new CommonService();
            Guid mid = commonService.GetMsgIdCache(msgid);
            if (mid == Guid.Empty)
            {//已经添加的问题答案，不再保存进系统
                commonService.SetMsgIdCache(msgid, rqid);

                //将图片信息保存进数据库            
                GenerateVoiceQuestionModel(requestMessage, msgid, model);

                //判断缓存里保存的问答ID是否是当前的对象ID    
                if (commonService.GetMsgIdCache(msgid) == rqid)
                {
                    //qadbll.Insert(qaModel, false);
                    try
                    {
                        bll.Insert(model);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Write("语音问题保存时出错" + ex.Message, LogHelper.LogMessageType.Error);
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
                model = bll.All.Where(p => p.MsgId == msgid).FirstOrDefault();
            } while (model.AutoGrenteId == 0 || i > 20);
            return model;
        }



        /// <summary>
        /// 生成问题模型
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="msgid"></param>
        /// <param name="model"></param>
        private void GenerateVoiceQuestionModel(RequestMessageVoice requestMessage, string msgid, bmQA model)
        {
            model.ID = Guid.NewGuid();

            model.WeiXinId = requestMessage.FromUserName;
            model.QARef = Guid.Parse(Reference.问答类别_问题);
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

        #endregion

        #region 用户取问题
        /// <summary>
        /// 按时间逆序获取用户提问的问题
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="skipNum"></param>
        /// <returns></returns>
        public IResponseMessageBase GetQuestionResponseMessage(RequestMessageText requestMessage)
        {
            //用户提交问题处理
            var skipNum = 1;
            var text = requestMessage.Content;            
            if (text.Contains(" "))
            {
                try 
                { 
                    var commond = text.Substring(0, text.IndexOf(" "));
                    var numValue = text.Substring(commond.Length + 1, text.Length - commond.Length - 1).Replace(" ","");
                    skipNum = Convert.ToInt32(String.IsNullOrEmpty(numValue) ? "1" : numValue);
                }
                catch
                {
                    return new InvalidCommondService().GetInvalidCommondResponseMessage(requestMessage as RequestMessageText);
                }
            }     
            return GetQuestionResponse(requestMessage, skipNum);
        }

        /// <summary>
        /// 取问题返回数据处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private IResponseMessageBase GetQuestionResponse(RequestMessageText requestMessage, int skipNum)
        {
            return QuestionResponse<RequestMessageText>(requestMessage, GetQuestion(requestMessage, skipNum));
        }

        /// <summary>
        /// 用户取问题
        /// </summary>
        /// <param name="requestMessage"></param>
        private bmQA GetQuestion(RequestMessageText requestMessage, int skipNum)
        {
            if (skipNum < 1)
                skipNum = 1;
            skipNum = skipNum - 1;
            var bll = new BaseBll<bmQA>();
            var qaRef = Guid.Parse(Reference.问答类别_问题);
            var curWeiXinAPP = Guid.Parse(CFG.邦马网_当前微信应用);
            var questionCount = bll.All.Where(p => p.WeiXinAPP != null && p.WeiXinAPP == curWeiXinAPP && p.WeiXinId == requestMessage.FromUserName && p.QARef == qaRef).Count();
            if (skipNum > questionCount)
                skipNum = questionCount - 1;
            var model = bll.All.Where(p => p.WeiXinAPP != null && p.WeiXinAPP == curWeiXinAPP && p.WeiXinId == requestMessage.FromUserName && p.QARef == qaRef).OrderByDescending(p => p.RegTime).Skip(skipNum).Take(1).FirstOrDefault();
            return model;
        }

        #endregion


        
    }
}