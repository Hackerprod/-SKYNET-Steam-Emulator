using System;

namespace SKYNET.Protocol
{
    public enum P2PTransportKind
    {
        Legacy = 0,
        Messages = 1,
        SocketsOpen = 2,
        SocketsAccept = 3,
        SocketsReject = 4,
        SocketsClose = 5,
        SocketsData = 6
    }

    /// <summary>
    /// Defines the versioned wire contract shared by the Steam API client and
    /// the relay server. Strings exist only at the JSON boundary; all internal
    /// dispatch uses P2PTransportKind so unknown traffic can never fall through
    /// to another Steam networking interface.
    /// </summary>
    public static class P2PTransportProtocol
    {
        public const int LegacyVersion = 0;
        public const int CurrentVersion = 1;
        public const int MaxBatchSize = 64;

        public static string ToWireValue(P2PTransportKind transport)
        {
            switch (transport)
            {
                case P2PTransportKind.Legacy:
                    return "legacy";
                case P2PTransportKind.Messages:
                    return "messages";
                case P2PTransportKind.SocketsOpen:
                    return "sockets_open";
                case P2PTransportKind.SocketsAccept:
                    return "sockets_accept";
                case P2PTransportKind.SocketsReject:
                    return "sockets_reject";
                case P2PTransportKind.SocketsClose:
                    return "sockets_close";
                case P2PTransportKind.SocketsData:
                    return "sockets_data";
                default:
                    throw new ArgumentOutOfRangeException(nameof(transport), transport, "Unknown P2P transport.");
            }
        }

