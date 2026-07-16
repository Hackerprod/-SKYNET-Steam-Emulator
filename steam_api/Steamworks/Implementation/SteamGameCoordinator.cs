using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks.Interfaces;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamGameCoordinator : ISteamInterface
    {
        public static SteamGameCoordinator Instance;

        private const uint GCProtobufMask = 0x80000000u;
        private const ulong InvalidJobId = ulong.MaxValue;
        private static readonly TimeSpan ServerPollInterval = TimeSpan.FromMilliseconds(250);

        private readonly Queue<GCMessage> clientMessages;
        private readonly Queue<GCMessage> gameServerMessages;
        private readonly object inMessagesLock = new object();
        private DateTime nextServerPollUtc = DateTime.MinValue;
        private int serverPollQueued;

        public SteamGameCoordinator()
        {
            Instance = this;
            InterfaceName = "SteamGameCoordinator";
            InterfaceVersion = "SteamGameCoordinator001";
            clientMessages = new Queue<GCMessage>();
            gameServerMessages = new Queue<GCMessage>();
        }

        public void PushMessage(uint MsgType, byte[] message, ulong targetJobId = InvalidJobId)
        {
            PushMessage(MsgType, message, targetJobId, false, false);
        }

        private void PushProtoMessage(uint MsgType, byte[] message, bool gameServer)
        {
            PushMessage(MsgType, message, InvalidJobId, true, gameServer);
        }

        public void PushServerMessage(uint msgType, byte[] message, ulong targetJobId, bool protobuf)
        {
            if (protobuf && targetJobId == InvalidJobId)
            {
                PushProtoMessage(msgType, message, false);
            }
            else
            {
                PushMessage(msgType, message, targetJobId, false, false);
            }
        }

        private void PushMessage(uint MsgType, byte[] message, ulong targetJobId, bool forceProtoHeader, bool gameServer)
        {
            byte[] body = message ?? Array.Empty<byte>();
            bool wrapPayload = forceProtoHeader || targetJobId != InvalidJobId;
            byte[] payload = wrapPayload ? BuildOutgoingGcPacket(MsgType, body, targetJobId) : body;
            uint queuedMsgType = wrapPayload ? MsgType | GCProtobufMask : MsgType;

            lock (inMessagesLock)
            {
                QueueFor(gameServer).Enqueue(new GCMessage(queuedMsgType, payload));
            }

            GCMessageAvailable_t data = new GCMessageAvailable_t
            {
                m_nMessageSize = (uint)payload.Length
            };
            if (gameServer)
            {
                CallbackManager.AddCallbackGameServer(data);
            }
            else
            {
                CallbackManager.AddCallback(data);
            }
        }

        public EGCResults SendMessage(IntPtr nativeSelf, uint unMsgType, IntPtr pubData, uint cubData)
        {
            bool gameServer = InterfaceManager.IsGameServerInterfacePointer(nativeSelf);
            if (cubData > 0 && pubData == IntPtr.Zero)
            {
                Write($"SendMessage (MsgType = {GetGCMsg(unMsgType)}, MsgSize = {cubData}) = k_EGCResultInvalidMessage");
                return EGCResults.k_EGCResultInvalidMessage;
            }

            uint gcMsg = GetGCMsg(unMsgType);
            byte[] bytes = pubData.GetBytes(cubData);
            GCRequestData requestData = ExtractMessageData(gcMsg, bytes);

            if (requestData.HeaderLength > 0 || requestData.SourceJobId != InvalidJobId)
            {
                Write($"SendMessageHeader MsgType={gcMsg} HeaderSize={requestData.HeaderLength} SourceJobId={requestData.SourceJobId}");
            }

            bool handled = TryQueueServerResponses(gcMsg, requestData.Body, requestData.SourceJobId, gameServer);
            if (!handled)
            {
                Write($"GC message queued without immediate server response (AppId = {SteamEmulator.AppID}, MsgType = {gcMsg}, BodySize = {requestData.Body.Length}, Body = {ToHex(requestData.Body, 64)})");
            }

            Write($"SendMessage (MsgType = {gcMsg}, MsgSize = {cubData}) = k_EGCResultOK");
            return EGCResults.k_EGCResultOK;
        }

        private bool TryQueueServerResponses(uint requestMsg, byte[] requestBody, ulong sourceJobId, bool gameServer)
        {
            if (!APIClient.IsEnabled)
            {
                Write($"GC server disabled; no local fallback (MsgType = {requestMsg})");
                return false;
            }

            var body = requestBody ?? Array.Empty<byte>();
            return WorkQueue.Enqueue($"GC exchange {requestMsg}", () =>
            {
                var response = APIClient.ExchangeGCMessage(SteamEmulator.AppID, requestMsg, body, sourceJobId, gameServer);
                if (response == null)
                {
                    Write($"GC server exchange unavailable; no local fallback (MsgType = {requestMsg})");
                    return;
                }

                var queued = QueueServerMessages(response, requestMsg, gameServer);
                if (!response.Handled && !queued)
                {
                    Write($"GC server did not handle message (MsgType = {requestMsg})");
                }
            }, highPriority: true);
        }

        private bool QueueServerMessages(APIClient.ApiGCExchangeResponse response, uint requestMsg, bool gameServer)
        {
            bool queued = false;
            if (response != null && response.Messages != null)
            {
                foreach (var message in response.Messages)
                {
                    if (message == null || message.MessageType == 0 || message.PayloadBase64 == null)
                    {
                        continue;
                    }

                    try
                    {
                        byte[] body = Convert.FromBase64String(message.PayloadBase64);
                        ulong targetJobId = message.TargetJobId ?? InvalidJobId;
                        if (message.Protobuf && targetJobId == InvalidJobId)
                        {
                            PushProtoMessage(message.MessageType, body, gameServer);
                        }
                        else
                        {
                            PushMessage(message.MessageType, body, targetJobId, false, gameServer);
                        }

                        queued = true;
                        Write($"GC server queued response (RequestMsgType = {requestMsg}, ResponseMsgType = {message.MessageType}, MsgSize = {body.Length}, TargetJobId = {targetJobId})");
                    }
                    catch (Exception ex)
                    {
                        Write($"GC server response decode failed (RequestMsgType = {requestMsg}, ResponseMsgType = {message.MessageType}): {ex.Message}");
                    }
                }
            }

            return queued;
        }

        private bool TryPollServerMessages()
        {
            if (!APIClient.IsEnabled)
            {
                return false;
            }

            // Client-bound async GC messages arrive through the /api/events pump
            // (gc_message events). Only a logged-on game server still drains the
            // poll channel, because the event pump skips game servers.
            if (SteamEmulator.SteamGameServer == null || !SteamEmulator.SteamGameServer.LoggedIn)
            {
                return false;
            }

            DateTime now = DateTime.UtcNow;
            if (now < nextServerPollUtc)
            {
                return false;
            }

            nextServerPollUtc = now.Add(ServerPollInterval);
            if (Interlocked.Exchange(ref serverPollQueued, 1) == 1)
            {
                return false;
            }

            var queued = WorkQueue.Enqueue("GC server poll", () =>
            {
                try
                {
                    QueueServerMessages(APIClient.PollGCMessages(SteamEmulator.AppID, true), 0, true);
                }
                catch (Exception ex)
                {
                    Write($"GC server poll failed: {ex.Message}");
                }
                finally
                {
                    Interlocked.Exchange(ref serverPollQueued, 0);
                }
            }, highPriority: true);

            if (!queued)
            {
                Interlocked.Exchange(ref serverPollQueued, 0);
            }

            return queued;
        }

        public bool IsMessageAvailable(IntPtr nativeSelf, ref uint pcubMsgSize)
        {
            bool gameServer = InterfaceManager.IsGameServerInterfacePointer(nativeSelf);
            lock (inMessagesLock)
            {
                var queue = QueueFor(gameServer);
                if (queue.Count > 0)
                {
                    pcubMsgSize = (uint)queue.Peek().MessageBody.Length;
                    Write("IsMessageAvailable = True");
                    return true;
                }
            }

            if (gameServer)
            {
                TryPollServerMessages();
            }

            lock (inMessagesLock)
            {
                var queue = QueueFor(gameServer);
                if (queue.Count > 0)
                {
                    pcubMsgSize = (uint)queue.Peek().MessageBody.Length;
                    Write("IsMessageAvailable = True");
                    return true;
                }
            }

            pcubMsgSize = 0;
            Write("IsMessageAvailable = False");
            return false;
        }

        public EGCResults RetrieveMessage(IntPtr nativeSelf, ref uint punMsgType, IntPtr pubDest, uint cubDest, ref uint pcubMsgSize)
        {
            bool gameServer = InterfaceManager.IsGameServerInterfacePointer(nativeSelf);
            EGCResults result = EGCResults.k_EGCResultNoMessage;
            pcubMsgSize = 0;
            punMsgType = 0;
            lock (inMessagesLock)
            {
                var queue = QueueFor(gameServer);
                if (queue.Count > 0)
                {
                    try
                    {
                        var message = queue.Peek();
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

                            queue.Dequeue();
                            result = EGCResults.k_EGCResultOK;
                        }
                    }
                    catch (Exception ex)
                    {
                        Write($"RetrieveMessage {ex}");
                    }
                }
            }
            Write($"RetrieveMessage (MsgType = {GetGCMsg(punMsgType)}, MsgSize = {pcubMsgSize}) = {result}");
            return result;
        }

        private Queue<GCMessage> QueueFor(bool gameServer)
        {
            return gameServer ? gameServerMessages : clientMessages;
        }

        private static uint GetGCMsg(uint msg)
        {
            return msg & 0x7FFFFFFFu;
        }

        private static GCRequestData ExtractMessageData(uint expectedMsg, byte[] source)
        {
            if (source == null || source.Length < 8)
            {
                return new GCRequestData(source ?? Array.Empty<byte>(), InvalidJobId, 0);
            }

            uint embeddedMsg = BitConverter.ToUInt32(source, 0) & 0x7FFFFFFFu;
            uint headerLength = BitConverter.ToUInt32(source, 4);
            ulong offset = 8UL + headerLength;
            if (embeddedMsg != expectedMsg || offset > (ulong)source.Length)
            {
                return new GCRequestData(source, InvalidJobId, 0);
            }

            ulong sourceJobId = InvalidJobId;
            if (headerLength > 0)
            {
                byte[] header = new byte[(int)headerLength];
                Array.Copy(source, 8, header, 0, header.Length);
                TryReadFixed64Field(header, 10, out sourceJobId);
            }

            byte[] body = new byte[source.Length - (int)offset];
            Array.Copy(source, (int)offset, body, 0, body.Length);
            return new GCRequestData(body, sourceJobId, headerLength);
        }

        private static byte[] BuildOutgoingGcPacket(uint msgType, byte[] body, ulong targetJobId)
        {
            var header = new List<byte>();
            if (targetJobId != InvalidJobId)
            {
                WriteFixed64Field(header, 11, targetJobId);
            }

            byte[] payload = body ?? Array.Empty<byte>();
            byte[] packet = new byte[8 + header.Count + payload.Length];
            Buffer.BlockCopy(BitConverter.GetBytes(msgType | GCProtobufMask), 0, packet, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(header.Count), 0, packet, 4, 4);
            if (header.Count > 0)
            {
                Buffer.BlockCopy(header.ToArray(), 0, packet, 8, header.Count);
            }

            if (payload.Length > 0)
            {
                Buffer.BlockCopy(payload, 0, packet, 8 + header.Count, payload.Length);
            }

            return packet;
        }

        private static void WriteFixed64Field(List<byte> destination, int fieldNumber, ulong value)
        {
            WriteVarint(destination, ((ulong)fieldNumber << 3) | 1UL);
            destination.AddRange(BitConverter.GetBytes(value));
        }

        private static void WriteVarint(List<byte> destination, ulong value)
        {
            while (value >= 0x80)
            {
                destination.Add((byte)(value | 0x80));
                value >>= 7;
            }

            destination.Add((byte)value);
        }

        private static bool TryReadFixed64Field(byte[] source, int expectedFieldNumber, out ulong value)
        {
            value = InvalidJobId;
            int index = 0;
            while (index < source.Length)
            {
                if (!TryReadVarint(source, ref index, out ulong tag))
                {
                    return false;
                }

                int fieldNumber = (int)(tag >> 3);
                int wireType = (int)(tag & 7);
                if (wireType == 1)
                {
                    if (index + 8 > source.Length)
                    {
                        return false;
                    }

                    ulong fieldValue = BitConverter.ToUInt64(source, index);
                    index += 8;

                    if (fieldNumber == expectedFieldNumber)
                    {
                        value = fieldValue;
                        return true;
                    }
                }
                else if (!SkipField(source, ref index, wireType))
                {
                    return false;
                }
            }

            return false;
        }

        private static bool TryReadVarint(byte[] source, ref int index, out ulong value)
        {
            value = 0;
            int shift = 0;

            while (index < source.Length && shift < 64)
            {
                byte current = source[index++];
                value |= (ulong)(current & 0x7F) << shift;
                if ((current & 0x80) == 0)
                {
                    return true;
                }

                shift += 7;
            }

            return false;
        }

        private static bool SkipField(byte[] source, ref int index, int wireType)
        {
            ulong length;
            switch (wireType)
            {
                case 0:
                    return TryReadVarint(source, ref index, out _);
                case 1:
                    index += 8;
                    return index <= source.Length;
                case 2:
                    if (!TryReadVarint(source, ref index, out length))
                    {
                        return false;
                    }

                    index += (int)length;
                    return index <= source.Length;
                case 5:
                    index += 4;
                    return index <= source.Length;
                default:
                    return false;
            }
        }

        private static string ToHex(byte[] data, int maxBytes)
        {
            if (data == null || data.Length == 0)
            {
                return string.Empty;
            }

            int length = Math.Min(data.Length, Math.Max(maxBytes, 0));
            if (length == 0)
            {
                return string.Empty;
            }

            byte[] slice = new byte[length];
            Array.Copy(data, slice, length);
            string suffix = data.Length > length ? "..." : string.Empty;
            return BitConverter.ToString(slice).Replace("-", " ") + suffix;
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

        private readonly struct GCRequestData
        {
            public byte[] Body { get; }
            public ulong SourceJobId { get; }
            public uint HeaderLength { get; }

            public GCRequestData(byte[] body, ulong sourceJobId, uint headerLength)
            {
                Body = body;
                SourceJobId = sourceJobId;
                HeaderLength = headerLength;
            }
        }
    }
}
