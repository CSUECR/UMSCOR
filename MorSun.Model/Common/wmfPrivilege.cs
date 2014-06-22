using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web.Mvc;
using HOHO18.Common;

namespace MorSun.Model
{
    //[Bind(Include = "OperationId,ResourcesId,PrivilegeName,PrivilegeCNName,DefaultValue,Description,Sort")]
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
            if (String.IsNullOrEmpty(PrivilegeCNName))
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfPrivilege>("权限名称不能为空"), "PrivilegeCNName");
            if (!String.IsNullOrEmpty(PrivilegeCNName) && PrivilegeCNName.Length > 25)
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfPrivilege>("权限名称长度不能超过25个字符"), "PrivilegeCNName");
            if (String.IsNullOrEmpty(ResourcesId.ToString()))
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfPrivilege>("请选择资源"), "ResourcesId");
            if (!String.IsNullOrEmpty(ResourcesId.ToString()) && !ModelStateValidate.IsGuid(ResourcesId.ToString()))
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfPrivilege>("请选择资源"), "ResourcesId");
            if (String.IsNullOrEmpty(OperationId.ToString()))
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfPrivilege>("请选择操作"), "OperationId");
            if (!String.IsNullOrEmpty(OperationId.ToString()) && !ModelStateValidate.IsGuid(OperationId.ToString()))
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfPrivilege>("请选择操作"), "OperationId");
            if (!String.IsNullOrEmpty(Description) && Description.Length > 50)
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfPrivilege>("描述长度不可大于50个字符"), "Description");
            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }

}