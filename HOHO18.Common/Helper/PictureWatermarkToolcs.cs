using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Text;
using System.Web.Security;
using System.Web.UI;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;


namespace HOHO18.Common.Helper
{
    public class PictureWatermarkTool
    {
        //private const string PERSONPICTURE = "~/相册库/";
        private static string webConfigImagePath = ConfigurationManager.AppSettings["UploadPicturePath"];
        //string path = Server.MapPath(webConfigImagePath);
        //--水印定位类型 1.左上 2.右上 3.左下 4.右下--
        private static string WatermarkType = ConfigurationManager.AppSettings["WatermarkType"];
        //--水印配对图地址--
        private static string WatermarkUrl = ConfigurationManager.AppSettings["WatermarkUrl"];
        //--水印默认图地址-
        private static string DefaultImageUrl = ConfigurationManager.AppSettings["NoImagePath_Small"];

        
        /// <summary>
        /// 水印操作
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pictureName">ID</param>
        /// <param name="scale">水印占对象图片的比例,1为百分百,0表示保持原样</param>
        public static void ProcessRequest(Stream imgStream, String savePath, String watermarkPath, float opacity, float scale)
        {
            System.Drawing.Image image = System.Drawing.Image.FromStream(imgStream);

            //对水印进行半透明处理
            float[][] nArray ={ new float[] {1, 0, 0, 0, 0},
                                new float[] {0, 1, 0, 0, 0},
                                new float[] {0, 0, 1, 0, 0},
                                new float[] {0, 0, 0, opacity, 0},
                                new float[] {0, 0, 0, 0, 1} };
            ColorMatrix matrix = new ColorMatrix(nArray);
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            Image srcWatermarkImage = System.Drawing.Image.FromFile(watermarkPath);
            Bitmap resultWatermarkImage = new Bitmap(srcWatermarkImage.Width, srcWatermarkImage.Height);
            Graphics watermarkG = Graphics.FromImage(resultWatermarkImage);
            watermarkG.DrawImage(srcWatermarkImage, new Rectangle(0, 0, srcWatermarkImage.Width, srcWatermarkImage.Height), 0, 0, srcWatermarkImage.Width, srcWatermarkImage.Height, GraphicsUnit.Pixel, attributes);

            //水印最终大小
            int width, height;
            if (scale > 0)//按比例
            {
                if (scale > 1) scale = 1;
                float wScale = (float)resultWatermarkImage.Width / resultWatermarkImage.Height;
                if (wScale > (float)image.Width / image.Height)
                {
                    width = (int)(image.Width * scale);
                    height = (int)(width / wScale);
                }
                else
                {
                    height = (int)(image.Height * scale);
                    width = (int)(height * wScale);
                }
            }
            else//原始大小
            {
                width = resultWatermarkImage.Width;
                height = resultWatermarkImage.Height;
            }

            Bitmap bm = new Bitmap(image);
            Graphics g = Graphics.FromImage(bm);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            if (WatermarkType == "1")
            {
                g.DrawImage(resultWatermarkImage, 0, 0, width, resultWatermarkImage.Height);
            }
            else if (WatermarkType == "2")
            {
                g.DrawImage(resultWatermarkImage, bm.Width - width, 0, width, height);
            }
            else if (WatermarkType == "3")
            {
                g.DrawImage(resultWatermarkImage, 0, bm.Height - height, width, height);
            }
            else if (WatermarkType == "4")
            {
                g.DrawImage(resultWatermarkImage, bm.Width - width, bm.Height - height, width, height);
            }
            else
            {
                g.DrawImage(resultWatermarkImage, bm.Width - width, bm.Height - height, width, height);
            }



            g.Dispose();
            resultWatermarkImage.Dispose();
            bm.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
            bm.Dispose();
            image.Dispose();
        }

        /// <summary>
        /// 水印操作
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pictureName">ID</param>
        public static void ProcessRequest(Stream imgStream, String savePath, String watermarkPath, float opacity)
        {
            ProcessRequest(imgStream, savePath, watermarkPath, opacity, (float)0.3);
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式:(HW:指定高宽缩放（可能变形）)
        /// (W:指定宽，高按比例)
        /// (H:指定高，宽按比例)
        /// (Cut:指定高宽裁减（不变形）)
        /// </param>    
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）
                    break;
                case "W"://指定宽，高按比例
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                case "WH_Max"://指定宽高,按比例缩放原图,原图并不会变形,但宽高取长的最比例
                    if ((double)ow / (double)oh > (double)towidth / (double)toheight)
                    {
                        //如果宽高比大于要设置的宽高比,那么先定宽,然后根据原图比例得到高
                        if (ow < towidth) towidth = ow;
                        toheight = oh * towidth / ow;
                    }
                    else
                    {
                        //如果宽高比小于等于要设置的宽高比,那么先定高,然后根据原图比例得到宽
                        if (oh < toheight) toheight = oh;//如果原图比要设置的图片还大,那么就取原来的大小
                        towidth = ow * toheight / oh;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);

            try
            {
                //以jpg格式保存缩略图
                bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }

    }
    public delegate void PictureWatermarkDelegate(Stream imgStream, String savePath, String watermarkPath, float opacity);
}