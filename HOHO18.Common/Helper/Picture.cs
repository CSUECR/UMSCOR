using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Text;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Drawing;


namespace HOHO18.Common.Helper
{
    public class Picture
    {
        //private const string PERSONPICTURE = "~/相册库/";
        string webConfigImagePath = ConfigurationManager.AppSettings["UploadPicturePath"];
        //string path = Server.MapPath(webConfigImagePath);
        //--水印定位类型 1.左上 2.右上 3.左下 4.右下--
        private  string WatermarkType = ConfigurationManager.AppSettings["WatermarkType"];
        //--水印配对图地址--
        private  string WatermarkUrl = ConfigurationManager.AppSettings["WatermarkUrl"];
        //--水印默认图地址-
        private  string DefaultImageUrl = ConfigurationManager.AppSettings["DefaultImageUrl"];

        #region IHttpHandler 成员

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context,String pictureName)
        {
            System.Drawing.Image image;
            Bitmap bm;
            string path = context.Request.MapPath(webConfigImagePath + pictureName);
            if (File.Exists(path))
            {
                image = System.Drawing.Image.FromFile(path);
                System.Drawing.Image watermark = System.Drawing.Image.FromFile(context.Request.MapPath(WatermarkUrl));
                bm = new Bitmap(image);
                Graphics g = Graphics.FromImage(bm);
                if (WatermarkType == "1")
                {                    
                    g.DrawImage(watermark, 0, 0, watermark.Width, watermark.Height);
                }
                else if (WatermarkType == "2")
                {
                    g.DrawImage(watermark, bm.Width - watermark.Width, 0, watermark.Width, watermark.Height);
                }
                else if (WatermarkType == "3")
                {
                    g.DrawImage(watermark, 0, bm.Height - watermark.Height, watermark.Width, watermark.Height);
                }
                else if (WatermarkType == "4")
                {
                    g.DrawImage(watermark, bm.Width - watermark.Width, bm.Height - watermark.Height, watermark.Width, watermark.Height);
                }
                else {
                    g.DrawImage(watermark, bm.Width - watermark.Width, bm.Height - watermark.Height, watermark.Width, watermark.Height);
                }
                g.Dispose();
                watermark.Dispose();
            }
            else
            {
                image = System.Drawing.Image.FromFile(context.Request.MapPath(DefaultImageUrl));
                bm = new Bitmap(image);

            }
            context.Request.ContentType = "image/gif";
            bm.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Gif);
            bm.Dispose();
            image.Dispose();
            context.Response.End();
        }

        #endregion
    }
}