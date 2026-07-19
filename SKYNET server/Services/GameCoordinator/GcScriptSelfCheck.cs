using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using SKYNET_server.Models;

namespace SKYNET_server.Services;

public static class GcScriptSelfCheck
{
    private const uint DotaAppId = 570;
    private const ulong TestSteamId = 76561197960287930UL;

    public static bool Run(Action<string> write)
    {
        var contentRoot = ResolveContentRoot(Directory.GetCurrentDirectory());
        write($"GC self-check content root: {contentRoot}");

        var trace = new GameCoordinatorTraceService();
        var plugin = new GameCoordinatorScriptPlugin(
            new SelfCheckEnvironment(contentRoot),
            NullLogger<GameCoordinatorScriptPlugin>.Instance,
            trace);

        var context = new GameCoordinatorContext
        {
            AppId = DotaAppId,
            SteamId = TestSteamId,
            AccountId = 15892202,
            PersonaName = "GcScriptSelfCheck",
            ClientIp = "127.0.0.1"
        };

        var ok = true;
        ok &= ExpectSequence(plugin, context, 4006, new uint[] { 4009, 4004, 4009 }, write);
        ok &= ExpectResponse(plugin, context, 2536, 2537, 1, write);
        ok &= ExpectResponse(plugin, context, 2581, 2582, 1, write);
        ok &= ExpectResponse(plugin, context, 2569, 2570, 1, write);
        ok &= ExpectResponse(plugin, context, 2577, 2578, 1, write);
        ok &= ExpectHandled(plugin, context, 2617, 0, write);
        ok &= ExpectHandled(plugin, context, 4523, 0, write);
        ok &= ExpectResponse(plugin, context, 7009, 7010, 1, write);
        ok &= ExpectChatFlow(plugin, context, write);
        ok &= ExpectResponse(plugin, context, 7026, 7546, 1, write);
        ok &= ExpectResponse(plugin, context, 7072, 7087, 1, write);
        ok &= ExpectResponse(plugin, context, 7078, 7079, 1, write);
        ok &= ExpectResponse(plugin, context, 7082, 7083, 1, write);
        ok &= ExpectResponse(plugin, context, 7200, 7201, 1, write);
        ok &= ExpectResponse(plugin, context, 7274, 7275, 1, write);
        ok &= ExpectResponse(plugin, context, 7381, 7382, 1, write);
        ok &= ExpectResponse(plugin, context, 7387, 7388, 1, write);
        ok &= ExpectResponse(plugin, context, 7408, 7409, 1, write);
        ok &= ExpectResponse(plugin, context, 7427, 7428, 1, write);
        ok &= ExpectHandled(plugin, context, 7497, 0, write);
        ok &= ExpectResponse(plugin, context, 7531, 7382, 1, write);
        ok &= ExpectResponse(plugin, context, 7521, 7522, 1, write);
        ok &= ExpectResponse(plugin, context, 7527, 7528, 1, write);
        ok &= ExpectResponse(plugin, context, 7534, 7535, 1, write);
        ok &= ExpectResponse(plugin, context, 7538, 7539, 1, write);
        ok &= ExpectResponse(plugin, context, 7584, 7586, 1, write);
        ok &= ExpectResponse(plugin, context, 7606, 7607, 1, write);
        ok &= ExpectResponse(plugin, context, 8082, 8083, 1, write);
        ok &= ExpectResponse(plugin, context, 8111, 8112, 1, write);
        ok &= ExpectResponse(plugin, context, 8113, 8114, 1, write);
        ok &= ExpectResponse(plugin, context, 8006, 8007, 1, write);
        ok &= ExpectResponse(plugin, context, 8009, 8010, 1, write);
        ok &= ExpectResponse(plugin, context, 8016, 8017, 1, write);
        ok &= ExpectHandled(plugin, context, 8041, 0, write);
        ok &= ExpectResponse(plugin, context, 8034, 8035, 1, write);
        ok &= ExpectResponse(plugin, context, 8073, 8074, 1, write);
        ok &= ExpectResponse(plugin, context, 8124, 8125, 1, write);
        ok &= ExpectHandled(plugin, context, 8255, 0, write);
        ok &= ExpectResponse(plugin, context, 8268, 8269, 1, write);
        ok &= ExpectResponse(plugin, context, 8270, 8271, 1, write);
        ok &= ExpectResponse(plugin, context, 8274, 8275, 1, write);
        ok &= ExpectResponse(plugin, context, 8303, 8304, 1, write);
        ok &= ExpectResponse(plugin, context, 8305, 8306, 1, write);
        ok &= ExpectResponse(plugin, context, 8332, 8333, 1, write);
        ok &= ExpectResponse(plugin, context, 8334, 8335, 1, write);
        ok &= ExpectResponse(plugin, context, 8349, 8350, 1, write);
        ok &= ExpectResponse(plugin, context, 8793, 8794, 1, write);
        ok &= ExpectResponse(plugin, context, 8798, 8799, 1, write);
        ok &= ExpectResponse(plugin, context, 8800, 8801, 1, write);
        ok &= ExpectResponse(plugin, context, 8879, 8880, 1, write);
        ok &= ExpectResponse(plugin, context, 8886, 8887, 1, write);
        ok &= ExpectUnhandled(plugin, context, 999999, write);

        foreach (var entry in trace.GetSince(0))
        {
            write($"trace {entry.Kind} app={entry.AppId} msg={entry.MessageType} size={entry.Size} {entry.Detail}");
        }

        write(ok ? "PASS" : "FAIL");
        return ok;
    }

