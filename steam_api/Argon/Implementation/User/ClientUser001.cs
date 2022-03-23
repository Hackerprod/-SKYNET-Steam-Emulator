using System;

using Core.Interface;
using SKYNET;

namespace InterfaceUser
{
    [Impl(Name = "CLIENTUSER_INTERFACE_VERSION001", ServerMapped = true)]
    public class ClientUser001 : IBaseInterface
    {
        public ClientUser001()
        {
        }

        public int GetHSteamUser()
        {
            Write("GetHSteamUser");
            return 1;
        }

        public void LogOn(ulong steamid)
        {
            // There is no real analogue of this directly in steamkit
            // so this will need to be reversed slightly further

            // This probably has something to do with anonymous login...
            // or when there is a loginkey that we can use
            Write("Logon(steamid) is not implemented");
        }

        public void LogOnWithPassword(string username, string password)
        {
            Write("LogOnWithPassword");
        }

        public void LogOnAndCreateNewSteamAccountIfNeeded()
        {
            // There is no real analog of this directly in steamkit
            // so this will need to be reverse slightly further

            Write("LogOnAndCreateNewSteamAccountIfNeeded() is not implemented");
        }

        // returns eresult
        public uint LogOnConnectionless()
        {
            return 0;
        }

        public void LogOff()
        {
        }

        public bool BLoggedOn()
        {
            Write("BLoggedOn");
            return true;
        }

        public uint GetLogonState()
        {
            Write("GetLogonState");
            return (uint)1;
        }

        public bool BConnected()
        {
            Write("BConnected");
            return true;
        }

        public bool BTryingToLogon()
        {
            Write("BTryingToLogon");
            return true;
        }

        public ulong GetSteamId()
        {
            return 76159310548620634;
        }

        // This should only be called on consoles...
        // Since we are not targetting that this can just return the normal steamid
        // and print an error
        public ulong GetConsoleSteamId()
        {
            Write("GetConsoleSteamId should never be called!");
            return 76159310548620634;
        }

        public ulong GetClientInstanceId()
        {
            return 1;
        }

        public bool IsVACBanned(uint game)
        {
            // Im not sure how this is implemented in the client
            // CClientJobVACBanStatus2::BYieldingRunClientJob calls CUser::AddBannedGames
            // which fills in an array that contains banned games in some form

            // Calling this function with a gameid of 0 returns whether there are any bans

            return false;
        }

        public bool SetEmail(string new_email)
        {
            // This just sets a string in CUser and has no noticable side-effects

            // Returning false is effectively saying "this email is invalid"
            // so we will need to check whether that is valid behaviour
            return false;
        }

        public bool SetConfigString(uint config_sub_tree, string key, string value)
        {
            return false;
        }
        public bool GetConfigString(uint config_sub_tree, string key, IntPtr value_out, int max_out)
        {
            return false;
        }
        public bool SetConfigInt(uint config_sub_tree, string key, int new_value)
        {
            return false;
        }
        public bool GetConfigInt(uint config_sub_tree, string key, ref int value)
        {
            return false;
        }
        public bool DeleteConfigKey(uint config_sub_tree, string key)
        {
            return false;
        }
        public bool GetConfigStoreKeyName(uint config_sub_tree, string key, IntPtr value_out, int max_out)
        {
            return false;
        }

        public int InitiateGameConnection(IntPtr blob, uint blob_count, ulong gameserver_id, uint server_ip, ushort server_port, bool secure)
        {
            // blob will get filled with a game connect token + the app ownership ticket
            // look in reverse folder for more information on what steam does here

            // because this is part of the old user connection handshake mechanism its unlikely that this will
            // be called...

            return 0;
        }

        // This should never be called
        public void InitiateGameConnectionOld()
        {
            Write("InitiateGameConnectionOld should NEVER be called!");
        }

        public void TerminateGameConnection(uint server_ip, ushort server_port)
        {

        }

        public bool TerminateAppMuliStep(uint a, uint b)
        {
            return false;
        }

        public void SetSelfAsPrimaryChatDestination()
        {

        }

