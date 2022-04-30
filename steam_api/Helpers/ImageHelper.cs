using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKYNET.Helper
{
    public class ImageHelper
    {
        public static Bitmap CaptureScreen()
        {
            Bitmap result = null;
            try
            {
                Rectangle bounds = Screen.GetBounds(Point.Empty);
                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    }
                    result = bitmap;
                }

                return result;
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                return result;
            }

        }

        public static Bitmap Resize(Bitmap image, int width, int height)
        {
            Rectangle destRect = new Rectangle(0, 0, width, height);
            Bitmap bitmap = new Bitmap(width, height);
            bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                using (ImageAttributes imageAttributes = new ImageAttributes())
                {
                    imageAttributes.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
                    return bitmap;
                }
            }
        }

        public static Image CreateCopy(Image image)
        {
            byte[] sourceImage = ImageToBytes(image);
            return ImageFromBytes(sourceImage);
        }

        public static byte[] ImageToBytes(Image image)
        {
            byte[] imageArray = new byte[0];

            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Jpeg);
                stream.Close();
                imageArray = stream.ToArray();
            }
            return imageArray;
        }

        public static byte[] ConvertToRGBA(Bitmap image)
        {
            byte[] rgbaB = new byte[4 * (image.Width * image.Height)];
            int i = 0;
            for (var y = 0; y < image.Width; y++)
            {
                for (var x = 0; x < image.Height; x++)
                {
                    Color pix = image.GetPixel(x, y);
                    rgbaB[i++] = pix.R;
                    rgbaB[i++] = pix.G;
                    rgbaB[i++] = pix.B;
                    rgbaB[i++] = pix.A;
                }
            }
            return rgbaB;
        }

        public static Bitmap GetDesktopWallpaper()
        {
            try
            {
                RegistryKey Key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
                if (Key == null) return null;
                string filePath = (string)Key.GetValue("WallPaper");
                if (string.IsNullOrEmpty(filePath)) return null;
                if (!File.Exists(filePath)) return null;
                return (Bitmap)Bitmap.FromFile(filePath);
            }
            catch 
            {
                return null;
            }
        }


        public static Image ImageFromBytes(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            return Image.FromStream(stream);
        }

        public static Image ImageFromStream(Stream stream)
        {
            return Image.FromStream(stream);
        }
    }
}
