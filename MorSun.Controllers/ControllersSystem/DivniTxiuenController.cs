using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using MorSun.Bll;
using MorSun.Model;
using MorSun.Controllers.Filter;
using System.Data.Objects;
using MorSun.Common;
using System.Text;
using HOHO18.Common;
using MorSun.Controllers.ViewModel;
using System.Collections;
using MorSun.Common.Privelege;
using MorSun.Common.类别;
using HOHO18.Common.WEB;
using HOHO18.Common.SSO;
using MorSun.Common.配置;
using Newtonsoft.Json;
using HOHO18.Common.DEncrypt;

namespace MorSun.Controllers.SystemController
{
    /// <summary>
    /// 操作
    /// </summary>
    [HandleError]
    public class DivniTxiuenController : BaseController<bmSellKaMe>
    {
        protected override string ResourceId
        {
            get { return 资源.系统参数配置; }
        }
                
        /// <summary>
        /// 用户JSON数据
        /// </summary>
        /// <param name="Tok"></param>
        /// <param name="SyncDT"></param>
        /// <param name="UIds"></param>
        /// <returns></returns>
        
        public string UJS(string Tok,DateTime? SyncDT, string UIds)
        {
            LogHelper.Write("同步User", LogHelper.LogMessageType.Info);
            var rz = false;
            rz = IsRZ(Tok, rz, Request);
            if (!rz)
            {
                LogHelper.Write("未认证", LogHelper.LogMessageType.Info);
                return "";
            }
            var newAuList = new List<aspnet_UsersJson>();   
            var _auList = new BaseBll<aspnet_Users>().All.Where(p => p.UserId != null);
            //同步时间，未传递时，从定制的时间范围开始取，有传递时，从传递时间开始取。
            if (String.IsNullOrEmpty(UIds))
            {
                if (!SyncDT.HasValue)
                {
                    var hours = 0 - Convert.ToDouble(CFG.邦马网_用户数据同步时间范围);
                    var dt = DateTime.Now.AddHours(hours);
                    _auList = _auList.Where(p => p.wmfUserInfo.RegTime > dt);
                }
                else
                {
                    _auList = _auList.Where(p => p.wmfUserInfo.RegTime > SyncDT);
                }
            }
            if (!String.IsNullOrEmpty(UIds))
            {
                try
                {
                    var dcUids = SecurityHelper.Decrypt(UIds);
                    var uidArr = dcUids.ToGuidList(CFG.邦马网_字符串分隔符);
                    _auList = _auList.Where(p => uidArr.Contains(p.UserId));
                }
                catch
                {
                    LogHelper.Write("无法解密", LogHelper.LogMessageType.Info);
                    return "";
                }
            }
            
            if (_auList.Count() == 0)
                return "";    

            var s = "";

            foreach(var u in _auList)
            {
                var t = new aspnet_UsersJson
                {
                    ApplicationId = u.ApplicationId,
                    UserId = u.UserId,
                    UserName = u.UserName,
                    LoweredUserName = u.LoweredUserName,
                    MobileAlias = u.MobileAlias,
                    IsAnonymous = u.IsAnonymous,
                    LastActivityDate = u.LastActivityDate
                };
                newAuList.Add(t);
            }
            s += ToJsonAndCompress(newAuList);
            s += CFG.邦马网_JSON数据间隔;
            //各自独立压缩，不然会与间隔符冲突

            var _userids = _auList.Select(p => p.UserId);
            var _mbUsers = new BaseBll<aspnet_Membership>().All.Where(p => _userids.Contains(p.UserId));

            //MembershipJson List
            var newMbList = new List<aspnet_MembershipJson>();
            foreach(var u in _mbUsers)
            {
                var t = new aspnet_MembershipJson
                {
                    ApplicationId = u.ApplicationId,
                    UserId = u.UserId,
                    Password = u.Password,
                    PasswordFormat = u.PasswordFormat,
                    PasswordSalt = u.PasswordSalt,
                    MobilePIN = u.MobilePIN,
                    Email = u.Email,
                    LoweredEmail = u.LoweredEmail,
                    PasswordQuestion = u.PasswordQuestion,
                    PasswordAnswer = u.PasswordAnswer,
                    IsApproved = u.IsApproved,
                    IsLockedOut = u.IsLockedOut,
                    CreateDate = u.CreateDate,
                    LastLoginDate = u.LastLoginDate,
                    LastPasswordChangedDate = u.LastPasswordChangedDate,
                    LastLockoutDate = u.LastLockoutDate,
                    FailedPasswordAttemptCount = u.FailedPasswordAttemptCount,
                    FailedPasswordAttemptWindowStart = u.FailedPasswordAnswerAttemptWindowStart,
                    FailedPasswordAnswerAttemptCount = u.FailedPasswordAnswerAttemptCount,
                    FailedPasswordAnswerAttemptWindowStart = u.FailedPasswordAnswerAttemptWindowStart,
                    Comment = u.Comment
                };
                newMbList.Add(t);
            }

            s += ToJsonAndCompress(newMbList);
            s += CFG.邦马网_JSON数据间隔;

            var _uiUsers = new BaseBll<wmfUserInfo>().All.Where(p => _userids.Contains(p.ID));
            //UserInfoJson List
            var newUIList = new List<wmfUserInfoJson>();
            foreach(var u in _uiUsers)
            {
                var t = new wmfUserInfoJson
                {
                    ID = u.ID,
                    AutoGeneticId = u.AutoGeneticId,
                    SID = u.SID,
                    ValidateCode = u.ValidateCode,
                    UserPassword = u.UserPassword,
                    OperatePassword = u.OperatePassword,
                    LastLogTime = u.LastLogTime,
                    ChangePasswordGenerateTime = u.ChangePasswordGenerateTime,
                    UserType = u.UserType,
                    UserNo = u.UserNo,
                    NickName = u.NickName,
                    TrueName = u.TrueName,
                    MobilePhone = u.MobilePhone,
                    EMail = u.EMail,
                    WeChatId = u.WeChatId,
                    Sex = u.Sex,
                    BirthDay = u.BirthDay,
                    IdCard = u.IdCard,
                    Country = u.Country,
                    ProvinceId = u.ProvinceId,
                    CityId = u.CityId,
                    CountyId = u.CountyId,
                    Address = u.Address,
                    RollMachine = u.RollMachine,
                    CheckNumber = u.CheckNumber,
                    IsNoCheck = u.IsNoCheck,
                    FlagWorker = u.FlagWorker,
                    FlagActive = u.FlagActive,
                    InviteCode = u.InviteCode,
                    HamInviteCode = u.HamInviteCode,
                    BeInviteCode = u.BeInviteCode,
                    InviteUser = u.InviteUser,
                    UserNameString = u.UserNameString,
                    PassWordString = u.PassWordString,
                    GetPassWordTime = u.GetPassWordTime,
                    Question1 = u.Question1,
                    Answer1 = u.Answer1,
                    Question2 = u.Question2,
                    Answer2 = u.Answer2,
                    Question3 = u.Question3,
                    Answer3 = u.Answer3,
                    CertificationLevel = u.CertificationLevel,
                    RegTime = u.RegTime,
                    ModTime = u.ModTime,
                    FlagTrashed = u.FlagTrashed,
                    FlagDeleted = u.FlagDeleted

                };
                newUIList.Add(t);
            }
            s += ToJsonAndCompress(newUIList);
            s += CFG.邦马网_JSON数据间隔;

            var eys = EncodeJson(s);
            return eys;
        }


