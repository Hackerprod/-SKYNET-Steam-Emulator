using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SKYNET.Managers
{
    public static class TicketManager
    {
        private static readonly ConcurrentDictionary<uint, TicketRecord> Tickets = new ConcurrentDictionary<uint, TicketRecord>();
        private static readonly ConcurrentDictionary<ulong, TicketData> ConnectedUsers = new ConcurrentDictionary<ulong, TicketData>();
        private static uint CurrentTicket;

        public static bool ConnectAndAuthenticate(uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, ulong pSteamIDUser)
        {
            var authBlob = pvAuthBlob == IntPtr.Zero || cubAuthBlobSize == 0
                ? new byte[0]
                : pvAuthBlob.GetBytes(cubAuthBlobSize);

            // Approve the connecting user locally and immediately (Goldberg-style).
            // Gating this on a blocking server round-trip stalls Dota's client
            // connection handshake, which makes the joining player fail VAC/session
            // verification. On a trusted LAN every SKYNET user is authorised, so we
            // accept here and only notify the server asynchronously for presence.
            ConnectedUsers[pSteamIDUser] = new TicketData
            {
                IPClient = unIPClient,
                AuthBlob = pvAuthBlob,
                BlobSize = cubAuthBlobSize,
                SteamID = pSteamIDUser
            };

            CallbackManager.AddCallbackGameServer(new GSClientApprove_t
            {
                SteamID = pSteamIDUser,
                OwnerSteamID = pSteamIDUser
            });

            if (SkyNetApiClient.IsEnabled)
            {
                SkyNetWorkQueue.Enqueue("ConnectAndAuthenticate", () => SkyNetApiClient.ConnectAndAuthenticate(unIPClient, authBlob, pSteamIDUser),
                    "ticket:connect:" + pSteamIDUser, true);
            }

            return true;
        }

        public static void RemoveTicket(ulong steamIDUser)
        {
            ConnectedUsers.TryRemove(steamIDUser, out _);
        }

        public static uint GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, out uint pcbTicket, bool gameServer = false)
        {
            pcbTicket = 0;

            try
            {
                // The ticket must be produced immediately: the joining client sends it
                // during the connection handshake, and a blocking server round-trip
                // here delays it enough that Dota fails the session (VAC). Generate it
                // locally (Goldberg-style) and register it with the server in the
                // background so cross-user tracking still works.
                if (SkyNetApiClient.IsEnabled)
                {
                    SkyNetWorkQueue.Enqueue("CreateAuthSessionTicket", () => SkyNetApiClient.CreateAuthSessionTicket(gameServer, cbMaxTicket),
                        null, true);
                }

                CurrentTicket++;

                var ticket = new LocalTicket
                {
                    AppID = SteamEmulator.AppID,
                    Handle = CurrentTicket,
                    TicketID = CSteamID.CreateOne().AccountID,
                    UserSteamID = (ulong)(gameServer ? SteamEmulator.SteamID_GS : SteamEmulator.SteamID),
                };

                var size = Marshal.SizeOf(ticket);
                pcbTicket = (uint)size;

                if (pTicket != IntPtr.Zero && cbMaxTicket >= size)
                {
                    Marshal.StructureToPtr(ticket, pTicket, false);
                }

                Tickets[CurrentTicket] = new TicketRecord
                {
                    Handle = CurrentTicket,
                    Bytes = StructureToBytes(ticket),
                    GameServer = gameServer
                };

                EmitAuthTicketResponse(CurrentTicket, gameServer);
                return CurrentTicket;
            }
            catch (Exception ex)
            {
                Write($"Error creating auth session ticket {ex}");
                return 0;
            }
        }

        public static int BeginAuthSession(IntPtr pAuthTicket, int cbAuthTicket, ulong steamID, bool gameServer = false)
        {
            var ticketBytes = pAuthTicket == IntPtr.Zero || cbAuthTicket <= 0
                ? new byte[0]
                : pAuthTicket.GetBytes((uint)cbAuthTicket);

            // Validate locally and immediately (Goldberg-style). A blocking server
            // round-trip here delays the ValidateAuthTicketResponse callback past the
            // point Dota expects it, so the session is treated as unverified (VAC).
            // The server is still notified asynchronously so it can track the session.
            if (SkyNetApiClient.IsEnabled)
            {
                SkyNetWorkQueue.Enqueue("ValidateAuthSessionTicket", () => SkyNetApiClient.ValidateAuthSessionTicket(ticketBytes, steamID, gameServer),
                    null, true);
            }

            EmitValidateAuthTicket(steamID, steamID, EAuthSessionResponse.OK, gameServer);
            return (int)EBeginAuthSessionResult.k_EBeginAuthSessionResultOK;
        }

        public static void EndAuthSession(ulong steamID, bool gameServer = false)
        {
            if (SkyNetApiClient.IsEnabled)
            {
                SkyNetWorkQueue.Enqueue("EndAuthSession", () => SkyNetApiClient.EndAuthSession(steamID, gameServer),
                    "ticket:end:" + steamID + ":" + (gameServer ? "gs" : "client"));
            }

            ConnectedUsers.TryRemove(steamID, out _);
        }

        public static void CancelAuthTicket(uint hAuthTicket, bool gameServer = false)
        {
            if (SkyNetApiClient.IsEnabled)
            {
                SkyNetWorkQueue.Enqueue("CancelAuthTicket", () => SkyNetApiClient.CancelAuthTicket(hAuthTicket, gameServer),
                    "ticket:cancel:" + hAuthTicket + ":" + (gameServer ? "gs" : "client"));
            }

            Tickets.TryRemove(hAuthTicket, out _);
        }

        private static void EmitAuthTicketResponse(uint handle, bool gameServer)
        {
            var callback = new GetAuthSessionTicketResponse_t
            {
                AuthTicket = handle,
                Result = EResult.k_EResultOK
            };

            if (gameServer)
            {
                CallbackManager.AddCallbackGameServer(callback);
            }
            else
            {
                CallbackManager.AddCallback(callback);
            }
        }

        private static void EmitValidateAuthTicket(ulong steamId, ulong ownerSteamId, EAuthSessionResponse response, bool gameServer)
        {
            var callback = new ValidateAuthTicketResponse_t
            {
                m_SteamID = steamId,
                m_OwnerSteamID = ownerSteamId,
                m_eAuthSessionResponse = response
            };

            if (gameServer)
            {
                CallbackManager.AddCallbackGameServer(callback);
            }
            else
            {
                CallbackManager.AddCallback(callback);
            }
        }

        private static byte[] StructureToBytes(LocalTicket ticket)
        {
            var size = Marshal.SizeOf(ticket);
            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(ticket, ptr, false);
                var data = new byte[size];
                Marshal.Copy(ptr, data, 0, size);
                return data;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        private static byte[] EncodeFixed(string text, int size)
        {
            var bytes = new byte[size];
            if (string.IsNullOrWhiteSpace(text))
            {
                return bytes;
            }

            var encoded = System.Text.Encoding.UTF8.GetBytes(text);
            Array.Copy(encoded, bytes, Math.Min(bytes.Length - 1, encoded.Length));
            return bytes;
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("TicketManager", msg);
        }

        private sealed class TicketRecord
        {
            public uint Handle { get; set; }
            public byte[] Bytes { get; set; }
            public bool GameServer { get; set; }
        }

        private sealed class TicketData
        {
            public uint IPClient { get; set; }
            public IntPtr AuthBlob { get; set; }
            public uint BlobSize { get; set; }
            public ulong SteamID { get; set; }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LocalTicket
        {
            public uint AppID;
            public uint Handle;
            public uint TicketID;
            public ulong UserSteamID;
        }
    }
}
