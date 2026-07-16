using System.Text;
using SKYNET_server.Models;
using System.Collections.Concurrent;

namespace SKYNET_server.Services;

public sealed class LuaGameCoordinatorBackend : ILuaGameCoordinatorBackend
{
    private const ulong InvalidJobId = ulong.MaxValue;

    public static Action<ulong, ApiGCMessage>? PendingMessageQueued { get; set; }

    private readonly GameCoordinatorContext _context;
    private readonly ApiGCExchangeRequest _request;
    private readonly byte[] _requestBody;
    private readonly ulong _sourceJobId;

    public LuaGameCoordinatorBackend(GameCoordinatorContext context, ApiGCExchangeRequest request)
    {
        _context = context;
        _request = request;
        _requestBody = Decode(request.BodyBase64);
        _sourceJobId = request.SourceJobId ?? InvalidJobId;
        Response = new ApiGCExchangeResponse { Handled = true };
    }

    public uint MessageType => _request.MessageType;
    public uint AppId => _context.AppId;
    public uint AccountId => _context.AccountId;
    public ulong SteamId => _context.SteamId;
    public string SteamIdString => _context.SteamId.ToString(System.Globalization.CultureInfo.InvariantCulture);
    public string AccountIdString => _context.AccountId.ToString(System.Globalization.CultureInfo.InvariantCulture);
    public string PersonaName => _context.PersonaName;
    public ulong SourceJobId => _sourceJobId;
    public string BodyBase64 => Encode(_requestBody);
    public string BodyHex => Convert.ToHexString(_requestBody);
    public ApiGCExchangeResponse Response { get; }

    public bool Ignore()
    {
        Response.Handled = true;
        return true;
    }

    public bool NotHandled()
    {
        Response.Handled = false;
        return false;
    }

    public bool Reply(uint responseMsg, string payloadBase64)
    {
        return ReplyBytes(responseMsg, Decode(payloadBase64));
    }

    public bool ReplyEmpty(uint responseMsg)
    {
        return ReplyBytes(responseMsg, Array.Empty<byte>());
    }

    public bool Proto(uint messageType, string payloadBase64)
    {
        AddProto(messageType, Decode(payloadBase64));
        return true;
    }

    public bool Raw(uint messageType, string payloadBase64)
    {
        AddRaw(messageType, Decode(payloadBase64), InvalidJobId);
        return true;
    }

    public bool MessageWithTargetJob(uint messageType, string payloadBase64, ulong targetJobId)
    {
        AddRaw(messageType, Decode(payloadBase64), targetJobId);
        return true;
    }

    public bool MessageWithTargetJobString(uint messageType, string payloadBase64, string targetJobId)
    {
        return MessageWithTargetJob(messageType, payloadBase64, ParseUInt64(targetJobId));
    }

