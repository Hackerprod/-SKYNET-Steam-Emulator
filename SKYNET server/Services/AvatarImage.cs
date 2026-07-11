using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace SKYNET_server.Services;

/// <summary>
/// Normalizes user-provided avatar images so players can upload larger or
/// heavier pictures without hitting a hard size limit. Any supported format is
/// decoded, center-cropped to a square, resized to a standard avatar size and
/// re-encoded as PNG, producing a small and compatible image every time.
/// </summary>
public static class AvatarImage
{
    // Standard square avatar edge, matching the full Steam avatar size and what
    // the client DLL already produces, so uploads stay consistent.
    private const int TargetSize = 184;

    // Guard applied to the raw upload before decoding, only to avoid abuse
    // (decompression bombs / huge files). Everything below is compressed down.
    public const int MaxSourceBytes = 20 * 1024 * 1024;

    /// <summary>
    /// Decodes <paramref name="source"/> (PNG, JPG, WEBP, BMP, GIF, ...),
    /// center-crops it to a square, resizes it to the standard avatar size and
    /// re-encodes it as PNG. Returns false if the input is empty, too large or
    /// not a decodable image.
    /// </summary>
    public static bool TryNormalize(byte[] source, out byte[] png)
    {
        png = Array.Empty<byte>();
        if (source == null || source.Length == 0 || source.Length > MaxSourceBytes)
        {
            return false;
        }

        try
        {
            using var image = Image.Load(source);
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(TargetSize, TargetSize),
                Mode = ResizeMode.Crop,               // center-crop to square, like Steam
                Position = AnchorPositionMode.Center
            }));

            using var output = new MemoryStream();
            image.Save(output, new PngEncoder());
            png = output.ToArray();
            return png.Length > 0;
        }
        catch
        {
            return false;
        }
    }
}
