using SKYNET.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

using HAuthTicket = System.UInt32;

namespace SKYNET.Managers
{
    public class TicketManager
    {
        private static Dictionary<uint, byte[]> ownership_ticket_store = new Dictionary<uint, byte[]>();

        private static List<AuthTicket> StoredTickets;

        private static uint CurrentTicket;
        private static List<AuthTicket> auth_ticket_store;
        public static uint ticket_request_count = 0;
        public static uint auth_sequence = 0;


        static TicketManager()
        {
            auth_ticket_store = new List<AuthTicket>();
            CurrentTicket = 0;
        }

        public static bool ConnectAndAuthenticate(uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, ulong pSteamIDUser)
        {
            AuthTicket data = new AuthTicket()
            {
                //IPClient = unIPClient,
                //AuthBlob = pvAuthBlob,
                //BlobSize = cubAuthBlobSize,
                SteamID = pSteamIDUser
            };
            StoredTickets.Add(data);
            return true;
        }

        public static void RemoveTicket(ulong steamIDUser)
        {
            for (int i = 0; i < StoredTickets.Count(); i++)
            {
                AuthTicket data = StoredTickets[i];
                if (data.SteamID == steamIDUser)
                {
                    StoredTickets.RemoveAt(i);
                    break;
                }
            }
        }

        internal static HAuthTicket GetAuthSessionTicket(IntPtr pTicket, ref int cbMaxTicket, ref uint pcbTicket, bool Server = false)
        {
            CurrentTicket++;
            var TicketBuffer = new SKYNET.Helper.Buffer();
            var ownership_ticket = GetAppOwnershipTicket(SteamEmulator.AppId);

            // Write the token into the ticket
            var token = new byte[] { 0x04, 0xF4 }; // For my
            TicketBuffer.Write(token.Length);
            TicketBuffer.Write(token);

            // Size of header
            TicketBuffer.Write(0x18);

            // This is all copied from what the steamclient method does
            TicketBuffer.Write(1);
            TicketBuffer.Write(2);

            var ip_bytes = NetworkManager.GetIPAddress().GetAddressBytes();
            Array.Reverse(ip_bytes);
            TicketBuffer.Write(ip_bytes);

            TicketBuffer.Write(NetworkManager.GetIPAddress().GetAddressBytes());
            TicketBuffer.Write(modCommon.MilisecondTime());
            TicketBuffer.Write(++ticket_request_count);

            var client_ticket_crc = BitConverter.ToUInt32(CRCHash(TicketBuffer.GetBuffer()), 0);
            var clientTicket = new AuthTicket()
            {
                AppID = SteamEmulator.AppId,
                Crc32 = client_ticket_crc,
                Handle = CurrentTicket,
                Ticket = TicketBuffer.GetBuffer(),
                cancelled = false,
            };
            auth_ticket_store.Add(clientTicket);

            // Create the ticket that will actually be sent to the server

            CurrentTicket++;

            var ServerTicket = new SKYNET.Helper.Buffer();

            var size = 8 + TicketBuffer.GetBuffer().Length + 4 + ownership_ticket.Length;

            ServerTicket.Write((ushort)size);

            ServerTicket.Write((ulong)SteamEmulator.SteamId);

            ServerTicket.Write(TicketBuffer.GetBuffer());

            // Write the ownership ticket data in here
            // We are just going to assume that our tickets are 100% correct...
            ServerTicket.Write(ownership_ticket.Length);
            ServerTicket.Write(ownership_ticket);

            var server_ticket_crc = BitConverter.ToUInt32(CRCHash(ServerTicket.GetBuffer()), 0);
            var serverTicket = new AuthTicket()
            {
                is_server_ticket = true,
                AppID = SteamEmulator.AppId,
                Crc32 = server_ticket_crc,
                Handle = CurrentTicket,
                Ticket = ServerTicket.GetBuffer(),
                cancelled = false,
            };
            auth_ticket_store.Add(serverTicket);

            if (Server)
            {
                pcbTicket = (uint)clientTicket.Ticket.Length;
                Marshal.Copy(clientTicket.Ticket, 0, pTicket, clientTicket.Ticket.Length);
                return clientTicket.Handle;
            }
            else
            {
                pcbTicket = (uint)serverTicket.Ticket.Length;
                Marshal.Copy(serverTicket.Ticket, 0, pTicket, serverTicket.Ticket.Length);
                return serverTicket.Handle;
            }

        }

        private static void Write(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            SteamEmulator.Write("Ticket Manager", msg);
        }

        public static byte[] GetAppOwnershipTicket(uint app_id)
        {
            if (ownership_ticket_store.TryGetValue(app_id, out var result))
            {
                return result;
            }
            else
            {
                return ownership_ticket_store[app_id];
            }
        }
        public static byte[] CRCHash(byte[] input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            using (var crc = new Crc32())
            {
                byte[] hash = crc.ComputeHash(input);
                Array.Reverse(hash);

                return hash;
            }
        }
    }
}

public class AuthTicket
{
    public bool is_server_ticket = false;
    public uint AppID;
    public uint Crc32;
    public HAuthTicket Handle;
    public byte[] Ticket;
    public bool cancelled = false;
    public ulong SteamID;
}