    private static bool ExpectSequence(
        GameCoordinatorScriptPlugin plugin,
        GameCoordinatorContext context,
        uint requestType,
        uint[] expectedResponseTypes,
        Action<string> write)
    {
        var response = plugin.Exchange(context, Request(requestType));
        var actual = response.Messages.Select(message => message.MessageType).ToArray();
        var ok = response.Handled
            && actual.Length == expectedResponseTypes.Length
            && actual.SequenceEqual(expectedResponseTypes)
            && response.Messages.All(message => message.Protobuf);
        write($"{requestType} -> handled={response.Handled}, messages=[{string.Join(",", actual)}], ok={ok}");
        return ok;
    }

    private static bool ExpectResponse(
        GameCoordinatorScriptPlugin plugin,
        GameCoordinatorContext context,
        uint requestType,
        uint expectedResponseType,
        int expectedCount,
        Action<string> write)
    {
        var response = plugin.Exchange(context, Request(requestType));
        var matching = response.Messages.Count(message => message.MessageType == expectedResponseType && message.Protobuf);
        var ok = response.Handled && response.Messages.Count == expectedCount && matching == expectedCount;
        write($"{requestType} -> handled={response.Handled}, messages={response.Messages.Count}, expected={expectedResponseType}, ok={ok}");
        return ok;
    }

    private static bool ExpectHandled(
        GameCoordinatorScriptPlugin plugin,
        GameCoordinatorContext context,
        uint requestType,
        int expectedCount,
        Action<string> write)
    {
        var response = plugin.Exchange(context, Request(requestType));
        var ok = response.Handled && response.Messages.Count == expectedCount;
        write($"{requestType} -> handled={response.Handled}, messages={response.Messages.Count}, ok={ok}");
        return ok;
    }

