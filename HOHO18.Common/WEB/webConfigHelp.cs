using System;
using System.Text;
using System.Configuration;
using System.Collections;
using System.Web.Caching;


namespace HOHO18.Common.Web
{

    public sealed class webConfigHelp
    {
        public webConfigHelp()
        {
        }

        //多语言支持
        private static string xmlMenuSystem = ConfigHelper.GetConfigString("XmlMenuSystem");
        private static Configuration cfgMenu { get; set; }
        /// <summary>
        /// 获取web.config中的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetWebConfigMenu(string key)
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
                    OpenWebMenuConfig();
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


        /// <summary>
        /// 保存webxml
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keyValue">value</param>
        /// <param name="isSave">是否保存，true可以保存了，false还不进行保存</param>
        public static void SetWebMenuConfig(string key, string keyValue, bool isSave)
        {
            try
            {
                //打开配置文件
                OpenWebMenuConfig();
                AppSettingsSection appSetting = cfgMenu.AppSettings;
                appSetting.Settings[key].Value = keyValue;

                if (isSave)
                    //生成保存
                    cfgMenu.Save();
            }
            catch
            {
            }
        }

        private static void OpenWebMenuConfig()
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



        private static string xmlSystemName = ConfigHelper.GetConfigString("XmlSystemName");
        private static Configuration cfg { get; set; }

        /// <summary>
        /// 获取web.config中的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetWebConfigValueObj(string key)
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
                    OpenWebConfig();
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

        /// <summary>
        /// 获取web.config中的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetWebConfigValue(string key)
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
                    OpenWebConfig();
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


        /// <summary>
        /// 保存webxml
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keyValue">value</param>
        /// <param name="isSave">是否保存，true可以保存了，false还不进行保存</param>
        public static void SetWebConfigValue(string key, string keyValue, bool isSave)
        {
            try
            {
                //打开配置文件
                OpenWebConfig();
                AppSettingsSection appSetting = cfg.AppSettings;
                appSetting.Settings[key].Value = keyValue;

                if (isSave)
                    //生成保存
                    cfg.Save();
            }
            catch
            {
            }
        }


        private static void OpenWebConfig()
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

        public static string GetWebConfigValueNoCache(string key)
        {
            var ret = string.Empty;

            cfg = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(xmlSystemName);

            AppSettingsSection appSetting = cfg.AppSettings;

            ret = appSetting.Settings[key].Value;

            return ret;
        }
    }
}
