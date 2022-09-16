using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace SKYNET.Helpers
{
    public class ImageHelper
    {
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

        public static Bitmap GetDesktopWallpaper(bool Aspect4x4 = false)
        {
            try
            {
                RegistryKey Key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
                if (Key == null) return null;
                string filePath = (string)Key.GetValue("WallPaper");
                if (string.IsNullOrEmpty(filePath)) return null;
                if (!File.Exists(filePath)) return null;
                if (Aspect4x4)
                {
                    return (Bitmap)AspectRatio4x4((Bitmap)Bitmap.FromFile(filePath));
                }
                return (Bitmap)Bitmap.FromFile(filePath);
            }
            catch 
            {
                return null;
            }
        }

        public static Image AspectRatio4x4(Image img)
        {
            // 4:3 Aspect Ratio. You can also add it as parameters
            double aspectRatio_X = 4;
            double aspectRatio_Y = 4;

            double imgWidth = Convert.ToDouble(img.Width);
            double imgHeight = Convert.ToDouble(img.Height);

            if (imgWidth / imgHeight > (aspectRatio_X / aspectRatio_Y))
            {
                double extraWidth = imgWidth - (imgHeight * (aspectRatio_X / aspectRatio_Y));
                double cropStartFrom = extraWidth / 2;
                Bitmap bmp = new Bitmap((int)(img.Width - extraWidth), img.Height);
                Graphics grp = Graphics.FromImage(bmp);
                grp.DrawImage(img, new Rectangle(0, 0, (int)(img.Width - extraWidth), img.Height), new Rectangle((int)cropStartFrom, 0, (int)(imgWidth - extraWidth), img.Height), GraphicsUnit.Pixel);
                return bmp;
            }
            else
                return null;
        }


        public static Image ImageFromBytes(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            return Image.FromStream(stream);
        }
        
                public static Bitmap Blur(Bitmap image, int radius)
        {
            Bitmap ProcessedImage = new Bitmap(image);
            unsafe
            {
                var rct = new Rectangle(0, 0, ProcessedImage.Width, ProcessedImage.Height);

                BitmapData bits = ProcessedImage.LockBits(rct, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                int* source = (int*)bits.Scan0;

                if (radius < 1) return ProcessedImage;

                int w = rct.Width;
                int h = rct.Height;
                int wm = w - 1;
                int hm = h - 1;
                int wh = w * h;
                int div = radius + radius + 1;
                var r = new int[wh];
                var g = new int[wh];
                var b = new int[wh];
                int rsum, gsum, bsum, x, y, i, p1, p2, yi;
                var vmin = new int[Max(w, h)];
                var vmax = new int[Max(w, h)];

                var dv = new int[256 * div];
                for (i = 0; i < 256 * div; i++)
                {
                    dv[i] = (i / div);
                }

                int yw = yi = 0;

                for (y = 0; y < h; y++)
                {
                    // blur horizontal
                    rsum = gsum = bsum = 0;
                    for (i = -radius; i <= radius; i++)
                    {
                        int p = source[yi + Min(wm, Max(i, 0))];
                        rsum += (p & 0xff0000) >> 16;
                        gsum += (p & 0x00ff00) >> 8;
                        bsum += p & 0x0000ff;
                    }
                    for (x = 0; x < w; x++)
                    {

                        r[yi] = dv[rsum];
                        g[yi] = dv[gsum];
                        b[yi] = dv[bsum];

                        if (y == 0)
                        {
                            vmin[x] = Min(x + radius + 1, wm);
                            vmax[x] = Max(x - radius, 0);
                        }
                        p1 = source[yw + vmin[x]];
                        p2 = source[yw + vmax[x]];

                        rsum += ((p1 & 0xff0000) - (p2 & 0xff0000)) >> 16;
                        gsum += ((p1 & 0x00ff00) - (p2 & 0x00ff00)) >> 8;
                        bsum += (p1 & 0x0000ff) - (p2 & 0x0000ff);
                        yi++;
                    }
                    yw += w;
                }

                for (x = 0; x < w; x++)
                {
                    // blur vertical
                    rsum = gsum = bsum = 0;
                    int yp = -radius * w;
                    for (i = -radius; i <= radius; i++)
                    {
                        yi = Max(0, yp) + x;
                        rsum += r[yi];
                        gsum += g[yi];
                        bsum += b[yi];
                        yp += w;
                    }
                    yi = x;
                    for (y = 0; y < h; y++)
                    {
                        source[yi] =
                            (int)(0xff000000u | (uint)(dv[rsum] << 16) | (uint)(dv[gsum] << 8) | (uint)dv[bsum]);
                        if (x == 0)
                        {
                            vmin[y] = Min(y + radius + 1, hm) * w;
                            vmax[y] = Max(y - radius, 0) * w;
                        }
                        p1 = x + vmin[y];
                        p2 = x + vmax[y];

                        rsum += r[p1] - r[p2];
                        gsum += g[p1] - g[p2];
                        bsum += b[p1] - b[p2];

                        yi += w;
                    }
                }

                ProcessedImage.UnlockBits(bits);
            }
            return ProcessedImage;
        }

        private static int Min(int a, int b)
        {
            return Math.Min(a, b);
        }

        private static int Max(int a, int b)
        {
            return Math.Max(a, b);
        }

    }
}
