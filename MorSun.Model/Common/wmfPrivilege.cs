using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web.Mvc;
using HOHO18.Common;
using System.ComponentModel.DataAnnotations;

namespace MorSun.Model
{
    [MetadataType(typeof(wmfPrivilegeMetadata))]
    public partial class wmfPrivilege : IModel
    {

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnParentIdChanging(Guid value);
        #endregion

        public string CheckedId { get; set; }
        public string ResourcesTree { get; set; }

        public bool IsValid
        {
            get { return (GetRuleViolations().Count() == 0); }
        }

        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            ParameterProcess.TrimParameter<wmfPrivilege>(this);            
            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }

    public class wmfPrivilegeMetadata
    {
        [Display(Name = "资源")]
        [Required(ErrorMessage = "{0}必选")]
        public System.String ResourceId;
        [Display(Name = "操作")]
        [Required(ErrorMessage = "{0}必选")]
        public System.String OperationId;
    }

}