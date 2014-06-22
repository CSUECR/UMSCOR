using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Text;

namespace MorSun.Model
{
    public static class EntityObjectExtension
    {
        /// <summary>
        /// 输出所有属性的属性名称和属性值的信息
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ToPropertyString(this EntityObject t)
        {
            var rtnBuilder = new StringBuilder();

            var type = t.GetType();
            var properties = type.GetProperties().Where(u => u.CanWrite);
            foreach (var pro in properties)
            {
                //是复杂类型
                var isComplexType = !TypeDescriptor.GetConverter(pro.PropertyType).CanConvertFrom(typeof(string));
                //非复杂类型
                if (!isComplexType)
                {
                    var proName = pro.Name;
                    var proValue = pro.GetValue(t, null);
                    rtnBuilder.AppendFormat("\"{0}\":\"{1}\",", proName, proValue);
                }
            }
            return string.Format("{{{0}}}", (rtnBuilder.Length > 1 ? rtnBuilder.ToString().TrimEnd(',') : rtnBuilder.ToString()));
        }
    }
}
