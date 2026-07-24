using System.Collections;
using System.Reflection;
using System.Security.Cryptography;
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
var baselinePath = TryReadStringOption(optionArgs, "--baseline");

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

var records = dumpFiles.Count > 0
    ? DecodeDumpFiles(dumpFiles, inputPath, outputPath, resolver, options)
    : LoadDecodedJsonFiles(inputPath);

if (dumpFiles.Count == 0)
{
    foreach (var record in records)
    {
        var safeName = string.IsNullOrWhiteSpace(record.File)
            ? $"{record.Sequence:D4}_{record.Direction}_{record.GcMessageType}.json"
            : record.File.Replace(Path.DirectorySeparatorChar, '_').Replace(Path.AltDirectorySeparatorChar, '_');
        safeName = Path.ChangeExtension(safeName, ".json");
        File.WriteAllText(Path.Combine(outputPath, safeName), JsonSerializer.Serialize(record, options), Encoding.UTF8);
    }
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
WriteForensicOutputs(outputPath, records, options, windowSize, queryAfter, baselinePath);
Console.WriteLine(dumpFiles.Count > 0
    ? $"Decoded {records.Count} GameCoordinator wrapper dumps."
    : $"Loaded {records.Count} decoded GameCoordinator JSON records.");
Console.WriteLine($"JSON output: {outputPath}");
Console.WriteLine($"Decoded payloads: {summary.decodedPayloads}; undecoded payloads: {summary.undecodedPayloads}");
Console.WriteLine("Forensic outputs: timeline.json, message-index.json, jobs.json, conversation-flows.json, transitions.json, after-windows.json, protocol-graph.json, pattern-summary.json, entities.json, graph-signature.json, graph-diff.json, flow-report.html, forensic-report.md");
return summary.undecodedPayloads == records.Count ? 1 : 0;

static List<DecodedGcRecord> DecodeDumpFiles(
    IReadOnlyList<string> dumpFiles,
    string inputPath,
    string outputPath,
    GcTypeResolver resolver,
    JsonSerializerOptions options)
{
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

    return records;
}

static List<DecodedGcRecord> LoadDecodedJsonFiles(string inputPath)
{
    return Directory.EnumerateFiles(inputPath, "*.json", SearchOption.AllDirectories)
        .Where(IsDecodedRecordJsonCandidate)
        .Select(TryLoadDecodedRecord)
        .Where(record => record != null)
        .Cast<DecodedGcRecord>()
        .OrderBy(record => record.Sequence)
        .ThenBy(record => record.File, StringComparer.Ordinal)
        .ToList();
}

static bool IsDecodedRecordJsonCandidate(string path)
{
    var name = Path.GetFileName(path);
    return !name.Equals("summary.json", StringComparison.OrdinalIgnoreCase) &&
           !name.Equals("timeline.json", StringComparison.OrdinalIgnoreCase) &&
           !name.Equals("message-index.json", StringComparison.OrdinalIgnoreCase) &&
           !name.Equals("jobs.json", StringComparison.OrdinalIgnoreCase) &&
           !name.Equals("conversation-flows.json", StringComparison.OrdinalIgnoreCase) &&
           !name.Equals("transitions.json", StringComparison.OrdinalIgnoreCase) &&
           !name.Equals("after-windows.json", StringComparison.OrdinalIgnoreCase) &&
           !name.Equals("protocol-graph.json", StringComparison.OrdinalIgnoreCase) &&
           !name.Equals("pattern-summary.json", StringComparison.OrdinalIgnoreCase) &&
           !name.Equals("graph-signature.json", StringComparison.OrdinalIgnoreCase);
}

static DecodedGcRecord? TryLoadDecodedRecord(string path)
{
    try
    {
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        if (!document.RootElement.TryGetProperty(nameof(DecodedGcRecord.GcMessageType), out _) &&
            !document.RootElement.TryGetProperty("gcMessageType", out _))
        {
            return null;
        }

        var record = JsonSerializer.Deserialize<DecodedGcRecord>(document.RootElement.GetRawText(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        if (record != null && string.IsNullOrWhiteSpace(record.SourceFile))
        {
            record.SourceFile = path;
        }

        return record;
    }
    catch
    {
        return null;
    }
}

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

static string? TryReadStringOption(string[] args, string name)
{
    for (var i = 0; i < args.Length - 1; i++)
    {
        if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
        {
            return args[i + 1];
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
    uint? queryAfter,
    string? baselinePath)
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

    var graph = BuildProtocolGraph(ordered, jobs, conversationFlows);
    File.WriteAllText(Path.Combine(outputPath, "protocol-graph.json"), JsonSerializer.Serialize(graph, options), Encoding.UTF8);

    var patterns = BuildPatternSummary(graph, ordered);
    File.WriteAllText(Path.Combine(outputPath, "pattern-summary.json"), JsonSerializer.Serialize(patterns, options), Encoding.UTF8);

    var entities = BuildEntities(graph.Nodes.Where(node => node.Kind is "inbound_msg" or "outbound_msg").ToList());
    File.WriteAllText(Path.Combine(outputPath, "entities.json"), JsonSerializer.Serialize(entities, options), Encoding.UTF8);

    var signature = BuildGraphSignature(graph, patterns);
    File.WriteAllText(Path.Combine(outputPath, "graph-signature.json"), JsonSerializer.Serialize(signature, options), Encoding.UTF8);

    var diff = BuildGraphDiff(signature, baselinePath);
    File.WriteAllText(Path.Combine(outputPath, "graph-diff.json"), JsonSerializer.Serialize(diff, options), Encoding.UTF8);

    WriteHtmlFlowReport(outputPath, ordered, graph, patterns, entities);
    WriteMarkdownReport(outputPath, ordered, messageIndex, jobs, transitions, patterns, diff, entities);
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

static ProtocolGraph BuildProtocolGraph(
    IReadOnlyList<DecodedGcRecord> ordered,
    IReadOnlyList<JobFlow> jobs,
    IReadOnlyList<ConversationFlow> conversationFlows)
{
    var graph = new ProtocolGraph
    {
        GeneratedAtUtc = DateTime.UtcNow,
        Records = ordered.Count
    };
    var eventNodesBySequence = new Dictionary<int, string>();

    foreach (var record in ordered)
    {
        var payloadFields = ExtractPayloadFields(record.Decoded).ToList();
        var entityHints = ExtractEntityHints(record.Decoded);
        var node = new ProtocolNode
        {
            Id = EventNodeId(record),
            Kind = record.Direction == "client" ? "inbound_msg" : "outbound_msg",
            Sequence = record.Sequence,
            Direction = record.Direction,
            MessageType = record.GcMessageType,
            MessageName = record.GcMessageName,
            ProtoType = record.ProtoType,
            SteamId = record.ClientSteamId?.ToString(),
            SourceJobId = record.SourceJobId?.ToString(),
            TargetJobId = record.TargetJobId?.ToString(),
            PayloadLength = record.PayloadLength,
            PayloadShapeHash = ShapeHash(payloadFields),
            PayloadFields = payloadFields,
            EntityHints = entityHints,
            KeyValues = ExtractEntityRefs(record.Decoded),
            Summary = ExtractLeafScalars(record.Decoded, 16),
            DecodeError = record.DecodeError
        };
        graph.Nodes.Add(node);
        eventNodesBySequence[record.Sequence] = node.Id;

        foreach (var so in ExtractSoObjects(record.Decoded, record.GcMessageType))
        {
            var soNodeId = $"so:{record.Sequence}:{so.ServiceId}:{so.OwnerType}:{so.OwnerId}:{so.TypeId}:{so.Ordinal}";
            graph.Nodes.Add(new ProtocolNode
            {
                Id = soNodeId,
                Kind = so.Action,
                Sequence = record.Sequence,
                MessageType = record.GcMessageType,
                MessageName = so.DecodedType ?? TypeIdLabel(so.TypeId),
                ServiceId = so.ServiceId,
                OwnerType = so.OwnerType,
                OwnerId = so.OwnerId,
                TypeId = so.TypeId,
                PayloadShapeHash = so.ObjectShapeHash,
                PayloadFields = so.ObjectFields
            });
            graph.Edges.Add(new ProtocolEdge
            {
                From = node.Id,
                To = soNodeId,
                Kind = so.Action == "so_create" ? "creates_so" : "updates_so",
                Confidence = "strong",
                Evidence = new List<string> { $"record {record.Sequence}", $"SO type_id={so.TypeId}" }
            });
        }
    }

    AddTemporalEdges(graph, ordered, eventNodesBySequence);
    AddJobEdges(graph, jobs, eventNodesBySequence);
    AddConversationEdges(graph, conversationFlows, eventNodesBySequence);
    AddEntityEdges(graph);
    return graph;
}

static void AddTemporalEdges(ProtocolGraph graph, IReadOnlyList<DecodedGcRecord> ordered, IReadOnlyDictionary<int, string> eventNodesBySequence)
{
    foreach (var pair in ordered.Zip(ordered.Skip(1), (current, next) => new { current, next }))
    {
        graph.Edges.Add(new ProtocolEdge
        {
            From = eventNodesBySequence[pair.current.Sequence],
            To = eventNodesBySequence[pair.next.Sequence],
            Kind = "temporal_next",
            Confidence = "exact",
            Evidence = new List<string> { $"sequence {pair.current.Sequence}->{pair.next.Sequence}" }
        });
    }
}

static void AddJobEdges(ProtocolGraph graph, IReadOnlyList<JobFlow> jobs, IReadOnlyDictionary<int, string> eventNodesBySequence)
{
    foreach (var job in jobs)
    {
        foreach (var request in job.ClientRequests)
        {
            foreach (var response in job.ServerResponses)
            {
                if (!eventNodesBySequence.TryGetValue(request.Sequence, out var from) ||
                    !eventNodesBySequence.TryGetValue(response.Sequence, out var to))
                {
                    continue;
                }

                graph.Edges.Add(new ProtocolEdge
                {
                    From = from,
                    To = to,
                    Kind = "responds_to",
                    Confidence = "exact",
                    Evidence = new List<string> { $"source_job_id/target_job_id {job.JobId}" }
                });
            }
        }
    }
}

static void AddConversationEdges(ProtocolGraph graph, IReadOnlyList<ConversationFlow> flows, IReadOnlyDictionary<int, string> eventNodesBySequence)
{
    foreach (var flow in flows)
    {
        if (!eventNodesBySequence.TryGetValue(flow.Request.Sequence, out var requestNode))
        {
            continue;
        }

        foreach (var response in flow.ImmediateServerResponses)
        {
            if (!eventNodesBySequence.TryGetValue(response.Sequence, out var responseNode))
            {
                continue;
            }

            graph.Edges.Add(new ProtocolEdge
            {
                From = requestNode,
                To = responseNode,
                Kind = "causes_immediate",
                Confidence = flow.SourceJobId == null ? "strong" : "inferred",
                Evidence = new List<string> { $"server response after request seq {flow.Request.Sequence}" }
            });
        }
    }
}

static void AddEntityEdges(ProtocolGraph graph)
{
    var entityNodes = graph.Nodes
        .Where(node => node.Kind is "inbound_msg" or "outbound_msg" && node.EntityHints.Count > 0)
        .OrderBy(node => node.Sequence)
        .ToList();

    foreach (var group in entityNodes.SelectMany(node => node.EntityHints.Select(hint => new { hint, node }))
                 .GroupBy(item => item.hint, StringComparer.Ordinal))
    {
        var ordered = group.Select(item => item.node).OrderBy(node => node.Sequence).ToList();
        foreach (var pair in ordered.Zip(ordered.Skip(1), (current, next) => new { current, next }))
        {
            graph.Edges.Add(new ProtocolEdge
            {
                From = pair.current.Id,
                To = pair.next.Id,
                Kind = "same_entity",
                Confidence = "strong",
                Evidence = new List<string> { group.Key }
            });
        }
    }
}

static List<FlowPattern> BuildPatternSummary(ProtocolGraph graph, IReadOnlyList<DecodedGcRecord> ordered)
{
    var patterns = new List<FlowPattern>();
    foreach (var request in ordered.Where(record => record.Direction == "client"))
    {
        var requestNode = EventNodeId(request);
        var outbound = graph.Edges
            .Where(edge => edge.From == requestNode && edge.Kind is "responds_to" or "causes_immediate")
            .Select(edge => graph.Nodes.FirstOrDefault(node => node.Id == edge.To))
            .Where(node => node != null && node.Kind == "outbound_msg")
            .Cast<ProtocolNode>()
            .OrderBy(node => node.Sequence)
            .ToList();

        var soMutations = graph.Edges
            .Where(edge => edge.From == requestNode && edge.Kind is "creates_so" or "updates_so")
            .Select(edge => graph.Nodes.FirstOrDefault(node => node.Id == edge.To))
            .Where(node => node != null)
            .Cast<ProtocolNode>()
            .ToList();
        var outboundNodeIds = outbound.Select(node => node.Id).ToHashSet(StringComparer.Ordinal);
        soMutations.AddRange(graph.Edges
            .Where(edge => outboundNodeIds.Contains(edge.From) && edge.Kind is "creates_so" or "updates_so")
            .Select(edge => graph.Nodes.FirstOrDefault(node => node.Id == edge.To))
            .Where(node => node != null)
            .Cast<ProtocolNode>());
        soMutations = soMutations.DistinctBy(node => node.Id).ToList();

        patterns.Add(new FlowPattern
        {
            Name = PatternName(request),
            Trigger = new PatternMessage
            {
                Sequence = request.Sequence,
                MessageType = request.GcMessageType,
                MessageName = request.GcMessageName,
                Direction = request.Direction
            },
            Responses = outbound.Select(node => new PatternMessage
            {
                Sequence = node.Sequence,
                MessageType = node.MessageType,
                MessageName = node.MessageName,
                Direction = node.Direction
            }).ToList(),
            SoMutations = soMutations.Select(node => new PatternSo
            {
                Sequence = node.Sequence,
                Action = node.Kind,
                TypeId = node.TypeId,
                OwnerType = node.OwnerType,
                OwnerId = node.OwnerId,
                ServiceId = node.ServiceId,
                ShapeHash = node.PayloadShapeHash
            }).ToList(),
            Status = request.DecodeError == null ? "decoded" : "decode_error"
        });
    }

    return patterns;
}

static GraphSignature BuildGraphSignature(ProtocolGraph graph, IReadOnlyList<FlowPattern> patterns)
{
    var messageShapes = graph.Nodes
        .Where(node => node.Kind is "inbound_msg" or "outbound_msg")
        .GroupBy(node => $"{node.Direction}:{node.MessageType}:{node.ProtoType}:{node.PayloadShapeHash}", StringComparer.Ordinal)
        .Select(group => new SignatureEntry
        {
            Key = group.Key,
            Count = group.Count(),
            FirstSequence = group.Min(node => node.Sequence),
            LastSequence = group.Max(node => node.Sequence)
        })
        .OrderBy(item => item.Key, StringComparer.Ordinal)
        .ToList();

    var edgeShapes = graph.Edges
        .Select(edge => new
        {
            edge.Kind,
            From = graph.Nodes.FirstOrDefault(node => node.Id == edge.From)?.MessageType,
            To = graph.Nodes.FirstOrDefault(node => node.Id == edge.To)?.MessageType
        })
        .GroupBy(item => $"{item.Kind}:{item.From}->{item.To}", StringComparer.Ordinal)
        .Select(group => new SignatureEntry
        {
            Key = group.Key,
            Count = group.Count()
        })
        .OrderBy(item => item.Key, StringComparer.Ordinal)
        .ToList();

    return new GraphSignature
    {
        GeneratedAtUtc = DateTime.UtcNow,
        MessageShapes = messageShapes,
        EdgeShapes = edgeShapes,
        PatternShapes = patterns.Select(pattern => new SignatureEntry
        {
            Key = $"{pattern.Name}:{pattern.Trigger.MessageType}->{string.Join(",", pattern.Responses.Select(response => response.MessageType))}",
            Count = 1,
            FirstSequence = pattern.Trigger.Sequence,
            LastSequence = pattern.Responses.Select(response => response.Sequence).DefaultIfEmpty(pattern.Trigger.Sequence).Max()
        }).ToList()
    };
}

static GraphDiff BuildGraphDiff(GraphSignature current, string? baselinePath)
{
    var diff = new GraphDiff
    {
        GeneratedAtUtc = DateTime.UtcNow,
        CurrentGeneratedAtUtc = current.GeneratedAtUtc,
        BaselinePath = baselinePath
    };

    if (string.IsNullOrWhiteSpace(baselinePath))
    {
        diff.Status = "no_baseline";
        return diff;
    }

    var signaturePath = Directory.Exists(baselinePath)
        ? Path.Combine(baselinePath, "graph-signature.json")
        : baselinePath;
    signaturePath = Path.GetFullPath(signaturePath);
    diff.BaselinePath = signaturePath;

    if (!File.Exists(signaturePath))
    {
        diff.Status = "baseline_missing";
        diff.Notes.Add($"Baseline signature not found: {signaturePath}");
        return diff;
    }

    try
    {
        var baseline = JsonSerializer.Deserialize<GraphSignature>(File.ReadAllText(signaturePath), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        if (baseline == null)
        {
            diff.Status = "baseline_invalid";
            return diff;
        }

        diff.BaselineGeneratedAtUtc = baseline.GeneratedAtUtc;
        diff.MessageShapeChanges = CompareSignatureEntries(baseline.MessageShapes, current.MessageShapes);
        diff.EdgeShapeChanges = CompareSignatureEntries(baseline.EdgeShapes, current.EdgeShapes);
        diff.PatternShapeChanges = CompareSignatureEntries(baseline.PatternShapes, current.PatternShapes);
        diff.Status = diff.MessageShapeChanges.Count == 0 && diff.EdgeShapeChanges.Count == 0 && diff.PatternShapeChanges.Count == 0
            ? "same"
            : "changed";
        return diff;
    }
    catch (Exception ex)
    {
        diff.Status = "baseline_error";
        diff.Notes.Add(ex.GetType().Name + ": " + ex.Message);
        return diff;
    }
}

static List<GraphDiffEntry> CompareSignatureEntries(IReadOnlyList<SignatureEntry> baseline, IReadOnlyList<SignatureEntry> current)
{
    var result = new List<GraphDiffEntry>();
    var baselineByKey = CollapseSignatureEntries(baseline);
    var currentByKey = CollapseSignatureEntries(current);
    foreach (var key in baselineByKey.Keys.Union(currentByKey.Keys, StringComparer.Ordinal).OrderBy(value => value, StringComparer.Ordinal))
    {
        var hasBaseline = baselineByKey.TryGetValue(key, out var baselineEntry);
        var hasCurrent = currentByKey.TryGetValue(key, out var currentEntry);
        if (!hasBaseline && currentEntry != null)
        {
            result.Add(new GraphDiffEntry { Kind = "added", Key = key, CurrentCount = currentEntry.Count });
            continue;
        }

        if (!hasCurrent && baselineEntry != null)
        {
            result.Add(new GraphDiffEntry { Kind = "removed", Key = key, BaselineCount = baselineEntry.Count });
            continue;
        }

        if (baselineEntry != null && currentEntry != null && baselineEntry.Count != currentEntry.Count)
        {
            result.Add(new GraphDiffEntry
            {
                Kind = "count_changed",
                Key = key,
                BaselineCount = baselineEntry.Count,
                CurrentCount = currentEntry.Count
            });
        }
    }

    return result;
}

static Dictionary<string, SignatureEntry> CollapseSignatureEntries(IReadOnlyList<SignatureEntry> entries)
{
    return entries
        .GroupBy(item => item.Key, StringComparer.Ordinal)
        .ToDictionary(
            group => group.Key,
            group => new SignatureEntry
            {
                Key = group.Key,
                Count = group.Sum(item => item.Count),
                FirstSequence = group.Min(item => item.FirstSequence),
                LastSequence = group.Max(item => item.LastSequence)
            },
            StringComparer.Ordinal);
}

static IEnumerable<string> ExtractPayloadFields(object? decoded)
{
    if (decoded == null)
    {
        return Array.Empty<string>();
    }

    var fields = new List<string>();
    WalkJson(JsonSerializer.SerializeToElement(decoded), "$", fields);
    return fields.Distinct(StringComparer.Ordinal).OrderBy(value => value, StringComparer.Ordinal);
}

static void WalkJson(JsonElement element, string path, List<string> fields)
{
    switch (element.ValueKind)
    {
        case JsonValueKind.Object:
            foreach (var property in element.EnumerateObject())
            {
                var childPath = $"{path}.{property.Name}";
                fields.Add(childPath);
                WalkJson(property.Value, childPath, fields);
            }
            break;
        case JsonValueKind.Array:
            var indexPath = $"{path}[]";
            fields.Add(indexPath);
            foreach (var item in element.EnumerateArray().Take(5))
            {
                WalkJson(item, indexPath, fields);
            }
            break;
    }
}

static string ShapeHash(IEnumerable<string> fields)
{
    var text = string.Join('\n', fields);
    var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(text));
    return Convert.ToHexString(bytes).ToLowerInvariant()[..16];
}

static List<string> ExtractEntityHints(object? decoded)
{
    if (decoded == null)
    {
        return new List<string>();
    }

    var hints = new List<string>();
    CollectEntityHints(JsonSerializer.SerializeToElement(decoded), hints);
    return hints.Distinct(StringComparer.Ordinal).OrderBy(value => value, StringComparer.Ordinal).ToList();
}

static void CollectEntityHints(JsonElement element, List<string> hints)
{
    if (element.ValueKind == JsonValueKind.Object)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (property.Value.ValueKind is JsonValueKind.String or JsonValueKind.Number &&
                IsEntityField(property.Name) &&
                TryReadScalar(property.Value, out var value) &&
                !string.IsNullOrWhiteSpace(value) &&
                value != "0")
            {
                hints.Add($"{property.Name}:{value}");
            }

            CollectEntityHints(property.Value, hints);
        }
    }
    else if (element.ValueKind == JsonValueKind.Array)
    {
        foreach (var item in element.EnumerateArray())
        {
            CollectEntityHints(item, hints);
        }
    }
}

static bool IsEntityField(string name)
{
    return name.EndsWith("lobby_id", StringComparison.OrdinalIgnoreCase) ||
           name.EndsWith("party_id", StringComparison.OrdinalIgnoreCase) ||
           name.EndsWith("match_id", StringComparison.OrdinalIgnoreCase) ||
           name.EndsWith("owner_id", StringComparison.OrdinalIgnoreCase) ||
           name.EndsWith("group_id", StringComparison.OrdinalIgnoreCase) ||
           name.EndsWith("steam_id", StringComparison.OrdinalIgnoreCase) ||
           name.EndsWith("account_id", StringComparison.OrdinalIgnoreCase);
}

// Generic id-like detector: any *_id / id field, minus structural metadata.
// No message-specific hardcoding so the tracker works on any GC proto.
static bool IsEntityRefField(string name)
{
    var lower = name.ToLowerInvariant();
    if (lower is "type_id" or "service_id" or "owner_type" or "version" or "eresult")
    {
        return false;
    }

    return IsEntityField(name) || lower == "id" || lower.EndsWith("_id", StringComparison.Ordinal);
}

// Value-level entity references: (field name, json path, concrete value) for every
// id-like scalar in the decoded payload. This is what lets an agent see the same
// steam_id / lobby_id / account_id resurface across messages.
static List<EntityRef> ExtractEntityRefs(object? decoded)
{
    if (decoded == null)
    {
        return new List<EntityRef>();
    }

    var list = new List<EntityRef>();
    CollectEntityRefs(JsonSerializer.SerializeToElement(decoded), "$", list);
    return list
        .GroupBy(reference => $"{reference.Name}|{reference.Path}|{reference.Value}", StringComparer.Ordinal)
        .Select(group => group.First())
        .OrderBy(reference => reference.Path, StringComparer.Ordinal)
        .ToList();
}

static void CollectEntityRefs(JsonElement element, string path, List<EntityRef> list)
{
    if (element.ValueKind == JsonValueKind.Object)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (property.Name is "base64" or "length" or "__type")
            {
                continue;
            }

            var childPath = $"{path}.{property.Name}";
            if (property.Value.ValueKind is JsonValueKind.String or JsonValueKind.Number &&
                IsEntityRefField(property.Name) &&
                TryReadScalar(property.Value, out var value) &&
                !string.IsNullOrWhiteSpace(value) &&
                value != "0")
            {
                list.Add(new EntityRef { Name = property.Name, Path = childPath, Value = value });
            }

            CollectEntityRefs(property.Value, childPath, list);
        }
    }
    else if (element.ValueKind == JsonValueKind.Array)
    {
        var index = 0;
        foreach (var item in element.EnumerateArray())
        {
            if (index >= 64)
            {
                break;
            }

            CollectEntityRefs(item, $"{path}[{index}]", list);
            index++;
        }
    }
}

// Compact "what does this message contain" digest: leaf scalar path=value pairs.
static List<string> ExtractLeafScalars(object? decoded, int max)
{
    if (decoded == null)
    {
        return new List<string>();
    }

    var pairs = new List<(string Path, string Value)>();
    CollectLeafScalars(JsonSerializer.SerializeToElement(decoded), string.Empty, pairs);
    return pairs.Take(max).Select(pair => $"{pair.Path}={pair.Value}").ToList();
}

static void CollectLeafScalars(JsonElement element, string path, List<(string Path, string Value)> pairs)
{
    if (pairs.Count >= 48)
    {
        return;
    }

    switch (element.ValueKind)
    {
        case JsonValueKind.Object:
            foreach (var property in element.EnumerateObject())
            {
                if (property.Name is "base64" or "length" or "__type" or "candidateDecodedTypes")
                {
                    continue;
                }

                CollectLeafScalars(property.Value, path.Length == 0 ? property.Name : $"{path}.{property.Name}", pairs);
            }
            break;
        case JsonValueKind.Array:
            var index = 0;
            foreach (var item in element.EnumerateArray())
            {
                if (index >= 6)
                {
                    break;
                }

                CollectLeafScalars(item, $"{path}[{index}]", pairs);
                index++;
            }
            break;
        default:
            if (TryReadScalar(element, out var value))
            {
                if (value.Length > 48)
                {
                    value = value[..48] + "...";
                }

                pairs.Add((path, value));
            }
            break;
    }
}

// Generic entity ledger: group every id-like value by its concrete value and list
// each message that carries it, ordered in time. Cross-message values (MessageCount >= 2)
// are the correlation links an agent can follow.
static List<EntityTrack> BuildEntities(IReadOnlyList<ProtocolNode> messageNodes)
{
    var map = new Dictionary<string, EntityTrack>(StringComparer.Ordinal);
    foreach (var node in messageNodes.OrderBy(node => node.Sequence))
    {
        foreach (var reference in node.KeyValues)
        {
            if (!map.TryGetValue(reference.Value, out var track))
            {
                track = new EntityTrack { Value = reference.Value };
                map[reference.Value] = track;
            }

            track.Names.Add(reference.Name);
            track.Occurrences.Add(new EntityOccurrence
            {
                Sequence = node.Sequence,
                Direction = node.Direction,
                MessageType = node.MessageType,
                MessageName = node.MessageName,
                FieldName = reference.Name,
                Path = reference.Path
            });
        }
    }

    foreach (var track in map.Values)
    {
        track.MessageCount = track.Occurrences.Select(occurrence => occurrence.Sequence).Distinct().Count();
        track.FirstSequence = track.Occurrences.Min(occurrence => occurrence.Sequence);
        track.LastSequence = track.Occurrences.Max(occurrence => occurrence.Sequence);
        track.Confidence = EntityConfidence(track);
    }

    return map.Values
        .OrderBy(track => track.Confidence switch { "strong" => 0, "medium" => 1, _ => 2 })
        .ThenByDescending(track => track.MessageCount)
        .ThenByDescending(track => track.Occurrences.Count)
        .ThenBy(track => track.FirstSequence)
        .ToList();
}

// Generic confidence, no message-specific tuning: a big/unique value shared across
// messages is a real entity link; a tiny int shared across different field names is
// almost always an enum collision (action_id 4 == token_id 4), so mark it weak.
static string EntityConfidence(EntityTrack track)
{
    if (!long.TryParse(track.Value, out var numeric))
    {
        return "strong";
    }

    if (Math.Abs(numeric) >= 1_000_000)
    {
        return "strong";
    }

    return track.Names.Count == 1 ? "medium" : "weak";
}

static List<SoObjectSummary> ExtractSoObjects(object? decoded, uint messageType)
{
    if (decoded == null)
    {
        return new List<SoObjectSummary>();
    }

    var result = new List<SoObjectSummary>();
    CollectSoObjects(JsonSerializer.SerializeToElement(decoded), result, new SoObjectSummary
    {
        Action = messageType switch
        {
            21 or 24 => "so_create",
            26 or 28 => "so_update",
            _ => "so_update"
        }
    });
    for (var i = 0; i < result.Count; i++)
    {
        result[i].Ordinal = i;
    }

    return result;
}

static void CollectSoObjects(JsonElement element, List<SoObjectSummary> result, SoObjectSummary? inherited = null)
{
    if (element.ValueKind == JsonValueKind.Object)
    {
        var current = inherited?.Clone() ?? new SoObjectSummary();
        if (TryGetJsonProperty(element, "service_id", out var serviceId) && TryReadInt(serviceId, out var serviceIdValue))
        {
            current.ServiceId = serviceIdValue;
        }

        if (TryGetJsonProperty(element, "owner_type", out var ownerType) && TryReadInt(ownerType, out var ownerTypeValue))
        {
            current.OwnerType = ownerTypeValue;
        }

        if (TryGetJsonProperty(element, "owner_id", out var ownerId) && TryReadScalar(ownerId, out var ownerIdValue))
        {
            current.OwnerId = ownerIdValue;
        }

        if (TryGetJsonProperty(element, "owner_soid", out var ownerSoid) && ownerSoid.ValueKind == JsonValueKind.Object)
        {
            if (TryGetJsonProperty(ownerSoid, "type", out var ownerSoidType) && TryReadInt(ownerSoidType, out var ownerSoidTypeValue))
            {
                current.OwnerType = ownerSoidTypeValue;
            }

            if (TryGetJsonProperty(ownerSoid, "id", out var ownerSoidId) && TryReadScalar(ownerSoidId, out var ownerSoidIdValue))
            {
                current.OwnerId = ownerSoidIdValue;
            }
        }

        if (TryGetJsonProperty(element, "type_id", out var typeId) && TryReadInt(typeId, out var typeIdValue))
        {
            current.TypeId = typeIdValue;
        }

        if (TryGetJsonProperty(element, "object_data", out var objectData))
        {
            current.ObjectFields = ExtractPayloadFields(objectData).ToList();
            current.ObjectShapeHash = ShapeHash(current.ObjectFields);
            if (objectData.ValueKind == JsonValueKind.Object &&
                TryGetJsonProperty(objectData, "decodedType", out var decodedType) &&
                decodedType.ValueKind == JsonValueKind.String)
            {
                current.DecodedType = decodedType.GetString();
            }

            if (current.TypeId != null)
            {
                result.Add(current.Clone());
            }
        }

        if (TryGetJsonProperty(element, "objects", out var objects) && objects.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in objects.EnumerateArray())
            {
                var nested = current.Clone();
                CollectSoObjects(item, result, nested);
            }
        }

        foreach (var property in element.EnumerateObject())
        {
            if (property.Name is "objects" or "object_data")
            {
                continue;
            }

            CollectSoObjects(property.Value, result, current);
        }
    }
    else if (element.ValueKind == JsonValueKind.Array)
    {
        foreach (var item in element.EnumerateArray())
        {
            CollectSoObjects(item, result, inherited);
        }
    }
}

