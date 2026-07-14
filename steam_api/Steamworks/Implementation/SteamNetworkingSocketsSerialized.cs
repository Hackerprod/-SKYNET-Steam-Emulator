using System;
using SKYNET.Steamworks.Interfaces;
using System.Collections.Generic;
using System.Text;
using SKYNET.Callback;
using SKYNET.Managers;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamNetworkingSocketsSerialized : ISteamInterface
    {
        public static SteamNetworkingSocketsSerialized Instance;

        // ---------------------------------------------------------------------
        // SDR SECURE-CERT MODE TOGGLE
        // ---------------------------------------------------------------------
        // false (default, INSECURE LAN):
        //     GetCertAsync succeeds with an empty certificate, matching the
        //     Goldberg LAN behaviour. The native SDR CA is not patched and the
        //     SKYNET server is never contacted for a certificate.
        //
        // true (SECURE MODE):
        //     GetCertAsync returns the signed certificate issued by the
        //     SKYNET server. Native SDR CA patching runs during emulator
        //     initialization and is retried in memory from networking calls.
        //
        // Configure this with [Network Settings] SecureNetworking in the client
        // INI. A game restart is required because the native networking library
        // caches authentication state for the lifetime of the process.
        // ---------------------------------------------------------------------
        internal static bool SecureCertMode => SteamEmulator.SecureNetworking;
        private static readonly object CertCacheLock = new object();
        private static readonly TimeSpan CertCacheTtl = TimeSpan.FromMinutes(5);
        private static SteamNetworkingSocketsCert_t CachedCert;
        private static DateTime CachedCertUtc;
        private static int SecureCertPatcherStartLogged;

        public SteamNetworkingSocketsSerialized()
        {
            Instance = this;
            InterfaceName = "SteamNetworkingSocketsSerialized";
            InterfaceVersion = "SteamNetworkingSocketsSerialized005";
        }

        public void SendP2PRendezvous(ulong steamIDRemote, uint unConnectionIDSrc, IntPtr pMsgRendezvous, uint cbRendezvous)
        {
            Write("SendP2PRendezvous");
        }

        public void SendP2PConnectionFailure(ulong steamIDRemote, uint unConnectionIDDest, uint nReason, string pszReason)
        {
            Write("SendP2PConnectionFailure");
        }

        public SteamAPICall_t GetCertAsync()
        {
            if (!SecureCertMode)
            {
                Write("GetCertAsync: insecure LAN mode, returning successful empty certificate");
                QueueCurrentNetworkStatusCallbacks();
                return CallbackManager.AddCallbackResult(CreateEmptyCertResult(EResult.k_EResultOK));
            }

            Write("GetCertAsync: secure SDR request queued");

            EnsureSecureCertPatcherStarted("GetCertAsync", 750);

            return WorkQueue.EnqueueCallbackResult(
                CreateEmptyCertResult(EResult.k_EResultNoConnection),
                () => BuildCertResult(),
                false,
                "GetCertAsync",
                "networking:get-cert",
                true);
        }

        private static SteamNetworkingSocketsCert_t CreateEmptyCertResult(EResult result)
        {
            return new SteamNetworkingSocketsCert_t
            {
                m_eResult = result,
                m_cbCert = 0,
                m_certOrMsg = new byte[512],
                m_caKeyID = 0,
                m_cbSignature = 0,
                m_signature = new byte[128],
                m_cbPrivKey = 0,
                m_privKey = new byte[128]
            };
        }

        private SteamNetworkingSocketsCert_t BuildCertResult()
        {
            var empty = CreateEmptyCertResult(EResult.k_EResultNoConnection);

            // Guard against the mode changing while the secure worker is queued.
            if (!SecureCertMode)
            {
                Write("GetCertAsync: switched to insecure LAN mode, returning successful empty certificate");
                return CreateEmptyCertResult(EResult.k_EResultOK);
            }

            if (!SkyNetApiClient.IsEnabled)
            {
                return empty;
            }

            if (TryGetCachedCert(out var cached))
            {
                Write("GetCertAsync: signed cert served from cache");
                return cached;
            }

            try
            {
                var dto = SkyNetApiClient.RequestSdrCert();
                if (dto == null)
                {
                    Write("GetCertAsync: server returned no cert");
                    return empty;
                }

                byte[] certBytes = FromBase64(dto.CertBase64);
                byte[] signatureBytes = FromBase64(dto.SignatureBase64);
                byte[] privKeyBytes = FromBase64(dto.PrivateKeyBase64);
                byte[] publicKeyBytes = FromBase64(dto.PublicKeyBase64);

                if (certBytes.Length == 0 || certBytes.Length > 512 ||
                    signatureBytes.Length != 64 || privKeyBytes.Length != 32)
                {
                    Write($"GetCertAsync: cert sizes invalid (cert={certBytes.Length}, sig={signatureBytes.Length}, key={privKeyBytes.Length})");
                    return empty;
                }

                ulong expectedSteamId = GetExpectedSdrSteamId();
                uint expectedAppId = GetExpectedSdrAppId();
                if (!TryValidateInnerCert(certBytes, publicKeyBytes, expectedSteamId, expectedAppId, out var validationError))
                {
                    Write($"GetCertAsync: cert protobuf invalid ({validationError})");
                    return empty;
                }
                if (TryReadInnerCert(certBytes, out var certInfo, out _))
                {
                    uint now = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    Write(
                        "GetCertAsync: cert fields " +
                        $"keyType={certInfo.KeyType} key={FormatBytes(certInfo.KeyData, 8)} " +
                        $"legacySteamId={certInfo.LegacySteamId} identity={certInfo.IdentityString ?? "<null>"} " +
                        $"appIds=[{string.Join(",", certInfo.AppIds.ToArray())}] " +
                        $"created={certInfo.TimeCreated} expires={certInfo.TimeExpiry} expiresIn={certInfo.TimeExpiry - now}s");
                }

                var cert = new SteamNetworkingSocketsCert_t
                {
                    m_eResult = EResult.k_EResultOK,
                    m_cbCert = (uint)certBytes.Length,
                    m_certOrMsg = FitBuffer(certBytes, 512),
                    m_caKeyID = dto.CaKeyId,
                    m_cbSignature = (uint)signatureBytes.Length,
                    m_signature = FitBuffer(signatureBytes, 128),
                    m_cbPrivKey = (uint)privKeyBytes.Length,
                    m_privKey = FitBuffer(privKeyBytes, 128)
                };

                Write(
                    "GetCertAsync: signed cert issued " +
                    $"(cert={certBytes.Length}, sig={signatureBytes.Length}, key={privKeyBytes.Length}, " +
                    $"caKeyId={dto.CaKeyId:X}, steamId={expectedSteamId}, appId={expectedAppId}, " +
                    $"certB64={Convert.ToBase64String(certBytes)}, sigB64={Convert.ToBase64String(signatureBytes)}, pub={FormatBytes(publicKeyBytes, 32)})");
                CacheCert(cert);
                return cert;
            }
            catch (Exception ex)
            {
                Write($"GetCertAsync failed: {ex.Message}");
                return empty;
            }
        }

        private static bool TryGetCachedCert(out SteamNetworkingSocketsCert_t cert)
        {
            lock (CertCacheLock)
            {
                cert = CachedCert;
                return CachedCertUtc != default(DateTime) &&
                    DateTime.UtcNow - CachedCertUtc < CertCacheTtl &&
                    CachedCert.m_eResult == EResult.k_EResultOK &&
                    CachedCert.m_cbCert > 0;
            }
        }

        private static void CacheCert(SteamNetworkingSocketsCert_t cert)
        {
            if (cert.m_eResult != EResult.k_EResultOK || cert.m_cbCert == 0)
            {
                return;
            }

            lock (CertCacheLock)
            {
                CachedCert = cert;
                CachedCertUtc = DateTime.UtcNow;
            }
        }

        private static byte[] FromBase64(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new byte[0];
            }

            try
            {
                return Convert.FromBase64String(value);
            }
            catch
            {
                return new byte[0];
            }
        }

        private static byte[] FitBuffer(byte[] source, int size)
        {
            var buffer = new byte[size];
            if (source.Length > 0)
            {
                Array.Copy(source, buffer, Math.Min(source.Length, size));
            }

            return buffer;
        }

        private static ulong GetExpectedSdrSteamId()
        {
            bool gameServer = SteamEmulator.SteamGameServer?.LoggedIn == true;
            return (ulong)(gameServer ? SteamEmulator.SteamID_GS : SteamEmulator.SteamID);
        }

        private static uint GetExpectedSdrAppId()
        {
            bool gameServer = SteamEmulator.SteamGameServer?.LoggedIn == true;
            return gameServer && SteamEmulator.SteamGameServer?.ServerData?.AppId != 0
                ? SteamEmulator.SteamGameServer.ServerData.AppId
                : SteamEmulator.AppID;
        }

        private static bool TryValidateInnerCert(
            byte[] certBytes,
            byte[] publicKeyBytes,
            ulong expectedSteamId,
            uint expectedAppId,
            out string error)
        {
            error = string.Empty;

            if (!TryReadInnerCert(certBytes, out var info, out error))
            {
                return false;
            }

            if (info.KeyType != 1)
            {
                error = $"key_type={info.KeyType}";
                return false;
            }

            if (info.KeyData == null || info.KeyData.Length != 32)
            {
                error = $"key_data={info.KeyData?.Length ?? 0}";
                return false;
            }

            if (publicKeyBytes.Length != 0 && (publicKeyBytes.Length != 32 || !BytesEqual(publicKeyBytes, info.KeyData)))
            {
                error = $"public key mismatch dto={publicKeyBytes.Length} cert={info.KeyData.Length}";
                return false;
            }

            if (expectedSteamId != 0)
            {
                if (info.LegacySteamId != 0 && info.LegacySteamId != expectedSteamId)
                {
                    error = $"legacy_steam_id={info.LegacySteamId} expected={expectedSteamId}";
                    return false;
                }

                string expectedIdentity = $"steamid:{expectedSteamId}";
                if (!string.Equals(info.IdentityString, expectedIdentity, StringComparison.Ordinal))
                {
                    error = $"identity_string={info.IdentityString ?? "<null>"} expected={expectedIdentity}";
                    return false;
                }
            }

            if (expectedAppId != 0 && !info.AppIds.Contains(expectedAppId))
            {
                error = $"missing app_id={expectedAppId}";
                return false;
            }

            uint now = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (info.TimeCreated > now + 300)
            {
                error = $"time_created={info.TimeCreated} now={now}";
                return false;
            }

            if (info.TimeExpiry <= now)
            {
                error = $"time_expiry={info.TimeExpiry} now={now}";
                return false;
            }

            return true;
        }

        private static bool TryReadInnerCert(byte[] certBytes, out InnerCertInfo info, out string error)
        {
            info = new InnerCertInfo();
            error = string.Empty;

            int offset = 0;
            while (offset < certBytes.Length)
            {
                if (!TryReadVarint(certBytes, ref offset, out var tag))
                {
                    error = "bad tag";
                    return false;
                }

                int field = (int)(tag >> 3);
                int wireType = (int)(tag & 7);
                switch (wireType)
                {
                    case 0:
                        if (!TryReadVarint(certBytes, ref offset, out var varintValue))
                        {
                            error = $"bad varint field={field}";
                            return false;
                        }

                        if (field == 1)
                        {
                            info.KeyType = (int)varintValue;
                        }
                        else if (field == 10)
                        {
                            info.AppIds.Add((uint)varintValue);
                        }
                        break;

                    case 1:
                        if (offset + 8 > certBytes.Length)
                        {
                            error = $"truncated fixed64 field={field}";
                            return false;
                        }

                        if (field == 4)
                        {
                            info.LegacySteamId = BitConverter.ToUInt64(certBytes, offset);
                        }
                        offset += 8;
                        break;

                    case 2:
                        if (!TryReadVarint(certBytes, ref offset, out var length64) || length64 > int.MaxValue)
                        {
                            error = $"bad length field={field}";
                            return false;
                        }

                        int length = (int)length64;
                        if (length < 0 || offset + length > certBytes.Length)
                        {
                            error = $"truncated bytes field={field}";
                            return false;
                        }

                        if (field == 2)
                        {
                            info.KeyData = new byte[length];
                            Array.Copy(certBytes, offset, info.KeyData, 0, length);
                        }
                        else if (field == 10)
                        {
                            if (!TryReadPackedAppIds(certBytes, offset, length, info.AppIds, out error))
                            {
                                return false;
                            }
                        }
                        else if (field == 12)
                        {
                            info.IdentityString = Encoding.UTF8.GetString(certBytes, offset, length);
                        }

                        offset += length;
                        break;

                    case 5:
                        if (offset + 4 > certBytes.Length)
                        {
                            error = $"truncated fixed32 field={field}";
                            return false;
                        }

                        uint fixed32 = BitConverter.ToUInt32(certBytes, offset);
                        if (field == 8)
                        {
                            info.TimeCreated = fixed32;
                        }
                        else if (field == 9)
                        {
                            info.TimeExpiry = fixed32;
                        }
                        offset += 4;
                        break;

                    default:
                        error = $"unsupported wire type={wireType} field={field}";
                        return false;
                }
            }

            return true;
        }

        private static bool TryReadPackedAppIds(byte[] bytes, int offset, int length, List<uint> appIds, out string error)
        {
            error = string.Empty;
            int end = offset + length;
            while (offset < end)
            {
                if (!TryReadVarint(bytes, ref offset, out var value) || offset > end)
                {
                    error = "bad packed app_ids";
                    return false;
                }
                appIds.Add((uint)value);
            }

            return offset == end;
        }

        private static bool TryReadVarint(byte[] bytes, ref int offset, out ulong value)
        {
            value = 0;
            int shift = 0;
            while (offset < bytes.Length && shift < 64)
            {
                byte b = bytes[offset++];
                value |= (ulong)(b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                {
                    return true;
                }
                shift += 7;
            }

            return false;
        }

        private static bool BytesEqual(byte[] left, byte[] right)
        {
            if (left.Length != right.Length)
            {
                return false;
            }

            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] != right[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static string FormatBytes(byte[] bytes, int maxBytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return "<empty>";
            }

            int count = Math.Min(bytes.Length, maxBytes);
            var prefix = new byte[count];
            Array.Copy(bytes, prefix, count);
            string text = BitConverter.ToString(prefix).Replace("-", "");
            return bytes.Length > count
                ? $"{text}...({bytes.Length})"
                : text;
        }

        private sealed class InnerCertInfo
        {
            public int KeyType { get; set; }
            public byte[] KeyData { get; set; }
            public ulong LegacySteamId { get; set; }
            public uint TimeCreated { get; set; }
            public uint TimeExpiry { get; set; }
            public List<uint> AppIds { get; } = new List<uint>();
            public string IdentityString { get; set; }
        }

        public int GetNetworkConfigJSON(IntPtr buf, uint cbBuf)
        {
            Write("GetNetworkConfigJSON");
            if (!SecureCertMode)
            {
                Write("GetNetworkConfigJSON: disabled in insecure LAN mode");
                return 0;
            }

            EnsureSecureCertPatcherStarted("GetNetworkConfigJSON");
            return WriteNetworkConfigJSON(buf, cbBuf);
        }

        public void CacheRelayTicket(IntPtr pTicket, uint cbTicket)
        {
            Write("CacheRelayTicket");
        }

        public uint GetCachedRelayTicketCount()
        {
            Write("GetCachedRelayTicketCount");
            return 0;
        }

        public int GetNetworkConfigJSON(IntPtr buf, uint cbBuf, string pszLauncherPartner)
        {
            Write($"GetNetworkConfigJSON (LauncherPartner = {pszLauncherPartner})");
            if (!SecureCertMode)
            {
                Write("GetNetworkConfigJSON: disabled in insecure LAN mode");
                return 0;
            }

            EnsureSecureCertPatcherStarted("GetNetworkConfigJSON(partner)");
            return WriteNetworkConfigJSON(buf, cbBuf);
        }

        public int GetCachedRelayTicket(uint idxTicket, IntPtr buf, uint cbBuf)
        {
            Write("GetCachedRelayTicket");
            return 0;
        }

        public void PostConnectionStateMsg(IntPtr pMsg, uint cbMsg)
        {
            Write("PostConnectionStateMsg");
            QueueCurrentNetworkStatusCallbacks();
        }

        public bool GetSTUNServer(int dont_know, IntPtr buf, int len)
        {
            Write("GetSTUNServer");
            return false;
        }

        public bool BAllowDirectConnectToPeer(IntPtr identity)
        {
            bool allow = !SecureCertMode;
            Write($"BAllowDirectConnectToPeer = {allow}");
            return allow;
        }

        public int BeginAsyncRequestFakeIP(int nNumPorts)
        {
            bool accepted = !SecureCertMode;
            Write($"BeginAsyncRequestFakeIP ({nNumPorts}) = {accepted}");
            // SteamNetworkingSocketsSerialized005 uses a 32-bit int here.
            // Returning an explicit 0/1 keeps the native x64 return register
            // defined, matching the Goldberg LAN implementation.
            return accepted ? 1 : 0;
        }

        private int WriteNetworkConfigJSON(IntPtr buf, uint cbBuf)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(GetNetworkConfigJson());
            int required = bytes.Length + 1;
            if (buf == IntPtr.Zero || cbBuf < required)
            {
                return required;
            }

            Marshal.Copy(bytes, 0, buf, bytes.Length);
            Marshal.WriteByte(buf, bytes.Length, 0);
            return required;
        }

        private static string GetNetworkConfigJson()
        {
            // Full SDR config (relay certs signed by our CA + POP/relay list), shared
            // with the SteamHTTP GetSDRConfig interception. Valve fetches the HTTP
            // config "only-if-cached" (never hits our HTTP), so this serialized path
            // is the one that actually feeds the native library its network config.
            return SteamHTTP.SdrConfigJson;
        }

        internal static bool EnsureSecureCertPatcherStarted(string reason, int waitMs = 0)
        {
            if (!SecureCertMode)
            {
                return false;
            }

            if (Interlocked.CompareExchange(ref SecureCertPatcherStartLogged, 1, 0) == 0)
            {
                SteamEmulator.Write("SteamNetworkingSocketsSerialized", $"Starting SDR cert patcher ({reason})");
            }

            bool patched = SdrCertPatcher.EnsurePatched(waitMs);
            if (waitMs > 0 && !patched)
            {
                SteamEmulator.Write("SteamNetworkingSocketsSerialized", $"SDR CA patch was not ready after {waitMs}ms; background patch remains active");
            }

            return patched;
        }


        internal static SteamRelayNetworkStatus_t BuildRelayNetworkStatus()
        {
            var status = new SteamRelayNetworkStatus_t
            {
                m_eAvail = ESteamNetworkingAvailability.k_ESteamNetworkingAvailability_Current,
                m_bPingMeasurementInProgress = 0,
                m_eAvailNetworkConfig = ESteamNetworkingAvailability.k_ESteamNetworkingAvailability_Current,
                m_eAvailAnyRelay = ESteamNetworkingAvailability.k_ESteamNetworkingAvailability_Current,
                DebugMsg = new byte[256]
            };
            byte[] message = Encoding.UTF8.GetBytes(SecureCertMode ? "Secure SDR" : "Insecure LAN");
            Array.Copy(message, status.DebugMsg, message.Length);
            return status;
        }

        internal static SteamNetAuthenticationStatus_t BuildAuthenticationStatus()
        {
            var status = new SteamNetAuthenticationStatus_t
            {
                Avail = ESteamNetworkingAvailability.k_ESteamNetworkingAvailability_Current,
                DebugMsg = new byte[256]
            };
            byte[] message = Encoding.UTF8.GetBytes(SecureCertMode ? "Secure SDR" : "Insecure LAN");
            Array.Copy(message, status.DebugMsg, message.Length);
            return status;
        }

        internal static void QueueCurrentNetworkStatusCallbacks(int delayMs = 0)
        {
            if (delayMs <= 0)
            {
                CallbackManager.AddCallback(BuildAuthenticationStatus());
                CallbackManager.AddCallback(BuildRelayNetworkStatus());
                SteamNetworkingUtils.Instance?.QueueNativeNetworkStatusCallbacks();
                return;
            }

            ThreadPool.QueueUserWorkItem(_ =>
            {
                Thread.Sleep(delayMs);
                CallbackManager.AddCallback(BuildAuthenticationStatus());
                CallbackManager.AddCallback(BuildRelayNetworkStatus());
                SteamNetworkingUtils.Instance?.QueueNativeNetworkStatusCallbacks();
            });
        }

    }
}
