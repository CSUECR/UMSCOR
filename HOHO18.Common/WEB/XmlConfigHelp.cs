using System;
using System.Text;
using System.Configuration;
using System.Collections;
using System.Web.Caching;


namespace HOHO18.Common.Web
{

    public sealed class XmlConfigHelp
    {
        public XmlConfigHelp()
        {
        }

        #region XMLMenu
        //多语言支持
        private static string xmlMenuSystem = "XmlMenuSystem".GetXmlConfig();
        private static Configuration cfgMenu { get; set; }
        /// <summary>
        /// 获取web.config中的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetXmlConfigMenu(string key)
        {
            //当前语言
            var language = SessionHelper.GetSessionLanguages();
            var keySaveCache = key + language;
            try
            {
                //获取路径
                string path = System.Web.HttpContext.Current.Server.MapPath(xmlMenuSystem.Replace("[==Language==]", language));

                //从缓存中读取
                object keyValueObject = CacheAccess.GetFromCache(keySaveCache);

                if (keyValueObject == null)
                {
                    CacheDependency fileDependency = new CacheDependency(path);
                    //打开配置文件
                    OpenXmlMenuConfig();
                    //Configuration cfg = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(xmlSystemName);
                    AppSettingsSection appSetting = cfgMenu.AppSettings;

                    string keyValue = appSetting.Settings[key].Value;

                    //保存到缓存中
                    CacheAccess.SaveToCacheByDependency(keySaveCache, keyValue, fileDependency);
                    keyValueObject = keyValue;
                }
                return keyValueObject.ToString();
            }
            catch
            {
                return "";
            }
        }      

        private static void OpenXmlMenuConfig()
        {
            var oldLanguage = "";
            //当前语言
            var language = SessionHelper.GetSessionLanguages();
            if (System.Web.HttpContext.Current.Session["OldLanguage"] != null)
            {
                oldLanguage = System.Web.HttpContext.Current.Session["OldLanguage"].ToString().ToLower();
            }
            if (cfgMenu == null || language != oldLanguage)
            {
                if (language != oldLanguage)
                {
                    System.Web.HttpContext.Current.Session["OldLanguage"] = language;
                }
                cfgMenu = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(xmlMenuSystem.Replace("[==Language==]", language));
            }
        }
        #endregion


        private static string xmlSystemName = "XmlSystemName".GetXmlConfig();
        private static Configuration cfg { get; set; }
              

        /// <summary>
        /// 获取XmlConfig中的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetXmlConfig(string key)
        {
            //当前语言
            var language = SessionHelper.GetSessionLanguages();
            var keySaveCache = key + language;
            try
            {
                //获取路径
                string path = System.Web.HttpContext.Current.Server.MapPath(xmlSystemName);

                //从缓存中读取
                object keyValueObject = CacheAccess.GetFromCache(keySaveCache);

                if (keyValueObject == null)
                {
                    CacheDependency fileDependency = new CacheDependency(path);
                    //打开配置文件
                    OpenXmlConfig();
                    //Configuration cfg = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(xmlSystemName);
                    AppSettingsSection appSetting = cfg.AppSettings;

                    string keyValue = appSetting.Settings[key].Value;

                    //保存到缓存中
                    CacheAccess.SaveToCacheByDependency(keySaveCache, keyValue, fileDependency);
                    keyValueObject = keyValue;
                }
                return keyValueObject.ToString();
            }
            catch
            {
                return "";
            }
        }       

        private static void OpenXmlConfig()
        {
            var oldLanguage = "";
            //当前语言
            var language = SessionHelper.GetSessionLanguages();
            if (System.Web.HttpContext.Current.Session["OldLanguage"] != null)
            {
                oldLanguage = System.Web.HttpContext.Current.Session["OldLanguage"].ToString().ToLower();
            }
            if (cfg == null || language != oldLanguage)
            {
                if (language != oldLanguage)
                {
                    System.Web.HttpContext.Current.Session["OldLanguage"] = language;
                }
                cfg = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(xmlSystemName);
            }
        }

        public static string GetXmlConfigValueNoCache(string key)
        {
            var ret = string.Empty;

            cfg = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(xmlSystemName);

            AppSettingsSection appSetting = cfg.AppSettings;

            ret = appSetting.Settings[key].Value;

            return ret;
        }




        #region 不用
        ///// <summary>
        ///// 保存webxml
        ///// </summary>
        ///// <param name="key">key</param>
        ///// <param name="keyValue">value</param>
        ///// <param name="isSave">是否保存，true可以保存了，false还不进行保存</param>
        //public static void SetXmlMenuConfig(string key, string keyValue, bool isSave)
        //{
        //    try
        //    {
        //        //打开配置文件
        //        OpenXmlMenuConfig();
        //        AppSettingsSection appSetting = cfgMenu.AppSettings;
        //        appSetting.Settings[key].Value = keyValue;

        //        if (isSave)
        //            //生成保存
        //            cfgMenu.Save();
        //    }
        //    catch
        //    {
        //    }
        //}

        ///// <summary>
        ///// 获取web.config中的值
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public string GetXmlConfigValueObj(string key)
        //{
        //    //当前语言
        //    var language = SessionHelper.GetSessionLanguages();
        //    var keySaveCache = key + language;
        //    try
        //    {
        //        //获取路径
        //        string path = System.Web.HttpContext.Current.Server.MapPath(xmlSystemName);

        //        //从缓存中读取
        //        object keyValueObject = CacheAccess.GetFromCache(keySaveCache);

        //        if (keyValueObject == null)
        //        {
        //            CacheDependency fileDependency = new CacheDependency(path);
        //            //打开配置文件
        //            OpenXmlConfig();
        //            //Configuration cfg = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(xmlSystemName);
        //            AppSettingsSection appSetting = cfg.AppSettings;

        //            string keyValue = appSetting.Settings[key].Value;

        //            //保存到缓存中
        //            CacheAccess.SaveToCacheByDependency(keySaveCache, keyValue, fileDependency);
        //            keyValueObject = keyValue;
        //        }
        //        return keyValueObject.ToString();
        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}

        ///// <summary>
        ///// 保存webxml
        ///// </summary>
        ///// <param name="key">key</param>
        ///// <param name="keyValue">value</param>
        ///// <param name="isSave">是否保存，true可以保存了，false还不进行保存</param>
        //public static void SetXmlConfigValue(string key, string keyValue, bool isSave)
        //{
        //    try
        //    {
        //        //打开配置文件
        //        OpenXmlConfig();
        //        AppSettingsSection appSetting = cfg.AppSettings;
        //        appSetting.Settings[key].Value = keyValue;

        //        if (isSave)
        //            //生成保存
        //            cfg.Save();
        //    }
        //    catch
        //    {
        //    }
        //}

        #endregion
    }
}
