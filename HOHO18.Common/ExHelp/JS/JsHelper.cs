using System;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Collections;
using FastReflectionLib;
using System.Drawing.Imaging;
using HOHO18.Common.Base;

namespace HOHO18.Common.ExHelp
{
    /// <summary>
    /// JsHelper 的摘要说明
    /// </summary>
    public static class JsHelper
    {
        /// <summary>
        /// 获取当前Page对象
        /// </summary>
        /// <returns></returns>
        static Page GetPage()
        {
            Page page = HttpContext.Current.Handler as Page;
            return page;
        }

        /// <summary>
        /// 将一个对象序列化成json格式
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string JsonObj(object obj,string format="")
        {
            var json = "null";
            if (obj != null)
            {
                if (obj.IsList())
                {
                    var jsonb = new StringBuilder("[");
                    var list = obj as IEnumerable;
                    var s = "";
                    foreach (var sub in list)
                    {
                        var par = Json(sub,format);
                        jsonb.Append(s).Append(par);
                        s = ",";
                    }
                    jsonb.Append("]");
                    json = jsonb.ToString();
                }
                else
                {
                    var jsonb = new StringBuilder("{");
                    var type = obj.GetType();
                    var pis = type.GetProperties();
                    var s = "";
                    BaseSQL<WmfPlaceableFileHeader> bill = new BaseSQL<WmfPlaceableFileHeader>();
                    var columns = bill.GetTableColumnName(type.Name);
                    foreach (var pi in pis)
                    {
                        var name = pi.Name;
                        if (columns.Contains(name))
                        {
                            var value = pi.GetValue(obj, null);
                            var v = Json(value, format);

                            Guid g = Guid.Empty;
                            try
                            {
                                Guid.TryParse(value.ToString(), out g);
                            }
                            catch
                            {

                            }

                            if (g != Guid.Empty)
                            {
                                v = g.ToString();
                                jsonb.Append(s).Append(name).Append(":").Append("'" + v + "'");
                            }
                            else
                            {
                                if (pi.PropertyType.Name == "Guid" || pi.Name.IndexOf("Link") != -1)
                                {
                                    jsonb.Append(s).Append(name).Append(":").Append("'" + v + "'");
                                }
                                else
                                {
                                    jsonb.Append(s).Append(name).Append(":").Append(v);
                                }
                            }
                            s = ",";
                        }
                    }

                    jsonb.Append("}");
                    json = jsonb.ToString();
                }
            }
            return json;
        }

        /// <summary>
        /// 生成表示js参数的字符串
        /// </summary>
        /// <param name="csString"></param>
        /// <returns></returns>
        public static string Json(object par,string format="")
        {
            string jsPar = null;
            if (par == null)
            {
                jsPar = "null";
            }
            else if (par is string)
            {
                jsPar = par.ToString().Replace("\n", "\\n").Replace("\r", "\\r").Replace("'", "\\'").Replace("\"", "\\\"");
                jsPar = string.Format("'{0}'", jsPar);
            }
            else if (par.GetType().IsValueType)
            {
                var type = par.GetType();
                if (type.IsGenericType)
                {
                    //如果是T?
                    if (string.IsNullOrEmpty(par.ToString()))
                    {
                        jsPar = "null";
                    }
                    else
                    {
                        var pi = type.GetProperty("Value");

                        var v = pi.FastGetValue(par);

                        jsPar = Json(v);
                    }
                }
                else if (type == typeof(bool))
                {
                    var ok = (bool)par;
                    jsPar = ok ? "true" : "false";
                }
                else if (type == typeof(DateTime))
                {
                    var time = Convert.ToDateTime(par);
                    if (!string.IsNullOrEmpty(format))
                    {
                        jsPar = "'" + string.Format(format, time)+"'";
                    }
                    else
                    {
                        jsPar = "'" + time.ToString() + "'";
                    }
                }
                else
                {
                    jsPar = par.ToString();
                }
            }
            else
            {
                jsPar = JsonObj(par, format);
            }
            return jsPar;
        }



