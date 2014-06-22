using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HOHO18.Common;
using System.Data.Linq;

namespace MorSun.Model
{
    public partial class wmfVillage : IModel
    {
        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnParentIdChanging(Guid value);
        #endregion

        public string CheckedId { get; set; }
        public Guid ProvinceId { get; set; }
        public Guid CityId { get; set; }
        public Guid CountyId { get; set; }
        public bool IsValid
        {
            get { return (GetRuleViolations().Count() == 0); }
        }

        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            ParameterProcess.TrimParameter<wmfVillage>(this);

            if (string.IsNullOrEmpty(VillageName))
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfProvince>("村名不能为空"), "VillageName");
            if (!String.IsNullOrEmpty(VillageName) && ModelStateValidate.IsNotEmpty(VillageName.ToString()) && VillageName.ToString().Length > 15)
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("村名长度不可大于15个字符"), "VillageName");

            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }
}
