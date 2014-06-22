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
                string _connectionString = ConfigHelper.GetConfigString("ApplicationServices");//ConfigurationManager.AppSettings["ConnectionString"];       
                string ConStringEncrypt = ConfigHelper.GetConfigString("ConStringEncrypt");//ConfigurationManager.AppSettings["ConStringEncrypt"];
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
            string connectionString = ConfigHelper.GetConfigString(configName);//ConfigurationManager.AppSettings[configName];
            string ConStringEncrypt = ConfigHelper.GetConfigString("ConStringEncrypt");//ConfigurationManager.AppSettings["ConStringEncrypt"];
            if (ConStringEncrypt == "true")
            {
                connectionString = DESEncrypt.Decrypt(connectionString);
            }
            return connectionString;
        }


    }
}
