using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamParentalSettings
    {
        bool BIsParentalLockEnabled(IntPtr _);
        bool BIsParentalLockLocked(IntPtr _);
        bool BIsAppBlocked(IntPtr nAppID);
        bool BIsAppInBlockList(IntPtr nAppID);
        bool BIsFeatureBlocked(EParentalFeature eFeature);
        bool BIsFeatureInBlockList(EParentalFeature eFeature);
    }
    // Feature types for parental settings
    public enum EParentalFeature : int
    {
        k_EFeatureInvalid = 0,
        k_EFeatureStore = 1,
        k_EFeatureCommunity = 2,
        k_EFeatureProfile = 3,
        k_EFeatureFriends = 4,
        k_EFeatureNews = 5,
        k_EFeatureTrading = 6,
        k_EFeatureSettings = 7,
        k_EFeatureConsole = 8,
        k_EFeatureBrowser = 9,
        k_EFeatureParentalSetup = 10,
        k_EFeatureLibrary = 11,
        k_EFeatureTest = 12,
        k_EFeatureSiteLicense = 13,
        k_EFeatureMax
    };
}
