using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Callback
{
    [Serializable]
    public class Buffer : ISerializable
    {
        [NonSerialized]
        MemoryStream stream;
        [NonSerialized]
        BinaryWriter writer;

        int alignment = 1;

        public Buffer()
        {
            Reset();
        }

        public void Reset()
        {
            stream = new MemoryStream();
            writer = new BinaryWriter(stream);
        }

        public byte[] GetBuffer()
        {
            return stream.ToArray();
        }

        public int Length()
        {
            return (int)stream.Length;
        }

        // Lines up with respective pragma(pack, n) statements
        public void SetAlignment(int amount)
        {
            alignment = amount;
        }

        private int GetPaddingForAignment(int align)
        {
            var alignment = (Length() % align);

            if (alignment != 0) alignment = align - alignment;

            // Console.WriteLine("l {0} a {1} r {2}", Length(), align, alignment);

            return alignment;
        }

        private void PadN(int n)
        {
            for (int i = 0; i < n; i++)
            {
                WriteByte(0);
            }
        }

        public void AlignForSize(int size_in_bytes)
        {
            if (alignment == 1) return;
            if (size_in_bytes == 1) return;

            if (alignment == 4)
            {
                if (size_in_bytes == 2)
                {
                    PadN(GetPaddingForAignment(2));
                }

                if (size_in_bytes == 4 || size_in_bytes == 8)
                {
                    PadN(GetPaddingForAignment(4));
                }
            }
            else
            {
                throw new ArgumentException("Unknown alignment");
            }
        }

        public int WriteToPointer(IntPtr ptr, int max_length)
        {
            int write_size = Length();
            if (write_size > max_length) write_size = max_length;

            Marshal.Copy(GetBuffer(), 0, ptr, write_size);
            return write_size;
        }

        public int ReadFromPointer(IntPtr ptr, int max_length)
        {
            byte[] temp = new byte[max_length];
            Marshal.Copy(ptr, temp, 0, max_length);

            Write(temp);
            return max_length;
        }

        public void Write(ulong value) { AlignForSize(sizeof(ulong)); writer.Write(value); }
        public void WriteULong(ulong value) => Write(value);

        public void Write(uint value) { AlignForSize(sizeof(uint)); writer.Write(value); }
        public void WriteUInt(uint value) => Write(value);

        public void Write(ushort value) { AlignForSize(sizeof(ushort)); writer.Write(value); }
        public void WriteUShort(ushort value) => Write(value);

        public void Write(string value) => writer.Write(value);
        public void WriteString(string value) => Write(value);

        public void Write(float value) { AlignForSize(sizeof(float)); writer.Write(value); }
        public void WriteFloat(float value) => Write(value);

        public void Write(sbyte value) { AlignForSize(sizeof(sbyte)); writer.Write(value); }
        public void WriteSByte(sbyte value) => Write(value);

        public void Write(long value) { AlignForSize(sizeof(long)); writer.Write(value); }
        public void WriteLong(long value) => Write(value);

        void Write(int value) { AlignForSize(sizeof(int)); writer.Write(value); }
        public void WriteInt(int value) => Write(value);

        void Write(short value) { AlignForSize(sizeof(short)); writer.Write(value); }
        public void WriteShort(short value) => Write(value);

        void Write(decimal value) { AlignForSize(sizeof(decimal)); writer.Write(value); }
        public void WriteDecimal(decimal value) => Write(value);

        public void Write(char[] chars, int index, int count)
            => writer.Write(chars, index, count);
        public void Write(char[] chars)
            => writer.Write(chars);

        public void Write(byte[] buffer, int index, int count)
            => writer.Write(buffer, index, count);
        public void Write(byte[] buffer)
            => writer.Write(buffer);

        void Write(byte value) { AlignForSize(1); writer.Write(value); }
        public void WriteByte(byte value) => Write(value);

        void Write(bool value) { AlignForSize(sizeof(bool)); writer.Write(value); }
        public void WriteBool(bool value) => Write(value);

        void Write(double value) { AlignForSize(sizeof(double)); writer.Write(value); }
        public void WriteDouble(double value) => Write(value);

        void Write(char value) { AlignForSize(sizeof(char)); writer.Write(value); }
        public void WriteChar(char ch) => Write(ch);

        protected Buffer(SerializationInfo info, StreamingContext context)
        {
            // Get the existing data out of the serialized data
            var existing_data = (byte[])info.GetValue("data", typeof(byte[]));

            // foreach (var b in existing_data)
            // {
            //     Console.Write("{0} ", b);
            // }

            // Console.WriteLine();

            // Rewrite to the stream
            Reset();
            writer.Write(existing_data);
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("data", GetBuffer());
        }
    }
}
