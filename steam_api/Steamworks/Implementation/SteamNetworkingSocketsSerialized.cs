using System;
using SKYNET.Steamworks.Interfaces;
using System.Text;
using SKYNET.Callback;
using SKYNET.Managers;
using System.Net;
using System.IO;
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
        //     GetCertAsync returns the real signed certificate issued by the
        //     SKYNET server and applies the CA public-key patch to
        //     steamnetworkingsockets.dll, so the game actually validates our
        //     cert against the emulator CA. Use this to verify GetCertAsync
        //     end to end.
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
                QueueCurrentNetworkStatusCallbacks(250);
                QueueCurrentNetworkStatusCallbacks(1500);
                return CallbackManager.AddCallbackResult(CreateEmptyCertResult(EResult.k_EResultOK));
            }

            Write("GetCertAsync: secure SDR request queued");

            // Secure mode only: patch the SDR CA so the game trusts our cert.
            EnsureSecureCertPatcherStarted("GetCertAsync");

            QueueCurrentNetworkStatusCallbacks();
            QueueCurrentNetworkStatusCallbacks(250);
            QueueCurrentNetworkStatusCallbacks(1500);

            var handle = CallbackManager.AddCallbackResult(CreateEmptyCertResult(EResult.k_EResultNoConnection), false);
            StartCertRequestWorker(handle);
            return handle;
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

        private void StartCertRequestWorker(SteamAPICall_t handle)
        {
            var thread = new Thread(() =>
            {
                var cert = BuildCertResult();
                CallbackManager.CompleteCallbackResult(handle, cert, false);
            })
            {
                IsBackground = true,
                Name = "SdrCertRequest"
            };

            thread.Start();
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

                if (certBytes.Length == 0 || certBytes.Length > 512 ||
                    signatureBytes.Length > 128 || privKeyBytes.Length > 128)
                {
                    Write($"GetCertAsync: cert sizes out of range (cert={certBytes.Length}, sig={signatureBytes.Length}, key={privKeyBytes.Length})");
                    return empty;
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

                Write($"GetCertAsync: signed cert issued (cert={certBytes.Length}, sig={signatureBytes.Length}, key={privKeyBytes.Length}, caKeyId={dto.CaKeyId:X})");
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

        public bool BeginAsyncRequestFakeIP(int nNumPorts)
        {
            bool accepted = !SecureCertMode;
            Write($"BeginAsyncRequestFakeIP ({nNumPorts}) = {accepted}");
            return accepted;
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
            return "{\"revision\":1,\"pops\":{\"iad\":{\"desc\":\"Local\",\"geo\":[-77.0365,38.8977],\"partners\":1,\"tier\":1,\"relays\":[{\"ipv4\":\"127.0.0.1\",\"port_range\":[27015,27060]}]}}}";
        }

        internal static void EnsureSecureCertPatcherStarted(string reason)
        {
            if (!SecureCertMode)
            {
                return;
            }

            if (Interlocked.CompareExchange(ref SecureCertPatcherStartLogged, 1, 0) == 0)
            {
                SteamEmulator.Write("SteamNetworkingSocketsSerialized", $"Starting SDR cert patcher ({reason})");
            }

            SdrCertPatcher.Start();
        }

        private static bool TryFetchSdrConfig(out string body)
        {
            body = null;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://api.steampowered.com/ISteamApps/GetSDRConfig/v1?appid=570");
                request.Method = "GET";
                request.Timeout = 3000;
                request.ReadWriteTimeout = 3000;
                request.UserAgent = "SKYNET Steam Emulator";
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    if ((int)response.StatusCode < 200 || (int)response.StatusCode >= 300)
                    {
                        return false;
                    }

                    body = reader.ReadToEnd();
                    return !string.IsNullOrEmpty(body);
                }
            }
            catch
            {
                return false;
            }
        }

        private static SteamRelayNetworkStatus_t BuildRelayNetworkStatus()
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

        private static SteamNetAuthenticationStatus_t BuildAuthenticationStatus()
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

        private static void QueueCurrentNetworkStatusCallbacks(int delayMs = 0)
        {
            if (delayMs <= 0)
            {
                CallbackManager.AddCallback(BuildAuthenticationStatus());
                CallbackManager.AddCallback(BuildRelayNetworkStatus());
                return;
            }

            ThreadPool.QueueUserWorkItem(_ =>
            {
                Thread.Sleep(delayMs);
                CallbackManager.AddCallback(BuildAuthenticationStatus());
                CallbackManager.AddCallback(BuildRelayNetworkStatus());
            });
        }
    }
}
