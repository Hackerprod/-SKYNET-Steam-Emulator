using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SKYNET.Client.Services;

/// <summary>Extracts an executable's icon as PNG bytes for the game list.</summary>
public static class IconExtractor
{
    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern uint ExtractIconEx(string szFileName, int nIconIndex, IntPtr[]? phiconLarge, IntPtr[]? phiconSmall, uint nIcons);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIcon(IntPtr hIcon);

    public static byte[]? ExtractPng(string exePath)
    {
        if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath)) return null;

        // Prefer the large icon (typically 32/48px) via ExtractIconEx.
        var large = new IntPtr[1];
        try
        {
            uint count = ExtractIconEx(exePath, 0, large, null, 1);
            if (count > 0 && large[0] != IntPtr.Zero)
            {
                using var icon = Icon.FromHandle(large[0]);
                return ToPng(icon);
            }
        }
        catch { /* fall through */ }
        finally
        {
            if (large[0] != IntPtr.Zero) DestroyIcon(large[0]);
        }

        // Fallback: associated icon (16/32px).
        try
        {
            using var icon = Icon.ExtractAssociatedIcon(exePath);
            return icon == null ? null : ToPng(icon);
        }
        catch
        {
            return null;
        }
    }

    private static byte[] ToPng(Icon icon)
    {
        using var bmp = icon.ToBitmap();
        using var ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Png);
        return ms.ToArray();
    }
}
