using ProtoBuf;

[ProtoContract(Name = @"CMsgRetrieveMatchVote")]
public sealed class CMsgRetrieveMatchVote : IExtensible
{
    private IExtension? _extensionData;
    IExtension IExtensible.GetExtensionObject(bool createIfMissing) =>
        Extensible.GetExtensionObject(ref _extensionData, createIfMissing);

    [ProtoMember(1, Name = @"match_id")]
    public ulong MatchId { get; set; }

    [ProtoMember(2, Name = @"incremental")]
    public uint Incremental { get; set; }
}

[ProtoContract(Name = @"CMsgMatchVoteResponse")]
public sealed class CMsgMatchVoteResponse : IExtensible
{
    private IExtension? _extensionData;
    IExtension IExtensible.GetExtensionObject(bool createIfMissing) =>
        Extensible.GetExtensionObject(ref _extensionData, createIfMissing);

    [ProtoMember(1, Name = @"eresult")]
    public uint Eresult { get; set; } = 2;

    [ProtoMember(2, Name = @"vote")]
    public DOTAMatchVote Vote { get; set; } = DOTAMatchVote.DOTAMatchVoteINVALID;

    [ProtoMember(3, Name = @"positive_votes")]
    public uint PositiveVotes { get; set; }

    [ProtoMember(4, Name = @"negative_votes")]
    public uint NegativeVotes { get; set; }
}

[ProtoContract(Name = @"CMsgClientToGCSocialMatchDetailsRequest")]
public sealed class CMsgClientToGCSocialMatchDetailsRequest : IExtensible
{
    private IExtension? _extensionData;
    IExtension IExtensible.GetExtensionObject(bool createIfMissing) =>
        Extensible.GetExtensionObject(ref _extensionData, createIfMissing);

    [ProtoMember(1, Name = @"match_id")]
    public ulong MatchId { get; set; }
}

[ProtoContract(Name = @"CMsgGCToClientSocialMatchDetailsResponse")]
public sealed class CMsgGCToClientSocialMatchDetailsResponse : IExtensible
{
    private IExtension? _extensionData;
    IExtension IExtensible.GetExtensionObject(bool createIfMissing) =>
        Extensible.GetExtensionObject(ref _extensionData, createIfMissing);

    [ProtoMember(1, Name = @"success")]
    public bool Success { get; set; }

    [ProtoMember(2, Name = @"comments")]
    public List<Comment> Comments { get; } = new();

    [ProtoContract(Name = @"Comment")]
    public sealed class Comment : IExtensible
    {
        private IExtension? _extensionData;
        IExtension IExtensible.GetExtensionObject(bool createIfMissing) =>
            Extensible.GetExtensionObject(ref _extensionData, createIfMissing);

        [ProtoMember(1, Name = @"account_id")]
        public uint AccountId { get; set; }

        [ProtoMember(2, Name = @"comment")]
        public string Text { get; set; } = string.Empty;

        [ProtoMember(3, Name = @"persona_name")]
        public string PersonaName { get; set; } = string.Empty;

        [ProtoMember(4, Name = @"timestamp")]
        public uint Timestamp { get; set; }
    }
}

[ProtoContract(Name = @"CMsgClientToGCSocialMatchPostCommentRequest")]
public sealed class CMsgClientToGCSocialMatchPostCommentRequest : IExtensible
{
    private IExtension? _extensionData;
    IExtension IExtensible.GetExtensionObject(bool createIfMissing) =>
        Extensible.GetExtensionObject(ref _extensionData, createIfMissing);

    [ProtoMember(1, Name = @"match_id")]
    public ulong MatchId { get; set; }

    [ProtoMember(2, Name = @"comment")]
    public string Comment { get; set; } = string.Empty;
}

[ProtoContract(Name = @"CMsgGCToClientSocialMatchPostCommentResponse")]
public sealed class CMsgGCToClientSocialMatchPostCommentResponse : IExtensible
{
    private IExtension? _extensionData;
    IExtension IExtensible.GetExtensionObject(bool createIfMissing) =>
        Extensible.GetExtensionObject(ref _extensionData, createIfMissing);

    [ProtoMember(1, Name = @"success")]
    public bool Success { get; set; }
}

[ProtoContract(Name = @"CMsgGCGetHeroTimedStats")]
public sealed class CMsgGCGetHeroTimedStats : IExtensible
{
    private IExtension? _extensionData;
    IExtension IExtensible.GetExtensionObject(bool createIfMissing) =>
        Extensible.GetExtensionObject(ref _extensionData, createIfMissing);

    [ProtoMember(2, Name = @"hero_id")]
    public uint HeroId { get; set; }
}
