using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web.Mvc;
using HOHO18.Common;
namespace MorSun.Model
{
    //[Bind]
    public partial class aspnet_Roles : IModel
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


        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            ParameterProcess.TrimParameter<aspnet_Roles>(this);
            RoleName.Clone();
            if (RoleName == null || String.IsNullOrEmpty(RoleName.Trim()))
                yield return "RoleName".ADE("请输入角色名");
            else
            {
                if (!String.IsNullOrEmpty(RoleName.Trim()) && !ModelStateValidate.IsChineseLetter(RoleName.Trim()))
                    yield return "RoleName".ADE("角色名应该是字母或汉字");

                if (!String.IsNullOrEmpty(RoleName) && RoleName.Length > 10)
                    yield return "RoleName".ADE("角色名长度不能超过10个字符"); 
            }

            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }

}