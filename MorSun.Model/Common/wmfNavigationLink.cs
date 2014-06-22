using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HOHO18.Common;
using System.Data.Linq;

namespace MorSun.Model
{
    public partial class wmfNavigationLink : IModel
    {
        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnParentIdChanging(Guid value);
        #endregion

        public string CheckedId { get; set; }

        /// <summary>
        /// 被选中的资源编号
        /// </summary>
        public Guid[] RIds { get; set; }

        public bool IsValid
        {
            get { return (GetRuleViolations().Count() == 0); }
        }

        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            ParameterProcess.TrimParameter<wmfNavigationLink>(this);

            if (RefId == null || RefId == Guid.Empty)
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfNavigationLink>("请选择类别"), "RefId");
            if (String.IsNullOrEmpty(MenuName))
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfNavigationLink>("输入菜单名称"), "MenuName");
            if (String.IsNullOrEmpty(Icon))
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfNavigationLink>("输入图标"), "Icon");
            if (String.IsNullOrEmpty(URL))
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfNavigationLink>("输入URL"), "URL");
            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }
}
