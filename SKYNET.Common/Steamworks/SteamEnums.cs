
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
}
