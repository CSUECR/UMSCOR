using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HOHO18.Common;
using System.Data.Linq;
using System.ComponentModel.DataAnnotations;

namespace MorSun.Model
{
    [MetadataType(typeof(bmSellKaMeMetadata))]
    public partial class bmSellKaMe : IModel
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
            ParameterProcess.TrimParameter<bmSellKaMe>(this);            
            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }


    public class bmSellKaMeMetadata
    {
        [Display(Name = "订单编号")]
        [Required(ErrorMessage = "{0}必选")]
        public System.String OrderNum;
        [Display(Name = "卡密")]
        [Required(ErrorMessage = "{0}必填")]
        public System.String KaMe;
        [Display(Name = "买家旺旺名")]
        [Required(ErrorMessage = "{0}必填")]
        public System.String Buyer;

        [Display(Name = "宝贝名称")]
        public System.String GoodsName;
        [Display(Name = "宝贝数量")]
        public System.String GoodsNum;
        [Display(Name = "是否已冲值")]
        public System.String Recharge;   
    }

}
