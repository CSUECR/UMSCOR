using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HOHO18.Common
{
    /// <summary>
    /// 参数处理类
    /// </summary>
    public class ParameterProcess
    {
        /// <summary>
        /// 将所有类型为字符串的参数前后去空格
        /// </summary>
        public static void TrimParameter<T>(T obj)
        {
            Type type = obj.GetType();
            System.Reflection.PropertyInfo[] pList = type.GetProperties();
            object tempValue;
            foreach (var p in pList)
            {
                if (p.CanRead && p.CanWrite && p.PropertyType.Name.ToLower() == "string")//string
                {
                    tempValue = p.GetValue(obj, null);
                    if (tempValue != null) p.SetValue(obj, tempValue.ToString().Trim(), null);
                }
            }
        }
    }
}
