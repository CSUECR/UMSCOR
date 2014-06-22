using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace HOHO18.Common.Helper
{
    /// <summary>
    /// 请求信息的处理工具类
    /// </summary>
    public class HttpRequestHelper
    {



        /// <summary>
        /// 根据请求数据来更新对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="Request"></param>
        /// <param name="propertyNames"></param>
        public static void UpdateRequestModule<T>(T model,System.Web.HttpRequest Request, params String[] propertyNames)
        {
            bool isGood = false;

            Type t = typeof(T);
            if (propertyNames == null || propertyNames.Length < 1)
            {
                object[] records = t.GetCustomAttributes(typeof(System.Web.Mvc.BindAttribute), false);
                if (records != null && records.Length > 0 && !String.IsNullOrEmpty(((System.Web.Mvc.BindAttribute)records[0]).Include))
                {
                    propertyNames = ((System.Web.Mvc.BindAttribute)records[0]).Include.Split(',');
                }
                else
                {
                    PropertyInfo[] allP = t.GetProperties();
                    foreach (var p in allP)
                    {
                        String v = Request[p.Name];
                        if (!String.IsNullOrEmpty(Request[p.Name]))
                        {
                            try
                            {
                                p.SetValue(model, v.Format(p.PropertyType), null);
                            }
                            catch { }
                        }
                    }
                    isGood = true;
                }
            }
            if (!isGood)
            {
                foreach (var pName in propertyNames)
                {
                    PropertyInfo p = t.GetProperty(pName);
                    if (p != null)
                    {
                        String v = Request[p.Name];
                        if (!String.IsNullOrEmpty(v))
                        {
                            try
                            {
                                p.SetValue(model, v.Format(p.PropertyType), null);
                            }
                            catch { }
                        }
                    }
                }
            }
        }
    }
}
