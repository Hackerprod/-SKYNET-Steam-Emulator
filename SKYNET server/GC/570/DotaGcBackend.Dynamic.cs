namespace SKYNET_server.GC.Dota2;

public sealed partial class DotaGcBackend
{
    private const uint JoinChatSuccess = 0;
    private const uint JoinChatResponseJoined = 1;
    private const uint DefaultChatMaxMembers = 250;

    public string DotaQuestProgressResponse()
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, 1);

        foreach (var questId in ReadVarintFields(_requestBody, 1).Select(value => (uint)value).Where(value => value != 0))
        {
            var quest = new List<byte>();
            WriteVarintField(quest, 1, questId);
            WriteBytesField(response, 2, quest.ToArray());
        }

        return Encode(response.ToArray());
    }

    public string DotaPeriodicResourceResponse()
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, 1);
        WriteVarintField(response, 2, 0);
        return Encode(response.ToArray());
    }

    public string DotaTeamsInfoResponse()
    {
        var response = new List<byte>();
        return Encode(response.ToArray());
    }

    public string DotaHeroStickersResponse()
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, 1);
        WriteBytesField(response, 2, Array.Empty<byte>());
        return Encode(response.ToArray());
    }

    public string DotaEmoticonDataResponse()
    {
        var access = new List<byte>();
        WriteVarintField(access, 1, AccountId);
        WriteBytesField(access, 2, BuildUnlockedEmoticonMask());

        var response = new List<byte>();
        WriteBytesField(response, 1, access.ToArray());
        return Encode(response.ToArray());
    }

    public string DotaJoinChatChannelResponse()
    {
        TryReadStringField(_requestBody, 2, out var requestedChannelName);
        TryReadVarintField(_requestBody, 4, out var requestedChannelType);
        var channelName = string.IsNullOrWhiteSpace(requestedChannelName)
            ? "regional"
            : requestedChannelName.Trim();
        var channelId = BuildStableChatChannelId(channelName, (uint)requestedChannelType);

        var member = new List<byte>();
        WriteFixed64Field(member, 1, SteamId);
        WriteStringField(member, 2, PersonaName);
        WriteVarintField(member, 3, AccountId);
        WriteVarintField(member, 4, 0);

        var response = new List<byte>();
        WriteVarintField(response, 1, JoinChatResponseJoined);
        WriteStringField(response, 2, channelName);
        WriteFixed64Field(response, 3, channelId);
        WriteVarintField(response, 4, DefaultChatMaxMembers);
        WriteBytesField(response, 5, member.ToArray());
        WriteVarintField(response, 6, requestedChannelType);
        WriteVarintField(response, 7, JoinChatSuccess);
        WriteVarintField(response, 9, AccountId);
        return Encode(response.ToArray());
    }

    private static byte[] BuildUnlockedEmoticonMask()
    {
        var unlocked = new byte[sizeof(ulong) * 9];
        for (var i = 0; i < unlocked.Length; i++)
        {
            unlocked[i] = 0xFF;
        }

        return unlocked;
    }

    private static ulong BuildStableChatChannelId(string channelName, uint channelType)
    {
        const ulong offset = 14695981039346656037UL;
        const ulong prime = 1099511628211UL;
        var hash = offset;
        foreach (var value in System.Text.Encoding.UTF8.GetBytes($"{channelType}:{channelName.ToLowerInvariant()}"))
        {
            hash ^= value;
            hash *= prime;
        }

        return hash == 0 ? 1 : hash;
    }
}
