using System;
using System.Runtime.InteropServices;

namespace SKYNET.Helpers
{
    internal static class Platform
    {

#if PLATFORM_WIN64
		public const int StructPlatformPackSize = 8;
#elif PLATFORM_WIN32
		public const int StructPlatformPackSize = 8;
#else
		public const int StructPlatformPackSize = 8;
#endif

        public const int StructPackSize = 4;
    }
}