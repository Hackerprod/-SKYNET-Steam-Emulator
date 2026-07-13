using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace SKYNET.Managers
{
    /// <summary>
    /// Patches the Steam Datagram (SDR) certificate authority public key inside
    /// steamnetworkingsockets.dll in memory, so certs signed by the emulator's CA
    /// are accepted. The DLL file on disk is never touched.
    ///
    /// The search key (Valve's CA public key, 32 bytes) comes from configuration;
    /// the replacement key (the emulator CA public key) comes from the server. All
    /// occurrences are replaced by signature scan, so the patch survives game
    /// updates as long as the embedded key value does not change.
    /// </summary>
    public static class SdrCertPatcher
    {
        private static readonly string[] TargetModules =
        {
            "steamnetworkingsockets.dll",
            "steamnetworkingsockets64.dll",
            "steamnetworkingsockets_x64.dll"
        };

        // Emulator CA public key. Replaces Valve's key inside
        // steamnetworkingsockets.dll so certs signed by the SKYNET server validate.
        // MUST match the CA public key the server pins/prints on startup: the server
        // signs with the seed = first 32 bytes of the reference private.pem, whose
        // Ed25519 public key is this value. Its Steam key-id (SHA256 leading 8 bytes,
        // little-endian) is 14779564839147732469 == Sdr:CaKeyId in appsettings.json.
        private const string EmulatorCaPublicKeyHex =
            "FEAA97C32C7E5BF684DF86F120F3C40C785DCECDEDCB91FC223E54E76AA30F59";

        // Valve stores the CA key-id in the OpenSSH comment ("ID<decimal>") and reads
        // it from there at runtime rather than re-deriving it from the key bytes. The
        // patcher only swaps the key body, so we must also rewrite this comment to the
        // key-id of our CA, otherwise the game looks up the wrong id and the cert's
        // ca_key_id (14779564839147732469) never matches. Both ids are 20 digits, so
        // the replacement is in place.
        private const string ValveCaKeyIdComment = "ID18220590129359924542";
        private const string EmulatorCaKeyIdComment = "ID14779564839147732469";

        // Valve's SDR cert-authority public key, extracted from
        // steamnetworkingsockets.dll. The DLL stores it as an OpenSSH ed25519 text
        // token (not raw bytes), so we patch the base64 text form in place; the raw
        // form is kept as a fallback for builds that embed it directly.
        private const string ValveCaPublicKeyHex =
            "9AECA04E1751CE6268D569002CA1E1FA1B2DBC26D36B4EA3A0083AD372829B84";

        private static int Started;
        private static int PatchSucceeded;
        private static readonly ManualResetEventSlim PatchFinished = new ManualResetEventSlim(false);

        public static void Start()
        {
            if (Interlocked.Exchange(ref Started, 1) == 1)
            {
                return;
            }

            var thread = new Thread(Run) { IsBackground = true, Name = "SdrCertPatcher" };
            thread.Start();
        }

        /// <summary>
        /// Starts the background patcher and, when a caller is about to ask the
        /// native networking library for a certificate, waits briefly for the
        /// in-memory CA replacement.  The timeout is deliberately bounded: a
        /// missing standalone networking module must never stall Dota startup.
        /// </summary>
        public static bool EnsurePatched(int timeoutMs)
        {
            Start();

            if (Volatile.Read(ref PatchSucceeded) != 0)
            {
                return true;
            }

            if (timeoutMs > 0)
            {
                PatchFinished.Wait(timeoutMs);
            }

            return Volatile.Read(ref PatchSucceeded) != 0;
        }

        private static void Run()
        {
            try
            {
                Write("SDR patcher thread started");
                byte[] valveRaw = ParseHex(ValveCaPublicKeyHex);
                byte[] emuRaw = ParseHex(EmulatorCaPublicKeyHex);
                if (valveRaw == null || valveRaw.Length != 32 || emuRaw == null || emuRaw.Length != 32)
                {
                    Write("SDR CA keys invalid in code, patcher aborted");
                    return;
                }

                // The key is embedded as an OpenSSH ed25519 base64 token; both tokens are
                // the same length (68 chars), so replacement is in place. Fall back to the
                // raw 32-byte form for builds that store the decoded key.
                var pairs = new List<KeyValuePair<byte[], byte[]>>
                {
                    new KeyValuePair<byte[], byte[]>(Ascii(ToOpenSshToken(valveRaw)), Ascii(ToOpenSshToken(emuRaw))),
                    new KeyValuePair<byte[], byte[]>(valveRaw, emuRaw),
                    // Rewrite the key-id comment so the game resolves our cert's ca_key_id.
                    new KeyValuePair<byte[], byte[]>(Ascii(ValveCaKeyIdComment), Ascii(EmulatorCaKeyIdComment))
                };

                // Start polling before GetCertAsync.  The networking module may not be
                // resident during Steam API initialization, but once it appears we patch
                // it in tens of milliseconds rather than after a half-second race.
                for (int attempt = 0; attempt < 1200; attempt++)
                {
                    if (TryPatchLoadedModule(pairs, out int patched) && patched > 0)
                    {
                        Interlocked.Exchange(ref PatchSucceeded, 1);
                        Write($"SDR CA public key patched ({patched} occurrence(s))");
                        return;
                    }

                    Thread.Sleep(25);
                }

                Write("steamnetworkingsockets module not found or key pattern absent after timeout");
            }
            catch (Exception ex)
            {
                Write($"SDR patcher failed: {ex.Message}");
            }
            finally
            {
                PatchFinished.Set();
            }
        }

        private static byte[] Ascii(string value)
        {
            return System.Text.Encoding.ASCII.GetBytes(value);
        }

        private static string ToOpenSshToken(byte[] key)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                WriteSshString(ms, Ascii("ssh-ed25519"));
                WriteSshString(ms, key);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private static void WriteSshString(System.IO.MemoryStream ms, byte[] value)
        {
            var len = BitConverter.GetBytes(value.Length);
            Array.Reverse(len);
            ms.Write(len, 0, 4);
            ms.Write(value, 0, value.Length);
        }

        private static bool TryPatchLoadedModule(List<KeyValuePair<byte[], byte[]>> pairs, out int totalPatched)
        {
            totalPatched = 0;
            IntPtr module = IntPtr.Zero;
            foreach (var name in TargetModules)
            {
                module = GetModuleHandle(name);
                if (module != IntPtr.Zero)
                {
                    break;
                }
            }

            if (module == IntPtr.Zero)
            {
                return false;
            }

            if (!GetModuleInformation(GetCurrentProcess(), module, out MODULEINFO info, (uint)Marshal.SizeOf<MODULEINFO>()))
            {
                return false;
            }

            long baseAddr = module.ToInt64();
            long endAddr = baseAddr + info.SizeOfImage;
            long cursor = baseAddr;

            while (cursor < endAddr)
            {
                if (VirtualQuery((IntPtr)cursor, out MEMORY_BASIC_INFORMATION mbi, (uint)Marshal.SizeOf<MEMORY_BASIC_INFORMATION>()) == UIntPtr.Zero)
                {
                    break;
                }

                long regionBase = mbi.BaseAddress.ToInt64();
                long regionSize = (long)mbi.RegionSize;
                long next = regionBase + regionSize;
                if (next <= cursor)
                {
                    break;
                }

                if (mbi.State == MEM_COMMIT && IsReadable(mbi.Protect))
                {
                    long scanStart = Math.Max(regionBase, baseAddr);
                    long scanEnd = Math.Min(next, endAddr);
                    totalPatched += ScanAndPatchRegion(scanStart, scanEnd - scanStart, pairs);
                }

                cursor = next;
            }

            return true;
        }

        private static int ScanAndPatchRegion(long start, long length, List<KeyValuePair<byte[], byte[]>> pairs)
        {
            var buffer = new byte[length];
            if (!ReadProcessMemory(GetCurrentProcess(), (IntPtr)start, buffer, (UIntPtr)length, out var read) || (long)read != length)
            {
                return 0;
            }

            int patched = 0;
            foreach (var pair in pairs)
            {
                patched += ScanAndPatchBuffer(buffer, start, pair.Key, pair.Value);
            }

            return patched;
        }

        private static int ScanAndPatchBuffer(byte[] buffer, long start, byte[] search, byte[] replacement)
        {
            if (buffer.Length < search.Length)
            {
                return 0;
            }

            int patched = 0;
            int i = 0;
            int limit = buffer.Length - search.Length;
            while (i <= limit)
            {
                if (Matches(buffer, i, search))
                {
                    if (WriteBytes((IntPtr)(start + i), replacement))
                    {
                        patched++;
                    }

                    i += search.Length;
                }
                else
                {
                    i++;
                }
            }

            return patched;
        }

        private static bool WriteBytes(IntPtr address, byte[] data)
        {
            if (!VirtualProtect(address, (UIntPtr)data.Length, PAGE_EXECUTE_READWRITE, out uint oldProtect))
            {
                return false;
            }

            try
            {
                if (!WriteProcessMemory(GetCurrentProcess(), address, data, (UIntPtr)data.Length, out var written) || (long)written != data.Length)
                {
                    return false;
                }

                FlushInstructionCache(GetCurrentProcess(), address, (UIntPtr)data.Length);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                VirtualProtect(address, (UIntPtr)data.Length, oldProtect, out _);
            }
        }

        private static bool Matches(byte[] haystack, int offset, byte[] needle)
        {
            for (int i = 0; i < needle.Length; i++)
            {
                if (haystack[offset + i] != needle[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsReadable(uint protect)
        {
            if ((protect & PAGE_GUARD) != 0 || (protect & PAGE_NOACCESS) != 0)
            {
                return false;
            }

            const uint readable = PAGE_READONLY | PAGE_READWRITE | PAGE_EXECUTE_READ | PAGE_EXECUTE_READWRITE | PAGE_WRITECOPY | PAGE_EXECUTE_WRITECOPY;
            return (protect & readable) != 0;
        }

        private static byte[] ParseHex(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
            {
                return null;
            }

            var cleaned = hex.Replace(" ", string.Empty).Replace("-", string.Empty).Replace(",", string.Empty);
            if (cleaned.StartsWith("0x") || cleaned.StartsWith("0X"))
            {
                cleaned = cleaned.Substring(2);
            }

            if (cleaned.Length % 2 != 0)
            {
                return null;
            }

            var bytes = new byte[cleaned.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                if (!byte.TryParse(cleaned.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber, null, out bytes[i]))
                {
                    return null;
                }
            }

            return bytes;
        }

        private static void Write(object message)
        {
            SteamEmulator.Write("SdrCertPatcher", message);
        }

        #region Native

        private const uint MEM_COMMIT = 0x1000;
        private const uint PAGE_NOACCESS = 0x01;
        private const uint PAGE_READONLY = 0x02;
        private const uint PAGE_READWRITE = 0x04;
        private const uint PAGE_WRITECOPY = 0x08;
        private const uint PAGE_EXECUTE_READ = 0x20;
        private const uint PAGE_EXECUTE_READWRITE = 0x40;
        private const uint PAGE_EXECUTE_WRITECOPY = 0x80;
        private const uint PAGE_GUARD = 0x100;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll")]
        private static extern UIntPtr VirtualQuery(IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("kernel32.dll")]
        private static extern bool FlushInstructionCache(IntPtr hProcess, IntPtr lpBaseAddress, UIntPtr dwSize);

        // Read/WriteProcessMemory fail gracefully (return false) on an invalid or
        // just-unmapped page instead of raising an uncatchable AccessViolation the
        // way Marshal.Copy does, which was crashing DLL load under SDR mode.
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UIntPtr nSize, out UIntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UIntPtr nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("psapi.dll", SetLastError = true)]
        private static extern bool GetModuleInformation(IntPtr hProcess, IntPtr hModule, out MODULEINFO lpmodinfo, uint cb);

        [StructLayout(LayoutKind.Sequential)]
        private struct MODULEINFO
        {
            public IntPtr lpBaseOfDll;
            public uint SizeOfImage;
            public IntPtr EntryPoint;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public UIntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        #endregion
    }
}
