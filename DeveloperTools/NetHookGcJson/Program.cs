using System.Collections;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ProtoBuf;

const uint ProtoMask = 0x80000000;
const uint ClientToGcOuterMessage = 5452;
const uint ClientFromGcOuterMessage = 5453;
const ulong InvalidJobId = ulong.MaxValue;

var repoRoot = FindRepoRoot(Directory.GetCurrentDirectory());
var optionArgs = args.Skip(2).ToArray();
var queryAfter = TryReadUIntOption(optionArgs, "--after");
var windowSize = (int)(TryReadUIntOption(optionArgs, "--window") ?? 10);

var inputPath = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.GetFullPath(Path.Combine(repoRoot, ".tmp", "Lobby", "nethook"));

var outputPath = args.Length > 1
    ? Path.GetFullPath(args[1])
    : Path.Combine(inputPath, "gc-json");

if (!Directory.Exists(inputPath))
{
    Console.Error.WriteLine($"Input path does not exist: {inputPath}");
    return 2;
}

Directory.CreateDirectory(outputPath);

var generatedAssembly = typeof(CMsgClientHello).Assembly;
var resolver = new GcTypeResolver(
    generatedAssembly,
    Path.GetFullPath(Path.Combine(repoRoot, "SKYNET server", "GC", "570", "contracts")));

var dumpFiles = Directory.EnumerateFiles(inputPath, "*.bin", SearchOption.AllDirectories)
    .Where(path => Path.GetFileName(path).Contains("_5452_", StringComparison.OrdinalIgnoreCase) ||
                   Path.GetFileName(path).Contains("_5453_", StringComparison.OrdinalIgnoreCase))
    .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
    .ToList();

var options = new JsonSerializerOptions
{
    WriteIndented = true
};

var records = new List<DecodedGcRecord>();
foreach (var file in dumpFiles)
{
    var record = DecodeFile(file, inputPath, resolver);
    records.Add(record);

    var safeName = Path.GetRelativePath(inputPath, file)
        .Replace(Path.DirectorySeparatorChar, '_')
        .Replace(Path.AltDirectorySeparatorChar, '_');
    safeName = Path.ChangeExtension(safeName, ".json");
    var destination = Path.Combine(outputPath, safeName);
    File.WriteAllText(destination, JsonSerializer.Serialize(record, options), Encoding.UTF8);
}

var summary = new
{
    inputPath,
    outputPath,
    generatedAtUtc = DateTime.UtcNow,
    totalFiles = records.Count,
    decodedPayloads = records.Count(record => record.DecodeError == null),
    undecodedPayloads = records.Count(record => record.DecodeError != null),
    messages = records
        .GroupBy(record => new { record.Direction, record.GcMessageType, record.GcMessageName, record.ProtoType })
        .Select(group => new
        {
            group.Key.Direction,
            group.Key.GcMessageType,
            group.Key.GcMessageName,
            group.Key.ProtoType,
            Count = group.Count(),
            DecodeErrors = group.Count(item => item.DecodeError != null)
        })
        .OrderBy(item => item.Direction, StringComparer.Ordinal)
        .ThenBy(item => item.GcMessageType)
        .ToList(),
    records = records.Select(record => new
    {
        record.Sequence,
        record.Direction,
        record.File,
        record.AppId,
        record.GcMessageType,
        record.GcRawMessageType,
        record.GcMessageName,
        record.ProtoType,
        record.PayloadLength,
        record.SourceJobId,
        record.TargetJobId,
        record.DecodeError,
        record.RoundtripMatches
    }).ToList()
};

File.WriteAllText(Path.Combine(outputPath, "summary.json"), JsonSerializer.Serialize(summary, options), Encoding.UTF8);
WriteForensicOutputs(outputPath, records, options, windowSize, queryAfter);
Console.WriteLine($"Decoded {records.Count} GameCoordinator wrapper dumps.");
Console.WriteLine($"JSON output: {outputPath}");
Console.WriteLine($"Decoded payloads: {summary.decodedPayloads}; undecoded payloads: {summary.undecodedPayloads}");
Console.WriteLine("Forensic outputs: timeline.json, message-index.json, jobs.json, conversation-flows.json, transitions.json, after-windows.json, forensic-report.md");
return summary.undecodedPayloads == records.Count ? 1 : 0;

static uint? TryReadUIntOption(string[] args, string name)
{
    for (var i = 0; i < args.Length - 1; i++)
    {
        if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase) &&
            uint.TryParse(args[i + 1], out var value))
        {
            return value;
        }
    }

    return null;
}

static string FindRepoRoot(string start)
{
    var current = new DirectoryInfo(start);
    while (current != null)
    {
        if (Directory.Exists(Path.Combine(current.FullName, "SKYNET server")) &&
            File.Exists(Path.Combine(current.FullName, "SKYNET server", "SKYNET server.csproj")))
        {
            return current.FullName;
        }

        current = current.Parent;
    }

    throw new InvalidOperationException($"Could not locate SKYNET repo root from {start}");
}

