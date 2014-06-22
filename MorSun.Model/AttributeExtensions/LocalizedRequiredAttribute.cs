using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MorSun.Model;
namespace System.ComponentModel.DataAnnotations
{
    public class LocalizedRequiredAttribute : ValidationAttribute,IClientValidatable
    {
        public LocalizedRequiredAttribute()
        {
            this.ErrorMessage = XmlHelper.GetKeyNameValidation("GlobalSetting", "RequireErrorMsg");
        }
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                var valueStr = value.ToString().Trim();
                if (!string.IsNullOrEmpty(valueStr))
                    return true;
            }
            return false;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var errorMessage = FormatErrorMessage(metadata.GetDisplayName());
            var rule=new ModelClientValidationRequiredRule(errorMessage);
            yield return rule;
        }
    }
}
