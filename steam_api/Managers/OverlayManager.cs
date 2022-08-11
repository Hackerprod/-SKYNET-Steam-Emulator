using SKYNET.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SKYNET.Managers
{
    public class OverlayManager
    {
        public static Direct3DVersion version;
        public static void Initialize()
        {
            ThreadPool.QueueUserWorkItem(InitializeInternal);
        }

        private static void InitializeInternal(object state)
        {
            version = Direct3DVersion.Unknown;

            IntPtr d3D9Loaded = IntPtr.Zero;
            IntPtr d3D10Loaded = IntPtr.Zero;
            IntPtr d3D10_1Loaded = IntPtr.Zero;
            IntPtr d3D11Loaded = IntPtr.Zero;
            IntPtr d3D11_1Loaded = IntPtr.Zero;

            int delayTime = 100;
            int retryCount = 0;

            while (d3D9Loaded == IntPtr.Zero && d3D10Loaded == IntPtr.Zero && d3D10_1Loaded == IntPtr.Zero && d3D11Loaded == IntPtr.Zero && d3D11_1Loaded == IntPtr.Zero)
            {
                retryCount++;
                d3D9Loaded = NativeMethods.GetModuleHandle("d3d9.dll");
                d3D10Loaded = NativeMethods.GetModuleHandle("d3d10.dll");
                d3D10_1Loaded = NativeMethods.GetModuleHandle("d3d10_1.dll");
                d3D11Loaded = NativeMethods.GetModuleHandle("d3d11.dll");
                d3D11_1Loaded = NativeMethods.GetModuleHandle("d3d11_1.dll");
                Thread.Sleep(delayTime);

                if (retryCount * delayTime > 5000)
                {
                    Write("Unsupported Direct3D version, or Direct3D DLL not loaded within 5 seconds.");
                    return;
                }
            }

            if (d3D11_1Loaded != IntPtr.Zero)
            {
                Write("Autodetect found Direct3D 11.1");
                version = Direct3DVersion.Direct3D11_1;
                IPCManager.SendDirect3DVersion(version);
            }
            if (d3D11Loaded != IntPtr.Zero)
            {
                Write("Autodetect found Direct3D 11");
                version = Direct3DVersion.Direct3D11;
                IPCManager.SendDirect3DVersion(version);
            }
            if (d3D10_1Loaded != IntPtr.Zero)
            {
                Write("Autodetect found Direct3D 10.1");
                version = Direct3DVersion.Direct3D10_1;
                IPCManager.SendDirect3DVersion(version);
            }
            if (d3D10Loaded != IntPtr.Zero)
            {
                Write("Autodetect found Direct3D 10");
                version = Direct3DVersion.Direct3D10;
                IPCManager.SendDirect3DVersion(version);
            }
            if (d3D9Loaded != IntPtr.Zero)
            {
                Write("Autodetect found Direct3D 9");
                version = Direct3DVersion.Direct3D9;
                IPCManager.SendDirect3DVersion(version);
            }
        }

        private static void Write(object msg)
        {
            SteamEmulator.Write("OverlayManager", msg);
        }

        public enum Direct3DVersion
        {
            Unknown,
            AutoDetect,
            Direct3D9,
            Direct3D10,
            Direct3D10_1,
            Direct3D11,
            Direct3D11_1,
        }

    }
}
