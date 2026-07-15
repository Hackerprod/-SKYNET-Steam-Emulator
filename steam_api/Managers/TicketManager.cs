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
        private const uint SteamAppTicketGcLength = 20;
        private const int SteamTicketMinSize = 4 + 8 + 8 + 4;
        private static uint CurrentTicket;

        public static bool ConnectAndAuthenticate(uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, IntPtr pSteamIDUser)
        {
            var approved = ConnectAndAuthenticate(unIPClient, pvAuthBlob, cubAuthBlobSize, out var steamIDUser);
            if (approved && pSteamIDUser != IntPtr.Zero)
            {
                Marshal.WriteInt64(pSteamIDUser, unchecked((long)steamIDUser));
            }

            return approved;
        }

        public static bool ConnectAndAuthenticate(uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, out ulong steamIDUser)
        {
            var authBlob = pvAuthBlob == IntPtr.Zero || cubAuthBlobSize == 0
                ? new byte[0]
                : pvAuthBlob.GetBytes(cubAuthBlobSize);

            steamIDUser = TryReadSteamIdFromAuthBlob(authBlob);
            if (steamIDUser == 0)
            {
                steamIDUser = SteamEmulator.SteamID.SteamID;
                Write($"ConnectAndAuthenticate auth blob did not contain a SteamID; falling back to local user {(CSteamID)steamIDUser}");
            }

            // Approve the connecting user locally and immediately (Goldberg-style).
            // Gating this on a blocking server round-trip stalls Dota's client
            // connection handshake, which makes the joining player fail VAC/session
            // verification. On a trusted LAN every SKYNET user is authorised, so we
            // accept here and only notify the server asynchronously for presence.
            ConnectedUsers[steamIDUser] = new TicketData
            {
                IPClient = unIPClient,
                AuthBlob = pvAuthBlob,
                BlobSize = cubAuthBlobSize,
                SteamID = steamIDUser
            };

            CallbackManager.AddCallbackGameServer(new GSClientApprove_t
            {
                SteamID = steamIDUser,
                OwnerSteamID = steamIDUser
            });

            var approvedSteamID = steamIDUser;
            if (SkyNetApiClient.IsEnabled)
            {
                WorkQueue.Enqueue("ConnectAndAuthenticate", () => SkyNetApiClient.ConnectAndAuthenticate(unIPClient, authBlob, approvedSteamID),
                    "ticket:connect:" + approvedSteamID, true);
            }

            Write($"ConnectAndAuthenticate approved {(CSteamID)steamIDUser} blob={cubAuthBlobSize}");
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
                    WorkQueue.Enqueue("CreateAuthSessionTicket", () => SkyNetApiClient.CreateAuthSessionTicket(gameServer, cbMaxTicket),
                        null, true);
                }

                CurrentTicket++;
                if (CurrentTicket == 0)
                {
                    CurrentTicket++;
                }

                var ticket = new LocalTicket
                {
                    GcTokenLength = SteamAppTicketGcLength,
                    Token = ((ulong)CurrentTicket << 32) | (uint)Environment.TickCount,
                    UserSteamID = (ulong)(gameServer ? SteamEmulator.SteamID_GS : SteamEmulator.SteamID),
                    TicketGeneratedDate = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                var size = Marshal.SizeOf(ticket);
                if (pTicket == IntPtr.Zero || cbMaxTicket < size)
                {
                    Write($"GetAuthSessionTicket buffer too small cbMax={cbMaxTicket} needed={size}");
                    pcbTicket = 0;
                    return 0;
                }

                pcbTicket = (uint)size;
                Marshal.StructureToPtr(ticket, pTicket, false);

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
            var ticketSteamId = TryReadSteamIdFromAuthBlob(ticketBytes);
            if (ticketSteamId != 0 && steamID != 0 && ticketSteamId != steamID)
            {
                Write($"BeginAuthSession rejected mismatched ticket steam={(CSteamID)ticketSteamId} expected={(CSteamID)steamID} size={cbAuthTicket}");
                EmitValidateAuthTicket(steamID, ticketSteamId, EAuthSessionResponse.AuthTicketInvalid, gameServer);
                return (int)EBeginAuthSessionResult.k_EBeginAuthSessionResultInvalidTicket;
            }

            var ownerSteamId = ticketSteamId != 0 ? ticketSteamId : steamID;

            // Validate locally and immediately (Goldberg-style). A blocking server
            // round-trip here delays the ValidateAuthTicketResponse callback past the
            // point Dota expects it, so the session is treated as unverified (VAC).
            // The server is still notified asynchronously so it can track the session.
            if (SkyNetApiClient.IsEnabled)
            {
                WorkQueue.Enqueue("ValidateAuthSessionTicket", () => SkyNetApiClient.ValidateAuthSessionTicket(ticketBytes, steamID, gameServer),
                    null, true);
            }

            EmitValidateAuthTicket(steamID, ownerSteamId, EAuthSessionResponse.OK, gameServer);
            Write($"BeginAuthSession approved {(CSteamID)steamID} owner={(CSteamID)ownerSteamId} size={cbAuthTicket}");
            return (int)EBeginAuthSessionResult.k_EBeginAuthSessionResultOK;
        }

        public static void EndAuthSession(ulong steamID, bool gameServer = false)
        {
            if (SkyNetApiClient.IsEnabled)
            {
                WorkQueue.Enqueue("EndAuthSession", () => SkyNetApiClient.EndAuthSession(steamID, gameServer),
                    "ticket:end:" + steamID + ":" + (gameServer ? "gs" : "client"));
            }

            ConnectedUsers.TryRemove(steamID, out _);
        }

        public static void CancelAuthTicket(uint hAuthTicket, bool gameServer = false)
        {
            if (SkyNetApiClient.IsEnabled)
            {
                WorkQueue.Enqueue("CancelAuthTicket", () => SkyNetApiClient.CancelAuthTicket(hAuthTicket, gameServer),
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

        private static ulong TryReadSteamIdFromAuthBlob(byte[] authBlob)
        {
            if (authBlob == null || authBlob.Length < SteamTicketMinSize)
            {
                return 0;
            }

            var magic = BitConverter.ToUInt32(authBlob, 0);
            if (magic == SteamAppTicketGcLength)
            {
                return BitConverter.ToUInt64(authBlob, 12);
            }

            // SmartSteamEmu ticket: "HEMU", version at +4, SteamID at +0x10.
            if (magic == 0x554D4548 && authBlob.Length >= 0x18)
            {
                return BitConverter.ToUInt64(authBlob, 0x10);
            }

            // RevEmu ticket marker at +8 ("rev"), SteamID at +0x10.
            if (authBlob.Length >= 0x18 && BitConverter.ToUInt32(authBlob, 0x08) == 0x00726576)
            {
                return BitConverter.ToUInt64(authBlob, 0x10);
            }

            return 0;
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

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct LocalTicket
        {
            public uint GcTokenLength;
            public ulong Token;
            public ulong UserSteamID;
            public uint TicketGeneratedDate;
        }
    }
}