        /// <summary>
        /// 问题JSON数据
        /// </summary>
        /// <param name="Tok"></param>
        /// <param name="SyncDT"></param>
        /// <returns></returns>
        public string QAJS(string Tok, DateTime? SyncDT)
        {
            LogHelper.Write("同步QA", LogHelper.LogMessageType.Info);
            var rz = false;
            rz = IsRZ(Tok, rz, Request);
            if (!rz)
                return "";

            var s = "";
            //自动获取时的时间处理
            var dt = DateTime.Now;
            if (!SyncDT.HasValue)
            {
                var hours = 0 - Convert.ToDouble(CFG.邦马网_问题数据同步时间范围);
                dt = DateTime.Now.AddHours(hours);
            }

            #region QA数据获取
            var newQAList = new List<bmQAJson>();
            var _qaList = new BaseBll<bmQA>().All.Where(p => p.ID != null);
            //同步时间，未传递时，从定制的时间范围开始取，有传递时，从传递时间开始取。

            if (!SyncDT.HasValue)
            {                
                _qaList = _qaList.Where(p => p.RegTime > dt);
            }
            else
            {
                _qaList = _qaList.Where(p => p.RegTime > SyncDT);
            }

            if (_qaList.Count() == 0)
                s += " ";
            else
            {
                foreach (var u in _qaList)
                {
                    var t = new bmQAJson
                    {
                        ID=u.ID,
                        AutoGrenteId = u.AutoGrenteId,
                        ParentId=u.ParentId,
                        UserId=u.UserId,
                        WeiXinId=u.WeiXinId,
                        QARef=u.QARef,
                        QAContent=u.QAContent,
                        MsgId=u.MsgId,
                        MsgType=u.MsgType,
                        MediaId=u.MediaId,
                        PicUrl=u.PicUrl,
                        Sort=u.Sort,
                        RegUser=u.RegUser,
                        RegTime=u.RegTime,
                        ModTime=u.ModTime,
                        FlagTrashed=u.FlagTrashed,
                        FlagDeleted = u.FlagDeleted        
                    };
                    newQAList.Add(t);
                }
                s += ToJsonAndCompress(newQAList);                
            }
            s += CFG.邦马网_JSON数据间隔;
            #endregion

            #region 用户马币数据获取
            var newUMBList = new List<bmUserMaBiRecordJson>();
            var mbSourceZS = Guid.Parse(Reference.马币来源_赠送);
            var mbSourceXF = Guid.Parse(Reference.马币来源_消费);
            var _umbList = new BaseBll<bmUserMaBiRecord>().All.Where(p => p.SourceRef == mbSourceZS || p.SourceRef == mbSourceXF);
            //同步时间，未传递时，从定制的时间范围开始取，有传递时，从传递时间开始取。

            if (!SyncDT.HasValue)
            {
                _umbList = _umbList.Where(p => p.RegTime > dt);
            }
            else
            {
                _umbList = _umbList.Where(p => p.RegTime > SyncDT);
            }

            if (_umbList.Count() == 0)
                s += " ";
            else
            {
                foreach (var u in _umbList)
                {
                    var t = new bmUserMaBiRecordJson
                    {
                        ID = u.ID,                        
                        UserId = u.UserId,        
                        QAId = u.QAId,        
                        SourceRef = u.SourceRef,        
                        MaBiRef = u.MaBiRef,        
                        MaBiNum = u.MaBiNum,        
                        IsSettle = u.IsSettle,        
                        Sort = u.Sort,
                        RegUser = u.RegUser,
                        RegTime = u.RegTime,
                        ModTime = u.ModTime,
                        FlagTrashed = u.FlagTrashed,
                        FlagDeleted = u.FlagDeleted
                    };
                    newUMBList.Add(t);
                }
                s += ToJsonAndCompress(newUMBList);
            }
            s += CFG.邦马网_JSON数据间隔;
            #endregion

            #region QA问题分配记录获取
            var newQADisList = new List<bmQADistributionJson>();
            var resultBS = Guid.Parse(Reference.分配答题操作_不是问题);
            var resultJD = Guid.Parse(Reference.分配答题操作_已解答);
            var _qaDisList = new BaseBll<bmQADistribution>().All.Where(p => p.Result == resultBS || p.Result == resultJD);
            //同步时间，未传递时，从定制的时间范围开始取，有传递时，从传递时间开始取。

            if (!SyncDT.HasValue)
            {
                _qaDisList = _qaDisList.Where(p => p.OperateTime > dt);
            }
            else
            {
                _qaDisList = _qaDisList.Where(p => p.OperateTime > SyncDT);
            }

            if (_qaDisList.Count() == 0)
                s += " ";
            else
            {
                foreach (var u in _qaDisList)
                {
                    var t = new bmQADistributionJson
                    {
                        ID = u.ID,
                        QAId=u.QAId,
                        UserId=u.UserId,
                        WeiXinId=u.WeiXinId,
                        DistributionTime=u.DistributionTime,
                        OperateTime=u.OperateTime,
                        Result=u.Result,        
                        Sort = u.Sort,
                        RegUser = u.RegUser,
                        RegTime = u.RegTime,
                        ModTime = u.ModTime,
                        FlagTrashed = u.FlagTrashed,
                        FlagDeleted = u.FlagDeleted
                    };
                    newQADisList.Add(t);
                }
                s += ToJsonAndCompress(newQADisList);
            }
            s += CFG.邦马网_JSON数据间隔;
            #endregion

            #region 异议数据获取
            var newOBList = new List<bmObjectionJson>();
            var _obList = new BaseBll<bmObjection>().All.Where(p => p.ID != null);
            //同步时间，未传递时，从定制的时间范围开始取，有传递时，从传递时间开始取。

            if (!SyncDT.HasValue)
            {
                _obList = _obList.Where(p => p.RegTime > dt);
            }
            else
            {
                _obList = _obList.Where(p => p.RegTime > SyncDT);
            }

            if (_obList.Count() == 0)
                s += " ";
            else
            {
                foreach (var u in _obList)
                {
                    var t = new bmObjectionJson
                    {
                        ID = u.ID,
                        QAId = u.QAId,
                        UserId = u.UserId,
                        WeiXinId = u.WeiXinId,
                        SubmitTime = u.SubmitTime,
                        ObjectionExplain = u.ObjectionExplain,
                        HandleUser = u.HandleUser,
                        Result = u.Result,
                        HandleTime = u.HandleTime,
                        HandleExplain = u.HandleExplain,
                        Sort = u.Sort,
                        RegUser = u.RegUser,
                        RegTime = u.RegTime,
                        ModTime = u.ModTime,
                        FlagTrashed = u.FlagTrashed,
                        FlagDeleted = u.FlagDeleted
                    };
                    newOBList.Add(t);
                }
                s += ToJsonAndCompress(newOBList);
            }
            s += CFG.邦马网_JSON数据间隔;
            #endregion

            #region 用户微信绑定数据获取
            var newUWList = new List<bmUserWeixinJson>();
            var _uwList = new BaseBll<bmUserWeixin>().All.Where(p => p.ID != null);
            //同步时间，未传递时，从定制的时间范围开始取，有传递时，从传递时间开始取。

            if (!SyncDT.HasValue)
            {
                _uwList = _uwList.Where(p => p.RegTime > dt);
            }
            else
            {
                _uwList = _uwList.Where(p => p.RegTime > SyncDT);
            }

            if (_qaList.Count() == 0)
                s += " ";
            else
            {
                foreach (var u in _uwList)
                {
                    var t = new bmUserWeixinJson
                    {
                        ID = u.ID,
                        UserId = u.UserId,
                        WeiXinId = u.WeiXinId,
                        WeiXinAPP = u.WeiXinAPP,
                        Sort = u.Sort,
                        RegUser = u.RegUser,
                        RegTime = u.RegTime,
                        ModTime = u.ModTime,
                        FlagTrashed = u.FlagTrashed,
                        FlagDeleted = u.FlagDeleted
                    };
                    newUWList.Add(t);
                }
                s += ToJsonAndCompress(newUWList);                
            }
            s += CFG.邦马网_JSON数据间隔;
            #endregion

            #region 用户取现数据获取
            var newTNList = new List<bmTakeNowJson>();
            var _tkList = new BaseBll<bmTakeNow>().All.Where(p => p.Effective == null && p.TakeRef == null); //每次都取出未处理的取现记录，直到处理为止
            //同步时间，未传递时，从定制的时间范围开始取，有传递时，从传递时间开始取。
                       
            if (_qaList.Count() == 0)
                s += " ";
            else
            {
                foreach (var u in _tkList)
                {
                    var t = new bmTakeNowJson
                    {
                        ID = u.ID,
                        UserId = u.UserId,
                        MaBiNum = u.MaBiNum,
                        Effective = u.Effective,
                        TakeRef = u.TakeRef,
                        UserRemark = u.UserRemark,
                        BMExplain = u.BMExplain,
                        TakeTime = u.TakeTime,

                        Sort = u.Sort,
                        RegUser = u.RegUser,
                        RegTime = u.RegTime,
                        ModTime = u.ModTime,
                        FlagTrashed = u.FlagTrashed,
                        FlagDeleted = u.FlagDeleted
                    };
                    newTNList.Add(t);
                }
                s += ToJsonAndCompress(newTNList);
            }
            s += CFG.邦马网_JSON数据间隔;
            #endregion

            var eys = EncodeJson(s);
            return eys;
        }

