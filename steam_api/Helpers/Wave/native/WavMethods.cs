using System;
using System.Runtime.InteropServices;

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
}