    public string Result(uint result)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, result);
        return Encode(response.ToArray());
    }

    public string FieldVarint(int fieldNumber, ulong value)
    {
        var response = new List<byte>();
        WriteVarintField(response, fieldNumber, value);
        return Encode(response.ToArray());
    }

    public string FieldVarintString(int fieldNumber, string value)
    {
        return FieldVarint(fieldNumber, ParseUInt64(value));
    }

    public string FieldBool(int fieldNumber, bool value)
    {
        return FieldVarint(fieldNumber, value ? 1UL : 0UL);
    }

    public string FieldFixed32(int fieldNumber, uint value)
    {
        var response = new List<byte>();
        WriteFixed32Field(response, fieldNumber, value);
        return Encode(response.ToArray());
    }

    public string FieldFixed64(int fieldNumber, ulong value)
    {
        var response = new List<byte>();
        WriteFixed64Field(response, fieldNumber, value);
        return Encode(response.ToArray());
    }

    public string FieldFixed64String(int fieldNumber, string value)
    {
        return FieldFixed64(fieldNumber, ParseUInt64(value));
    }

    public string FieldString(int fieldNumber, string value)
    {
        return FieldBytes(fieldNumber, Encoding.UTF8.GetBytes(value ?? string.Empty));
    }

    public string FieldBytes(int fieldNumber, string payloadBase64)
    {
        return FieldBytes(fieldNumber, Decode(payloadBase64));
    }

    public string Concat(params string[] payloadsBase64)
    {
        var response = new List<byte>();
        foreach (var payload in payloadsBase64 ?? Array.Empty<string>())
        {
            response.AddRange(Decode(payload));
        }

        return Encode(response.ToArray());
    }

    public ulong ReadVarint(int fieldNumber, ulong defaultValue = 0)
    {
        return GcWire.TryReadVarintField(_requestBody, fieldNumber, 1, out var value) ? value : defaultValue;
    }

    public ulong ReadVarintAt(int fieldNumber, int occurrence, ulong defaultValue = 0)
    {
        return GcWire.TryReadVarintField(_requestBody, fieldNumber, occurrence, out var value) ? value : defaultValue;
    }

    public string ReadVarintString(int fieldNumber, string defaultValue = "0")
    {
        return ReadVarintAtString(fieldNumber, 1, defaultValue);
    }

    public string ReadVarintAtString(int fieldNumber, int occurrence, string defaultValue = "0")
    {
        return GcWire.TryReadVarintField(_requestBody, fieldNumber, occurrence, out var value)
            ? value.ToString(System.Globalization.CultureInfo.InvariantCulture)
            : defaultValue;
    }

    public string ReadFixed64String(int fieldNumber, string defaultValue = "0")
    {
        return ReadFixed64AtString(fieldNumber, 1, defaultValue);
    }

    public string ReadFixed64AtString(int fieldNumber, int occurrence, string defaultValue = "0")
    {
        return GcWire.TryReadFixed64Field(_requestBody, fieldNumber, occurrence, out var value)
            ? value.ToString(System.Globalization.CultureInfo.InvariantCulture)
            : defaultValue;
    }

    public uint ReadFixed32(int fieldNumber, uint defaultValue = 0)
    {
        return ReadFixed32At(fieldNumber, 1, defaultValue);
    }

    public uint ReadFixed32At(int fieldNumber, int occurrence, uint defaultValue = 0)
    {
        return GcWire.TryReadFixed32Field(_requestBody, fieldNumber, occurrence, out var value)
            ? value
            : defaultValue;
    }

    public string ReadString(int fieldNumber)
    {
        return ReadStringAt(fieldNumber, 1);
    }

    public string ReadStringAt(int fieldNumber, int occurrence)
    {
        return GcWire.TryReadLengthDelimitedField(_requestBody, fieldNumber, occurrence, out var value)
            ? Encoding.UTF8.GetString(value)
            : string.Empty;
    }

    public string ReadBytes(int fieldNumber)
    {
        return ReadBytesAt(fieldNumber, 1);
    }

    public string ReadBytesAt(int fieldNumber, int occurrence)
    {
        return GcWire.TryReadLengthDelimitedField(_requestBody, fieldNumber, occurrence, out var value)
            ? Encode(value)
            : string.Empty;
    }

    public int FieldCount(int fieldNumber)
    {
        return GcWire.CountFields(_requestBody, fieldNumber);
    }

    public bool QueueTo(ulong steamId, uint messageType, string payloadBase64, bool protobuf = true)
    {
        var message = new ApiGCMessage
        {
            AppId = AppId,
            MessageType = messageType,
            PayloadBase64 = Encode(Decode(payloadBase64)),
            Protobuf = protobuf
        };

        PendingMessageQueued?.Invoke(steamId, message);
        return true;
    }

    public bool QueueToString(string steamId, uint messageType, string payloadBase64, bool protobuf = true)
    {
        return QueueTo(ParseUInt64(steamId), messageType, payloadBase64, protobuf);
    }

    public bool QueueReplyTo(ulong steamId, uint messageType, string payloadBase64, ulong targetJobId)
    {
        var message = new ApiGCMessage
        {
            AppId = AppId,
            MessageType = messageType,
            PayloadBase64 = Encode(Decode(payloadBase64)),
            TargetJobId = targetJobId == InvalidJobId ? null : targetJobId
        };

        PendingMessageQueued?.Invoke(steamId, message);
        return true;
    }

    public bool QueueReplyToString(string steamId, uint messageType, string payloadBase64, string targetJobId)
    {
        return QueueReplyTo(ParseUInt64(steamId), messageType, payloadBase64, ParseUInt64(targetJobId));
    }

    // Delivery for game-server recipients: they do not consume /api/events, they
    // drain /gamecoordinator/poll instead.
    public bool QueueToPoll(ulong steamId, uint messageType, string payloadBase64, bool protobuf = true)
    {
        GameCoordinatorPendingMessages.Enqueue(AppId, steamId, new ApiGCMessage
        {
            AppId = AppId,
            MessageType = messageType,
            PayloadBase64 = Encode(Decode(payloadBase64)),
            Protobuf = protobuf
        });

        return true;
    }

    public bool QueueToPollString(string steamId, uint messageType, string payloadBase64, bool protobuf = true)
    {
        return QueueToPoll(ParseUInt64(steamId), messageType, payloadBase64, protobuf);
    }

    public bool QueueReplyToPoll(ulong steamId, uint messageType, string payloadBase64, ulong targetJobId)
    {
        GameCoordinatorPendingMessages.Enqueue(AppId, steamId, new ApiGCMessage
        {
            AppId = AppId,
            MessageType = messageType,
            PayloadBase64 = Encode(Decode(payloadBase64)),
            TargetJobId = targetJobId == InvalidJobId ? null : targetJobId
        });

        return true;
    }

    public bool QueueReplyToPollString(string steamId, uint messageType, string payloadBase64, string targetJobId)
    {
        return QueueReplyToPoll(ParseUInt64(steamId), messageType, payloadBase64, ParseUInt64(targetJobId));
    }

    private bool ReplyBytes(uint responseMsg, byte[] payload)
    {
        if (_sourceJobId == InvalidJobId)
        {
            AddProto(responseMsg, payload);
        }
        else
        {
            AddRaw(responseMsg, payload, _sourceJobId);
        }

        return true;
    }

    private void AddProto(uint messageType, byte[] payload)
    {
        Response.Messages.Add(new ApiGCMessage
        {
            MessageType = messageType,
            PayloadBase64 = Encode(payload),
            Protobuf = true
        });
    }

    private void AddRaw(uint messageType, byte[] payload, ulong targetJobId)
    {
        Response.Messages.Add(new ApiGCMessage
        {
            MessageType = messageType,
            PayloadBase64 = Encode(payload),
            TargetJobId = targetJobId == InvalidJobId ? null : targetJobId
        });
    }

    private static byte[] Decode(string? payload)
    {
        return string.IsNullOrWhiteSpace(payload) ? Array.Empty<byte>() : Convert.FromBase64String(payload);
    }

    private static string Encode(byte[] payload)
    {
        return Convert.ToBase64String(payload ?? Array.Empty<byte>());
    }

    private static void WriteVarintField(List<byte> destination, int fieldNumber, ulong value)
    {
        WriteVarint(destination, ((ulong)fieldNumber << 3) | 0UL);
        WriteVarint(destination, value);
    }

    private static void WriteFixed64Field(List<byte> destination, int fieldNumber, ulong value)
    {
        WriteVarint(destination, ((ulong)fieldNumber << 3) | 1UL);
        destination.AddRange(BitConverter.GetBytes(value));
    }

    private static void WriteFixed32Field(List<byte> destination, int fieldNumber, uint value)
    {
        WriteVarint(destination, ((ulong)fieldNumber << 3) | 5UL);
        destination.AddRange(BitConverter.GetBytes(value));
    }

    private static string FieldBytes(int fieldNumber, byte[] value)
    {
        var response = new List<byte>();
        WriteVarint(response, ((ulong)fieldNumber << 3) | 2UL);
        WriteVarint(response, (ulong)(value?.Length ?? 0));
        if (value?.Length > 0)
        {
            response.AddRange(value);
        }

        return Encode(response.ToArray());
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

    private static ulong ParseUInt64(string value)
    {
        return ulong.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : 0UL;
    }

}

