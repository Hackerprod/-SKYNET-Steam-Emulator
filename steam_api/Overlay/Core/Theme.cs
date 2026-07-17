using System.Drawing;

namespace Overlay.Core;

internal static class Theme
{
    public static readonly Color BgBase = Hex("#0a0a0a");
    public static readonly Color BgDark = Hex("#111111");
    public static readonly Color BgCard = Hex("#161616");
    public static readonly Color BgPanel = Hex("#101010");
    public static readonly Color Border = Hex("#252525");
    public static readonly Color BorderSoft = Hex("#303030");

    public static readonly Color TextMuted = Hex("#8a8f98");
    public static readonly Color TextBody = Hex("#e5e7eb");
    public static readonly Color TextTitle = Hex("#ffffff");

    public static readonly Color Accent = Hex("#3b82f6");
    public static readonly Color AccentHover = Hex("#2563eb");
    public static readonly Color Success = Hex("#22c55e");
    public static readonly Color Error = Hex("#ef4444");
    public static readonly Color Warning = Hex("#eab308");

    public static readonly string FontName = "Segoe UI";

    public static Color Soft(Color color, int alpha)
    {
        return Color.FromArgb(alpha, color.R, color.G, color.B);
    }

    private static Color Hex(string hex)
    {
        return ColorTranslator.FromHtml(hex);
    }
}
