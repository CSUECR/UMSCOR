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
    [MetadataType(typeof(wmfResourceMetadata))]
    public partial class wmfResource : IModel
    {
        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnParentIdChanging(Guid value);
        #endregion

        public bool IsValid
        {
            get { return (GetRuleViolations().Count() == 0); }
        }

        public string ResourceTree
        {
            get;
            set;
        }

        public string isTree { get; set; }

        public string CheckedId { get; set; }

        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            ParameterProcess.TrimParameter<wmfResource>(this);
            if (String.IsNullOrEmpty(ResourceCNName))
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfResource>("资源名称不能为空"), "ResourceCNName");
            if (!String.IsNullOrEmpty(ResourceCNName) && ResourceCNName.Length > 15)
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfResource>("资源名称长度不可大于15个字符"), "ResourceCNName");
            if (!String.IsNullOrEmpty(Description) && Description.Length > 50)
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfResource>("描述长度不可大于50个字符"), "Description");
            //if (MenuId == null || MenuId == Guid.Empty)
            //    yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfResource>("请选择所属菜单"), "MenuId");
            yield break;
        }

        public virtual string TreeName { get; set; }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }


    public class wmfResourceMetadata
    {
        [Display(Name = "父节点")]
        public System.String ParentId;
        [Display(Name = "资源名")]
        [Required(ErrorMessage = "{0}必填,可用','分隔批量添加资源")]
        public System.String ResourceCNName;
        [Display(Name = "图标")]
        public System.String Icon;
        [Display(Name = "路径")]
        public System.String URL;
    }
}
