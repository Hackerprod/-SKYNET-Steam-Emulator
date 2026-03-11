using System;
using System.Runtime.InteropServices;
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

        private readonly Queue<GCMessage> inMessages;
        private readonly object inMessagesLock = new object();

        public SteamGameCoordinator()
        {
            Instance = this;
            InterfaceName = "SteamGameCoordinator";
            InterfaceVersion = "SteamGameCoordinator001";
            inMessages = new Queue<GCMessage>();

            // CMsgConnectionStatus serialized
            byte[] ConnectionStatus = new byte[] { 0xA4, 0x0F, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00 };
            //InMessages.TryAdd(4009U, ConnectionStatus);
        }

        public void PushMessage(uint MsgType, byte[] message)
        {
            byte[] payload = message ?? Array.Empty<byte>();

            lock (inMessagesLock)
            {
                inMessages.Enqueue(new GCMessage(MsgType, payload));
            }

            GCMessageAvailable_t data = new GCMessageAvailable_t
            {
                m_nMessageSize = (uint)payload.Length
            };
            CallbackManager.AddCallback(data);
        }

        public EGCResults SendMessage(uint unMsgType, IntPtr pubData, uint cubData)
        {
            if (cubData > 0 && pubData == IntPtr.Zero)
            {
                Write($"SendMessage (MsgType = {GetGCMsg(unMsgType)}, MsgSize = {cubData}) = k_EGCResultInvalidMessage");
                return EGCResults.k_EGCResultInvalidMessage;
            }

            uint gCMsg = GetGCMsg(unMsgType);
            byte[] bytes = pubData.GetBytes(cubData);
            if (SkyNetApiClient.IsEnabled && !SkyNetApiClient.SendGCMessage(gCMsg, bytes))
            {
                Write($"SendMessage (MsgType = {gCMsg}, MsgSize = {cubData}) = k_EGCResultInvalidMessage");
                return EGCResults.k_EGCResultInvalidMessage;
            }
            Write($"SendMessage (MsgType = {gCMsg}, MsgSize = {cubData}) = k_EGCResultOK");
            return EGCResults.k_EGCResultOK;
        }

        public bool IsMessageAvailable(ref uint pcubMsgSize)
        {
            lock (inMessagesLock)
            {
                if (inMessages.Count > 0)
                {
                    pcubMsgSize = (uint)inMessages.Peek().MessageBody.Length;
                    Write($"IsMessageAvailable = True");
                    return true;
                }
            }

            pcubMsgSize = 0;
            Write($"IsMessageAvailable = False");
            return false;
        }

        public EGCResults RetrieveMessage(ref uint punMsgType, IntPtr pubDest, uint cubDest, ref uint pcubMsgSize)
        {
            EGCResults result = EGCResults.k_EGCResultNoMessage;
            pcubMsgSize = 0;
            punMsgType = 0;
            lock (inMessagesLock)
            {
                if (inMessages.Count > 0)
                {
                    try
                    {
                        var message = inMessages.Peek();
                        pcubMsgSize = (uint)message.MessageBody.Length;
                        punMsgType = message.MessageType;

                        if (cubDest < pcubMsgSize || (pcubMsgSize > 0 && pubDest == IntPtr.Zero))
                        {
                            result = EGCResults.k_EGCResultBufferTooSmall;
                        }
                        else
                        {
                            if (pcubMsgSize > 0)
                            {
                                Marshal.Copy(message.MessageBody, 0, pubDest, message.MessageBody.Length);
                            }

                            inMessages.Dequeue();
                            result = EGCResults.k_EGCResultOK;
                        }
                    }
                    catch (Exception ex)
                    {
                        Write($"RetrieveMessage {ex}");
                    }
                }
            }
            Write($"RetrieveMessage (MsgType = {punMsgType}, MsgSize = {pcubMsgSize}) = {result}");
            return result;
        }

        private uint GetGCMsg(uint msg)
        {
            return msg & 0x7FFFFFFFu;
        }

        private readonly struct GCMessage
        {
            public uint MessageType { get; }
            public byte[] MessageBody { get; }

            public GCMessage(uint messageType, byte[] messageBody)
            {
                MessageType = messageType;
                MessageBody = messageBody;
            }
        }
    }
}
