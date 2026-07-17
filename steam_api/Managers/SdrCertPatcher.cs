using System;
using System.Security.Cryptography;
using System.Threading;

namespace SKYNET.Managers
{
    /// <summary>
    /// Patches, in memory, the Steam Datagram (SDR) certificate-authority public key
    /// inside steamnetworkingsockets.dll so certs signed by the emulator's CA
    /// validate. The DLL on disk is never modified.
    ///
    /// Anchor on the version-independent OpenSSH ed25519 text marker
    /// ("ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAI") and rewrite, in place (same length):
    ///   (1) the 43-char base64 key body with the emulator CA's body, and
    ///   (2) the trailing " ID&lt;digits&gt;" comment with the emulator key-id decimal.
    /// Both are required: the game asserts the CA key-id is a substring of the whole
    /// hardcoded root-CA blob (which includes the comment), so patching only the key
    /// body crashes the game at the menu (certstore.cpp Fatal Assertion).
    ///
    /// The marker is constant for every ed25519 key, so this survives Valve rotating
    /// their CA key across Dota updates (unlike matching a hard-coded Valve key).
    ///
    /// Timing/threading: the patch must run on a game thread with the loader lock
    /// free, never the init thread (touching modules or creating a thread while Dota
    /// loads DLLs deadlocks at ~30MB). It is driven by TryPatchNow() on the game's
    /// first SDR cert request (GetCertAsync), which is also why games that never use
    /// SDR pay nothing: they never load the DLL and never call in here.
    /// </summary>
    public static class SdrCertPatcher
    {
        private static readonly string[] TargetModules =
        {
            "steamnetworkingsockets.dll",
            "steamnetworkingsockets64.dll",
            "steamnetworkingsockets_x64.dll"
        };

        // Constant OpenSSH ed25519 text prefix. Every ed25519 authorized-keys line
        // begins with exactly this (12-char "ssh-ed25519 " + the fixed base64 header
        // "AAAAC3NzaC1lZDI1NTE5AAAAI" that encodes [len]"ssh-ed25519"[len]). Anchoring
        // here is key-agnostic: it does not depend on Valve's current key.
        private const string SshEd25519Prefix = "ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAI";
        private const int PrefixLen = 37;      // SshEd25519Prefix.Length
        private const int KeyBodyLen = 43;     // base64 tail encoding the 32-byte key

        // Emulator CA public key (32 raw bytes). MUST equal the server's SDR CA
        // public key; the server signs certs with the key-id derived from it (see
        // Sdr:CaKeyId in the server appsettings). Swapping this into the DLL makes
        // the game trust SKYNET-signed SDR certs.
        private const string EmulatorCaPublicKeyHex =
            "FEAA97C32C7E5BF684DF86F120F3C40C785DCECDEDCB91FC223E54E76AA30F59";

        private static int _started;
        private static int _patched;   // 1 once the module is patched (or confirmed already patched); never repeats
        private static int _patching;  // reentrancy guard for TryPatchNow
        private static byte[] _anchorCache;
        private static byte[] _emuBodyCache;

        /// <summary>
        /// Announces the patcher. Does NO work on the calling (init) thread: it must
        /// not create a thread or touch module memory here. Creating a thread during
        /// Dota's startup triggers DLL_THREAD_ATTACH, which needs the loader lock
        /// Dota holds while loading DLLs on other threads -> deadlock (init hangs at
        /// ~30MB). The real patch happens later, from TryPatchNow(), when the game
        /// makes its first SDR cert request (GetCertAsync) on a game thread with the
        /// loader lock free.
        /// </summary>
        public static void Start()
        {
            if (Interlocked.Exchange(ref _started, 1) != 0)
            {
                return;
            }

            Write("SDR CA patcher armed; patches steamnetworkingsockets in-memory on the first SDR cert request");
        }