static bool TryGetJsonProperty(JsonElement element, string name, out JsonElement value)
{
    if (element.TryGetProperty(name, out value))
    {
        return true;
    }

    var camel = SnakeToCamel(name);
    return element.TryGetProperty(camel, out value);
}

static string SnakeToCamel(string value)
{
    var parts = value.Split('_', StringSplitOptions.RemoveEmptyEntries);
    if (parts.Length == 0)
    {
        return value;
    }

    return parts[0] + string.Concat(parts.Skip(1).Select(part => char.ToUpperInvariant(part[0]) + part[1..]));
}

static bool TryReadInt(JsonElement element, out int value)
{
    if (element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out value))
    {
        return true;
    }

    if (element.ValueKind == JsonValueKind.String && int.TryParse(element.GetString(), out value))
    {
        return true;
    }

    value = 0;
    return false;
}

static bool TryReadScalar(JsonElement element, out string value)
{
    value = element.ValueKind switch
    {
        JsonValueKind.String => element.GetString() ?? string.Empty,
        JsonValueKind.Number => element.GetRawText(),
        JsonValueKind.True => "true",
        JsonValueKind.False => "false",
        _ => string.Empty
    };
    return value.Length > 0;
}

static string EventNodeId(DecodedGcRecord record)
{
    return $"event:{record.Sequence}:{record.Direction}:{record.GcMessageType}";
}

