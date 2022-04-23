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
        private List<byte[]> InMessages;

        public SteamGameCoordinator()
        {
            InterfaceVersion = "SteamGameCoordinator";
            InMessages = new List<byte[]>();

            // Connection Status test
            byte[] ConnectionStatus = new byte[] { 0xA4, 0x0F, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00 };
            InMessages.Add(ConnectionStatus);
        }

        public void PushMessage(byte[] message)
        {
            InMessages.Add(message);
            uint MsgSize = (uint)Marshal.SizeOf(message);

            GCMessageAvailable_t data = new GCMessageAvailable_t();
            data.m_nMessageSize = (uint)message.Length;
            CallbackManager.AddCallbackResult(data);
        }

        public EGCResults SendMessage(uint unMsgType, IntPtr pubData, uint cubData)
        {
            uint msgType = GetGCMsg(unMsgType);
            byte[] bytes = pubData.GetBytes(cubData);

            Write($"SendMessage [{msgType}], {bytes.Length} bytes");
            if (SteamEmulator.GameCoordinatorPlugin != null)
            {
                SteamEmulator.GameCoordinatorPlugin.MessageFromGame(bytes);
            }
            return EGCResults.k_EGCResultOK;
        }

        public bool IsMessageAvailable(ref uint pcubMsgSize)
        {
            Write("IsMessageAvailable");
            if (InMessages.Any())
            {
                pcubMsgSize = (uint)InMessages[0].Length;
                Write($"Found Message Available, {pcubMsgSize} bytes");
                return true;
            }
            return false;
        }

        public EGCResults RetrieveMessage(ref uint punMsgType, IntPtr pubDest, uint cubDest, ref uint pcubMsgSize)
        {
            uint msgType = GetGCMsg(punMsgType);
            Write($"RetrieveMessage [{msgType}]");
            EGCResults Result = EGCResults.k_EGCResultNoMessage;
            uint size = 0;

            MutexHelper.Wait("RetrieveMessage", delegate
            {
                if (InMessages.Any())
                {
                    try
                    {
                        byte[] msg = InMessages[0];

                        Marshal.Copy(msg, 0, pubDest, msg.Length);
                        size = (uint)msg.Length;

                        InMessages.RemoveAt(0);
                        Result = EGCResults.k_EGCResultOK;
                    }
                    catch
                    {

                    }
                }
            });

            pcubMsgSize = size;
            return Result;
        }

        private uint GetGCMsg(uint msg)
        {
            return msg & 0x7FFFFFFFu;
        }
    }
}
