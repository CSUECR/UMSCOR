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
        //������֧��
        private static string xmlMenuSystem = "XmlMenuSystem".GW();
        private static Configuration cfgMenu { get; set; }
        /// <summary>
        /// ��ȡweb.config�е�ֵ
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetXmlConfigMenu(string key)
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
                    OpenXmlMenuConfig();
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

        private static void OpenXmlMenuConfig()
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
        #endregion


        private static string xmlSystemName = "XmlSystemName".GW();

        /// <summary>
        /// ��ȡXmlConfig�е�ֵ  XMLConfig��Ҫ�������Ա�ʶ
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetXmlConfig(string key)
        {
            //��ǰ����            
            var keySaveCache = key;
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
                    Configuration cfg = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(xmlSystemName);
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
    }
}