        /// <summary>
        /// One inline patch attempt on the CURRENT thread, then never again once it
        /// succeeds (_patched latches). Safe only from a game/interface-call context
        /// (loader lock free) — never the init thread. Driven by the game's first SDR
        /// cert request (GetCertAsync): games that never touch SDR never load
        /// steamnetworkingsockets.dll and never call this, so there is zero per-frame
        /// or background cost for them. Returns true if patched or already patched.
        /// </summary>
        public static bool TryPatchNow()
        {
            if (Volatile.Read(ref _patched) != 0)
            {
                return true;
            }

            if (Interlocked.Exchange(ref _patching, 1) != 0)
            {
                return Volatile.Read(ref _patched) != 0;
            }

            try
            {
                if (TryPatchLoadedModules(out bool _))
                {
                    Interlocked.Exchange(ref _patched, 1);
                    return true;
                }

                return false;
            }
            finally
            {
                Interlocked.Exchange(ref _patching, 0);
            }
        }

        // ================= in-memory =================

        // Builds the anchor and emu key body once and caches them, so a repeated
        // patch attempt does not re-parse the key / re-encode base64.
        private static bool TryGetPatchInputs(out byte[] anchor, out byte[] emuBody)
        {
            anchor = Volatile.Read(ref _anchorCache);
            emuBody = Volatile.Read(ref _emuBodyCache);
            if (anchor != null && emuBody != null)
            {
                return true;
            }

            if (!TryBuildEmuBody(out emuBody))
            {
                anchor = null;
                return false;
            }

            anchor = Ascii(SshEd25519Prefix);
            Volatile.Write(ref _emuBodyCache, emuBody);
            Volatile.Write(ref _anchorCache, anchor);
            return true;
        }

        private static bool TryPatchLoadedModules(out bool moduleFound)
        {
            moduleFound = false;

            if (!TryGetPatchInputs(out byte[] anchor, out byte[] emuBody))
            {
                Write("SDR emulator CA key invalid in code; memory patch aborted");
                return false;
            }

            bool ready = false;

            // Resolve each target by name with GetModuleHandle instead of walking
            // Process.Modules: enumerating the module list takes the loader lock,
            // which deadlocks while Dota is still busy loading DLLs at startup.
            foreach (var name in TargetModules)
            {
                IntPtr handle = GetModuleHandle(name);
                if (handle == IntPtr.Zero)
                {
                    continue;
                }

                moduleFound = true;
                ready |= TryPatchLoadedModule(handle, name, anchor, emuBody);
            }

            return ready;
        }

        private static bool TryPatchLoadedModule(IntPtr moduleHandle, string name, byte[] anchor, byte[] emuBody)
        {
            try
            {
                long moduleBase = moduleHandle.ToInt64();
                int imageSize = ReadSizeOfImage(moduleHandle);
                if (imageSize <= 0)
                {
                    return false;
                }
                long moduleEnd = moduleBase + imageSize;

                int patched = 0;
                bool alreadyPatched = false;

                // Walk the module's virtual address range region by region. The
                // module's ModuleMemorySize is the VIRTUAL size and its tail pages may
                // be uncommitted/guarded; copying across them faults with an
                // AccessViolation that the hosted CLR cannot unwind (it hangs the
                // process). So only read committed, readable regions.
                long cursor = moduleBase;
                int mbiSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION));
                while (cursor < moduleEnd)
                {
                    if (VirtualQuery((IntPtr)cursor, out MEMORY_BASIC_INFORMATION mbi, (UIntPtr)mbiSize) == UIntPtr.Zero)
                    {
                        break;
                    }

                    long regionBase = mbi.BaseAddress.ToInt64();
                    long regionSize = mbi.RegionSize.ToInt64();
                    if (regionSize <= 0)
                    {
                        break;
                    }

                    long readStart = Math.Max(regionBase, cursor);
                    long readEnd = Math.Min(regionBase + regionSize, moduleEnd);

                    if (mbi.State == MemCommit && IsReadable(mbi.Protect) && readEnd > readStart)
                    {
                        int len = (int)(readEnd - readStart);
                        var buf = new byte[len];
                        System.Runtime.InteropServices.Marshal.Copy((IntPtr)readStart, buf, 0, len);

                        foreach (int at in FindAll(buf, anchor))
                        {
                            int bodyAt = at + PrefixLen;
                            if (bodyAt + KeyBodyLen > buf.Length)
                            {
                                continue;
                            }

                            // (1) Key body: replace the 43-char base64 tail in place.
                            if (Matches(buf, bodyAt, emuBody))
                            {
                                alreadyPatched = true;
                            }
                            else
                            {
                                WriteProcessMemoryAbsolute((IntPtr)(readStart + bodyAt), emuBody);
                                patched++;
                            }

                            // (2) ID comment: the game asserts the CA key-id (decimal)
                            // is a substring of the whole hardcoded root-CA blob, which
                            // includes the trailing " ID<digits>" comment. Patching only
                            // the key body crashes at the menu with a Fatal Assertion in
                            // steamnetworkingsockets_certstore.cpp. So the comment digits
                            // must carry our key-id too.
                            switch (PatchIdComment(buf, bodyAt + KeyBodyLen, out byte[] idBytes, out int digitsAbsOffset))
                            {
                                case CommentPatch.Patched:
                                    WriteProcessMemoryAbsolute((IntPtr)(readStart + digitsAbsOffset), idBytes);
                                    patched++;
                                    break;
                                case CommentPatch.AlreadyPatched:
                                    alreadyPatched = true;
                                    break;
                            }
                        }
                    }

                    cursor = regionBase + regionSize;
                }

