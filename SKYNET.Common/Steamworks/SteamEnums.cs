
namespace SKYNET.Steamworks
{
	public enum EAccountType 
	{
		k_EAccountTypeInvalid = 0,
		k_EAccountTypeIndividual = 1,
		k_EAccountTypeMultiseat = 2,
		k_EAccountTypeGameServer = 3,
		k_EAccountTypeAnonGameServer = 4,
		k_EAccountTypePending = 5,
		k_EAccountTypeContentServer = 6,
		k_EAccountTypeClan = 7,
		k_EAccountTypeChat = 8,
		k_EAccountTypeConsoleUser = 9,
		k_EAccountTypeAnonUser = 10,
		k_EAccountTypeMax = 11
	}

    public enum EUniverse : int
    {
        k_EUniverseInvalid,
        k_EUniversePublic,
        k_EUniverseBeta,
        k_EUniverseInternal,
        k_EUniverseDev,
        k_EUniverseMax
    }

    public enum EChatMemberStateChange
    {
        k_EChatMemberStateChangeEntered = 0x0001,
        k_EChatMemberStateChangeLeft = 0x0002,
        k_EChatMemberStateChangeDisconnected = 0x0004,
        k_EChatMemberStateChangeKicked = 0x0008,
        k_EChatMemberStateChangeBanned = 0x0010,
    };

    public enum EChatRoomEnterResponse
    {
        k_EChatRoomEnterResponseSuccess = 1,
        k_EChatRoomEnterResponseDoesntExist = 2,
        k_EChatRoomEnterResponseNotAllowed = 3,
        k_EChatRoomEnterResponseFull = 4,
        k_EChatRoomEnterResponseError = 5,
        k_EChatRoomEnterResponseBanned = 6,
        k_EChatRoomEnterResponseLimited = 7,
        k_EChatRoomEnterResponseClanDisabled = 8,
        k_EChatRoomEnterResponseCommunityBan = 9,
        k_EChatRoomEnterResponseMemberBlockedYou = 10,
        k_EChatRoomEnterResponseYouBlockedMember = 11,
        k_EChatRoomEnterResponseRatelimitExceeded = 15
    }
}
