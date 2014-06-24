using System;
using System.Configuration;
using HOHO18.Common.DEncrypt;
namespace HOHO18.Common
{

    public sealed class PubConstant
    {        
        /// <summary>
        /// ��ȡ�����ַ���
        /// </summary>
        public static string ConnectionString
        {           
            get 
            {
                string _connectionString = "ApplicationServices".GetXmlConfig();//ConfigurationManager.AppSettings["ConnectionString"];       
                string ConStringEncrypt = "ConStringEncrypt".GetXmlConfig();//ConfigurationManager.AppSettings["ConStringEncrypt"];
                if (ConStringEncrypt == "true")
                {
                    _connectionString = DESEncrypt.Decrypt(_connectionString);
                }
                return _connectionString; 
            }
        }

        /// <summary>
        /// �õ�web.config������������ݿ������ַ�����
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static string GetConnectionString(string configName)
        {
            string connectionString = configName.GetXmlConfig();//ConfigurationManager.AppSettings[configName];
            string ConStringEncrypt = "ConStringEncrypt".GetXmlConfig();//ConfigurationManager.AppSettings["ConStringEncrypt"];
            if (ConStringEncrypt == "true")
            {
                connectionString = DESEncrypt.Decrypt(connectionString);
            }
            return connectionString;
        }


    }
}
