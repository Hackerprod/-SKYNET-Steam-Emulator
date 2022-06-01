using Microsoft.Win32;
using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Manager;
using SKYNET.Managers;
using SKYNET.Types;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SKYNET.Hook.Handles
{
    using HSteamPipe = System.UInt32;
    using HSteamUser = System.UInt32;
    using SteamAPICall_t = System.UInt64;

    public partial class SteamAPI : BaseHook
    {
        public override bool Installed { get; set; }
        public override void Install()
        {
            try
            {
                MethodInfo methodInfo = GetMethodInfo("SteamAPI_Init");

                Write(methodInfo == null);

                //var del = (Func<int>)Delegate.CreateDelegate(typeof(Func<int>), this, methodInfo);

                //Type DelegateType = CreateDelegate(methodInfo);
                //Write(DelegateType == null);
            }
            catch (Exception ex)
            {
                Write(ex);
            }






            //return;
            // SteamApi Handles
            base.Install<SteamAPI_InitDelegate>("SteamAPI_Init", _SteamAPI_InitDelegate, new SteamAPI_InitDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_Init));
            base.Install<SteamAPI_RunCallbacksDelegate>("SteamAPI_RunCallbacks", _SteamAPI_RunCallbacksDelegate, new SteamAPI_RunCallbacksDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_RunCallbacks));
            base.Install<SteamAPI_RegisterCallResultDelegate>("SteamAPI_RegisterCallResult", _SteamAPI_RegisterCallResultDelegate, new SteamAPI_RegisterCallResultDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_RegisterCallResult));
            base.Install<SteamAPI_ShutdownDelegate>("SteamAPI_Shutdown", _SteamAPI_ShutdownDelegate, new SteamAPI_ShutdownDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_Shutdown));
            base.Install<SteamAPI_UnregisterCallbackDelegate>("SteamAPI_UnregisterCallback", _SteamAPI_UnregisterCallbackDelegate, new SteamAPI_UnregisterCallbackDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_UnregisterCallback));
            base.Install<SteamAPI_UnregisterCallResultDelegate>("SteamAPI_UnregisterCallResult", _SteamAPI_UnregisterCallResultDelegate, new SteamAPI_UnregisterCallResultDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_UnregisterCallResult));
            base.Install<SteamAPI_RegisterCallbackDelegate>("SteamAPI_RegisterCallback", _SteamAPI_RegisterCallbackDelegate, new SteamAPI_RegisterCallbackDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_RegisterCallback));
            base.Install<SteamAPI_InitSafeDelegate>("SteamAPI_InitSafe", _SteamAPI_InitSafeDelegate, new SteamAPI_InitSafeDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_InitSafe));
            base.Install<SteamAPI_InitAnonymousUserDelegate>("SteamAPI_InitAnonymousUser", _SteamAPI_InitAnonymousUserDelegate, new SteamAPI_InitAnonymousUserDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_InitAnonymousUser));
            base.Install<SteamAPI_IsSteamRunningDelegate>("SteamAPI_IsSteamRunning", _SteamAPI_IsSteamRunningDelegate, new SteamAPI_IsSteamRunningDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_IsSteamRunning));
            base.Install<SteamAPI_RestartAppIfNecessaryDelegate>("SteamAPI_RestartAppIfNecessary", _SteamAPI_RestartAppIfNecessaryDelegate, new SteamAPI_RestartAppIfNecessaryDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_RestartAppIfNecessary));
            base.Install<SteamAPI_GetSteamInstallPathDelegate>("SteamAPI_GetSteamInstallPath", _SteamAPI_GetSteamInstallPathDelegate, new SteamAPI_GetSteamInstallPathDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_GetSteamInstallPath));
            base.Install<SteamAPI_GetHSteamUserDelegate>("SteamAPI_GetHSteamUser", _SteamAPI_GetHSteamUserDelegate, new SteamAPI_GetHSteamUserDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_GetHSteamUser));
            base.Install<SteamAPI_GetHSteamPipeDelegate>("SteamAPI_GetHSteamPipe", _SteamAPI_GetHSteamPipeDelegate, new SteamAPI_GetHSteamPipeDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_GetHSteamPipe));
            base.Install<GetHSteamPipeDelegate>("GetHSteamPipe", _GetHSteamPipeDelegate, new GetHSteamPipeDelegate(SKYNET.Steamworks.Exported.SteamAPI.GetHSteamPipe));
            base.Install<GetHSteamUserDelegate>("GetHSteamUser", _GetHSteamUserDelegate, new GetHSteamUserDelegate(SKYNET.Steamworks.Exported.SteamAPI.GetHSteamUser));
            base.Install<SteamAPI_SetTryCatchCallbacksDelegate>("SteamAPI_SetTryCatchCallbacks", _SteamAPI_SetTryCatchCallbacksDelegate, new SteamAPI_SetTryCatchCallbacksDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SetTryCatchCallbacks));
            base.Install<SteamAPI_SetBreakpadAppIDDelegate>("SteamAPI_SetBreakpadAppID", _SteamAPI_SetBreakpadAppIDDelegate, new SteamAPI_SetBreakpadAppIDDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SetBreakpadAppID));
            base.Install<SteamAPI_UseBreakpadCrashHandlerDelegate>("SteamAPI_UseBreakpadCrashHandler", _SteamAPI_UseBreakpadCrashHandlerDelegate, new SteamAPI_UseBreakpadCrashHandlerDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_UseBreakpadCrashHandler));
            base.Install<SteamAPI_ManualDispatch_RunFrameDelegate>("SteamAPI_ManualDispatch_RunFrame", _SteamAPI_ManualDispatch_RunFrameDelegate, new SteamAPI_ManualDispatch_RunFrameDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_ManualDispatch_RunFrame));
            base.Install<SteamAPI_ManualDispatch_GetNextCallbackDelegate>("SteamAPI_ManualDispatch_GetNextCallback", _SteamAPI_ManualDispatch_GetNextCallbackDelegate, new SteamAPI_ManualDispatch_GetNextCallbackDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_ManualDispatch_GetNextCallback));
            base.Install<SteamAPI_ManualDispatch_FreeLastCallbackDelegate>("SteamAPI_ManualDispatch_FreeLastCallback", _SteamAPI_ManualDispatch_FreeLastCallbackDelegate, new SteamAPI_ManualDispatch_FreeLastCallbackDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_ManualDispatch_FreeLastCallback));
            base.Install<SteamAPI_ManualDispatch_GetAPICallResultDelegate>("SteamAPI_ManualDispatch_GetAPICallResult", _SteamAPI_ManualDispatch_GetAPICallResultDelegate, new SteamAPI_ManualDispatch_GetAPICallResultDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_ManualDispatch_GetAPICallResult));
            base.Install<SteamAPI_SetMiniDumpCommentDelegate>("SteamAPI_SetMiniDumpComment", _SteamAPI_SetMiniDumpCommentDelegate, new SteamAPI_SetMiniDumpCommentDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SetMiniDumpComment));
            base.Install<SteamAPI_WriteMiniDumpDelegate>("SteamAPI_WriteMiniDump", _SteamAPI_WriteMiniDumpDelegate, new SteamAPI_WriteMiniDumpDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_WriteMiniDump));
            base.Install<SteamAPI_ReleaseCurrentThreadMemoryDelegate>("SteamAPI_ReleaseCurrentThreadMemory", _SteamAPI_ReleaseCurrentThreadMemoryDelegate, new SteamAPI_ReleaseCurrentThreadMemoryDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_ReleaseCurrentThreadMemory));
            //base.Install<SteamAPI_gameserveritem_t_ConstructDelegate>("SteamAPI_gameserveritem_t_Construct", _SteamAPI_gameserveritem_t_ConstructDelegate, new SteamAPI_gameserveritem_t_ConstructDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_gameserveritem_t_Construct));
            //base.Install<SteamAPI_gameserveritem_t_GetNameDelegate>("SteamAPI_gameserveritem_t_GetName", _SteamAPI_gameserveritem_t_GetNameDelegate, new SteamAPI_gameserveritem_t_GetNameDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_gameserveritem_t_GetName));
            //base.Install<SteamAPI_gameserveritem_t_SetNameDelegate>("SteamAPI_gameserveritem_t_SetName", _SteamAPI_gameserveritem_t_SetNameDelegate, new SteamAPI_gameserveritem_t_SetNameDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_gameserveritem_t_SetName));
            base.Install<g_pSteamClientGameServerDelegate>("g_pSteamClientGameServer", _g_pSteamClientGameServerDelegate, new g_pSteamClientGameServerDelegate(SKYNET.Steamworks.Exported.SteamAPI.g_pSteamClientGameServer));
            base.Install<SteamAPI_SteamAppList_v001Delegate>("SteamAPI_SteamAppList_v001", _SteamAPI_SteamAppList_v001Delegate, new SteamAPI_SteamAppList_v001Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamAppList_v001));
            base.Install<SteamAPI_SteamApps_v008Delegate>("SteamAPI_SteamApps_v008", _SteamAPI_SteamApps_v008Delegate, new SteamAPI_SteamApps_v008Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamApps_v008));
            base.Install<SteamAPI_SteamController_v008Delegate>("SteamAPI_SteamController_v008", _SteamAPI_SteamController_v008Delegate, new SteamAPI_SteamController_v008Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamController_v008));
            base.Install<SteamAPI_SteamFriends_v017Delegate>("SteamAPI_SteamFriends_v017", _SteamAPI_SteamFriends_v017Delegate, new SteamAPI_SteamFriends_v017Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamFriends_v017));
            base.Install<SteamAPI_SteamUtils_v010Delegate>("SteamAPI_SteamUtils_v010", _SteamAPI_SteamUtils_v010Delegate, new SteamAPI_SteamUtils_v010Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamUtils_v010));
            base.Install<SteamAPI_SteamGameServerUtils_v010Delegate>("SteamAPI_SteamGameServerUtils_v010", _SteamAPI_SteamGameServerUtils_v010Delegate, new SteamAPI_SteamGameServerUtils_v010Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamGameServerUtils_v010));
            base.Install<SteamAPI_SteamMatchmaking_v009Delegate>("SteamAPI_SteamMatchmaking_v009", _SteamAPI_SteamMatchmaking_v009Delegate, new SteamAPI_SteamMatchmaking_v009Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamMatchmaking_v009));
            base.Install<SteamAPI_SteamMatchmakingServers_v002Delegate>("SteamAPI_SteamMatchmakingServers_v002", _SteamAPI_SteamMatchmakingServers_v002Delegate, new SteamAPI_SteamMatchmakingServers_v002Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamMatchmakingServers_v002));
            base.Install<SteamAPI_SteamGameSearch_v001Delegate>("SteamAPI_SteamGameSearch_v001", _SteamAPI_SteamGameSearch_v001Delegate, new SteamAPI_SteamGameSearch_v001Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamGameSearch_v001));
            base.Install<SteamAPI_SteamParties_v002Delegate>("SteamAPI_SteamParties_v002", _SteamAPI_SteamParties_v002Delegate, new SteamAPI_SteamParties_v002Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamParties_v002));
            base.Install<SteamAPI_SteamNetworking_v006Delegate>("SteamAPI_SteamNetworking_v006", _SteamAPI_SteamNetworking_v006Delegate, new SteamAPI_SteamNetworking_v006Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamNetworking_v006));
            base.Install<SteamAPI_SteamGameServerNetworking_v006Delegate>("SteamAPI_SteamGameServerNetworking_v006", _SteamAPI_SteamGameServerNetworking_v006Delegate, new SteamAPI_SteamGameServerNetworking_v006Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamGameServerNetworking_v006));
            //base.Install<SteamAPI_SteamScreenshots_v003Delegate>("SteamAPI_SteamScreenshots_v003", _SteamAPI_SteamScreenshots_v003Delegate, new SteamAPI_SteamScreenshots_v003Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamScreenshots_v003));
            //base.Install<SteamAPI_SteamMusic_v001Delegate>("SteamAPI_SteamMusic_v001", _SteamAPI_SteamMusic_v001Delegate, new SteamAPI_SteamMusic_v001Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamMusic_v001));
            base.Install<SteamAPI_SteamMusicRemote_v001Delegate>("SteamAPI_SteamMusicRemote_v001", _SteamAPI_SteamMusicRemote_v001Delegate, new SteamAPI_SteamMusicRemote_v001Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamMusicRemote_v001));
            //base.Install<SteamAPI_SteamHTTP_v003Delegate>("SteamAPI_SteamHTTP_v003", _SteamAPI_SteamHTTP_v003Delegate, new SteamAPI_SteamHTTP_v003Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamHTTP_v003));
            //base.Install<SteamAPI_SteamGameServerHTTP_v003Delegate>("SteamAPI_SteamGameServerHTTP_v003", _SteamAPI_SteamGameServerHTTP_v003Delegate, new SteamAPI_SteamGameServerHTTP_v003Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamGameServerHTTP_v003));
            base.Install<SteamAPI_SteamHTMLSurface_v005Delegate>("SteamAPI_SteamHTMLSurface_v005", _SteamAPI_SteamHTMLSurface_v005Delegate, new SteamAPI_SteamHTMLSurface_v005Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamHTMLSurface_v005));
            base.Install<SteamAPI_SteamInventory_v003Delegate>("SteamAPI_SteamInventory_v003", _SteamAPI_SteamInventory_v003Delegate, new SteamAPI_SteamInventory_v003Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamInventory_v003));
            base.Install<SteamAPI_SteamGameServerInventory_v003Delegate>("SteamAPI_SteamGameServerInventory_v003", _SteamAPI_SteamGameServerInventory_v003Delegate, new SteamAPI_SteamGameServerInventory_v003Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamGameServerInventory_v003));
            base.Install<SteamAPI_SteamVideo_v002Delegate>("SteamAPI_SteamVideo_v002", _SteamAPI_SteamVideo_v002Delegate, new SteamAPI_SteamVideo_v002Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamVideo_v002));
            base.Install<SteamAPI_SteamParentalSettings_v001Delegate>("SteamAPI_SteamParentalSettings_v001", _SteamAPI_SteamParentalSettings_v001Delegate, new SteamAPI_SteamParentalSettings_v001Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamParentalSettings_v001));
            base.Install<SteamAPI_SteamRemotePlay_v001Delegate>("SteamAPI_SteamRemotePlay_v001", _SteamAPI_SteamRemotePlay_v001Delegate, new SteamAPI_SteamRemotePlay_v001Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamRemotePlay_v001));
            base.Install<SteamAPI_SteamNetworkingMessages_SteamAPI_v002Delegate>("SteamAPI_SteamNetworkingMessages_SteamAPI_v002", _SteamAPI_SteamNetworkingMessages_SteamAPI_v002Delegate, new SteamAPI_SteamNetworkingMessages_SteamAPI_v002Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamNetworkingMessages_SteamAPI_v002));
            base.Install<SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002Delegate>("SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002", _SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002Delegate, new SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002));
            base.Install<SteamAPI_SteamGameServerStats_v001Delegate>("SteamAPI_SteamGameServerStats_v001", _SteamAPI_SteamGameServerStats_v001Delegate, new SteamAPI_SteamGameServerStats_v001Delegate(SKYNET.Steamworks.Exported.SteamAPI.SteamAPI_SteamGameServerStats_v001));
            base.Install<SteamClientDelegate>("SteamClient", _SteamClientDelegate, new SteamClientDelegate(SKYNET.Steamworks.Exported.SteamAPI.SteamClient));

            Installed = true;
        }

        public static Type CreateDelegate(MethodInfo methodInfo)
        {
            string Name = methodInfo.Name;
            AssemblyBuilder asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(Name), AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = asmBuilder.DefineDynamicModule(Name, Name + ".dll");

            TypeBuilder del = moduleBuilder.DefineType(methodInfo.Name, TypeAttributes.Class | TypeAttributes.Sealed, typeof(MulticastDelegate));

            CustomAttributeBuilder unmanagedPointer = new CustomAttributeBuilder(typeof(UnmanagedFunctionPointerAttribute).GetConstructor(new[] { typeof(CallingConvention) }), new object[] { CallingConvention.ThisCall });
            del.SetCustomAttribute(unmanagedPointer);

            MethodAttributes ctorAttr = MethodAttributes.RTSpecialName | MethodAttributes.Public | MethodAttributes.Static;
            ConstructorBuilder ctor = del.DefineConstructor(ctorAttr, CallingConventions.Standard, new Type[] { typeof(object), typeof(System.IntPtr) });
            ctor.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

            Type[] parameterTypes = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();

            MethodBuilder invokeMethod = del.DefineMethod("Invoke", methodInfo.Attributes & ~MethodAttributes.Abstract, methodInfo.ReturnType, parameterTypes);
            invokeMethod.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);
            return del.CreateType();
        }

        public MethodInfo GetMethodInfo(string name)
        {
            return typeof(Steamworks.Exported.SteamAPI).GetMethod("SteamAPI_Init"); ;
        }

        public bool SteamAPI_Init()
        {
            Write($"SteamAPI_Init");
            return true;
        }

        public override void Write(object v)
        {
            Main.Write("SteamApi", v);
        }
    }
}
