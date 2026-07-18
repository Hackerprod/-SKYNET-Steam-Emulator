using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SKYNET.Client.Services;

/// <summary>Converts raw PNG/JPG bytes into a frozen WPF <see cref="ImageSource"/>.</summary>
public static class Images
{
    public static ImageSource? FromBytes(byte[]? bytes)
    {
        if (bytes == null || bytes.Length == 0) return null;
        try
        {
            var bmp = new BitmapImage();
            using var ms = new MemoryStream(bytes);
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.StreamSource = ms;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }
        catch
        {
            return null;
        }
    }
}
