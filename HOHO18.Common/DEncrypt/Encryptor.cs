using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace HOHO18.Common.DEncrypt
{

��/**//// <summary>
��/// ������-���ڶ��������ݽ���Hashɢ�У��ﵽ����Ч��
��/// </summary>
����public sealed class Encryptor
����{
��������/**//// <summary>
��������/// ʹ��MD5�㷨��Hashɢ��
��������/// </summary>
��������/// <param name="text">����</param>
��������/// <returns>ɢ��ֵ</returns>
��������public static string MD5Encrypt(string text)
��������{
������������return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(text, "MD5");
��������}

��������/**//// <summary>
��������/// ʹ��SHA1�㷨��Hashɢ��
��������/// </summary>
��������/// <param name="text">����</param>
��������/// <returns>ɢ��ֵ</returns>
��������public static string SHA1Encrypt(string text)
��������{
������������return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(text, "SHA1");
��������}
����}


}

