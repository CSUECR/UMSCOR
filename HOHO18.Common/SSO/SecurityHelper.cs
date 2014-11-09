using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HOHO18.Common.SSO
{
    public static class SecurityHelper
    {
        //加密时的SALT值(撒盐值)
        public static readonly string SALT = "AKW#*#IN2f#fhch38&#*74J32FAJD*#yhvn86*&&#*#^$*&y$^&fh";

        //加解密时的KEY(密钥)
        public static readonly string KEY = "HFFHHJEK*(^$WQhf389uHH4wfbj#&@qk17438924&*(^$&@#&@&";


        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="input">加密后的字符串</param>
        /// <returns>加密前的字符串</returns>
        public static string Decrypt(string input)
        {
            // 盐值（与加密时设置的值必须一致）
            //string saltValue = "saltValue";
            //// 密码值（与加密时设置的值必须一致）
            //string pwdValue = "pwdValue";

            byte[] encryptBytes;
            try
            {
                encryptBytes = Convert.FromBase64String(input);
            }
            catch (FormatException)
            {
                return "参数格式错误或原对称加密值改变";
            }
            byte[] salt = Encoding.UTF8.GetBytes(SALT);

            AesManaged aes = new AesManaged();

            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(KEY, salt);

            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.IV = rfc.GetBytes(aes.BlockSize / 8);

            // 用当前的 Key 属性和初始化向量 IV 创建对称解密器对象
            ICryptoTransform decryptTransform = aes.CreateDecryptor();

            // 解密后的输出流
            MemoryStream decryptStream = new MemoryStream();

            // 将解密后的目标流（decryptStream）与解密转换（decryptTransform）相连接
            CryptoStream decryptor = new CryptoStream(decryptStream, decryptTransform, CryptoStreamMode.Write);

            try
            {
                // 将一个字节序列写入当前 CryptoStream （完成解密的过程）
                decryptor.Write(encryptBytes, 0, encryptBytes.Length);
                decryptor.Close();
            }
            catch (Exception)
            {
                return "对称加密值改变";
            }

            // 将解密后所得到的流转换为字符串
            byte[] decryptBytes = decryptStream.ToArray();
            string decryptedString = Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);

            return decryptedString;
        }


        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="input">加密前的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt(string input)
        {
            // 盐值 （加解密的程序中这个值必须一致）
            //string saltValue = "saltValue";
            // 密码值 （加解密的程序中这个值必须一致）
            //string pwdValue = "pwdValue";

            byte[] data = Encoding.UTF8.GetBytes(input);
            byte[] salt = Encoding.UTF8.GetBytes(SALT);

            // AesManaged - 高级加密标准(AES) 对称算法的管理类

            //安全加密命名空间：System.Security.Cryptography 
            AesManaged aes = new AesManaged();

            // Rfc2898DeriveBytes - 通过使用基于 HMACSHA1 的伪随机数生成器，实现基于密码的密钥派生功能 (PBKDF2 - 一种基于密码的密钥派生函数)
            // 通过密码和salt派生密钥
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(KEY, salt);


            /*
            * AesManaged.BlockSize - 加密操作的块大小（单位：bit）
            * AesManaged.LegalBlockSizes - 对称算法支持的块大小（单位：bit）
            * AesManaged.KeySize - 对称算法的密钥大小（单位：bit）
            * AesManaged.LegalKeySizes - 对称算法支持的密钥大小（单位：bit）
            * AesManaged.Key - 对称算法的密钥
            * AesManaged.IV - 对称算法的密钥大小
            * Rfc2898DeriveBytes.GetBytes(int 需要生成的伪随机密钥字节数) - 生成密钥
            */

            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.IV = rfc.GetBytes(aes.BlockSize / 8);

            // 用当前的 Key 属性和初始化向量 IV 创建对称加密器对象
            ICryptoTransform encryptTransform = aes.CreateEncryptor();

            // 加密后的输出流
            MemoryStream encryptStream = new MemoryStream();

            // 将加密后的目标流（encryptStream）与加密转换（encryptTransform）相连接
            CryptoStream encryptor = new CryptoStream(encryptStream, encryptTransform, CryptoStreamMode.Write);

            // 将一个字节序列写入当前 CryptoStream （完成加密的过程）
            encryptor.Write(data, 0, data.Length);
            encryptor.Close();

            // 将加密后所得到的流转换成字节数组，再用Base64编码将其转换为字符串
            string encryptedString = Convert.ToBase64String(encryptStream.ToArray());

            return encryptedString;
        }
    }
}
