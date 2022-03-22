using System;

using Core;
using Core.Interface;

using System.Runtime.InteropServices;

namespace InterfaceUser
{
    [Impl(Name = "SteamUser019", ServerMapped = true)]
    public class SteamUser019 : IBaseInterface
    {
        public SteamUser019()
        {
            

        }

        public int GetHSteamUser()
        {
            Write("GetHSteamUser");
            // Make sure to return the handle not the id
            return (int)SteamEmulator.HSteamUser;
        }

        public bool LoggedOn()
        {
            Write("LoggedOn");
            return true;
        }

        public ulong GetSteamID()
        {
            Write("GetSteamID");
            return SteamEmulator.SteamId;
        }

        public int InitiateGameConnection(IntPtr blob, uint blob_count, ulong gameserver_id, uint server_ip, ushort server_port, bool secure)
        {
            Write("InitiateGameConnection");
            return 0;
        }

        public void TerminateGameConnection(uint server_ip, ushort server_port)
        {
            Write("TerminateGameConnection");
        }

        public void TrackAppUsageEvent(ulong game_id, int usage_event, string extra_info)
        {
            Write("TrackAppUsageEvent");
        }

        public bool GetUserDataFolder(string buffer, int count)
        {
            Write("GetUserDataFolder");
            return false;
        }

        public void StartVoiceRecording()
        {
            Write("StartVoiceRecording");
        }

        public void StopVoiceRecording()
        {
            Write("StopVoiceRecording");

        }

        public uint GetAvailableVoice(uint[] compressed_data, uint[] uncompressed, uint desired_sample_rate)
        {
            Write("GetAvailableVoice");
            return 0;
        }

        public int GetVoice(bool want_compressed, IntPtr dest_buffer, uint dest_buffer_size, ref uint compressed_bytes_written,
                            bool wants_uncompressed, IntPtr uncompressed_dest, uint uncompressed_buffer_size,
                            ref uint bytes_written, uint uncompressed_desired_samplerate)
        {
            Write("GetVoice");
            return 0;
        }

        public int DecompressVoice(IntPtr compressed, uint compressed_size, IntPtr dest_buffer, uint dest_size, ref uint bytes_written, uint sample_rate)
        {
            Write("DecompressVoice");
            return 0;
        }

        public uint GetOptimalSampleRate()
        {
            Write("GetOptimalSampleRate");
            return 0;
        }

        public int GetAuthSessionTicket(IntPtr ticket, uint ticket_size, ref int ticket_written)
        {
            Write("GetAuthSessionTicket");

            //var app_id = u.GetAppIdForPipe(PipeId);

            //var new_ticket = u.GetAuthTicket(u.GetAuthSessionTicket(app_id, PipeId));

            //if (new_ticket.ticket.Length > ticket_size)
            //{
            //    Write("AuthTicket length is bigger than buffer allocated for it!");
            //    return -1; // Invalid ticket
            //}

            //// Write the ticket out
            //Marshal.Copy(new_ticket.ticket, 0, ticket, new_ticket.ticket.Length);
            //ticket_written = new_ticket.ticket.Length;

            return 0;
        }

        public uint BeginAuthSession(IntPtr ticket, uint ticket_size, ulong steamid)
        {
            Write("BeginAuthSession");
            return 0;
        }

        public void EndAuthSession(ulong steam_id)
        {
            Write("EndAuthSession");
        }

        public void CancelAuthTicket(int ticket_handle)
        {
            Write("CancelAuthTicket");
        }

        public uint UserHasLicenseForApp(ulong steamID, uint appID)
        {
            Write("UserHasLicenseForApp");
            return 0;
        }

        bool IsBehindNAT()
        {
            Write("IsBehindNAT");
            return false;
        }

        public void AdvertiseGame(ulong game_server_id, uint server_ip, ushort server_port)
        {
            Write("AdvertiseGame");
        }

        public uint RequestEncryptedAppTicket(IntPtr data_to_include, uint data_size)
        {
            Write("RequestEncryptedAppTicket");
            return 0;
        }

        public int GetGameBadgeLevel(int seris, bool foil)
        {
            Write("GetGameBadgeLevel");
            return 0;
        }

        public int GetSteamLevel()
        {
            Write("GetSteamLevel");
            return 0;
        }

        public uint RequestStoreAuthURL(string redirect_url)
        {
            Write("RequestStoreAuthURL");
            return 0;
        }

        public bool IsPhoneVerified()
        {
            Write("IsPhoneVerified");
            return false;
        }

        public bool IsTwoFactorEnabled()
        {
            Write("IsTwoFactorEnabled");
            return false;
        }

        public bool IsPhoneIdentifying()
        {
            Write("IsPhoneIdentifying");
            return false;
        }

        public bool IsPhoneRequiringVerification()
        {
            Write("IsPhoneRequiringVerification");
            return false;
        }
    }
}
