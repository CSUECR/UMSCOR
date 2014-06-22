using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MorSun.Model
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class RequiredIfAttribute : ValidationAttribute, IClientValidatable
    {
        private RequiredAttribute required = new RequiredAttribute();

        private List<object> targetValues = new List<object>();
        /// <summary>
        /// 依赖于哪个属性
        /// <remarks>必须是对象下的一个属性</remarks>
        /// </summary>
        public string DependentProperty { get; set; }

        public RequiredIfAttribute(string dependentProperty, object targetValue)
            : this(dependentProperty, targetValue, null)
        {

        }

        public RequiredIfAttribute(string dependentProperty, params object[] targetValues)
            : this(dependentProperty, targetValues, null)
        {
        }

        public RequiredIfAttribute(string dependentProperty, object targetValue, string errorMessage)
            : base(errorMessage)
        {
            this.DependentProperty = dependentProperty;
            this.targetValues.Add(targetValue);
        }



        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule() { ErrorMessage=this.FormatErrorMessage(metadata.GetDisplayName()), ValidationType="requiredif" };

            var tvs = this.targetValues.Select(v =>
                {
                    if (v.GetType() == typeof(bool))
                        return v.ToString().ToLower();
                    else
                        return v.ToString();
                });

            var ser = new JavaScriptSerializer();
            var values = ser.Serialize(tvs);
            //只能是小写
            rule.ValidationParameters.Add("dependencyvalue", values);
            rule.ValidationParameters.Add("dependency",string.Format("*.{0}",this.DependentProperty));

            yield return rule;
        }

        public override string FormatErrorMessage(string name)
        {
            if (!string.IsNullOrEmpty(this.ErrorMessageString))
                required.ErrorMessage = this.ErrorMessageString;

            return required.FormatErrorMessage(name);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var containerType = validationContext.ObjectInstance.GetType();
            var field = containerType.GetProperty(this.DependentProperty);
            if (field == null)
                throw new MissingMemberException(containerType.Name, this.DependentProperty);

            var dependentValue = field.GetValue(validationContext.ObjectInstance, null);

            if ((dependentValue == null && (this.targetValues == null || this.targetValues.Count == 0))
                || (dependentValue != null && this.targetValues.Any(t => t.Equals(dependentValue))))
            {
                if (!required.IsValid(value))
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new[] { validationContext.MemberName });
                }
            }
            return ValidationResult.Success;
        }
    }
}
