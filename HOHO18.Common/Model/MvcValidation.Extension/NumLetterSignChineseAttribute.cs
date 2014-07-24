using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace MvcValidation.Extension
{
    public sealed class NumLetterSignChineseAttribute : ValidationAttribute, IClientValidatable
    {
        public const string reg = @"^[~`!@#%&-_=\\]\\};:',|0-9A-Za-z\\u002E\\u0024\\u005E\\u007B\\u005B\\u0028\\u007C\\u0029\\u002A\\u002B\\u003F\\u005C\\u4E00-\\u9FA5\\uF900-\\uFA2D]*$";

        public NumLetterSignChineseAttribute()
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
                ValidationType = "numletterchinesesign",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };
            yield return rule;
        }
    }
}