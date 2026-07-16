using SKYNET;
using SKYNET.Managers;
using SKYNET.Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace SKYNET.Helper
{
    public class Settings
    {
        private static INIParser IniParser;

        public static void Load()
        {
            // Verify Paths
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "SKYNET"));
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "SKYNET", "Storage"));
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "SKYNET", "AvatarCache"));

            try
            {
                string fileName = Path.Combine(Common.GetPath(), "SKYNET", "steam_api.ini");

                // Migrate an older "[SKYNET] steam_api.ini" to the new name so
                // existing installs keep their configuration.
                string legacyFileName = Path.Combine(Common.GetPath(), "SKYNET", "[SKYNET] steam_api.ini");
                if (!File.Exists(fileName) && File.Exists(legacyFileName))
                {
                    try { File.Move(legacyFileName, fileName); }
                    catch { }
                }

                if (!File.Exists(fileName))
                {
                    StringBuilder config = new StringBuilder();

                    // User Configuration

                    config.AppendLine("[User Settings]");
                    config.AppendLine($"ClientInstanceId = {GenerateClientInstanceId()}");
                    config.AppendLine($"FallbackPersonaName = {Environment.UserName}");
                    config.AppendLine($"FallbackAccountId = {GenerateStableAccountId()}");
                    config.AppendLine();

                    config.AppendLine("[Game Settings]");
                    config.AppendLine($"Languaje = english");
                    config.AppendLine($"AppId = 570");
                    config.AppendLine("# Report every DLC as owned and installed (default). Set to false and list the");
                    config.AppendLine("# owned DLC in the [DLC] section below (or in a \"DLC.txt\" file) to restrict.");
                    config.AppendLine("UnlockAllDLC = true");
                    config.AppendLine();

                    config.AppendLine("[DLC]");
                    config.AppendLine("# Optional list of owned DLC, one entry per line: <AppId> = <Name>");
                    config.AppendLine("# Only enforced when UnlockAllDLC = false. Example:");
                    config.AppendLine("# 247175 = Left 4 Dead 2 - The Passing");
                    config.AppendLine();

                    // Network Configuration

                    config.AppendLine("[Network Settings]");
                    config.AppendLine("# When the emulator is in LAN mode (without dedicated server) it sends and receives data through broadcast ");
                    config.AppendLine("UseServerApi = true");
                    config.AppendLine("# Enable SKYNET-signed SDR certificates and patch the native SDR CA.");
                    config.AppendLine("# Disabled by default for unauthenticated LAN play. Restart the game after changing it.");
                    config.AppendLine("SecureNetworking = false");
                    config.AppendLine("ServerUrl = http://127.0.0.1:27080/");
                    config.AppendLine("PollIntervalMs = 50");
                    config.AppendLine("HttpTimeoutMs = 8000");
                    config.AppendLine("DiscoveryPort = 27081");
                    config.AppendLine("UseActiveWebUser = true");
                    config.AppendLine("BroadCastPort = 28032");
                    config.AppendLine();

                    // Log Configuration

                    config.AppendLine("[Log Settings]");
                    config.AppendLine("File = false");
                    config.AppendLine("Console = false");
                    config.AppendLine();

                    File.WriteAllText(fileName, config.ToString());
                }

                IniParser = new INIParser();
                IniParser.Load(fileName);
                bool configChanged = EnsureMissingDefaults();

                SteamEmulator.PersonaName = GetString("User Settings", "FallbackPersonaName", Environment.UserName);
                SteamEmulator.Language = (string)IniParser["Game Settings"]["Languaje"];
                SteamEmulator.ClientInstanceId = GetString("User Settings", "ClientInstanceId", GenerateClientInstanceId());
                var fallbackAccountId = GetUInt("User Settings", "FallbackAccountId", GenerateStableAccountId());
                SteamEmulator.SteamID = new CSteamID(fallbackAccountId, Steamworks.EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeIndividual);

                foreach (var item in IniParser["Game Settings"].Settings)
                    if (item.Key == "AppId")
                        if (uint.TryParse((string)item.Value, out uint appId))
                            SteamEmulator.AppID = appId;

                LoadDLCs();

                SteamEmulator.SendLog = (bool)IniParser["Log Settings"]["File"];
                SteamEmulator.LogToFile = SteamEmulator.SendLog;

                SteamEmulator.ConsoleLog = (bool)IniParser["Log Settings"]["Console"];
                SteamEmulator.LogToConsole = SteamEmulator.ConsoleLog;

                foreach (var item in IniParser["Network Settings"].Settings)
                {
                    switch (item.Key)
                    {
                        case "SecureNetworking":
                            if (bool.TryParse(item.Value.ToString(), out bool secureNetworking))
                            {
                                SteamEmulator.SecureNetworking = secureNetworking;
                            }
                            break;
                        case "UseServerApi":
                            if (bool.TryParse(item.Value.ToString(), out bool useServerApi))
                            {
                                SteamEmulator.UseServerApi = useServerApi;
                            }
                            break;
                        case "ServerUrl":
                        case "SkyNetServerUrl":
                            SteamEmulator.ServerUrl = item.Value.ToString();
                            break;
                        case "PollIntervalMs":
                        case "SkyNetPollIntervalMs":
                            if (int.TryParse(item.Value.ToString(), out int pollIntervalMs))
                            {
                                SteamEmulator.PollIntervalMs = Math.Max(10, pollIntervalMs);
                            }
                            break;
                        case "HttpTimeoutMs":
                        case "SkyNetHttpTimeoutMs":
                            if (int.TryParse(item.Value.ToString(), out int timeoutMs))
                            {
                                SteamEmulator.HttpTimeoutMs = Math.Max(250, timeoutMs);
                            }
                            break;
                        case "DiscoveryPort":
                        case "SkyNetDiscoveryPort":
                            if (int.TryParse(item.Value.ToString(), out int discoveryPort))
                            {
                                SteamEmulator.DiscoveryPort = discoveryPort;
                            }
                            break;
                        case "AccessToken":
                        case "SkyNetAccessToken":
                            SteamEmulator.AccessToken = item.Value.ToString();
                            break;
                        case "RefreshToken":
                        case "SkyNetRefreshToken":
                            SteamEmulator.RefreshToken = item.Value.ToString();
                            break;
                        case "UseActiveWebUser":
                        case "SkyNetUseActiveWebUser":
                            if (bool.TryParse(item.Value.ToString(), out bool useActiveWebUser))
                            {
                                SteamEmulator.UseActiveWebUser = useActiveWebUser;
                            }
                            break;
                        case "BroadCastPort":
                            if (int.TryParse(item.Value.ToString(), out int broadCastPort))
                            {
                                SteamEmulator.BroadCastPort = broadCastPort;
                            }
                            break;
                    }
                }

                if ((ulong)SteamEmulator.SteamID == 0)
                {
                    var accountId = GenerateStableAccountId();
                    SteamEmulator.SteamID = new CSteamID(accountId, Steamworks.EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeIndividual);
                    IniParser["User Settings"]["FallbackAccountId"] = accountId.ToString();
                    configChanged = true;
                }

                if (configChanged)
                {
                    IniParser.Save();
                }

                if (SteamEmulator.ConsoleLog)
                {
                    Common.ActiveConsoleOutput();
                }

                string data = $"Loaded fallback user data from file \n PersonaName: {SteamEmulator.PersonaName} \n SteamId:  {SteamEmulator.SteamID} \n Languaje: {SteamEmulator.Language} \n";
                SteamEmulator.Write("Settings", data);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"SKYNET settings load failed: {ex.Message}");
            }

        }

        private static bool EnsureMissingDefaults()
        {
            bool changed = false;
            changed |= EnsureSetting("Game Settings", "UnlockAllDLC", "true");
            changed |= EnsureSetting("User Settings", "ClientInstanceId", GenerateClientInstanceId());
            changed |= MigrateSettingName("User Settings", "PersonaName", "FallbackPersonaName", Environment.UserName);
            changed |= MigrateSettingName("User Settings", "AccountId", "FallbackAccountId", GenerateStableAccountId().ToString());
            changed |= EnsureSetting("User Settings", "FallbackPersonaName", Environment.UserName);
            changed |= EnsureSetting("User Settings", "FallbackAccountId", GenerateStableAccountId().ToString());

            // Drop the "SkyNet" prefix from the network keys, preserving values.
            changed |= MigrateSettingName("Network Settings", "SkyNetServerUrl", "ServerUrl", "http://127.0.0.1:27080/");
            changed |= MigrateSettingName("Network Settings", "SkyNetPollIntervalMs", "PollIntervalMs", "50");
            changed |= MigrateSettingName("Network Settings", "SkyNetHttpTimeoutMs", "HttpTimeoutMs", "8000");
            changed |= MigrateSettingName("Network Settings", "SkyNetDiscoveryPort", "DiscoveryPort", "27081");
            changed |= MigrateSettingName("Network Settings", "SkyNetUseActiveWebUser", "UseActiveWebUser", "true");

            changed |= EnsureSetting("Network Settings", "PollIntervalMs", "50");
            changed |= EnsureSetting("Network Settings", "SecureNetworking", "false");
            changed |= EnsureSetting("Network Settings", "HttpTimeoutMs", "8000");
            changed |= EnsureSetting("Network Settings", "DiscoveryPort", "27081");
            changed |= EnsureSetting("Network Settings", "UseActiveWebUser", "true");
            changed |= EnsureSetting("Network Settings", "BroadCastPort", "28032");
            changed |= MigrateSetting("Network Settings", "PollIntervalMs", "1000", "50");
            changed |= MigrateSetting("Network Settings", "HttpTimeoutMs", "2000", "8000");
            changed |= MigrateSetting("Network Settings", "BroadCastPort", "28025", "28032");
            changed |= RemoveSetting("Network Settings", "VacSecureGameServer");
            changed |= RemoveSetting("Network Settings", "ServerIP");
            changed |= RemoveSetting("Network Settings", "AccessToken");
            changed |= RemoveSetting("Network Settings", "RefreshToken");
            changed |= RemoveSetting("Network Settings", "SkyNetAccessToken");
            changed |= RemoveSetting("Network Settings", "SkyNetRefreshToken");
            return changed;
        }

        private static bool EnsureSetting(string section, string key, string value)
        {
            if (IniParser[section][key] != null)
            {
                return false;
            }

            IniParser[section][key] = value;
            return true;
        }

        private static bool MigrateSetting(string section, string key, string oldValue, string newValue)
        {
            var value = IniParser[section][key];
            if (value == null || !string.Equals(value.ToString(), oldValue, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            IniParser[section][key] = newValue;
            return true;
        }

        private static bool MigrateSettingName(string section, string oldKey, string newKey, string fallback)
        {
            bool changed = false;
            var oldValue = IniParser[section][oldKey];
            if (IniParser[section][newKey] == null)
            {
                IniParser[section][newKey] = oldValue == null ? fallback : oldValue.ToString();
                changed = true;
            }

            return RemoveSetting(section, oldKey) || changed;
        }

        private static bool RemoveSetting(string section, string key)
        {
            var settings = IniParser[section].Settings;
            int removed = settings.RemoveAll(s => string.Equals(s.Key, key, StringComparison.OrdinalIgnoreCase));
            return removed > 0;
        }

        private static bool GetBool(string section, string key, bool fallback)
        {
            var value = IniParser[section][key];
            if (value != null && bool.TryParse(value.ToString(), out var parsed))
            {
                return parsed;
            }

            return fallback;
        }

        /// <summary>
        /// Loads DLC ownership configuration:
        ///  - [Game Settings] UnlockAllDLC toggles reporting every DLC as owned (default true).
        ///  - The optional [DLC] ini section and a "DLC.txt" file list owned DLC as
        ///    "AppId = Name". If the DLC.txt file exists, unlock-all is forced off so only the
        ///    listed DLC are reported.
        /// </summary>
        private static void LoadDLCs()
        {
            try
            {
                DLCManager.Clear();
                DLCManager.UnlockAll = GetBool("Game Settings", "UnlockAllDLC", true);

                // DLC entries declared inline in the ini [DLC] section.
                foreach (var item in IniParser["DLC"].Settings)
                {
                    AddDLCEntry(item.Key, item.Value == null ? string.Empty : item.Value.ToString());
                }

                // DLC entries declared in an external plain-text file.
                string dlcFile = Path.Combine(Common.GetPath(), "SKYNET", "DLC.txt");
                string legacyDlcFile = Path.Combine(Common.GetPath(), "SKYNET", "[SKYNET] DLC.txt");
                if (!File.Exists(dlcFile) && File.Exists(legacyDlcFile))
                {
                    try { File.Move(legacyDlcFile, dlcFile); }
                    catch { }
                }
                if (File.Exists(dlcFile))
                {
                    DLCManager.UnlockAll = false;
                    foreach (var rawLine in File.ReadAllLines(dlcFile))
                    {
                        string line = rawLine.Trim();
                        if (line.Length == 0 || line[0] == '#' || line[0] == ';')
                        {
                            continue;
                        }

                        int separator = line.IndexOf('=');
                        if (separator <= 0)
                        {
                            continue;
                        }

                        AddDLCEntry(line.Substring(0, separator).Trim(), line.Substring(separator + 1).Trim());
                    }
                }

                SteamEmulator.Write("Settings", $"Loaded DLC config: UnlockAll = {DLCManager.UnlockAll}, Count = {DLCManager.Count}");
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("Settings", $"Failed to load DLC config: {ex.Message}");
            }
        }

        private static void AddDLCEntry(string appIdText, string name)
        {
            if (string.IsNullOrWhiteSpace(appIdText))
            {
                return;
            }

            if (uint.TryParse(appIdText.Trim(), out uint appId) && appId != 0)
            {
                DLCManager.AddOrUpdate(appId, name == null ? string.Empty : name.Trim(), true);
            }
        }

        private static string GetString(string section, string key, string fallback)
        {
            var value = IniParser[section][key];
            return value == null ? fallback : value.ToString();
        }

        private static uint GetUInt(string section, string key, uint fallback)
        {
            var value = IniParser[section][key];
            if (value != null && uint.TryParse(value.ToString(), out var parsed))
            {
                return parsed;
            }

            return fallback;
        }

        private static uint GenerateStableAccountId()
        {
            var hash = ComputeHash($"{Environment.MachineName}|{Environment.UserName}|{Common.GetPath()}|SKYNET_ACCOUNT");
            uint accountId = BitConverter.ToUInt32(hash, 0);
            if (accountId < 100000)
            {
                accountId += 100000;
            }

            return accountId;
        }

        private static string GenerateClientInstanceId()
        {
            var hash = ComputeHash($"{Environment.MachineName}|{Environment.UserName}|{Common.GetPath()}|SKYNET_INSTANCE");
            return BitConverter.ToString(hash, 0, 16).Replace("-", "").ToLowerInvariant();
        }

        private static byte[] ComputeHash(string value)
        {
            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(value ?? string.Empty));
            }
        }
    }
}
