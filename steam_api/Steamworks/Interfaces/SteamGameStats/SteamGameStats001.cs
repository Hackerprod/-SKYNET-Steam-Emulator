using System;
using System.Runtime.InteropServices;

using SteamAPICall_t = System.UInt64;
using RTime32 = System.UInt32;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamGameStats001")]
    public class SteamGameStats001 : ISteamInterface
    {
        public SteamAPICall_t GetNewSession(IntPtr _, sbyte nAccountType, ulong ulAccountID, int nAppID, RTime32 rtTimeStarted)
        {
            Write("GetNewSession");
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t EndSession(IntPtr _, ulong ulSessionID, RTime32 rtTimeEnded, int nReasonCode)
        {
            Write("EndSession");
            return k_uAPICallInvalid;
        }

        public EResult AddSessionAttributeInt(IntPtr _, ulong ulSessionID, string pstrName, int nData)
        {
            Write("AddSessionAttributeInt");
            return EResult.k_EResultFail;
        }

        public EResult AddSessionAttributeString(IntPtr _, ulong ulSessionID, string pstrName, string pstrData)
        {
            Write("AddSessionAttributeString");
            return EResult.k_EResultFail;
        }

        public EResult AddSessionAttributeFloat(IntPtr _, ulong ulSessionID, string pstrName, float fData)
        {
            Write("AddSessionAttributeFloat");
            return EResult.k_EResultFail;
        }

        public EResult AddNewRow(IntPtr _, IntPtr pulRowID, ulong ulSessionID, string pstrTableName)
        {
            if (pulRowID != IntPtr.Zero)
            {
                Marshal.WriteInt64(pulRowID, 0);
            }
            Write("AddNewRow");
            return EResult.k_EResultFail;
        }

        public EResult CommitRow(IntPtr _, ulong ulRowID)
        {
            Write("CommitRow");
            return EResult.k_EResultFail;
        }

        public EResult CommitOutstandingRows(IntPtr _, ulong ulSessionID)
        {
            Write("CommitOutstandingRows");
            return EResult.k_EResultFail;
        }

        public EResult AddRowAttributeInt(IntPtr _, ulong ulRowID, string pstrName, int nData)
        {
            Write("AddRowAttributeInt");
            return EResult.k_EResultFail;
        }

        public EResult AddRowAtributeString(IntPtr _, ulong ulRowID, string pstrName, string pstrData)
        {
            Write("AddRowAtributeString");
            return EResult.k_EResultFail;
        }

        public EResult AddRowAttributeFloat(IntPtr _, ulong ulRowID, string pstrName, float fData)
        {
            Write("AddRowAttributeFloat");
            return EResult.k_EResultFail;
        }

        public EResult AddSessionAttributeInt64(IntPtr _, ulong ulSessionID, string pstrName, long llData)
        {
            Write("AddSessionAttributeInt64");
            return EResult.k_EResultFail;
        }

        public EResult AddRowAttributeInt64(IntPtr _, ulong ulRowID, string pstrName, long llData)
        {
            Write("AddRowAttributeInt64");
            return EResult.k_EResultFail;
        }

        private static void Write(string message)
        {
            SteamEmulator.Write("SteamGameStats001", message);
        }
    }
}
