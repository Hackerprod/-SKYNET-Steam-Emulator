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

    public enum EChatSteamIDInstanceFlags 
    {
        k_EChatAccountInstanceMask = 4095,
        k_EChatInstanceFlagMMSLobby = 131072,
        k_EChatInstanceFlagLobby = 262144,
        k_EChatInstanceFlagClan = 524288
    }
}
