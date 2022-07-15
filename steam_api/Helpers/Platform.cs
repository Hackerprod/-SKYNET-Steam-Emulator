using System;
using System.Runtime.InteropServices;

namespace SKYNET.Helpers
{
    internal static class Platform
    {
        public const int StructPlatformPackSize = 8;

        public const CallingConvention CC = CallingConvention.Cdecl;
        public const int StructPackSize = 4;

        public static DateTime LoadTime { get; set; } = DateTime.Now;
        public static int MilisecondTime()
        {
            return (DateTime.Now - LoadTime).Milliseconds;
        }
    }
}