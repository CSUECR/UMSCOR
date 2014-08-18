using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web.Mvc;
using HOHO18.Common;
using System.ComponentModel.DataAnnotations;
namespace MorSun.Model
{
    [MetadataType(typeof(RoleMetadata))]
    public partial class aspnet_Roles : IModel
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
            ParameterProcess.TrimParameter<aspnet_Roles>(this);
            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }
    public class RoleMetadata
    {
        [Display(Name = "角色名")]
        [Required(ErrorMessage = "{0}必填")]
        [StringLength(10, ErrorMessage = "角色名长度不可超过10")]
        public System.String RoleName;        
    }
}