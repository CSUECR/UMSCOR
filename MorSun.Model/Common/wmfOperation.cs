using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data.Linq;
using HOHO18.Common;
namespace MorSun.Model
{
    //[Bind(Include = "OperationName,OperationCNName,Description")]
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
            if (String.IsNullOrEmpty(OperationCNName))
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfOperation>("操作名称不能为空"), "OperationCNName");

            if (!String.IsNullOrEmpty(OperationCNName) && OperationCNName.Length > 25)
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfOperation>("操作名称长度不可大于25个字符"), "OperationCNName");

            if (!String.IsNullOrEmpty(Description) && Description.Length > 50)
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfOperation>("描述长度不可大于50个字符"), "Description");

            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }

}
