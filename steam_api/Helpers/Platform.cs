using System.Runtime.InteropServices;

namespace SKYNET.Helper
{
    internal static class Platform
    {
        public const int StructPlatformPackSize = 8;

        public const CallingConvention CC = CallingConvention.Cdecl;
        public const int StructPackSize = 4;
    }
}