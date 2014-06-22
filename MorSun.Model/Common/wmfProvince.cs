using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HOHO18.Common;
using System.Data.Linq;

namespace MorSun.Model
{
    public partial class wmfProvince : IModel
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
            ParameterProcess.TrimParameter<wmfProvince>(this);

            if (string.IsNullOrEmpty(ProvinceName))
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfProvince>("省名不能为空"), "ProvinceName");
            if (!String.IsNullOrEmpty(ProvinceName) && ModelStateValidate.IsNotEmpty(ProvinceName.ToString()) && ProvinceName.ToString().Length > 15)
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("省名长度不可大于15个字符"), "ProvinceName");

            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }
}
