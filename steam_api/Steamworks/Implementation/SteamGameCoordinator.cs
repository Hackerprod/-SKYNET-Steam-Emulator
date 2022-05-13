using System;
using System.Runtime.InteropServices;
using SKYNET.Helper;
using System.Collections.Generic;
using SKYNET.Callback;
using System.Linq;
using SKYNET.Managers;
using System.Collections.Concurrent;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamGameCoordinator : ISteamInterface
    {
        private ConcurrentDictionary<uint, byte[]> InMessages;

        public SteamGameCoordinator()
        {
            InterfaceName = "SteamGameCoordinator";
            InterfaceVersion = "SteamGameCoordinator001";
            InMessages = new ConcurrentDictionary<uint, byte[]>();

            // CMsgConnectionStatus serialized
            byte[] ConnectionStatus = new byte[] { 0xA4, 0x0F, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00 };
            //InMessages.TryAdd(4009U, ConnectionStatus);
        }

        public void PushMessage(uint MsgType, byte[] message)
        {
            InMessages.TryAdd(MsgType, message);

            GCMessageAvailable_t data = new GCMessageAvailable_t();
            data.m_nMessageSize = (uint)message.Length;
            CallbackManager.AddCallbackResult(data);
        }

        public EGCResults SendMessage(uint unMsgType, IntPtr pubData, uint cubData)
        {
            uint msgType = GetGCMsg(unMsgType);
            byte[] bytes = pubData.GetBytes(cubData);

            Write($"SendMessage (MsgType = {msgType}], {bytes.Length} bytes)");
            if (SteamEmulator.GameCoordinatorPlugin != null)
            {
                SteamEmulator.GameCoordinatorPlugin.MessageFromGame(bytes);
            }
            return EGCResults.k_EGCResultOK;
        }

        public bool IsMessageAvailable(ref uint pcubMsgSize)
        {
            bool Result = false;
            if (InMessages.Any())
            {
                Result = true;
            }
            Write($"IsMessageAvailable (MsgSize = {pcubMsgSize}) = {Result}");
            return Result;
        }

        public EGCResults RetrieveMessage(ref uint punMsgType, IntPtr pubDest, uint cubDest, ref uint pcubMsgSize)
        {
            EGCResults Result = EGCResults.k_EGCResultNoMessage;

            if (InMessages.Any())
            {
                try
                {
                    var msg = InMessages.First();
                    Marshal.Copy(msg.Value, 0, pubDest, msg.Value.Length);
                    //pcubMsgSize = (uint)msg.Value.Length;
                    //punMsgType = msg.Key;
                    Result = EGCResults.k_EGCResultOK;

                    InMessages.TryRemove(msg.Key, out _);
                }
                catch (Exception ex)
                {
                    Write($"{ex.Message} {ex.StackTrace}");
                }
            }
            Write($"RetrieveMessage (MsgType = {punMsgType}, {pcubMsgSize} bytes) = {Result}");
            return Result;
        }

        private uint GetGCMsg(uint msg)
        {
            return msg & 0x7FFFFFFFu;
        }
    }
}
