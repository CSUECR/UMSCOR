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
    [MetadataType(typeof(wmfRefGroupMetadata))]
    public partial class wmfRefGroup : IModel
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

        public string RefGroupNameTree
        {
            get;
            set;
        }

        public string CheckedId { get; set; }

        public string isTree { get; set; }

        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            ParameterProcess.TrimParameter<wmfRefGroup>(this);

            if (String.IsNullOrEmpty(RefGroupName))
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfRefGroup>("类别组名为空"), "RefGroupName");
            
            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }

    public class wmfRefGroupMetadata
    {
        [Display(Name = "父节点")]        
        public System.String ParentId;
        [Display(Name = "类别组名")]
        [Required(ErrorMessage = "{0}必填,可用','分隔批量添加类别组")]        
        public System.String RefGroupName;
    }

}
