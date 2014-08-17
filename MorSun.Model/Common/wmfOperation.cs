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
    [MetadataType(typeof(wmfOperationMetadata))]
    public partial class wmfOperation : IModel
    {
        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnParentIdChanging(Guid value);
        #endregion

        public string CheckedId { get; set; }
       
        public bool IsValid
        {
            get { return (GetRuleViolations().Count() == 0); }
        }

        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            ParameterProcess.TrimParameter<wmfOperation>(this);            
            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }

    public class wmfOperationMetadata
    {        
        [Display(Name = "操作名")]
        [Required(ErrorMessage = "{0}必填,可用','分隔批量添加资源")]
        public System.String OperationCNName;        
    }
}
