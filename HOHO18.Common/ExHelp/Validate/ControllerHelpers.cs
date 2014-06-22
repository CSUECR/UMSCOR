using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web;
using MorSun.Model;
using System.Web.Mvc;
using HOHO18.Common;


namespace System
{
    /// <summary>
    /// 文本处理
    /// </summary>
    public static class ControllerHelpers
    {  
        /// <summary>
        /// 返回配置好的错误信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="errKey"></param>
        /// <param name="defaultErrMessage"></param>
        /// <returns></returns>
        public static RuleViolation AddRuleViolation(this string propertyName, string errMessage)            
        {
            propertyName = propertyName.Trim();
            errMessage = errMessage.Trim();
            //升级版：从配置读取数据都从OperationResultType里取，包括ModelState，ModelState可以自动生成客户端验证
            //想开启读取配置文件功能，去掉以下注释并修改相应代码
            //var _errMessage = XmlHelper.GetKeyNameValidation<T>(errMessage);
            //if (_errMessage.IsWhite())
            //    _errMessage =errMessage;
            var _errMessage = errMessage;
            return new RuleViolation(_errMessage, propertyName);
        }
        /// <summary>
        /// 将RuleViolation的配置信息放到ModelState里去
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="errors"></param>
        public static void FromRuleViolations(this ModelStateDictionary modelState,
        IEnumerable<RuleViolation> errors)
        {
            foreach (RuleViolation issue in errors)
            {
                modelState.AddModelError(issue.PropertyName, issue.ErrorMessage);
            }
        }
        

    }
}
