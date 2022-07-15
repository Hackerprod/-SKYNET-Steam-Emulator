using SKYNET.Callback;
using SKYNET.Helpers;
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

                Ticket ticket = new Ticket()
                {
                    AppID = SteamEmulator.AppID,
                    Handle = CurrentTicket,
                    TicketID = CSteamID.CreateOne().AccountID,
                    UserSteamID = (ulong)SteamEmulator.SteamID,
                };

                int size = Marshal.SizeOf(ticket);
                pcbTicket = (uint)size;
                Marshal.StructureToPtr(ticket, pTicket, false);

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

        private static void Write(string msg)
        {
            SteamEmulator.Write("TicketManager", msg);
        }

        [StructLayout(LayoutKind.Sequential)]
        private class Ticket
        {
            public uint AppID;
            public uint Handle;
            public uint TicketID;
            public ulong UserSteamID;
        }
    }
}