internal static class GcWire
{
    public static int CountFields(byte[] source, int expectedFieldNumber)
    {
        int count = 0;
        int index = 0;
        while (index < source.Length)
        {
            if (!TryReadVarint(source, ref index, out ulong tag))
            {
                return count;
            }

            int fieldNumber = (int)(tag >> 3);
            int wireType = (int)(tag & 7);
            if (fieldNumber == expectedFieldNumber)
            {
                count++;
            }

            if (!SkipField(source, ref index, wireType))
            {
                return count;
            }
        }

        return count;
    }

    public static bool TryReadVarintField(byte[] source, int expectedFieldNumber, int occurrence, out ulong value)
    {
        value = 0;
        int seen = 0;
        int index = 0;
        while (index < source.Length)
        {
            if (!TryReadVarint(source, ref index, out ulong tag))
            {
                return false;
            }

            int fieldNumber = (int)(tag >> 3);
            int wireType = (int)(tag & 7);
            if (wireType == 0)
            {
                if (!TryReadVarint(source, ref index, out ulong fieldValue))
                {
                    return false;
                }

                if (fieldNumber == expectedFieldNumber && ++seen == occurrence)
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

    public static bool TryReadLengthDelimitedField(byte[] source, int expectedFieldNumber, int occurrence, out byte[] value)
    {
        value = Array.Empty<byte>();
        int seen = 0;
        int index = 0;
        while (index < source.Length)
        {
            if (!TryReadVarint(source, ref index, out ulong tag))
            {
                return false;
            }

            int fieldNumber = (int)(tag >> 3);
            int wireType = (int)(tag & 7);
            if (wireType == 2)
            {
                if (!TryReadVarint(source, ref index, out ulong length) || index + (int)length > source.Length)
                {
                    return false;
                }

                if (fieldNumber == expectedFieldNumber && ++seen == occurrence)
                {
                    value = source.Skip(index).Take((int)length).ToArray();
                    return true;
                }

                index += (int)length;
            }
            else if (!SkipField(source, ref index, wireType))
            {
                return false;
            }
        }

        return false;
    }

    public static bool TryReadFixed64Field(byte[] source, int expectedFieldNumber, int occurrence, out ulong value)
    {
        value = 0;
        int seen = 0;
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

                if (fieldNumber == expectedFieldNumber && ++seen == occurrence)
                {
                    value = BitConverter.ToUInt64(source, index);
                    return true;
                }

                index += 8;
            }
            else if (!SkipField(source, ref index, wireType))
            {
                return false;
            }
        }

        return false;
    }

    public static bool TryReadFixed32Field(byte[] source, int expectedFieldNumber, int occurrence, out uint value)
    {
        value = 0;
        int seen = 0;
        int index = 0;
        while (index < source.Length)
        {
            if (!TryReadVarint(source, ref index, out ulong tag))
            {
                return false;
            }

            int fieldNumber = (int)(tag >> 3);
            int wireType = (int)(tag & 7);
            if (wireType == 5)
            {
                if (index + 4 > source.Length)
                {
                    return false;
                }

                if (fieldNumber == expectedFieldNumber && ++seen == occurrence)
                {
                    value = BitConverter.ToUInt32(source, index);
                    return true;
                }

                index += 4;
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
        switch (wireType)
        {
            case 0:
                return TryReadVarint(source, ref index, out _);
            case 1:
                index += 8;
                return index <= source.Length;
            case 2:
                if (!TryReadVarint(source, ref index, out ulong length))
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
}

public sealed class LuaGameCoordinatorRuntime
{
    private static readonly ConcurrentDictionary<string, string> Store = new(StringComparer.Ordinal);
    private static long ObjectCounter = Environment.TickCount64 & 0xFFFFFF;

    private readonly string _scriptRoot;
    private readonly ILogger _logger;
    private readonly GameCoordinatorContext _context;
    private readonly GameCoordinatorTraceService? _trace;

    public LuaGameCoordinatorRuntime(string scriptRoot, ILogger logger, GameCoordinatorContext context, GameCoordinatorTraceService? trace = null)
    {
        _scriptRoot = Path.GetFullPath(scriptRoot);
        _logger = logger;
        _context = context;
        _trace = trace;
    }

    public string SessionKey => $"{_context.AppId}:{_context.SteamId}";

    public string Fixture(string relativePath)
    {
        return Convert.ToBase64String(File.ReadAllBytes(Resolve(relativePath)));
    }

    public string Text(string relativePath)
    {
        return File.ReadAllText(Resolve(relativePath), Encoding.UTF8);
    }

    public bool Exists(string relativePath)
    {
        return File.Exists(Resolve(relativePath, requireExists: false));
    }

    public string Hex(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
        {
            return string.Empty;
        }

        return Convert.ToBase64String(Convert.FromHexString(RemoveHexWhitespace(hex)));
    }

    public string Base64Text(string value)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value ?? string.Empty));
    }

    public void Log(string message)
    {
        _logger.LogInformation("GC Lua: {Message}", message);
        _trace?.Record("log", _context.AppId, _context.SteamId, 0, 0, message ?? string.Empty);
    }

    // MoonSharp's JSON reader rejects JSON \uXXXX escapes (it expects Lua's
    // \u{...} form), so any non-ASCII or specially-escaped character in the JSON
    // (personas, item names, '<' '>' '+' '&' ...) crashes JsonToTable and, with
    // it, the whole GC exchange (e.g. a client's GCClientHello). Normalize first:
    // round-trip through System.Text.Json with relaxed escaping so those
    // characters are emitted literally instead of as \uXXXX.
    private static readonly System.Text.Json.JsonSerializerOptions LuaSafeJsonOptions = new()
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public MoonSharp.Interpreter.Table? JsonToTable(string json)
    {
        var normalized = NormalizeJsonForLua(json);
        return normalized is null
            ? null
            : MoonSharp.Interpreter.Serialization.Json.JsonTableConverter.JsonToTable(normalized);
    }

    /// <summary>
    /// Makes a JSON payload safe for MoonSharp's reader: it removes \uXXXX escapes
    /// (unsupported by MoonSharp, which expects Lua's \u{...}) and strips null
    /// values (which otherwise become a *truthy* JsonNull userdata that crashes on
    /// Lua field access, defeating `if x then` / `x == nil` guards). Returns null
    /// when the payload should map to Lua nil (empty or the JSON literal null).
    /// </summary>
    public static string? NormalizeJsonForLua(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            var node = System.Text.Json.Nodes.JsonNode.Parse(json);
            if (node is null)
            {
                return null;
            }

            StripJsonNulls(node);
            return node.ToJsonString(LuaSafeJsonOptions);
        }
        catch (System.Text.Json.JsonException)
        {
            // Not parseable by System.Text.Json; hand the original to MoonSharp,
            // which will surface its own error.
            return json;
        }
    }

    private static void StripJsonNulls(System.Text.Json.Nodes.JsonNode node)
    {
        if (node is System.Text.Json.Nodes.JsonObject obj)
        {
            var nullKeys = new List<string>();
            foreach (var pair in obj)
            {
                if (pair.Value is null)
                {
                    nullKeys.Add(pair.Key);
                }
                else
                {
                    StripJsonNulls(pair.Value);
                }
            }

            foreach (var key in nullKeys)
            {
                obj.Remove(key);
            }
        }
        else if (node is System.Text.Json.Nodes.JsonArray arr)
        {
            for (var i = arr.Count - 1; i >= 0; i--)
            {
                var item = arr[i];
                if (item is null)
                {
                    arr.RemoveAt(i);
                }
                else
                {
                    StripJsonNulls(item);
                }
            }
        }
    }

    public string TableToJson(MoonSharp.Interpreter.Table? table)
    {
        return table == null
            ? "[]"
            : MoonSharp.Interpreter.Serialization.Json.JsonTableConverter.TableToJson(table);
    }

    public uint NowUnix()
    {
        return (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public string NextObjectId()
    {
        ulong timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        ulong counter = (ulong)Interlocked.Increment(ref ObjectCounter) & 0xFFFFFFUL;
        return ((timestamp << 24) | counter).ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    public int ProtoCount(string payloadBase64, int fieldNumber)
    {
        return GcWire.CountFields(DecodePayload(payloadBase64), fieldNumber);
    }

    public ulong ProtoVarint(string payloadBase64, int fieldNumber, int occurrence = 1, ulong defaultValue = 0)
    {
        return GcWire.TryReadVarintField(DecodePayload(payloadBase64), fieldNumber, occurrence, out var value)
            ? value
            : defaultValue;
    }

    public string ProtoVarintString(string payloadBase64, int fieldNumber, int occurrence = 1, string defaultValue = "0")
    {
        return GcWire.TryReadVarintField(DecodePayload(payloadBase64), fieldNumber, occurrence, out var value)
            ? value.ToString(System.Globalization.CultureInfo.InvariantCulture)
            : defaultValue;
    }

    public string ProtoFixed64String(string payloadBase64, int fieldNumber, int occurrence = 1, string defaultValue = "0")
    {
        return GcWire.TryReadFixed64Field(DecodePayload(payloadBase64), fieldNumber, occurrence, out var value)
            ? value.ToString(System.Globalization.CultureInfo.InvariantCulture)
            : defaultValue;
    }

    public uint ProtoFixed32(string payloadBase64, int fieldNumber, int occurrence = 1, uint defaultValue = 0)
    {
        return GcWire.TryReadFixed32Field(DecodePayload(payloadBase64), fieldNumber, occurrence, out var value)
            ? value
            : defaultValue;
    }

    public string ProtoString(string payloadBase64, int fieldNumber, int occurrence = 1)
    {
        return GcWire.TryReadLengthDelimitedField(DecodePayload(payloadBase64), fieldNumber, occurrence, out var value)
            ? Encoding.UTF8.GetString(value)
            : string.Empty;
    }

    public string ProtoBytes(string payloadBase64, int fieldNumber, int occurrence = 1)
    {
        return GcWire.TryReadLengthDelimitedField(DecodePayload(payloadBase64), fieldNumber, occurrence, out var value)
            ? Convert.ToBase64String(value)
            : string.Empty;
    }

    private static byte[] DecodePayload(string? payloadBase64)
    {
        return string.IsNullOrWhiteSpace(payloadBase64) ? Array.Empty<byte>() : Convert.FromBase64String(payloadBase64);
    }

    public string StoreGet(string scope, string key)
    {
        return Store.TryGetValue(MakeStoreKey(scope, key), out var value) ? value : string.Empty;
    }

    public void StoreSet(string scope, string key, string value)
    {
        Store[MakeStoreKey(scope, key)] = value ?? string.Empty;
    }

    public void StoreDelete(string scope, string key)
    {
        Store.TryRemove(MakeStoreKey(scope, key), out _);
    }

    public string SessionGet(string key)
    {
        return StoreGet(SessionKey, key);
    }

    public void SessionSet(string key, string value)
    {
        StoreSet(SessionKey, key, value);
    }

    public void SessionDelete(string key)
    {
        StoreDelete(SessionKey, key);
    }

    private string Resolve(string relativePath, bool requireExists = true)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            throw new InvalidOperationException("GC Lua path is empty.");
        }

        var normalized = relativePath.Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.GetFullPath(Path.Combine(_scriptRoot, normalized));
        if (!fullPath.StartsWith(_scriptRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"GC Lua path escapes plugin root: {relativePath}");
        }

        if (requireExists && !File.Exists(fullPath))
        {
            throw new FileNotFoundException($"GC Lua file was not found: {relativePath}", fullPath);
        }

        return fullPath;
    }

    private string MakeStoreKey(string scope, string key)
    {
        return $"{_scriptRoot}:{scope ?? string.Empty}:{key ?? string.Empty}";
    }

    private static string RemoveHexWhitespace(string hex)
    {
        var builder = new StringBuilder(hex.Length);
        foreach (var ch in hex)
        {
            if (!char.IsWhiteSpace(ch) && ch != '-')
            {
                builder.Append(ch);
            }
        }

        return builder.ToString();
    }
}
