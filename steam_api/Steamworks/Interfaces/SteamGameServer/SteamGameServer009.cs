using System;
using System.Runtime.InteropServices;
using SKYNET.Helpers;

namespace SKYNET.Steamworks.Interfaces
{
    /// <summary>Legacy ISteamGameServer ABI used by Left 4 Dead.</summary>
    [Interface("SteamGameServer009")]
    public class SteamGameServer009 : ISteamInterface
    {
        public void LogOn(IntPtr _)
        {
            SteamEmulator.SteamGameServer.LogOnAnonymous();
        }

        public void LogOff(IntPtr _)
        {
            SteamEmulator.SteamGameServer.LogOff();
        }

        public bool BLoggedOn(IntPtr _)
        {
            return SteamEmulator.SteamGameServer.BLoggedOn();
        }

        public bool BSecure(IntPtr _)
        {
            return SteamEmulator.SteamGameServer.BSecure();
        }

        public IntPtr GetSteamID(IntPtr _, IntPtr pSteamID)
        {
            return NativeSteamId.Write(pSteamID, SteamEmulator.SteamGameServer.GetSteamID());
        }

        public bool SendUserConnectAndAuthenticate(IntPtr _, uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, IntPtr pSteamIDUser)
        {
            return SteamEmulator.SteamGameServer.SendUserConnectAndAuthenticate(unIPClient, pvAuthBlob, cubAuthBlobSize, pSteamIDUser);
        }

        public IntPtr CreateUnauthenticatedUserConnection(IntPtr _, IntPtr pSteamID)
        {
            return NativeSteamId.Write(pSteamID, SteamEmulator.SteamGameServer.CreateUnauthenticatedUserConnection());
        }

        public void SendUserDisconnect(IntPtr _, ulong steamIDUser)
        {
            SteamEmulator.SteamGameServer.SendUserDisconnect(steamIDUser);
        }

        public bool BUpdateUserData(IntPtr _, ulong steamIDUser, IntPtr pchPlayerName, uint uScore)
        {
            SteamEmulator.Write("SteamGameServer009", $"BUpdateUserData name=0x{pchPlayerName.ToInt64():X}");
            return SteamEmulator.SteamGameServer.BUpdateUserData(steamIDUser, ReadAnsi(pchPlayerName), uScore);
        }

        public bool BSetServerType(
            IntPtr _,
            uint unServerFlags,
            uint unGameIP,
            ushort unGamePort,
            ushort unSpectatorPort,
            ushort usQueryPort,
            IntPtr pchGameDir,
            IntPtr pchVersion,
            bool bLANMode)
        {
            SteamEmulator.Write(
                "SteamGameServer009",
                $"BSetServerType flags={unServerFlags} ip={unGameIP} gamePort={unGamePort} spectatorPort={unSpectatorPort} queryPort={usQueryPort} gameDir=0x{pchGameDir.ToInt64():X} version=0x{pchVersion.ToInt64():X} lan={bLANMode}");
            bool result = SteamEmulator.SteamGameServer.InitGameServer(
                unGameIP,
                unGamePort,
                usQueryPort,
                unServerFlags,
                SteamEmulator.InternalAppId,
                ReadAnsi(pchVersion));
            SteamEmulator.SteamGameServer.SetModDir(ReadAnsi(pchGameDir));
            SteamEmulator.SteamGameServer.SetSpectatorPort(unSpectatorPort);
            return result;
        }

        public void UpdateServerStatus(
            IntPtr _,
            int cPlayers,
            int cPlayersMax,
            int cBotPlayers,
            IntPtr pchServerName,
            IntPtr pSpectatorServerName,
            IntPtr pchMapName)
        {
            SteamEmulator.Write(
                "SteamGameServer009",
                $"UpdateServerStatus serverName=0x{pchServerName.ToInt64():X} spectatorName=0x{pSpectatorServerName.ToInt64():X} map=0x{pchMapName.ToInt64():X}");
            SteamEmulator.SteamGameServer.SetMaxPlayerCount(cPlayersMax);
            SteamEmulator.SteamGameServer.SetBotPlayerCount(cBotPlayers);
            SteamEmulator.SteamGameServer.SetServerName(ReadAnsi(pchServerName));
            SteamEmulator.SteamGameServer.SetSpectatorServerName(ReadAnsi(pSpectatorServerName));
            SteamEmulator.SteamGameServer.SetMapName(ReadAnsi(pchMapName));
        }

        public void UpdateSpectatorPort(IntPtr _, ushort unSpectatorPort)
        {
            SteamEmulator.SteamGameServer.SetSpectatorPort(unSpectatorPort);
        }

        public void SetGameType(IntPtr _, IntPtr pchGameType)
        {
            SteamEmulator.Write("SteamGameServer009", $"SetGameType value=0x{pchGameType.ToInt64():X}");
            SteamEmulator.SteamGameServer.SetGameTags(ReadAnsi(pchGameType));
        }

        public bool BGetUserAchievementStatus(IntPtr _, ulong steamID, IntPtr pchAchievementName)
        {
            SteamEmulator.Write("SteamGameServer009", $"BGetUserAchievementStatus name=0x{pchAchievementName.ToInt64():X}");
            return false;
        }

        public void GetGameplayStats(IntPtr _)
        {
            SteamEmulator.SteamGameServer.GetGameplayStats();
        }

        public bool RequestUserGroupStatus(IntPtr _, ulong steamIDUser, ulong steamIDGroup)
        {
            return SteamEmulator.SteamGameServer.RequestUserGroupStatus(steamIDUser, steamIDGroup);
        }

        public uint GetPublicIP_old(IntPtr _)
        {
            return SteamEmulator.SteamGameServer.GetPublicIP_old();
        }

        public void SetGameData(IntPtr _, IntPtr pchGameData)
        {
            SteamEmulator.Write("SteamGameServer009", $"SetGameData value=0x{pchGameData.ToInt64():X}");
            SteamEmulator.SteamGameServer.SetGameData(ReadAnsi(pchGameData));
        }

        public int UserHasLicenseForApp(IntPtr _, ulong steamID, uint appID)
        {
            return SteamEmulator.SteamGameServer.UserHasLicenseForApp(steamID, appID);
        }

        private static string ReadAnsi(IntPtr value)
        {
            long address = value.ToInt64();
            if (address == 0)
            {
                return string.Empty;
            }
            if (address > 0 && address < 0x10000)
            {
                return $"<invalid:0x{address:X}>";
            }

            try
            {
                return Marshal.PtrToStringAnsi(value) ?? string.Empty;
            }
            catch
            {
                return $"<invalid:0x{address:X}>";
            }
        }
    }
}
