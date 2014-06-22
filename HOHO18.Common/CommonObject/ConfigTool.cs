using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;

namespace HOHO18.Common
{
    /// <summary>
    /// 用于配置文件操作的工具类
    /// </summary>
    public static class ConfigTool
    {
        //配置文件NBBSConfig.xml的绝对路径
        private static readonly string configFilePath = HttpContext.Current.Server.MapPath("~/Config.config");
        private static XmlDocument _xmlDoc = null;
        private static XmlDocument xmlDoc
        {
            get
            {
                if (_xmlDoc == null)
                {
                    _xmlDoc = new XmlDocument();
                    _xmlDoc.Load(@configFilePath);
                }
                return _xmlDoc;
            }
        }

        /// <summary>
        /// 获取网站Url
        /// </summary>
        /// <returns>网站Url</returns>
        public static string GetSiteUrl()
        {
            XmlNodeList url = xmlDoc.GetElementsByTagName("SiteUrl");

            return url[0].InnerText.ToString();
        }

        /// <summary>
        /// 设置网站Url
        /// </summary>
        /// <param name="siteurl">网站Url</param>
        public static void SetSiteUrl(string siteurl)
        {
            XmlNodeList url = xmlDoc.GetElementsByTagName("SiteUrl");

            url[0].InnerText = siteurl.ToString();

            xmlDoc.Save(@configFilePath);
        }

        /// <summary>
        /// 获取网站Id,这边只有一个网站，先这么做吧
        /// </summary>
        /// <returns>网站Id</returns>
        public static string GetCompany()
        {
            XmlNodeList company = xmlDoc.GetElementsByTagName("Company");

            return company[0].InnerText.ToString();
        }

        /// <summary>
        /// 设置网站Id,瞒放在这边，一般用不上
        /// </summary>
        /// <param name="siteurl">网站Id</param>
        public static void SetCompany(string siteurl)
        {
            XmlNodeList company = xmlDoc.GetElementsByTagName("Company");

            company[0].InnerText = siteurl.ToString();

            xmlDoc.Save(@configFilePath);
        }

        /// <summary>
        /// 获取网站Id,这边只有一个网站，先这么做吧
        /// </summary>
        /// <returns>网站Id</returns>
        public static string GetCompanyGuid()
        {
            XmlNodeList company = xmlDoc.GetElementsByTagName("CompanyGuid");

            return company[0].InnerText.ToString();
        }

        /// <summary>
        /// 设置网站Id,瞒放在这边，一般用不上
        /// </summary>
        /// <param name="siteurl">网站Id</param>
        public static void SetCompanyGuid(string siteurl)
        {
            XmlNodeList company = xmlDoc.GetElementsByTagName("CompanyGuid");

            company[0].InnerText = siteurl.ToString();

            xmlDoc.Save(@configFilePath);
        }

        /// <summary>
        /// 获取模块Id,
        /// </summary>
        /// <returns>网站Id</returns>
        public static string GetApplicationId()
        {
            XmlNodeList ApplicationId = xmlDoc.GetElementsByTagName("ApplicationId");

            return ApplicationId[0].InnerText.ToString();
        }

        /// <summary>
        /// 设置模块
        /// </summary>
        /// <param name="siteurl">网站Id</param>
        public static void SetApplicationId(string siteurl)
        {
            XmlNodeList ApplicationId = xmlDoc.GetElementsByTagName("ApplicationId");

            ApplicationId[0].InnerText = siteurl.ToString();

            xmlDoc.Save(@configFilePath);
        }


        /// <summary>
        /// 获取过滤字符串
        /// </summary>
        /// <returns>由“,”分隔的所有过滤字符串组成的长字符串</returns>
        public static string GetFilter()
        {
            XmlNodeList filters = xmlDoc.GetElementsByTagName("Filter");

            int i;
            string filterStr = "";
            for (i = 0; i < filters[0].ChildNodes.Count; i++)
            {
                filterStr += filters[0].ChildNodes[i].InnerText;
                if (i != filters[0].ChildNodes.Count - 1)
                {
                    filterStr += ",";
                }
            }

            return filterStr;
        }

