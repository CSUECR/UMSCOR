using HOHO18.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MvcValidation.Extension;
using System.Linq;

namespace MorSun.Model
{

    #region 模型
    [PropertiesMustMatch("NewPassword", "ConfirmPassword", ErrorMessage = "新密码和确认密码不匹配")]
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "{0}必填")]
        [DataType(DataType.Password)]
        [DisplayName("当前密码")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [DisplayName("新密码")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [DataType(DataType.Password)]
        [DisplayName("确认新密码")]
        public string ConfirmPassword { get; set; }
    }

    [PropertiesMustMatch("NewPassword", "ConfirmPassword", ErrorMessage = "新密码和确认密码不匹配")]
    public class ECPWModel
    {      

        [Required(ErrorMessage = "{0}必填")]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [DisplayName("新密码")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [DataType(DataType.Password)]
        [DisplayName("确认新密码")]
        public string ConfirmPassword { get; set; }

        public string id { get; set; }
    }

    public class ForgetModel
    {
        [Required(ErrorMessage = "{0}必填")]
        [DisplayName("常用电子邮件")]
        [Email(ErrorMessage = "请输入正确的邮件格式")]
        [Remote("CheckUserNameTrue", "Account", ErrorMessage = "该邮件不存在")]   
        public string UserName { get; set; }
        [Required(ErrorMessage = "{0}必填")]
        [DisplayName("验证码")]
        public string Verifycode { get; set; }

        [DisplayName("验证码保存字段")]
        public string VerifycodeRandom { get; set; }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "{0}必填")]
        [DisplayName("常用电子邮件")]
        [Email(ErrorMessage = "请输入正确的邮件格式")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [DataType(DataType.Password)]
        [DisplayName("密码")]
        public string Password { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [DisplayName("验证码")]
        public string Verifycode { get; set; }

        [DisplayName("验证码保存字段")]
        public string VerifycodeRandom { get; set; }

        [DisplayName("记住我?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }

    [PropertiesMustMatch("Password", "ConfirmPassword", ErrorMessage = "密码和确认密码不匹配")]
    public class RegisterModel
    {
        [Required(ErrorMessage = "{0}必填")]
        [StringLength(50,MinimumLength=6,ErrorMessage="邮件长度控制在6~50个字符之间")]
        [Remote("CheckUserName","Account",ErrorMessage="该邮件不能注册")]        
        [Email(ErrorMessage="请输入正确的邮件格式")]
        //[Num(ErrorMessage = "只能录入数字")]
        //[NumLetter(ErrorMessage = "只能录入数字字母")]        
        [DisplayName("常用电子邮件")]
        public string UserName { get; set; }

        [DisplayName("昵称")]
        [StringLength(25, ErrorMessage = "昵称长度请控制在25个字符内")]
        public string NickName { get; set; }

        //[Required(ErrorMessage = "{0}必填")]
        [DataType(DataType.EmailAddress)]
        [DisplayName("电子邮件地址")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [StringLength(25, MinimumLength = 6, ErrorMessage = "密码长度控制在6~25个字符之间")]
        [DataType(DataType.Password)]
        [DisplayName("密码")]        
        public string Password { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [DataType(DataType.Password)]
        [DisplayName("确认密码")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [DisplayName("验证码")]
        public string Verifycode { get; set; }

        [DisplayName("验证码保存字段")]
        public string VerifycodeRandom { get; set; }

        [DisplayName("被邀请码")]
        [StringLength(200, ErrorMessage = "邀请码长度超过200个字符")]
        public string BeInviteCode { get; set; }

        public Guid? Uid { get; set; }
    }

    /// <summary>
    /// 用户类
    /// </summary>
    public class UInfo
    {
        [Required(ErrorMessage = "{0}必填")]
        [DisplayName("昵称")]
        [StringLength(25, ErrorMessage = "昵称长度请控制在25个字符内")]
        public string NickName { get; set; }

        public bool IsBoundZYB { get; set; }

        [DisplayName("微信发送绑定代码")]
        public string BoundCode { get; set; }
    }

    public class UserCL
    {
        [Required(ErrorMessage = "{0}必填")]
        [DisplayName("用户ID")]
        public Guid UserId { get; set; }

        [DisplayName("昵称")]
        public string NickName { get; set; }

        [DisplayName("用户名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [DisplayName("认证级别")]
        public Guid? CLevel { get; set; }
    }

    /// <summary>
    /// 充值
    /// </summary>
    public class Recharge
    {
        [Required(ErrorMessage = "{0}必填")]
        [DisplayName("卡密")]
        [StringLength(80, MinimumLength = 80, ErrorMessage = "请输入80个字符的卡密，或者请直接复制并粘贴到文本框里")]
        public string KaMe { get; set; }

        public IQueryable<bmRecharge> rList { get; set; }
    }

    /// <summary>
    /// 提取卡密
    /// </summary>
    public class TKM
    {
        [Display(Name = "订单编号")]
        [Required(ErrorMessage = "{0}必选")]
        public System.String OrderNum;

        [Display(Name = "卡密")]        
        public System.String KaMe;

        [Display(Name = "买家旺旺名")]
        [Required(ErrorMessage = "{0}必填")]
        public System.String Buyer;
    }

    /// <summary>
    /// 取现
    /// </summary>
    public class Take
    {
        [Required(ErrorMessage = "{0}必填")]
        [DisplayName("币值")]        
        [RangeAttribute(50000,50000000,ErrorMessage="输入范围从50000到50000000，并且是50000的整数倍")]
        public decimal MaBiNum { get; set; }

        [DisplayName("备注")]
        [StringLength(200, ErrorMessage = "字符限制在200字以内")]
        public string UserRemark { get; set; }

        public IQueryable<bmTakeNow> tList { get; set; }

        public IQueryable<bmTakeNow> thisWeakTake { get; set; }
    }

    public class MaBiSettle
    {
        public IQueryable<bmUserMaBiSettleRecord> mabiList { get; set; }
        public IQueryable<bmUserMaBiSettleRecord> bbiList { get; set; }
        public IQueryable<bmUserMaBiSettleRecord> banbiList { get; set; }
    }

    /// <summary>
    /// 首页模型
    /// </summary>
    public class IndexModel
    {
        //通知列表
        public IQueryable<bmNew> nList { get; set; }
    }

    //public class UserMaBi
    //{
    //    public decimal MaBi { get; set; }
    //    public decimal BBi { get; set; }
    //    public decimal BanBi { get; set; }
    //}

    public class AddMBRModel
    {
        public AddMBRModel()
        {
            this.UIds = new List<Guid>();    
        }
        public List<Guid> UIds { get; set; }
        public Guid SR { get; set; }

        public Guid MBR { get; set; }

        public decimal MBN { get; set; }

        public Guid QAId { get; set; }
    }

    public class AddObjection
    {
        [Display(Name = "问题")]
        [Required(ErrorMessage = "{0}必选")]
        public System.Guid QAId { get; set; }

        [Display(Name = "错题数量")]
        [Required(ErrorMessage = "{0}必填")]
        public System.Int32 ErrorNum { get; set; }

        [Display(Name = "异议说明")]
        [Required(ErrorMessage = "{0}必填")]
        public System.String ObjectionExplain { get; set; }

        public string ReturnUrl { get; set; }
    }    
    #endregion

    #region Services
    // FormsAuthentication 类型是密封的且包含静态成员，因此很难对
    // 调用其成员的代码进行单元测试。下面的接口和 Helper 类演示
    // 如何围绕这种类型创建一个抽象包装，以便可以对 AccountController
    // 代码进行单元测试。

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email, Guid? uid);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
    }

    public class AccountMembershipService : IMembershipService
    {
        private readonly MembershipProvider _provider;

        public AccountMembershipService()
            : this(null)
        {
        }

        public AccountMembershipService(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("值不能为 null 或为空。", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("值不能为 null 或为空。", "password");

            return _provider.ValidateUser(userName, password);
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email, Guid? uid)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("值不能为 null 或为空。", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("值不能为 null 或为空。", "password");
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("值不能为 null 或为空。", "email");

            if (uid == null)
                uid = Guid.NewGuid();
            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, email, null, null, true, uid, out status);//自定义UID
            return status;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("值不能为 null 或为空。", "userName");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("值不能为 null 或为空。", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("值不能为 null 或为空。", "newPassword");

            // 在某些出错情况下，基础 ChangePassword() 将引发异常，
            // 而不是返回 false。
            try
            {
                MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
                return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }
    }

    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("值不能为 null 或为空。", "userName");

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
    #endregion

    #region Validation
    public static class AccountValidation
    {
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // 请参见 http://go.microsoft.com/fwlink/?LinkID=177550 以查看
            // 状态代码的完整列表。
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "用户名已存在。请另输入一个用户名。";

                case MembershipCreateStatus.DuplicateEmail:
                    return "已存在与该电子邮件地址对应的用户名。请另输入一个电子邮件地址。";

                case MembershipCreateStatus.InvalidPassword:
                    return "提供的密码无效。请输入有效的密码值。";

                case MembershipCreateStatus.InvalidEmail:
                    return "提供的电子邮件地址无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidAnswer:
                    return "提供的密码取回答案无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidQuestion:
                    return "提供的密码取回问题无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidUserName:
                    return "提供的用户名无效。请检查该值并重试。";

                case MembershipCreateStatus.ProviderError:
                    return "身份验证提供程序返回了错误。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";

                case MembershipCreateStatus.UserRejected:
                    return "已取消用户创建请求。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";

                default:
                    return "发生未知错误。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class PropertiesMustMatchAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "'{0}' 和 '{1}' 不匹配。";
        private readonly object _typeId = new object();

        public PropertiesMustMatchAttribute(string originalProperty, string confirmProperty)
            : base(_defaultErrorMessage)
        {
            OriginalProperty = originalProperty;
            ConfirmProperty = confirmProperty;
        }

        public string ConfirmProperty { get; private set; }
        public string OriginalProperty { get; private set; }

        public override object TypeId
        {
            get
            {
                return _typeId;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
                OriginalProperty, ConfirmProperty);
        }

        public override bool IsValid(object value)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
            object originalValue = properties.Find(OriginalProperty, true /* ignoreCase */).GetValue(value);
            object confirmValue = properties.Find(ConfirmProperty, true /* ignoreCase */).GetValue(value);
            return Object.Equals(originalValue, confirmValue);
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidatePasswordLengthAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "'{0}' 必须至少包含 {1} 个字符。";
        private readonly int _minCharacters = Membership.Provider.MinRequiredPasswordLength;

        public ValidatePasswordLengthAttribute()
            : base(_defaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
                name, _minCharacters);
        }

        public override bool IsValid(object value)
        {
            string valueAsString = value as string;
            return (valueAsString != null && valueAsString.Length >= _minCharacters);
        }
    }
    #endregion


}
