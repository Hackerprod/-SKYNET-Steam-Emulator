using System;
using System.Collections.Generic;

namespace SKYNET.Managers
{
    /// <summary>
    /// Patches the Steam Datagram (SDR) certificate authority public key inside
    /// steamnetworkingsockets.dll on disk, so certs signed by the emulator's CA
    /// are accepted. Memory patching is intentionally disabled; this mirrors the
    /// old SKYNET NetworkingPatcher flow and lets us isolate whether disk patching
    /// is enough for the current Dota build.
    ///
    /// The search keys are Valve's CA and the older SKYNET CA seen in existing
    /// patched installs. The replacement key is the emulator CA used by the
    /// server. All occurrences are replaced by signature scan, so the patch
    /// survives game updates as long as the embedded key value does not change.
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
        private const string StaleSkynetCaKeyIdComment = "ID14497934795134905411";
        private const string EmulatorCaKeyIdComment = "ID14779564839147732469";

        // Valve's SDR cert-authority public key, extracted from
        // steamnetworkingsockets.dll. The DLL stores it as an OpenSSH ed25519 text
        // token (not raw bytes), so we patch the base64 text form in place; the raw
        // form is kept as a fallback for builds that embed it directly.
        private const string ValveCaPublicKeyHex =
            "9AECA04E1751CE6268D569002CA1E1FA1B2DBC26D36B4EA3A0083AD372829B84";

        // Older SKYNET builds patched steamnetworkingsockets.dll on disk with a
        // different CA. Patch that stale key too so installs can recover without
        // manually restoring Valve's original DLL first.
        private const string StaleSkynetCaPublicKeyHex =
            "AE8DA5FECF4DDE11247217C047B8F469984982DC84E4562D0BF4586133988C84";

        public static void Start()
        {
            Write("SDR memory CA patcher disabled; disk patch mode only");
        }

        public static bool EnsureDiskPatched()
        {
            string baseDirectory;
            try
            {
                baseDirectory = global::SKYNET.Common.GetPath();
            }
            catch (Exception ex)
            {
                Write($"SDR disk patch skipped: process path unavailable ({ex.Message})");
                return false;
            }

            return EnsureDiskPatched(baseDirectory);
        }

        internal static bool EnsureDiskPatched(string baseDirectory)
        {
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                Write("SDR disk patch skipped: process path unavailable");
                return false;
            }

            var pairs = BuildPatchPairs();
            byte[] emuRaw = ParseHex(EmulatorCaPublicKeyHex);
            if (pairs == null || emuRaw == null || emuRaw.Length != 32)
            {
                Write("SDR CA keys invalid in code, disk patch aborted");
                return false;
            }

