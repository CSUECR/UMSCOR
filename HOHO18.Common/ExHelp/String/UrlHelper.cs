using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HOHO18.Common.DEncrypt;
using System.Web.UI;

namespace System
{
    public static class UrlHelper
    {
        //Url编码
        public static string Encoder(string input)
        {
            Base64Encoder encoder = new Base64Encoder();
            return encoder.GetEncoded(input);
        }

        //Url解码
        public static string Decoder(string input)
        {
            Base64Decoder decoder = new Base64Decoder();
            return decoder.GetDecoded(input, true);
        }

        // 获取当前Url,添加参数时添到Url最后面.
        //param:需添加的参数,exsitsParam:判断已存在的参数
        public static string GetCurrentUrl(this Page p, string param, string exsitsParam)
        {
            var ret = p.Request.Url.Segments[2].ToString();
            if (!string.IsNullOrEmpty(p.Request.Url.Query) && p.Request.Url.Query.IndexOf(exsitsParam) == -1)
            {
                ret += p.Request.Url.Query + "&" + param;
            }
            else if (!string.IsNullOrEmpty(p.Request.Url.Query) && p.Request.Url.Query.IndexOf(exsitsParam) != -1)
            {
                var i = p.Request.Url.Query.IndexOf(exsitsParam);
                ret += p.Request.Url.Query.Substring(0, i) + param;
            }
            else if (string.IsNullOrEmpty(p.Request.Url.Query) && p.Request.Url.Query.IndexOf(exsitsParam) == -1)
            {
                ret += p.Request.Url.Query + "?" + param;
            }
            return ret;
        }
    }
}
