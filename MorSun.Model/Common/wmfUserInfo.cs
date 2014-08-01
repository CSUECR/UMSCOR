using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data.Linq;
using HOHO18.Common;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace MorSun.Model
{
    [MetadataType(typeof(wmfUserInfoMetadata))]
    public partial class wmfUserInfo : IModel
    {
        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnParentIdChanging(Guid value);
        #endregion

        public string formProvince
        {
            get;
            set;
        }

        public string formCity
        {
            get;
            set;
        }

        public string formTown
        {
            get;
            set;
        }

        public string uName
        {
            get;
            set;
        }

        public Guid deptId { get; set; }



        /// <summary>
        /// 创建用户的时候可选择网点和职位
        /// </summary>
        public string formUserType { get; set; }
        public bool IsValid
        {
            get { return (GetRuleViolations().Count() == 0); }
        }

        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            ParameterProcess.TrimParameter<wmfUserInfo>(this);

            if (String.IsNullOrEmpty(TrueName) || TrueName == "")
                yield return new RuleViolation("真实姓名不能为空", "TrueName");
            
            if (!String.IsNullOrEmpty(TrueName) && TrueName.Length > 25)
                yield return new RuleViolation("真实姓名长度不能超过25个字符", "TrueName");
            
            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }

    public class ConfirmQuestionModel
    {
        [Display(Name = "问题1")]
        [StringLength(25, ErrorMessage = "问题1长度请控制在25个字符内")]
        public System.String Question1 { get; set; }

        [Display(Name = "答案1")]
        [StringLength(25, ErrorMessage = "答案1长度请控制在25个字符内")]
        public System.String Answer1 { get; set; }

        [Display(Name = "问题2")]
        [StringLength(25, ErrorMessage = "问题2长度请控制在25个字符内")]
        public System.String Question2 { get; set; }

        [Display(Name = "答案2")]
        [StringLength(25, ErrorMessage = "答案2长度请控制在25个字符内")]
        public System.String Answer2 { get; set; }

        [Display(Name = "问题3")]
        [StringLength(25, ErrorMessage = "问题3长度请控制在25个字符内")]
        public System.String Question3 { get; set; }

        [Display(Name = "答案3")]
        [StringLength(25, ErrorMessage = "答案3长度请控制在25个字符内")]
        public System.String Answer3 { get; set; }

        public System.String uName { get; set; }
    }


    public class wmfUserInfoMetadata
    {
        [Display(Name = "问题1")]
        [StringLength(25, ErrorMessage = "问题1长度请控制在25个字符内")]
        public System.String Question1;

        [Display(Name = "答案1")]
        [StringLength(25, ErrorMessage = "答案1长度请控制在25个字符内")]
        public System.String Answer1;

        [Display(Name = "问题2")]
        [StringLength(25, ErrorMessage = "问题2长度请控制在25个字符内")]
        public System.String Question2;

        [Display(Name = "答案2")]
        [StringLength(25, ErrorMessage = "答案2长度请控制在25个字符内")]
        public System.String Answer2;

        [Display(Name = "问题3")]
        [StringLength(25, ErrorMessage = "问题3长度请控制在25个字符内")]
        public System.String Question3;

        [Display(Name = "答案3")]
        [StringLength(25, ErrorMessage = "答案3长度请控制在25个字符内")]
        public System.String Answer3;
    }
}
