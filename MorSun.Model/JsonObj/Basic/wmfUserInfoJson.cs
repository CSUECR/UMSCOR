using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web.Mvc;
using HOHO18.Common;

namespace MorSun.Model
{
    public class wmfUserInfoJson
    {
        public Guid ID
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long AutoGeneticId
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SID
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ValidateCode
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserPassword
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OperatePassword
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? LastLogTime
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ChangePasswordGenerateTime
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? UserType
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserNo
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NickName
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TrueName
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MobilePhone
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EMail
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string WeChatId
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Sex
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? BirthDay
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IdCard
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Country
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? ProvinceId
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? CityId
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? CountyId
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Address
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? RollMachine
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CheckNumber
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsNoCheck
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? FlagWorker
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? FlagActive
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InviteCode
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string HamInviteCode
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeInviteCode
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? InviteUser
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserNameString
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PassWordString
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? GetPassWordTime
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Question1
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Answer1
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Question2
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Answer2
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Question3
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Answer3
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? CertificationLevel
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? RegTime
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ModTime
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool FlagTrashed
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool FlagDeleted
        { get; set; }
    }

}