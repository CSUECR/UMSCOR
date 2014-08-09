using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Configuration;
using System.Web;
using System.Web.Caching;
using System.IO;
using HOHO18.Common;
using System.Reflection;
using FastReflectionLib;

namespace HOHO18.Common
{
    public class XmlHelper
    {

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <returns></returns>
        public static Validation Deserialize()
        {
            //获取路径
            var path = System.Web.HttpContext.Current.Server.MapPath("XmlValidationConfigName".GX());
            //当前语言
            var language = SessionHelper.GetSessionLanguages();
            //多语言支持
            path = path.Replace("[==Language==]", language);

            var languageValidation = "Validation_" + language;

            //从缓存中读取
            object validationObject = CacheAccess.GetFromCache(languageValidation);
            if (validationObject == null)
            {
                CacheDependency fileDependency = new CacheDependency(path);
                //反序列化
                Validation validation = new Validation();
                validation = (Validation)validation.Deserialize(path);

                //保存到缓存中
                CacheAccess.SaveToCacheByDependency(languageValidation, validation, fileDependency);
                validationObject = validation;
            }

            return validationObject as Validation;
        }

        /// <summary>
        /// 读取验证信息
        /// </summary>
        /// <param name="FKeyName">类名称</param>
        /// <param name="KeyName">验证信息名称</param>
        /// <returns></returns>
        public static string GetKeyNameValidation(string FKeyName, string KeyName)
        {
            //读取数据验证信息
            Validation validation = XmlHelper.Deserialize();
            string validationStr = KeyName;
            try
            {
                validationStr = validation.Biao_List.Where(b => b.KeyName == FKeyName).FirstOrDefault().Field_List.Where(d => d.KeyName == KeyName).FirstOrDefault().KeyValue;
            }
            catch
            {
                if (string.IsNullOrEmpty(KeyName))
                    validationStr = "输入错误:还未配置错误原因！";
            }
            return validationStr;
        }

        /// <summary>
        /// 读取验证信息
        /// </summary>
        /// <param name="FKeyName">类名称</param>
        /// <param name="KeyName">验证信息名称</param>
        /// <returns></returns>
        public static string GetKeyNameValidation<T>(string KeyName)
            where T : class
        {
            string FKeyName = typeof(T).Name;
            return GetKeyNameValidation(FKeyName, KeyName);
        }

        /// <summary>
        /// 读取验证信息
        /// </summary>
        /// <param name="FKeyName">类名称</param>
        /// <param name="KeyName">验证信息名称</param>
        /// <returns></returns>
        public static string GetPagesString(string FKeyName, string KeyName)
        {
            //读取数据验证信息
            Validation validation = XmlHelper.Deserialize();
            string validationStr = KeyName;
            try
            {
                validationStr = validation.Biao_List.Where(b => b.KeyName == FKeyName).FirstOrDefault().Pages_List.FirstOrDefault().Field_List.Where(d => d.KeyName == KeyName).FirstOrDefault().KeyValue;
            }
            catch
            {
                if (string.IsNullOrEmpty(KeyName))
                    validationStr = "输入错误:还未配置错误原因！";
            }
            return validationStr;
        }

        /// <summary>
        /// 读取验证信息
        /// </summary>
        /// <param name="FKeyName">类名称</param>
        /// <param name="KeyName">验证信息名称</param>
        /// <returns></returns>
        public static string GetPagesString<T>(string KeyName)
            where T : class
        {
            string FKeyName = typeof(T).Name;
            return GetPagesString(FKeyName, KeyName);
        }
    }
}
