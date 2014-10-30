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
        private ResponseMessageNews QuestionResponse<T>(T requestMessage, bmQA model)
            where T : RequestMessageBase
        {
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
                Title = "看答案",
                Description = "看答案",
                PicUrl = "",
                Url = CFG.网站域名 + "/QA/Q/" + model.ID.ToString()
            });//再增加 加码 求解题思路            
            responseMessage.Articles.Add(new Article()
            {//美元图片
                Title = "加马币",
                Description = "加马币",
                PicUrl = "",
                Url = CFG.网站域名 + "/QA/Q/" + model.ID.ToString()
            });
            responseMessage.Articles.Add(new Article()
            {//问号图片
                Title = "求思路",
                Description = "求思路",
                PicUrl = "",
                Url = CFG.网站域名 + "/QA/Q/" + model.ID.ToString()
            });
            responseMessage.Articles.Add(new Article()
            {//问号图片
                Title = "直接看答案请发送:   " + CFG.看答案前缀 + " " + model.AutoGrenteId,
                Description = "直接看答案",
                PicUrl = "",
                Url = CFG.网站域名 + "/QA/Q/" + model.ID.ToString()
            });

            //判断用户是否绑定，未绑定显示注册账号并绑定，已经绑定显示分享链接
            var userWeiXin = new UserMaBiService().GetUserByWeiXinId(requestMessage.FromUserName);
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

            return responseMessage;
        } 
        #endregion

        #region 提问处理
        /// <summary>
        /// 用户提问处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public ResponseMessageNews GetSubmitQuestionResponseMessage(RequestMessageImage requestMessage)
        {
            //用户提交问题处理
            return SubmitQuestionResponse(requestMessage);            
        }  
        
        /// <summary>
        /// 提问返回数据处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private ResponseMessageNews SubmitQuestionResponse(RequestMessageImage requestMessage)
        {
            return QuestionResponse<RequestMessageImage>(requestMessage, SubmitQuestion(requestMessage));            
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
            Guid mid = UserQAService.GetMsgIdCache(msgid);
            if (mid == Guid.Empty)
            {//已经添加的问题答案，不再保存进系统
                UserQAService.SetMsgIdCache(msgid, qaid);

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
                    var bmOU = new UserQADistributionService().GetQADistribution(true);
                    qaModel.WeiXinId = bmOU == null ? CFG.默认收费问题微信号 : bmOU.WeiXinId;
                }
                else
                {
                    //免费问题的分配
                    var bmOU = new UserQADistributionService().GetQADistribution(false);
                    qaModel.WeiXinId = bmOU == null ? CFG.默认免费问题微信号 : bmOU.WeiXinId;
                }
                //判断缓存里保存的问答ID是否是当前的对象ID    
                if (UserQAService.GetMsgIdCache(msgid) == model.ID)
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
                if (UserQAService.GetMsgIdCache(msgid) != model.ID)
                {
                    System.Threading.Thread.Sleep(500);
                    i++;
                }
                model = bll.All.Where(p => p.MsgId == msgid).FirstOrDefault();                
            } while (model.AutoGrenteId == 0 || i > 20);
            return model;
        }
        #endregion

        #region 用户取问题
        /// <summary>
        /// 按时间逆序获取用户提问的问题
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="skipNum"></param>
        /// <returns></returns>
        public ResponseMessageNews GetQuestionResponseMessage(RequestMessageText requestMessage)
        {
            //用户提交问题处理
            var skipNum = 1;
            var text = requestMessage.Content;
            try 
            { 
                if (text.Contains(" "))
                {
                    var commond = text.Substring(0, text.IndexOf(" "));
                    var numValue = text.Substring(commond.Length + 1, text.Length - commond.Length - 1).Replace(" ","");
                    skipNum = Convert.ToInt32(String.IsNullOrEmpty(numValue) ? "1" : numValue);
                }
            }
            catch
            {
                return new InvalidCommondService().GetResponseMessage(requestMessage as RequestMessageText); 
            }
            
            return GetQuestionResponse(requestMessage, skipNum);
        }

        /// <summary>
        /// 取问题返回数据处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private ResponseMessageNews GetQuestionResponse(RequestMessageText requestMessage, int skipNum)
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
            var questionCount = bll.All.Where(p => p.WeiXinId == requestMessage.FromUserName).Count();
            if (skipNum > questionCount)
                skipNum = questionCount - 1;
            var model = bll.All.Where(p => p.WeiXinId == requestMessage.FromUserName).OrderByDescending(p => p.RegTime).Skip(skipNum).Take(1).FirstOrDefault();
            return model;
        }

        #endregion
    }
}