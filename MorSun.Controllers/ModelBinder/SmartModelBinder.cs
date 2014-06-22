using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MorSun.Controllers
{
    public class SmartModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = base.BindModel(controllerContext, bindingContext);
            if (value is string)
                return (value as string).Trim();
            return value;
        }

        protected override void OnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Dictionary<string, bool> startedValid = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            //获取模型的验证结果
            var results = ModelValidator.GetModelValidator(bindingContext.ModelMetadata, controllerContext).Validate(null);

            foreach (var validationResult in results)
            {
                string subPropertyName = CreateSubPropertyName(bindingContext.ModelName, validationResult.MemberName);
                //if(bindingContext.PropertyFilter(subPropertyName))
                //bindingContext.PropertyFilter 是一个 delegate, 如果指定的 member 在 BindAttribute 的 Include 的列表内（或者非 Exclude 的列表内），返回 true, 否则为 false
                //部分验证的功能通过这里实现的
                if (bindingContext.PropertyFilter(validationResult.MemberName))
                {
                    if (!startedValid.ContainsKey(subPropertyName))
                    {
                        startedValid[subPropertyName] = bindingContext.ModelState.IsValidField(subPropertyName);
                    }
                    if (startedValid[subPropertyName])
                    {
                        bindingContext.ModelState.AddModelError(subPropertyName, validationResult.Message);
                    }
                }
            }
        }
    }
}