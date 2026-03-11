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

            if (SkyNetApiClient.IsEnabled)
            {
                var result = SkyNetApiClient.ConnectAndAuthenticate(unIPClient, authBlob, pSteamIDUser);
                if (result == null)
                {
                    return false;
                }

                if (result.Success)
                {
                    ConnectedUsers[pSteamIDUser] = new TicketData
                    {
                        IPClient = unIPClient,
                        BlobSize = cubAuthBlobSize,
                        SteamID = pSteamIDUser
                    };

                    CallbackManager.AddCallbackGameServer(new GSClientApprove_t
                    {
                        SteamID = result.SteamId != 0 ? result.SteamId : pSteamIDUser,
                        OwnerSteamID = result.OwnerSteamId != 0 ? result.OwnerSteamId : pSteamIDUser
                    });
                }
                else
                {
                    CallbackManager.AddCallbackGameServer(new GSClientDeny_t
                    {
                        SteamID = pSteamIDUser,
                        DenyReason = (DenyReason)result.DenyReason,
                        OptionalText = EncodeFixed(result.DenyMessage, 128)
                    });
                }

                return result.Success;
            }

            ConnectedUsers[pSteamIDUser] = new TicketData
            {
                IPClient = unIPClient,
                AuthBlob = pvAuthBlob,
                BlobSize = cubAuthBlobSize,
                SteamID = pSteamIDUser
            };
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
                if (SkyNetApiClient.IsEnabled)
                {
                    var response = SkyNetApiClient.CreateAuthSessionTicket(gameServer, cbMaxTicket);
                    if (response == null)
                    {
                        return 0;
                    }

                    var ticketBytes = string.IsNullOrWhiteSpace(response.TicketBase64)
                        ? new byte[0]
                        : Convert.FromBase64String(response.TicketBase64);

                    pcbTicket = response.TicketSize != 0 ? response.TicketSize : (uint)ticketBytes.Length;

                    if (pTicket != IntPtr.Zero && cbMaxTicket > 0 && ticketBytes.Length > 0)
                    {
                        Marshal.Copy(ticketBytes, 0, pTicket, Math.Min(cbMaxTicket, ticketBytes.Length));
                    }

                    Tickets[response.Handle] = new TicketRecord
                    {
                        Handle = response.Handle,
                        Bytes = ticketBytes,
                        GameServer = gameServer
                    };

                    EmitAuthTicketResponse(response.Handle, gameServer);
                    return response.Handle;
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

            if (SkyNetApiClient.IsEnabled)
            {
                var response = SkyNetApiClient.ValidateAuthSessionTicket(ticketBytes, steamID, gameServer);
                if (response == null)
                {
                    EmitValidateAuthTicket(steamID, steamID, EAuthSessionResponse.AuthTicketInvalid, gameServer);
                    return (int)EBeginAuthSessionResult.k_EBeginAuthSessionResultInvalidTicket;
                }

                EmitValidateAuthTicket(steamID, response.OwnerSteamId != 0 ? response.OwnerSteamId : steamID, (EAuthSessionResponse)response.AuthSessionResponse, gameServer);
                return response.BeginAuthSessionResult;
            }

            EmitValidateAuthTicket(steamID, steamID, EAuthSessionResponse.OK, gameServer);
            return (int)EBeginAuthSessionResult.k_EBeginAuthSessionResultOK;
        }

        public static void EndAuthSession(ulong steamID, bool gameServer = false)
        {
            if (SkyNetApiClient.IsEnabled)
            {
                SkyNetApiClient.EndAuthSession(steamID, gameServer);
            }

            ConnectedUsers.TryRemove(steamID, out _);
        }

        public static void CancelAuthTicket(uint hAuthTicket, bool gameServer = false)
        {
            if (SkyNetApiClient.IsEnabled)
            {
                SkyNetApiClient.CancelAuthTicket(hAuthTicket, gameServer);
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