static DecodedGcRecord DecodeFile(string file, string root, GcTypeResolver resolver)
{
    var bytes = File.ReadAllBytes(file);
    var record = new DecodedGcRecord
    {
        File = Path.GetRelativePath(root, file),
        SourceFile = file,
        Size = bytes.Length,
        Sequence = TryReadSequence(file),
        Direction = Path.GetFileName(file).Contains("_out_", StringComparison.OrdinalIgnoreCase) ? "client" : "server",
        PayloadBase64 = Convert.ToBase64String(bytes)
    };

    try
    {
        if (bytes.Length < 8)
        {
            record.DecodeError = "Steam message is too short to contain a protobuf header.";
            return record;
        }

        var rawOuter = BitConverter.ToUInt32(bytes, 0);
        record.OuterRawMessageType = rawOuter;
        record.OuterMessageType = rawOuter & ~ProtoMask;
        record.OuterProtobuf = (rawOuter & ProtoMask) != 0;
        if (!record.OuterProtobuf)
        {
            record.DecodeError = $"Outer Steam message {record.OuterMessageType} is not protobuf encoded.";
            return record;
        }

        if (record.OuterMessageType is not (ClientToGcOuterMessage or ClientFromGcOuterMessage))
        {
            record.DecodeError = $"Outer Steam message {record.OuterMessageType} is not GameCoordinator traffic.";
            return record;
        }

        var headerLength = checked((int)BitConverter.ToUInt32(bytes, 4));
        record.HeaderLength = headerLength;
        if (headerLength < 0 || 8 + headerLength > bytes.Length)
        {
            record.DecodeError = $"Invalid Steam protobuf header length {headerLength}.";
            return record;
        }

        var headerBytes = bytes.AsSpan(8, headerLength).ToArray();
        using (var headerStream = new MemoryStream(headerBytes))
        {
            var header = Serializer.Deserialize<CMsgProtoBufHeader>(headerStream);
            record.ClientSteamId = header.ShouldSerializeClientSteamId() ? header.ClientSteamId : null;
            record.SourceAppId = header.ShouldSerializeSourceAppId() ? header.SourceAppId : null;
            record.SourceJobId = NormalizeJobId(header.ShouldSerializeJobIdSource() ? header.JobIdSource : null);
            record.TargetJobId = NormalizeJobId(header.ShouldSerializeJobIdTarget() ? header.JobIdTarget : null);
            record.Header = ProtoJson.ToJsonObject(header, resolver);
        }

        var bodyBytes = bytes.AsSpan(8 + headerLength).ToArray();
        record.EnvelopeLength = bodyBytes.Length;
        using var envelopeStream = new MemoryStream(bodyBytes);
        var envelope = Serializer.Deserialize<SteamGcEnvelope>(envelopeStream);
        record.AppId = envelope.Appid;
        var gcPacket = DecodeGcPacket(envelope.Msgtype, envelope.Payload ?? Array.Empty<byte>(), resolver);
        record.GcRawMessageType = gcPacket.RawMessageType;
        record.GcMessageType = gcPacket.MessageType;
        record.GcProtobuf = gcPacket.Protobuf;
        record.GcHeaderLength = gcPacket.HeaderLength;
        record.GcHeader = gcPacket.Header;
        record.PayloadLength = gcPacket.Body.Length;
        record.PayloadBase64 = Convert.ToBase64String(gcPacket.Body);
        record.GcPacketLength = envelope.Payload?.Length ?? 0;
        record.GcPacketBase64 = Convert.ToBase64String(envelope.Payload ?? Array.Empty<byte>());
        record.SourceJobId ??= gcPacket.HeaderSourceJobId;
        record.TargetJobId ??= gcPacket.HeaderTargetJobId;
        record.GcMessageName = resolver.GetMessageName(record.GcMessageType);
        record.ProtoType = resolver.ResolveProtoType(record.GcMessageType, record.Direction)?.Name;

        if (gcPacket.Body.Length == 0)
        {
            record.Decoded = new Dictionary<string, object?> { ["empty"] = true };
            record.RoundtripMatches = true;
            return record;
        }

        var protoType = resolver.ResolveProtoType(record.GcMessageType, record.Direction);
        if (protoType == null)
        {
            record.DecodeError = $"No protobuf contract mapped for GC message {record.GcMessageType} ({record.GcMessageName}).";
            return record;
        }

        using var payloadStream = new MemoryStream(gcPacket.Body);
        var decoded = Serializer.NonGeneric.Deserialize(protoType, payloadStream);
        record.Decoded = ProtoJson.ToJsonObject(decoded, resolver);
        record.RoundtripMatches = RoundtripMatches(protoType, decoded, gcPacket.Body);
    }
    catch (Exception ex)
    {
        record.DecodeError = ex.GetType().Name + ": " + ex.Message;
    }

    return record;
}

