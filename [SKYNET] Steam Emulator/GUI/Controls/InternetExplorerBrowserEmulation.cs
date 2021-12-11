using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;

namespace SKYNET
{
    internal sealed class InternetExplorerBrowserEmulation
    {
        private static string InternetExplorerRootKey = "Software\\Microsoft\\Internet Explorer";
        private static string BrowserEmulationKey = InternetExplorerRootKey + "\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION";

        public static BrowserEmulationVersion GetBrowserEmulationVersion()
        {
            BrowserEmulationVersion emulationVersion = BrowserEmulationVersion.Default;
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(BrowserEmulationKey, true);
                if (registryKey != null)
                {
                    string fileName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
                    object objectValue = RuntimeHelpers.GetObjectValue(registryKey.GetValue(fileName, (object)null));
                    if (objectValue != null)
                        emulationVersion = (BrowserEmulationVersion)Convert.ToInt32(RuntimeHelpers.GetObjectValue(objectValue));
                }
            }
            catch (SecurityException ex)
            {
                ProjectData.SetProjectError((Exception)ex);
                ProjectData.ClearProjectError();
            }
            catch (UnauthorizedAccessException ex)
            {
                ProjectData.SetProjectError((Exception)ex);
                ProjectData.ClearProjectError();
            }
            return emulationVersion;
        }

        public static int GetInternetExplorerMajorVersion()
        {
            int result = 0;
            try
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(InternetExplorerRootKey);
                if (registryKey != null)
                {
                    object objectValue = RuntimeHelpers.GetObjectValue(registryKey.GetValue("svcVersion", (object)null) ?? registryKey.GetValue("Version", (object)null));
                    if (objectValue != null)
                    {
                        string str = objectValue.ToString();
                        int length = str.IndexOf('.');
                        if (length != -1)
                            int.TryParse(str.Substring(0, length), out result);
                    }
                }
            }
            catch (SecurityException ex)
            {
                ProjectData.SetProjectError((Exception)ex);
                ProjectData.ClearProjectError();
            }
            catch (UnauthorizedAccessException ex)
            {
                ProjectData.SetProjectError((Exception)ex);
                ProjectData.ClearProjectError();
            }
            return result;
        }

        public static bool IsBrowserEmulationSet()
        {
            return (uint)GetBrowserEmulationVersion() > 0U;
        }

        public static bool SetBrowserEmulationVersion(BrowserEmulationVersion browserEmulationVersion__1)
        {
            bool flag = false;
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(BrowserEmulationKey, true);
                if (registryKey == null)
                {
                    Registry.CurrentUser.CreateSubKey(BrowserEmulationKey).Close();
                    registryKey = Registry.CurrentUser.OpenSubKey(BrowserEmulationKey, true);
                }
                if (registryKey != null)
                {
                    string fileName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
                    if (browserEmulationVersion__1 != BrowserEmulationVersion.Default)
                        registryKey.SetValue(fileName, (object)browserEmulationVersion__1, RegistryValueKind.DWord);
                    else
                        registryKey.DeleteValue(fileName, false);
                    flag = true;
                }
            }
            catch (SecurityException ex)
            {
                ProjectData.SetProjectError((Exception)ex);
                ProjectData.ClearProjectError();
            }
            catch (UnauthorizedAccessException ex)
            {
                ProjectData.SetProjectError((Exception)ex);
                ProjectData.ClearProjectError();
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                ProjectData.ClearProjectError();
            }
            return flag;
        }
        public static short IE_VERSION;

        public static bool SetBrowserEmulationVersion()
        {
            int explorerMajorVersion = GetInternetExplorerMajorVersion();
            IE_VERSION = checked((short)explorerMajorVersion);
            BrowserEmulationVersion browserEmulationVersion__1;
            if (explorerMajorVersion >= 11)
            {
                browserEmulationVersion__1 = BrowserEmulationVersion.Version11;
            }
            else
            {
                switch (explorerMajorVersion - 8)
                {
                    case 0:
                        browserEmulationVersion__1 = BrowserEmulationVersion.Version8;
                        break;
                    case 1:
                        browserEmulationVersion__1 = BrowserEmulationVersion.Version9;
                        break;
                    case 2:
                        browserEmulationVersion__1 = BrowserEmulationVersion.Version10;
                        break;
                    default:
                        browserEmulationVersion__1 = BrowserEmulationVersion.Version7;
                        break;
                }
            }
            return SetBrowserEmulationVersion(browserEmulationVersion__1);
        }
    }
    public enum BrowserEmulationVersion
    {
        Default = 0,
        Version7 = 7000,
        Version8 = 8000,
        Version8Standards = 8888,
        Version9 = 9000,
        Version9Standards = 9999,
        Version10 = 10000,
        Version10Standards = 10001,
        Version11 = 11000,
        Version11Edge = 11001,
    }

}