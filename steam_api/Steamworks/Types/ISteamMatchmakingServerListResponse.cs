using System;
using System.Runtime.InteropServices;
using System.Text;
using SKYNET.Types;

using HServerListRequest = System.IntPtr;

namespace SKYNET.Steamworks.Types
{
    internal static class NativeGameServerItem
    {
        public const int Size = 372;

        private const int ConnectionPortOffset = 0;
        private const int QueryPortOffset = 2;
        private const int IpOffset = 4;
        private const int PingOffset = 8;
        private const int SuccessfulResponseOffset = 12;
        private const int DoNotRefreshOffset = 13;
        private const int GameDirOffset = 14;
        private const int MapOffset = 46;
        private const int DescriptionOffset = 78;
        private const int AppIdOffset = 144;
        private const int PlayersOffset = 148;
        private const int MaxPlayersOffset = 152;
        private const int BotPlayersOffset = 156;
        private const int PasswordOffset = 160;
        private const int SecureOffset = 161;
        private const int LastPlayedOffset = 164;
        private const int ServerVersionOffset = 168;
        private const int ServerNameOffset = 172;
        private const int TagsOffset = 236;
        private const int SteamIdOffset = 364;

        public static IntPtr Allocate(GameServerData server, int ping)
        {
            var pointer = Marshal.AllocHGlobal(Size);
            Marshal.Copy(new byte[Size], 0, pointer, Size);

            Marshal.WriteInt16(pointer, ConnectionPortOffset, unchecked((short)ClampPort(server.Port)));
            Marshal.WriteInt16(pointer, QueryPortOffset, unchecked((short)ClampPort(server.QueryPort)));
            Marshal.WriteInt32(pointer, IpOffset, unchecked((int)server.IP));
            Marshal.WriteInt32(pointer, PingOffset, Math.Max(0, ping));
            Marshal.WriteByte(pointer, SuccessfulResponseOffset, 1);
            Marshal.WriteByte(pointer, DoNotRefreshOffset, 0);
            WriteUtf8(pointer, GameDirOffset, 32, server.ModDir);
            WriteUtf8(pointer, MapOffset, 32, server.MapName);
            WriteUtf8(pointer, DescriptionOffset, 64, server.Description);
            Marshal.WriteInt32(pointer, AppIdOffset, unchecked((int)server.AppId));
            var playerCount = (server.Players?.Count ?? 0) + Math.Max(0, server.BotPlayers);
            Marshal.WriteInt32(pointer, PlayersOffset, playerCount);
            Marshal.WriteInt32(pointer, MaxPlayersOffset, Math.Max(0, server.MaxPlayers));
            Marshal.WriteInt32(pointer, BotPlayersOffset, Math.Max(0, server.BotPlayers));
            Marshal.WriteByte(pointer, PasswordOffset, server.PasswordProtected ? (byte)1 : (byte)0);
            Marshal.WriteByte(pointer, SecureOffset, server.Secure != 0 ? (byte)1 : (byte)0);
            Marshal.WriteInt32(pointer, LastPlayedOffset, 0);
            Marshal.WriteInt32(pointer, ServerVersionOffset, ParseServerVersion(server.VersionString));
            WriteUtf8(pointer, ServerNameOffset, 64, server.ServerName);
            WriteUtf8(pointer, TagsOffset, 128, server.GameTags);
            Marshal.WriteInt64(pointer, SteamIdOffset, unchecked((long)server.SteamId));
            return pointer;
        }

        private static ushort ClampPort(int port)
        {
            return port <= 0 ? (ushort)0 : (ushort)Math.Min(ushort.MaxValue, port);
        }

        private static int ParseServerVersion(string value)
        {
            if (int.TryParse(value, out var numeric))
            {
                return numeric;
            }

            if (Version.TryParse(value, out var version))
            {
                return Math.Max(0,
                    version.Major * 1000000 +
                    Math.Max(0, version.Minor) * 10000 +
                    Math.Max(0, version.Build) * 100 +
                    Math.Max(0, version.Revision));
            }

            return 0;
        }

        private static void WriteUtf8(IntPtr destination, int offset, int capacity, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value ?? string.Empty);
            var length = Math.Min(bytes.Length, capacity - 1);
            if (length > 0)
            {
                Marshal.Copy(bytes, 0, IntPtr.Add(destination, offset), length);
            }