static string PatternName(DecodedGcRecord record)
{
    return string.IsNullOrWhiteSpace(record.GcMessageName)
        ? $"msg_{record.GcMessageType}"
        : record.GcMessageName;
}

static string TypeIdLabel(int? typeId)
{
    return typeId switch
    {
        1 => "SO Econ Item",
        7 => "SO Econ Game Account",
        2002 => "SO Dota Game Account",
        2003 => "SO Party",
        2004 => "SO Lobby",
        2006 => "SO Party Invite",
        2010 => "SO Player Challenge",
        2011 => "SO Lobby Invite",
        2012 => "SO Dota Plus Account",
        2013 => "SO Dynamic Lobby/Lobby Invite Cache",
        2014 => "SO Static Lobby",
        2015 => "SO Server Lobby",
        2016 => "SO Server Static Lobby",
        _ => typeId?.ToString() ?? string.Empty
    };
}

static void WriteMarkdownReport(
    string outputPath,
    IReadOnlyList<DecodedGcRecord> ordered,
    IReadOnlyList<MessageGroup> messageIndex,
    IReadOnlyList<JobFlow> jobs,
    IReadOnlyList<TransitionSummary> transitions,
    IReadOnlyList<FlowPattern> patterns,
    GraphDiff diff,
    IReadOnlyList<EntityTrack> entities)
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

    var clientCount = ordered.Count(record => record.Direction == "client");
    var serverCount = ordered.Count - clientCount;
    var correlatedJobs = jobs.Count(job => job.ClientRequests.Count > 0 && job.ServerResponses.Count > 0);
    var hello = ordered.FirstOrDefault(record => record.GcMessageName.Contains("ClientHello", StringComparison.OrdinalIgnoreCase));
    var welcome = ordered.FirstOrDefault(record => record.GcMessageName.Contains("ClientWelcome", StringComparison.OrdinalIgnoreCase));
    var soTypeLabels = patterns
        .SelectMany(pattern => pattern.SoMutations)
        .Select(mutation => TypeIdLabel(mutation.TypeId))
        .Where(label => !string.IsNullOrWhiteSpace(label))
        .Distinct(StringComparer.Ordinal)
        .OrderBy(label => label, StringComparer.Ordinal)
        .ToList();

    report.AppendLine("## Communication Model (agent mental map)");
    report.AppendLine();
    report.AppendLine("Three actors talk over the GameCoordinator channel:");
    report.AppendLine();
    report.AppendLine("- **Client** (the game) sends requests to the GC.");
    report.AppendLine("- **Game Coordinator (GC)** answers and pushes state.");
    report.AppendLine("- **SO Cache** is the SharedObject state the GC replicates into the client. GC messages create/update it; that is the state machine to reimplement.");
    report.AppendLine();
    report.AppendLine($"Direction split: `{clientCount}` client->GC and `{serverCount}` GC->client messages.");
    if (hello != null && welcome != null)
    {
        report.AppendLine($"Session bootstrap: `{hello.GcMessageName}` (seq {hello.Sequence}) -> `{welcome.GcMessageName}` (seq {welcome.Sequence}) opens the GC session and seeds the initial SO cache subscription.");
    }

    report.AppendLine($"Request/response correlation: `{correlatedJobs}` job id(s) pair a client request with a server reply exactly (`responds_to`); rely on these to know which reply answers which request. Everything else is only timing-inferred (`causes_immediate`).");
    if (soTypeLabels.Count > 0)
    {
        report.AppendLine($"SharedObject types mutated in this capture: {string.Join(", ", soTypeLabels.Select(label => $"`{label}`"))}.");
    }

    report.AppendLine();
    report.AppendLine("Open `flow-report.html` for the interactive sequence-diagram canvas of this same flow.");
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
    report.AppendLine("## Protocol Patterns");
    foreach (var pattern in patterns.Take(60))
    {
        var responses = pattern.Responses.Count == 0
            ? "none"
            : string.Join(", ", pattern.Responses.Select(response => $"{response.MessageType} {response.MessageName}".Trim()));
        var so = pattern.SoMutations.Count == 0
            ? "none"
            : string.Join(", ", pattern.SoMutations.Select(item => $"{item.Action}:type={item.TypeId}:owner={item.OwnerType}"));
        report.AppendLine($"- `{pattern.Trigger.MessageType}` `{pattern.Name}` seq={pattern.Trigger.Sequence} responses=[{responses}] so=[{so}] status={pattern.Status}");
    }

    report.AppendLine();
    report.AppendLine("## Entity Tracking (value correlation)");
    report.AppendLine("Concrete id/steam_id/account_id values that surface in more than one message, with the time-ordered chain of messages (and the exact field) that share each value. Follow a chain to see which request carries an id and which later message echoes it back.");
    report.AppendLine();
    var crossEntities = entities.Where(entity => entity.MessageCount >= 2 && entity.Confidence != "weak").Take(40).ToList();
    if (crossEntities.Count == 0)
    {
        report.AppendLine("- none: no id value appeared in two or more messages.");
    }
    foreach (var entity in crossEntities)
    {
        var occurrences = entity.Occurrences.OrderBy(occurrence => occurrence.Sequence).ToList();
        var chain = string.Join(" -> ", occurrences.Take(10).Select(occurrence =>
            $"seq{occurrence.Sequence}({occurrence.MessageName}.{occurrence.FieldName})"));
        if (occurrences.Count > 10)
        {
            chain += $" -> (+{occurrences.Count - 10} more)";
        }

        var span = entity.LastSequence - entity.FirstSequence;
        report.AppendLine($"- `{entity.Value}` [{string.Join("/", entity.Names.OrderBy(name => name, StringComparer.Ordinal))}] ({entity.Confidence}) in {entity.MessageCount} msgs, span {span} seq: {chain}");
    }

    report.AppendLine();
    report.AppendLine("## Graph Diff");
    report.AppendLine($"- status: `{diff.Status}`");
    if (!string.IsNullOrWhiteSpace(diff.BaselinePath))
    {
        report.AppendLine($"- baseline: `{diff.BaselinePath}`");
    }

    report.AppendLine($"- message shape changes: `{diff.MessageShapeChanges.Count}`");
    report.AppendLine($"- edge shape changes: `{diff.EdgeShapeChanges.Count}`");
    report.AppendLine($"- pattern shape changes: `{diff.PatternShapeChanges.Count}`");
    foreach (var change in diff.MessageShapeChanges.Concat(diff.EdgeShapeChanges).Concat(diff.PatternShapeChanges).Take(30))
    {
        report.AppendLine($"- `{change.Kind}` `{change.Key}` baseline={change.BaselineCount} current={change.CurrentCount}");
    }

    report.AppendLine();
    report.AppendLine("## Output Files");
    report.AppendLine("- `timeline.json`: all GC records in chronological order.");
    report.AppendLine("- `message-index.json`: all records grouped by GC message type.");
    report.AppendLine("- `jobs.json`: `source_job_id` to `target_job_id` correlation.");
    report.AppendLine("- `conversation-flows.json`: each client message with immediate server replies and job-linked replies.");
    report.AppendLine("- `transitions.json`: global consecutive-message transition counts.");
    report.AppendLine("- `after-windows.json`: next records after each anchor message, optionally filtered with `--after`.");
    report.AppendLine("- `protocol-graph.json`: agent-readable causal graph with message, SO and state-relation nodes.");
    report.AppendLine("- `pattern-summary.json`: compact protocol patterns grouped from graph edges.");
    report.AppendLine("- `entities.json`: every id/steam_id value and the ordered chain of messages (with field paths) that share it.");
    report.AppendLine("- `graph-signature.json`: stable shape signature for capture/proto-version comparisons.");
    report.AppendLine("- `graph-diff.json`: baseline comparison when `--baseline` is provided.");
    report.AppendLine("- `flow-report.html`: self-contained visual canvas for the message/SO timeline.");
    report.AppendLine("- Per-record JSON files: full decoded payloads and nested SO object data.");
    File.WriteAllText(Path.Combine(outputPath, "forensic-report.md"), report.ToString(), Encoding.UTF8);
}