static DecodedGcPacket DecodeGcPacket(uint envelopeMessageType, byte[] payload, GcTypeResolver resolver)
{
    if (payload.Length < 8)
    {
        return new DecodedGcPacket(
            envelopeMessageType,
            envelopeMessageType & ~ProtoMask,
            (envelopeMessageType & ProtoMask) != 0,
            0,
            null,
            null,
            null,
            payload);
    }

    var rawMessageType = BitConverter.ToUInt32(payload, 0);
    if ((rawMessageType & ~ProtoMask) != (envelopeMessageType & ~ProtoMask))
    {
        return new DecodedGcPacket(
            envelopeMessageType,
            envelopeMessageType & ~ProtoMask,
            (envelopeMessageType & ProtoMask) != 0,
            0,
            null,
            null,
            null,
            payload);
    }

    var protobuf = (rawMessageType & ProtoMask) != 0;
    if (!protobuf)
    {
        return new DecodedGcPacket(rawMessageType, rawMessageType, false, 0, null, null, null, payload.AsSpan(4).ToArray());
    }

    var headerLength = checked((int)BitConverter.ToUInt32(payload, 4));
    if (headerLength < 0 || 8 + headerLength > payload.Length)
    {
        return new DecodedGcPacket(
            envelopeMessageType,
            envelopeMessageType & ~ProtoMask,
            (envelopeMessageType & ProtoMask) != 0,
            0,
            null,
            null,
            null,
            payload);
    }

    object? headerJson = null;
    ulong? sourceJobId = null;
    ulong? targetJobId = null;
    var headerBytes = payload.AsSpan(8, headerLength).ToArray();
    using (var headerStream = new MemoryStream(headerBytes))
    {
        var header = Serializer.Deserialize<CMsgProtoBufHeader>(headerStream);
        sourceJobId = NormalizeJobId(header.ShouldSerializeJobIdSource() ? header.JobIdSource : null);
        targetJobId = NormalizeJobId(header.ShouldSerializeJobIdTarget() ? header.JobIdTarget : null);
        headerJson = ProtoJson.ToJsonObject(header, resolver);
    }

    return new DecodedGcPacket(
        rawMessageType,
        rawMessageType & ~ProtoMask,
        true,
        headerLength,
        headerJson,
        sourceJobId,
        targetJobId,
        payload.AsSpan(8 + headerLength).ToArray());
}

static ulong? NormalizeJobId(ulong? value)
{
    return value is null or InvalidJobId ? null : value;
}

static int TryReadSequence(string file)
{
    var match = Regex.Match(Path.GetFileName(file), @"^(\d+)_", RegexOptions.CultureInvariant);
    return match.Success && int.TryParse(match.Groups[1].Value, out var sequence) ? sequence : -1;
}

static bool RoundtripMatches(Type type, object value, byte[] original)
{
    try
    {
        using var stream = new MemoryStream();
        Serializer.NonGeneric.Serialize(stream, value);
        return stream.ToArray().AsSpan().SequenceEqual(original);
    }
    catch
    {
        return false;
    }
}

static void WriteForensicOutputs(
    string outputPath,
    IReadOnlyList<DecodedGcRecord> records,
    JsonSerializerOptions options,
    int windowSize,
    uint? queryAfter)
{
    var ordered = records
        .OrderBy(record => record.Sequence)
        .ThenBy(record => record.File, StringComparer.Ordinal)
        .ToList();

    var timeline = ordered.Select(ToTimelineItem).ToList();
    File.WriteAllText(Path.Combine(outputPath, "timeline.json"), JsonSerializer.Serialize(timeline, options), Encoding.UTF8);

    var messageIndex = ordered
        .GroupBy(record => record.GcMessageType)
        .OrderBy(group => group.Key)
        .Select(group => new MessageGroup
        {
            MessageType = group.Key,
            MessageName = group.Select(record => record.GcMessageName).FirstOrDefault(name => !string.IsNullOrWhiteSpace(name)) ?? string.Empty,
            ProtoTypes = group.Select(record => record.ProtoType)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.Ordinal)
                .Cast<string>()
                .ToList(),
            Count = group.Count(),
            ClientCount = group.Count(record => record.Direction == "client"),
            ServerCount = group.Count(record => record.Direction == "server"),
            DecodeErrors = group.Count(record => record.DecodeError != null),
            FirstSequence = group.Min(record => record.Sequence),
            LastSequence = group.Max(record => record.Sequence),
            Records = group.Select(ToTimelineItem).ToList()
        })
        .ToList();
    File.WriteAllText(Path.Combine(outputPath, "message-index.json"), JsonSerializer.Serialize(messageIndex, options), Encoding.UTF8);

    var jobs = BuildJobs(ordered);
    File.WriteAllText(Path.Combine(outputPath, "jobs.json"), JsonSerializer.Serialize(jobs, options), Encoding.UTF8);

    var conversationFlows = BuildConversationFlows(ordered, jobs);
    File.WriteAllText(Path.Combine(outputPath, "conversation-flows.json"), JsonSerializer.Serialize(conversationFlows, options), Encoding.UTF8);

    var transitions = BuildTransitions(ordered);
    File.WriteAllText(Path.Combine(outputPath, "transitions.json"), JsonSerializer.Serialize(transitions, options), Encoding.UTF8);

    var afterWindows = BuildAfterWindows(ordered, windowSize, queryAfter);
    File.WriteAllText(Path.Combine(outputPath, "after-windows.json"), JsonSerializer.Serialize(afterWindows, options), Encoding.UTF8);

    WriteMarkdownReport(outputPath, ordered, messageIndex, jobs, transitions);
}

static TimelineItem ToTimelineItem(DecodedGcRecord record)
{
    return new TimelineItem
    {
        Sequence = record.Sequence,
        Direction = record.Direction,
        File = record.File,
        AppId = record.AppId,
        MessageType = record.GcMessageType,
        MessageName = record.GcMessageName,
        ProtoType = record.ProtoType,
        PayloadLength = record.PayloadLength,
        SourceJobId = record.SourceJobId,
        TargetJobId = record.TargetJobId,
        DecodeError = record.DecodeError,
        RoundtripMatches = record.RoundtripMatches
    };
}

