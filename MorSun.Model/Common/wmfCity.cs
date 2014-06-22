using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HOHO18.Common;
using System.Data.Linq;

namespace MorSun.Model
{
    public partial class wmfCity : IModel
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
            ParameterProcess.TrimParameter<wmfCity>(this);

            if (string.IsNullOrEmpty(CityName))
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfProvince>("市名不能为空"), "VillageName");
            if (!String.IsNullOrEmpty(CityName) && ModelStateValidate.IsNotEmpty(CityName.ToString()) && CityName.ToString().Length > 15)
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("市名长度不可大于15个字符"), "CityName");
            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }
}