        /// <summary>
        /// 设置过滤字符串
        /// </summary>
        /// <param name="filterStr">由“,”分隔的所有过滤字符串组成的长字符串</param>
        public static void SetFilter(string filterStr)
        {
            XmlNodeList filters = xmlDoc.GetElementsByTagName("Filter");

            string[] filterWords = filterStr.Split(',');

            filters[0].RemoveAll();
            int i;
            for (i = 0; i < filterWords.Length; i++)
            {
                XmlNode newNode = xmlDoc.CreateNode(XmlNodeType.Element, "FilterItem", "");
                newNode.InnerText = filterWords[i];
                filters[0].AppendChild(newNode);
            }

            xmlDoc.Save(@configFilePath);
        }


        /// <summary>
        /// 获取项目全局流程配置
        /// </summary>
        /// <returns>由“,”分隔的所有过滤字符串组成的长字符串</returns>
        public static string GetProjectFlow()
        {
            XmlNodeList s = xmlDoc.GetElementsByTagName("ProjectFlow");

            int i;
            StringBuilder sb = new StringBuilder();
            for (i = 0; i < s[0].ChildNodes.Count; i++)
            {
                sb.Append(s[0].ChildNodes[i].InnerText);
                if (i != s[0].ChildNodes.Count - 1)
                {
                    sb.Append(",");
                }
            }

            return sb.ToString();
        }


        /// <summary>
        /// 获取网站底部信息
        /// </summary>
        /// <returns>获取网站底部信息</returns>
        public static string GetFooterContent()
        {
            XmlNodeList company = xmlDoc.GetElementsByTagName("FooterContent");

            return System.Web.HttpUtility.HtmlDecode(company[0].InnerXml);
        }

        /// <summary>
        /// 设置网站底部信息
        /// </summary>
        /// <param name="siteurl">网站底部信息</param>
        public static void SetFooterContent(string name)
        {
            XmlNodeList company = xmlDoc.GetElementsByTagName("FooterContent");

            company[0].InnerText = System.Web.HttpUtility.HtmlEncode(name.ToString());

            xmlDoc.Save(@configFilePath);
        }

        /// <summary>
        /// 获取网站是否验证
        /// </summary>
        /// <returns>网站是否验证</returns>
        public static string GetDeptVerifyContent()
        {
            XmlNodeList company = xmlDoc.GetElementsByTagName("OpenDeptVerify");

            return System.Web.HttpUtility.HtmlDecode(company[0].InnerText.ToString());
        }

        /// <summary>
        /// 设置网站是否验证
        /// </summary>
        /// <param name="siteurl">网站是否验证</param>
        public static void SetDeptVerifyContent(string name)
        {
            XmlNodeList company = xmlDoc.GetElementsByTagName("OpenDeptVerify");

            company[0].InnerText = System.Web.HttpUtility.HtmlEncode(name.ToString());

            xmlDoc.Save(@configFilePath);
        }

        //***********************************************************************************************************************************邮件
        /// <summary>
        /// 获取修改密码邮件主体
        /// </summary>
        /// <returns>获取修改密码邮件主体</returns>
        public static string GetEmailChangePasswordBody()
        {
            XmlNodeList configString = xmlDoc.GetElementsByTagName("EmailChangePasswordBody");

            return System.Web.HttpUtility.HtmlDecode(configString[0].InnerText.ToString());
        }

        /// <summary>
        /// 获取激活邮件主体
        /// </summary>
        /// <returns>获取激活邮件主体</returns>
        public static string GetEmailConfirmBody()
        {
            XmlNodeList configString = xmlDoc.GetElementsByTagName("EmailConfirmBody");

            return System.Web.HttpUtility.HtmlDecode(configString[0].InnerText.ToString());
        }
        //***********************************************************************************************************************************类别

    }


}
