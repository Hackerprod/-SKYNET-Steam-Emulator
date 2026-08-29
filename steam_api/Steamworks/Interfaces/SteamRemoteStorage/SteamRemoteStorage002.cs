using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Interfaces
{
    /// <summary>Legacy ISteamRemoteStorage ABI used by Left 4 Dead.</summary>
    [Interface("STEAMREMOTESTORAGE_INTERFACE_VERSION002")]
    public class SteamRemoteStorage002 : ISteamInterface
    {
        public bool FileWrite(IntPtr _, string pchFile, IntPtr pvData, int cubData)
        {
            return SteamEmulator.SteamRemoteStorage.FileWrite(pchFile, pvData, cubData);
        }

        public int GetFileSize(IntPtr _, string pchFile)
        {
            return SteamEmulator.SteamRemoteStorage.GetFileSize(pchFile);
        }

        public int FileRead(IntPtr _, string pchFile, IntPtr pvData, int cubDataToRead)
        {
            return SteamEmulator.SteamRemoteStorage.FileRead(pchFile, pvData, cubDataToRead);
        }

        public bool FileExists(IntPtr _, string pchFile)
        {
            return SteamEmulator.SteamRemoteStorage.FileExists(pchFile);
        }

        public int GetFileCount(IntPtr _)
        {
            return SteamEmulator.SteamRemoteStorage.GetFileCount();
        }

        public string GetFileNameAndSize(IntPtr _, int iFile, ref int pnFileSizeInBytes)
        {
            return SteamEmulator.SteamRemoteStorage.GetFileNameAndSize(iFile, ref pnFileSizeInBytes);
        }

        public bool GetQuota(IntPtr _, IntPtr pnTotalBytes, IntPtr puAvailableBytes)
        {
            ulong total = 0;
            ulong available = 0;
            bool result = SteamEmulator.SteamRemoteStorage.GetQuota(ref total, ref available);

            if (pnTotalBytes != IntPtr.Zero)
            {
                Marshal.WriteInt32(pnTotalBytes, unchecked((int)Math.Min(total, int.MaxValue)));
            }
            if (puAvailableBytes != IntPtr.Zero)
            {
                Marshal.WriteInt32(puAvailableBytes, unchecked((int)Math.Min(available, int.MaxValue)));
            }

            return result;
        }
    }
}
