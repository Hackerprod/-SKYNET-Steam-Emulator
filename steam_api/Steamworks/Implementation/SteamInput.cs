using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks.Interfaces;

using InputHandle_t = System.UInt64;
using InputActionSetHandle_t = System.UInt64;
using InputDigitalActionHandle_t = System.UInt64;
using InputAnalogActionHandle_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public sealed class SteamInput : ISteamInterface
    {
        private const int MaxControllers = 16;
        private const int XInputControllerCount = 4;
        private const int XInputType = 3;
        private const int JoystickMoveMode = 6;
        private const ulong HandleNamespace = 0x534B590000000000UL;
        private const ushort XInputGamepadDPadUp = 0x0001;
        private const ushort XInputGamepadDPadDown = 0x0002;
        private const ushort XInputGamepadDPadLeft = 0x0004;
        private const ushort XInputGamepadDPadRight = 0x0008;
        private const ushort XInputGamepadStart = 0x0010;
        private const ushort XInputGamepadBack = 0x0020;
        private const ushort XInputGamepadLeftThumb = 0x0040;
        private const ushort XInputGamepadRightThumb = 0x0080;
        private const ushort XInputGamepadLeftShoulder = 0x0100;
        private const ushort XInputGamepadRightShoulder = 0x0200;
        private const ushort XInputGamepadA = 0x1000;
        private const ushort XInputGamepadB = 0x2000;
        private const ushort XInputGamepadX = 0x4000;
        private const ushort XInputGamepadY = 0x8000;

        private readonly object gate = new object();
        private readonly Dictionary<string, ulong> actionSetHandles =
            new Dictionary<string, ulong>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, ulong> digitalHandles =
            new Dictionary<string, ulong>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, ulong> analogHandles =
            new Dictionary<string, ulong>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<ulong, string> actionSetNames = new Dictionary<ulong, string>();
        private readonly Dictionary<ulong, string> digitalNames = new Dictionary<ulong, string>();
        private readonly Dictionary<ulong, string> analogNames = new Dictionary<ulong, string>();
        private readonly Dictionary<string, List<InputBinding>> bindings =
            new Dictionary<string, List<InputBinding>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<ulong, ulong> activeActionSets = new Dictionary<ulong, ulong>();
        private readonly Dictionary<ulong, HashSet<ulong>> activeLayers = new Dictionary<ulong, HashSet<ulong>>();
        private readonly XInputState[] states = new XInputState[XInputControllerCount];
        private readonly bool[] connected = new bool[XInputControllerCount];
        private readonly AutoResetEvent inputChanged = new AutoResetEvent(false);
        private readonly Dictionary<ActionEventKey, InputDigitalActionData_t> deliveredDigitalActions =
            new Dictionary<ActionEventKey, InputDigitalActionData_t>();
        private readonly Dictionary<ActionEventKey, InputAnalogActionData_t> deliveredAnalogActions =
            new Dictionary<ActionEventKey, InputAnalogActionData_t>();

        private Timer pollingTimer;
        private IntPtr actionEventCallback;
        private bool deviceCallbacksEnabled;
        private string manifestPath;
        private int bindingMajor;
        private int bindingMinor;
        private bool initialized;
        private bool explicitlyRunFrame;
        private bool hasUnreadData;

        public static SteamInput Instance;

        public SteamInput()
        {
            Instance = this;
            InterfaceName = "SteamInput";
            InterfaceVersion = "SteamInput006";
        }

        public bool Init(bool bExplicitlyCallRunFrame)
        {
            return Initialize(bExplicitlyCallRunFrame);
        }

        public bool Init()
        {
            return Initialize(false);
        }

        private bool Initialize(bool explicitRunFrame)
        {
            lock (gate)
            {
                if (initialized)
                {
                    explicitlyRunFrame = explicitRunFrame;
                    return true;
                }

                initialized = true;
                explicitlyRunFrame = explicitRunFrame;
                TryLoadDefaultManifest();
                if (!explicitlyRunFrame)
                {
                    PollControllers();
                    pollingTimer = new Timer(_ => PollControllers(), null, 8, 8);
                }
            }

            Write("Init");
            return true;
        }

        public bool Shutdown()
        {
            Timer timer;
            lock (gate)
            {
                timer = pollingTimer;
                pollingTimer = null;
                initialized = false;
                explicitlyRunFrame = false;
                Array.Clear(connected, 0, connected.Length);
                hasUnreadData = false;
                actionEventCallback = IntPtr.Zero;
                deviceCallbacksEnabled = false;
                deliveredDigitalActions.Clear();
                deliveredAnalogActions.Clear();
            }

            timer?.Dispose();
            inputChanged.Set();
            Write("Shutdown");
            return true;
        }

        public void RunFrame(bool bReservedValue)
        {
            RunFrame();
        }

        public void RunFrame()
        {
            EnsureInitialized();
            PollControllers();
            DispatchActionEvents();
        }

        public bool BNewDataAvailable()
        {
            EnsureInitialized();
            lock (gate)
            {
                var result = hasUnreadData;
                hasUnreadData = false;
                return result;
            }
        }

        public bool BWaitForData(bool bWaitForever, uint unTimeout)
        {
            EnsureInitialized();
            if (BNewDataAvailable())
            {
                return true;
            }

            var timeout = bWaitForever
                ? Timeout.Infinite
                : unTimeout > int.MaxValue ? int.MaxValue : (int)unTimeout;
            if (!inputChanged.WaitOne(timeout))
            {
                return false;
            }

            lock (gate)
            {
                hasUnreadData = false;
            }
            return true;
        }

        public bool SetInputActionManifestFilePath(string pchInputActionManifestAbsolutePath)
        {
            if (string.IsNullOrWhiteSpace(pchInputActionManifestAbsolutePath))
            {
                return false;
            }

            try
            {
                var fullPath = Path.GetFullPath(pchInputActionManifestAbsolutePath);
                if (!File.Exists(fullPath))
                {
                    Write($"SetInputActionManifestFilePath missing '{fullPath}'");
                    return false;
                }

                var manifest = ValveKeyValue.ParseFile(fullPath);
                var parsedBindings = ParseManifest(manifest, fullPath);
                lock (gate)
                {
                    manifestPath = fullPath;
                    actionSetHandles.Clear();
                    digitalHandles.Clear();
                    analogHandles.Clear();
                    actionSetNames.Clear();
                    digitalNames.Clear();
                    analogNames.Clear();
                    bindings.Clear();

                    foreach (var actionSet in parsedBindings.ActionSets)
                    {
                        RegisterHandle(actionSetHandles, actionSetNames, "set", actionSet);
                    }
                    foreach (var digitalAction in parsedBindings.DigitalActions)
                    {
                        RegisterHandle(digitalHandles, digitalNames, "digital", digitalAction);
                    }
                    foreach (var analogAction in parsedBindings.AnalogActions)
                    {
                        RegisterHandle(analogHandles, analogNames, "analog", analogAction);
                    }
                    foreach (var pair in parsedBindings.Bindings)
                    {
                        bindings[pair.Key] = pair.Value;
                    }

                    bindingMajor = parsedBindings.MajorRevision;
                    bindingMinor = parsedBindings.MinorRevision;
                }

                Write($"Loaded Steam Input manifest '{fullPath}' actions={digitalHandles.Count + analogHandles.Count} bindings={parsedBindings.Bindings.Sum(pair => pair.Value.Count)}");
                return true;
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("SteamInput manifest", ex);
                return false;
            }
        }

        public InputActionSetHandle_t GetActionSetHandle(string pszActionSetName)
        {
            EnsureInitialized();
            return FindHandle(actionSetHandles, pszActionSetName);
        }

        public InputDigitalActionHandle_t GetDigitalActionHandle(string pszActionName)
        {
            EnsureInitialized();
            return FindHandle(digitalHandles, pszActionName);
        }

        public InputAnalogActionHandle_t GetAnalogActionHandle(string pszActionName)
        {
            EnsureInitialized();
            return FindHandle(analogHandles, pszActionName);
        }

        public void ActivateActionSet(InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle)
        {
            if (!IsValidController(inputHandle) || !actionSetNames.ContainsKey(actionSetHandle))
            {
                return;
            }

            lock (gate)
            {
                activeActionSets[inputHandle] = actionSetHandle;
            }
        }

        public InputActionSetHandle_t GetCurrentActionSet(InputHandle_t inputHandle)
        {
            lock (gate)
            {
                return activeActionSets.TryGetValue(inputHandle, out var handle) ? handle : 0;
            }
        }

        public void ActivateActionSetLayer(InputHandle_t inputHandle, InputActionSetHandle_t actionSetLayerHandle)
        {
            if (!IsValidController(inputHandle) || !actionSetNames.ContainsKey(actionSetLayerHandle))
            {
                return;
            }

            lock (gate)
            {
                if (!activeLayers.TryGetValue(inputHandle, out var layers))
                {
                    layers = new HashSet<ulong>();
                    activeLayers[inputHandle] = layers;
                }
                layers.Add(actionSetLayerHandle);
            }
        }

        public void DeactivateActionSetLayer(InputHandle_t inputHandle, InputActionSetHandle_t actionSetLayerHandle)
        {
            lock (gate)
            {
                if (activeLayers.TryGetValue(inputHandle, out var layers))
                {
                    layers.Remove(actionSetLayerHandle);
                }
            }
        }

        public void DeactivateAllActionSetLayers(InputHandle_t inputHandle)
        {
            lock (gate)
            {
                activeLayers.Remove(inputHandle);
            }
        }

        public int GetActiveActionSetLayers(InputHandle_t inputHandle, ref InputActionSetHandle_t[] handlesOut)
        {
            var layers = GetLayerSnapshot(inputHandle);
            if (handlesOut != null)
            {
                Array.Copy(layers, handlesOut, Math.Min(layers.Length, handlesOut.Length));
            }
            return layers.Length;
        }

        public int GetActiveActionSetLayers(InputHandle_t inputHandle, IntPtr handlesOut)
        {
            var layers = GetLayerSnapshot(inputHandle);
            WriteUInt64Array(handlesOut, layers, MaxControllers);
            return layers.Length;
        }

        public int GetConnectedControllers(ref InputHandle_t[] handlesOut)
        {
            var handles = GetConnectedControllerSnapshot();
            if (handlesOut != null)
            {
                Array.Copy(handles, handlesOut, Math.Min(handles.Length, handlesOut.Length));
            }
            return handles.Length;
        }

        public int GetConnectedControllers(IntPtr handlesOut)
        {
            var handles = GetConnectedControllerSnapshot();
            WriteUInt64Array(handlesOut, handles, MaxControllers);
            return handles.Length;
        }

        public InputHandle_t GetControllerForGamepadIndex(int nIndex)
        {
            EnsureInitialized();
            if (nIndex < 0 || nIndex >= XInputControllerCount)
            {
                return 0;
            }

            lock (gate)
            {
                return connected[nIndex] ? ControllerHandle(nIndex) : 0;
            }
        }

        public int GetGamepadIndexForController(InputHandle_t inputHandle)
        {
            return TryGetControllerIndex(inputHandle, out var index) ? index : -1;
        }

        public int GetInputTypeForHandle(InputHandle_t inputHandle)
        {
            return IsValidController(inputHandle) ? XInputType : 0;
        }

        public InputDigitalActionData_t GetDigitalActionData(InputHandle_t inputHandle, InputDigitalActionHandle_t digitalActionHandle)
        {
            if (!TryGetControllerState(inputHandle, out var state) ||
                !digitalNames.TryGetValue(digitalActionHandle, out var actionName))
            {
                return default;
            }

            var applicable = GetApplicableBindings(inputHandle, actionName, false);
            if (applicable.Count == 0)
            {
                return default;
            }

            var pressed = applicable.Any(binding => IsPressed(state.Gamepad, binding.Source, binding.Control));
            return new InputDigitalActionData_t
            {
                bActive = 1,
                bState = pressed ? (byte)1 : (byte)0
            };
        }

        public InputAnalogActionData_t GetAnalogActionData(InputHandle_t inputHandle, InputAnalogActionHandle_t analogActionHandle)
        {
            if (!TryGetControllerState(inputHandle, out var state) ||
                !analogNames.TryGetValue(analogActionHandle, out var actionName))
            {
                return default;
            }

            var applicable = GetApplicableBindings(inputHandle, actionName, true);
            if (applicable.Count == 0)
            {
                return default;
            }

            var binding = applicable[0];
            var rightStick = string.Equals(binding.Source, "right_joystick", StringComparison.OrdinalIgnoreCase);
            var x = rightStick ? state.Gamepad.ThumbRX : state.Gamepad.ThumbLX;
            var y = rightStick ? state.Gamepad.ThumbRY : state.Gamepad.ThumbLY;
            return new InputAnalogActionData_t
            {
                eMode = JoystickMoveMode,
                x = NormalizeStick(x),
                y = NormalizeStick(y),
                bActive = 1
            };
        }

        public int GetDigitalActionOrigins(InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputDigitalActionHandle_t digitalActionHandle, ref int[] originsOut)
        {
            var origins = GetOrigins(inputHandle, actionSetHandle, digitalActionHandle, false);
            if (originsOut != null)
            {
                Array.Copy(origins, originsOut, Math.Min(origins.Length, originsOut.Length));
            }
            return origins.Length;
        }

        public int GetDigitalActionOrigins(InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputDigitalActionHandle_t digitalActionHandle, IntPtr originsOut)
        {
            var origins = GetOrigins(inputHandle, actionSetHandle, digitalActionHandle, false);
            WriteInt32Array(originsOut, origins, MaxControllers);
            return origins.Length;
        }

        public int GetAnalogActionOrigins(InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputAnalogActionHandle_t analogActionHandle, ref int[] originsOut)
        {
            var origins = GetOrigins(inputHandle, actionSetHandle, analogActionHandle, true);
            if (originsOut != null)
            {
                Array.Copy(origins, originsOut, Math.Min(origins.Length, originsOut.Length));
            }
            return origins.Length;
        }

        public int GetAnalogActionOrigins(InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputAnalogActionHandle_t analogActionHandle, IntPtr originsOut)
        {
            var origins = GetOrigins(inputHandle, actionSetHandle, analogActionHandle, true);
            WriteInt32Array(originsOut, origins, MaxControllers);
            return origins.Length;
        }

        public string GetStringForActionOrigin(int eOrigin)
        {
            return OriginName(eOrigin);
        }

        public string GetStringForXboxOrigin(int eOrigin)
        {
            return XboxOriginName(eOrigin);
        }

        public string GetStringForAnalogActionName(InputAnalogActionHandle_t eActionHandle)
        {
            return analogNames.TryGetValue(eActionHandle, out var name) ? name : string.Empty;
        }

        public string GetStringForDigitalActionName(InputAnalogActionHandle_t eActionHandle)
        {
            return digitalNames.TryGetValue(eActionHandle, out var name) ? name : string.Empty;
        }

        public int GetActionOriginFromXboxOrigin(InputHandle_t inputHandle, int eOrigin)
        {
            const int xboxOriginCount = 28;
            const int xboxOneOriginBase = 114;
            return IsValidController(inputHandle) && eOrigin >= 0 && eOrigin < xboxOriginCount
                ? xboxOneOriginBase + eOrigin
                : 0;
        }

        public int GetActionOriginFromint(InputHandle_t inputHandle, int eOrigin)
        {
            return GetActionOriginFromXboxOrigin(inputHandle, eOrigin);
        }

        public int TranslateActionOrigin(int eDestinationInputType, int eSourceOrigin)
        {
            return eDestinationInputType == XInputType || eDestinationInputType == 0 ? eSourceOrigin : 0;
        }

        public string GetGlyphForActionOrigin(int eOrigin)
        {
            return string.Empty;
        }

        public IntPtr GetGlyphForActionOrigin_Legacy(int eOrigin)
        {
            return NativeStringCache.ToUtf8Ptr(GetGlyphForActionOrigin(eOrigin));
        }

        public IntPtr GetGlyphForint(int eOrigin)
        {
            return GetGlyphForActionOrigin_Legacy(eOrigin);
        }

        public string GetGlyphPNGForActionOrigin(int eOrigin, int eSize, uint unFlags)
        {
            return string.Empty;
        }

        public string GetGlyphSVGForActionOrigin(int eOrigin, uint unFlags)
        {
            return string.Empty;
        }

        public string GetGlyphForXboxOrigin(int eOrigin)
        {
            int actionOrigin = eOrigin >= 0 && eOrigin < 28 ? 114 + eOrigin : 0;
            return GetGlyphForActionOrigin(actionOrigin);
        }

        public IntPtr GetStringForint(int eOrigin)
        {
            return NativeStringCache.ToUtf8Ptr(GetStringForActionOrigin(eOrigin));
        }

        public bool GetDeviceBindingRevision(InputHandle_t inputHandle, int pMajor, int pMinor)
        {
            return IsValidController(inputHandle) && !string.IsNullOrEmpty(manifestPath);
        }

        public bool GetDeviceBindingRevision(InputHandle_t inputHandle, IntPtr pMajor, IntPtr pMinor)
        {
            if (!IsValidController(inputHandle) || string.IsNullOrEmpty(manifestPath))
            {
                return false;
            }

            if (pMajor != IntPtr.Zero)
            {
                Marshal.WriteInt32(pMajor, bindingMajor);
            }
            if (pMinor != IntPtr.Zero)
            {
                Marshal.WriteInt32(pMinor, bindingMinor);
            }
            return true;
        }

        public InputMotionData_t GetMotionData(InputHandle_t inputHandle)
        {
            return default;
        }

        public ushort GetSessionInputConfigurationSettings()
        {
            return (ushort)SteamInputConfigurationEnableType.Xbox;
        }

        public void TriggerVibration(InputHandle_t inputHandle, short usLeftSpeed, short usRightSpeed)
        {
            TriggerVibration(inputHandle, unchecked((ushort)usLeftSpeed), unchecked((ushort)usRightSpeed));
        }

        public void TriggerVibration(InputHandle_t inputHandle, ushort usLeftSpeed, ushort usRightSpeed)
        {
            if (!TryGetControllerIndex(inputHandle, out var index))
            {
                return;
            }

            TrySetVibration(index, usLeftSpeed, usRightSpeed);
        }

        public void TriggerVibrationExtended(InputHandle_t inputHandle, short usLeftSpeed, short usRightSpeed, short usLeftTriggerSpeed, short usRightTriggerSpeed)
        {
            TriggerVibration(inputHandle, unchecked((ushort)usLeftSpeed), unchecked((ushort)usRightSpeed));
        }

        public void TriggerVibrationExtended(InputHandle_t inputHandle, ushort usLeftSpeed, ushort usRightSpeed, ushort usLeftTriggerSpeed, ushort usRightTriggerSpeed)
        {
            TriggerVibration(inputHandle, usLeftSpeed, usRightSpeed);
        }

        public void TriggerHapticPulse(ulong inputHandle, ESteamControllerPad eTargetPad, short usDurationMicroSec)
        {
            TriggerHapticPulse(inputHandle, eTargetPad, unchecked((ushort)usDurationMicroSec));
        }

        public void TriggerHapticPulse(ulong inputHandle, ESteamControllerPad eTargetPad, ushort usDurationMicroSec)
        {
            var speed = (ushort)Math.Min(ushort.MaxValue, Math.Max(1, (int)usDurationMicroSec) * 32);
            TriggerVibration(inputHandle,
                eTargetPad == ESteamControllerPad.k_ESteamControllerPad_Right ? (ushort)0 : speed,
                eTargetPad == ESteamControllerPad.k_ESteamControllerPad_Left ? (ushort)0 : speed);
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Thread.Sleep(Math.Max(1, usDurationMicroSec / 1000));
                TriggerVibration(inputHandle, 0, 0);
            });
        }

        public void TriggerRepeatedHapticPulse(ulong inputHandle, ESteamControllerPad eTargetPad, short usDurationMicroSec, short usOffMicroSec, short unRepeat, int nFlags)
        {
            TriggerRepeatedHapticPulse(inputHandle, eTargetPad, unchecked((ushort)usDurationMicroSec), unchecked((ushort)usOffMicroSec), unchecked((ushort)unRepeat), unchecked((uint)nFlags));
        }

        public void TriggerRepeatedHapticPulse(ulong inputHandle, ESteamControllerPad eTargetPad, ushort usDurationMicroSec, ushort usOffMicroSec, ushort unRepeat, uint nFlags)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                for (var repeat = 0; repeat < unRepeat; repeat++)
                {
                    TriggerHapticPulse(inputHandle, eTargetPad, usDurationMicroSec);
                    Thread.Sleep(Math.Max(1, (usDurationMicroSec + usOffMicroSec) / 1000));
                }
            });
        }

        public void Legacy_TriggerHapticPulse(InputHandle_t inputHandle, int eTargetPad, ushort usDurationMicroSec)
        {
            TriggerHapticPulse(inputHandle, (ESteamControllerPad)eTargetPad, usDurationMicroSec);
        }

        public void Legacy_TriggerRepeatedHapticPulse(InputHandle_t inputHandle, int eTargetPad, ushort usDurationMicroSec, ushort usOffMicroSec, ushort unRepeat, uint nFlags)
        {
            TriggerRepeatedHapticPulse(inputHandle, (ESteamControllerPad)eTargetPad, usDurationMicroSec, usOffMicroSec, unRepeat, nFlags);
        }

        public void SetLEDColor(InputHandle_t inputHandle, int nColorR, int nColorG, int nColorB, int nFlags)
        {
        }

        public void SetLEDColor(InputHandle_t inputHandle, byte nColorR, byte nColorG, byte nColorB, uint nFlags)
        {
        }

        public void SetDualSenseTriggerEffect(InputHandle_t inputHandle, IntPtr pParam)
        {
        }

        public void TriggerSimpleHapticEvent(InputHandle_t inputHandle, int eHapticLocation, int nIntensity, string nGainDB, int nOtherIntensity, string nOtherGainDB)
        {
            TriggerVibration(inputHandle, (ushort)(Math.Max(0, Math.Min(100, nIntensity)) * 655), (ushort)(Math.Max(0, Math.Min(100, nOtherIntensity)) * 655));
        }

        public void TriggerSimpleHapticEvent(InputHandle_t inputHandle, int eHapticLocation, byte nIntensity, sbyte nGainDB, byte nOtherIntensity, sbyte nOtherGainDB)
        {
            TriggerSimpleHapticEvent(inputHandle, eHapticLocation, nIntensity, null, nOtherIntensity, null);
        }

        public void StopAnalogActionMomentum(InputHandle_t inputHandle, InputAnalogActionHandle_t eAction)
        {
        }

        public void EnableDeviceCallbacks()
        {
            ulong[] handles;
            lock (gate)
            {
                deviceCallbacksEnabled = true;
                handles = Enumerable.Range(0, XInputControllerCount)
                    .Where(index => connected[index])
                    .Select(ControllerHandle)
                    .ToArray();
            }

            foreach (ulong handle in handles)
            {
                CallbackManager.AddCallback(new SKYNET.Callback.SteamInputDeviceConnected_t
                {
                    ConnectedDeviceHandle = handle
                });
            }
        }

        public void EnableActionEventCallbacks(IntPtr callback)
        {
            lock (gate)
            {
                actionEventCallback = callback;
                deliveredDigitalActions.Clear();
                deliveredAnalogActions.Clear();
            }
        }

        public bool ShowBindingPanel(InputHandle_t inputHandle)
        {
            return IsValidController(inputHandle) && !string.IsNullOrEmpty(manifestPath);
        }

        public uint GetRemotePlaySessionID(InputHandle_t inputHandle)
        {
            return 0;
        }

        public IntPtr SteamAPI_SteamInput_v005()
        {
            return InterfaceManager.FindOrCreateInterface("SteamInput005");
        }

        private void EnsureInitialized()
        {
            if (!initialized)
            {
                Init();
            }
        }

        private void TryLoadDefaultManifest()
        {
            var candidates = new[]
            {
                Path.Combine(Environment.CurrentDirectory, "steam_input_manifest.vdf"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "steam_input_manifest.vdf")
            };
            var candidate = candidates.FirstOrDefault(File.Exists);
            if (candidate != null)
            {
                SetInputActionManifestFilePath(candidate);
            }
        }

        private void PollControllers()
        {
            var changed = false;
            var connectedHandles = new List<ulong>();
            var disconnectedHandles = new List<ulong>();
            lock (gate)
            {
                for (var index = 0; index < XInputControllerCount; index++)
                {
                    bool wasConnected = connected[index];
                    var available = TryGetState(index, out var state);
                    if (available != wasConnected ||
                        (available && state.PacketNumber != states[index].PacketNumber))
                    {
                        changed = true;
                    }

                    connected[index] = available;
                    states[index] = state;
                    if (deviceCallbacksEnabled && available != wasConnected)
                    {
                        var changedHandle = ControllerHandle(index);
                        if (available)
                        {
                            connectedHandles.Add(changedHandle);
                        }
                        else
                        {
                            disconnectedHandles.Add(changedHandle);
                        }
                    }
                    if (!available)
                    {
                        var handle = ControllerHandle(index);
                        activeActionSets.Remove(handle);
                        activeLayers.Remove(handle);
                        RemoveDeliveredActions(handle);
                    }
                }

                if (changed)
                {
                    hasUnreadData = true;
                }
            }

            if (changed)
            {
                inputChanged.Set();
            }

            foreach (ulong handle in connectedHandles)
            {
                CallbackManager.AddCallback(new SKYNET.Callback.SteamInputDeviceConnected_t
                {
                    ConnectedDeviceHandle = handle
                });
            }
            foreach (ulong handle in disconnectedHandles)
            {
                CallbackManager.AddCallback(new SKYNET.Callback.SteamInputDeviceDisconnected_t
                {
                    DisconnectedDeviceHandle = handle
                });
            }
        }

        private void DispatchActionEvents()
        {
            IntPtr callback;
            KeyValuePair<ulong, string>[] digitalActions;
            KeyValuePair<ulong, string>[] analogActions;
            lock (gate)
            {
                callback = actionEventCallback;
                if (callback == IntPtr.Zero)
                {
                    return;
                }

                digitalActions = digitalNames.ToArray();
                analogActions = analogNames.ToArray();
            }

            var events = new List<byte[]>();
            foreach (ulong controller in GetConnectedControllerSnapshot())
            {
                foreach (var action in digitalActions)
                {
                    var data = GetDigitalActionData(controller, action.Key);
                    var key = new ActionEventKey(controller, action.Key);
                    bool changed;
                    lock (gate)
                    {
                        changed = !deliveredDigitalActions.TryGetValue(key, out var previous) ||
                                  previous.bState != data.bState ||
                                  previous.bActive != data.bActive;
                        deliveredDigitalActions[key] = data;
                    }
                    if (changed)
                    {
                        events.Add(BuildDigitalActionEvent(controller, action.Key, data));
                    }
                }

                foreach (var action in analogActions)
                {
                    var data = GetAnalogActionData(controller, action.Key);
                    var key = new ActionEventKey(controller, action.Key);
                    bool changed;
                    lock (gate)
                    {
                        changed = !deliveredAnalogActions.TryGetValue(key, out var previous) ||
                                  previous.eMode != data.eMode ||
                                  previous.x != data.x ||
                                  previous.y != data.y ||
                                  previous.bActive != data.bActive;
                        deliveredAnalogActions[key] = data;
                    }
                    if (changed)
                    {
                        events.Add(BuildAnalogActionEvent(controller, action.Key, data));
                    }
                }
            }

            NativeActionEventCallback nativeCallback;
            try
            {
                nativeCallback = Marshal.GetDelegateForFunctionPointer<NativeActionEventCallback>(callback);
            }
            catch (Exception ex)
            {
                Write($"EnableActionEventCallbacks rejected invalid callback: {ex.Message}");
                EnableActionEventCallbacks(IntPtr.Zero);
                return;
            }

            foreach (byte[] actionEvent in events)
            {
                IntPtr nativeEvent = Marshal.AllocHGlobal(actionEvent.Length);
                try
                {
                    Marshal.Copy(actionEvent, 0, nativeEvent, actionEvent.Length);
                    nativeCallback(nativeEvent);
                }
                catch (Exception ex)
                {
                    Write($"Steam Input action callback failed: {ex.Message}");
                    EnableActionEventCallbacks(IntPtr.Zero);
                    return;
                }
                finally
                {
                    Marshal.FreeHGlobal(nativeEvent);
                }
            }
        }

        private void RemoveDeliveredActions(ulong controller)
        {
            foreach (ActionEventKey key in deliveredDigitalActions.Keys.Where(key => key.Controller == controller).ToArray())
            {
                deliveredDigitalActions.Remove(key);
            }
            foreach (ActionEventKey key in deliveredAnalogActions.Keys.Where(key => key.Controller == controller).ToArray())
            {
                deliveredAnalogActions.Remove(key);
            }
        }

        private static byte[] BuildDigitalActionEvent(
            ulong controller,
            ulong action,
            InputDigitalActionData_t data)
        {
            var buffer = new byte[33];
            CopyBytes(buffer, 0, BitConverter.GetBytes(controller));
            CopyBytes(buffer, 8, BitConverter.GetBytes(0));
            CopyBytes(buffer, 12, BitConverter.GetBytes(action));
            buffer[20] = data.bState;
            buffer[21] = data.bActive;
            return buffer;
        }

        private static byte[] BuildAnalogActionEvent(
            ulong controller,
            ulong action,
            InputAnalogActionData_t data)
        {
            var buffer = new byte[33];
            CopyBytes(buffer, 0, BitConverter.GetBytes(controller));
            CopyBytes(buffer, 8, BitConverter.GetBytes(1));
            CopyBytes(buffer, 12, BitConverter.GetBytes(action));
            CopyBytes(buffer, 20, BitConverter.GetBytes(data.eMode));
            CopyBytes(buffer, 24, BitConverter.GetBytes(data.x));
            CopyBytes(buffer, 28, BitConverter.GetBytes(data.y));
            buffer[32] = data.bActive;
            return buffer;
        }

        private static void CopyBytes(byte[] destination, int offset, byte[] source)
        {
            Buffer.BlockCopy(source, 0, destination, offset, source.Length);
        }

        private ulong[] GetConnectedControllerSnapshot()
        {
            EnsureInitialized();
            lock (gate)
            {
                return Enumerable.Range(0, XInputControllerCount)
                    .Where(index => connected[index])
                    .Select(ControllerHandle)
                    .ToArray();
            }
        }

        private ulong[] GetLayerSnapshot(ulong inputHandle)
        {
            lock (gate)
            {
                return activeLayers.TryGetValue(inputHandle, out var layers)
                    ? layers.Take(MaxControllers).ToArray()
                    : Array.Empty<ulong>();
            }
        }

        private bool TryGetControllerState(ulong handle, out XInputState state)
        {
            state = default;
            if (!TryGetControllerIndex(handle, out var index))
            {
                return false;
            }

            lock (gate)
            {
                if (!connected[index])
                {
                    return false;
                }
                state = states[index];
                return true;
            }
        }

        private bool IsValidController(ulong handle)
        {
            return TryGetControllerState(handle, out _);
        }

        private static ulong ControllerHandle(int index)
        {
            return HandleNamespace | ((ulong)(uint)index + 1UL);
        }

        private static bool TryGetControllerIndex(ulong handle, out int index)
        {
            var value = handle & 0xFFFFFFFFUL;
            index = (int)value - 1;
            return (handle & 0xFFFFFF0000000000UL) == HandleNamespace &&
                   index >= 0 &&
                   index < XInputControllerCount;
        }

        private List<InputBinding> GetApplicableBindings(ulong inputHandle, string actionName, bool analog)
        {
            string actionSet = null;
            var layerNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            lock (gate)
            {
                if (activeActionSets.TryGetValue(inputHandle, out var setHandle))
                {
                    actionSetNames.TryGetValue(setHandle, out actionSet);
                }
                if (activeLayers.TryGetValue(inputHandle, out var layers))
                {
                    foreach (var layer in layers)
                    {
                        if (actionSetNames.TryGetValue(layer, out var layerName))
                        {
                            layerNames.Add(layerName);
                        }
                    }
                }
            }

            if (!bindings.TryGetValue(actionName, out var actionBindings))
            {
                return new List<InputBinding>();
            }

            return actionBindings.Where(binding =>
                    binding.Analog == analog &&
                    (string.IsNullOrEmpty(actionSet) ||
                     string.Equals(binding.ActionSet, actionSet, StringComparison.OrdinalIgnoreCase) ||
                     layerNames.Contains(binding.ActionSet)))
                .ToList();
        }

        private int[] GetOrigins(ulong inputHandle, ulong actionSetHandle, ulong actionHandle, bool analog)
        {
            if (!IsValidController(inputHandle) ||
                !actionSetNames.TryGetValue(actionSetHandle, out var actionSet) ||
                !(analog ? analogNames : digitalNames).TryGetValue(actionHandle, out var actionName) ||
                !bindings.TryGetValue(actionName, out var actionBindings))
            {
                return Array.Empty<int>();
            }

            return actionBindings
                .Where(binding => binding.Analog == analog &&
                                  string.Equals(binding.ActionSet, actionSet, StringComparison.OrdinalIgnoreCase))
                .Select(binding => OriginFor(binding.Source, binding.Control))
                .Where(origin => origin != 0)
                .Distinct()
                .Take(MaxControllers)
                .ToArray();
        }

        private static bool IsPressed(XInputGamepad gamepad, string source, string control)
        {
            switch ((control ?? string.Empty).ToLowerInvariant())
            {
                case "button_a": return (gamepad.Buttons & XInputGamepadA) != 0;
                case "button_b": return (gamepad.Buttons & XInputGamepadB) != 0;
                case "button_x": return (gamepad.Buttons & XInputGamepadX) != 0;
                case "button_y": return (gamepad.Buttons & XInputGamepadY) != 0;
                case "button_escape":
                case "button_start": return (gamepad.Buttons & XInputGamepadStart) != 0;
                case "button_back": return (gamepad.Buttons & XInputGamepadBack) != 0;
                case "button_left_bumper": return (gamepad.Buttons & XInputGamepadLeftShoulder) != 0;
                case "button_right_bumper": return (gamepad.Buttons & XInputGamepadRightShoulder) != 0;
                case "button_left_stick": return (gamepad.Buttons & XInputGamepadLeftThumb) != 0;
                case "button_right_stick": return (gamepad.Buttons & XInputGamepadRightThumb) != 0;
                case "dpad_north": return (gamepad.Buttons & XInputGamepadDPadUp) != 0;
                case "dpad_south": return (gamepad.Buttons & XInputGamepadDPadDown) != 0;
                case "dpad_west": return (gamepad.Buttons & XInputGamepadDPadLeft) != 0;
                case "dpad_east": return (gamepad.Buttons & XInputGamepadDPadRight) != 0;
                case "left_trigger": return gamepad.LeftTrigger > 30;
                case "right_trigger": return gamepad.RightTrigger > 30;
                default:
                    if (string.Equals(source, "left_trigger", StringComparison.OrdinalIgnoreCase))
                    {
                        return gamepad.LeftTrigger > 30;
                    }
                    if (string.Equals(source, "right_trigger", StringComparison.OrdinalIgnoreCase))
                    {
                        return gamepad.RightTrigger > 30;
                    }
                    return false;
            }
        }

        private static int OriginFor(string source, string control)
        {
            switch ((control ?? string.Empty).ToLowerInvariant())
            {
                case "button_a": return 114;
                case "button_b": return 115;
                case "button_x": return 116;
                case "button_y": return 117;
                case "button_left_bumper": return 118;
                case "button_right_bumper": return 119;
                case "button_escape":
                case "button_start": return 120;
                case "button_back": return 121;
                case "button_left_stick": return 127;
                case "button_right_stick": return 133;
                case "dpad_north": return 138;
                case "dpad_south": return 139;
                case "dpad_west": return 140;
                case "dpad_east": return 141;
            }

            switch ((source ?? string.Empty).ToLowerInvariant())
            {
                case "left_trigger": return 122;
                case "right_trigger": return 124;
                case "joystick": return 126;
                case "right_joystick": return 132;
                default: return 0;
            }
        }

        private static string OriginName(int origin)
        {
            switch (origin)
            {
                case 114: return "A";
                case 115: return "B";
                case 116: return "X";
                case 117: return "Y";
                case 118: return "Left Bumper";
                case 119: return "Right Bumper";
                case 120: return "Menu";
                case 121: return "View";
                case 122: return "Left Trigger";
                case 124: return "Right Trigger";
                case 126: return "Left Stick";
                case 127: return "Left Stick Click";
                case 132: return "Right Stick";
                case 133: return "Right Stick Click";
                case 138: return "D-Pad Up";
                case 139: return "D-Pad Down";
                case 140: return "D-Pad Left";
                case 141: return "D-Pad Right";
                default: return "None";
            }
        }

        private static string XboxOriginName(int origin)
        {
            switch (origin)
            {
                case 0: return "A";
                case 1: return "B";
                case 2: return "X";
                case 3: return "Y";
                case 4: return "Left Bumper";
                case 5: return "Right Bumper";
                case 6: return "Menu";
                case 7: return "View";
                case 8: return "Left Trigger";
                case 9: return "Left Trigger Click";
                case 10: return "Right Trigger";
                case 11: return "Right Trigger Click";
                case 12: return "Left Stick";
                case 13: return "Left Stick Click";
                case 14: return "Left Stick Up";
                case 15: return "Left Stick Down";
                case 16: return "Left Stick Left";
                case 17: return "Left Stick Right";
                case 18: return "Right Stick";
                case 19: return "Right Stick Click";
                case 20: return "Right Stick Up";
                case 21: return "Right Stick Down";
                case 22: return "Right Stick Left";
                case 23: return "Right Stick Right";
                case 24: return "D-Pad Up";
                case 25: return "D-Pad Down";
                case 26: return "D-Pad Left";
                case 27: return "D-Pad Right";
                default: return "None";
            }
        }

        private static float NormalizeStick(short value)
        {
            return value < 0 ? value / 32768f : value / 32767f;
        }

        private static void RegisterHandle(
            IDictionary<string, ulong> namesToHandles,
            IDictionary<ulong, string> handlesToNames,
            string category,
            string name)
        {
            if (string.IsNullOrWhiteSpace(name) || namesToHandles.ContainsKey(name))
            {
                return;
            }

            var handle = StableHandle(category, name);
            while (handlesToNames.ContainsKey(handle))
            {
                handle++;
            }

            namesToHandles[name] = handle;
            handlesToNames[handle] = name;
        }

        private static ulong FindHandle(IDictionary<string, ulong> handles, string name)
        {
            return !string.IsNullOrWhiteSpace(name) && handles.TryGetValue(name, out var handle) ? handle : 0;
        }

        private static ulong StableHandle(string category, string name)
        {
            const ulong offset = 14695981039346656037UL;
            const ulong prime = 1099511628211UL;
            var hash = offset;
            foreach (var character in (category + ":" + name).ToLowerInvariant())
            {
                hash ^= character;
                hash *= prime;
            }
            return hash == 0 ? 1 : hash;
        }

        private static ParsedManifest ParseManifest(ValveKeyValue root, string manifestFile)
        {
            var parsed = new ParsedManifest();
            var manifest = root.Child("Action Manifest") ?? root.Children.FirstOrDefault(child => child.IsObject) ?? root;
            var actions = manifest.Child("actions");
            if (actions != null)
            {
                ParseActionDefinitions(actions, parsed);
            }

            var layers = manifest.Child("action_layers");
            if (layers != null)
            {
                ParseActionDefinitions(layers, parsed);
            }

            var configuration = manifest.Child("configurations")
                ?.Child("controller_xboxone")
                ?.Children.FirstOrDefault(child => child.IsObject);
            var relativePath = configuration?.GetValue("path");
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return parsed;
            }

            var mappingPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(manifestFile) ?? string.Empty, relativePath));
            if (!File.Exists(mappingPath))
            {
                return parsed;
            }

            ParseControllerMapping(ValveKeyValue.ParseFile(mappingPath), parsed);
            return parsed;
        }

        private static void ParseActionDefinitions(ValveKeyValue actions, ParsedManifest parsed)
        {
            foreach (var actionSet in actions.Children.Where(child => child.IsObject))
            {
                parsed.ActionSets.Add(actionSet.Name);
                var buttons = actionSet.Child("Button");
                if (buttons != null)
                {
                    foreach (var action in buttons.Children)
                    {
                        parsed.DigitalActions.Add(action.Name);
                    }
                }

                var analog = actionSet.Child("StickPadGyro");
                if (analog != null)
                {
                    foreach (var action in analog.Children)
                    {
                        parsed.AnalogActions.Add(action.Name);
                    }
                }
            }
        }

        private static void ParseControllerMapping(ValveKeyValue root, ParsedManifest parsed)
        {
            var mapping = root.Child("controller_mappings") ?? root.Children.FirstOrDefault(child => child.IsObject) ?? root;
            int.TryParse(mapping.GetValue("major_revision"), out parsed.MajorRevision);
            int.TryParse(mapping.GetValue("minor_revision"), out parsed.MinorRevision);

            var groups = new Dictionary<string, ValveKeyValue>(StringComparer.OrdinalIgnoreCase);
            foreach (var group in mapping.ChildrenNamed("group"))
            {
                var id = group.GetValue("id");
                if (!string.IsNullOrWhiteSpace(id))
                {
                    groups[id] = group;
                }
            }

            foreach (var preset in mapping.ChildrenNamed("preset"))
            {
                var presetName = preset.GetValue("name");
                if (string.IsNullOrWhiteSpace(presetName))
                {
                    continue;
                }

                var groupSources = preset.Child("group_source_bindings");
                if (groupSources == null)
                {
                    continue;
                }

                foreach (var sourceEntry in groupSources.Children.Where(child => !child.IsObject))
                {
                    if (!groups.TryGetValue(sourceEntry.Name, out var group) ||
                        string.IsNullOrWhiteSpace(sourceEntry.Value) ||
                        sourceEntry.Value.IndexOf("active", StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        continue;
                    }

                    var source = sourceEntry.Value.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[0];
                    ParseGroupBindings(group, presetName, source, parsed);
                }
            }
        }

        private static void ParseGroupBindings(ValveKeyValue group, string presetName, string source, ParsedManifest parsed)
        {
            var inputs = group.Child("inputs");
            if (inputs != null)
            {
                foreach (var input in inputs.Children.Where(child => child.IsObject))
                {
                    foreach (var binding in input.Descendants().Where(node =>
                                 !node.IsObject &&
                                 string.Equals(node.Name, "binding", StringComparison.OrdinalIgnoreCase)))
                    {
                        ParseBindingValue(binding.Value, presetName, source, input.Name, parsed);
                    }
                }
            }

            var gameActions = group.Child("gameactions");
            if (gameActions == null)
            {
                return;
            }

            foreach (var action in gameActions.Children.Where(child => !child.IsObject))
            {
                var actionSet = action.Name;
                var actionName = (action.Value ?? string.Empty).Split(',')[0].Trim();
                if (string.IsNullOrWhiteSpace(actionName))
                {
                    continue;
                }

                parsed.AnalogActions.Add(actionName);
                AddBinding(parsed, actionName, new InputBinding
                {
                    ActionSet = string.IsNullOrWhiteSpace(actionSet) ? presetName : actionSet,
                    Source = source,
                    Control = source,
                    Analog = true
                });
            }
        }

        private static void ParseBindingValue(string value, string presetName, string source, string control, ParsedManifest parsed)
        {
            var command = (value ?? string.Empty).Split(',')[0].Trim();
            var parts = command.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3 || !string.Equals(parts[0], "game_action", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var actionSet = parts[1];
            var actionName = parts[2];
            parsed.DigitalActions.Add(actionName);
            AddBinding(parsed, actionName, new InputBinding
            {
                ActionSet = string.IsNullOrWhiteSpace(actionSet) ? presetName : actionSet,
                Source = source,
                Control = control,
                Analog = false
            });
        }

        private static void AddBinding(ParsedManifest parsed, string actionName, InputBinding binding)
        {
            if (!parsed.Bindings.TryGetValue(actionName, out var actionBindings))
            {
                actionBindings = new List<InputBinding>();
                parsed.Bindings[actionName] = actionBindings;
            }

            if (!actionBindings.Any(current =>
                    current.Analog == binding.Analog &&
                    string.Equals(current.ActionSet, binding.ActionSet, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(current.Source, binding.Source, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(current.Control, binding.Control, StringComparison.OrdinalIgnoreCase)))
            {
                actionBindings.Add(binding);
            }
        }

        private static void WriteUInt64Array(IntPtr destination, IReadOnlyList<ulong> values, int maximum)
        {
            if (destination == IntPtr.Zero)
            {
                return;
            }

            for (var index = 0; index < Math.Min(maximum, values.Count); index++)
            {
                Marshal.WriteInt64(destination, index * sizeof(long), unchecked((long)values[index]));
            }
        }

        private static void WriteInt32Array(IntPtr destination, IReadOnlyList<int> values, int maximum)
        {
            if (destination == IntPtr.Zero)
            {
                return;
            }

            for (var index = 0; index < Math.Min(maximum, values.Count); index++)
            {
                Marshal.WriteInt32(destination, index * sizeof(int), values[index]);
            }
        }

        private static bool TryGetState(int index, out XInputState state)
        {
            state = default;
            try
            {
                return XInputGetState((uint)index, out state) == 0;
            }
            catch (DllNotFoundException)
            {
                return false;
            }
            catch (EntryPointNotFoundException)
            {
                return false;
            }
        }

        private static void TrySetVibration(int index, ushort left, ushort right)
        {
            try
            {
                var vibration = new XInputVibration
                {
                    LeftMotorSpeed = left,
                    RightMotorSpeed = right
                };
                XInputSetState((uint)index, ref vibration);
            }
            catch (DllNotFoundException)
            {
            }
            catch (EntryPointNotFoundException)
            {
            }
        }

        [DllImport("xinput1_4.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern uint XInputGetState(uint dwUserIndex, out XInputState pState);

        [DllImport("xinput1_4.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern uint XInputSetState(uint dwUserIndex, ref XInputVibration pVibration);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void NativeActionEventCallback(IntPtr actionEvent);

        private readonly struct ActionEventKey : IEquatable<ActionEventKey>
        {
            public ActionEventKey(ulong controller, ulong action)
            {
                Controller = controller;
                Action = action;
            }

            public ulong Controller { get; }
            public ulong Action { get; }

            public bool Equals(ActionEventKey other)
            {
                return Controller == other.Controller && Action == other.Action;
            }

            public override bool Equals(object obj)
            {
                return obj is ActionEventKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Controller.GetHashCode() * 397) ^ Action.GetHashCode();
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct XInputState
        {
            public uint PacketNumber;
            public XInputGamepad Gamepad;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct XInputGamepad
        {
            public ushort Buttons;
            public byte LeftTrigger;
            public byte RightTrigger;
            public short ThumbLX;
            public short ThumbLY;
            public short ThumbRX;
            public short ThumbRY;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct XInputVibration
        {
            public ushort LeftMotorSpeed;
            public ushort RightMotorSpeed;
        }

        private sealed class InputBinding
        {
            public string ActionSet;
            public string Source;
            public string Control;
            public bool Analog;
        }

        private sealed class ParsedManifest
        {
            public readonly HashSet<string> ActionSets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            public readonly HashSet<string> DigitalActions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            public readonly HashSet<string> AnalogActions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            public readonly Dictionary<string, List<InputBinding>> Bindings =
                new Dictionary<string, List<InputBinding>>(StringComparer.OrdinalIgnoreCase);
            public int MajorRevision;
            public int MinorRevision;
        }
    }
}
