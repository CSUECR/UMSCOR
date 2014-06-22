using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web.Mvc;
using HOHO18.Common;
namespace MorSun.Model
{
    //[Bind]
    public partial class wmfPosition : IModel
    {

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnParentIdChanging(Guid value);

        public string CheckedId { get; set; }
        #endregion

        public bool IsValid
        {
            get { return (GetRuleViolations().Count() == 0); }
        }


        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            ParameterProcess.TrimParameter<wmfPosition>(this);

            if (PositionName == null || String.IsNullOrEmpty(PositionName.Trim()))
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfPosition>("岗位名为空"), "PositionName");
            else
            {
                if (!String.IsNullOrEmpty(PositionName.Trim()) && !ModelStateValidate.IsChineseLetter(PositionName.Trim()))
                    yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfPosition>("岗位名过滤"), "PositionName");

                if (!String.IsNullOrEmpty(PositionName) && PositionName.Length > 20)
                    yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfPosition>("岗位名长度"), "PositionName");
            }
            //if (DeptId == null)
            //    yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfPosition>("岗位名所属部门"), "DeptId");
            yield break;
        }

        //partial void OnValidate(ChangeAction action)
        //{
        //    if (!IsValid)
        //        throw new ApplicationException("Rule violations prevent saving");
        //}
    }

}