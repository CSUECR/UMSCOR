using System;
using System.Configuration;
using HOHO18.Common.DEncrypt;
namespace HOHO18.Common
{

    public sealed class PubConstant
    {        
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public static string ConnectionString
        {           
            get 
            {
                string _connectionString = WebConfigHelper.GetConfigString("ApplicationServices");//ConfigurationManager.AppSettings["ConnectionString"];       
                string ConStringEncrypt = WebConfigHelper.GetConfigString("ConStringEncrypt");//ConfigurationManager.AppSettings["ConStringEncrypt"];
                if (ConStringEncrypt == "true")
                {
                    _connectionString = DESEncrypt.Decrypt(_connectionString);
                }
                return _connectionString; 
            }
        }

        /// <summary>
        /// 得到web.config里配置项的数据库连接字符串。
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static string GetConnectionString(string configName)
        {
            string connectionString = WebConfigHelper.GetConfigString(configName);//ConfigurationManager.AppSettings[configName];
            string ConStringEncrypt = WebConfigHelper.GetConfigString("ConStringEncrypt");//ConfigurationManager.AppSettings["ConStringEncrypt"];
            if (ConStringEncrypt == "true")
            {
                connectionString = DESEncrypt.Decrypt(connectionString);
            }
            return connectionString;
        }


    }
}
