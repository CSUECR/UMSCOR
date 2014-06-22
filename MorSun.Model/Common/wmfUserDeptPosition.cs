using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web.Mvc;
using HOHO18.Common;

namespace MorSun.Model
{
    //[Bind(Include = "formDeptId,formPositionId,formUserId")]
    public partial class wmfUserDeptPosition : IModel
    {

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnParentIdChanging(Guid value);        
        #endregion
        public string formDeptId { get; set; }

        public string formPositionId { get; set; }

        public string formUserId { get; set; }

        public bool IsValid
        {
            get { return (GetRuleViolations().Count() == 0); }
        }      

        public IEnumerable<RuleViolation> GetRuleViolations()
        {

            ParameterProcess.TrimParameter<wmfUserDeptPosition>(this);

            if (String.IsNullOrEmpty(formPositionId) || !ModelStateValidate.IsGuid(formPositionId))
                yield return new RuleViolation("请选择职位", "PositionName");

            //if (String.IsNullOrEmpty(formDeptId) || !ModelStateValidate.IsGuid(formDeptId))
            //    yield return new RuleViolation("请选择网点", "DeptName");

            if (String.IsNullOrEmpty(formUserId) || !ModelStateValidate.IsGuid(formUserId))
                yield return new RuleViolation("请选择人员", "UserId");
            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }

}