using System;
using System.Runtime.InteropServices;

using PublishedFileId_t = System.UInt64;
using UGCHandle_t = System.UInt64;
using SteamItemInstanceID_t = System.UInt64;
using AppId_t = System.UInt32;
using SteamItemDef_t = System.UInt32;

namespace SKYNET.Steamworks
{
    public struct FriendGameInfo_t
    {
        public uint GameID;
        public uint GameIP;
        public ushort GamePort;
        public ushort QueryPort;
        public ulong steamIDLobby;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct SteamUGCDetails_t
    {
        public PublishedFileId_t m_nPublishedFileId;

        public EResult m_eResult;

        public EWorkshopFileType m_eFileType;

        public AppId_t m_nCreatorAppID;

        public AppId_t m_nConsumerAppID;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 129)]
        public string m_rgchTitle;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
        public string m_rgchDescription;

        public ulong m_ulSteamIDOwner;

        public uint m_rtimeCreated;

        public uint m_rtimeUpdated;

        public uint m_rtimeAddedToUserList;

        public ERemoteStoragePublishedFileVisibility m_eVisibility;

        [MarshalAs(UnmanagedType.I1)]
        public bool m_bBanned;

        [MarshalAs(UnmanagedType.I1)]
        public bool m_bAcceptedForUse;

        [MarshalAs(UnmanagedType.I1)]
        public bool m_bTagsTruncated;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1025)]
        public string m_rgchTags;

        public UGCHandle_t m_hFile;

        public UGCHandle_t m_hPreviewFile;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string m_pchFileName;

        public int m_nFileSize;

        public int m_nPreviewFileSize;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string m_rgchURL;

        public uint m_unVotesUp;

        public uint m_unVotesDown;

        public float m_flScore;

        public uint m_unNumChildren;

        public ulong m_ulTotalFilesSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ControllerDigitalActionData_t
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool bState;

        [MarshalAs(UnmanagedType.I1)]
        public bool bActive;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ControllerAnalogActionData_t
    {
        public int eMode;
        public float x, y;
        [MarshalAs(UnmanagedType.I1)]
        public bool bActive;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct InputDigitalActionData_t
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool bState;
        [MarshalAs(UnmanagedType.I1)]
        public bool bActive;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct InputAnalogActionData_t
    {
        public int eMode;
        public float x, y;
        [MarshalAs(UnmanagedType.I1)]
        public bool bActive;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct InputMotionData_t
    {
        public float rotQuatX;
        public float rotQuatY;
        public float rotQuatZ;
        public float rotQuatW;

        // Positional acceleration
        public float posAccelX;
        public float posAccelY;
        public float posAccelZ;

        // Angular velocity
        public float rotVelX;
        public float rotVelY;
        public float rotVelZ;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct ControllerMotionData_t
    {
        public float rotQuatX;
        public float rotQuatY;
        public float rotQuatZ;
        public float rotQuatW;

        public float posAccelX;
        public float posAccelY;
        public float posAccelZ;

        public float rotVelX;
        public float rotVelY;
        public float rotVelZ;
    };

    public struct SteamItemDetails_t
    {
        SteamItemInstanceID_t m_itemId;
        SteamItemDef_t m_iDefinition;
        ushort m_unQuantity;
        ushort m_unFlags; // see ESteamItemFlags
    };

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct P2PSessionState_t
    {
        public byte m_bConnectionActive; 
        public byte m_bConnecting; 
        public byte m_eP2PSessionError;
        public byte m_bUsingRelay;
        public int m_nBytesQueuedForSend;
        public int m_nPacketsQueuedForSend;
        public uint m_nRemoteIP; 
        public ushort m_nRemotePort;
    };


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SteamNetworkPingLocation_t
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] m_data;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct SteamPartyBeaconLocation_t
    {
        public uint m_eType;
        public ulong m_ulLocationID;
    };

    public struct TicketData
    {
        public ulong SteamID;
        public uint IPClient;
        public IntPtr AuthBlob;
        public uint BlobSize;
    };
}
