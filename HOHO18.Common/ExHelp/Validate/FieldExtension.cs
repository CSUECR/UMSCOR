using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using HOHO18.Common;

namespace System.Web.Mvc.Html
{
    public static  class FieldExtension
    {
        /// <summary>
        /// 通过xml中获取显示字段的名称（也就是页面文字的xml配置化）
        /// </summary>
        /// <param name="keyName">键</param>
        /// <param name="modelName">模型名称（一般是表的名称）</param>
        /// <returns></returns>
        public static MvcHtmlString Filed(this HtmlHelper helper,string keyName, string modelName)
        {
            var filedValue = XmlHelper.GetPagesString(modelName, keyName);
            return MvcHtmlString.Create(filedValue);
        }
    }
}
