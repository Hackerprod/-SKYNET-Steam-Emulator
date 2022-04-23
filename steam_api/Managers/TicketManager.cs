using SKYNET.Callback;
using SKYNET.Helper;
using System;
using System.Collections.Generic;
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
                ulong steam_id = (ulong)SteamEmulator.SteamId;
                byte[] first4 = pTicket.GetBytes(4);
                first4[0] = 0x14;
                first4[1] = 0;
                first4[2] = 0;
                first4[3] = 0;
                Marshal.Copy(first4, 0, pTicket, 4);
                Marshal.WriteInt64(pTicket, 12, (long)steam_id);
                Marshal.WriteInt32(pcbTicket, 0, cbMaxTicket);
                Marshal.WriteInt32(pTicket, sizeof(UInt64), (int)CurrentTicket);

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
