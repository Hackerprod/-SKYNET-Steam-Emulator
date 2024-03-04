using System;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Concurrent;
using SKYNET.Helpers;
using SKYNET.Callback;
using SKYNET.Managers;
using SKYNET.Steamworks.Interfaces;
using System.Collections.Generic;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamGameCoordinator : ISteamInterface
    {
        public static SteamGameCoordinator Instance;

        private ConcurrentDictionary<uint, byte[]> InMessages;

        public SteamGameCoordinator()
        {
            Instance = this;
            InterfaceName = "SteamGameCoordinator";
            InterfaceVersion = "SteamGameCoordinator001";
            InMessages = new ConcurrentDictionary<uint, byte[]>();

            // CMsgConnectionStatus serialized
            byte[] ConnectionStatus = new byte[] { 0xA4, 0x0F, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00 };
            //InMessages.TryAdd(4009U, ConnectionStatus);
        }

        public void PushMessage(uint MsgType, byte[] message)
        {
            if (InMessages.Any())
            {
                InMessages.TryAdd(MsgType, message);
            }

            GCMessageAvailable_t data = new GCMessageAvailable_t();
            data.m_nMessageSize = (uint)message.Length;
            CallbackManager.AddCallbackResult(data);
        }

        public EGCResults SendMessage(uint unMsgType, IntPtr pubData, uint cubData)
        {
            uint gCMsg = GetGCMsg(unMsgType);
            byte[] bytes = pubData.GetBytes(cubData);
            //IPCManager.SendGCMessage(bytes, gCMsg);
            Write($"SendMessage (MsgType = {gCMsg}, MsgSize = {cubData}) = k_EGCResultOK");
            return EGCResults.k_EGCResultOK;
        }

        public bool IsMessageAvailable(ref uint pcubMsgSize)
        {
            bool result = false;
            if (InMessages.Any())
            {
                //pcubMsgSize = (uint)InMessages.First().Value.Length;
                result = true;
            }
            Write($"IsMessageAvailable = {result}");
            return result;
        }

        public EGCResults RetrieveMessage(ref uint punMsgType, IntPtr pubDest, uint cubDest, ref uint pcubMsgSize)
        {
            EGCResults result = EGCResults.k_EGCResultNoMessage;
            pcubMsgSize = 0;
            punMsgType = 0;
            if (InMessages.Any())
            {
                try
                {
                    var keyValuePair = InMessages.First();
                    var MsgType = keyValuePair.Key;
                    var MsgBody = keyValuePair.Value;
                    //Marshal.Copy(MsgBody, 0, pubDest, MsgBody.Length);
                    //pcubMsgSize = (uint)MsgBody.Length;
                    //punMsgType = MsgType;
                    //result = EGCResults.k_EGCResultOK;
                    InMessages.TryRemove(keyValuePair.Key, out var _);
                }
                catch (Exception ex)
                {
                    Write($"RetrieveMessage {ex}");
                }
            }
            Write($"RetrieveMessage (MsgType = {punMsgType}, MsgSize = {pcubMsgSize}) = {result}");
            return result;
        }

        private uint GetGCMsg(uint msg)
        {
            return msg & 0x7FFFFFFFu;
        }
    }
}
