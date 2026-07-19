namespace SKYNET_server.Services;

public sealed class DotaChatStore
{
    public const uint MaxMembers = 200;

    private readonly object _sync = new();
    private readonly Dictionary<(uint ChannelType, string NameKey), ChatChannel> _channelsByKey = new();
    private readonly Dictionary<ulong, ChatChannel> _channelsById = new();
    private ulong _nextChannelId = 258000;

    public DotaChatChannelSnapshot? Join(string channelName, uint channelType, ulong steamId, uint accountId, string personaName)
    {
        var name = NormalizeName(channelName);
        lock (_sync)
        {
            var key = (channelType, name.ToLowerInvariant());
            if (!_channelsByKey.TryGetValue(key, out var channel))
            {
                channel = new ChatChannel
                {
                    Id = _nextChannelId++,
                    Name = name,
                    ChannelType = channelType
                };
                _channelsByKey[key] = channel;
                _channelsById[channel.Id] = channel;
            }

            var justJoined = false;
            if (!channel.Members.TryGetValue(steamId, out var member))
            {
                if (channel.Members.Count >= MaxMembers)
                {
                    return null;
                }

                member = new ChatMember
                {
                    SteamId = steamId,
                    AccountId = accountId,
                    PersonaName = personaName ?? string.Empty,
                    ChannelUserId = channel.NextChannelUserId++
                };
                channel.Members[steamId] = member;
                justJoined = true;
            }

            return Snapshot(channel, steamId, justJoined);
        }
    }

    public DotaChatChannelSnapshot? Get(ulong channelId, ulong steamId)
    {
        lock (_sync)
        {
            return _channelsById.TryGetValue(channelId, out var channel)
                ? Snapshot(channel, steamId)
                : null;
        }
    }

    public bool Leave(ulong channelId, ulong steamId)
    {
        lock (_sync)
        {
            if (!_channelsById.TryGetValue(channelId, out var channel) || !channel.Members.Remove(steamId))
            {
                return false;
            }

            if (channel.Members.Count == 0)
            {
                _channelsById.Remove(channel.Id);
                _channelsByKey.Remove((channel.ChannelType, channel.Name.ToLowerInvariant()));
            }

            return true;
        }
    }

    public List<ulong> GetMemberSteamIds(ulong channelId)
    {
        lock (_sync)
        {
            return _channelsById.TryGetValue(channelId, out var channel)
                ? channel.Members.Keys.ToList()
                : new List<ulong>();
        }
    }

    private static string NormalizeName(string channelName)
    {
        var trimmed = (channelName ?? string.Empty).Trim();
        return trimmed.Length == 0 ? "regional" : trimmed;
    }

    private static DotaChatChannelSnapshot Snapshot(ChatChannel channel, ulong selfSteamId, bool selfJustJoined = false)
    {
        var selfMember = channel.Members.GetValueOrDefault(selfSteamId);
        return new DotaChatChannelSnapshot
        {
            ChannelId = channel.Id,
            ChannelName = channel.Name,
            ChannelType = channel.ChannelType,
            MaxMembers = MaxMembers,
            SelfIsMember = selfMember != null,
            SelfChannelUserId = selfMember?.ChannelUserId ?? 0,
            SelfJustJoined = selfJustJoined,
            Members = channel.Members.Values
                .Select(member => new DotaChatMemberSnapshot
                {
                    SteamId = member.SteamId,
                    AccountId = member.AccountId,
                    PersonaName = member.PersonaName,
                    ChannelUserId = member.ChannelUserId
                })
                .ToList()
        };
    }

    private sealed class ChatChannel
    {
        public ulong Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public uint ChannelType { get; init; }
        public uint NextChannelUserId { get; set; }
        public Dictionary<ulong, ChatMember> Members { get; } = new();
    }

    private sealed class ChatMember
    {
        public ulong SteamId { get; init; }
        public uint AccountId { get; init; }
        public string PersonaName { get; init; } = string.Empty;
        public uint ChannelUserId { get; init; }
    }
}

public sealed class DotaChatChannelSnapshot
{
    public ulong ChannelId { get; init; }
    public string ChannelName { get; init; } = string.Empty;
    public uint ChannelType { get; init; }
    public uint MaxMembers { get; init; }
    public bool SelfIsMember { get; init; }
    public uint SelfChannelUserId { get; init; }
    public bool SelfJustJoined { get; init; }
    public List<DotaChatMemberSnapshot> Members { get; init; } = new();
}

public sealed class DotaChatMemberSnapshot
{
    public ulong SteamId { get; init; }
    public uint AccountId { get; init; }
    public string PersonaName { get; init; } = string.Empty;
    public uint ChannelUserId { get; init; }
}
