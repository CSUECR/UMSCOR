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
        /// <param name="id"></param>
        /// <param name="tok"></param>
        /// <returns></returns>
        [HttpGet]
        public string UJS(string Tok,bool? Auto, List<Guid> UIds)
        {
            var rz = false;
            rz = IsRZ(Tok, rz);
            if (!rz)
                return "";
            var newAuList = new List<aspnet_UsersJson>();   
            var _auList = new BaseBll<aspnet_Users>().All.Where(p => p.UserId != null);
            if(Auto.Value)
            {
                var hours = 0 - Convert.ToDouble(CFG.邦马网_用户数据同步时间范围);
                var dt = DateTime.Now.AddHours(hours);
                _auList = _auList.Where(p => p.wmfUserInfo.RegTime > dt);
            }
            if(UIds != null && UIds.Count() > 0)
            {
                _auList = _auList.Where(p => UIds.Contains(p.UserId));
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

            var eys = EncodeJson(s);
            return eys;
        }        

        public string DC()
        {                        
            var id = UJS("",true,null);
            var s = DecodeJson(id);          
            var _list = JsonConvert.DeserializeObject<List<aspnet_Users>>(s);
            return "";
        }

        

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