            Marshal.WriteByte(destination, offset + length, 0);
        }
    }

    internal static class NativeMatchmakingCallbacks
    {
        public static void ServerResponded(IntPtr target, HServerListRequest request, int serverIndex) =>
            Invoke(target, 0, typeof(ListResponseDelegate), typeof(ListResponseThisCallDelegate),
                callback => ((ListResponseDelegate)callback)(target, request, serverIndex),
                callback => ((ListResponseThisCallDelegate)callback)(target, request, serverIndex));

        public static void ServerFailedToRespond(IntPtr target, HServerListRequest request, int serverIndex) =>
            Invoke(target, 1, typeof(ListResponseDelegate), typeof(ListResponseThisCallDelegate),
                callback => ((ListResponseDelegate)callback)(target, request, serverIndex),
                callback => ((ListResponseThisCallDelegate)callback)(target, request, serverIndex));

        public static void RefreshComplete(IntPtr target, HServerListRequest request, EMatchMakingServerResponse response) =>
            Invoke(target, 2, typeof(RefreshCompleteDelegate), typeof(RefreshCompleteThisCallDelegate),
                callback => ((RefreshCompleteDelegate)callback)(target, request, response),
                callback => ((RefreshCompleteThisCallDelegate)callback)(target, request, response));

        public static void PingResponded(IntPtr target, IntPtr server) =>
            Invoke(target, 0, typeof(PingRespondedDelegate), typeof(PingRespondedThisCallDelegate),
                callback => ((PingRespondedDelegate)callback)(target, server),
                callback => ((PingRespondedThisCallDelegate)callback)(target, server));

        public static void PingFailed(IntPtr target) =>
            Invoke(target, 1, typeof(NoArgumentDelegate), typeof(NoArgumentThisCallDelegate),
                callback => ((NoArgumentDelegate)callback)(target),
                callback => ((NoArgumentThisCallDelegate)callback)(target));

        public static void AddPlayer(IntPtr target, string name, int score, float timePlayed)
        {
            var nativeName = StringToUtf8(name);
            try
            {
                Invoke(target, 0, typeof(PlayerDelegate), typeof(PlayerThisCallDelegate),
                    callback => ((PlayerDelegate)callback)(target, nativeName, score, timePlayed),
                    callback => ((PlayerThisCallDelegate)callback)(target, nativeName, score, timePlayed));
            }
            finally
            {
                Marshal.FreeHGlobal(nativeName);
            }
        }

        public static void PlayersFailed(IntPtr target) =>
            Invoke(target, 1, typeof(NoArgumentDelegate), typeof(NoArgumentThisCallDelegate),
                callback => ((NoArgumentDelegate)callback)(target),
                callback => ((NoArgumentThisCallDelegate)callback)(target));

        public static void PlayersComplete(IntPtr target) =>
            Invoke(target, 2, typeof(NoArgumentDelegate), typeof(NoArgumentThisCallDelegate),
                callback => ((NoArgumentDelegate)callback)(target),
                callback => ((NoArgumentThisCallDelegate)callback)(target));

        public static void RuleResponded(IntPtr target, string key, string value)
        {
            var nativeKey = StringToUtf8(key);
            var nativeValue = StringToUtf8(value);
            try
            {
                Invoke(target, 0, typeof(RuleDelegate), typeof(RuleThisCallDelegate),
                    callback => ((RuleDelegate)callback)(target, nativeKey, nativeValue),
                    callback => ((RuleThisCallDelegate)callback)(target, nativeKey, nativeValue));
            }
            finally
            {
                Marshal.FreeHGlobal(nativeKey);
                Marshal.FreeHGlobal(nativeValue);
            }
        }

        public static void RulesFailed(IntPtr target) =>
            Invoke(target, 1, typeof(NoArgumentDelegate), typeof(NoArgumentThisCallDelegate),
                callback => ((NoArgumentDelegate)callback)(target),
                callback => ((NoArgumentThisCallDelegate)callback)(target));

        public static void RulesComplete(IntPtr target) =>
            Invoke(target, 2, typeof(NoArgumentDelegate), typeof(NoArgumentThisCallDelegate),
                callback => ((NoArgumentDelegate)callback)(target),
                callback => ((NoArgumentThisCallDelegate)callback)(target));

        private static void Invoke(
            IntPtr target,
            int slot,
            Type cdeclType,
            Type thisCallType,
            Action<Delegate> invokeCdecl,
            Action<Delegate> invokeThisCall)
        {
            if (target == IntPtr.Zero)
            {
                return;
            }

            var vtable = Marshal.ReadIntPtr(target);
            if (vtable == IntPtr.Zero)
            {
                return;
            }

            var function = Marshal.ReadIntPtr(vtable, slot * IntPtr.Size);
            if (function == IntPtr.Zero)
            {
                return;
            }

            if (IntPtr.Size == 4)
            {
                invokeThisCall(Marshal.GetDelegateForFunctionPointer(function, thisCallType));
            }
            else
            {
                invokeCdecl(Marshal.GetDelegateForFunctionPointer(function, cdeclType));
            }
        }

        private static IntPtr StringToUtf8(string value)
        {
            var bytes = Encoding.UTF8.GetBytes((value ?? string.Empty) + "\0");
            var pointer = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, pointer, bytes.Length);
            return pointer;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ListResponseDelegate(IntPtr self, HServerListRequest request, int serverIndex);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate void ListResponseThisCallDelegate(IntPtr self, HServerListRequest request, int serverIndex);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void RefreshCompleteDelegate(IntPtr self, HServerListRequest request, EMatchMakingServerResponse response);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate void RefreshCompleteThisCallDelegate(IntPtr self, HServerListRequest request, EMatchMakingServerResponse response);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void PingRespondedDelegate(IntPtr self, IntPtr server);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate void PingRespondedThisCallDelegate(IntPtr self, IntPtr server);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void NoArgumentDelegate(IntPtr self);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate void NoArgumentThisCallDelegate(IntPtr self);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void PlayerDelegate(IntPtr self, IntPtr name, int score, float timePlayed);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate void PlayerThisCallDelegate(IntPtr self, IntPtr name, int score, float timePlayed);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void RuleDelegate(IntPtr self, IntPtr key, IntPtr value);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate void RuleThisCallDelegate(IntPtr self, IntPtr key, IntPtr value);
    }
}
