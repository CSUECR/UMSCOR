using HOHO18.Common.WEB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;


namespace MorSun.Controllers.Filter
{
    public class ExceptionFilter : FilterAttribute, IExceptionFilter
    {
        void IExceptionFilter.OnException(ExceptionContext filterContext)
        {
            //filterContext.Controller.ViewData["ErrorMessage"] = filterContext.Exception.Message;
            //filterContext.Controller.ViewData["ErrorHelpLink"] = filterContext.Exception.HelpLink;
            //filterContext.Controller.ViewData["ErrorSource"] = filterContext.Exception.Source;
            //filterContext.Controller.ViewData["ErrorStackTrace"] = filterContext.Exception.StackTrace;
            //filterContext.Controller.ViewData["ErrorExceptionString"] = filterContext.Exception.ToString();
            //filterContext.Result = new ViewResult()
            //{
                 
            //    ViewName = "Error",
                
            //    ViewData = filterContext.Controller.ViewData,
            //};
            //filterContext.ExceptionHandled = true;

            LogHelper.Write("\r\n客户机IP:" + filterContext.HttpContext.Request.UserHostAddress + "\r\n原始URL:" + filterContext.HttpContext.Request.RawUrl + "\r\n浏览器:" + filterContext.HttpContext.Request.Browser + "\r\n错误地址:" + filterContext.HttpContext.Request.Url + "\r\n异常信息:" + filterContext.Exception.Message, LogHelper.LogMessageType.Error);
        }

    }
}