static List<JobFlow> BuildJobs(IReadOnlyList<DecodedGcRecord> ordered)
{
    var jobs = new Dictionary<ulong, JobFlow>();
    foreach (var record in ordered)
    {
        if (record.SourceJobId is { } sourceJob)
        {
            GetJob(jobs, sourceJob).ClientRequests.Add(ToJobRecord(record));
        }

        if (record.TargetJobId is { } targetJob)
        {
            GetJob(jobs, targetJob).ServerResponses.Add(ToJobRecord(record));
        }
    }

    foreach (var flow in jobs.Values)
    {
        var sequences = flow.ClientRequests.Concat(flow.ServerResponses).Select(item => item.Sequence).ToList();
        flow.FirstSequence = sequences.Count == 0 ? -1 : sequences.Min();
        flow.LastSequence = sequences.Count == 0 ? -1 : sequences.Max();
    }

    return jobs.Values
        .OrderBy(flow => flow.FirstSequence)
        .ThenBy(flow => flow.JobId)
        .ToList();
}

static JobFlow GetJob(Dictionary<ulong, JobFlow> jobs, ulong jobId)
{
    if (!jobs.TryGetValue(jobId, out var flow))
    {
        flow = new JobFlow { JobId = jobId };
        jobs[jobId] = flow;
    }

    return flow;
}

static JobRecord ToJobRecord(DecodedGcRecord record)
{
    return new JobRecord
    {
        Sequence = record.Sequence,
        Direction = record.Direction,
        File = record.File,
        MessageType = record.GcMessageType,
        MessageName = record.GcMessageName,
        ProtoType = record.ProtoType,
        DecodeError = record.DecodeError
    };
}

static List<ConversationFlow> BuildConversationFlows(IReadOnlyList<DecodedGcRecord> ordered, IReadOnlyList<JobFlow> jobs)
{
    var jobsById = jobs.ToDictionary(job => job.JobId);
    var flows = new List<ConversationFlow>();
    for (var i = 0; i < ordered.Count; i++)
    {
        var record = ordered[i];
        if (record.Direction != "client")
        {
            continue;
        }

        var immediateServerResponses = ordered
            .Skip(i + 1)
            .TakeWhile(next => next.Direction != "client")
            .Where(next => next.Direction == "server")
            .Select(ToJobRecord)
            .ToList();

        var jobResponses = record.SourceJobId is { } sourceJobId && jobsById.TryGetValue(sourceJobId, out var job)
            ? job.ServerResponses.ToList()
            : new List<JobRecord>();

        flows.Add(new ConversationFlow
        {
            Request = ToJobRecord(record),
            SourceJobId = record.SourceJobId,
            ImmediateServerResponses = immediateServerResponses,
            JobResponses = jobResponses,
            FirstResponseSequence = immediateServerResponses.Concat(jobResponses)
                .Select(item => item.Sequence)
                .DefaultIfEmpty(-1)
                .Min()
        });
    }

    return flows;
}

static List<TransitionSummary> BuildTransitions(IReadOnlyList<DecodedGcRecord> ordered)
{
    return ordered
        .Zip(ordered.Skip(1), (current, next) => new { current, next })
        .GroupBy(pair => new
        {
            FromType = pair.current.GcMessageType,
            FromName = pair.current.GcMessageName,
            FromDirection = pair.current.Direction,
            ToType = pair.next.GcMessageType,
            ToName = pair.next.GcMessageName,
            ToDirection = pair.next.Direction
        })
        .Select(group => new TransitionSummary
        {
            From = new MessageEndpoint
            {
                Direction = group.Key.FromDirection,
                MessageType = group.Key.FromType,
                MessageName = group.Key.FromName
            },
            To = new MessageEndpoint
            {
                Direction = group.Key.ToDirection,
                MessageType = group.Key.ToType,
                MessageName = group.Key.ToName
            },
            Count = group.Count(),
            ExampleSequences = group.Select(pair => pair.current.Sequence).Take(10).ToList()
        })
        .OrderByDescending(item => item.Count)
        .ThenBy(item => item.From.MessageType)
        .ThenBy(item => item.To.MessageType)
        .ToList();
}

static List<AfterWindow> BuildAfterWindows(IReadOnlyList<DecodedGcRecord> ordered, int windowSize, uint? queryAfter)
{
    var result = new List<AfterWindow>();
    for (var i = 0; i < ordered.Count; i++)
    {
        var record = ordered[i];
        if (queryAfter != null && record.GcMessageType != queryAfter.Value)
        {
            continue;
        }

        result.Add(new AfterWindow
        {
            Anchor = ToJobRecord(record),
            WindowSize = windowSize,
            Next = ordered.Skip(i + 1).Take(windowSize).Select(ToJobRecord).ToList()
        });
    }

    return result;
}