        /// <summary>
        /// 生成表示js参数的字符串（带有双引号）
        /// </summary>
        /// <param name="csString"></param>
        /// <returns></returns>
        public static string Json2(object par)
        {
            string jsPar = null;
            if (par == null)
            {
                jsPar = "null";
            }
            else if (par is string)
            {
                jsPar = par.ToString().Replace("\n", "\\n").Replace("\r", "\\r").Replace("'", "\\'").Replace("\"", "\\\"");
                jsPar = string.Format("\"{0}\"", jsPar);
            }
            else if (par.GetType().IsValueType)
            {
                var type = par.GetType();
                if (type.IsGenericType)
                {
                    //如果是T?
                    if (string.IsNullOrEmpty(par.ToString()))
                    {
                        jsPar = "null";
                    }
                    else
                    {
                        var pi = type.GetProperty("Value");

                        var v = pi.FastGetValue(par);


                        jsPar = Json2(v);
                    }
                }
                else if (type == typeof(bool))
                {
                    var ok = (bool)par;
                    jsPar = ok ? "true" : "false";
                }
                else if (type == typeof(DateTime))
                {
                    var time = Convert.ToDateTime(par);
                    jsPar = string.Format("new Date({0},{1},{2},{3},{4},{5},{6})", time.Year, time.Month - 1, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
                }
                else
                {
                    jsPar = par.ToString();
                }
            }
            else
            {
                jsPar = JsonObj2(par);
            }
            return jsPar;
        }

        /// <summary>
        /// 将一个对象序列化成json格式（带有双引号）
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string JsonObj2(object obj)
        {
            var json = "null";
            if (obj != null)
            {
                if (obj.IsList())
                {
                    var jsonb = new StringBuilder("[");
                    var list = obj as IEnumerable;
                    var s = "";
                    foreach (var sub in list)
                    {
                        var par = Json2(sub);
                        jsonb.Append(s).Append(par);
                        s = ",";
                    }
                    jsonb.Append("]");
                    json = jsonb.ToString();
                }
                else
                {
                    var jsonb = new StringBuilder("{");
                    var type = obj.GetType();
                    var pis = type.GetProperties();

                    var s = "";
                    foreach (var pi in pis)
                    {
                        var name = pi.Name;
                        name = string.Format("\"{0}\"", name);
                        var value = pi.GetValue(obj, null);
                        var v = Json2(value);
                        jsonb.Append(s).Append(name).Append(":").Append(v);
                        s = ",";
                    }

                    jsonb.Append("}");
                    json = jsonb.ToString();
                }
            }
            return json;
        }

        /// <summary>
        /// 格式化脚本文本 Formate("alert({0})","abc")将得到脚本文本alert('abc')，字符串自动加'单引号'
        /// </summary>
        /// <param name="formateText">格式文本</param>
        /// <param name="pars">参数</param>
        /// <returns></returns>
        public static string Formate(string formateScript, params object[] pars)
        {
            string[] jsPars = new string[pars.Length];
            for (int i = 0; i < pars.Length; i++)
            {
                object par = pars[i];
                string jsPar = Json(par);
                jsPars[i] = jsPar;
            }
            var script = formateScript;
            if (pars != null && pars.Length > 0)
            {
                script = string.Format(formateScript, jsPars) + "\n";
            }
            return script;
        }

        /// <summary>
        /// 注册脚本运行
        /// </summary>
        /// <param name="scriptFormate">脚本格式字符串</param>
        /// <param name="pars">参数</param>
        public static void Call(string formateScript, params object[] pars)
        {
            Page page = GetPage();
            string script = Formate(formateScript, pars);
            page.ClientScript.RegisterStartupScript(page.GetType(), NewJsKey(), script, true);
        }

        public static string NewJsKey()
        {
            return string.Format("js{0:n}", Guid.NewGuid());
        }

        /// <summary>
        /// 提示框输出
        /// </summary>
        /// <param name="text"></param>
        public static void Alert(object text)
        {
            Call("alert({0});", text);
        }
        /// <summary>
        /// 页面转向
        /// </summary>
        /// <param name="href"></param>
        public static void TurnTo(string href)
        {
            Call("window.location.href={0};", href);
        }


        /// <summary>
        /// 引用路径的
        /// </summary>
        /// <param name="src"></param>
        public static void Src(string src)
        {
            var script = string.Format(
                "<script type=\"text/javascript\" src=\"{0}\"></script>", src);
            var page = GetPage();
            //page.Response.Write(script);
            page.ClientScript.RegisterStartupScript(page.GetType(), NewJsKey(), script);
        }


        private const string JAVASCRIPT_HEAD = "<script language='javascript'>\n";
        private const string JAVASCRIPT_FOOT = "\n</script>";

        //客户端浏览器执行传入的javascript代码
        //eval('javascript代码'),eval函数参数中的 javascript代码会被浏览器执行
        public static void Eval(Page page, string jsCode)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(JAVASCRIPT_HEAD);
            sb.Append("eval(\"");
            sb.Append(@jsCode);
            sb.Append("\");");
            sb.Append(JAVASCRIPT_FOOT);

            page.ClientScript.RegisterStartupScript(typeof(System.Web.UI.Page), "message", sb.ToString());
        }


