using System;
using System.Collections.Generic;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace HOHO18.Common.DEncrypt
{
    public class SSOUtil
    {
        /// <summary>
        /// 随机生成指定长度的字符串
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static string GetRandomString(int length)
        {
            StringBuilder sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                Random random = new Random(unchecked(i * (int)(DateTime.Now.Ticks)));

                int ret = random.Next(122);

                while (ret < 48 || (ret > 57 && ret < 65) || (ret > 90 && ret < 97))
                {
                    ret = random.Next(122);
                }

                sb.Append((char)ret);
            }
            return sb.ToString();

        }


        /// <summary>
        /// 加密提交信息
        /// </summary>
        /// <param name="text">加密字符串</param>
        /// <param name="key">加密key</param>
        /// <returns></returns>
        public static string DESEncrypt(string text, string key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Mode = System.Security.Cryptography.CipherMode.ECB;
            des.Padding = PaddingMode.Zeros;
            des.Key = ASCIIEncoding.ASCII.GetBytes(key);

            byte[] inputBuffer = Encoding.GetEncoding("UTF-8").GetBytes(text);
            byte[] outputBuffer = des.CreateEncryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);

            return Encrypt(Convert.ToBase64String(outputBuffer));
        }

        /// <summary>
        /// 解密提交信息
        /// </summary>
        /// <param name="text">解密字符串</param>
        /// <param name="key">解密key</param>
        /// <returns></returns>
        public static string DESDecrypt(string text, string key)
        {
            //解密数据
            text = Decrypt(text);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Mode = System.Security.Cryptography.CipherMode.ECB;
            des.Padding = PaddingMode.Zeros;
            des.Key = ASCIIEncoding.ASCII.GetBytes(key);

            byte[] inputBuffer = Convert.FromBase64String(text);
            byte[] outputBuffer = des.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);

            return Encoding.GetEncoding("UTF-8").GetString(outputBuffer);
        }

        public static string GetSiteUrl()
        {
            string path = HttpContext.Current.Request.ApplicationPath;
            if (path.EndsWith("/") && path.Length == 1)
            {
                return GetHostUrl();
            }
            else
            {
                return GetHostUrl() + path;
            }
        }

        public static string GetHostUrl()
        {
            return string.Format("{0}://{1}:{2}",
                HttpContext.Current.Request.Url.Scheme,
                HttpContext.Current.Request.Url.Host,
                HttpContext.Current.Request.Url.Port);
        }

        /// <summary>
        /// 用户信息提交前加密
        /// </summary>
        /// <param name="uid">用户名</param>
        /// <param name="isLogin">是否已经登录成功：true成功，false失败</param>
        /// <param name="defaulturl">访问链接</param>
        /// <param name="title">链接标题</param>
        /// <returns></returns>
        public static string GetLoginKey(string ssoKey, string username, bool isLogin, string defaulturl, string title)
        {
            try
            {
                StringBuilder userInfo = new StringBuilder();
                userInfo.Append("<userinfo>");
                if (isLogin)
                {
                    userInfo.Append("<username>").Append(username).Append("</username>");
                    userInfo.Append("<loginvalidatecode>").Append(Guid.NewGuid()).Append("</loginvalidatecode>");
                    userInfo.Append("<title>").Append(Encrypt(title)).Append("</title>");
                    userInfo.Append("<defaulturl>").Append(Encrypt(defaulturl)).Append("</defaulturl>");
                    userInfo.Append("<islongin>true</islongin>");
                }
                else
                {
                    userInfo.Append("<username>").Append("").Append("</username>");
                    userInfo.Append("<loginvalidatecode>").Append("").Append("</loginvalidatecode>");
                    userInfo.Append("<title>").Append("").Append("</title>");
                    userInfo.Append("<defaulturl>").Append("").Append("</defaulturl>");
                    userInfo.Append("<islongin>false</islongin>");
                }
                userInfo.Append("<synchdate>").Append(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")).Append("</synchdate>");
                userInfo.Append("</userinfo>");

                return SSOUtil.DESEncrypt(userInfo.ToString(), ssoKey.Substring(ssoKey.Length / 2 - 1, 8));
            }
            catch
            {
                return null;
            }
        }


        #region ========加密========

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Encrypt(string Text)
        {
            return Encrypt(Text, "HOHO18Tools");
        }
        /// <summary> 
        /// 加密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string Encrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(Text);
            des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        #endregion

        #region ========解密========


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Decrypt(string Text)
        {
            return Decrypt(Text, "HOHO18Tools");
        }
        /// <summary> 
        /// 解密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string Decrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            if (Text == null)
            {
                return "";
            }
            len = Text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }

        #endregion
    }
}
