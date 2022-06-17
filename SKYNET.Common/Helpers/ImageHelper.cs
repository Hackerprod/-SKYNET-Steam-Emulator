using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
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

        public static Bitmap RotateBitmap_NoPadding(Bitmap image, double degrees, Color? bgColor)
        {
            PointF[] points = new PointF[] {
                new PointF(0, 0),
                new PointF(image.Width, 0),
                new PointF(0, image.Height),
                new PointF(image.Width, image.Height),
                };

            double angle = PolarPoint.ToRadians(degrees);

            float minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;
            for (int i = 0; i < points.Length; i++)
            {
                PolarPoint p = new PolarPoint(points[i]);
                p.Angle += angle;
                points[i] = p.ToPointF();

                minX = Math.Min(minX, points[i].X);
                minY = Math.Min(minY, points[i].Y);
                maxX = Math.Max(maxX, points[i].X);
                maxY = Math.Max(maxY, points[i].Y);
            }

            Size size = Size.Ceiling(new SizeF(maxX - minX, maxY - minY));
            Bitmap target = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(target))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                if (bgColor != null && bgColor != Color.Transparent)
                {
                    using (Brush b = new SolidBrush(bgColor.Value))
                        g.FillRectangle(b, 0, 0, size.Width, size.Height);
                }

                g.TranslateTransform(-minX, -minY);
                g.DrawImage(image, new PointF[] { points[0], points[1], points[2] });
            }
            return target;
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

        public static Image ImageFromStream(Stream stream)
        {
            return Image.FromStream(stream);
        }

        public class ImageResizer
        {
            internal static void AdjustSizes(Bitmap bitmap, ref int xSize, ref int ySize)
            {
                if (xSize != 0 && ySize == 0)
                    ySize = Math.Abs((int)(xSize * bitmap.Height / bitmap.Width));
                else if (xSize == 0 && ySize != 0)
                    xSize = Math.Abs((int)(ySize * bitmap.Width / bitmap.Height));
                else if (xSize == 0 && ySize == 0)
                {
                    xSize = bitmap.Width;
                    ySize = bitmap.Height;
                }
            }

            //Internal resize for indexed colored images
            static unsafe Bitmap IndexedResize(Bitmap bitmap, int xSize, int ySize, ImageFormat format)
            {
                AdjustSizes(bitmap, ref xSize, ref ySize);

                Bitmap scaledBitmap = new Bitmap(xSize, ySize, bitmap.PixelFormat);
                scaledBitmap.Palette = bitmap.Palette;

                int sourceWidth = bitmap.Width;   // width of source
                int sourceHeight = bitmap.Height;  // height of source
                int destWidth = scaledBitmap.Width;  // width of dest
                int destHeight = scaledBitmap.Height; // height of dest

                switch (bitmap.PixelFormat)
                {
                    case PixelFormat.Format1bppIndexed:
                    case PixelFormat.Format4bppIndexed:
                        throw new ArgumentException("Unsupported pixel format " + bitmap.PixelFormat);
                    case PixelFormat.Format8bppIndexed:
                        break;
                    default:
                        throw new ArgumentException("Unsupported pixel format " + bitmap.PixelFormat);
                }

                float xRatio = (float)sourceWidth / destWidth;
                int xOffset = (int)(xRatio / 2);
                float yRatio = (float)sourceHeight / destHeight;
                int yOffset = (int)(yRatio / 2);

                BitmapData sourceBitmapData = bitmap.LockBits(new Rectangle(0, 0, sourceWidth, sourceHeight), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                try
                {
                    BitmapData destBitmapData = scaledBitmap.LockBits(new Rectangle(0, 0, destWidth, destHeight), ImageLockMode.WriteOnly, scaledBitmap.PixelFormat);
                    try
                    {
                        byte* s0 = (byte*)sourceBitmapData.Scan0.ToPointer();
                        int sourceStride = sourceBitmapData.Stride;
                        byte* d0 = (byte*)destBitmapData.Scan0.ToPointer();
                        int destStride = destBitmapData.Stride;

                        for (int y = 0; y < destHeight; y++)
                        {
                            byte* d = d0 + y * destStride;
                            byte* sRow = s0 + ((int)(y * yRatio) + yOffset) * sourceStride + xOffset;

                            // nearest y neighbor row

                            for (int x = 0; x < destWidth; x++)
                            {
                                *d++ = *(sRow + (int)(x * xRatio));
                            }
                        }

                    }
                    finally
                    {
                        scaledBitmap.UnlockBits(destBitmapData);
                    }
                }
                finally
                {
                    bitmap.UnlockBits(sourceBitmapData);
                }

                MemoryStream ms = new MemoryStream();
                scaledBitmap.Save(ms, format);
                scaledBitmap.Dispose();
                ms.Seek(0, SeekOrigin.Begin);
                return new Bitmap(new MemoryStream(ms.ToArray()));
            }


            private static void DoDrawImage(Graphics g, Bitmap bitmap, Rectangle destRect, Rectangle srcRect)
            {
                /*
                                using (bitmap = bitmap.Clone(srcRect, bitmap.PixelFormat))
                                    using (TextureBrush brush = new TextureBrush(bitmap, WrapMode.Tile, new Rectangle(0, 0, bitmap.Width, bitmap.Height)))
                                        g.FillRectangle(brush, destRect);
                */

                g.DrawImage(bitmap, destRect, srcRect, GraphicsUnit.Pixel);
            }

            static Bitmap StandardResize(Bitmap bitmap, int xSize, int ySize)
            {
                AdjustSizes(bitmap, ref xSize, ref ySize);
                return new Bitmap(bitmap, xSize, ySize);
            }

            public static Size ScaledResize(Bitmap bitmap, Size maxSize)
            {
                double width = bitmap.Width;
                double height = bitmap.Height;

                if (width > maxSize.Width)
                {
                    double d = maxSize.Width / (double)bitmap.Width;
                    width = maxSize.Width;
                    height = (int)(height * d);
                }

                if (height > maxSize.Height)
                {
                    double d = maxSize.Height / (double)height;
                    height = maxSize.Height;
                    width = (int)(width * d);
                }

                return new Size((int)Math.Max(width, 1), (int)Math.Max(height, 1));
            }

            public static Bitmap ResizeBitmap(Bitmap bitmap, int xSize, int ySize, ImageFormat format)
            {
                if (IsIndexedWithTransparency(bitmap, format))
                {
                    try
                    {
                        return IndexedResize(bitmap, xSize, ySize, format);
                    }
                    catch (Exception e)
                    {
                        //Warning:  aborted execution of IndexedResize has been seen to cause
                        //"object in use" errors downstream when using the source bitmap.
                        //Avoid the use of this method for this image!
                        //Trace.Fail("An error while using the indexed image resize algorithm", e.ToString());
                    }
                }

                switch (bitmap.PixelFormat)
                {
                    case PixelFormat.Format24bppRgb:
                    case PixelFormat.Format32bppArgb:
                    case PixelFormat.Format32bppPArgb:
                    case PixelFormat.Format32bppRgb:
                    //return RGBResize(bitmap, xSize, ySize);
                    default:
                        return StandardResize(bitmap, xSize, ySize);
                }
            }
        }

        public static Image FromFile(string imagePath)
        {
            byte[] bytes = File.ReadAllBytes(imagePath);
            Image image = ImageFromBytes(bytes);
            return image;
        }

        public static void ToFile(string imagePath, Image image)
        {
            byte[] bytes = ImageHelper.ImageToBytes(image);
            File.WriteAllBytes(imagePath, bytes);
        }

        /// <summary>
        /// Use indexed resize if we are dealing with a transparent, indexed format.
        /// </summary>
        private static bool IsIndexedWithTransparency(Bitmap bitmap, ImageFormat targetFormat)
        {
            if ((bitmap.PixelFormat & PixelFormat.Indexed) != PixelFormat.Indexed)
                return false;

            // Bug 580406: This doesn't work with our indexed resize algorithm
            if (bitmap.PixelFormat == PixelFormat.Format1bppIndexed)
                return false;

            //avoid bug 449902 - Writer will crash when trying to resize an indexed PNG
            if (targetFormat.Equals(ImageFormat.Png))
                return false;

            foreach (Color c in bitmap.Palette.Entries)
                if (c.A == 0)
                    return true;

            return false;
        }

        internal struct PolarPoint
        {
            /// <summary>
            /// Angle in radians
            /// </summary>
            public double Angle;
            public double Radius;

            public PolarPoint(PointF p)
            {
                Angle = Math.Atan2(p.Y, p.X);
                Radius = Math.Sqrt(p.X * p.X + p.Y * p.Y);
            }

            public PointF ToPointF()
            {
                return new PointF(
                    (float)(Radius * Math.Cos(Angle)),
                    (float)(Radius * Math.Sin(Angle)));
            }

            public static double ToRadians(double degrees)
            {
                return Math.PI / 180 * degrees;
            }

            public static double ToDegrees(double radians)
            {
                return radians * 180 / Math.PI;
            }
        }


    }
}