        public static bool TryParse(
            int version,
            string wireValue,
            out P2PTransportKind transport,
            out string error)
        {
            transport = P2PTransportKind.Legacy;
            error = string.Empty;

            if (version < LegacyVersion || version > CurrentVersion)
            {
                error = "Unsupported P2P transport version.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(wireValue))
            {
                if (version == LegacyVersion)
                {
                    return true;
                }

                error = "Versioned P2P frames must declare a transport.";
                return false;
            }

            switch (wireValue)
            {
                case "legacy":
                    transport = P2PTransportKind.Legacy;
                    return true;
                case "messages":
                    transport = P2PTransportKind.Messages;
                    break;
                case "sockets_open":
                    transport = P2PTransportKind.SocketsOpen;
                    break;
                case "sockets_accept":
                    transport = P2PTransportKind.SocketsAccept;
                    break;
                case "sockets_reject":
                    transport = P2PTransportKind.SocketsReject;
                    break;
                case "sockets_close":
                    transport = P2PTransportKind.SocketsClose;
                    break;
                case "sockets_data":
                    transport = P2PTransportKind.SocketsData;
                    break;
                default:
                    error = "Unknown P2P transport.";
                    return false;
            }

            if (version == LegacyVersion)
            {
                error = "Legacy P2P frames can only use the legacy transport.";
                transport = P2PTransportKind.Legacy;
                return false;
            }

            return true;
        }

        public static bool TryValidateFrame(
            int version,
            P2PTransportKind transport,
            int channel,
            int virtualPort,
            uint sourceConnectionId,
            uint targetConnectionId,
            int payloadLength,
            out string error)
        {
            error = string.Empty;
            if (version < LegacyVersion || version > CurrentVersion)
            {
                error = "Unsupported P2P transport version.";
                return false;
            }

            if (payloadLength < 0)
            {
                error = "P2P payload length is invalid.";
                return false;
            }

            if (IsSocketControl(transport) && payloadLength != 0)
            {
                error = "Socket control frames cannot contain a payload.";
                return false;
            }

            // Version 0 represents clients that predate transport multiplexing.
            // Keep that ABI readable only for the original legacy packet shape;
            // all newer transports must use the versioned envelope so their
            // metadata invariants cannot be bypassed by a downgrade.
            if (version == LegacyVersion)
            {
                if (transport != P2PTransportKind.Legacy)
                {
                    error = "Legacy P2P frames can only use the legacy transport.";
                    return false;
                }

                if (virtualPort != 0 || sourceConnectionId != 0 || targetConnectionId != 0)
                {
                    error = "Legacy P2P frames cannot carry socket metadata.";
                    return false;
                }

                return true;
            }

            switch (transport)
            {
                case P2PTransportKind.Legacy:
                case P2PTransportKind.Messages:
                    if (virtualPort != 0 || sourceConnectionId != 0 || targetConnectionId != 0)
                    {
                        error = "This P2P transport cannot carry socket metadata.";
                        return false;
                    }
                    return true;

                case P2PTransportKind.SocketsOpen:
                    return ValidateSocketFrame(
                        channel,
                        sourceConnectionId != 0 && targetConnectionId == 0,
                        "Socket open frames require only a source connection ID.",
                        out error);

                case P2PTransportKind.SocketsAccept:
                    return ValidateSocketFrame(
                        channel,
                        sourceConnectionId != 0 && targetConnectionId != 0,
                        "Socket accept frames require source and target connection IDs.",
                        out error);

                case P2PTransportKind.SocketsReject:
                    return ValidateSocketFrame(
                        channel,
                        targetConnectionId != 0,
                        "Socket reject frames require a target connection ID.",
                        out error);

                case P2PTransportKind.SocketsClose:
                case P2PTransportKind.SocketsData:
                    // A caller may close or queue data while ConnectP2P is still
                    // awaiting acceptance. The source ID identifies that flow;
                    // the target ID becomes available in the accept frame.
                    return ValidateSocketFrame(
                        channel,
                        sourceConnectionId != 0,
                        "Socket close and data frames require a source connection ID.",
                        out error);

                default:
                    error = "Unknown P2P transport.";
                    return false;
            }
        }

        public static bool TryGetDecodedPayloadLength(string payloadBase64, out int payloadLength)
        {
            payloadLength = 0;
            if (payloadBase64 == null)
            {
                return false;
            }

            var meaningfulCharacters = 0;
            var padding = 0;
            var sawPadding = false;

            foreach (var character in payloadBase64)
            {
                if (char.IsWhiteSpace(character))
                {
                    continue;
                }

                meaningfulCharacters++;
                if (character == '=')
                {
                    sawPadding = true;
                    padding++;
                    if (padding > 2)
                    {
                        return false;
                    }
                    continue;
                }

                if (sawPadding || !IsBase64Character(character))
                {
                    return false;
                }
            }

            if (meaningfulCharacters == 0)
            {
                return true;
            }

            if ((meaningfulCharacters & 3) != 0)
            {
                return false;
            }

            payloadLength = (meaningfulCharacters / 4 * 3) - padding;
            return payloadLength >= 0;
        }

        public static bool TryDecodePayload(string payloadBase64, out byte[] payload)
        {
            payload = Array.Empty<byte>();
            if (!TryGetDecodedPayloadLength(payloadBase64, out _))
            {
                return false;
            }

            try
            {
                payload = Convert.FromBase64String(payloadBase64);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static bool IsSockets(P2PTransportKind transport)
        {
            return transport >= P2PTransportKind.SocketsOpen &&
                   transport <= P2PTransportKind.SocketsData;
        }

        public static bool IsSocketControl(P2PTransportKind transport)
        {
            return transport >= P2PTransportKind.SocketsOpen &&
                   transport <= P2PTransportKind.SocketsClose;
        }

        private static bool ValidateSocketFrame(
            int channel,
            bool connectionIdsAreValid,
            string connectionIdError,
            out string error)
        {
            if (channel != 0)
            {
                error = "Socket frames cannot carry a legacy P2P channel.";
                return false;
            }

            if (!connectionIdsAreValid)
            {
                error = connectionIdError;
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static bool IsBase64Character(char value)
        {
            return (value >= 'A' && value <= 'Z') ||
                   (value >= 'a' && value <= 'z') ||
                   (value >= '0' && value <= '9') ||
                   value == '+' ||
                   value == '/';
        }
    }
}
