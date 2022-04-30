using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Helper
{
    public static class StreamHelpers
    {
        private static byte[] data;
        private static void EnsureInitialized()
        {
            if (data == null)
            {
                data = new byte[8];
            }
        }
        public static float ReadFloat(this Stream stream)
        {
            EnsureInitialized();
            stream.Read(data, 0, 4);
            return BitConverter.ToSingle(data, 0);
        }
        public static long ReadInt64(this Stream stream)
        {
            EnsureInitialized();
            stream.Read(data, 0, 8);
            return BitConverter.ToInt64(data, 0);
        }
        public static ulong ReadUInt64(this Stream stream)
        {
            EnsureInitialized();
            stream.Read(data, 0, 8);
            return BitConverter.ToUInt64(data, 0);
        }
        public static int ReadInt32(this Stream stream)
        {
            EnsureInitialized();
            stream.Read(data, 0, 4);
            return BitConverter.ToInt32(data, 0);
        }

        private static byte Short1(short x)
        {
            return (byte)(x >> 8);
        }

        private static byte Short0(short x)
        {
            return (byte)x;
        }

        private static byte Int3(int x)
        {
            return (byte)(x >> 24);
        }

        private static byte Int2(int x)
        {
            return (byte)(x >> 16);
        }

        private static byte Int1(int x)
        {
            return (byte)(x >> 8);
        }

        private static byte Int0(int x)
        {
            return (byte)x;
        }

        private static byte Long7(long x)
        {
            return (byte)(x >> 56);
        }

        private static byte Long6(long x)
        {
            return (byte)(x >> 48);
        }

        private static byte Long5(long x)
        {
            return (byte)(x >> 40);
        }

        private static byte Long4(long x)
        {
            return (byte)(x >> 32);
        }

        private static byte Long3(long x)
        {
            return (byte)(x >> 24);
        }

        private static byte Long2(long x)
        {
            return (byte)(x >> 16);
        }

        private static byte Long1(long x)
        {
            return (byte)(x >> 8);
        }

        private static byte Long0(long x)
        {
            return (byte)x;
        }

        private static short MakeInt16L(byte b0, byte b1)
        {
            return (short)((b1 << 8) | (b0 & 0xFF));
        }

        private static short MakeInt16B(byte b1, byte b0)
        {
            return MakeInt16L(b1, b0);
        }

        private static int MakeInt32L(byte b0, byte b1, byte b2, byte b3)
        {
            return ((b3 & 0xFF) << 24) | ((b2 & 0xFF) << 16) | ((b1 & 0xFF) << 8) | (b0 & 0xFF);
        }

        private static int MakeInt32B(byte b3, byte b2, byte b1, byte b0)
        {
            return MakeInt32L(b3, b2, b1, b0);
        }

        private static long MakeInt64L(byte b0, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7)
        {
            return (((long)b7 & 255L) << 56) | (((long)b6 & 255L) << 48) | (((long)b5 & 255L) << 40) | (((long)b4 & 255L) << 32) | (((long)b3 & 255L) << 24) | (((long)b2 & 255L) << 16) | (((long)b1 & 255L) << 8) | ((long)b0 & 255L);
        }

        private static long MakeInt64B(byte b7, byte b6, byte b5, byte b4, byte b3, byte b2, byte b1, byte b0)
        {
            return MakeInt64L(b7, b6, b5, b4, b3, b2, b1, b0);
        }

        public static short ReadInt16L(this Stream stream)
        {
            return MakeInt16L((byte)stream.ReadByte(), (byte)stream.ReadByte());
        }

        public static ushort ReadUInt16L(this Stream stream)
        {
            return (ushort)ReadInt16L(stream);
        }

        public static short ReadInt16B(this Stream stream)
        {
            return MakeInt16B((byte)stream.ReadByte(), (byte)stream.ReadByte());
        }

        public static ushort ReadUInt16B(this Stream stream)
        {
            return (ushort)ReadInt16B(stream);
        }

        public static void WriteInt16L(this Stream stream, short v)
        {
            stream.WriteByte(Short0(v));
            stream.WriteByte(Short1(v));
        }

        public static void WriteUInt16L(this Stream stream, ushort v)
        {
            WriteInt16L(stream, (short)v);
        }

        public static void WriteInt16B(this Stream stream, short v)
        {
            stream.WriteByte(Short1(v));
            stream.WriteByte(Short0(v));
        }

        public static void WriteUInt16B(this Stream stream, ushort v)
        {
            WriteInt16B(stream, (short)v);
        }

        public static int ReadInt32L(this Stream stream)
        {
            return MakeInt32L((byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte());
        }

        public static uint ReadUInt32L(this Stream stream)
        {
            return (uint)ReadInt32L(stream);
        }

        public static int ReadInt32B(this Stream stream)
        {
            return MakeInt32B((byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte());
        }

        public static uint ReadUInt32B(this Stream stream)
        {
            return (uint)ReadInt32B(stream);
        }

        public static void WriteInt32L(this Stream stream, int v)
        {
            stream.WriteByte(Int0(v));
            stream.WriteByte(Int1(v));
            stream.WriteByte(Int2(v));
            stream.WriteByte(Int3(v));
        }

        public static void WriteUInt32L(this Stream stream, uint v)
        {
            WriteInt32L(stream, (int)v);
        }

        public static void WriteInt32B(this Stream stream, int v)
        {
            stream.WriteByte(Int3(v));
            stream.WriteByte(Int2(v));
            stream.WriteByte(Int1(v));
            stream.WriteByte(Int0(v));
        }

        public static void WriteUInt32B(this Stream stream, uint v)
        {
            WriteInt32B(stream, (int)v);
        }

        public static long ReadInt64L(this Stream stream)
        {
            return MakeInt64L((byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte());
        }

        public static ulong ReadUInt64L(this Stream stream)
        {
            return (ulong)ReadInt64L(stream);
        }

        public static long ReadInt64B(this Stream stream)
        {
            return MakeInt64B((byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte());
        }

        public static ulong ReadUInt64B(this Stream stream)
        {
            return (ulong)ReadInt64B(stream);
        }

        public static void WriteInt64L(this Stream stream, long v)
        {
            stream.WriteByte(Long0(v));
            stream.WriteByte(Long1(v));
            stream.WriteByte(Long2(v));
            stream.WriteByte(Long3(v));
            stream.WriteByte(Long4(v));
            stream.WriteByte(Long5(v));
            stream.WriteByte(Long6(v));
            stream.WriteByte(Long7(v));
        }

        public static void WriteUInt64L(this Stream stream, ulong v)
        {
            WriteInt64L(stream, (long)v);
        }

        public static void WriteInt64B(this Stream stream, long v)
        {
            stream.WriteByte(Long7(v));
            stream.WriteByte(Long6(v));
            stream.WriteByte(Long5(v));
            stream.WriteByte(Long4(v));
            stream.WriteByte(Long3(v));
            stream.WriteByte(Long2(v));
            stream.WriteByte(Long1(v));
            stream.WriteByte(Long0(v));
        }

        public static void WriteUInt64B(this Stream stream, ulong v)
        {
            WriteInt64B(stream, (long)v);
        }

        private static int SingleToInt32Bits(float f)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(f), 0);
        }

        private static float Int32BitsToSingle(int i)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(i), 0);
        }

        public static float ReadFloatL(this Stream stream)
        {
            return Int32BitsToSingle(ReadInt32L(stream));
        }

        public static float ReadFloatB(this Stream stream)
        {
            return Int32BitsToSingle(ReadInt32B(stream));
        }

        public static void WriteFloatL(this Stream stream, float v)
        {
            WriteInt32L(stream, SingleToInt32Bits(v));
        }

        public static void WriteFloatB(this Stream stream, float v)
        {
            WriteInt32B(stream, SingleToInt32Bits(v));
        }

        public static double ReadDoubleL(this Stream stream)
        {
            return BitConverter.Int64BitsToDouble(ReadInt64L(stream));
        }

        public static double GetDoubleB(this Stream stream)
        {
            return BitConverter.Int64BitsToDouble(ReadInt64B(stream));
        }

        public static void WriteDoubleL(this Stream stream, double v)
        {
            WriteInt64L(stream, BitConverter.DoubleToInt64Bits(v));
        }

        public static void WriteDoubleB(this Stream stream, double v)
        {
            WriteInt64B(stream, BitConverter.DoubleToInt64Bits(v));
        }

        public static byte[] ReadBytes(this Stream stream, int count)
        {
            byte[] array = new byte[count];
            stream.Read(array, 0, count);
            return array;
        }

        public static void WriteBytes(this Stream stream, byte[] buffer)
        {
            stream.Write(buffer, 0, buffer.Length);
        }

        public static void WriteBytes(this Stream stream, byte[] buffer, int offset, int length)
        {
            stream.Write(buffer, offset, length);
        }

        public static string ReadWideString(this Stream stream)
        {
            StringBuilder stringBuilder = new StringBuilder();
            char c = (char)ReadUInt16L(stream);
            while (true)
            {
                switch (c)
                {
                    case '\v':
                        stringBuilder.Append("\\v");
                        break;
                    default:
                        stringBuilder.Append(c);
                        break;
                    case '\0':
                        return stringBuilder.ToString();
                }
                c = (char)ReadUInt16L(stream);
            }
        }

        public static void WriteWideString(this Stream stream, string str, Encoding encoding)
        {
            string text = str.Replace("\\v", "\v");
            if (!text.EndsWith("\0"))
            {
                text += "\0";
            }
            byte[] bytes = encoding.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
        }

        public static string ReadNullTermString(this Stream stream, Encoding encoding)
        {
            int byteCount = encoding.GetByteCount("e");
            using (MemoryStream memoryStream = new MemoryStream())
            {
                while (true)
                {
                    byte[] array = new byte[byteCount];
                    stream.Read(array, 0, byteCount);
                    if (encoding.GetString(array, 0, byteCount) == "\0")
                    {
                        break;
                    }
                    memoryStream.Write(array, 0, array.Length);
                }
                return encoding.GetString(memoryStream.ToArray());
            }
        }

        public static void WriteNullTermString(this Stream stream, string value, Encoding encoding)
        {
            value = (value ?? string.Empty);
            int byteCount = encoding.GetByteCount(value);
            byte[] array = new byte[byteCount + 1];
            encoding.GetBytes(value, 0, value.Length, array, 0);
            array[byteCount] = 0;
            stream.Write(array, 0, array.Length);
        }

        public static async Task<byte[]> ReadAllBytesAsync(string filename)
        {
            byte[] buffer = new byte[4096];
            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream fs = File.OpenRead(filename))
                {
                    int count;
                    while ((count = await fs.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await ms.WriteAsync(buffer, 0, count);
                    }
                }
                return ms.ToArray();
            }
        }

        public static async Task WriteAllBytesAsync(string filename, byte[] bytes)
        {
            using (FileStream fs = File.Open(filename, FileMode.Create))
            {
                await fs.WriteAsync(bytes, 0, bytes.Length);
            }
        }

        public static async Task<string> ReadAllTextAsync(string filename)
        {
            Encoding uTF = Encoding.UTF8;
            return uTF.GetString(await ReadAllBytesAsync(filename).ConfigureAwait(continueOnCapturedContext: false));
        }

    }
    }
