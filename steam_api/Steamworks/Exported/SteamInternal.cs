using System;
using System.Runtime.InteropServices;
using SKYNET.Managers;
using SKYNET.Types;

using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;

namespace SKYNET.Steamworks.Exported
{
    public class SteamInternal
    {
        static SteamInternal()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamInternal_SteamAPI_Init()
        {
            Write($"SteamInternal_SteamAPI_Init");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamInternal_GameServer_Init_V2()
        {
            Write($"SteamInternal_GameServer_Init_V2");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamInternal_FindOrCreateUserInterface(int hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_FindOrCreateUserInterface {pszVersion}");
            return InterfaceManager.FindOrCreateInterface(pszVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamInternal_FindOrCreateGameServerInterface(HSteamUser hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_FindOrCreateGameServerInterface {pszVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, 1, pszVersion, true);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamInternal_CreateInterface([MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_CreateInterface {pszVersion}");
            return InterfaceManager.FindOrCreateInterface(pszVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamInternal_GameServer_Init(uint unIP, int usPort, int usGamePort, uint usQueryPort, uint eServerMode, string pchVersionString)
        {
            var unFlags = eServerMode == (int)EServerMode.AuthenticationAndSecure ? Constants.k_unServerFlagSecure : 0;
            var result = SteamEmulator.SteamGameServer.InitGameServer(unIP, usPort, (int)usQueryPort, unFlags, SteamEmulator.AppID, pchVersionString);
            Write($"SteamInternal_GameServer_Init = {result}");
            return result;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamInternal_ContextInit(IntPtr pContextInitData)
        {
            if (pContextInitData == IntPtr.Zero)
            {
                Console.WriteLine("Error: Puntero de inicialización nulo.");
                return IntPtr.Zero;
            }

            try
            {
                // Leer la estructura que contiene los datos de inicialización
                var contextInitData = Marshal.PtrToStructure<SteamContextInitData>(pContextInitData);

                // Validar que la función delegada no sea nula
                if (contextInitData.pFn == IntPtr.Zero)
                {
                    Console.WriteLine("Error: Delegado nulo en pFn.");
                    return IntPtr.Zero;
                }

                // Obtener el puntero del contexto de la API
                IntPtr apiContextPtr = pContextInitData + IntPtr.Size * 2;

                // Leer el contador y verificar si el contexto ya fue inicializado
                int counter = Marshal.ReadInt32(pContextInitData, IntPtr.Size);
                if (counter != 1)
                {
                    // Actualizar el contador e invocar la función de inicialización
                    Marshal.WriteInt32(pContextInitData, IntPtr.Size, 1);
                    var initFn = Marshal.GetDelegateForFunctionPointer<SteamInitFunction>(contextInitData.pFn);
                    initFn(apiContextPtr);
                    Console.WriteLine("Contexto inicializado correctamente.");
                }
                else
                {
                    Console.WriteLine("El contexto ya está inicializado.");
                }

                return apiContextPtr;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al inicializar el contexto: {ex.Message}");
                return IntPtr.Zero;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SteamContextInitData
        {
            public IntPtr pFn; // Puntero a la función de inicialización
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SteamInitFunction(IntPtr contextPtr);

        private static void Write(object msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}

