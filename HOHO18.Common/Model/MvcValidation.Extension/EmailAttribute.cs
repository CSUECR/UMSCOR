using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace MvcValidation.Extension
{
    public sealed class EmailAttribute : ValidationAttribute, IClientValidatable
    {
        public const string reg = @"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]*$";

        public EmailAttribute()
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
                ValidationType = "email",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };
            yield return rule;
        }
    }
}