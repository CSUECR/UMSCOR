using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HOHO18.Common;
using System.Data.Linq;
using System.ComponentModel.DataAnnotations;

namespace MorSun.Model
{
    [MetadataType(typeof(wmfNavigationLinkMetadata))]
    public partial class wmfNavigationLink : IModel
    {
        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnParentIdChanging(Guid value);
        #endregion

        public string CheckedId { get; set; }

        /// <summary>
        /// 被选中的资源编号
        /// </summary>
        public Guid[] RIds { get; set; }

        public Guid yRefId { get; set; }

        public string NavLinkTree
        {
            get;
            set;
        }

        public bool IsValid
        {
            get { return (GetRuleViolations().Count() == 0); }
        }

        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            ParameterProcess.TrimParameter<wmfNavigationLink>(this);

            //if (RefId == null || RefId == Guid.Empty)
            //    yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfNavigationLink>("请选择类别"), "RefId");
            //if (String.IsNullOrEmpty(MenuName))
            //    yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfNavigationLink>("输入菜单名称"), "MenuName");
            //if (String.IsNullOrEmpty(Icon))
            //    yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfNavigationLink>("输入图标"), "Icon");
            //if (String.IsNullOrEmpty(URL))
            //    yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfNavigationLink>("输入URL"), "URL");
            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }


    public class wmfNavigationLinkMetadata
    {
        [Display(Name = "导航菜单")]
        [Required(ErrorMessage = "{0}必选")]
        public System.Guid RefId;
        [Display(Name = "父节点")]
        public System.String ParentId;
        [Display(Name = "导航名称")]
        [Required(ErrorMessage = "{0}必填")]
        public System.String MenuName;
        [Display(Name = "图标")]
        [StringLength(50, ErrorMessage = "图标长度不可超过50")]
        public System.String Icon;
        [Display(Name = "路径")]
        [StringLength(500, ErrorMessage = "路径长度不可超过500")]
        public System.String URL;
        [Display(Name = "资源")]
        public System.String RIds;
    }

}