        /// <summary>
        /// 充值JSON获取
        /// </summary>
        /// <param name="Tok"></param>
        /// <returns></returns>
        public string RCJS(string Tok)
        {
            LogHelper.Write("同步RC", LogHelper.LogMessageType.Debug);
            var rz = false;
            rz = IsRZ(Tok, rz, Request);
            if (!rz)
                return "";

            var s = "";

            #region RC数据获取
            var newRCList = new List<bmRechargeJson>();
            var wcz = Guid.Parse(Reference.卡密充值_未充值);
            var _rcList = new BaseBll<bmRecharge>().All.Where(p => p.Effective == null && (p.Recharge == null || p.Recharge == wcz));

            if (_rcList.Count() == 0)
                return "";
            else
            {
                foreach (var u in _rcList)
                {
                    var t = new bmRechargeJson
                    {
                        ID = u.ID,
                        KaMeRef = u.KaMeRef,
                        MaBiNum = u.MaBiNum,
                        KaMeUse = u.KaMeUse,
                        UserId = u.UserId,
                        KaMe = u.KaMe,
                        Effective = u.Effective,
                        Recharge = u.Recharge,
                        Sort = u.Sort,
                        RegUser = u.RegUser,
                        RegTime = u.RegTime,
                        ModTime = u.ModTime,
                        FlagTrashed = u.FlagTrashed,
                        FlagDeleted = u.FlagDeleted
                    };
                    newRCList.Add(t);
                }
                s += ToJsonAndCompress(newRCList);
            }
            s += CFG.邦马网_JSON数据间隔;
            #endregion

            var eys = EncodeJson(s);
            return eys;
        }