            string emuToken = ToOpenSshToken(emuRaw);
            bool foundTarget = false;
            bool ready = false;
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var name in TargetModules)
            {
                string path = System.IO.Path.Combine(baseDirectory, name);
                if (!seen.Add(path) || !System.IO.File.Exists(path))
                {
                    continue;
                }

                foundTarget = true;
                ready |= TryPatchFileOnDisk(path, pairs, emuRaw, emuToken);
            }

            if (!foundTarget)
            {
                Write($"SDR disk patch skipped: no steamnetworkingsockets DLL in {baseDirectory}");
            }

            return ready;
        }

        /// <summary>
        /// Disk-only patch check. The memory patcher is disabled by design while
        /// validating the old NetworkingPatcher-style flow.
        /// </summary>
        public static bool EnsurePatched(int timeoutMs)
        {
            return EnsureDiskPatched();
        }

        private static List<KeyValuePair<byte[], byte[]>> BuildPatchPairs()
        {
            byte[] valveRaw = ParseHex(ValveCaPublicKeyHex);
            byte[] staleSkynetRaw = ParseHex(StaleSkynetCaPublicKeyHex);
            byte[] emuRaw = ParseHex(EmulatorCaPublicKeyHex);
            if (valveRaw == null || valveRaw.Length != 32 ||
                staleSkynetRaw == null || staleSkynetRaw.Length != 32 ||
                emuRaw == null || emuRaw.Length != 32)
            {
                return null;
            }

            // The key is embedded as an OpenSSH ed25519 base64 token; all ed25519
            // tokens have the same length, so replacement is in place. Fall back to
            // the raw 32-byte form for builds that store the decoded key.
            return new List<KeyValuePair<byte[], byte[]>>
            {
                new KeyValuePair<byte[], byte[]>(Ascii(ToOpenSshToken(valveRaw)), Ascii(ToOpenSshToken(emuRaw))),
                new KeyValuePair<byte[], byte[]>(Ascii(ToOpenSshToken(staleSkynetRaw)), Ascii(ToOpenSshToken(emuRaw))),
                new KeyValuePair<byte[], byte[]>(valveRaw, emuRaw),
                new KeyValuePair<byte[], byte[]>(staleSkynetRaw, emuRaw),
                // Rewrite the key-id comment so the game resolves our cert's ca_key_id.
                new KeyValuePair<byte[], byte[]>(Ascii(ValveCaKeyIdComment), Ascii(EmulatorCaKeyIdComment)),
                new KeyValuePair<byte[], byte[]>(Ascii(StaleSkynetCaKeyIdComment), Ascii(EmulatorCaKeyIdComment))
            };
        }

        private static bool TryPatchFileOnDisk(string path, List<KeyValuePair<byte[], byte[]>> pairs, byte[] emuRaw, string emuToken)
        {
            string name = System.IO.Path.GetFileName(path);
            try
            {
                byte[] bytes = System.IO.File.ReadAllBytes(path);
                int patched = PatchBuffer(bytes, pairs);
                if (patched > 0)
                {
                    string backupPath = GetDiskPatchBackupPath(path);
                    if (!System.IO.File.Exists(backupPath))
                    {
                        System.IO.File.Copy(path, backupPath, false);
                    }

                    System.IO.File.WriteAllBytes(path, bytes);
                    Write($"SDR disk CA patched in {name} ({patched} occurrence(s)); backup {System.IO.Path.GetFileName(backupPath)}");
                    return true;
                }

                if (ContainsEmulatorCa(bytes, emuRaw, emuToken))
                {
                    Write($"SDR disk CA already patched in {name}");
                    return true;
                }

                Write($"SDR disk CA pattern not found in {name}");
            }
            catch (Exception ex)
            {
                Write($"SDR disk patch failed for {name}: {ex.Message}");
            }

            return false;
        }

        private static string GetDiskPatchBackupPath(string path)
        {
            string directory = System.IO.Path.GetDirectoryName(path);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            string extension = System.IO.Path.GetExtension(path);
            return System.IO.Path.Combine(directory, fileName + ".skynet-prepatch" + extension);
        }

        private static int PatchBuffer(byte[] buffer, List<KeyValuePair<byte[], byte[]>> pairs)
        {
            int patched = 0;
            foreach (var pair in pairs)
            {
                if (pair.Key.Length != pair.Value.Length)
                {
                    Write($"SDR disk patch pair size mismatch ({pair.Key.Length} != {pair.Value.Length})");
                    continue;
                }

                patched += ReplaceInBuffer(buffer, pair.Key, pair.Value);
            }

            return patched;
        }

        private static int ReplaceInBuffer(byte[] buffer, byte[] search, byte[] replacement)
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
                    Array.Copy(replacement, 0, buffer, i, replacement.Length);
                    patched++;
                    i += search.Length;
                }
                else
                {
                    i++;
                }
            }

            return patched;
        }

        private static bool ContainsEmulatorCa(byte[] buffer, byte[] emuRaw, string emuToken)
        {
            bool hasEmulatorKey = ContainsPattern(buffer, Ascii(emuToken)) || ContainsPattern(buffer, emuRaw);
            bool hasEmulatorId = ContainsPattern(buffer, Ascii(EmulatorCaKeyIdComment));
            return hasEmulatorKey && hasEmulatorId;
        }

        private static bool ContainsPattern(byte[] buffer, byte[] pattern)
        {
            if (buffer.Length < pattern.Length)
            {
                return false;
            }

            int limit = buffer.Length - pattern.Length;
            for (int i = 0; i <= limit; i++)
            {
                if (Matches(buffer, i, pattern))
                {
                    return true;
                }
            }

            return false;
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
    }
}