static void WriteMarkdownReport(
    string outputPath,
    IReadOnlyList<DecodedGcRecord> ordered,
    IReadOnlyList<MessageGroup> messageIndex,
    IReadOnlyList<JobFlow> jobs,
    IReadOnlyList<TransitionSummary> transitions)
{
    var unmatched = jobs.Where(job => job.ClientRequests.Count > 0 && job.ServerResponses.Count == 0).ToList();
    var report = new StringBuilder();
    report.AppendLine("# NetHook GC Forensic Report");
    report.AppendLine();
    report.AppendLine($"Generated UTC: `{DateTime.UtcNow:O}`");
    report.AppendLine($"Records: `{ordered.Count}`");
    report.AppendLine($"Decoded payloads: `{ordered.Count(record => record.DecodeError == null)}`");
    report.AppendLine($"Decode errors: `{ordered.Count(record => record.DecodeError != null)}`");
    report.AppendLine($"Unique GC message types: `{messageIndex.Count}`");
    report.AppendLine($"Tracked job ids: `{jobs.Count}`");
    report.AppendLine($"Client jobs without server response: `{unmatched.Count}`");
    report.AppendLine();
    report.AppendLine("## Hot Messages");
    foreach (var item in messageIndex.OrderByDescending(item => item.Count).ThenBy(item => item.MessageType).Take(30))
    {
        report.AppendLine($"- `{item.MessageType}` `{item.MessageName}` count={item.Count} client={item.ClientCount} server={item.ServerCount} decodeErrors={item.DecodeErrors}");
    }

    report.AppendLine();
    report.AppendLine("## Job Links");
    foreach (var job in jobs.Take(60))
    {
        report.AppendLine($"- job `{job.JobId}` requests={job.ClientRequests.Count} responses={job.ServerResponses.Count} first={job.FirstSequence} last={job.LastSequence}");
    }

    report.AppendLine();
    report.AppendLine("## Hot Transitions");
    foreach (var transition in transitions.Take(30))
    {
        report.AppendLine($"- `{transition.From.MessageType}` `{transition.From.MessageName}` {transition.From.Direction} -> `{transition.To.MessageType}` `{transition.To.MessageName}` {transition.To.Direction} count={transition.Count}");
    }

    report.AppendLine();
    report.AppendLine("## Output Files");
    report.AppendLine("- `timeline.json`: all GC records in chronological order.");
    report.AppendLine("- `message-index.json`: all records grouped by GC message type.");
    report.AppendLine("- `jobs.json`: `source_job_id` to `target_job_id` correlation.");
    report.AppendLine("- `conversation-flows.json`: each client message with immediate server replies and job-linked replies.");
    report.AppendLine("- `transitions.json`: global consecutive-message transition counts.");
    report.AppendLine("- `after-windows.json`: next records after each anchor message, optionally filtered with `--after`.");
    report.AppendLine("- Per-record JSON files: full decoded payloads and nested SO object data.");
    File.WriteAllText(Path.Combine(outputPath, "forensic-report.md"), report.ToString(), Encoding.UTF8);
}

[ProtoContract]
public sealed class SteamGcEnvelope
{
    [ProtoMember(1, Name = "appid")]
    public uint Appid { get; set; }

    [ProtoMember(2, Name = "msgtype")]
    public uint Msgtype { get; set; }

    [ProtoMember(3, Name = "payload")]
    public byte[] Payload { get; set; } = Array.Empty<byte>();
}

public sealed class DecodedGcRecord
{
    public int Sequence { get; set; }
    public string File { get; set; } = string.Empty;
    public string SourceFile { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public int Size { get; set; }
    public uint OuterRawMessageType { get; set; }
    public uint OuterMessageType { get; set; }
    public bool OuterProtobuf { get; set; }
    public int HeaderLength { get; set; }
    public int EnvelopeLength { get; set; }
    public ulong? ClientSteamId { get; set; }
    public uint? SourceAppId { get; set; }
    public ulong? SourceJobId { get; set; }
    public ulong? TargetJobId { get; set; }
    public object? Header { get; set; }
    public uint AppId { get; set; }
    public uint GcRawMessageType { get; set; }
    public uint GcMessageType { get; set; }
    public bool GcProtobuf { get; set; }
    public int GcHeaderLength { get; set; }
    public object? GcHeader { get; set; }
    public int GcPacketLength { get; set; }
    public string GcPacketBase64 { get; set; } = string.Empty;
    public string GcMessageName { get; set; } = string.Empty;
    public string? ProtoType { get; set; }
    public int PayloadLength { get; set; }
    public string PayloadBase64 { get; set; } = string.Empty;
    public object? Decoded { get; set; }
    public bool? RoundtripMatches { get; set; }
    public string? DecodeError { get; set; }
}

public sealed record DecodedGcPacket(
    uint RawMessageType,
    uint MessageType,
    bool Protobuf,
    int HeaderLength,
    object? Header,
    ulong? HeaderSourceJobId,
    ulong? HeaderTargetJobId,
    byte[] Body);

public sealed class TimelineItem
{
    public int Sequence { get; set; }
    public string Direction { get; set; } = string.Empty;
    public string File { get; set; } = string.Empty;
    public uint AppId { get; set; }
    public uint MessageType { get; set; }
    public string MessageName { get; set; } = string.Empty;
    public string? ProtoType { get; set; }
    public int PayloadLength { get; set; }
    public ulong? SourceJobId { get; set; }
    public ulong? TargetJobId { get; set; }
    public string? DecodeError { get; set; }
    public bool? RoundtripMatches { get; set; }
}

public sealed class MessageGroup
{
    public uint MessageType { get; set; }
    public string MessageName { get; set; } = string.Empty;
    public List<string> ProtoTypes { get; set; } = new();
    public int Count { get; set; }
    public int ClientCount { get; set; }
    public int ServerCount { get; set; }
    public int DecodeErrors { get; set; }
    public int FirstSequence { get; set; }
    public int LastSequence { get; set; }
    public List<TimelineItem> Records { get; set; } = new();
}

public sealed class JobFlow
{
    public ulong JobId { get; set; }
    public int FirstSequence { get; set; }
    public int LastSequence { get; set; }
    public List<JobRecord> ClientRequests { get; set; } = new();
    public List<JobRecord> ServerResponses { get; set; } = new();
}

public sealed class JobRecord
{
    public int Sequence { get; set; }
    public string Direction { get; set; } = string.Empty;
    public string File { get; set; } = string.Empty;
    public uint MessageType { get; set; }
    public string MessageName { get; set; } = string.Empty;
    public string? ProtoType { get; set; }
    public string? DecodeError { get; set; }
}

public sealed class ConversationFlow
{
    public JobRecord Request { get; set; } = new();
    public ulong? SourceJobId { get; set; }
    public int FirstResponseSequence { get; set; }
    public List<JobRecord> ImmediateServerResponses { get; set; } = new();
    public List<JobRecord> JobResponses { get; set; } = new();
}

public sealed class MessageEndpoint
{
    public string Direction { get; set; } = string.Empty;
    public uint MessageType { get; set; }
    public string MessageName { get; set; } = string.Empty;
}

public sealed class TransitionSummary
{
    public MessageEndpoint From { get; set; } = new();
    public MessageEndpoint To { get; set; } = new();
    public int Count { get; set; }
    public List<int> ExampleSequences { get; set; } = new();
}

public sealed class AfterWindow
{
    public JobRecord Anchor { get; set; } = new();
    public int WindowSize { get; set; }
    public List<JobRecord> Next { get; set; } = new();
}

public sealed class GcTypeResolver
{
    private readonly Dictionary<string, Type> _types = new(StringComparer.Ordinal);
    private readonly Dictionary<string, uint> _messageIdsByName = new(StringComparer.Ordinal);
    private readonly Dictionary<uint, string> _messageNames = new();
    private readonly Dictionary<uint, Type> _clientTypes = new();
    private readonly Dictionary<uint, Type> _serverTypes = new();
    private readonly Dictionary<int, Type> _soTypes = new();
    private readonly List<Type> _soCandidateTypes = new();

