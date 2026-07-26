using System.Text;

namespace SKYNET.Client.Services;

/// <summary>
/// Reads only the PE import/export tables needed by the launcher. Keeping this
/// parser managed avoids loading a payload DLL into the launcher merely to inspect
/// its exports, which also keeps x86 game support available from an x64 launcher.
/// </summary>
internal static class PeImports
{
    internal sealed class ImportSymbol
    {
        public string ModuleName { get; set; } = "";
        public string? Name { get; set; }
        public ushort? Ordinal { get; set; }
        public uint IatRva { get; set; }
        public int PointerSize { get; set; }
    }

    public static bool ImportsModule(string path, string moduleName)
    {
        try
        {
            return ReadImports(path).Any(import =>
                string.Equals(import.ModuleName, moduleName, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return false;
        }
    }

    public static IReadOnlyList<ImportSymbol> ReadImports(string path)
    {
        using var image = new PeImage(path);
        return image.ReadImports();
    }

    public static IReadOnlyList<uint> FindImportNameFieldRvas(string path, string moduleName)
    {
        using var image = new PeImage(path);
        return image.FindImportNameFieldRvas(moduleName);
    }

    private sealed class PeImage : IDisposable
    {
        private const ushort DosSignature = 0x5A4D;
        private const uint PeSignature = 0x00004550;
        private const ushort Pe32 = 0x10B;
        private const ushort Pe32Plus = 0x20B;
        private readonly FileStream _stream;
        private readonly BinaryReader _reader;
        private readonly List<Section> _sections = new();
        private readonly uint _importRva;

        public PeImage(string path)
        {
            _stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            _reader = new BinaryReader(_stream, Encoding.ASCII, leaveOpen: true);

            if (ReadUInt16(0) != DosSignature)
                throw new InvalidDataException("The file is not a PE image.");

            var peOffset = ReadUInt32(0x3C);
            if (ReadUInt32(peOffset) != PeSignature)
                throw new InvalidDataException("The file has an invalid PE signature.");

            var sectionCount = ReadUInt16(peOffset + 6);
            var optionalHeaderSize = ReadUInt16(peOffset + 20);
            var optionalHeader = peOffset + 24;
            var magic = ReadUInt16(optionalHeader);
            PointerSize = magic switch
            {
                Pe32 => 4,
                Pe32Plus => 8,
                _ => throw new InvalidDataException("The file has an unsupported PE optional header.")
            };

            var dataDirectories = optionalHeader + (PointerSize == 8 ? 112u : 96u);
            _importRva = ReadUInt32(dataDirectories + 8);

            var sectionOffset = optionalHeader + optionalHeaderSize;
            for (var index = 0; index < sectionCount; index++)
            {
                var offset = sectionOffset + (uint)(index * 40);
                _sections.Add(new Section(
                    ReadUInt32(offset + 8),
                    ReadUInt32(offset + 12),
                    ReadUInt32(offset + 16),
                    ReadUInt32(offset + 20)));
            }
        }

        public int PointerSize { get; }

        public IReadOnlyList<ImportSymbol> ReadImports()
        {
            var result = new List<ImportSymbol>();
            if (_importRva == 0)
                return result;

            var descriptorOffset = RvaToOffset(_importRva);
            for (var descriptorIndex = 0; ; descriptorIndex++)
            {
                var offset = descriptorOffset + (uint)(descriptorIndex * 20);
                var originalFirstThunk = ReadUInt32(offset);
                var nameRva = ReadUInt32(offset + 12);
                var firstThunk = ReadUInt32(offset + 16);
                if (originalFirstThunk == 0 && nameRva == 0 && firstThunk == 0)
                    break;
                if (nameRva == 0 || firstThunk == 0)
                    continue;

                var moduleName = ReadAnsiZ(RvaToOffset(nameRva));
                var lookupThunk = originalFirstThunk == 0 ? firstThunk : originalFirstThunk;
                for (var thunkIndex = 0; ; thunkIndex++)
                {
                    var thunkRva = lookupThunk + (uint)(thunkIndex * PointerSize);
                    var thunkValue = PointerSize == 8
                        ? ReadUInt64(RvaToOffset(thunkRva))
                        : ReadUInt32(RvaToOffset(thunkRva));
                    if (thunkValue == 0)
                        break;

                    var isOrdinal = PointerSize == 8
                        ? (thunkValue & 0x8000000000000000UL) != 0
                        : (thunkValue & 0x80000000U) != 0;
                    var iatRva = firstThunk + (uint)(thunkIndex * PointerSize);
                    if (isOrdinal)
                    {
                        result.Add(new ImportSymbol
                        {
                            ModuleName = moduleName,
                            Ordinal = (ushort)(thunkValue & 0xFFFF),
                            IatRva = iatRva,
                            PointerSize = PointerSize
                        });
                        continue;
                    }

                    var importByNameOffset = RvaToOffset((uint)thunkValue);
                    result.Add(new ImportSymbol
                    {
                        ModuleName = moduleName,
                        Name = ReadAnsiZ(importByNameOffset + 2),
                        IatRva = iatRva,
                        PointerSize = PointerSize
                    });
                }
            }

            return result;
        }

        public IReadOnlyList<uint> FindImportNameFieldRvas(string moduleName)
        {
            var result = new List<uint>();
            if (_importRva == 0)
                return result;

            var descriptorOffset = RvaToOffset(_importRva);
            for (var descriptorIndex = 0; ; descriptorIndex++)
            {
                var descriptorRva = _importRva + (uint)(descriptorIndex * 20);
                var offset = descriptorOffset + (uint)(descriptorIndex * 20);
                var originalFirstThunk = ReadUInt32(offset);
                var nameRva = ReadUInt32(offset + 12);
                var firstThunk = ReadUInt32(offset + 16);
                if (originalFirstThunk == 0 && nameRva == 0 && firstThunk == 0)
                    break;
                if (nameRva != 0 && string.Equals(ReadAnsiZ(RvaToOffset(nameRva)), moduleName, StringComparison.OrdinalIgnoreCase))
                    result.Add(descriptorRva + 12);
            }

            return result;
        }

        private uint RvaToOffset(uint rva)
        {
            foreach (var section in _sections)
            {
                var size = Math.Max(section.VirtualSize, section.RawSize);
                if (rva >= section.VirtualAddress && rva < section.VirtualAddress + size)
                    return section.RawOffset + rva - section.VirtualAddress;
            }

            throw new InvalidDataException($"RVA 0x{rva:X8} is outside the PE sections.");
        }

        private ushort ReadUInt16(uint offset)
        {
            _stream.Position = offset;
            return _reader.ReadUInt16();
        }

        private uint ReadUInt32(uint offset)
        {
            _stream.Position = offset;
            return _reader.ReadUInt32();
        }

        private ulong ReadUInt64(uint offset)
        {
            _stream.Position = offset;
            return _reader.ReadUInt64();
        }

        private string ReadAnsiZ(uint offset)
        {
            _stream.Position = offset;
            var bytes = new List<byte>();
            for (var index = 0; index < 4096; index++)
            {
                var value = _reader.ReadByte();
                if (value == 0)
                    return Encoding.ASCII.GetString(bytes.ToArray());
                bytes.Add(value);
            }

            throw new InvalidDataException("PE string exceeds the supported length.");
        }

        public void Dispose()
        {
            _reader.Dispose();
            _stream.Dispose();
        }

        private readonly struct Section
        {
            public Section(uint virtualSize, uint virtualAddress, uint rawSize, uint rawOffset)
            {
                VirtualSize = virtualSize;
                VirtualAddress = virtualAddress;
                RawSize = rawSize;
                RawOffset = rawOffset;
            }

            public uint VirtualSize { get; }
            public uint VirtualAddress { get; }
            public uint RawSize { get; }
            public uint RawOffset { get; }
        }
    }
}