                if (patched > 0)
                {
                    Write($"SDR memory CA patched in {name} ({patched} occurrence(s)); disk file untouched. Emu key-id {DescribeEmuKeyId()}");
                    return true;
                }

                if (alreadyPatched)
                {
                    Write($"SDR memory CA already patched in {name}");
                    return true;
                }

                Write($"SDR memory CA marker not found in {name}");
            }
            catch (Exception ex)
            {
                Write($"SDR memory patch failed for {name}: {ex.Message}");
            }

            return false;
        }

        private static bool IsReadable(uint protect)
        {
            if ((protect & (PageGuard | PageNoAccess)) != 0)
            {
                return false;
            }

            const uint readable = PageReadOnly | PageReadWrite | PageWriteCopy |
                                  PageExecuteRead | PageExecuteReadWrite | PageExecuteWriteCopy;
            return (protect & readable) != 0;
        }

        private static void WriteProcessMemoryAbsolute(IntPtr address, byte[] replacement)
        {
            if (!VirtualProtect(address, (UIntPtr)replacement.Length, PageExecuteReadWrite, out uint oldProtect))
            {
                throw new InvalidOperationException("VirtualProtect failed before write");
            }

            try
            {
                System.Runtime.InteropServices.Marshal.Copy(replacement, 0, address, replacement.Length);
            }
            finally
            {
                VirtualProtect(address, (UIntPtr)replacement.Length, oldProtect, out _);
            }
        }

        // ================= emu key body =================

        // Builds the 43-char base64 key tail for the emulator key, i.e. the portion
        // of the OpenSSH ed25519 token that follows the constant prefix. This is what
        // replaces Valve's key body in place.
        private static bool TryBuildEmuBody(out byte[] emuBody)
        {
            emuBody = null;
            byte[] emuRaw = ParseHex(EmulatorCaPublicKeyHex);
            if (emuRaw == null || emuRaw.Length != 32)
            {
                return false;
            }

            string token = ToOpenSshToken(emuRaw); // 68 chars, starts with the constant prefix's base64
            if (token.Length != PrefixLen - 12 + KeyBodyLen)
            {
                // 25 (prefix base64) + 43 (key body) = 68; guard against format drift.
                return false;
            }

            string tail = token.Substring(25); // 43-char key body
            if (tail.Length != KeyBodyLen)
            {
                return false;
            }

            emuBody = Ascii(tail);
            return true;
        }

        private enum CommentPatch { None, Patched, AlreadyPatched }

        /// <summary>
        /// Inspects the " ID&lt;digits&gt;" comment that follows the key body. Emits the
        /// emulator key-id decimal, right-aligned and zero-padded to the original
        /// digit count (leading zeros are fine: the game does a substring search).
        /// Returns None if the comment is absent/malformed or the emu id is longer
        /// than the original field (cannot fit without resizing).
        /// </summary>
        private static CommentPatch PatchIdComment(byte[] buf, int commentStart, out byte[] idBytes, out int digitsOffset)
        {
            idBytes = null;
            digitsOffset = -1;

            // Expect a single space then the literal "ID".
            if (commentStart + 3 > buf.Length ||
                buf[commentStart] != (byte)' ' ||
                buf[commentStart + 1] != (byte)'I' ||
                buf[commentStart + 2] != (byte)'D')
            {
                return CommentPatch.None;
            }

            int digitsAt = commentStart + 3;
            int len = 0;
            while (digitsAt + len < buf.Length && buf[digitsAt + len] >= (byte)'0' && buf[digitsAt + len] <= (byte)'9')
            {
                len++;
            }
            if (len == 0)
            {
                return CommentPatch.None;
            }

            string emuId = DescribeEmuKeyId();
            if (emuId == "unknown" || emuId.Length > len)
            {
                return CommentPatch.None;
            }

            string padded = emuId.PadLeft(len, '0');
            byte[] desired = Ascii(padded);

            if (Matches(buf, digitsAt, desired))
            {
                return CommentPatch.AlreadyPatched;
            }

            idBytes = desired;
            digitsOffset = digitsAt;
            return CommentPatch.Patched;
        }

        private static string DescribeEmuKeyId()
        {
            byte[] emuRaw = ParseHex(EmulatorCaPublicKeyHex);
            if (emuRaw == null || emuRaw.Length != 32)
            {
                return "unknown";
            }

            using (var sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(emuRaw);
                ulong id = BitConverter.ToUInt64(hash, 0); // little-endian leading 8 bytes
                return id.ToString();
            }
        }

        // ================= helpers =================

        // SizeOfImage from the module's PE optional header. The header page is always
        // committed and readable, so these small reads are safe.
        private static int ReadSizeOfImage(IntPtr moduleBase)
        {
            try
            {
                int e_lfanew = System.Runtime.InteropServices.Marshal.ReadInt32(moduleBase, 0x3C);
                // IMAGE_NT_HEADERS: Signature(4) + IMAGE_FILE_HEADER(20); SizeOfImage is
                // at offset 56 (0x38) inside the optional header (same for PE32/PE32+).
                return System.Runtime.InteropServices.Marshal.ReadInt32(moduleBase, e_lfanew + 4 + 20 + 56);
            }
            catch
            {
                return 0;
            }
        }

        private static System.Collections.Generic.IEnumerable<int> FindAll(byte[] haystack, byte[] needle)
        {
            if (needle.Length == 0 || haystack.Length < needle.Length)
            {
                yield break;
            }

            int limit = haystack.Length - needle.Length;
            int i = 0;
            while (i <= limit)
            {
                if (Matches(haystack, i, needle))
                {
                    yield return i;
                    i += needle.Length;
                }
                else
                {
                    i++;
                }
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

        private static byte[] Ascii(string value) => System.Text.Encoding.ASCII.GetBytes(value);

        // OpenSSH ed25519 token: base64( [len]"ssh-ed25519" [len] key32 ). All ed25519
        // tokens share the same length (68 chars) and the same 25-char base64 prefix.
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
            Array.Reverse(len); // big-endian length prefix
            ms.Write(len, 0, 4);
            ms.Write(value, 0, value.Length);
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

        private const uint PageNoAccess = 0x01;
        private const uint PageReadOnly = 0x02;
        private const uint PageReadWrite = 0x04;
        private const uint PageWriteCopy = 0x08;
        private const uint PageExecuteRead = 0x20;
        private const uint PageExecuteReadWrite = 0x40;
        private const uint PageExecuteWriteCopy = 0x80;
        private const uint PageGuard = 0x100;
        private const uint MemCommit = 0x1000;

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern UIntPtr VirtualQuery(IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, UIntPtr dwLength);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private static void Write(object message)
        {
            SteamEmulator.Write("SdrCertPatcher", message);
        }
    }
}
