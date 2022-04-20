using System;
using System.Runtime.InteropServices;
using SKYNET.Helper;
using System.Collections.Generic;
using SKYNET.Callback;
using System.Linq;
using SKYNET.Managers;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamGameCoordinator : ISteamInterface
    {
        private List<object> InMessages;
        public void PushMessage(object message)
        {
            InMessages.Add(message);
            uint MsgSize = (uint)Marshal.SizeOf(message);

            GCMessageAvailable_t data;
            data.m_nMessageSize = MsgSize;
            CallbackManager.addCBResult(GCMessageAvailable_t.k_iCallback, data, MsgSize);
        }

        public SteamGameCoordinator()
        {
            InterfaceVersion = "SteamGameCoordinator";
            InMessages = new List<object>();
        }

        public EGCResults SendMessage(uint unMsgType, IntPtr pubData, uint cubData)
        {
            uint msgType = GetGCMsg(unMsgType);
            byte[] bytes = pubData.GetBytes(cubData);

            Write($"SendMessage [{msgType}], {bytes.Length} bytes");

            return EGCResults.k_EGCResultOK;
        }

        public bool IsMessageAvailable(uint pcubMsgSize)
        {
            Write("IsMessageAvailable");
            if (InMessages.Any())
            {
                return true;
            }
            return false;
        }

        public EGCResults RetrieveMessage(uint punMsgType, IntPtr pubDest, uint cubDest, uint pcubMsgSize)
        {
            uint msgType = GetGCMsg(punMsgType);

            Write($"RetrieveMessage [{msgType}]");
            return EGCResults.k_EGCResultNoMessage;
        }

        private uint GetGCMsg(uint msg)
        {
            return msg & 0x7FFFFFFFu;
        }
    }
}
