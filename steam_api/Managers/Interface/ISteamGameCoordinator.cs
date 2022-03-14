using System;
using SKYNET.Types;
using Steamworks;

namespace SKYNET.Interface
{
    public interface ISteamGameCoordinator
    {
        EGCResults SendMessage_(UInt32 unMsgType, IntPtr pubData, UInt32 cubData);

        // returns true if there is a message waiting from the game coordinator
        bool IsMessageAvailable(UInt32 pcubMsgSize);

        // fills the provided buffer with the first message in the queue and returns k_EGCResultOK or 
        // returns k_EGCResultNoMessage if there is no message waiting. pcubMsgSize is filled with the message size.
        // If the provided buffer is not large enough to fit the entire message, k_EGCResultBufferTooSmall is returned
        // and the message remains at the head of the queue.
        EGCResults RetrieveMessage(UInt32 punMsgType, IntPtr pubDest, UInt32 cubDest, UInt32 pcubMsgSize);


    }
}
