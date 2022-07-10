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
    }

    public struct ControllerDigitalActionData_t
    {
        public bool bState;

        public bool bActive;
    };

    public struct ControllerAnalogActionData_t
    {
        public int eMode;
        public float x, y;
        public bool bActive;
    };

    public struct InputDigitalActionData_t
    {
        public bool bState;
        public bool bActive;
    };

    public struct InputAnalogActionData_t
    {
        public int eMode;
        public float x, y;
        public bool bActive;
    };
    public struct InputMotionData_t
    {
        float rotQuatX;
        float rotQuatY;
        float rotQuatZ;
        float rotQuatW;

        // Positional acceleration
        float posAccelX;
        float posAccelY;
        float posAccelZ;

        // Angular velocity
        float rotVelX;
        float rotVelY;
        float rotVelZ;
    };

    public struct SteamItemDetails_t
    {
        SteamItemInstanceID_t m_itemId;
        SteamItemDef_t m_iDefinition;
        uint m_unQuantity;
        uint m_unFlags; // see ESteamItemFlags
    };

    public struct P2PSessionState_t
    {
        public byte m_bConnectionActive; 
        public byte m_bConnecting; 
        public uint m_eP2PSessionError; 
        public bool m_bUsingRelay;  
        public uint m_nBytesQueuedForSend;
        public uint m_nPacketsQueuedForSend;
        public uint m_nRemoteIP; 
        public uint m_nRemotePort;
    };


    public struct SteamNetworkPingLocation_t
    {
        public uint m_data;
    };

    public struct SteamPartyBeaconLocation_t
    {
        uint m_eType;
        uint m_ulLocationID;
    };

    public struct TicketData
    {
        public ulong SteamID;
        public uint IPClient;
        public IntPtr AuthBlob;
        public uint BlobSize;
    };
}
