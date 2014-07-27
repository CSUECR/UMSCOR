using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace MvcValidation.Extension
{
    public sealed class NumAttribute : ValidationAttribute, IClientValidatable
    {
        public const string reg = @"^[0-9]*$";

        public NumAttribute()
        {
        }

        //重写基类方法
        public override bool IsValid(object value)
        {
            if (value == null)
                return true;

            if (value is string)
            {
                Regex regEx = new Regex(reg, RegexOptions.Singleline);
                return regEx.IsMatch(value.ToString());
            }
            return false;
        }

        public System.Collections.Generic.IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule rule = new ModelClientValidationRule
            {
                ValidationType = "num",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };
            yield return rule;
        }
    }
}