using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SKYNET.Wave.Native
{
    internal delegate void waveOutProc(IntPtr hdrvr,int uMsg,int dwUser,int dwParam1,int dwParam2);

    internal delegate void waveInProc(IntPtr hdrvr,int uMsg,int dwUser,int dwParam1,int dwParam2);

    internal class WavMethods
    {
		[DllImport("winmm.dll")]
		public static extern int waveInAddBuffer(IntPtr hWaveOut,IntPtr lpWaveOutHdr,int uSize);

        [DllImport("winmm.dll")]
		public static extern int waveInClose(IntPtr hWaveOut);

        [DllImport("winmm.dll")]
        public static extern uint waveInGetDevCaps(uint hwo,ref WAVEOUTCAPS pwoc,int cbwoc);

        [DllImport("winmm.dll")]
        public static extern int waveInGetNumDevs();

		[DllImport("winmm.dll")]
		public static extern int waveInOpen(out IntPtr hWaveOut,int uDeviceID,WAVEFORMATEX lpFormat,waveInProc dwCallback,int dwInstance,int dwFlags);

		[DllImport("winmm.dll")]
		public static extern int waveInPrepareHeader(IntPtr hWaveOut,IntPtr lpWaveOutHdr,int uSize);

        [DllImport("winmm.dll")]
		public static extern int waveInReset(IntPtr hWaveOut);

        [DllImport("winmm.dll")]
		public static extern int waveInStart(IntPtr hWaveOut);

        [DllImport("winmm.dll")]
		public static extern int waveInStop(IntPtr hWaveOut);

		[DllImport("winmm.dll")]
		public static extern int waveInUnprepareHeader(IntPtr hWaveOut,IntPtr lpWaveOutHdr,int uSize);

        [DllImport("winmm.dll")]
		public static extern int waveOutClose(IntPtr hWaveOut);
                
        [DllImport("winmm.dll")]
        public static extern uint waveOutGetDevCaps(uint hwo,ref WAVEOUTCAPS pwoc,int cbwoc);

        [DllImport("winmm.dll")]
		public static extern int waveOutGetNumDevs();
        
        [DllImport("winmm.dll")]
		public static extern int waveOutGetPosition(IntPtr hWaveOut,out int lpInfo,int uSize);

        [DllImport("winmm.dll")]
		public static extern int waveOutGetVolume(IntPtr hWaveOut,out int dwVolume);

		[DllImport("winmm.dll")]
		public static extern int waveOutOpen(out IntPtr hWaveOut,int uDeviceID,WAVEFORMATEX lpFormat,waveOutProc dwCallback,int dwInstance,int dwFlags);
        
        [DllImport("winmm.dll")]
		public static extern int waveOutPause(IntPtr hWaveOut);

		[DllImport("winmm.dll")]
		public static extern int waveOutPrepareHeader(IntPtr hWaveOut,IntPtr lpWaveOutHdr,int uSize);

        [DllImport("winmm.dll")]
		public static extern int waveOutReset(IntPtr hWaveOut);

        [DllImport("winmm.dll")]
		public static extern int waveOutRestart(IntPtr hWaveOut);

        [DllImport("winmm.dll")]
		public static extern int waveOutSetVolume(IntPtr hWaveOut,int dwVolume);

		[DllImport("winmm.dll")]
		public static extern int waveOutUnprepareHeader(IntPtr hWaveOut,IntPtr lpWaveOutHdr,int uSize);

		[DllImport("winmm.dll")]
		public static extern int waveOutWrite(IntPtr hWaveOut,IntPtr lpWaveOutHdr,int uSize);

    }

    #region Structures
    [StructLayout(LayoutKind.Sequential)]
    internal struct WAVEOUTCAPS
    {
        public ushort wMid;
        public ushort wPid;
        public uint vDriverVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szPname;
        public uint dwFormats;
        public ushort wChannels;
        public ushort wReserved1;
        public uint dwSupport;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WAVEHDR
    {
        public IntPtr lpData;
        public uint dwBufferLength;
        public uint dwBytesRecorded;
        public IntPtr dwUser;
        public uint dwFlags;
        public uint dwLoops;
        public IntPtr lpNext;
        public uint reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class WAVEFORMATEX
    {
        public ushort wFormatTag;
        public ushort nChannels;
        public uint nSamplesPerSec;
        public uint nAvgBytesPerSec;
        public ushort nBlockAlign;
        public ushort wBitsPerSample;
        public ushort cbSize;

        #region method ToString

        public override string ToString()
        {
            StringBuilder retVal = new StringBuilder();
            retVal.Append("wFormatTag: " + wFormatTag + "\r\n");
            retVal.Append("nChannels: " + nChannels + "\r\n");
            retVal.Append("nSamplesPerSec: " + nSamplesPerSec + "\r\n");
            retVal.Append("nAvgBytesPerSec: " + nAvgBytesPerSec + "\r\n");
            retVal.Append("nBlockAlign: " + nBlockAlign + "\r\n");
            retVal.Append("wBitsPerSample: " + wBitsPerSample + "\r\n");
            retVal.Append("cbSize: " + cbSize + "\r\n");

            return retVal.ToString();
        }

        #endregion
    }
    #endregion

    internal class WavConstants
    {
        public const int MM_WOM_OPEN = 0x3BB;
        public const int MM_WOM_CLOSE = 0x3BC;
        public const int MM_WOM_DONE = 0x3BD;

        public const int MM_WIM_OPEN = 0x3BE;
        public const int MM_WIM_CLOSE = 0x3BF;
        public const int MM_WIM_DATA = 0x3C0;

        public const int CALLBACK_FUNCTION = 0x00030000;

        public const int WAVERR_STILLPLAYING = 0x21;

        public const int WHDR_DONE = 0x00000001;
        public const int WHDR_PREPARED = 0x00000002;
        public const int WHDR_BEGINLOOP = 0x00000004;
        public const int WHDR_ENDLOOP = 0x00000008;
        public const int WHDR_INQUEUE = 0x00000010;
    }

    internal enum MMSYSERR
    {
        NOERROR = 0,
        ERROR = 1,
        BADDEVICEID = 2,
        NOTENABLED = 3,
        ALLOCATED = 4,
        INVALHANDLE = 5,
        NODRIVER = 6,
        NOMEM = 7,
        NOTSUPPORTED = 8,
        BADERRNUM = 9,
        INVALFLAG = 1,
        INVALPARAM = 11,
        HANDLEBUSY = 12,
        INVALIDALIAS = 13,
        BADDB = 14,
        KEYNOTFOUND = 15,
        READERROR = 16,
        WRITEERROR = 17,
        DELETEERROR = 18,
        VALNOTFOUND = 19,
        NODRIVERCB = 20,
        LASTERROR = 20,
    }
}
