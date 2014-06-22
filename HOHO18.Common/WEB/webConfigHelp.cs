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

        //������֧��
        private static string xmlMenuSystem = ConfigHelper.GetConfigString("XmlMenuSystem");
        private static Configuration cfgMenu { get; set; }
        /// <summary>
        /// ��ȡweb.config�е�ֵ
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetWebConfigMenu(string key)
        {
            //��ǰ����
            var language = SessionHelper.GetSessionLanguages();
            var keySaveCache = key + language;
            try
            {
                //��ȡ·��
                string path = System.Web.HttpContext.Current.Server.MapPath(xmlMenuSystem.Replace("[==Language==]", language));

                //�ӻ����ж�ȡ
                object keyValueObject = CacheAccess.GetFromCache(keySaveCache);

                if (keyValueObject == null)
                {
                    CacheDependency fileDependency = new CacheDependency(path);
                    //�������ļ�
                    OpenWebMenuConfig();
                    //Configuration cfg = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(xmlSystemName);
                    AppSettingsSection appSetting = cfgMenu.AppSettings;

                    string keyValue = appSetting.Settings[key].Value;

                    //���浽������
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
        /// ����webxml
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keyValue">value</param>
        /// <param name="isSave">�Ƿ񱣴棬true���Ա����ˣ�false�������б���</param>
        public static void SetWebMenuConfig(string key, string keyValue, bool isSave)
        {
            try
            {
                //�������ļ�
                OpenWebMenuConfig();
                AppSettingsSection appSetting = cfgMenu.AppSettings;
                appSetting.Settings[key].Value = keyValue;

                if (isSave)
                    //���ɱ���
                    cfgMenu.Save();
            }
            catch
            {
            }
        }

        private static void OpenWebMenuConfig()
        {
            var oldLanguage = "";
            //��ǰ����
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
        /// ��ȡweb.config�е�ֵ
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetWebConfigValueObj(string key)
        {
            //��ǰ����
            var language = SessionHelper.GetSessionLanguages();
            var keySaveCache = key + language;
            try
            {
                //��ȡ·��
                string path = System.Web.HttpContext.Current.Server.MapPath(xmlSystemName);

                //�ӻ����ж�ȡ
                object keyValueObject = CacheAccess.GetFromCache(keySaveCache);

                if (keyValueObject == null)
                {
                    CacheDependency fileDependency = new CacheDependency(path);
                    //�������ļ�
                    OpenWebConfig();
                    //Configuration cfg = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(xmlSystemName);
                    AppSettingsSection appSetting = cfg.AppSettings;

                    string keyValue = appSetting.Settings[key].Value;

                    //���浽������
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
        /// ��ȡweb.config�е�ֵ
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetWebConfigValue(string key)
        {
            //��ǰ����
            var language = SessionHelper.GetSessionLanguages();
            var keySaveCache = key + language;
            try
            {
                //��ȡ·��
                string path = System.Web.HttpContext.Current.Server.MapPath(xmlSystemName);

                //�ӻ����ж�ȡ
                object keyValueObject = CacheAccess.GetFromCache(keySaveCache);

                if (keyValueObject == null)
                {
                    CacheDependency fileDependency = new CacheDependency(path);
                    //�������ļ�
                    OpenWebConfig();
                    //Configuration cfg = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(xmlSystemName);
                    AppSettingsSection appSetting = cfg.AppSettings;

                    string keyValue = appSetting.Settings[key].Value;

                    //���浽������
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
        /// ����webxml
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keyValue">value</param>
        /// <param name="isSave">�Ƿ񱣴棬true���Ա����ˣ�false�������б���</param>
        public static void SetWebConfigValue(string key, string keyValue, bool isSave)
        {
            try
            {
                //�������ļ�
                OpenWebConfig();
                AppSettingsSection appSetting = cfg.AppSettings;
                appSetting.Settings[key].Value = keyValue;

                if (isSave)
                    //���ɱ���
                    cfg.Save();
            }
            catch
            {
            }
        }


        private static void OpenWebConfig()
        {
            var oldLanguage = "";
            //��ǰ����
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
