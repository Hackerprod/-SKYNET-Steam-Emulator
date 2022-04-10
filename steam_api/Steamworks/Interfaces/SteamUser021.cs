using Steamworks;
using System;

namespace SKYNET.Interface
{
    [Interface("SteamUser021")]
    public class SteamUser021 : ISteamInterface
    {
        public int GetHSteamUser(IntPtr _)
        {
            return (int)SteamEmulator.SteamUser.GetHSteamUser(_);
        }
        public bool LoggedOn(IntPtr _)
        {
            return SteamEmulator.SteamUser.BLoggedOn(_);
        }
        public ulong GetSteamID(IntPtr _)
        {
            return (ulong)SteamEmulator.SteamUser.GetSteamID(_);
        }
        public int InitiateGameConnection(IntPtr _, IntPtr blob, uint blob_count, ulong gameserver_id, uint server_ip, short server_port, bool secure)
        {
            return SteamEmulator.SteamUser.InitiateGameConnection(_, blob, (int)blob_count, (IntPtr)gameserver_id, server_ip, (uint)server_port, secure);
        }
        public void TerminateGameConnection(IntPtr _, uint server_ip, short server_port)
        {
            SteamEmulator.SteamUser.TerminateGameConnection(_, server_ip, (uint)server_port);
        }
        public void TrackAppUsageEvent(IntPtr _, ulong game_id, int usage_event, string extra_info)
        {
            SteamEmulator.SteamUser.TrackAppUsageEvent(_, (IntPtr)game_id, usage_event, extra_info);
        }
        public bool GetUserDataFolder(IntPtr _, string buffer, int count)
        {
            return SteamEmulator.SteamUser.GetUserDataFolder(_, buffer, count);
        }
        public void StartVoiceRecording(IntPtr _)
        {
            SteamEmulator.SteamUser.StartVoiceRecording(_);
        }
        public void StopVoiceRecording(IntPtr _)
        {
            SteamEmulator.SteamUser.StopVoiceRecording(_);
        }
        public uint GetAvailableVoice(IntPtr _, System.UInt32[] compressed_data, System.UInt32[] uncompressed, uint desired_sample_rate)
        {
            //return SteamEmulator.SteamUser.GetAvailableVoice(_, new uint[0], );
            return 0;
        }
        public int GetVoice(IntPtr _, bool want_compressed, IntPtr dest_buffer, uint dest_buffer_size, ref uint compressed_bytes_written, bool wants_uncompressed, IntPtr uncompressed_dest, uint uncompressed_buffer_size, ref uint bytes_written, uint uncompressed_desired_samplerate)
        {
            //return SteamEmulator.SteamUser.GetVoice(_);
            return 0;
        }
        public int DecompressVoice(IntPtr _, IntPtr compressed, uint compressed_size, IntPtr dest_buffer, uint dest_size, ref uint bytes_written, uint sample_rate)
        {
            //return SteamEmulator.SteamUser.DecompressVoice(_);
            return 0;
        }
        public uint GetOptimalSampleRate(IntPtr _)
        {
            // return SteamEmulator.SteamUser.GetOptimalSampleRate(_);
            return 0;
        }
        public int GetAuthSessionTicket(IntPtr _, IntPtr ticket, uint ticket_size, ref int ticket_written)
        {
            return (int)SteamEmulator.SteamUser.GetAuthSessionTicket(_, ticket, (int)ticket_size, (uint)ticket_written);
        }
        public uint BeginAuthSession(IntPtr _, IntPtr ticket, uint ticket_size, ulong steamid)
        {
            return (uint)SteamEmulator.SteamUser.BeginAuthSession(_, ticket, (int)ticket_size, (IntPtr)steamid);
        }
        public void EndAuthSession(IntPtr _, ulong steam_id)
        {
            SteamEmulator.SteamUser.EndAuthSession(_, (IntPtr)steam_id);
        }
        public void CancelAuthTicket(IntPtr _, int ticket_handle)
        {
            SteamEmulator.SteamUser.CancelAuthTicket(_, ticket_handle);
        }
        public uint UserHasLicenseForApp(IntPtr _, ulong steamID, uint appID)
        {
            return (uint)SteamEmulator.SteamUser.UserHasLicenseForApp(_, (IntPtr)steamID, (AppId_t)appID);
        }
        public void AdvertiseGame(IntPtr _, ulong game_server_id, uint server_ip, short server_port)
        {
            SteamEmulator.SteamUser.AdvertiseGame(_, (IntPtr)game_server_id, server_ip, (uint)server_port);
        }
        public uint RequestEncryptedAppTicket(IntPtr _, IntPtr data_to_include, uint data_size)
        {
            return (uint)SteamEmulator.SteamUser.RequestEncryptedAppTicket(_, data_to_include, (int)data_size);
        }
        public int GetGameBadgeLevel(IntPtr _, int seris, bool foil)
        {
            return SteamEmulator.SteamUser.GetGameBadgeLevel(_, seris, foil);
        }
        public int GetSteamLevel(IntPtr _)
        {
            return SteamEmulator.SteamUser.GetPlayerSteamLevel(_);
        }
        public uint RequestStoreAuthURL(IntPtr _, string redirect_url)
        {
            return (uint)SteamEmulator.SteamUser.RequestStoreAuthURL(_, redirect_url).m_SteamAPICall;
        }
        public bool IsPhoneVerified(IntPtr _)
        {
            return SteamEmulator.SteamUser.BIsPhoneVerified(_);
        }
        public bool IsTwoFactorEnabled(IntPtr _)
        {
            return SteamEmulator.SteamUser.BIsTwoFactorEnabled(_);
        }
        public bool IsPhoneIdentifying(IntPtr _)
        {
            return SteamEmulator.SteamUser.BIsPhoneIdentifying(_);
        }
        public bool IsPhoneRequiringVerification(IntPtr _)
        {
            return SteamEmulator.SteamUser.BIsPhoneRequiringVerification(_);
        }
    }
}
