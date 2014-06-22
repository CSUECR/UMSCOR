﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data.Linq;
using HOHO18.Common;
namespace MorSun.Model
{
    //[Bind(Include = "RefGroupName")]
    public partial class wmfRefGroup : IModel
    {
        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnParentIdChanging(Guid value);
        #endregion

        public bool IsValid
        {
            get { return (GetRuleViolations().Count() == 0); }
        }

        public string RefGroupNameTree
        {
            get;
            set;
        }

        public string CheckedId { get; set; }

        public string isTree { get; set; }

        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            ParameterProcess.TrimParameter<wmfRefGroup>(this);

            if (String.IsNullOrEmpty(RefGroupName))
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfRefGroup>("类别组名为空"), "RefGroupName");

            if (!String.IsNullOrEmpty(RefGroupName) && RefGroupName.Length > 20)
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfRefGroup>("类别组名长度"), "RefGroupName");
            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }

}
