using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using HOHO18.Common;
namespace MorSun.Model
{
    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        public LocalizedDisplayNameAttribute(Type modelType,string resourceKey)
        {
            ResourceKey = resourceKey;
            ModelType = modelType;
        }

        public override string DisplayName
        {
            get
            {
                string displayName = XmlHelper.GetKeyNameValidation(ModelType.Name, ResourceKey); //w.Toi18nString(ResourceKey);//MyResource.ResourceManager.GetString(ResourceKey);

                return string.IsNullOrEmpty(displayName)
                    ? string.Format("[[{0}]]", ResourceKey)
                    : displayName;
            }
        }

        private string ResourceKey { get; set; }
        private Type ModelType { get; set; }
    }
}