        public bool IsPrimaryChatDestination()
        {
            return false;
        }

        public void RequestLegacyCDKey(uint appid)
        {

        }

        public bool AckGuestPass(string passcode)
        {
            return false;
        }

        public bool RedeemGuestPass(string passcode)
        {
            return false;
        }

        public uint GuestPassToGiveCount()
        {
            return 0;
        }

        public uint GetGuestPassToRedeemCount()
        {
            return 0;
        }

        public bool GetGuestPassToGiveInfo(uint nPassIndex, ref uint pgidGuestPassID, ref uint pnPackageID, ref uint pRTime32Created, ref uint pRTime32Expiration, ref uint pRTime32Sent, ref uint pRTime32Redeemed, IntPtr pchRecipientAddress, int cRecipientAddressSize)
        {
            return false;
        }
        public bool GetGuestPassToGiveOut(uint nPassIndex)
        {
            return false;
        }
        public bool GetGuestPassToRedeem(uint nPassIndex)
        {
            return false;
        }
        public bool GetGuestPassToRedeemInfo(uint nPassIndex, ref uint pgidGuestPassID, ref uint pnPackageID, ref uint pRTime32Created, ref uint pRTime32Expiration, ref uint pRTime32Sent, ref uint pRTime32Redeemed)
        {
            return false;
        }
        public bool GetGuestPassToRedeemSenderName(uint nPassIndex, IntPtr pchSenderName, int cSenderNameSize)
        {
            return false;
        }

        public uint GetNumAppsInGuestPassesToRedeem()
        {
            return 0;
        }

        public uint GetAppsInGuestPassesToRedeem(IntPtr app_array_to_fill, int max_fill)
        {
            return 0;
        }

        public uint GetCountUserNotifications()
        {
            return 0;
        }

        public uint GetCountUserNotification(uint type)
        {
            return 0;
        }

        public void RequestStoreAuthURL(string a)
        {
            // TODO: revese this
        }

        public void SetLanguage(string a)
        {
            // TODO: reverse this
        }

        public void TrackAppUsageEvent(ulong game_id, int usage_event, string extra_info)
        {

        }

        public uint RaiseConnectionPriority(uint new_priority)
        {
            return 0;
        }

        public void ResetConnectionPriority()
        {

        }

        public bool BHasCachedCredentials(string a)
        {
            return false;
        }

        public bool SetAccountNameForCachedCredentialLogin(string name, bool a)
        {
            return false;
        }

        public bool GetCurrentWebAuthToken(IntPtr out_string, int out_max)
        {
            return false;
        }
        public uint RequestWebAuthToken()
        {
            // Returns SteamApiCall
            return 0;
        }

        public void SetLoginInformation(string username, string password, bool remember_password)
        {
            if (remember_password)
            {
                Write("SetLoginInformation: remember_password set but we cant!");
            }

        }

        public void SetTwoFactorCode(string code)
        {

        }

        public void ClearLoginInformation()
        {

        }

        public bool GetLanguage(IntPtr out_string, int out_max)
        {
            return false;
        }

        public bool BIsCyberCafe()
        {
            return false;
        }

        public bool BIsAcademicAccount()
        {
            return false;
        }

        public bool BIsPortal2EducationAccount()
        {
            return false;
        }

        public bool BIsAlienwareDemoAccount()
        {
            return false;
        }

        public void CreateAccount(string new_username, string new_password, string new_email)
        {

        }

        public uint ResetPassword(string account_name, string old_password, string new_password, string code, string answer)
        {
            return 0;
        }

        public void ValidatePasswordResetCodeAndSendSms(string a, string b)
        {

        }

        public void TrackNatTraversalStat(IntPtr stat_out)
        {

        }

        public void TrackSteamUsageEvent(uint usage_event, string extra, uint a)
        {

        }

        public void TrackSteamGuiUsage(string a)
        {

        }

        public void SetComputerInUse()
        {

        }

        private void Write(string v)
        {
            Main.Write(InterfaceVersion, v);
        }
    }
}