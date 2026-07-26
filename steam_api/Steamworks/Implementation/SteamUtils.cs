using System;
using System.Runtime.InteropServices;
using System.Text;
using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks.Interfaces;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamUtils : ISteamInterface
    {
        public static SteamUtils Instance;

        public DateTime ActiveTime;
        private readonly object TextInputGate = new object();
        private string EnteredGamepadText = string.Empty;
        private bool HasEnteredGamepadText;
        private bool GamepadTextInputPending;

        public SteamUtils()
        {
            Instance = this;
            InterfaceName = "SteamUtils";
            InterfaceVersion = "SteamUtils010";
            ActiveTime = DateTime.Now;
        }

        public uint GetSecondsSinceAppActive()
        {
            Write("GetSecondsSinceAppActive");
            return checked((uint)Math.Min(
                uint.MaxValue,
                Math.Max(0, (DateTime.Now - ActiveTime).TotalSeconds)));
        }

        public uint GetSecondsSinceComputerActive()
        {
            Write("GetSecondsSinceComputerActive");
            var inactive = Common.GetInactiveTimeSpan();
            return inactive.HasValue
                ? checked((uint)Math.Min(uint.MaxValue, Math.Max(0, inactive.Value.TotalSeconds)))
                : 0;
        }

        public int GetConnectedUniverse()
        {
            Write("GetConnectedUniverse");
            return (int)EUniverse.k_EUniversePublic;
        }

        public uint GetServerRealTime()
        {
            uint ServerTime = (uint)(new DateTimeOffset(DateTime.Now)).ToUnixTimeSeconds(); 
            Write($"GetServerRealTime");
            return ServerTime;
        }

        public string GetIPCountry()
        {
            Write("GetIPCountry");
            return "US";
        }

        public bool GetImageSize(int iImage, ref uint pnWidth, ref uint pnHeight)
        {
            Write($"GetImageSize");
            var Result = false;
            int Width = 0;
            int Height = 0;
            MutexHelper.Wait("GetImageSize", delegate
            {
                try
                {
                    var (width, height) = SteamFriends.Instance.GetImageSize(iImage);

                    if (width > 0 && height > 0)
                    {
                        Width  = width;
                        Height = height;
                        Result = true;
                    }
                }
                catch
                {

                }
            });

            pnWidth = (uint)Width;
            pnHeight = (uint)Height;
            return Result;
        }

        public bool GetImageRGBA(int iImage, IntPtr pubDest, int nDestBufferSize)
        {
            Write($"GetImageRGBA, {nDestBufferSize} bytes");
            var Result = false;

            MutexHelper.Wait("GetImageRGBA", delegate
            {
                try
                {
                    var avatar = SteamFriends.Instance.GetImageAvatar(iImage);
                    if (avatar != null && pubDest != IntPtr.Zero && nDestBufferSize >= 0)
                    {
                        byte[] bytes = avatar.GetImage(iImage);
                        if (bytes != null && bytes.Length > 0 && nDestBufferSize >= bytes.Length)
                        {
                            Marshal.Copy(bytes, 0, pubDest, bytes.Length);
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Write("" + ex);
                }
            });

            return Result;
        }

        public bool GetCSERIPPort(IntPtr unIP, IntPtr usPort)
        {
            Write("GetCSERIPPort");
            if (unIP != IntPtr.Zero)
            {
                Marshal.WriteInt32(unIP, 0);
            }
            if (usPort != IntPtr.Zero)
            {
                Marshal.WriteInt16(usPort, 0);
            }
            return false;
        }

        public byte GetCurrentBatteryPower()
        {
            try
            {
                var status = System.Windows.Forms.SystemInformation.PowerStatus;
                if (status.PowerLineStatus == System.Windows.Forms.PowerLineStatus.Online ||
                    status.BatteryChargeStatus == System.Windows.Forms.BatteryChargeStatus.NoSystemBattery)
                {
                    Write("GetCurrentBatteryPower AC");
                    return byte.MaxValue;
                }

                var percentage = status.BatteryLifePercent;
                if (float.IsNaN(percentage) || float.IsInfinity(percentage) || percentage < 0)
                {
                    Write("GetCurrentBatteryPower unknown");
                    return byte.MaxValue;
                }

                var result = checked((byte)Math.Round(
                    Math.Min(1.0f, percentage) * 100.0f,
                    MidpointRounding.AwayFromZero));
                Write($"GetCurrentBatteryPower {result}");
                return result;
            }
            catch (Exception ex)
            {
                Write($"GetCurrentBatteryPower unavailable: {ex.Message}");
                return byte.MaxValue;
            }
        }

        public uint GetAppID()
        {
            uint appId = SteamEmulator.AppID;
            Write($"GetAppID {appId}");
            return appId;
        }

        public void SetOverlayNotificationPosition(int eNotificationPosition)
        {
            if (!Enum.IsDefined(typeof(ENotificationPosition), eNotificationPosition))
            {
                SteamClient.ReportWarning(1, $"Ignored invalid overlay notification position {eNotificationPosition}.");
                return;
            }

            var position = (ENotificationPosition)eNotificationPosition;
            OverlayManager.SetNotificationPosition(position);
            Write($"SetOverlayNotificationPosition {position}");
        }

        public bool IsAPICallCompleted(SteamAPICall_t hSteamAPICall, ref bool pbFailed)
        {
            bool Result = CallbackManager.IsCompleted(hSteamAPICall);
            pbFailed = false;

            if (Result && CallbackManager.GetCallResult(hSteamAPICall, out var callback))
            {
                pbFailed = callback.IOFailure;
            }

            Write($"IsAPICallCompleted (SteamAPICall = {hSteamAPICall}) = {Result}");
            return Result;
        }

        public int GetAPICallFailureReason(SteamAPICall_t hSteamAPICall)
        {
            int result = CallbackManager.GetAPICallFailureReason(hSteamAPICall);
            Write($"GetAPICallFailureReason (SteamAPICall = {hSteamAPICall}) = {(ESteamAPICallFailure)result}");
            return result;
        }

        public bool GetAPICallResult(SteamAPICall_t handle, IntPtr callback, int callback_size, int callback_expected, ref bool failed)
        {
            bool Result = false;
            try
            {
                Result = CallbackManager.GetAPICallResult(handle, callback, callback_size, callback_expected, ref failed);
            }
            catch (Exception ex)
            {
                Write($"GetAPICallResult {ex}");
            }
            Write($"GetAPICallResult (SteamAPICall = {handle}, CallbackExpected = {(CallbackType)callback_expected}) = {Result}");
            return Result;
        }

        public void RunFrame()
        {
            Write("RunFrame");
        }

        public uint GetIPCCallCount()
        {
            Write("GetIPCCallCount");
            // The emulator does not communicate with a local Steam client over IPC.
            // Server HTTP traffic is not Steam IPC and must not be reported as such.
            return 0;
        }

        public void SetWarningMessageHook(IntPtr pFunction)
        {
            SteamEmulator.SteamClient.SetWarningMessageHook(pFunction);
        }

        public bool IsOverlayEnabled()
        {
            Write("IsOverlayEnabled");
            return OverlayManager.IsOverlayEnabled();
        }

        public bool BOverlayNeedsPresent()
        {
            Write("BOverlayNeedsPresent");
            return false;
        }

        public SteamAPICall_t CheckFileSignature(string szFileName)
        {
            Write("CheckFileSignature");
            // CheckFileSignature_t
            return k_uAPICallInvalid;
        }

        public bool ShowGamepadTextInput(int eInputMode, int eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText)
        {
            Write("ShowGamepadTextInput");
            if (unCharMax == 0)
            {
                return false;
            }

            lock (TextInputGate)
            {
                if (GamepadTextInputPending)
                {
                    return false;
                }
                GamepadTextInputPending = true;
                HasEnteredGamepadText = false;
                EnteredGamepadText = string.Empty;
            }

            var shown = OverlayManager.ShowTextInput(
                pchDescription,
                unCharMax,
                pchExistingText,
                multiline: eLineInputMode == 1,
                password: eInputMode == 1,
                completed: (submitted, text) =>
                {
                    uint submittedBytes = 0;
                    lock (TextInputGate)
                    {
                        GamepadTextInputPending = false;
                        HasEnteredGamepadText = submitted;
                        EnteredGamepadText = submitted ? text ?? string.Empty : string.Empty;
                        if (submitted)
                        {
                            submittedBytes = checked((uint)Encoding.UTF8.GetByteCount(EnteredGamepadText) + 1);
                        }
                    }

                    CallbackManager.AddCallback(new GamepadTextInputDismissed_t
                    {
                        Submitted = submitted,
                        SubmittedText = submittedBytes,
                        AppID = SteamEmulator.AppID
                    });
                });

            if (!shown)
            {
                lock (TextInputGate)
                {
                    GamepadTextInputPending = false;
                }
            }
            return shown;
        }

        public uint GetEnteredGamepadTextLength()
        {
            Write("GetEnteredGamepadTextLength");
            lock (TextInputGate)
            {
                return HasEnteredGamepadText
                    ? checked((uint)Encoding.UTF8.GetByteCount(EnteredGamepadText) + 1)
                    : 0;
            }
        }

        public bool GetEnteredGamepadTextInput(IntPtr pchText, uint cchText)
        {
            Write("GetEnteredGamepadTextInput");
            lock (TextInputGate)
            {
                if (!HasEnteredGamepadText || pchText == IntPtr.Zero)
                {
                    return false;
                }

                var required = checked((uint)Encoding.UTF8.GetByteCount(EnteredGamepadText) + 1);
                if (cchText != required || cchText > int.MaxValue)
                {
                    return false;
                }

                NativeStringCache.WriteUtf8Buffer(pchText, checked((int)cchText), EnteredGamepadText);
                return true;
            }
        }

        public bool ShowFloatingGamepadTextInput(
            int keyboardMode,
            int textFieldX,
            int textFieldY,
            int textFieldWidth,
            int textFieldHeight)
        {
            Write("ShowFloatingGamepadTextInput");
            return OverlayManager.ShowFloatingTextInput(
                keyboardMode,
                () => CallbackManager.AddCallback(new FloatingGamepadTextInputDismissed_t()));
        }

        public bool DismissFloatingGamepadTextInput()
        {
            Write("DismissFloatingGamepadTextInput");
            return OverlayManager.DismissFloatingTextInput();
        }

        public string GetSteamUILanguage()
        {
            Write("GetSteamUILanguage");
            return SteamEmulator.Language;
        }

        public bool IsSteamRunningInVR()
        {
            Write("IsSteamRunningInVR");
            return false;
        }

        public void SetOverlayNotificationInset(int nHorizontalInset, int nVerticalInset)
        {
            OverlayManager.SetNotificationInset(nHorizontalInset, nVerticalInset);
            Write(
                $"SetOverlayNotificationInset " +
                $"{Math.Max(0, nHorizontalInset)},{Math.Max(0, nVerticalInset)}");
        }

        public bool IsSteamInBigPictureMode()
        {
            Write("IsSteamInBigPictureMode");
            return false;
        }

        public void StartVRDashboard()
        {
            Write("StartVRDashboard");
        }

        public bool IsVRHeadsetStreamingEnabled()
        {
            Write("IsVRHeadsetStreamingEnabled");
            return false;
        }

        public void SetVRHeadsetStreamingEnabled(bool bEnabled)
        {
            Write("SetVRHeadsetStreamingEnabled");
        }

        public bool IsSteamChinaLauncher()
        {
            Write("IsSteamChinaLauncher");
            return false;
        }

        public bool InitFilterText()
        {
            Write("InitFilterText");
            return false;
        }

        public int FilterText(string pchOutFilteredText, uint nByteSizeOutFilteredText, string pchInputMessage, bool bLegalOnly)
        {
            Write($"FilterText");
            return 0;
        }

        public int FilterText(int eContext, ulong sourceSteamID, string pchInputMessage, string pchOutFilteredText, uint nByteSizeOutFilteredText)
        {
            Write($"FilterText");
            return 0;
        }

        public int GetIPv6ConnectivityState(int eProtocol)
        {
            Write("GetIPv6ConnectivityState");
            return (int)ESteamIPv6ConnectivityState.k_ESteamIPv6ConnectivityState_Unknown;
        }
    }
}
