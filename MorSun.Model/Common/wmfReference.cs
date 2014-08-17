using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data.Linq;
using HOHO18.Common;
using System.ComponentModel.DataAnnotations;
namespace MorSun.Model
{
    [MetadataType(typeof(wmfReferenceMetadata))]
    public partial class wmfReference : IModel
    {
        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnParentIdChanging(Guid value);
        #endregion

        //public Guid RefGroupId
        //{
        //    get;
        //    set;
        //}
        public string refgId { get; set; }
        public string CheckedId { get; set; }
        public bool IsValid
        {
            get { return (GetRuleViolations().Count() == 0); }
        }

        public string ReferenceTree
        {
            get;
            set;
        }

        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            ParameterProcess.TrimParameter<wmfReference>(this);
            
            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }

    }

    public class wmfReferenceMetadata
    {
        [Display(Name = "类别组")]
        [Required(ErrorMessage = "{0}必选")]
        public System.String RefGroupId;
        [Display(Name = "类别名")]
        //[Required(ErrorMessage = "{0}必填,可用','分隔批量添加类别组")]
        public System.String ItemValue;
        [Display(Name = "类别信息")]
        [Required(ErrorMessage = "{0}必填,可用','分隔批量添加类别")]
        [StringLength(500, ErrorMessage = "类别信息长度不可超过500")]
        public System.String ItemInfo;
        [Display(Name = "查看路径")]
        [StringLength(500, ErrorMessage = "查看路径长度不可超过500")]
        public System.String SeeUrl;
        [Display(Name = "图标")]
        [StringLength(500, ErrorMessage = "图标长度不可超过500")]
        public System.String Icon;
    }
}
