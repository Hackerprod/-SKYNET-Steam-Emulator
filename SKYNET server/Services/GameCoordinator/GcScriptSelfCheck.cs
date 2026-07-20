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
        var serverContext = new GameCoordinatorContext
        {
            AppId = DotaAppId,
            SteamId = 85568397966950859UL,
            AccountId = 0,
            PersonaName = "GcScriptSelfCheck Dedicated",
            ClientIp = "192.168.212.252"
        };
        var queuedMessages = new List<(ulong SteamId, ApiGCMessage Message)>();
        var selfCheckDb = Path.Combine(Path.GetTempPath(), "skynet-gc-selfcheck", Guid.NewGuid().ToString("N"), "app.db");
        DotaStatsAccountIdentity? ResolveIdentity(uint accountId)
        {
            return accountId == context.AccountId
                ? new DotaStatsAccountIdentity(context.AccountId, context.SteamId, context.PersonaName)
                : null;
        }

        DotaGcRuntimeServices.StatsStore = new DotaStatsStore(selfCheckDb, ResolveIdentity);
        DotaGcRuntimeServices.GuildStore = new DotaGuildStore(selfCheckDb, ResolveIdentity);
        DotaGcRuntimeServices.PendingMessageQueued = (steamId, message) =>
            queuedMessages.Add((steamId, message));
        DotaGcRuntimeServices.TeamJsonProvider = teamId => teamId == 7733573
            ? """
              {
                "teamId":"7733573",
                "name":"SKYNET",
                "tag":"",
                "teamJson":"{\"teamLogo\":\"3255294647392078090\",\"teamBaseLogo\":\"7163376947542189088\",\"teamBannerLogo\":\"7954877705993612385\",\"teamLogoUrl\":\"\",\"teamAbbreviation\":\"\"}"
              }
              """
            : "{}";
        DotaGcRuntimeServices.TeamsForAccountJsonProvider = accountId => accountId == context.AccountId
            ? """
              [
                {
                  "teamId":"7733573",
                  "name":"SKYNET",
                  "tag":"",
                  "teamJson":"{\"teamLogo\":\"3255294647392078090\",\"teamBaseLogo\":\"7163376947542189088\",\"teamBannerLogo\":\"7954877705993612385\",\"teamLogoUrl\":\"\",\"teamAbbreviation\":\"\"}",
                  "role":1
                }
              ]
              """
            : "[]";
        SeedSocialMatchData(DotaGcRuntimeServices.StatsStore, context);

        var ok = true;
        ok &= ExpectSequence(plugin, context, 4006, new uint[] { 4009, 4004, 4009 }, write);
        ok &= ExpectResponse(plugin, context, 2536, 2537, 1, write);
        ok &= ExpectResponse(plugin, context, 2581, 2582, 1, write);
        ok &= ExpectResponse(plugin, context, 2569, 2570, 1, write);
        ok &= ExpectResponse(plugin, context, 2577, 2578, 1, write);
        ok &= ExpectHandled(plugin, context, 2617, 0, write);
        ok &= ExpectResponse(plugin, context, 4501, 4502, 1, write);
        ok &= ExpectHandled(plugin, context, 4523, 0, write);
        ok &= ExpectResponse(plugin, context, 7009, 7010, 1, write);
        ok &= ExpectChatFlow(plugin, context, write);
        ok &= ExpectGameServerWelcomeFlow(plugin, serverContext, write);
        ok &= ExpectCreateLobbyFlow(plugin, context, write);
        ok &= ExpectApplyTeamFlow(plugin, context, write);
        ok &= ExpectLaunchFlow(plugin, context, write);
        ok &= ExpectDedicatedAttachFlow(plugin, serverContext, queuedMessages, write);
        ok &= ExpectConnectedPlayersFlow(plugin, context, serverContext, queuedMessages, write);
        ok &= ExpectResponse(plugin, context, 7026, 7546, 1, write);
        ok &= ExpectResponse(plugin, context, 7072, 7087, 1, write);
        ok &= ExpectResponse(plugin, context, 7078, 7079, 1, write);
        ok &= ExpectResponse(plugin, context, 7082, 7083, 1, write);
        ok &= ExpectResponse(plugin, context, 7095, 7096, 1, write);
        ok &= ExpectSocialMatchDetails(plugin, context, 90000000000042UL, write);
        ok &= ExpectResponse(plugin, context, 7200, 7201, 1, write);
        ok &= ExpectResponse(plugin, context, 7274, 7275, 1, write);
        ok &= ExpectResponse(plugin, context, 7381, 7382, 1, write);
        ok &= ExpectResponse(plugin, context, 7387, 7388, 1, write);
        ok &= ExpectResponse(plugin, context, 7408, 7409, 1, write);
        ok &= ExpectResponse(plugin, context, 7427, 7428, 1, write);
        ok &= ExpectHandled(plugin, context, 7497, 0, write);
        ok &= ExpectResponse(plugin, context, 7503, 7504, 1, write);
        ok &= ExpectResponse(plugin, context, 7531, 7382, 1, write);
        ok &= ExpectResponse(plugin, context, 7536, 7537, 1, write);
        ok &= ExpectResponse(plugin, context, 7541, 7542, 1, write);
        ok &= ExpectResponse(plugin, context, 7543, 7544, 1, write);
        ok &= ExpectResponse(plugin, context, 7550, 7551, 1, write);
        ok &= ExpectResponse(plugin, context, 7552, 7553, 1, write);
        ok &= ExpectResponse(plugin, context, 7521, 7522, 1, write);
        ok &= ExpectResponse(plugin, context, 7527, 7528, 1, write);
        ok &= ExpectResponse(plugin, context, 7534, 7535, 1, write);
        ok &= ExpectResponse(plugin, context, 7538, 7539, 1, write);
        ok &= ExpectResponse(plugin, context, 7584, 7586, 1, write);
        ok &= ExpectResponse(plugin, context, 7606, 7607, 1, write);
        ok &= ExpectResponse(plugin, context, 8078, 8079, 1, write);
        ok &= ExpectResponse(plugin, context, 8082, 8083, 1, write);
        ok &= ExpectResponse(plugin, context, 8095, 8096, 1, write);
        ok &= ExpectResponse(plugin, context, 8111, 8112, 1, write);
        ok &= ExpectResponse(plugin, context, 8113, 8114, 1, write);
        ok &= ExpectResponse(plugin, context, 8137, 8136, 1, write);
        ok &= ExpectResponse(plugin, context, 8211, 8212, 1, write);
        ok &= ExpectResponse(plugin, context, 8006, 8007, 1, write);
        ok &= ExpectResponse(plugin, context, 8009, 8010, 1, write);
        ok &= ExpectResponse(plugin, context, 8016, 8017, 1, write);
        ok &= ExpectSocialMatchPostComment(plugin, context, 90000000000042UL, write);
        ok &= ExpectHandled(plugin, context, 8041, 0, write);
        ok &= ExpectResponse(plugin, context, 8034, 8035, 1, write);
        ok &= ExpectResponse(plugin, context, 8073, 8074, 1, write);
        ok &= ExpectResponse(plugin, context, 8124, 8125, 1, write);
        ok &= ExpectHandled(plugin, context, 8255, 0, write);
        ok &= ExpectResponse(plugin, context, 8262, 8263, 1, write);
        ok &= ExpectResponse(plugin, context, 8268, 8269, 1, write);
        ok &= ExpectResponse(plugin, context, 8270, 8271, 1, write);
        ok &= ExpectResponse(plugin, context, 8274, 8275, 1, write);
        ok &= ExpectResponse(plugin, context, 8303, 8304, 1, write);
        ok &= ExpectResponse(plugin, context, 8305, 8306, 1, write);
        ok &= ExpectResponse(plugin, context, 8332, 8333, 1, write);
        ok &= ExpectResponse(plugin, context, 8334, 8335, 1, write);
        ok &= ExpectResponse(plugin, context, 8349, 8350, 1, write);
        ok &= ExpectResponse(plugin, context, 8676, 8677, 1, write);
        ok &= ExpectResponse(plugin, context, 8673, 8674, 1, write);
        ok &= ExpectResponse(plugin, context, 8687, 8688, 1, write);
        ok &= ExpectResponse(plugin, context, 8716, 8717, 1, write);
        ok &= ExpectHandled(plugin, context, 8718, 0, write);
        ok &= ExpectResponse(plugin, context, 8727, 8728, 1, write);
        ok &= ExpectResponse(plugin, context, 8729, 8730, 1, write);
        ok &= ExpectResponse(plugin, context, 8793, 8794, 1, write);
        ok &= ExpectResponse(plugin, context, 8798, 8799, 1, write);
        ok &= ExpectResponse(plugin, context, 8800, 8801, 1, write);
        ok &= ExpectResponse(plugin, context, 8851, 8852, 1, write);
        ok &= ExpectResponse(plugin, context, 8853, 8854, 1, write);
        ok &= ExpectResponse(plugin, context, 8879, 8880, 1, write);
        ok &= ExpectResponse(plugin, context, 8886, 8887, 1, write);
        ok &= ExpectResponse(plugin, context, 8944, 8945, 1, write);
        ok &= ExpectResponse(plugin, context, 9023, 9024, 1, write);
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

    private static bool ExpectTargetedResponse(
        GameCoordinatorScriptPlugin plugin,
        GameCoordinatorContext context,
        uint requestType,
        uint expectedResponseType,
        ulong sourceJobId,
        Action<string> write,
        byte[]? body = null)
    {
        var response = plugin.Exchange(context, body == null
            ? Request(requestType, sourceJobId: sourceJobId)
            : Request(requestType, body, sourceJobId: sourceJobId));
        var directReply = response.Messages.SingleOrDefault(message => message.MessageType == expectedResponseType);
        var pushMessagesUntargeted = response.Messages
            .Where(message => message.MessageType != expectedResponseType)
            .All(message => message.TargetJobId == null);
        var ok = response.Handled
            && directReply != null
            && directReply.Protobuf
            && directReply.TargetJobId == sourceJobId
            && pushMessagesUntargeted;
        var actual = response.Messages
            .Select(message => $"{message.MessageType}:{(message.TargetJobId.HasValue ? message.TargetJobId.Value.ToString() : "-")}");
        write(
            $"{requestType} -> handled={response.Handled}, messages=[{string.Join(",", actual)}], expected={expectedResponseType}:{sourceJobId}, ok={ok}");
        return ok;
    }

    private static bool ExpectCreateLobbyFlow(
        GameCoordinatorScriptPlugin plugin,
        GameCoordinatorContext context,
        Action<string> write)
    {
        const ulong sourceJobId = 61;
        var response = plugin.Exchange(context, Request(7038, PracticeLobbyCreateBody(), sourceJobId: sourceJobId));
        var subscribe = response.Messages.Count >= 1 && response.Messages[0].MessageType == 24
            ? Deserialize<CMsgSOCacheSubscribed>(response.Messages[0].PayloadBase64)
            : null;
        var singleObject = response.Messages.Count >= 2 && response.Messages[1].MessageType == 21
            ? Deserialize<CMsgSOSingleObject>(response.Messages[1].PayloadBase64)
            : null;
        var result = response.Messages.Count >= 3 && response.Messages[2].MessageType == 7055
            ? Deserialize<CMsgGenericResult>(response.Messages[2].PayloadBase64)
            : null;
        var subscribedLobbyPayload = subscribe?.Objects.FirstOrDefault(item => item.TypeId == 2004)?.ObjectDatas.FirstOrDefault();
        var staticLobbyPayload = subscribe?.Objects.FirstOrDefault(item => item.TypeId == 2014)?.ObjectDatas.FirstOrDefault();
        var serverLobbyPayload = subscribe?.Objects.FirstOrDefault(item => item.TypeId == 2015)?.ObjectDatas.FirstOrDefault();
        var serverStaticLobbyPayload = subscribe?.Objects.FirstOrDefault(item => item.TypeId == 2016)?.ObjectDatas.FirstOrDefault();
        var subscribedLobby = subscribedLobbyPayload is { Length: > 0 }
            ? DeserializeBytes<CSODOTALobby>(subscribedLobbyPayload)
            : null;
        var staticLobby = staticLobbyPayload is { Length: > 0 }
            ? DeserializeBytes<CSODOTAStaticLobby>(staticLobbyPayload)
            : null;
        var serverLobby = serverLobbyPayload is { Length: > 0 }
            ? DeserializeBytes<CSODOTAServerLobby>(serverLobbyPayload)
            : null;
        var serverStaticLobby = serverStaticLobbyPayload is { Length: > 0 }
            ? DeserializeBytes<CSODOTAServerStaticLobby>(serverStaticLobbyPayload)
            : null;
        var singleLobby = singleObject?.ObjectData is { Length: > 0 }
            ? DeserializeBytes<CSODOTALobby>(singleObject.ObjectData)
            : null;
        var subscribeTypes = subscribe?.Objects.Select(item => item.TypeId).ToArray() ?? Array.Empty<int>();
        var extraMessage = subscribedLobby?.ExtraMessages.FirstOrDefault();
        var serverStaticMember = serverStaticLobby?.AllMembers.FirstOrDefault();
        var ok = response.Handled
            && response.Messages.Count == 3
            && response.Messages[0].TargetJobId == null
            && response.Messages[1].TargetJobId == null
            && response.Messages[2].TargetJobId == sourceJobId
            && subscribe != null
            && subscribeTypes.SequenceEqual([2004, 2013, 2014, 2015, 2016])
            && singleObject?.TypeId == 2004
            && result?.Eresult == 1
            && subscribedLobby?.LobbyId == singleLobby?.LobbyId
            && subscribedLobby?.AllowSpectating == false
            && subscribedLobby?.LobbyId > 9007199254740991UL
            && subscribedLobby.SeriesType == 0
            && subscribedLobby.TeamDetails.Count == 0
            && extraMessage?.Id == 8821
            && extraMessage.Contents.SequenceEqual(new byte[] { 8, 0 })
            && staticLobby?.AllMembers.Count == 1
            && staticLobby.AllMembers[0].Name == context.PersonaName
            && serverLobby?.AllMembers.Count == 1
            && serverStaticLobby?.AllMembers.Count == 1
            && serverStaticMember?.SteamId == context.SteamId
            && serverStaticMember.IsPlusSubscriber
            && serverStaticMember.FavoriteTeamPacked == 0UL
            && serverStaticMember.BannedHeroIds.SequenceEqual([75, 0, 0, 0]);
        var actual = response.Messages
            .Select(message => $"{message.MessageType}:{(message.TargetJobId.HasValue ? message.TargetJobId.Value.ToString() : "-")}");
        write(
            $"create lobby flow -> handled={response.Handled}, messages=[{string.Join(",", actual)}], " +
            $"singleType={singleObject?.TypeId}, subscribeTypes=[{string.Join(",", subscribeTypes)}], " +
            $"lobbyId={subscribedLobby?.LobbyId}, allowSpectating={subscribedLobby?.AllowSpectating}, " +
            $"singleLobbyId={singleLobby?.LobbyId}, " +
            $"seriesType={subscribedLobby?.SeriesType}, teamDetails={subscribedLobby?.TeamDetails.Count}, " +
            $"extraMsg={extraMessage?.Id}:{(extraMessage?.Contents is null ? "" : Convert.ToHexString(extraMessage.Contents))}, " +
            $"staticMembers={staticLobby?.AllMembers.Count}, staticName={staticLobby?.AllMembers.FirstOrDefault()?.Name}, " +
            $"serverLobbyMembers={serverLobby?.AllMembers.Count}, serverStaticMembers={serverStaticLobby?.AllMembers.Count}, " +
            $"serverStaticSteamId={serverStaticMember?.SteamId}, plus={serverStaticMember?.IsPlusSubscriber}, " +
            $"result={result?.Eresult}, ok={ok}");
        return ok;
    }

    private static bool ExpectGameServerWelcomeFlow(
        GameCoordinatorScriptPlugin plugin,
        GameCoordinatorContext serverContext,
        Action<string> write)
    {
        var response = plugin.Exchange(
            serverContext,
            RequestFor(
                serverContext,
                4007,
                Serialize(new CMsgClientHello { Version = 6863 }),
                gameServer: true));
        var welcome = response.Messages.Count == 1 && response.Messages[0].MessageType == 4005
            ? Deserialize<CMsgClientWelcome>(response.Messages[0].PayloadBase64)
            : null;
        var ok = response.Handled
            && response.Messages.Count == 1
            && response.Messages[0].TargetJobId == null
            && response.Messages[0].Protobuf
            && welcome?.Version == 6860
            && welcome.GcSocacheFileVersion == 20;
        write(
            $"game server welcome flow -> handled={response.Handled}, messages={response.Messages.Count}, " +
            $"version={welcome?.Version}, gcSocache={welcome?.GcSocacheFileVersion}, ok={ok}");
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

    private static bool ExpectApplyTeamFlow(
        GameCoordinatorScriptPlugin plugin,
        GameCoordinatorContext context,
        Action<string> write)
    {
        var response = plugin.Exchange(context, Request(7142, Serialize(new CMsgApplyTeamToPracticeLobby { TeamId = 7733573 })));
        var update = response.Messages.Count >= 1 && response.Messages[0].MessageType == 26
            ? Deserialize<CMsgSOMultipleObjects>(response.Messages[0].PayloadBase64)
            : null;
        var result = response.Messages.Count >= 2 && response.Messages[1].MessageType == 2579
            ? Deserialize<CMsgGenericResult>(response.Messages[1].PayloadBase64)
            : null;
        var lobbyPayload = update?.ObjectsModifieds.FirstOrDefault(item => item.TypeId == 2004)?.ObjectData;
        var lobby = lobbyPayload is { Length: > 0 } ? DeserializeBytes<CSODOTALobby>(lobbyPayload) : null;
        var radiant = lobby?.TeamDetails.Count > 0 ? lobby.TeamDetails[0] : null;
        var dire = lobby?.TeamDetails.Count > 1 ? lobby.TeamDetails[1] : null;
        var ok = response.Handled
            && response.Messages.Count == 2
            && update != null
            && result?.Eresult == 1
            && radiant?.TeamId == 7733573
            && radiant.TeamName == "SKYNET"
            && radiant.TeamComplete
            && radiant.TeamLogo == 3255294647392078090UL
            && radiant.TeamBaseLogo == 7163376947542189088UL
            && radiant.TeamBannerLogo == 7954877705993612385UL
            && dire != null
            && dire.TeamId == 0;
        write(
            $"apply team flow -> handled={response.Handled}, messages={response.Messages.Count}, " +
            $"team={radiant?.TeamId}, name={radiant?.TeamName}, result={result?.Eresult}, ok={ok}");
        return ok;
    }

    private static bool ExpectLaunchFlow(
        GameCoordinatorScriptPlugin plugin,
        GameCoordinatorContext context,
        Action<string> write)
    {
        const ulong sourceJobId = 62;
        var response = plugin.Exchange(
            context,
            Request(7041, Serialize(new CMsgPracticeLobbyLaunch { ClientVersion = 6856 }), sourceJobId: sourceJobId));
        var update = response.Messages.Count >= 1 && response.Messages[0].MessageType == 26
            ? Deserialize<CMsgSOMultipleObjects>(response.Messages[0].PayloadBase64)
            : null;
        var result = response.Messages.Count >= 2 && response.Messages[1].MessageType == 2579
            ? Deserialize<CMsgGenericResult>(response.Messages[1].PayloadBase64)
            : null;
        var lobbyPayload = update?.ObjectsModifieds.FirstOrDefault(item => item.TypeId == 2004)?.ObjectData;
        var lobby = lobbyPayload is { Length: > 0 } ? DeserializeBytes<CSODOTALobby>(lobbyPayload) : null;
        var radiant = lobby?.TeamDetails.Count > 0 ? lobby.TeamDetails[0] : null;
        var ok = response.Handled
            && response.Messages.Count == 2
            && update != null
            && result?.Eresult == 1
            && response.Messages[0].TargetJobId == null
            && response.Messages[1].TargetJobId == sourceJobId
            && lobby != null
            && lobby.state == CSODOTALobby.State.Serversetup
            && lobby.Lan
            && lobby.ServerRegion == 0
            && lobby.GameStartTime == 0
            && radiant?.TeamId == 7733573
            && !radiant.TeamComplete;
        write(
            $"launch flow -> handled={response.Handled}, messages={response.Messages.Count}, " +
            $"state={lobby?.state}, result={result?.Eresult}, teamComplete={radiant?.TeamComplete}, ok={ok}");
        return ok;
    }

    private static bool ExpectDedicatedAttachFlow(
        GameCoordinatorScriptPlugin plugin,
        GameCoordinatorContext serverContext,
        List<(ulong SteamId, ApiGCMessage Message)> queuedMessages,
        Action<string> write)
    {
        queuedMessages.Clear();

        var info = new CMsgGameServerInfo
        {
            ServerPublicIpAddr = IpToUInt32(97, 120, 234, 36),
            ServerPrivateIpAddr = IpToUInt32(192, 168, 212, 252),
            ServerPort = 27015,
            ServerTvPort = 37025
        };
        var infoResponse = plugin.Exchange(serverContext, RequestFor(serverContext, 4508, Serialize(info), gameServer: true));
        var availableResponse = plugin.Exchange(serverContext, RequestFor(serverContext, 4506, gameServer: true));
        var lobby = LastQueuedClientLobby(queuedMessages);
        var realtimeStatsMessage = availableResponse.Messages.FirstOrDefault(message => message.MessageType == 8042);
        var realtimeStats = realtimeStatsMessage == null
            ? null
            : Deserialize<CMsgGCToServerRealtimeStatsStartStop>(realtimeStatsMessage.PayloadBase64);
        var radiant = lobby?.TeamDetails.Count > 0 ? lobby.TeamDetails[0] : null;
        var ok = infoResponse.Handled
            && availableResponse.Handled
            && realtimeStats?.Delayed == true
            && lobby != null
            && lobby.state == CSODOTALobby.State.Run
            && lobby.ServerId == serverContext.SteamId
            && lobby.Connect == "97.120.234.36:27015 192.168.212.252:27015"
            && lobby.GameStartTime > 0
            && radiant?.TeamId == 7733573
            && !radiant.TeamComplete;
        write(
            $"dedicated attach flow -> infoHandled={infoResponse.Handled}, availableHandled={availableResponse.Handled}, " +
            $"queued={queuedMessages.Count}, state={lobby?.state}, connect={lobby?.Connect}, " +
            $"realtimeDelayed={realtimeStats?.Delayed}, ok={ok}");
        return ok;
    }

    private static bool ExpectConnectedPlayersFlow(
        GameCoordinatorScriptPlugin plugin,
        GameCoordinatorContext clientContext,
        GameCoordinatorContext serverContext,
        List<(ulong SteamId, ApiGCMessage Message)> queuedMessages,
        Action<string> write)
    {
        queuedMessages.Clear();

        var players = new CMsgConnectedPlayers
        {
            GameState = DOTAGameState.DotaGamerulesStateHeroSelection,
            send_reason = CMsgConnectedPlayers.SendReason.GameState
        };
        players.ConnectedPlayers.Add(new CMsgConnectedPlayers.Player
        {
            SteamId = clientContext.SteamId,
            HeroId = 0
        });

        var response = plugin.Exchange(serverContext, RequestFor(serverContext, 7034, Serialize(players), gameServer: true));
        var lobby = LastQueuedClientLobby(queuedMessages);
        var member = lobby?.AllMembers.FirstOrDefault(player => player.Id == clientContext.SteamId);
        var ok = response.Handled
            && lobby != null
            && lobby.state == CSODOTALobby.State.Run
            && lobby.GameState == DOTAGameState.DotaGamerulesStateHeroSelection
            && member != null
            && member.LeaverStatus == DOTALeaverStatust.DotaLeaverNone;
        write(
            $"connected players flow -> handled={response.Handled}, queued={queuedMessages.Count}, " +
            $"state={lobby?.state}, gameState={lobby?.GameState}, leaver={member?.LeaverStatus}, ok={ok}");
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

    private static bool ExpectSocialMatchDetails(
        GameCoordinatorScriptPlugin plugin,
        GameCoordinatorContext context,
        ulong matchId,
        Action<string> write)
    {
        var response = plugin.Exchange(context, Request(7095, Serialize(new CMsgGCMatchDetailsRequest { MatchId = matchId })));
        var details = response.Messages.Count == 1 && response.Messages[0].MessageType == 7096
            ? Deserialize<CMsgGCMatchDetailsResponse>(response.Messages[0].PayloadBase64)
            : null;
        var ok = response.Handled
            && details != null
            && details.Result == 1
            && details.Match != null
            && details.Match.MatchId == matchId
            && details.Match.Players.Count == 1
            && details.Match.Players[0].AccountId == context.AccountId;
        write(
            $"social match details -> handled={response.Handled}, result={details?.Result}, " +
            $"match={details?.Match?.MatchId}, players={details?.Match?.Players.Count}, " +
            $"account={details?.Match?.Players.FirstOrDefault()?.AccountId}, ok={ok}");
        return ok;
    }

    private static bool ExpectSocialMatchPostComment(
        GameCoordinatorScriptPlugin plugin,
        GameCoordinatorContext context,
        ulong matchId,
        Action<string> write)
    {
        var request = new CMsgClientToGCSocialFeedPostCommentRequest
        {
            EventId = matchId,
            Comment = "self-check social match comment"
        };
        var response = plugin.Exchange(context, Request(8016, Serialize(request)));
        var result = response.Messages.Count == 1 && response.Messages[0].MessageType == 8017
            ? Deserialize<CMsgGCToClientSocialFeedPostCommentResponse>(response.Messages[0].PayloadBase64)
            : null;
        var comments = DotaGcRuntimeServices.StatsStore?.GetSocialMatchComments(matchId) ?? Array.Empty<DotaStatsComment>();
        var ok = response.Handled
            && result != null
            && result.Success
            && comments.Any(comment =>
                comment.AccountId == context.AccountId &&
                comment.Comment == "self-check social match comment");
        write($"social match post comment -> handled={response.Handled}, persisted={comments.Count}, ok={ok}");
        return ok;
    }

    private static void SeedSocialMatchData(DotaStatsStore store, GameCoordinatorContext context)
    {
        var match = new DotaStatsMatch
        {
            MatchId = 90000000000042UL,
            OwnerSteamId = context.SteamId,
            ServerSteamId = 85568392920047069UL,
            StartTime = (uint)DateTimeOffset.UtcNow.AddMinutes(-40).ToUnixTimeSeconds(),
            Duration = 1800,
            GameMode = 22,
            LobbyType = 7,
            GoodGuysWin = true,
            MatchFlags = 0,
            RadiantScore = 42,
            DireScore = 31,
            Cluster = 227,
            FirstBloodTime = 82
        };
        match.Players.Add(new DotaStatsMatchPlayer
        {
            MatchId = match.MatchId,
            AccountId = context.AccountId,
            SteamId = context.SteamId,
            PersonaName = context.PersonaName,
            Team = 0,
            PlayerSlot = 0,
            HeroId = 1,
            Kills = 10,
            Deaths = 2,
            Assists = 14,
            Winner = true,
            GoodGuys = true,
            Gold = 1200,
            GoldSpent = 17500,
            Gpm = 560,
            Xpm = 720,
            LastHits = 210,
            Denies = 12,
            HeroDamage = 26000,
            TowerDamage = 4200,
            HeroHealing = 0,
            Level = 24,
            NetWorth = 21000,
            Items = new List<uint> { 50, 63, 116, 147, 160, 168 },
            StartTime = match.StartTime,
            Duration = match.Duration,
            GameMode = match.GameMode,
            LobbyType = match.LobbyType,
            GoodGuysWin = match.GoodGuysWin,
            MatchFlags = match.MatchFlags,
            RadiantScore = match.RadiantScore,
            DireScore = match.DireScore,
            Cluster = match.Cluster,
            FirstBloodTime = match.FirstBloodTime
        });
        store.RecordMatch(match);
    }

    private static byte[] PracticeLobbyCreateBody()
    {
        return Serialize(new CMsgPracticeLobbyCreate
        {
            ClientVersion = 6856,
            PassKey = string.Empty,
            LobbyDetails = new CMsgPracticeLobbySetDetails
            {
                LobbyId = 0,
                GameName = "Sala 1",
                ServerRegion = 0,
                GameMode = 1,
                CmPick = DotaCmPick.DotaCmRandom,
                BotDifficultyRadiant = DOTABotDifficulty.BotDifficultyHard,
                BotDifficultyDire = DOTABotDifficulty.BotDifficultyHard,
                AllowSpectating = true,
                PassKey = string.Empty,
                Leagueid = 0,
                PenaltyLevelRadiant = 0,
                PenaltyLevelDire = 0,
                SeriesType = 0,
                RadiantSeriesWins = 0,
                DireSeriesWins = 0,
                Allchat = false,
                DotaTvDelay = LobbyDotaTVDelay.LobbyDotaTV10,
                Lan = true,
                Visibility = DOTALobbyVisibility.DOTALobbyVisibilityPublic,
                PauseSetting = LobbyDotaPauseSetting.LobbyDotaPauseSettingUnlimited
            }
        });
    }

    private static ApiGCExchangeRequest Request(uint messageType, ulong? sourceJobId = null)
    {
        return new ApiGCExchangeRequest
        {
            AppId = DotaAppId,
            MessageType = messageType,
            BodyBase64 = string.Empty,
            SteamId = TestSteamId,
            GameServer = false,
            SourceJobId = sourceJobId
        };
    }

    private static ApiGCExchangeRequest Request(uint messageType, byte[] body, ulong? sourceJobId = null)
    {
        var request = Request(messageType, sourceJobId);
        request.BodyBase64 = Convert.ToBase64String(body);
        return request;
    }

    private static ApiGCExchangeRequest RequestFor(
        GameCoordinatorContext context,
        uint messageType,
        bool gameServer = false,
        ulong? sourceJobId = null)
    {
        return new ApiGCExchangeRequest
        {
            AppId = context.AppId,
            MessageType = messageType,
            BodyBase64 = string.Empty,
            SteamId = context.SteamId,
            GameServer = gameServer,
            SourceJobId = sourceJobId
        };
    }

    private static ApiGCExchangeRequest RequestFor(
        GameCoordinatorContext context,
        uint messageType,
        byte[] body,
        bool gameServer = false,
        ulong? sourceJobId = null)
    {
        var request = RequestFor(context, messageType, gameServer, sourceJobId);
        request.BodyBase64 = Convert.ToBase64String(body);
        return request;
    }

    private static CSODOTALobby? LastQueuedClientLobby(List<(ulong SteamId, ApiGCMessage Message)> queuedMessages)
    {
        for (var i = queuedMessages.Count - 1; i >= 0; i--)
        {
            var message = queuedMessages[i].Message;
            if (message.MessageType != 26)
            {
                continue;
            }

            var update = Deserialize<CMsgSOMultipleObjects>(message.PayloadBase64);
            var lobbyPayload = update.ObjectsModifieds.FirstOrDefault(item => item.TypeId == 2004)?.ObjectData;
            if (lobbyPayload is { Length: > 0 })
            {
                return DeserializeBytes<CSODOTALobby>(lobbyPayload);
            }
        }

        return null;
    }

    private static uint IpToUInt32(byte a, byte b, byte c, byte d)
    {
        return (uint)(a | (b << 8) | (c << 16) | (d << 24));
    }

    private static byte[] Serialize<TMessage>(TMessage message)
    {
        using var stream = new MemoryStream();
        ProtoBuf.Serializer.Serialize(stream, message);
        return stream.ToArray();
    }

    private static TMessage Deserialize<TMessage>(string? payloadBase64)
    {
        return DeserializeBytes<TMessage>(Convert.FromBase64String(payloadBase64 ?? string.Empty));
    }

    private static TMessage DeserializeBytes<TMessage>(byte[] payload)
    {
        using var stream = new MemoryStream(payload);
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
