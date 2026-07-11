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
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "SKYNET"));
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "SKYNET", "Storage"));
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "SKYNET", "AvatarCache"));

            try
            {
                string fileName = Path.Combine(modCommon.GetPath(), "SKYNET", "[SKYNET] steam_api.ini");

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
                    config.AppendLine("# owned DLC in the [DLC] section below (or in a \"[SKYNET] DLC.txt\" file) to restrict.");
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
                    config.AppendLine("SkyNetServerUrl = http://127.0.0.1:27080/");
                    config.AppendLine("SkyNetPollIntervalMs = 50");
                    config.AppendLine("SkyNetHttpTimeoutMs = 8000");
                    config.AppendLine("SkyNetDiscoveryPort = 27081");
                    config.AppendLine("SkyNetUseActiveWebUser = true");
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
                SteamEmulator.SkyNetClientInstanceId = GetString("User Settings", "ClientInstanceId", GenerateClientInstanceId());
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
                        case "UseServerApi":
                            if (bool.TryParse(item.Value.ToString(), out bool useServerApi))
                            {
                                SteamEmulator.UseServerApi = useServerApi;
                            }
                            break;
                        case "SkyNetServerUrl":
                            SteamEmulator.SkyNetServerUrl = item.Value.ToString();
                            break;
                        case "SkyNetPollIntervalMs":
                            if (int.TryParse(item.Value.ToString(), out int pollIntervalMs))
                            {
                                SteamEmulator.SkyNetPollIntervalMs = Math.Max(10, pollIntervalMs);
                            }
                            break;
                        case "SkyNetHttpTimeoutMs":
                            if (int.TryParse(item.Value.ToString(), out int timeoutMs))
                            {
                                SteamEmulator.SkyNetHttpTimeoutMs = Math.Max(250, timeoutMs);
                            }
                            break;
                        case "SkyNetDiscoveryPort":
                            if (int.TryParse(item.Value.ToString(), out int discoveryPort))
                            {
                                SteamEmulator.SkyNetDiscoveryPort = discoveryPort;
                            }
                            break;
                        case "SkyNetAccessToken":
                            SteamEmulator.SkyNetAccessToken = item.Value.ToString();
                            break;
                        case "SkyNetRefreshToken":
                            SteamEmulator.SkyNetRefreshToken = item.Value.ToString();
                            break;
                        case "SkyNetUseActiveWebUser":
                            if (bool.TryParse(item.Value.ToString(), out bool useActiveWebUser))
                            {
                                SteamEmulator.SkyNetUseActiveWebUser = useActiveWebUser;
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
                    modCommon.ActiveConsoleOutput();
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
            changed |= EnsureSetting("Network Settings", "SkyNetPollIntervalMs", "50");
            changed |= EnsureSetting("Network Settings", "SkyNetHttpTimeoutMs", "8000");
            changed |= EnsureSetting("Network Settings", "SkyNetDiscoveryPort", "27081");
            changed |= EnsureSetting("Network Settings", "SkyNetUseActiveWebUser", "true");
            changed |= EnsureSetting("Network Settings", "BroadCastPort", "28032");
            changed |= MigrateSetting("Network Settings", "SkyNetPollIntervalMs", "1000", "50");
            changed |= MigrateSetting("Network Settings", "SkyNetHttpTimeoutMs", "2000", "8000");
            changed |= MigrateSetting("Network Settings", "BroadCastPort", "28025", "28032");
            changed |= RemoveSetting("Network Settings", "ServerIP");
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
        ///  - The optional [DLC] ini section and a "[SKYNET] DLC.txt" file list owned DLC as
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
                string dlcFile = Path.Combine(modCommon.GetPath(), "SKYNET", "[SKYNET] DLC.txt");
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
            var hash = ComputeHash($"{Environment.MachineName}|{Environment.UserName}|{modCommon.GetPath()}|SKYNET_ACCOUNT");
            uint accountId = BitConverter.ToUInt32(hash, 0);
            if (accountId < 100000)
            {
                accountId += 100000;
            }

            return accountId;
        }

        private static string GenerateClientInstanceId()
        {
            var hash = ComputeHash($"{Environment.MachineName}|{Environment.UserName}|{modCommon.GetPath()}|SKYNET_INSTANCE");
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