        /// <summary>
        /// 判断服务器是否可用
        /// </summary>
        /// <returns></returns>
        public string ServerIsOk()
        {
            return "true";
        }

        /// <summary>
        /// 用户卡密充值及认证
        /// </summary>
        /// <param name="Tok"></param>
        /// <param name="AncyData"></param>
        /// <returns></returns>
        public string AncyRCMB(string Tok, string AncyData)
        {            
            var rz = false;
            rz = IsRZ(Tok, rz, Request);
            if (!rz)
                return "";
            
            if(!String.IsNullOrEmpty(AncyData))
            {
                LogHelper.Write("开始同步充值记录与马币", LogHelper.LogMessageType.Debug);
                var s = "";
                try { s = DecodeJson(AncyData); }
                catch
                {
                    s = "";
                    LogHelper.Write("解密异常", LogHelper.LogMessageType.Info);
                }

                if (!String.IsNullOrEmpty(s))
                {       
                    var bmRC = s.Substring(0, s.IndexOf(CFG.邦马网_JSON数据间隔)).Trim();
                    s = s.Substring(s.IndexOf(CFG.邦马网_JSON数据间隔) + CFG.邦马网_JSON数据间隔.Length);
                    var bmMB = s.Substring(0, s.IndexOf(CFG.邦马网_JSON数据间隔)).Trim();
                    s = s.Substring(s.IndexOf(CFG.邦马网_JSON数据间隔) + CFG.邦马网_JSON数据间隔.Length);
                    var rzUId = s.Substring(0, s.IndexOf(CFG.邦马网_JSON数据间隔)).Trim();
                    s = s.Substring(s.IndexOf(CFG.邦马网_JSON数据间隔) + CFG.邦马网_JSON数据间隔.Length);   
                    
                    try
                    {
                        //用户充值记录
                        if (!String.IsNullOrEmpty(bmRC))
                        {
                            bmRC = Compression.DecompressString(bmRC);
                            var _list = JsonConvert.DeserializeObject<List<bmRecharge>>(bmRC);
                            if (_list.Count() > 0)
                            {
                                var aids = new List<Guid>();
                                aids = _list.Select(p => p.ID).ToList();
                                var bll = new BaseBll<bmRecharge>();
                                //取出待修改的充值记录
                                var dblist = bll.All.Where(p => aids.Contains(p.ID)); 
                                foreach (var l in dblist)
                                {
                                    //更新充值记录只要三个字段
                                    var jsl = _list.FirstOrDefault(p => p.ID == l.ID);
                                    l.Recharge = jsl.Recharge;
                                    l.Effective = jsl.Effective;
                                    l.KaMeRef = jsl.KaMeRef;
                                    l.MaBiNum = jsl.MaBiNum;
                                }
                                bll.UpdateChanges();
                            }
                        }
                        //马币记录
                        if (!String.IsNullOrEmpty(bmMB))
                        {
                            bmMB = Compression.DecompressString(bmMB);
                            var _list = JsonConvert.DeserializeObject<List<bmUserMaBiRecord>>(bmMB);
                            if (_list.Count() > 0)
                            {
                                var aids = new List<Guid>();
                                aids = _list.Select(p => p.ID).ToList();
                                var bll = new BaseBll<bmUserMaBiRecord>();
                                //过滤掉已经添加的数据                    
                                var alreadyQIds = bll.All.Where(p => aids.Contains(p.ID)).Select(p => p.ID);
                                aids = aids.Except(alreadyQIds).ToList();
                                _list = _list.Where(p => aids.Contains(p.ID)).ToList();
                                foreach (var l in _list)
                                {
                                    bll.Insert(l, false);
                                }
                                bll.UpdateChanges();
                            }
                        }                        
                        //用户认证
                        if (!String.IsNullOrEmpty(rzUId))
                        {
                            rzUId = Compression.DecompressString(rzUId);
                            var _list = JsonConvert.DeserializeObject<List<Guid>>(rzUId);
                            if (_list.Count() > 0)
                            {
                                //var aids = new List<Guid>();
                                //aids = _list.Select(p => p.ID).ToList();
                                var bll = new BaseBll<wmfUserInfo>();
                                //过滤掉已经添加的数据  
                                var rzUsers = bll.All.Where(p => _list.Contains(p.ID));
                                
                                if(rzUsers.Count() > 0)
                                {
                                    var rzUR = Guid.Parse(Reference.认证类别_认证邦主);
                                    foreach (var l in rzUsers)
                                    {
                                        l.CertificationLevel = rzUR;
                                    }
                                    bll.UpdateChanges();
                                }
                            }
                        }                        
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Write(ex.Message + "异常导致同步不成功", LogHelper.LogMessageType.Info);
                        return ex.Message + "异常导致同步不成功";
                    }
                }
                else
                {
                    LogHelper.Write("未传递同步数据", LogHelper.LogMessageType.Info);
                    return "未传递同步数据";
                } 
            }
            return "true";
        }

