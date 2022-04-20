using System.Runtime.InteropServices;

namespace SKYNET.Helper
{
    internal static class Platform
    {
#if PLATFORM_WIN64
		public const int StructPlatformPackSize = 8;
		public const string LibraryName = "steam_api64";
#elif PLATFORM_WIN32
		public const int StructPlatformPackSize = 8;
		public const string LibraryName = "steam_api";
#else 
        public const int StructPlatformPackSize = 8;
#endif

        public const CallingConvention CC = CallingConvention.Cdecl;
        public const int StructPackSize = 4;
    }
}