    public GcTypeResolver(Assembly generatedAssembly, string contractsPath)
    {
        RegisterTypes(generatedAssembly);
        RegisterMessageEnums(generatedAssembly);
        RegisterSoTypes();
        RegisterRoutes(contractsPath);
        RegisterSdkMessages();
    }

    public string GetMessageName(uint messageType)
    {
        return _messageNames.TryGetValue(messageType, out var name) ? name : string.Empty;
    }

    public Type? ResolveProtoType(uint messageType, string direction)
    {
        var directional = string.Equals(direction, "client", StringComparison.Ordinal)
            ? _clientTypes
            : _serverTypes;
        if (directional.TryGetValue(messageType, out var type))
        {
            return type;
        }

        var fallback = string.Equals(direction, "client", StringComparison.Ordinal)
            ? _serverTypes
            : _clientTypes;
        return fallback.TryGetValue(messageType, out type) ? type : null;
    }

    public Type? ResolveSoType(int typeId)
    {
        return _soTypes.TryGetValue(typeId, out var type) ? type : null;
    }

    public SoDecodeResult? DecodeSoObject(int typeId, byte[] bytes)
    {
        var knownType = ResolveSoType(typeId);
        if (knownType != null)
        {
            return TryDecodeSoObject(knownType, bytes) is { } decoded
                ? new SoDecodeResult(knownType, decoded, ExactRoundtripMatches(knownType, decoded, bytes), Array.Empty<string>())
                : null;
        }

        if (bytes.Length == 0)
        {
            return null;
        }

        var matches = new List<(Type Type, object Value)>();
        foreach (var candidate in _soCandidateTypes)
        {
            var decoded = TryDecodeSoObject(candidate, bytes);
            if (decoded == null || !ExactRoundtripMatches(candidate, decoded, bytes))
            {
                continue;
            }

            matches.Add((candidate, decoded));
        }

        if (matches.Count == 1)
        {
            return new SoDecodeResult(matches[0].Type, matches[0].Value, true, Array.Empty<string>());
        }

        if (matches.Count > 1)
        {
            return new SoDecodeResult(null, null, null, matches.Select(match => match.Type.Name).OrderBy(name => name, StringComparer.Ordinal).ToArray());
        }

        return null;
    }

    private void RegisterTypes(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.GetCustomAttribute<ProtoContractAttribute>() == null)
            {
                continue;
            }

