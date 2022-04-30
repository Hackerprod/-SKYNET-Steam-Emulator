using SKYNET.Callback;
using SKYNET.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

        internal static uint GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, IntPtr pcbTicket)
        {
            int STEAM_AUTH_TICKET_SIZE = 234;
            int STEAM_TICKET_MIN_SIZE = (4 + 8 + 8);
            if (cbMaxTicket < STEAM_TICKET_MIN_SIZE) return 0;
            if (cbMaxTicket > STEAM_AUTH_TICKET_SIZE) cbMaxTicket = STEAM_AUTH_TICKET_SIZE;

            try
            {
                CurrentTicket++;

                byte[] Token = new byte[4] { 0x14, 0x00, 0x00, 0x00 };
                byte[] IPAddr = NetworkManager.GetIPAddress().GetAddressBytes();

                MemoryStream ticketStream = new MemoryStream();
                ticketStream.WriteBytes(Token);
                ticketStream.WriteInt32L(0x18); // Header size
                ticketStream.WriteBytes(IPAddr);

                Marshal.Copy(ticketStream.ToArray(), 0, pTicket, ticketStream.ToArray().Length);

                GetAuthSessionTicketResponse_t data = new GetAuthSessionTicketResponse_t()
                {
                    AuthTicket = CurrentTicket,
                    Result = Types.EResult.k_EResultOK
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
            SteamEmulator.Write("Ticket Manager", msg);
        }
    }
}

struct TicketData
{
    public ulong SteamID;
    public uint IPClient;
    public IntPtr AuthBlob;
    public uint BlobSize;
};