    private static bool ExpectChatFlow(
        GameCoordinatorScriptPlugin plugin,
        GameCoordinatorContext context,
        Action<string> write)
    {
        var joinResponse = plugin.Exchange(context, Request(7009));
        var joinOk = joinResponse.Handled
            && joinResponse.Messages.Count == 1
            && joinResponse.Messages[0].MessageType == 7010;
        if (!joinOk)
        {
            write($"chat flow -> join handled={joinResponse.Handled}, messages={joinResponse.Messages.Count}, ok=False");
            return false;
        }

        var join = Deserialize<CMsgDOTAJoinChatChannelResponse>(joinResponse.Messages[0].PayloadBase64);
        var channelId = join.ChannelId;
        joinOk = channelId != 0 && join.Members.Count == 1 && join.Members[0].SteamId == context.SteamId;

        var chatBody = Serialize(new CMsgDOTAChatMessage { ChannelId = channelId, Text = "hello" });
        var chatResponse = plugin.Exchange(context, Request(7273, chatBody));
        var echo = chatResponse.Messages.Count == 1 && chatResponse.Messages[0].MessageType == 7273
            ? Deserialize<CMsgDOTAChatMessage>(chatResponse.Messages[0].PayloadBase64)
            : null;
        var chatOk = chatResponse.Handled
            && echo != null
            && echo.ChannelId == channelId
            && echo.Text == "hello"
            && echo.PersonaName == context.PersonaName
            && echo.AccountId == context.AccountId;

        var leaveResponse = plugin.Exchange(context, Request(7272, Serialize(new CMsgDOTALeaveChatChannel { ChannelId = channelId })));
        var leaveOk = leaveResponse.Handled && leaveResponse.Messages.Count == 0;

        var afterLeaveResponse = plugin.Exchange(context, Request(7273, chatBody));
        var afterLeaveOk = afterLeaveResponse.Handled && afterLeaveResponse.Messages.Count == 0;

        var ok = joinOk && chatOk && leaveOk && afterLeaveOk;
        write($"chat flow -> join={joinOk}, echo={chatOk}, leave={leaveOk}, afterLeave={afterLeaveOk}, ok={ok}");
        return ok;
    }

    private static bool ExpectUnhandled(
        GameCoordinatorScriptPlugin plugin,
        GameCoordinatorContext context,
        uint requestType,
        Action<string> write)
    {
        var response = plugin.Exchange(context, Request(requestType));
        var ok = !response.Handled && response.Messages.Count == 0;
        write($"{requestType} -> handled={response.Handled}, messages={response.Messages.Count}, ok={ok}");
        return ok;
    }

    private static ApiGCExchangeRequest Request(uint messageType)
    {
        return new ApiGCExchangeRequest
        {
            AppId = DotaAppId,
            MessageType = messageType,
            BodyBase64 = string.Empty,
            SteamId = TestSteamId,
            GameServer = false
        };
    }

    private static ApiGCExchangeRequest Request(uint messageType, byte[] body)
    {
        var request = Request(messageType);
        request.BodyBase64 = Convert.ToBase64String(body);
        return request;
    }

    private static byte[] Serialize<TMessage>(TMessage message)
    {
        using var stream = new MemoryStream();
        ProtoBuf.Serializer.Serialize(stream, message);
        return stream.ToArray();
    }

    private static TMessage Deserialize<TMessage>(string? payloadBase64)
    {
        using var stream = new MemoryStream(Convert.FromBase64String(payloadBase64 ?? string.Empty));
        return ProtoBuf.Serializer.Deserialize<TMessage>(stream);
    }

    private static string ResolveContentRoot(string start)
    {
        var current = new DirectoryInfo(start);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "GC", "570", "main.ts")))
            {
                return current.FullName;
            }

            var nested = Path.Combine(current.FullName, "SKYNET server");
            if (File.Exists(Path.Combine(nested, "GC", "570", "main.ts")))
            {
                return nested;
            }

            current = current.Parent;
        }

        return start;
    }

    private sealed class SelfCheckEnvironment : IHostEnvironment
    {
        public SelfCheckEnvironment(string contentRootPath)
        {
            ContentRootPath = contentRootPath;
            ContentRootFileProvider = new PhysicalFileProvider(contentRootPath);
        }

        public string EnvironmentName { get; set; } = Environments.Development;
        public string ApplicationName { get; set; } = "SKYNET server";
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
    }
}