            RegisterType(type);
        }
    }

    private void RegisterType(Type type)
    {
        _types[type.Name] = type;
        var contractName = type.GetCustomAttribute<ProtoContractAttribute>()?.Name;
            if (!string.IsNullOrWhiteSpace(contractName))
            {
                _types[contractName] = type;
            }

            if (type.Name.StartsWith("CSODOTA", StringComparison.Ordinal))
            {
                _soCandidateTypes.Add(type);
            }

            foreach (var nested in type.GetNestedTypes(BindingFlags.Public))
            {
                RegisterType(nested);
        }
    }

    private void RegisterMessageEnums(Assembly assembly)
    {
        foreach (var enumType in assembly.GetTypes().Where(type => type.IsEnum))
        {
            foreach (var name in Enum.GetNames(enumType))
            {
                var field = enumType.GetField(name);
                if (field == null)
                {
                    continue;
                }

                var signedValue = Convert.ToInt64(Enum.Parse(enumType, name));
                if (signedValue < 0 || signedValue > uint.MaxValue)
                {
                    continue;
                }

                var value = (uint)signedValue;
                foreach (var alias in BuildMessageAliases(name, field.GetCustomAttribute<ProtoEnumAttribute>()?.Name))
                {
                    RegisterMessageAlias(value, alias, overwriteDisplayName: false);
                }
            }
        }
    }

    private void RegisterSoTypes()
    {
        if (_types.TryGetValue(nameof(CSODOTALobby), out var lobbyType))
        {
            _soTypes[2004] = lobbyType;
        }

        if (_types.TryGetValue(nameof(CSODOTALobbyInvite), out var lobbyInviteType))
        {
            _soTypes[2013] = lobbyInviteType;
        }
    }

    private void RegisterRoutes(string contractsPath)
    {
        var routesFile = Path.Combine(contractsPath, "routes.json");
        if (!File.Exists(routesFile))
        {
            return;
        }

        using var document = JsonDocument.Parse(File.ReadAllText(routesFile));
        foreach (var route in document.RootElement.EnumerateArray())
        {
            RegisterRouteSide(route, "requestMessage", "requestProto", _clientTypes);
            RegisterRouteSide(route, "responseMessage", "responseProto", _serverTypes);
        }

        var extrasFile = Path.Combine(contractsPath, "extra-message-ids.json");
        if (File.Exists(extrasFile))
        {
            using var extras = JsonDocument.Parse(File.ReadAllText(extrasFile));
            foreach (var property in extras.RootElement.EnumerateObject())
            {
                RegisterMessageAlias(property.Value.GetUInt32(), property.Name, overwriteDisplayName: false);
            }
        }
    }

    private void RegisterRouteSide(JsonElement route, string messageProperty, string protoProperty, Dictionary<uint, Type> destination)
    {
        if (!route.TryGetProperty(messageProperty, out var messageElement) ||
            !route.TryGetProperty(protoProperty, out var protoElement))
        {
            return;
        }

        var messageName = messageElement.GetString();
        var protoName = protoElement.GetString();
        if (string.IsNullOrWhiteSpace(messageName) || string.IsNullOrWhiteSpace(protoName))
        {
            return;
        }

        var messageType = ResolveMessageId(messageName);
        if (messageType == null || !_types.TryGetValue(protoName, out var protoType))
        {
            return;
        }

        destination[messageType.Value] = protoType;
        RegisterMessageAlias(messageType.Value, messageName, overwriteDisplayName: false);
    }

    private void RegisterSdkMessages()
    {
        MapSdk(21, "SOSingleObject", nameof(CMsgSOSingleObject));
        MapSdk(24, "SOCacheSubscribed", nameof(CMsgSOCacheSubscribed));
        MapSdk(26, "SOCacheUpdated", nameof(CMsgSOMultipleObjects));
        MapSdk(28, "SOCacheSubscriptionRefresh", nameof(CMsgSOCacheSubscriptionRefresh));
        MapSdk(29, "SOCacheSubscribedUpToDate", nameof(CMsgSOCacheSubscribedUpToDate));
        MapClient(4006, "GCClientHello", nameof(CMsgClientHello));
        MapServer(4004, "GCClientWelcome", nameof(CMsgClientWelcome));
        MapServer(4009, "GCClientConnectionStatus", nameof(CMsgConnectionStatus));
        MapClient(4523, "ClientToGCAggregateMetrics", nameof(CMsgClientToGCAggregateMetrics));
        MapClient(2617, "ClientToGCCancelUnfinalizedTransactions", nameof(CMsgClientToGCCancelUnfinalizedTransactions));
        MapServer(2618, "ClientToGCCancelUnfinalizedTransactionsResponse", nameof(CMsgClientToGCCancelUnfinalizedTransactionsResponse));
        MapClient(7197, "GCMatchmakingStatsRequest", nameof(CMsgDOTAMatchmakingStatsRequest));
        MapServer(7198, "GCMatchmakingStatsResponse", nameof(CMsgDOTAMatchmakingStatsResponse));
        MapServer(2538, "GCRequestStoreSalesDataUpToDateResponse", nameof(CMsgGCRequestStoreSalesDataUpToDateResponse));
        MapServer(8136, "GCToClientTeamsInfo", nameof(CMsgDOTATeamsInfo));
        MapServer(8678, "GCToClientGuildMembershipUpdated", nameof(CMsgGCToClientGuildMembershipUpdated));
        MapServer(8772, "GCToClientOverwatchCasesAvailable", nameof(CMsgGCToClientOverwatchCasesAvailable));
    }

    private void MapSdk(uint id, string name, string protoType)
    {
        RegisterMessageAlias(id, name, overwriteDisplayName: true);
        if (_types.TryGetValue(protoType, out var type))
        {
            _serverTypes[id] = type;
            _clientTypes[id] = type;
        }
    }

    private void MapClient(uint id, string name, string protoType)
    {
        RegisterMessageAlias(id, name, overwriteDisplayName: true);
        if (_types.TryGetValue(protoType, out var type))
        {
            _clientTypes[id] = type;
        }
    }

    private void MapServer(uint id, string name, string protoType)
    {
        RegisterMessageAlias(id, name, overwriteDisplayName: true);
        if (_types.TryGetValue(protoType, out var type))
        {
            _serverTypes[id] = type;
        }
    }

    private void RegisterMessageAlias(uint id, string name, bool overwriteDisplayName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        _messageIdsByName[name] = id;
        if (overwriteDisplayName || !_messageNames.ContainsKey(id))
        {
            _messageNames[id] = name;
        }
    }

    private uint? ResolveMessageId(string messageName)
    {
        if (_messageIdsByName.TryGetValue(messageName, out var id))
        {
            return id;
        }

        return null;
    }

    private static IEnumerable<string> BuildMessageAliases(string enumFieldName, string? protoName)
    {
        foreach (var source in new[] { enumFieldName, protoName }.Where(value => !string.IsNullOrWhiteSpace(value))!)
        {
            var cleaned = source!;
            if (cleaned.StartsWith("k_EMsg", StringComparison.Ordinal))
            {
                cleaned = cleaned["k_EMsg".Length..];
            }
            else if (cleaned.StartsWith("kEMsg", StringComparison.Ordinal))
            {
                cleaned = cleaned["kEMsg".Length..];
            }
            else
            {
                continue;
            }

            yield return cleaned;
            yield return source!;
        }
    }

    private static object? TryDecodeSoObject(Type type, byte[] bytes)
    {
        try
        {
            using var stream = new MemoryStream(bytes);
            return Serializer.NonGeneric.Deserialize(type, stream);
        }
        catch
        {
            return null;
        }
    }

    private static bool ExactRoundtripMatches(Type type, object value, byte[] original)
    {
        try
        {
            using var stream = new MemoryStream();
            Serializer.NonGeneric.Serialize(stream, value);
            return stream.ToArray().AsSpan().SequenceEqual(original);
        }
        catch
        {
            return false;
        }
    }
}