        /// <summary>
        /// 自动按比例缩放图片
        /// </summary>
        /// <param name="hpf">上传文件</param>
        /// <param name="maxwidth">最大宽度</param>
        /// <param name="maxheight">最大高度</param>
        /// <param name="RelativePath">文件存放相对路径</param>
        /// <param name="imgfilename">文件名</param>
        /// <param name="PicType">0=图片大小完全一致 1=图片等比例缩小（不放大） 2=图片等比例缩小或放大</param>
        /// <param name="errorMsg">错误信息</param>
        /// <returns></returns>
        public static bool resizeimg(HttpPostedFile hpf, int maxwidth, int maxheight, string RelativePath, string imgfilename, int PicType, out string errorMsg)
        {
            try
            {
                bool IsResizeImg = false;
                float nPercent = 0;
                float nPercentW = 0;
                float nPercentH = 0;
                int imageHeight = maxheight;
                int imageWidth = maxwidth;

                int destWidth = 0;
                int destHeight = 0;

                System.Drawing.Image fullSizeImg = System.Drawing.Image.FromStream(hpf.InputStream);
                switch (PicType)
                {
                    case 0:
                        destWidth = fullSizeImg.Width;
                        destHeight = fullSizeImg.Height;
                        IsResizeImg = true;
                        break;
                    case 1:
                        if (fullSizeImg.Width < maxwidth && fullSizeImg.Height < maxheight)
                        {
                            errorMsg = string.Format("图片像素高宽最小应为：{0}*{1}", maxwidth, maxheight);
                            return false;
                        }


                        nPercentW = ((float)imageWidth / (float)fullSizeImg.Width);
                        nPercentH = ((float)imageHeight / (float)fullSizeImg.Height);

                        if (nPercentH < nPercentW)
                            nPercent = nPercentH;
                        else
                            nPercent = nPercentW;

                        destWidth = (int)(fullSizeImg.Width * nPercent);
                        destHeight = (int)(fullSizeImg.Height * nPercent);

                        IsResizeImg = true;
                        break;
                    default:
                        nPercentW = ((float)imageWidth / (float)fullSizeImg.Width);
                        nPercentH = ((float)imageHeight / (float)fullSizeImg.Height);

                        if (nPercentH < nPercentW)
                            nPercent = nPercentH;
                        else
                            nPercent = nPercentW;

                        destWidth = (int)(fullSizeImg.Width * nPercent);
                        destHeight = (int)(fullSizeImg.Height * nPercent);

                        IsResizeImg = true;
                        break;
                }

                String MyString = imgfilename + ".jpg";

                ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
                // Create an Encoder object based on the GUID
                // for the Quality parameter category.
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                // Create an EncoderParameters object.
                // An EncoderParameters object has an array of EncoderParameter
                // objects. In this case, there is only one
                // EncoderParameter object in the array.
                EncoderParameters myEncoderParameters = new EncoderParameters(1);

                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                myEncoderParameters.Param[0] = myEncoderParameter;


                if (IsResizeImg)
                {
                    System.Drawing.Image.GetThumbnailImageAbort dummyCallBack = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);

                    System.Drawing.Image thumbNailImg = fullSizeImg.GetThumbnailImage(destWidth, destHeight, dummyCallBack, IntPtr.Zero);
                    //Save the thumbnail in Png format. You may change it to a diff format with the ImageFormat property
                    //thumbNailImg.Save(HttpContext.Current.Request.PhysicalApplicationPath + PhysicsPath + MyString, ImageFormat.Jpeg);                      

                    //thumbNailImg.Save(HttpContext.Current.Server.MapPath(RelativePath) + MyString, ImageFormat.Jpeg);

                    thumbNailImg.Save(HttpContext.Current.Server.MapPath(RelativePath) + MyString, jgpEncoder, myEncoderParameters);

                }
                else
                {
                    fullSizeImg.Save(HttpContext.Current.Server.MapPath(RelativePath) + MyString, jgpEncoder, myEncoderParameters);
                }





                errorMsg = "";
                return true;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        //this function is reqd for thumbnail creation
        public static bool ThumbnailCallback()
        {
            return false;
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public static void AlertAndRedirect(string message, string toURL)
        {
            string text1 = "<script language=javascript>alert('{0}');window.location.replace('{1}')</script>";
            HttpContext.Current.Response.Write(string.Format(text1, message, toURL));
        }
    }
}