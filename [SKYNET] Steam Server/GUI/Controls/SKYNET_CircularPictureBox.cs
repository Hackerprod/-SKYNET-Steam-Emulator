using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Microsoft.VisualBasic.CompilerServices;
using System.Net;
using System.IO;

namespace SKYNET.GUI.Controls
{
    public partial class SKYNET_CircularPictureBox : UserControl
    {
        private Image _Image;

        public Image Image
        {
            get
            {
                return _Image;
            }
            set
            {
                _Image = value;
                pictureBox.Image = CropToCircle(value);
            }
        }
        public SKYNET_CircularPictureBox()
        {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.Inherit;
        }

        public bool LoadFromUrl(string url)
        {
            try
            {
                var request = WebRequest.Create(url);
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    Image image = Image.FromStream(stream);
                    _Image = image;

                    pictureBox.Image = CropToCircle(image);
                }
                return true;
            }
            catch { return false; }
        }
        private void CircularPictureBox_Resize(object sender, EventArgs e)
        {
            this.Size = new Size(Size.Width, Size.Width);
        }
        public static Image CropToCircle(Image image)
        {
            if (image == null)
                return (Image)default;

            if (image.Width < 1000)
            {
                image = ResizeImage((Bitmap)image, 1000, 1000);
            }

            Bitmap bm = (Bitmap)image;
            Bitmap bt = new Bitmap(bm.Width, bm.Height);
            Graphics g = Graphics.FromImage(bt);
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(10, 10, bm.Width - 20, bm.Height - 20);
            g.Clear(Color.Magenta);
            g.SetClip(gp);
            g.DrawImage(bm, new Rectangle(0, 0, bm.Width, bm.Height), 0, 0, bm.Width, bm.Height, GraphicsUnit.Pixel);
            g.Dispose();
            bt.MakeTransparent(Color.Magenta);
            return bt;
        }
        public static Bitmap ResizeImage(Bitmap image, int maxWidth, int maxHeight)
        {
            Bitmap result = (Bitmap)default;
            try
            {
                int width = image.Width;
                int height = image.Height;
                float val = (float)maxWidth / (float)width;
                float val2 = (float)maxHeight / (float)height;
                float num = Math.Min(val, val2);
                checked
                {
                    int width2 = (int)Math.Round((double)unchecked((float)width * num));
                    int height2 = (int)Math.Round((double)unchecked((float)height * num));
                    result = new Bitmap(width2, height2, PixelFormat.Format24bppRgb);
                    using (Graphics graphics = Graphics.FromImage(result))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.DrawImage(image, 0, 0, width2, height2);
                    }
                    ImageCodecInfo encoderInfo = GetEncoderInfo(ImageFormat.Jpeg);
                    System.Drawing.Imaging.Encoder quality = System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameters encoderParameters = new EncoderParameters(1);
                    EncoderParameter encoderParameter = new EncoderParameter(quality, 75L);
                    encoderParameters.Param[0] = encoderParameter;
                    return result;
                }
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                ProjectData.ClearProjectError();
            }
            return result;
        }

        private static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault((ImageCodecInfo c) => c.FormatID == format.Guid);
        }

        private void PictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            base.OnMouseClick(e);
        }

        private void PictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
        }

        internal bool LoadFromStream(MemoryStream stream)
        {
            try
            {
                Image image = Image.FromStream(stream);
                _Image = image;

                pictureBox.Image = CropToCircle(image);
                return true;
            }
            catch { return false; }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
        }
    }
}