static void WriteHtmlFlowReport(
    string outputPath,
    IReadOnlyList<DecodedGcRecord> ordered,
    ProtocolGraph graph,
    IReadOnlyList<FlowPattern> patterns,
    IReadOnlyList<EntityTrack> entities)
{
    // Build an agent-facing view model. The HTML renders it as a live sequence
    // diagram (Client / Game Coordinator / SO Cache lifelines) plus a written
    // "mental map" so a coding agent can reconstruct the communication flow.
    var messageNodes = graph.Nodes
        .Where(node => node.Kind is "inbound_msg" or "outbound_msg")
        .OrderBy(node => node.Sequence)
        .ToList();

    var vmMessages = messageNodes.Select(node => new
    {
        id = node.Id,
        seq = node.Sequence,
        dir = node.Direction,
        type = node.MessageType,
        name = node.MessageName,
        proto = node.ProtoType,
        len = node.PayloadLength,
        src = node.SourceJobId,
        tgt = node.TargetJobId,
        hints = node.EntityHints,
        kv = node.KeyValues.Select(reference => new { name = reference.Name, path = reference.Path, value = reference.Value }).ToList(),
        sum = node.Summary,
        fields = node.PayloadFields.Take(48).ToList(),
        err = node.DecodeError
    }).ToList();

    var vmSo = graph.Nodes
        .Where(node => node.Kind is "so_create" or "so_update")
        .Select(node => new
        {
            id = node.Id,
            seq = node.Sequence,
            action = node.Kind,
            typeId = node.TypeId,
            label = TypeIdLabel(node.TypeId),
            ownerType = node.OwnerType,
            ownerId = node.OwnerId,
            serviceId = node.ServiceId
        }).ToList();

    var vmEdges = graph.Edges
        .Where(edge => edge.Kind is "responds_to" or "causes_immediate" or "creates_so" or "updates_so" or "same_entity")
        .Select(edge => new
        {
            from = edge.From,
            to = edge.To,
            kind = edge.Kind,
            confidence = edge.Confidence,
            evidence = edge.Evidence
        }).ToList();

    var vmPatterns = patterns.Select(pattern => new
    {
        name = pattern.Name,
        seq = pattern.Trigger.Sequence,
        type = pattern.Trigger.MessageType,
        status = pattern.Status,
        responses = pattern.Responses.Select(response => new
        {
            seq = response.Sequence,
            type = response.MessageType,
            name = response.MessageName
        }).ToList(),
        so = pattern.SoMutations.Select(mutation => new
        {
            seq = mutation.Sequence,
            action = mutation.Action,
            typeId = mutation.TypeId,
            ownerType = mutation.OwnerType
        }).ToList()
    }).ToList();

    var vmEntities = entities.Where(entity => entity.MessageCount >= 2 && entity.Confidence != "weak").Take(200).Select(entity => new
    {
        value = entity.Value,
        names = entity.Names.OrderBy(name => name, StringComparer.Ordinal).ToList(),
        count = entity.MessageCount,
        confidence = entity.Confidence,
        first = entity.FirstSequence,
        last = entity.LastSequence,
        occ = entity.Occurrences.OrderBy(occurrence => occurrence.Sequence).Select(occurrence => new
        {
            seq = occurrence.Sequence,
            dir = occurrence.Direction,
            name = occurrence.MessageName,
            type = occurrence.MessageType,
            field = occurrence.FieldName
        }).ToList()
    }).ToList();

    var model = new
    {
        meta = new
        {
            records = ordered.Count,
            nodes = graph.Nodes.Count,
            edges = graph.Edges.Count,
            patterns = patterns.Count,
            entities = vmEntities.Count,
            generatedUtc = DateTime.UtcNow
        },
        messages = vmMessages,
        so = vmSo,
        edges = vmEdges,
        patterns = vmPatterns,
        entities = vmEntities
    };

    var modelJson = JsonSerializer.Serialize(model).Replace("</", "<\\/", StringComparison.Ordinal);

    const string template =
"""
<!doctype html>
<html lang="en">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>NetHook GC Communication Flow</title>
<style>
:root{--bg:#0d1117;--panel:#161b22;--border:#30363d;--muted:#8b949e;--text:#e6edf3;--client:#3b82f6;--server:#10b981;--so:#f59e0b;--job:#a855f7;--imm:#94a3b8;--entity:#ec4899;}
*{box-sizing:border-box}
body{margin:0;background:var(--bg);color:var(--text);font-family:'Segoe UI',system-ui,Arial,sans-serif;font-size:14px}
header{padding:14px 22px;background:linear-gradient(90deg,#111827,#0d1117);border-bottom:1px solid var(--border)}
header h1{margin:0;font-size:17px}
.stats{display:flex;gap:16px;flex-wrap:wrap;margin-top:8px;color:var(--muted);font-size:12px}
.stats b{color:var(--text)}
.layout{display:grid;grid-template-columns:1fr 360px;height:calc(100vh - 78px)}
@media(max-width:920px){.layout{grid-template-columns:1fr;height:auto}}
.canvasWrap{position:relative;overflow:auto;border-right:1px solid var(--border)}
canvas{display:block}
.side{overflow:auto;padding:16px;background:var(--panel)}
.side h2{font-size:12px;text-transform:uppercase;letter-spacing:.5px;color:var(--muted);margin:0 0 8px}
.card{background:var(--bg);border:1px solid var(--border);border-radius:8px;padding:12px;margin-bottom:14px}
.legend div{display:flex;align-items:center;gap:8px;margin:5px 0;font-size:12px}
.swatch{width:14px;height:14px;border-radius:3px;flex:0 0 auto}
.line{width:22px;border-top-width:3px;border-top-style:solid;flex:0 0 auto}
.controls label{display:flex;align-items:center;gap:6px;font-size:12px;margin:4px 0;cursor:pointer}
.controls input[type=search]{width:100%;padding:6px 8px;background:var(--bg);border:1px solid var(--border);border-radius:6px;color:var(--text);margin-top:8px}
.detail .row{display:flex;justify-content:space-between;gap:10px;padding:4px 0;border-bottom:1px solid var(--border);font-size:12px}
.detail .row span:first-child{color:var(--muted)}
.tag{display:inline-block;padding:1px 8px;border-radius:20px;font-size:11px;font-weight:600}
.tag.client{background:rgba(59,130,246,.18);color:#93c5fd}
.tag.server{background:rgba(16,185,129,.18);color:#6ee7b7}
.chip{display:inline-block;background:var(--bg);border:1px solid var(--border);border-radius:5px;padding:1px 6px;margin:2px 3px 0 0;font-size:11px;color:var(--muted)}
.interp{font-size:12.5px;line-height:1.55;color:#cbd5e1}
.interp p{margin:0 0 8px}
ul.mini{margin:6px 0 0;padding-left:16px;font-size:12px;color:#cbd5e1}
.tooltip{position:absolute;pointer-events:none;background:#000;border:1px solid var(--border);border-radius:6px;padding:6px 9px;font-size:11px;max-width:280px;display:none;z-index:5;color:var(--text);line-height:1.45}
.muted{color:var(--muted)}
code{background:var(--panel);border:1px solid var(--border);border-radius:4px;padding:1px 4px;font-size:11px}
</style>
</head>
<body>
<header>
<h1>NetHook GC Communication Flow</h1>
<div class="stats" id="stats"></div>
</header>
<div class="layout">
<div class="canvasWrap" id="canvasWrap">
<canvas id="cv"></canvas>
<div class="tooltip" id="tip"></div>
</div>
<div class="side">
<div class="card"><h2>How to read this flow</h2><div class="interp" id="mentalMapBody"></div></div>
<div class="card controls">
<h2>Filters</h2>
<label><input type="checkbox" id="f_responds_to" checked> Job-matched replies (responds_to)</label>
<label><input type="checkbox" id="f_causes_immediate" checked> Timing-inferred replies</label>
<label><input type="checkbox" id="f_so" checked> SO cache mutations</label>
<label><input type="checkbox" id="f_same_entity"> Same-entity links</label>
<input type="search" id="search" placeholder="Filter by message name / id...">
</div>
<div class="card legend">
<h2>Legend</h2>
<div><span class="swatch" style="background:var(--client)"></span> Client &#8594; GC request</div>
<div><span class="swatch" style="background:var(--server)"></span> GC &#8594; Client message</div>
<div><span class="swatch" style="background:var(--so)"></span> SO cache object (state)</div>
<div><span class="line" style="border-color:var(--job)"></span> responds_to &mdash; job id match (exact)</div>
<div><span class="line" style="border-color:var(--imm)"></span> causes_immediate &mdash; reply by timing</div>
<div><span class="line" style="border-color:var(--so)"></span> creates / updates SO</div>
<div><span class="line" style="border-color:var(--entity)"></span> same entity (lobby / party / match)</div>
</div>
<div class="card" id="entitiesCard">
<h2>Top correlated IDs</h2>
<div class="interp muted" id="entitiesBody">&mdash;</div>
</div>
<div class="card detail" id="detail">
<h2>Message detail</h2>
<div class="muted" id="detailBody">Click any arrow to inspect its decoded payload (what it contains), its id values, job correlation, SO cache effects, and every other message that carries the same id. This panel is the per-message spec for reimplementing the flow.</div>
</div>
</div>
</div>
<script>
const MODEL = /*__MODEL__*/;
(function(){
const C={client:'#3b82f6',server:'#10b981',so:'#f59e0b',job:'#a855f7',imm:'#94a3b8',entity:'#ec4899',bg:'#0d1117',grid:'#30363d',mut:'#8b949e',text:'#e6edf3'};
const msgs=MODEL.messages||[], soList=MODEL.so||[], edges=MODEL.edges||[], entities=MODEL.entities||[];
const byId=new Map(msgs.map(m=>[m.id,m]));
const soById=new Map(soList.map(s=>[s.id,s]));
const out=new Map(), inc=new Map();
function push(map,k,v){ if(!map.has(k)) map.set(k,[]); map.get(k).push(v); }
for(const e of edges){ push(out,e.from,e); push(inc,e.to,e); }
function esc(s){ return (''+s).replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;'); }
// Value-level correlation index: which messages carry each concrete id value.
const valueMap=new Map();
for(const m of msgs){ for(const k of (m.kv||[])){ if(!valueMap.has(k.value)) valueMap.set(k.value,[]); valueMap.get(k.value).push({seq:m.seq,field:k.name,dir:m.dir,id:m.id}); } }
// Drop coincidental enum collisions: a tiny int shared across DIFFERENT field names is noise.
function weakPair(k,o){ const n=Number(k.value); return Number.isFinite(n)&&Math.abs(n)<1000000&&k.name!==o.field; }
function linkedIdsFor(m){ const set=new Set(); for(const k of (m.kv||[])){ for(const o of (valueMap.get(k.value)||[])){ if(o.id!==m.id && !weakPair(k,o)) set.add(o.id); } } return set; }
let linkedIds=new Set();

const MAXROWS=800;
const rows=msgs.slice(0,MAXROWS);
const top=64, rowH=42;
const rowIndex=new Map(rows.map((m,i)=>[m.id,i]));
const yOf=i=>top+i*rowH;

const cc=msgs.filter(m=>m.dir==='client').length, sc=msgs.length-cc;
document.getElementById('stats').innerHTML=
 '<span>Records <b>'+(MODEL.meta.records)+'</b></span>'+
 '<span>Client&#8594;GC <b>'+cc+'</b></span>'+
 '<span>GC&#8594;Client <b>'+sc+'</b></span>'+
 '<span>SO mutations <b>'+soList.length+'</b></span>'+
 '<span>Edges <b>'+MODEL.meta.edges+'</b></span>'+
 '<span>Correlated IDs <b>'+entities.length+'</b></span>'+
 '<span>Patterns <b>'+MODEL.meta.patterns+'</b></span>'+
 '<span>Generated <b>'+new Date(MODEL.meta.generatedUtc).toISOString().slice(0,19)+'Z</b></span>';

const jobPairs=edges.filter(e=>e.kind==='responds_to').length;
const immPairs=edges.filter(e=>e.kind==='causes_immediate').length;
const soTypes=[...new Set(soList.map(s=>s.label).filter(Boolean))];
const hello=msgs.find(m=>/ClientHello/i.test(m.name||''));
const welcome=msgs.find(m=>/ClientWelcome/i.test(m.name||''));
let mm='';
mm+='<p>Three vertical lifelines: the <b style="color:'+C.client+'">Client</b> (game), the <b style="color:'+C.server+'">Game Coordinator</b> (Dota server logic, "GC"), and the <b style="color:'+C.so+'">SO Cache</b> (SharedObject state the GC replicates into the client).</p>';
mm+='<p>Read top&#8594;bottom in capture order. Arrows to the right are <b>client requests</b>; arrows to the left are <b>GC messages</b>. Orange branches to the SO Cache lane mark where a message <b>created or updated</b> game state (lobby, party, match&hellip;).</p>';
if(hello&&welcome) mm+='<p><b>Session bootstrap:</b> <code>'+esc(hello.name||('#'+hello.type))+'</code> (#'+hello.seq+') &#8594; <code>'+esc(welcome.name||('#'+welcome.type))+'</code> (#'+welcome.seq+') opens the GC session and seeds the initial SO cache subscription.</p>';
mm+='<p><b>Correlation for agents:</b> '+jobPairs+' reply link(s) match by <b style="color:'+C.job+'">job id</b> (purple arcs, exact &mdash; trust these to pair reply&#8596;request). '+immPairs+' more are inferred only from timing (grey).</p>';
if(soTypes.length) mm+='<p><b>State touched:</b> '+soTypes.length+' SharedObject type(s): '+soTypes.slice(0,8).map(t=>'<span class=chip>'+esc(t)+'</span>').join(' ')+'. Follow the orange branches to rebuild the state machine.</p>';
mm+='<p><b>Value correlation:</b> '+entities.length+' concrete id value(s) (steam_id / lobby_id / account_id&hellip;) reappear across &#8805;2 messages. Selecting a message reveals every other message carrying the same id and how many sequences apart &mdash; that is the data-flow map (which id a request carries, which later message echoes it).</p>';
if(msgs.length>MAXROWS) mm+='<p class=muted>Showing first '+MAXROWS+' of '+msgs.length+' messages for readability.</p>';
mm+='<p class=muted>Click any arrow &#8594; per-message spec (contents, id values, job ids, SO effects, correlated messages).</p>';
document.getElementById('mentalMapBody').innerHTML=mm;

const eb=document.getElementById('entitiesBody');
if(entities.length===0){ eb.innerHTML='No id value appears in two or more messages.'; }
else {
  eb.classList.remove('muted');
  eb.innerHTML=entities.slice(0,14).map((en,ix)=>{
    const nm=(en.names||[]).join('/');
    const seqs=en.occ.map(o=>o.seq).join(', ');
    return '<div class="entRow" data-i="'+ix+'" style="padding:5px 0;border-bottom:1px solid var(--border);cursor:pointer">'+
      '<code>'+esc(en.value.length>22?en.value.slice(0,22)+'…':en.value)+'</code> '+
      '<span class="muted">'+esc(nm)+'</span><br>'+
      '<span class="muted" style="font-size:11px">'+en.count+' msgs &middot; span '+(en.last-en.first)+' &middot; seq '+esc(seqs.length>60?seqs.slice(0,60)+'…':seqs)+'</span></div>';
  }).join('');
  eb.querySelectorAll('.entRow').forEach(el=>{ el.addEventListener('click',()=>{ const en=entities[+el.dataset.i]; const first=en.occ[0]; const m=rows.find(r=>r.seq===first.seq); if(m){ selected=m; linkedIds=linkedIdsFor(m); renderDetail(m); scrollToRow(m); draw(); } }); });
}
function scrollToRow(m){ const i=rowIndex.get(m.id); if(i==null) return; wrap.scrollTop=Math.max(0,yOf(i)-wrap.clientHeight/2); }

const cv=document.getElementById('cv'), ctx=cv.getContext('2d');
const wrap=document.getElementById('canvasWrap'), tip=document.getElementById('tip');
let W=0,H=0,dpr=window.devicePixelRatio||1;
const L={client:180,server:0,so:0};
const filters={responds_to:true,causes_immediate:true,so:true,same_entity:false};
let term='', selected=null, hoverIdx=-1;

function layout(){
  W=Math.max(wrap.clientWidth-2,760);
  L.client=180; L.server=Math.round(W*0.52); L.so=Math.round(W*0.85);
  H=top+rows.length*rowH+40;
  cv.style.width=W+'px'; cv.style.height=H+'px';
  cv.width=Math.round(W*dpr); cv.height=Math.round(H*dpr);
  ctx.setTransform(dpr,0,0,dpr,0,0);
}
function matches(m){ if(!term) return true; return ((m.name||'')+' '+m.type+' '+(m.proto||'')).toLowerCase().includes(term); }
function head(x,y,dir,color){ ctx.fillStyle=color; ctx.beginPath(); ctx.moveTo(x,y); ctx.lineTo(x-dir*8,y-4); ctx.lineTo(x-dir*8,y+4); ctx.closePath(); ctx.fill(); }

function draw(){
  ctx.clearRect(0,0,W,H);
  const lanes=[[L.client,'CLIENT',C.client],[L.server,'GAME COORDINATOR',C.server],[L.so,'SO CACHE (state)',C.so]];
  ctx.textAlign='center'; ctx.font='600 12px Segoe UI,Arial';
  for(const l of lanes){ ctx.strokeStyle=C.grid; ctx.lineWidth=1; ctx.beginPath(); ctx.moveTo(l[0],top-22); ctx.lineTo(l[0],H-20); ctx.stroke(); ctx.fillStyle=l[2]; ctx.fillText(l[1],l[0],top-32); }

  for(const e of edges){
    if(e.kind!=='responds_to'&&e.kind!=='causes_immediate') continue;
    if(e.kind==='responds_to'&&!filters.responds_to) continue;
    if(e.kind==='causes_immediate'&&!filters.causes_immediate) continue;
    if(!rowIndex.has(e.from)||!rowIndex.has(e.to)) continue;
    const y1=yOf(rowIndex.get(e.from)), y2=yOf(rowIndex.get(e.to));
    const hl=selected&&(e.from===selected.id||e.to===selected.id);
    const depth=Math.min(95,24+Math.abs(y2-y1)*0.18);
    ctx.strokeStyle=e.kind==='responds_to'?C.job:C.imm; ctx.globalAlpha=hl?0.95:0.32; ctx.lineWidth=hl?2:1;
    ctx.beginPath(); ctx.moveTo(L.client-6,y1); ctx.quadraticCurveTo(L.client-6-depth,(y1+y2)/2,L.client-6,y2); ctx.stroke(); ctx.globalAlpha=1;
  }

  if(filters.same_entity){
    ctx.setLineDash([3,3]);
    for(const e of edges){ if(e.kind!=='same_entity') continue; if(!rowIndex.has(e.from)||!rowIndex.has(e.to)) continue;
      const y1=yOf(rowIndex.get(e.from)), y2=yOf(rowIndex.get(e.to));
      ctx.strokeStyle=C.entity; ctx.globalAlpha=0.3; ctx.lineWidth=1;
      ctx.beginPath(); ctx.moveTo(22,y1); ctx.quadraticCurveTo(6,(y1+y2)/2,22,y2); ctx.stroke(); ctx.globalAlpha=1;
    }
    ctx.setLineDash([]);
  }

  if(filters.so){
    const perRow=new Map();
    for(const e of edges){ if(e.kind!=='creates_so'&&e.kind!=='updates_so') continue; if(!rowIndex.has(e.from)) continue;
      const so=soById.get(e.to); if(!so) continue;
      const i=rowIndex.get(e.from); const n=perRow.get(i)||0; perRow.set(i,n+1);
      const y=yOf(i)+n*14;
      ctx.strokeStyle=C.so; ctx.globalAlpha=0.6; ctx.lineWidth=1.3;
      ctx.beginPath(); ctx.moveTo(L.server,yOf(i)); ctx.lineTo(L.so,y); ctx.stroke(); ctx.globalAlpha=1;
      head(L.so,y,1,C.so);
      ctx.fillStyle=C.so; ctx.textAlign='left'; ctx.font='11px Segoe UI,Arial';
      ctx.fillText((e.kind==='creates_so'?'+ ':'~ ')+(so.label||('type '+so.typeId)),L.so+8,y+3);
    }
  }

  ctx.font='12px Segoe UI,Arial';
  for(let i=0;i<rows.length;i++){
    const m=rows[i], y=yOf(i);
    const dim=!matches(m), isSel=selected&&selected.id===m.id, isHov=hoverIdx===i;
    const col=m.dir==='client'?C.client:C.server;
    const x1=m.dir==='client'?L.client:L.server;
    const x2=m.dir==='client'?L.server:L.client;
    const dir=m.dir==='client'?1:-1;
    const linked=linkedIds.has(m.id);
    ctx.globalAlpha=dim?0.16:1;
    if(linked){ ctx.fillStyle='rgba(236,72,153,.12)'; ctx.fillRect(0,y-rowH/2,W,rowH); }
    if(isSel||isHov){ ctx.fillStyle=m.dir==='client'?'rgba(59,130,246,.10)':'rgba(16,185,129,.10)'; ctx.fillRect(0,y-rowH/2,W,rowH); }
    if(linked){ ctx.fillStyle=C.entity; ctx.beginPath(); ctx.arc(12,y,4,0,7); ctx.fill(); }
    ctx.strokeStyle=m.err?'#ef4444':col; ctx.lineWidth=isSel?2.4:1.6;
    ctx.beginPath(); ctx.moveTo(x1,y); ctx.lineTo(x2-dir*10,y); ctx.stroke();
    head(x2,y,dir,m.err?'#ef4444':col);
    ctx.fillStyle=col; ctx.beginPath(); ctx.arc(x1,y,3,0,7); ctx.fill();
    ctx.fillStyle=dim?C.mut:C.text; ctx.textAlign=m.dir==='client'?'left':'right';
    ctx.fillText(('#'+m.seq+'  '+m.type+'  '+(m.name||('msg '+m.type))).slice(0,58), m.dir==='client'?x1+8:x1-8, y-6);
    ctx.globalAlpha=1;
  }
}

function idxAt(py){ const i=Math.round((py-top)/rowH); if(i<0||i>=rows.length) return -1; return Math.abs(py-yOf(i))<=rowH/2?i:-1; }

wrap.addEventListener('mousemove',ev=>{
  const r=cv.getBoundingClientRect(), py=ev.clientY-r.top, px=ev.clientX-r.left;
  const i=idxAt(py); hoverIdx=i;
  if(i>=0){ const m=rows[i]; tip.style.display='block'; tip.style.left=Math.min(px+14,W-280)+'px'; tip.style.top=(py+14)+'px';
    tip.innerHTML='<b>#'+m.seq+'</b> '+(m.dir==='client'?'Client&#8594;GC':'GC&#8594;Client')+'<br>'+esc(m.name||('msg '+m.type))+' <span class=muted>('+m.type+')</span><br><span class=muted>'+esc(m.proto||'no proto')+' &middot; '+m.len+'B</span>'; }
  else tip.style.display='none';
  draw();
});
wrap.addEventListener('mouseleave',()=>{ hoverIdx=-1; tip.style.display='none'; draw(); });
cv.addEventListener('click',ev=>{ const r=cv.getBoundingClientRect(); const i=idxAt(ev.clientY-r.top); if(i>=0){ selected=rows[i]; linkedIds=linkedIdsFor(selected); renderDetail(selected); draw(); } });

function renderDetail(m){
  const outE=out.get(m.id)||[], incE=inc.get(m.id)||[];
  const replies=outE.filter(e=>e.kind==='responds_to'||e.kind==='causes_immediate').map(e=>byId.get(e.to)).filter(Boolean);
  const answers=incE.filter(e=>e.kind==='responds_to'||e.kind==='causes_immediate').map(e=>byId.get(e.from)).filter(Boolean);
  const soMut=outE.filter(e=>e.kind==='creates_so'||e.kind==='updates_so').map(e=>({e,so:soById.get(e.to)})).filter(x=>x.so);
  const ent=[...new Set([...outE,...incE].filter(e=>e.kind==='same_entity').map(e=>e.from===m.id?e.to:e.from))].map(id=>byId.get(id)).filter(Boolean);
  const row=(k,v)=>'<div class="row"><span>'+k+'</span><span>'+v+'</span></div>';
  let h='<div style="margin-bottom:8px"><span class="tag '+m.dir+'">'+(m.dir==='client'?'CLIENT &#8594; GC':'GC &#8594; CLIENT')+'</span></div>';
  h+=row('Sequence','#'+m.seq);
  h+=row('Message',esc(m.name||'&mdash;')+' <code>'+m.type+'</code>');
  if(m.proto) h+=row('Proto','<code>'+esc(m.proto)+'</code>');
  h+=row('Payload',m.len+' B');
  if(m.src) h+=row('source_job_id','<code>'+esc(m.src)+'</code>');
  if(m.tgt) h+=row('target_job_id','<code>'+esc(m.tgt)+'</code>');
  if(m.err) h+=row('Decode error','<span style="color:#ef4444">'+esc(m.err)+'</span>');
  let it='';
  if(m.dir==='client'){ it+='The client issues <b>'+esc(m.name||('message '+m.type))+'</b>. '+(replies.length?'The GC answers with '+replies.map(r=>'<b>'+esc(r.name||('msg '+r.type))+'</b> (#'+r.seq+')').join(', ')+'.':'No reply was captured for this request.'); }
  else { it+='The GC sends <b>'+esc(m.name||('message '+m.type))+'</b> to the client. '+(answers.length?'It answers client request '+answers.map(a=>'<b>'+esc(a.name||('msg '+a.type))+'</b> (#'+a.seq+')').join(', ')+'. ':''); }
  if(soMut.length) it+=' It '+(soMut.some(x=>x.e.kind==='creates_so')?'creates/':'')+'updates '+soMut.length+' SharedObject(s): '+soMut.map(x=>'<b>'+esc(x.so.label||('type '+x.so.typeId))+'</b>').join(', ')+'.';
  h+='<div class="interp" style="margin:10px 0">'+it+'</div>';
  if(soMut.length) h+='<h2>SO cache effects</h2><ul class="mini">'+soMut.map(x=>'<li>'+(x.e.kind==='creates_so'?'create':'update')+' &middot; '+esc(x.so.label||('type '+x.so.typeId))+' &middot; owner '+(x.so.ownerType==null?'?':x.so.ownerType)+(x.so.ownerId?(' ('+esc(x.so.ownerId)+')'):'')+'</li>').join('')+'</ul>';
  if(m.kv&&m.kv.length) h+='<h2>Key IDs</h2><div>'+m.kv.map(k=>'<span class=chip title="'+esc(k.path)+'">'+esc(k.name)+'=<b>'+esc(k.value.length>18?k.value.slice(0,18)+'…':k.value)+'</b></span>').join('')+'</div>';
  // Value correlation: every other message that carries one of this message's ids.
  const corr=[];
  for(const k of (m.kv||[])){ for(const o of (valueMap.get(k.value)||[])){ if(o.id===m.id||weakPair(k,o)) continue; corr.push({o,k}); } }
  if(corr.length){ corr.sort((a,b)=>a.o.seq-b.o.seq);
    h+='<h2>Same id in other messages</h2><ul class="mini">'+corr.slice(0,24).map(c=>{ const d=c.o.seq-m.seq; const rel=d===0?'same':(d>0?('+'+d+' later'):(d+' earlier')); return '<li>#'+c.o.seq+' <b>'+esc(byId.get(c.o.id)?(byId.get(c.o.id).name||('msg '+byId.get(c.o.id).type)):'?')+'</b> <span class=muted>('+rel+' seq)</span><br><span class=muted>'+esc(c.k.name)+'='+esc(c.k.value.length>18?c.k.value.slice(0,18)+'…':c.k.value)+' &rarr; '+esc(c.o.field)+'</span></li>'; }).join('')+'</ul>'; }
  if(m.sum&&m.sum.length) h+='<h2>Contains (payload digest)</h2><div>'+m.sum.slice(0,16).map(f=>'<span class=chip>'+esc(f)+'</span>').join('')+'</div>';
  if(m.hints&&m.hints.length) h+='<h2>Entity hints</h2><div>'+m.hints.slice(0,20).map(x=>'<span class=chip>'+esc(x)+'</span>').join('')+'</div>';
  if(ent.length) h+='<h2>Same entity</h2><div class=interp>'+ent.slice(0,12).map(r=>'#'+r.seq+' '+esc(r.name||('msg '+r.type))).join('<br>')+'</div>';
  if(m.fields&&m.fields.length) h+='<h2>Payload fields</h2><div>'+m.fields.slice(0,40).map(f=>'<span class=chip>'+esc(f)+'</span>').join('')+'</div>';
  document.getElementById('detailBody').innerHTML=h;
}

for(const k of ['responds_to','causes_immediate','so','same_entity']){ const el=document.getElementById('f_'+k); if(el){ el.checked=filters[k]; el.addEventListener('change',()=>{ filters[k]=el.checked; draw(); }); } }
const search=document.getElementById('search'); search.addEventListener('input',()=>{ term=search.value.trim().toLowerCase(); draw(); });
window.addEventListener('resize',()=>{ layout(); draw(); });
layout(); draw();
})();
</script>
</body>
</html>
""";

    var html = template.Replace("/*__MODEL__*/", modelJson);
    File.WriteAllText(Path.Combine(outputPath, "flow-report.html"), html, Encoding.UTF8);
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

public sealed class ProtocolGraph
{
    public DateTime GeneratedAtUtc { get; set; }
    public int Records { get; set; }
    public List<ProtocolNode> Nodes { get; set; } = new();
    public List<ProtocolEdge> Edges { get; set; } = new();
}

public sealed class ProtocolNode
{
    public string Id { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public int Sequence { get; set; }
    public string Direction { get; set; } = string.Empty;
    public uint MessageType { get; set; }
    public string MessageName { get; set; } = string.Empty;
    public string? ProtoType { get; set; }
    public string? SteamId { get; set; }
    public string? SourceJobId { get; set; }
    public string? TargetJobId { get; set; }
    public int PayloadLength { get; set; }
    public string PayloadShapeHash { get; set; } = string.Empty;
    public List<string> PayloadFields { get; set; } = new();
    public List<string> EntityHints { get; set; } = new();
    public int? ServiceId { get; set; }
    public int? OwnerType { get; set; }
    public string? OwnerId { get; set; }
    public int? TypeId { get; set; }
    public List<EntityRef> KeyValues { get; set; } = new();
    public List<string> Summary { get; set; } = new();
    public string? DecodeError { get; set; }
}

public sealed class EntityRef
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public sealed class EntityOccurrence
{
    public int Sequence { get; set; }
    public string Direction { get; set; } = string.Empty;
    public uint MessageType { get; set; }
    public string MessageName { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
}

public sealed class EntityTrack
{
    public string Value { get; set; } = string.Empty;
    public HashSet<string> Names { get; set; } = new(StringComparer.Ordinal);
    public int MessageCount { get; set; }
    public int FirstSequence { get; set; }
    public int LastSequence { get; set; }
    public string Confidence { get; set; } = string.Empty;
    public List<EntityOccurrence> Occurrences { get; set; } = new();
}

public sealed class ProtocolEdge
{
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public string Confidence { get; set; } = string.Empty;
    public List<string> Evidence { get; set; } = new();
}

public sealed class SoObjectSummary
{
    public int Ordinal { get; set; }
    public string Action { get; set; } = "so_update";
    public int? ServiceId { get; set; }
    public int? OwnerType { get; set; }
    public string? OwnerId { get; set; }
    public int? TypeId { get; set; }
    public string? DecodedType { get; set; }
    public string ObjectShapeHash { get; set; } = string.Empty;
    public List<string> ObjectFields { get; set; } = new();

    public SoObjectSummary Clone()
    {
        return new SoObjectSummary
        {
            Ordinal = Ordinal,
            Action = Action,
            ServiceId = ServiceId,
            OwnerType = OwnerType,
            OwnerId = OwnerId,
            TypeId = TypeId,
            DecodedType = DecodedType,
            ObjectShapeHash = ObjectShapeHash,
            ObjectFields = ObjectFields.ToList()
        };
    }
}

public sealed class FlowPattern
{
    public string Name { get; set; } = string.Empty;
    public PatternMessage Trigger { get; set; } = new();
    public List<PatternMessage> Responses { get; set; } = new();
    public List<PatternSo> SoMutations { get; set; } = new();
    public string Status { get; set; } = string.Empty;
}

public sealed class PatternMessage
{
    public int Sequence { get; set; }
    public string Direction { get; set; } = string.Empty;
    public uint MessageType { get; set; }
    public string MessageName { get; set; } = string.Empty;
}

public sealed class PatternSo
{
    public int Sequence { get; set; }
    public string Action { get; set; } = string.Empty;
    public int? TypeId { get; set; }
    public int? OwnerType { get; set; }
    public string? OwnerId { get; set; }
    public int? ServiceId { get; set; }
    public string ShapeHash { get; set; } = string.Empty;
}

public sealed class GraphSignature
{
    public DateTime GeneratedAtUtc { get; set; }
    public List<SignatureEntry> MessageShapes { get; set; } = new();
    public List<SignatureEntry> EdgeShapes { get; set; } = new();
    public List<SignatureEntry> PatternShapes { get; set; } = new();
}

public sealed class SignatureEntry
{
    public string Key { get; set; } = string.Empty;
    public int Count { get; set; }
    public int FirstSequence { get; set; }
    public int LastSequence { get; set; }
}

public sealed class GraphDiff
{
    public DateTime GeneratedAtUtc { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? BaselinePath { get; set; }
    public DateTime? BaselineGeneratedAtUtc { get; set; }
    public DateTime CurrentGeneratedAtUtc { get; set; }
    public List<string> Notes { get; set; } = new();
    public List<GraphDiffEntry> MessageShapeChanges { get; set; } = new();
    public List<GraphDiffEntry> EdgeShapeChanges { get; set; } = new();
    public List<GraphDiffEntry> PatternShapeChanges { get; set; } = new();
}

public sealed class GraphDiffEntry
{
    public string Kind { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int BaselineCount { get; set; }
    public int CurrentCount { get; set; }
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
        MapServer(7013, "GCOtherJoinedChannel", nameof(CMsgDOTAOtherJoinedChatChannel));
        MapServer(7014, "GCOtherLeftChannel", nameof(CMsgDOTAOtherLeftChatChannel));
        MapClient(7273, "GCChatMessage", nameof(CMsgDOTAChatMessage));
        MapServer(7273, "GCChatMessage", nameof(CMsgDOTAChatMessage));
        MapClient(4512, "GCInviteToLobby", nameof(CMsgInviteToLobby));
        MapClient(7682, "ClientToGCMMInfo", nameof(CMsgClientToGCMMInfo));
        MapServer(7681, "GCToClientRequestMMInfo", nameof(CMsgGCToClientRequestMMInfo));
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
