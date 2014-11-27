using System;
using System.IO;
using System.IO.Compression;
using System.Text;
namespace HOHO18.Common.DEncrypt
{
	/// <summary>
	/// DES����/�����ࡣ
	/// </summary>
    public sealed class Compression
	{
        /// <summary>
        /// ���ַ�������ѹ��
        /// </summary>
        /// <param name="str">��ѹ�����ַ���</param>
        /// <returns>ѹ������ַ���</returns>
        public static string CompressString(string str)
        {
            string compressString = "";
            byte[] compressBeforeByte = Encoding.GetEncoding("UTF-8").GetBytes(str);
            byte[] compressAfterByte = Compress(compressBeforeByte);
            //compressString = Encoding.GetEncoding("UTF-8").GetString(compressAfterByte);
            compressString = Convert.ToBase64String(compressAfterByte);
            return compressString;
        }
        /// <summary>
        /// ���ַ������н�ѹ��
        /// </summary>
        /// <param name="str">����ѹ�����ַ���</param>
        /// <returns>��ѹ������ַ���</returns>
        public static string DecompressString(string str)
        {
            string compressString = "";
            //byte[] compressBeforeByte = Encoding.GetEncoding("UTF-8").GetBytes(str);
            byte[] compressBeforeByte = Convert.FromBase64String(str);
            byte[] compressAfterByte = Decompress(compressBeforeByte);
            compressString = Encoding.GetEncoding("UTF-8").GetString(compressAfterByte);
            return compressString;
        }
        /// <summary>
        /// ���ļ�����ѹ��
        /// </summary>
        /// <param name="sourceFile">��ѹ�����ļ���</param>
        /// <param name="destinationFile">ѹ������ļ���</param>
        public static void CompressFile(string sourceFile, string destinationFile)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// ���ļ����н�ѹ��
        /// </summary>
        /// <param name="sourceFile">����ѹ�����ļ���</param>
        /// <param name="destinationFile">��ѹ������ļ���</param>
        /// <returns></returns>
        public static void DecompressFile(string sourceFile, string destinationFile)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// ��byte�������ѹ��
        /// </summary>
        /// <param name="data">��ѹ����byte����</param>
        /// <returns>ѹ�����byte����</returns>
        public static byte[] Compress(byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
                zip.Write(data, 0, data.Length);
                zip.Close();
                byte[] buffer = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buffer, 0, buffer.Length);
                ms.Close();
                return buffer;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public static byte[] Decompress(byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream(data);
                GZipStream zip = new GZipStream(ms, CompressionMode.Decompress, true);
                MemoryStream msreader = new MemoryStream();
                byte[] buffer = new byte[0x1000];
                while (true)
                {
                    int reader = zip.Read(buffer, 0, buffer.Length);
                    if (reader <= 0)
                    {
                        break;
                    }
                    msreader.Write(buffer, 0, reader);
                }
                zip.Close();
                ms.Close();
                msreader.Position = 0;
                buffer = msreader.ToArray();
                msreader.Close();
                return buffer;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
	}
}
