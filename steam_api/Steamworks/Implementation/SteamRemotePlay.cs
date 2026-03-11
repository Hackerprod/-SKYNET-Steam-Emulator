using SKYNET.Steamworks.Interfaces;

using RemotePlaySessionID_t = System.UInt32;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamRemotePlay : ISteamInterface
    {
        public static SteamRemotePlay Instance;
        private uint nextCursorId = 1;

        public SteamRemotePlay()
        {
            Instance = this;
            InterfaceName = "SteamRemotePlay";
            InterfaceVersion = "STEAMREMOTEPLAY_INTERFACE_VERSION003";
        }

        public uint GetSessionCount()
        {
            Write("GetSessionCount");
            return 0;
        }

        public RemotePlaySessionID_t GetSessionID(int iSessionIndex)
        {
            Write("GetSessionID");
            return 0;
        }

        public CSteamID GetSessionSteamID(RemotePlaySessionID_t unSessionID)
        {
            Write("GetSessionSteamID");
            return CSteamID.CreateOne();
        }

        public string GetSessionClientName(RemotePlaySessionID_t unSessionID)
        {
            Write("GetSessionClientName");
            return "";
        }

        public int GetSessionClientFormFactor(RemotePlaySessionID_t unSessionID)
        {
            Write("GetSessionClientFormFactor");
            return 0;
        }

        public bool BGetSessionClientResolution(RemotePlaySessionID_t unSessionID, int pnResolutionX, int pnResolutionY)
        {
            Write("BGetSessionClientResolution");
            return default;
        }

        public bool BSendRemotePlayTogetherInvite(ulong steamIDFriend)
        {
            Write("BSendRemotePlayTogetherInvite");
            return false;
        }

        public bool ShowRemotePlayTogetherUI()
        {
            Write("ShowRemotePlayTogetherUI");
            return false;
        }

        public bool BEnableRemotePlayTogetherDirectInput()
        {
            Write("BEnableRemotePlayTogetherDirectInput");
            return false;
        }

        public void DisableRemotePlayTogetherDirectInput()
        {
            Write("DisableRemotePlayTogetherDirectInput");
        }

        public uint GetInput(System.IntPtr pInput, uint unMaxEvents)
        {
            Write("GetInput");
            return 0;
        }

        public void SetMouseVisibility(RemotePlaySessionID_t unSessionID, bool bVisible)
        {
            Write("SetMouseVisibility");
        }

        public void SetMousePosition(RemotePlaySessionID_t unSessionID, float flNormalizedX, float flNormalizedY)
        {
            Write("SetMousePosition");
        }

        public uint CreateMouseCursor(int nWidth, int nHeight, int nHotX, int nHotY, System.IntPtr pBGRA, int nPitch)
        {
            Write("CreateMouseCursor");
            return nextCursorId++;
        }

        public void SetMouseCursor(RemotePlaySessionID_t unSessionID, uint unCursorID)
        {
            Write("SetMouseCursor");
        }
    }
}
