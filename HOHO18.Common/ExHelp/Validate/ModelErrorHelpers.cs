using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web;
using System.Web.Mvc;
using HOHO18.Common;


namespace System
{
    /// <summary>
    /// 文本处理
    /// </summary>
    public static class ModelErrorHelpers
    {  
        /// <summary>
        /// 生成RuleViolation信息 全称：AddRuleViolation 本来是ADR 想起了干脆面，ADE不错的样子
        /// </summary>       
        /// <param name="propertyName"></param>
        /// <param name="errKey"></param>
        /// <param name="defaultErrMessage"></param>
        /// <returns></returns>
        public static RuleViolation AE(this string propertyName, string errMessage)            
        {
            propertyName = propertyName.Trim();
            errMessage = errMessage.Trim();
            //升级版：从配置读取数据都从GetErrorMessagesByModelState里取，包括ModelState，ModelState可以自动生成客户端验证            
            return new RuleViolation(errMessage, propertyName);
        }
        /// <summary>
        /// 将RuleViolation的所有配置信息放到ModelState里去 全称：PullFromRuleViolation
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="errors"></param>
        public static void PR(this ModelStateDictionary modelState, IEnumerable<RuleViolation> errors)
        {
            foreach (RuleViolation issue in errors)
            {
                modelState.AddModelError(issue.PropertyName, issue.ErrorMessage);
            }
        }

        /// <summary>
        /// 添加ModelErroe扩展，全称 AddModelError 变态扩展，您确认这方法能减少写代码的量,已经用了。 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="errKey"></param>
        /// <param name="defaultErrMessage"></param>
        /// <returns></returns>
        public static void AE(this string propertyName,string errMessage, ModelStateDictionary modelState)
        {
            propertyName = propertyName.Trim();
            errMessage = errMessage.Trim();
            modelState.AddModelError(propertyName, errMessage);
        }

        /// <summary>
        /// 获取当前错误的列表信息 全称：GetErrorMessagesByModelState
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ModelStateErrorMessage> GE(this ModelStateDictionary modelState)
        {
            var list = new List<ModelStateErrorMessage>();
            list = modelState.Where(u => u.Value.Errors.Any()).Select(u => new ModelStateErrorMessage() { Key = u.Key, ErrorMessages = u.Value.Errors.Select(e => e.ErrorMessage) }).ToList();
            //想要读取配置XML信息，从这开始取
            return list;
        }
    }
}
