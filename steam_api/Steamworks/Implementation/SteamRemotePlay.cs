using SKYNET.Steamworks.Interfaces;

using RemotePlaySessionID_t = System.UInt32;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamRemotePlay : ISteamInterface
    {
        public static SteamRemotePlay Instance;
        private uint nextCursorId = 1;
        private const int MaximumCursorDimension = 4096;
        private const int MaximumCursorCount = 64;
        private readonly object cursorGate = new object();
        private readonly System.Collections.Generic.Dictionary<uint, RemoteCursor> cursors =
            new System.Collections.Generic.Dictionary<uint, RemoteCursor>();

        public SteamRemotePlay()
        {
            Instance = this;
            InterfaceName = "SteamRemotePlay";
            InterfaceVersion = "STEAMREMOTEPLAY_INTERFACE_VERSION004";
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
            return CSteamID.Invalid;
        }

        public bool BSessionRemotePlayTogether(RemotePlaySessionID_t unSessionID)
        {
            Write("BSessionRemotePlayTogether");
            return false;
        }

        public uint GetSessionGuestID(RemotePlaySessionID_t unSessionID)
        {
            Write("GetSessionGuestID");
            return 0;
        }

        public int GetSmallSessionAvatar(RemotePlaySessionID_t unSessionID)
        {
            Write("GetSmallSessionAvatar");
            return 0;
        }

        public int GetMediumSessionAvatar(RemotePlaySessionID_t unSessionID)
        {
            Write("GetMediumSessionAvatar");
            return 0;
        }

        public int GetLargeSessionAvatar(RemotePlaySessionID_t unSessionID)
        {
            Write("GetLargeSessionAvatar");
            return 0;
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

        public bool BGetSessionClientResolution(RemotePlaySessionID_t unSessionID, System.IntPtr pnResolutionX, System.IntPtr pnResolutionY)
        {
            Write("BGetSessionClientResolution");
            if (pnResolutionX != System.IntPtr.Zero)
            {
                System.Runtime.InteropServices.Marshal.WriteInt32(pnResolutionX, 0);
            }
            if (pnResolutionY != System.IntPtr.Zero)
            {
                System.Runtime.InteropServices.Marshal.WriteInt32(pnResolutionY, 0);
            }
            return default;
        }

        public bool BSendRemotePlayTogetherInvite(ulong steamIDFriend)
        {
            Write($"BSendRemotePlayTogetherInvite unavailable friend={steamIDFriend}");
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
            if (nWidth <= 0 || nHeight <= 0 ||
                nWidth > MaximumCursorDimension || nHeight > MaximumCursorDimension ||
                nHotX < 0 || nHotX >= nWidth || nHotY < 0 || nHotY >= nHeight ||
                pBGRA == System.IntPtr.Zero || nPitch < checked(nWidth * 4))
            {
                return 0;
            }

            int byteCount;
            try
            {
                byteCount = checked(nPitch * nHeight);
            }
            catch (System.OverflowException)
            {
                return 0;
            }

            var pixels = new byte[byteCount];
            System.Runtime.InteropServices.Marshal.Copy(pBGRA, pixels, 0, pixels.Length);
            lock (cursorGate)
            {
                if (cursors.Count >= MaximumCursorCount)
                {
                    var oldest = uint.MaxValue;
                    foreach (var cursorId in cursors.Keys)
                    {
                        if (cursorId < oldest)
                        {
                            oldest = cursorId;
                        }
                    }
                    if (oldest != uint.MaxValue)
                    {
                        cursors.Remove(oldest);
                    }
                }

                var id = nextCursorId++;
                if (id == 0)
                {
                    id = nextCursorId++;
                }
                cursors[id] = new RemoteCursor(nWidth, nHeight, nHotX, nHotY, nPitch, pixels);
                return id;
            }
        }

        public void SetMouseCursor(RemotePlaySessionID_t unSessionID, uint unCursorID)
        {
            Write("SetMouseCursor");
        }

        private sealed class RemoteCursor
        {
            public RemoteCursor(int width, int height, int hotX, int hotY, int pitch, byte[] pixels)
            {
                Width = width;
                Height = height;
                HotX = hotX;
                HotY = hotY;
                Pitch = pitch;
                Pixels = pixels;
            }

            public int Width { get; }
            public int Height { get; }
            public int HotX { get; }
            public int HotY { get; }
            public int Pitch { get; }
            public byte[] Pixels { get; }
        }
    }
}
