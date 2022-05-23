using SKYNET.Callback;
using SKYNET.Helper;
using SKYNET.Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Managers
{
    public class TicketManager
    {
        private static List<TicketData> StoredTickets;

        private static uint CurrentTicket;

        static TicketManager()
        {
            StoredTickets = new List<TicketData>();
            CurrentTicket = 0;
        }

        public static bool ConnectAndAuthenticate(uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, ulong pSteamIDUser)
        {
            TicketData data = new TicketData()
            {
                IPClient = unIPClient,
                AuthBlob = pvAuthBlob,
                BlobSize = cubAuthBlobSize,
                SteamID = pSteamIDUser
            };
            StoredTickets.Add(data);
            return true;
        }

        public static void RemoveTicket(ulong steamIDUser)
        {
            for (int i = 0; i < StoredTickets.Count(); i++)
            {
                TicketData data = StoredTickets[i];
                if (data.SteamID == steamIDUser)
                {
                    StoredTickets.RemoveAt(i);
                    break;
                }
            }
        }

        internal static uint GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
        {
            try
            {
                CurrentTicket++;

                //return CreateTicket(pTicket, cbMaxTicket, ref pcbTicket); // Old way
                //return GoldbergWay(pTicket, cbMaxTicket, ref pcbTicket);
                //return ArgonWay(pTicket, cbMaxTicket, ref pcbTicket);

                ulong steam_id = (ulong)SteamEmulator.SteamID;

                MemoryStream stream = new MemoryStream();
                stream.WriteByte(0x14);
                stream.WriteByte(0);
                stream.WriteByte(0);
                stream.WriteByte(0);
                stream.WriteUInt64L(steam_id);

                byte[] Ticket = new byte[cbMaxTicket];
                Ticket[0] = 0x14;
                Ticket[1] = 0;
                Ticket[2] = 0;
                Ticket[3] = 0;

                //Offset 8
                Ticket[9] = 0x14;
                Ticket[10] = 0;
                Ticket[11] = 0;
                Ticket[12] = 0;

                //Offset 12
                Ticket[13] = (byte)steam_id;
                Ticket[14] = (byte)steam_id;
                Ticket[15] = (byte)steam_id;
                Ticket[16] = (byte)steam_id;
                Ticket[17] = (byte)steam_id;
                Ticket[18] = (byte)steam_id;
                Ticket[19] = (byte)steam_id;
                Ticket[20] = (byte)steam_id;

                Marshal.Copy(Ticket, 0, pTicket, Ticket.Length);
                pcbTicket = (uint)Ticket.Length;
                CreateCallback();
                return CurrentTicket;


                byte[] Token = new byte[] { 0x14, 0x00, 0x00, 0x00 };
                Marshal.Copy(Token, 0, pTicket, Token.Length);

                IntPtr TicketOffset = pTicket + 4;

                Ticket ticket = new Ticket()
                {
                    //Header = SteamEmulator.AppID, //new byte[] { 0x14, 0x00, 0x00, 0x00 },
                    Handle = CurrentTicket,
                    TicketID = CSteamID.CreateOne().AccountID,
                    UserSteamID = (ulong)SteamEmulator.SteamID,
                };

                int size = Marshal.SizeOf(Ticket);
                pcbTicket = (uint)size;
                Marshal.StructureToPtr(Ticket, TicketOffset, false);

                GetAuthSessionTicketResponse_t data = new GetAuthSessionTicketResponse_t()
                {
                    AuthTicket = CurrentTicket,
                    Result = EResult.k_EResultOK
                };

                CallbackManager.AddCallbackResult(data);
            }
            catch (Exception ex)
            {
                Write($"Error creating auth Session Ticket {ex}");
            }
            
            return CurrentTicket;
        }

        private static uint GoldbergWay(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
        {
            int STEAM_AUTH_TICKET_SIZE = 234;
            int STEAM_TICKET_MIN_SIZE = (4 + 8 + 8);
            if (cbMaxTicket < STEAM_TICKET_MIN_SIZE) return 0;
            if (cbMaxTicket > STEAM_AUTH_TICKET_SIZE) cbMaxTicket = STEAM_AUTH_TICKET_SIZE;

            try
            {
                ulong steam_id = (ulong)SteamEmulator.SteamID;
                byte[] first4 = pTicket.GetBytes(1024);
                first4[0] = 0x14;
                first4[1] = 0;
                first4[2] = 0;
                first4[3] = 0;
                Marshal.Copy(first4, 0, pTicket, 4);
                Marshal.WriteInt64(pTicket, 12, (long)steam_id);
                Marshal.WriteInt32(pcbTicket, 0, cbMaxTicket);
                Marshal.WriteInt32(pTicket, sizeof(UInt64), (int)CurrentTicket);

                CreateCallback();
            }
            catch (Exception ex)
            {
                Write($"Error creating auth Session Ticket {ex}");
            }

            return CurrentTicket;
        }

        private static void CreateCallback()
        {
            GetAuthSessionTicketResponse_t data = new GetAuthSessionTicketResponse_t()
            {
                AuthTicket = CurrentTicket,
                Result = EResult.k_EResultOK
            };
            CallbackManager.AddCallbackResult(data);
        }

        private static uint ArgonWay(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
        {
            IPAddress public_ip = NetworkManager.GetIPAddress();
            var client_ticket_buffer = new Buffer2();

            // Write the token into the ticket
            var token = new byte[] { 0x14, 0x00, 0x00, 0x00 }; 
            client_ticket_buffer.Write(token.Length);
            client_ticket_buffer.Write(token);

            // Size of header
            client_ticket_buffer.Write(0x18);

            // This is all copied from what the steamclient method does
            client_ticket_buffer.Write(1);
            client_ticket_buffer.Write(2);

            var ip_bytes = public_ip.GetAddressBytes();
            Array.Reverse(ip_bytes);
            client_ticket_buffer.Write(ip_bytes);

            client_ticket_buffer.Write(public_ip.GetAddressBytes());
            client_ticket_buffer.Write(Platform.MilisecondTime());
            client_ticket_buffer.Write(CurrentTicket);

            //var client_ticket_crc = BitConverter.ToUInt32(CryptoHelper.CRCHash(client_ticket_buffer.GetBuffer()), 0);

            //auth_ticket_store.Add(new AuthTicket()
            //{
            //    app_id = app_id,
            //    pipe_id = pipe,
            //    crc32 = client_ticket_crc,
            //    handle = auth_ticket_store.Count + 1,
            //    ticket = client_ticket_buffer.GetBuffer(),
            //    cancelled = false,
            //});

            byte[] Buffer = client_ticket_buffer.GetBuffer();

            pcbTicket = (uint)Buffer.Length;
            Marshal.Copy(Buffer, 0, pTicket, Buffer.Length);

            CreateCallback();

            return CurrentTicket;
        }

        private static uint CreateTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
        {
            byte[] Token = new byte[] { 0x14, 0x00, 0x00, 0x00 };
            byte[] IPAddr = NetworkManager.GetIPAddress().GetAddressBytes();

            MemoryStream ticketStream = new MemoryStream();
            ticketStream.WriteBytes(Token);                 // Ticket token
            ticketStream.WriteInt32L(0x18);                 // Header size
            ticketStream.WriteBytes(IPAddr);                // IP address    

            byte[] Buffer = ticketStream.ToArray();
            pcbTicket = (uint)Buffer.Length;
            Marshal.Copy(Buffer, 0, pTicket, Buffer.Length);
            Marshal.WriteInt64(pTicket, 12, (long)(ulong)SteamEmulator.SteamID);

            CreateCallback();

            return CurrentTicket;
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("Ticket Manager", msg);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public class Ticket
    {
        //[FieldOffset(0)]
        //public uint Header;

        [FieldOffset(0)]        // 4
        public uint Handle;

        [FieldOffset(4)]        // 8
        public uint TicketID;

        [FieldOffset(8)]       // 12
        public ulong UserSteamID;
    }
}

[Serializable]
public class Buffer2 : ISerializable
{
    [NonSerialized]
    MemoryStream stream;
    [NonSerialized]
    BinaryWriter writer;

    int alignment = 1;

    public Buffer2()
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

    protected Buffer2(SerializationInfo info, StreamingContext context)
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

        // foreach (var b in GetBuffer())
        // {
        //     Console.Write("{0} ", b);
        // }

        // Console.WriteLine();
    }
}
