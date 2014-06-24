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
        //������֧��
        private static string xmlMenuSystem = "XmlMenuSystem".GetXmlConfig();
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


        private static string xmlSystemName = "XmlSystemName".GetXmlConfig();
        private static Configuration cfg { get; set; }
              

        /// <summary>
        /// ��ȡXmlConfig�е�ֵ
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetXmlConfig(string key)
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
                    OpenXmlConfig();
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

        private static void OpenXmlConfig()
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

        public static string GetXmlConfigValueNoCache(string key)
        {
            var ret = string.Empty;

            cfg = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(xmlSystemName);

            AppSettingsSection appSetting = cfg.AppSettings;

            ret = appSetting.Settings[key].Value;

            return ret;
        }




        #region ����
        ///// <summary>
        ///// ����webxml
        ///// </summary>
        ///// <param name="key">key</param>
        ///// <param name="keyValue">value</param>
        ///// <param name="isSave">�Ƿ񱣴棬true���Ա����ˣ�false�������б���</param>
        //public static void SetXmlMenuConfig(string key, string keyValue, bool isSave)
        //{
        //    try
        //    {
        //        //�������ļ�
        //        OpenXmlMenuConfig();
        //        AppSettingsSection appSetting = cfgMenu.AppSettings;
        //        appSetting.Settings[key].Value = keyValue;

        //        if (isSave)
        //            //���ɱ���
        //            cfgMenu.Save();
        //    }
        //    catch
        //    {
        //    }
        //}

        ///// <summary>
        ///// ��ȡweb.config�е�ֵ
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public string GetXmlConfigValueObj(string key)
        //{
        //    //��ǰ����
        //    var language = SessionHelper.GetSessionLanguages();
        //    var keySaveCache = key + language;
        //    try
        //    {
        //        //��ȡ·��
        //        string path = System.Web.HttpContext.Current.Server.MapPath(xmlSystemName);

        //        //�ӻ����ж�ȡ
        //        object keyValueObject = CacheAccess.GetFromCache(keySaveCache);

        //        if (keyValueObject == null)
        //        {
        //            CacheDependency fileDependency = new CacheDependency(path);
        //            //�������ļ�
        //            OpenXmlConfig();
        //            //Configuration cfg = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(xmlSystemName);
        //            AppSettingsSection appSetting = cfg.AppSettings;

        //            string keyValue = appSetting.Settings[key].Value;

        //            //���浽������
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
        ///// ����webxml
        ///// </summary>
        ///// <param name="key">key</param>
        ///// <param name="keyValue">value</param>
        ///// <param name="isSave">�Ƿ񱣴棬true���Ա����ˣ�false�������б���</param>
        //public static void SetXmlConfigValue(string key, string keyValue, bool isSave)
        //{
        //    try
        //    {
        //        //�������ļ�
        //        OpenXmlConfig();
        //        AppSettingsSection appSetting = cfg.AppSettings;
        //        appSetting.Settings[key].Value = keyValue;

        //        if (isSave)
        //            //���ɱ���
        //            cfg.Save();
        //    }
        //    catch
        //    {
        //    }
        //}

        #endregion
    }
}
