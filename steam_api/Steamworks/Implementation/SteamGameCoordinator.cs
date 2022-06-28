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
            uint msgType = GetGCMsg(unMsgType);
            byte[] bytes = pubData.GetBytes(cubData);

            Write($"SendMessage (MsgType = {msgType}], {bytes.Length} bytes)");
            IPCManager.GCRequest(msgType, bytes);
            return EGCResults.k_EGCResultOK;
        }

        public unsafe bool IsMessageAvailable(uint* pcubMsgSize)
        {
            bool Result = false;
            uint SizeResult = 0;
            MutexHelper.Wait("InMessages", delegate
            {
                if (InMessages.Any())
                {
                    var size = InMessages.FirstOrDefault().Value.Length;
                    SizeResult = (uint)size;
                    Result = true;
                }
            });

            var sizes = new uint[1] { SizeResult };
            fixed (uint* p = &sizes[0])
            {
                pcubMsgSize = p;
            }

            Write($"IsMessageAvailable (MsgSize = {*pcubMsgSize}) = {Result}");
            return Result;
        }

        public unsafe int RetrieveMessage(uint* punMsgType, IntPtr pubDest, uint cubDest, uint* pcubMsgSize)
        {
            EGCResults Result = EGCResults.k_EGCResultNoMessage;
            byte[] Body = null;
            uint MsgType = 0;
            uint MsgSize = 0;
            return (int)Result;

            MutexHelper.Wait("InMessages", delegate
            {
                if (InMessages.Any())
                {
                    var msg = InMessages.First();
                    Body = msg.Value;
                    MsgType = msg.Key;
                    MsgSize = (uint)msg.Value.Length;

                    Result = EGCResults.k_EGCResultOK;
                    InMessages.TryRemove(msg.Key, out _);
                }
            });

            if (Body == null)
            {
                return (int)EGCResults.k_EGCResultNoMessage;
            }

            try
            {
                Marshal.Copy(Body, 0, pubDest, (int)MsgSize);
                var values = new uint[2] { MsgType, MsgSize };
                fixed (uint* Type = &values[0])
                {
                    punMsgType = Type;
                }
                fixed (uint* Size = &values[1])
                {
                    pcubMsgSize = Size;
                }
            }
            catch (Exception ex)
            {
                Write($"{ex.Message} {ex.StackTrace}");
            }

            Write($"RetrieveMessage (MsgType = {*punMsgType}, {*pcubMsgSize} bytes) = {Result}");
            return (int)Result;
        }

        private uint GetGCMsg(uint msg)
        {
            return msg & 0x7FFFFFFFu;
        }
    }
}