public sealed record SoDecodeResult(Type? Type, object? Value, bool? RoundtripMatches, IReadOnlyList<string> CandidateTypes);

public static class ProtoJson
{
    public static object? ToJsonObject(object? value, GcTypeResolver resolver, int? parentSoType = null)
    {
        if (value == null)
        {
            return null;
        }

        if (value is string or bool or int or uint or short or ushort or byte or sbyte or float or double or decimal)
        {
            return value;
        }

        if (value is long longValue)
        {
            return longValue.ToString();
        }

        if (value is ulong ulongValue)
        {
            return ulongValue.ToString();
        }

        if (value is Enum enumValue)
        {
            return new Dictionary<string, object?>
            {
                ["name"] = enumValue.ToString(),
                ["value"] = Convert.ToInt64(enumValue)
            };
        }

        if (value is byte[] bytes)
        {
            return ByteArrayToJson(bytes, resolver, parentSoType);
        }

        if (value is IEnumerable enumerable && value is not string)
        {
            var array = new List<object?>();
            foreach (var item in enumerable)
            {
                array.Add(ToJsonObject(item, resolver, parentSoType));
            }

            return array;
        }

        return MessageToJson(value, resolver);
    }

    private static object MessageToJson(object value, GcTypeResolver resolver)
    {
        var result = new Dictionary<string, object?>
        {
            ["__type"] = value.GetType().Name
        };

        var localSoType = ReadIntProperty(value, "TypeId");
        foreach (var property in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(property => property.GetCustomAttribute<ProtoMemberAttribute>() != null))
        {
            if (!ShouldInclude(value, property))
            {
                continue;
            }

            var jsonName = JsonName(property);
            var propertyValue = property.GetValue(value);
            var soTypeForValue = property.Name.Contains("ObjectData", StringComparison.Ordinal) ? localSoType : null;
            result[jsonName] = ToJsonObject(propertyValue, resolver, soTypeForValue);
        }

        return result;
    }

    private static object ByteArrayToJson(byte[] bytes, GcTypeResolver resolver, int? parentSoType)
    {
        var result = new Dictionary<string, object?>
        {
            ["length"] = bytes.Length,
            ["base64"] = Convert.ToBase64String(bytes)
        };

        if (parentSoType != null)
        {
            var decodedObject = resolver.DecodeSoObject(parentSoType.Value, bytes);
            if (decodedObject != null)
            {
                if (decodedObject.Type != null && decodedObject.Value != null)
                {
                    result["decodedType"] = decodedObject.Type.Name;
                    result["decoded"] = ToJsonObject(decodedObject.Value, resolver);
                    result["roundtripMatches"] = decodedObject.RoundtripMatches;
                }

                if (decodedObject.CandidateTypes.Count > 0)
                {
                    result["candidateDecodedTypes"] = decodedObject.CandidateTypes;
                }
            }
        }

        return result;
    }

    private static bool ShouldInclude(object instance, PropertyInfo property)
    {
        var shouldSerialize = instance.GetType().GetMethod("ShouldSerialize" + property.Name, Type.EmptyTypes);
        if (shouldSerialize != null && shouldSerialize.ReturnType == typeof(bool))
        {
            return (bool)shouldSerialize.Invoke(instance, null)!;
        }

        var value = property.GetValue(instance);
        return value switch
        {
            null => false,
            string text => text.Length > 0,
            Array array => array.Length > 0,
            ICollection collection => collection.Count > 0,
            _ => true
        };
    }

    private static int? ReadIntProperty(object instance, string propertyName)
    {
        var property = instance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (property == null || property.PropertyType != typeof(int))
        {
            return null;
        }

        var shouldSerialize = instance.GetType().GetMethod("ShouldSerialize" + property.Name, Type.EmptyTypes);
        if (shouldSerialize != null && shouldSerialize.ReturnType == typeof(bool) &&
            !(bool)shouldSerialize.Invoke(instance, null)!)
        {
            return null;
        }

        return (int)property.GetValue(instance)!;
    }

    private static string JsonName(PropertyInfo property)
    {
        var protoName = property.GetCustomAttribute<ProtoMemberAttribute>()?.Name;
        return string.IsNullOrWhiteSpace(protoName)
            ? char.ToLowerInvariant(property.Name[0]) + property.Name[1..]
            : protoName;
    }
}
