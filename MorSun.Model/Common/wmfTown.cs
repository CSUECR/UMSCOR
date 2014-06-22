using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using HOHO18.Common;

namespace MorSun.Model
{
    public partial class wmfTown : IModel
    {
        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnParentIdChanging(Guid value);
        #endregion

        public Guid ProvinceId { get; set; }
        public Guid CityId { get; set; }
        public string CheckedId { get; set; }

        public bool IsValid
        {
            get { return (GetRuleViolations().Count() == 0); }
        }

        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            ParameterProcess.TrimParameter<wmfTown>(this);

            if (string.IsNullOrEmpty(TownName))
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfProvince>("镇名不能为空"), "TownName");
            if (!String.IsNullOrEmpty(TownName) && ModelStateValidate.IsNotEmpty(TownName.ToString()) && TownName.ToString().Length > 15)
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("镇名长度不可大于15个字符"), "TownName");

            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }
}
