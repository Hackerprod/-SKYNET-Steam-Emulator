using SKYNET;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks.Implementation;
using SKYNET.Types;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class SteamEmulator
{
    public static SteamEmulator Instance;

    // Callbacks
    public static CallbackManager Client_Callback = new CallbackManager();
    public static CallbackManager Server_Callback = new CallbackManager();

    public event EventHandler<object> OnMessage;

    #region Client Info

    public static string Language { get; set; }
    public static string PersonaName { get; set; }
    public static ulong SteamId { get; set; }
    public static ulong SteamId_GS { get; set; }
    public static bool AsClient { get; set; }
    public static uint AppId { get; set; }

    public static HSteamUser HSteamUser;
    public static HSteamPipe HSteamPipe;

    public static HSteamUser HSteamUser_GS;
    public static HSteamPipe HSteamPipe_GS;

    public static CSteamApiContext Context;

    #endregion

    public static bool Initialized { get; set; }
    public static string SteamApiPath { get; set; }

    public static Dictionary<HSteamPipe, Steam_Pipe> steam_pipes;

    #region Interfaces 

    //Client
    public static SteamClient SteamClient;
    public static SteamUser SteamUser;
    public static SteamFriends SteamFriends;
    public static SteamUtils SteamUtils;
    public static SteamMatchmaking SteamMatchmaking;
    public static SteamMatchMakingServers SteamMatchMakingServers;
    public static SteamUserStats SteamUserStats;
    public static SteamApps SteamApps;
    public static SteamNetworking SteamNetworking;
    public static SteamRemoteStorage SteamRemoteStorage;
    public static SteamScreenshots SteamScreenshots;
    public static SteamHTTP SteamHTTP;
    public static SteamController SteamController;
    public static SteamUGC SteamUGC;
    public static SteamAppList SteamAppList;
    public static SteamMusic SteamMusic;
    public static SteamMusicRemote SteamMusicRemote;
    public static SteamHTMLSurface SteamHTMLSurface;
    public static SteamInventory SteamInventory;
    public static SteamVideo SteamVideo;
    public static SteamParentalSettings SteamParentalSettings;
    public static SteamNetworkingSockets SteamNetworkingSockets;
    public static SteamNetworkingSocketsSerialized SteamNetworkingSocketsSerialized;
    public static SteamNetworkingMessages SteamNetworkingMessages;
    public static SteamGameCoordinator SteamGameCoordinator;
    public static SteamNetworkingUtils SteamNetworkingUtils;
    public static SteamGameSearch SteamGameSearch;
    public static SteamInput SteamInput;
    public static SteamParties SteamParties;
    public static SteamRemotePlay SteamRemotePlay;
    public static SteamTV SteamTV;

    //GameServer
    public static SteamGameServer SteamGameServer;
    public static SteamUtils SteamGameServerUtils;
    public static SteamGameServerStats SteamGameServerStats;
    public static SteamNetworking SteamGameServerNetworking;
    public static SteamHTTP SteamGameServerHttp;
    public static SteamInventory SteamGameServerInventory;
    public static SteamUGC SteamGameServerUgc;
    public static SteamApps SteamGameServerApps;
    public static SteamNetworkingSockets SteamGameServerNetworkingSockets;
    public static SteamNetworkingSocketsSerialized SteamGameServerNetworkingSocketsSerialized;
    public static SteamNetworkingMessages SteamGameServerNetworkingMessages;
    public static SteamGameCoordinator SteamGameServerGamecoordinator;
    public static SteamMasterServerUpdater SteamMasterServerUpdater;

    #endregion

    public SteamEmulator(bool asClient)
    {
        steam_pipes = new Dictionary<HSteamPipe, Steam_Pipe>();
        Instance = this;
        AsClient = asClient;
    }

    public void Initialize()
    {
        Test(typeof(SteamClient));

        string _file = Path.Combine(modCommon.GetPath(), "[SKYNET] steam_api.ini");

        modCommon.LoadSettings();

        InterfaceManager.Initialize();

        if (Client_Callback == null) Client_Callback = new CallbackManager();
        if (Server_Callback == null) Server_Callback = new CallbackManager();

        steam_pipes = new Dictionary<HSteamPipe, Steam_Pipe>();

        InterfaceManager.Initialize();

        #region Interface Initialization

        // Client Interfaces

        SteamClient = CreateInterface<SteamClient>();

        SteamUser = CreateInterface<SteamUser>();

        SteamFriends = CreateInterface<SteamFriends>();

        SteamUtils = CreateInterface<SteamUtils>();

        SteamMatchmaking = CreateInterface<SteamMatchmaking>();   

        SteamMatchMakingServers = CreateInterface<SteamMatchMakingServers>();    

        SteamUserStats = CreateInterface<SteamUserStats>();  

        SteamApps = CreateInterface<SteamApps>();   

        SteamNetworking = CreateInterface<SteamNetworking>();   

        SteamRemoteStorage = CreateInterface<SteamRemoteStorage>();  

        SteamScreenshots = CreateInterface<SteamScreenshots>();   

        SteamHTTP = CreateInterface<SteamHTTP>();

        SteamController = CreateInterface<SteamController>();

        SteamUGC = CreateInterface<SteamUGC>();

        SteamAppList = CreateInterface<SteamAppList>();

        SteamMusic = CreateInterface<SteamMusic>();

        SteamMusicRemote = CreateInterface<SteamMusicRemote>();

        SteamHTMLSurface = CreateInterface<SteamHTMLSurface>();

        SteamInventory = CreateInterface<SteamInventory>();

        SteamVideo = CreateInterface<SteamVideo>();

        SteamParentalSettings = CreateInterface<SteamParentalSettings>();

        SteamNetworkingSockets = CreateInterface<SteamNetworkingSockets>();

        SteamNetworkingSocketsSerialized = CreateInterface<SteamNetworkingSocketsSerialized>();

        SteamNetworkingMessages = CreateInterface<SteamNetworkingMessages>();

        SteamGameCoordinator = CreateInterface<SteamGameCoordinator>();

        SteamNetworkingUtils = CreateInterface<SteamNetworkingUtils>();

        SteamGameSearch = CreateInterface<SteamGameSearch>();

        SteamParties = CreateInterface<SteamParties>();

        SteamRemotePlay = CreateInterface<SteamRemotePlay>();

        SteamTV = CreateInterface<SteamTV>();

        SteamInput = CreateInterface<SteamInput>();


        // Server Interfaces

        SteamGameServer = CreateInterface<SteamGameServer>();

        SteamGameServerUtils = CreateInterface<SteamUtils>();

        SteamGameServerStats = CreateInterface<SteamGameServerStats>();

        SteamGameServerNetworking = CreateInterface<SteamNetworking>();

        SteamHTTP = CreateInterface<SteamHTTP>();

        SteamGameServerInventory = CreateInterface<SteamInventory>();

        SteamGameServerUgc = CreateInterface<SteamUGC>();

        SteamGameServerApps = CreateInterface<SteamApps>();

        SteamGameServerNetworkingSockets = CreateInterface<SteamNetworkingSockets>();

        SteamGameServerNetworkingSocketsSerialized = CreateInterface<SteamNetworkingSocketsSerialized>();

        SteamGameServerNetworkingMessages = CreateInterface<SteamNetworkingMessages>();

        SteamGameServerGamecoordinator = CreateInterface<SteamGameCoordinator>();

        SteamMasterServerUpdater = CreateInterface<SteamMasterServerUpdater>();

        #endregion

        HSteamUser = (HSteamUser)1;
        HSteamPipe = (HSteamPipe)1;

        HSteamUser_GS = (HSteamUser)1;
        HSteamPipe_GS = (HSteamPipe)1;

        SteamClient.ConnectToGlobalUser((int)HSteamPipe);

        Context = new CSteamApiContext();
        var success = Context.Init();
        //if (success)
        //{
        //    Write("SteamApi Context created successfully");
        //}
        //else
        //{
        //    Write("Error creating SteamApi Context");
        //}

        //SteamClient context = (SteamClient)Interface.Bind<ISteamClient>(SteamClient.MemoryAddress);
        //context.SetPersonaName("Fede");


        Initialized = true;

    }

    private T CreateInterface<T>()  where T : ISteamInterface
    {
        T baseClass = InterfaceManager.CreateInterface<T>(out IntPtr BaseAddress);
        baseClass.MemoryAddress = BaseAddress;
        return (T)baseClass;
    }

    public static HSteamUser CreateSteamUser()
    {
        if (HSteamUser == null)
        {
            HSteamUser = (HSteamUser)1;
            Write($"Creating user {HSteamUser}");
        }
        return HSteamUser;
    }

    public static HSteamPipe CreateSteamPipe()
    {
        if (HSteamPipe == null)
        {
            HSteamPipe = (HSteamPipe)1;
            Write($"Creating pipe {HSteamPipe}");
            steam_pipes[HSteamPipe] = Steam_Pipe.NO_USER;
        }
        return HSteamPipe;
    }

    public static void Write(object sender, object msg)
    {
        Write(sender + ": " + msg);
    }

//#if Debug

    public static void Write(object msg)
    {
        if (AsClient)
        {
            Instance.OnMessage?.Invoke(Instance, msg);
        }
        Log.Write(msg);
    }
    //#else

    //    public static void Write(object msg)
    //    {
    //        // TODO
    //    }

    //#endif

    private void Test(Type type)
    {
        string Name = type.Name;
        Write("Creating dll");
        try
        {

            AssemblyBuilder asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Scenarios"), AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = asmBuilder.DefineDynamicModule("Scenarios", "Scenarios.dll");

            foreach (var methodInfo in InterfaceMethodsForType(type))
            {
                TypeAttributes typeAttr = TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.AnsiClass | TypeAttributes.AutoClass;
                TypeBuilder del = moduleBuilder.DefineType(methodInfo.Name, typeAttr, typeof(MulticastDelegate));

                CustomAttributeBuilder unmanagedPointer = new CustomAttributeBuilder(typeof(UnmanagedFunctionPointerAttribute).GetConstructor(new[] { typeof(CallingConvention) }), new object[] { CallingConvention.ThisCall });
                del.SetCustomAttribute(unmanagedPointer);

                MethodAttributes ctorAttr = MethodAttributes.RTSpecialName | MethodAttributes.Public;
                ConstructorBuilder ctor = del.DefineConstructor(ctorAttr, CallingConventions.Standard, new Type[] { typeof(object), typeof(System.IntPtr) });
                ctor.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

                Type[] parameterTypes = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();

                MethodBuilder invokeMethod = del.DefineMethod("Invoke", methodInfo.Attributes & ~MethodAttributes.Abstract, methodInfo.ReturnType, parameterTypes);
                invokeMethod.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

                //ParameterInfo[] parameters = methodInfo.GetParameters();
                //for (int i = 0; i < parameters.Length; i++)
                //{
                //    ilGenerator.Emit(OpCodes.Ldarg_S, i + 1);
                //}
                //ilGenerator.Emit(OpCodes.Callvirt, delegateType.GetMethod("Invoke"));

                //if (methodInfo.ReturnType == typeof(string))
                //{
                //    ilGenerator.Emit(OpCodes.Call, typeof(Interface).GetMethod("PtrToStringUtf8"));
                //}

                //ilGenerator.Emit(OpCodes.Ret);
                del.CreateType();
            }

            asmBuilder.Save("Scenarios.dll");

            //////////////////////////////////////////////////////////////////////////////////
            ///

            //string delTypeName = "UniqueName";

            //AssemblyBuilder asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Scenarios"), AssemblyBuilderAccess.RunAndSave);
            //ModuleBuilder moduleBuilder = asmBuilder.DefineDynamicModule("Scenarios", "Scenarios.dll");

            //TypeAttributes typeAttr = TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.AnsiClass | TypeAttributes.AutoClass;
            //TypeBuilder del = moduleBuilder.DefineType(delTypeName, typeAttr, typeof(MulticastDelegate));

            //ConstructorInfo ufpa = typeof(UnmanagedFunctionPointerAttribute).GetConstructor(new Type[] { typeof(CallingConvention) });
            //CustomAttributeBuilder unmanagedPointer = new CustomAttributeBuilder(typeof(UnmanagedFunctionPointerAttribute).GetConstructor(new[] { typeof(CallingConvention) }), new object[] { CallingConvention.ThisCall });
            //del.SetCustomAttribute(unmanagedPointer);

            //MethodAttributes ctorAttr = MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public;
            //ConstructorBuilder ctor = del.DefineConstructor(ctorAttr, CallingConventions.Standard, new Type[] { typeof(object), typeof(System.IntPtr) });
            //ctor.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

            //Type[] parameterTypes = new Type[0] { };
            //MethodAttributes methodAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;

            //MethodBuilder invokeMethod = del.DefineMethod("Invoke", methodAttr, typeof(string), parameterTypes);
            //invokeMethod.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

            //del.CreateType();
            //asmBuilder.Save("Scenarios.dll");
        }
        catch (Exception ex)
        {
            Write(ex.Message + " " + ex.StackTrace);
        }


        Write("Dll created");
    }
    public static List<MethodInfo> InterfaceMethodsForType(Type t)
    {
        var all_methods = new List<MethodInfo>(t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
        all_methods.RemoveAll(x => x.Name.StartsWith("get_") || x.Name.StartsWith("set_"));
        return all_methods;
    }

}

public enum Steam_Pipe : int
{
    NO_USER,
    CLIENT,
    SERVER
};

public interface IUserClient
{
    string GetUserName();
    void SetSteamId(long steamId);
}


