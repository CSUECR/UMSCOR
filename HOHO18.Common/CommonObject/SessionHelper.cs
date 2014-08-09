using System;
using System.Configuration;
using System.Text;
using System.Data;

namespace HOHO18.Common
{
    /// <summary>
    /// ��ȡSessionֵ
    /// </summary>
    public class SessionHelper
    {

        #region

        /// <summary>
        /// ��ǰ����
        /// </summary>
        /// <returns></returns>
        public static string GetSessionLanguages()
        {
            
            //ϵͳĬ������
            var DefaultLanguage = "zh_cn";
            try
            {
                if (System.Web.HttpContext.Current.Session["Language"] != null)
                {
                    //ת��ΪСд
                    DefaultLanguage = System.Web.HttpContext.Current.Session["Language"].ToString().ToLower();
                }
                else
                {
                    //ת��ΪСд
                    DefaultLanguage = "DefaultLanguage".GX().ToLower();

                    //���浽Session��
                    System.Web.HttpContext.Current.Session["Language"] = DefaultLanguage;
                }
            }
            catch
            {
            }
            return DefaultLanguage;
        }
        #endregion


    }
}
