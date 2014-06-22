using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data.Linq;
using HOHO18.Common;
using System.Web;
namespace MorSun.Model
{
    //[Bind(Include = "UserId,formUserType,UserNo,NickName,TrueName,Sex,BirthDay,IdCard,Country,Province,City,Town,Address,PostCode,Account,Signature,EnableSign,Blog,Interest,EnablePhoto,QQ,TelPhone,MobilePhone,Fax")]
    public partial class wmfUploadFile : IModel
    {
        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnParentIdChanging(Guid value);
        #endregion

        public string PreUrl { get; set; }

        public virtual Guid? ProjectId { get; set; }

        public virtual Guid? DocumentId { get; set; }

        public string CheckedId { get; set; }

        public int x
        {
            get;
            set;
        }
        public int y
        {
            get;
            set;
        }
        public int w
        {
            get;
            set;
        }
        public int h
        {
            get;
            set;
        }

        public HttpPostedFileBase HttpUploadFile { get; set; }


        public bool IsValid
        {
            get { return (GetRuleViolations().Count() == 0); }
        }

        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            ParameterProcess.TrimParameter<wmfUploadFile>(this);

            if (HttpUploadFile == null)
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Users>("请选择图片"), "HttpUploadFile");
            //if (!Sex.HasValue)
            //    yield return new RuleViolation("性别必须选择", "Sex");
            //if (Sex.HasValue && Sex.Value > 3)
            //    yield return new RuleViolation("性别选择错误", "Sex");
            //if (String.IsNullOrEmpty(formUserType) || !ModelStateValidate.IsGuid(formUserType))
            //    yield return new RuleViolation("请选择用户类型", "UserType");
            //if (!String.IsNullOrEmpty(NickName) && NickName.Length > 25)
            //    yield return new RuleViolation("昵称长度不能超过25个字符", "NickName");

            //if (!String.IsNullOrEmpty(Country) && Country.Length > 25)
            //    yield return new RuleViolation("国家长度不能超过25个字符", "Country");
            //if (!String.IsNullOrEmpty(Address) && Address.Length > 25)
            //    yield return new RuleViolation("地址长度不能超过25个字符", "Address");
            //if (!String.IsNullOrEmpty(Signature) && Signature.Length > 50)
            //    yield return new RuleViolation("签名长度不能超过50个字符", "Signature");
            //if (!String.IsNullOrEmpty(Blog) && Blog.Length > 25)
            //    yield return new RuleViolation("博客长度不能超过25个字符", "Blog");
            //if (!String.IsNullOrEmpty(Interest) && Interest.Length > 25)
            //    yield return new RuleViolation("兴趣长度不能超过25个字符", "Interest");
            //if (!String.IsNullOrEmpty(PhotoLocation) && PhotoLocation.Length > 150)
            //    yield return new RuleViolation("头像地址长度不能超过150个字符", "PhotoLocation");
            //if (!String.IsNullOrEmpty(Fax) && Fax.Length > 25)
            //    yield return new RuleViolation("传真长度不能超过25个字符", "Fax");
            //if (BirthDay.HasValue && BirthDay < Convert.ToDateTime("1700-1-1 0:0:0"))
            //    yield return new RuleViolation("生日格式不正确", "IdCard");
            //if (!String.IsNullOrEmpty(IdCard) && !ModelStateValidate.IsIdcard(IdCard))
            //    yield return new RuleViolation("身份证格式不正确", "IdCard");
            //if (!String.IsNullOrEmpty(QQ) && !ModelStateValidate.IsQq(QQ))
            //    yield return new RuleViolation("QQ格式不正确", "QQ");
            //if (!String.IsNullOrEmpty(TelPhone) && !ModelStateValidate.IsPhone(TelPhone))
            //    yield return new RuleViolation("电话格式不正确", "TelPhone");
            //if (!String.IsNullOrEmpty(MobilePhone) && !ModelStateValidate.IsMobileMe(MobilePhone))
            //    yield return new RuleViolation("手机格式不正确", "MobilePhone");
            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }
}
