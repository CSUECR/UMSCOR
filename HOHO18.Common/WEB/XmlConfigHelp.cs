using System;
using System.Text;
using System.Configuration;
using System.Collections;
using System.Web.Caching;


namespace HOHO18.Common.Web
{

    public sealed class XmlConfigHelper
    {
        public XmlConfigHelper()
        {
        }

        #region XMLMenu
        //多语言支持
        private static string xmlMenuSystem = "XmlMenuSystem".GW();
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


        private static string xmlSystemName = "XmlSystemName".GW();

        /// <summary>
        /// 获取XmlConfig中的值  XMLConfig不要增加语言标识
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetXmlConfig(string key)
        {
            //当前语言            
            var keySaveCache = key;
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
                    Configuration cfg = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(xmlSystemName);
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
    }
}