using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web.Mvc;
using HOHO18.Common;

namespace MorSun.Model
{
    public class aspnet_MembershipJson
    {
        public Guid ApplicationId
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid UserId
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Password
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PasswordFormat
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PasswordSalt
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MobilePIN
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Email
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LoweredEmail
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PasswordQuestion
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PasswordAnswer
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsApproved
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsLockedOut
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime LastLoginDate
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime LastPasswordChangedDate
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime LastLockoutDate
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int FailedPasswordAttemptCount
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime FailedPasswordAttemptWindowStart
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int FailedPasswordAnswerAttemptCount
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime FailedPasswordAnswerAttemptWindowStart
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Comment
        { get; set; }
    }

}