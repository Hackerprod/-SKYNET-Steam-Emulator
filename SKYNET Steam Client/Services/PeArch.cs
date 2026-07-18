using SKYNET.Client.Models;

namespace SKYNET.Client.Services;

/// <summary>Reads the PE header of an executable to detect x86 vs x64.</summary>
public static class PeArch
{
    // IMAGE_FILE_MACHINE_*
    private const ushort I386 = 0x014c;
    private const ushort AMD64 = 0x8664;

    public static GameArch Detect(string exePath)
    {
        try
        {
            using var fs = new FileStream(exePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var br = new BinaryReader(fs);

            // DOS header: 'MZ', e_lfanew at 0x3C.
            if (br.ReadUInt16() != 0x5A4D) return GameArch.Unknown;
            fs.Position = 0x3C;
            int peOffset = br.ReadInt32();

            fs.Position = peOffset;
            if (br.ReadUInt32() != 0x00004550) return GameArch.Unknown; // 'PE\0\0'

            ushort machine = br.ReadUInt16();
            return machine switch
            {
                AMD64 => GameArch.X64,
                I386 => GameArch.X86,
                _ => GameArch.Unknown
            };
        }
        catch
        {
            return GameArch.Unknown;
        }
    }
}
