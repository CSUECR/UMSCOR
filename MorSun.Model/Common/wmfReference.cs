using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data.Linq;
using HOHO18.Common;
namespace MorSun.Model
{
    //[Bind(Include = "ItemOrder,ItemValue,ItemInfo,RefGroupId")]
    public partial class wmfReference : IPModel
    {
        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnParentIdChanging(Guid value);
        #endregion

        //public Guid RefGroupId
        //{
        //    get;
        //    set;
        //}
        public string refgId { get; set; }
        public string CheckedId { get; set; }
        public bool IsValid
        {
            get { return (GetRuleViolations().Count() == 0); }
        }

        public string ReferenceTree
        {
            get;
            set;
        }

        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            ParameterProcess.TrimParameter<wmfReference>(this);

            if (RefGroupId == Guid.Empty)
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("类别组未选择!"), "RefGroupId");
            //if (ItemOrder <= 0)
            //    yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("项排序错误,必须是整数且最小为1"), "ItemOrder");
            //if (String.IsNullOrEmpty(ItemValue) || ModelStateValidate.IsEmpty(ItemValue))
            //    yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("项值不能为空"), "ItemValue");
            if (String.IsNullOrEmpty(ItemInfo) || ModelStateValidate.IsEmpty(ItemInfo.ToString().Trim()))
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("项值不能为空"), "ItemInfo");

            //if (!String.IsNullOrEmpty(ItemValue) && ModelStateValidate.IsNotEmpty(ItemValue.ToString()) && ItemValue.ToString().Length > 15)
            //    yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("项值长度不可大于15个字符"), "ItemValue");
            //if (!String.IsNullOrEmpty(ItemInfo) && ModelStateValidate.IsNotEmpty(ItemInfo.ToString()) && ItemInfo.ToString().Length > 15)
            //    yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("项信息长度不可大于15个字符"), "ItemInfo");

            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }

    }
}
