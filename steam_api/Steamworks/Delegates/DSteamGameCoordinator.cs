using System;
using System.Runtime.InteropServices;
using Core.Interface;
using SKYNET.Types;
using Steamworks;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamGameCoordinator")]
    public class DSteamGameCoordinator
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EGCResults SendMessage_(UInt32 unMsgType, IntPtr pubData, UInt32 cubData);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsMessageAvailable(UInt32 pcubMsgSize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EGCResults RetrieveMessage(UInt32 punMsgType, IntPtr pubDest, UInt32 cubDest, UInt32 pcubMsgSize);
    }
}