        /// <summary>
        /// 用户的认证邦主修改角色,使之可以充值
        /// </summary>
        /// <param name="Tok"></param>
        /// <param name="AncyData"></param>
        /// <returns></returns>
        public string AncyCU(string Tok, string AncyData)
        {
            var rz = false;
            rz = IsRZ(Tok, rz, Request);
            if (!rz)
                return "";

            if (!String.IsNullOrEmpty(AncyData))
            {
                LogHelper.Write("开始认证用户并修改角色", LogHelper.LogMessageType.Debug);
                var s = "";
                try { s = DecodeJson(AncyData); }
                catch
                {
                    s = "";
                    LogHelper.Write("解密异常", LogHelper.LogMessageType.Info);
                }

                if (!String.IsNullOrEmpty(s))
                {
                    var rzUId = s.Substring(0, s.IndexOf(CFG.邦马网_JSON数据间隔)).Trim();
                    s = s.Substring(s.IndexOf(CFG.邦马网_JSON数据间隔) + CFG.邦马网_JSON数据间隔.Length);
                    var boolRZ = s.Substring(0, s.IndexOf(CFG.邦马网_JSON数据间隔)).Trim();
                    s = s.Substring(s.IndexOf(CFG.邦马网_JSON数据间隔) + CFG.邦马网_JSON数据间隔.Length);

                    try
                    {
                        //用户认证
                        if (!String.IsNullOrEmpty(rzUId))
                        {
                            rzUId = Compression.DecompressString(rzUId);
                            var _list = JsonConvert.DeserializeObject<List<Guid>>(rzUId);
                            if (_list.Count() > 0)
                            {
                                //根据传过来的数据来修改角色
                                var roleId = CFG.注册默认角色;
                                //解压
                                boolRZ = Compression.DecompressString(boolRZ);
                                boolRZ = JsonConvert.DeserializeObject<string>(boolRZ);
                                //LogHelper.Write("认证boolRZ" + boolRZ, LogHelper.LogMessageType.Info);
                                if (boolRZ.Trim().ToLower().Eql("true"))
                                    roleId = CFG.作业邦认证默认角色;
                                //先删除用户的角色，再添加
                                var constr = "";
                                var roleBll = new BaseBll<aspnet_Roles>();
                                var bll = new BaseBll<wmfUserInfo>();
                                var rzUR = Guid.Parse(Reference.认证类别_认证邦主);
                                var nonrzUR = Guid.Parse(Reference.认证类别_未认证);
                                foreach(var u in _list)
                                {
                                    //LogHelper.Write("认证" + u, LogHelper.LogMessageType.Info);
                                    constr += @"DELETE FROM [aspnet_UsersInRoles] WHERE [UserId] = '" + u + "'";
                                    constr += @"Insert Into aspnet_UsersInRoles ([UserId],[RoleId])  VALUES ('" + u + "','" + roleId + "')";

                                    //修改用户信息表                                    
                                    //过滤掉已经添加的数据  
                                    var uinfo = bll.GetModel(u);

                                    if (uinfo != null)
                                    {
                                        if (boolRZ.Trim().ToLower().Eql("true"))
                                            uinfo.CertificationLevel = rzUR;
                                        else
                                            uinfo.CertificationLevel = nonrzUR;
                                    }
                                }
                                bll.UpdateChanges();
                                roleBll.Db.ExecuteStoreCommand(constr);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Write(ex.Message + "异常导致同步不成功", LogHelper.LogMessageType.Info);
                        return ex.Message + "异常导致同步不成功";
                    }
                }
                else
                {
                    LogHelper.Write("未传递同步数据", LogHelper.LogMessageType.Info);
                    return "未传递同步数据";
                }
            }
            return "true";
        }


        //public string DC()
        //{                        
        //    var id = UJS("",null,null);
        //    var s = DecodeJson(id);          
        //    var _list = JsonConvert.DeserializeObject<List<aspnet_Users>>(s);
        //    return "";
        //}
        

        /// <summary>
        /// 不让查询
        /// </summary>
        /// <returns></returns>
        public override ActionResult I()
        {
            return RedirectToAction("I", "H");
        }

        public override ActionResult Sort(string returnUrl)
        {
            return RedirectToAction("I", "H");
        }
        
        //编辑前验证
        protected override string OnEditCK(bmSellKaMe t)
        {            
            return "";
        }

        //创建前验证
        protected override string OnAddCK(bmSellKaMe t)
        {            
            return "true";
        }
        

        //删除前验证
        protected override string OnDelCk(bmSellKaMe t)
        {            
            return "";
        }        

    }
}
