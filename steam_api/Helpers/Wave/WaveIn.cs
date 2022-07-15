using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

using SKYNET.Wave.Native;

namespace SKYNET.Wave
{
    public delegate void BufferFullHandler(byte[] buffer);

    public class WaveIn
    {

        private class BufferItem
        {
            private GCHandle m_HeaderHandle;
            private GCHandle m_DataHandle;
            private int      m_DataSize = 0;

            public BufferItem(ref GCHandle headerHandle,ref GCHandle dataHandle,int dataSize)
            {
                m_HeaderHandle = headerHandle;
                m_DataHandle   = dataHandle;
                m_DataSize     = dataSize;
            }

            public void Dispose()
            {
                m_HeaderHandle.Free();
                m_DataHandle.Free();
            }

            public GCHandle HeaderHandle
            {
                get{ return m_HeaderHandle; }
            }

            public WAVEHDR Header
            {
                get{ return (WAVEHDR)m_HeaderHandle.Target; }
            }

            public GCHandle DataHandle
            {
                get{ return m_DataHandle; }
            }

            public byte[] Data
            {
                get{ return (byte[])m_DataHandle.Target; }
            }

            public int DataSize
            {
                get{ return m_DataSize; }
            }
        }

        private WavInDevice      m_pInDevice     = null;
        private int              m_SamplesPerSec = 8000;
        private int              m_BitsPerSample = 8;
        private int              m_Channels      = 1;
        private int              m_BufferSize    = 400;
        private IntPtr           m_pWavDevHandle = IntPtr.Zero;
        private int              m_BlockSize     = 0;
        private List<BufferItem> m_pBuffers      = null;
        private waveInProc       m_pWaveInProc   = null;
        private bool             m_IsRecording   = false;
        private bool             m_IsDisposed    = false;

        public WaveIn(WavInDevice device,int samplesPerSec,int bitsPerSample,int channels,int bufferSize)
        {
            if(device == null){
                throw new ArgumentNullException("device");
            }
            if(samplesPerSec < 8000){
                throw new ArgumentException("Argument 'samplesPerSec' value must be >= 8000.");
            }
            if(bitsPerSample < 8){
                throw new ArgumentException("Argument 'bitsPerSample' value must be >= 8.");
            }
            if(channels < 1){
                throw new ArgumentException("Argument 'channels' value must be >= 1.");
            }

            m_pInDevice     = device;
            m_SamplesPerSec = samplesPerSec;
            m_BitsPerSample = bitsPerSample;
            m_Channels      = channels;
            m_BufferSize    = bufferSize;
            m_BlockSize     = m_Channels * (m_BitsPerSample / 8);
            m_pBuffers      = new List<BufferItem>();

            // Try to open wav device.            
            WAVEFORMATEX format = new WAVEFORMATEX();
            format.wFormatTag      = 0x0001;
            format.nChannels       = (ushort)m_Channels;
            format.nSamplesPerSec  = (uint)samplesPerSec;                        
            format.nAvgBytesPerSec = (uint)(m_SamplesPerSec * m_Channels * (m_BitsPerSample / 8));
            format.nBlockAlign     = (ushort)m_BlockSize;
            format.wBitsPerSample  = (ushort)m_BitsPerSample;
            format.cbSize          = 0; 
            // We must delegate reference, otherwise GC will collect it.
            m_pWaveInProc = new waveInProc(this.OnWaveInProc);
            int result = WavMethods.waveInOpen(out m_pWavDevHandle,m_pInDevice.Index,format,m_pWaveInProc,0,WavConstants.CALLBACK_FUNCTION);

            if (result != (int)MMSYSERR.NOERROR)
            {
                SteamEmulator.Write("AudioManager", $"Failed to open wav device, error: {(MMSYSERR)result}");
            }

            EnsureBuffers();
        }
        
        ~WaveIn()
        {
            Dispose();
        }

        public void Dispose()
        {
            if(m_IsDisposed){
                return;
            }
            m_IsDisposed = true;

            // Release events.
            this.BufferFull = null;

            try{
                // If recording, we need to reset wav device first.
                WavMethods.waveInReset(m_pWavDevHandle);
                
                // If there are unprepared wav headers, we need to unprepare these.
                foreach(BufferItem item in m_pBuffers){
                    WavMethods.waveInUnprepareHeader(m_pWavDevHandle,item.HeaderHandle.AddrOfPinnedObject(),Marshal.SizeOf(item.Header));
                    item.Dispose();
                }
                
                // Close input device.
                WavMethods.waveInClose(m_pWavDevHandle);

                m_pInDevice     = null;
                m_pWavDevHandle = IntPtr.Zero;
            }
            catch{                
            }
        }

        public void Start()
        {
            if(m_IsRecording){
                return;
            }
            m_IsRecording = true;

            int result = WavMethods.waveInStart(m_pWavDevHandle);
            if(result != (int)MMSYSERR.NOERROR){
                SteamEmulator.Write("AudioManager", $"Failed to start wav device, error: {(MMSYSERR)result}");
            }
        }

        public void Stop()
        {
            if(!m_IsRecording){
                return;
            }
            m_IsRecording = false;
            
            int result = WavMethods.waveInStop(m_pWavDevHandle);
            if(result != (int)MMSYSERR.NOERROR){
                SteamEmulator.Write("AudioManager", $"Failed to stop wav device, error: {(MMSYSERR)result}");
            }
        }

        private void OnWaveInProc(IntPtr hdrvr,int uMsg,int dwUser,int dwParam1,int dwParam2)
        {   
            // NOTE: MSDN warns, we may not call any wav related methods here.

            try{
                if(uMsg == WavConstants.MM_WIM_DATA){ 
                    ThreadPool.QueueUserWorkItem(new WaitCallback(this.ProcessFirstBuffer));
                }
            }
            catch{
            }
        }

        private void ProcessFirstBuffer(object state)
        {
            try{            
                lock(m_pBuffers){
                    BufferItem item = m_pBuffers[0];

                    // Raise BufferFull event.
                    OnBufferFull(item.Data);

                    // Clean up.
                    WavMethods.waveInUnprepareHeader(m_pWavDevHandle,item.HeaderHandle.AddrOfPinnedObject(),Marshal.SizeOf(item.Header));                    
                    m_pBuffers.Remove(item);
                    item.Dispose();
                }

                EnsureBuffers();
            }
            catch{
            }
        }

        private void EnsureBuffers()
        {
            // We keep 3 x buffer.
            lock(m_pBuffers){
                while(m_pBuffers.Count < 3){
                    byte[]   data       = new byte[m_BufferSize];
                    GCHandle dataHandle = GCHandle.Alloc(data,GCHandleType.Pinned);

                    WAVEHDR wavHeader = new WAVEHDR();
                    wavHeader.lpData          = dataHandle.AddrOfPinnedObject();
                    wavHeader.dwBufferLength  = (uint)data.Length;
                    wavHeader.dwBytesRecorded = 0;
                    wavHeader.dwUser          = IntPtr.Zero;
                    wavHeader.dwFlags         = 0;
                    wavHeader.dwLoops         = 0;
                    wavHeader.lpNext          = IntPtr.Zero;
                    wavHeader.reserved        = 0;
                    GCHandle headerHandle = GCHandle.Alloc(wavHeader,GCHandleType.Pinned);
                    int result = 0;        
                    result = WavMethods.waveInPrepareHeader(m_pWavDevHandle,headerHandle.AddrOfPinnedObject(),Marshal.SizeOf(wavHeader));
                    if(result == (int)MMSYSERR.NOERROR){
                        m_pBuffers.Add(new BufferItem(ref headerHandle,ref dataHandle,m_BufferSize));

                        result = WavMethods.waveInAddBuffer(m_pWavDevHandle,headerHandle.AddrOfPinnedObject(),Marshal.SizeOf(wavHeader));
                        if(result != (int)MMSYSERR.NOERROR){
                            SteamEmulator.Write("AudioManager", $"Error adding wave in buffer, error: {(MMSYSERR)result}");
                        }
                    }
                }
            }
        }

        public static WavInDevice[] Devices
        {
            get{
                List<WavInDevice> retVal = new List<WavInDevice>();
                // Get all available output devices and their info.                
                int devicesCount = WavMethods.waveInGetNumDevs();
                for(int i=0;i<devicesCount;i++){
                    WAVEOUTCAPS pwoc = new WAVEOUTCAPS();
                    if(WavMethods.waveInGetDevCaps((uint)i,ref pwoc,Marshal.SizeOf(pwoc)) == (int)MMSYSERR.NOERROR){
                        retVal.Add(new WavInDevice(i,pwoc.szPname,pwoc.wChannels));
                    }
                }

                return retVal.ToArray();
            }
        }


        public bool IsDisposed
        {
            get{ return m_IsDisposed; }
        }

        public WavInDevice InputDevice
        {
            get{
                if(m_IsDisposed){
                    throw new ObjectDisposedException("WavRecorder");
                }

                return m_pInDevice; 
            }
        }

        public int SamplesPerSec
        {
            get{                 
                if(m_IsDisposed){
                    throw new ObjectDisposedException("WavRecorder");
                }

                return m_SamplesPerSec; 
            }
        }

        public int BitsPerSample
        {
            get{ 
                if(m_IsDisposed){
                    throw new ObjectDisposedException("WavRecorder");
                }
                
                return m_BitsPerSample; 
            }
        }

        public int Channels
        {
            get{ 
                if(m_IsDisposed){
                    throw new ObjectDisposedException("WavRecorder");
                }
                
                return m_Channels; 
            }
        }

        public int BufferSize
        {
            get{ 
                if(m_IsDisposed){
                    throw new ObjectDisposedException("WavRecorder");
                }
                
                return m_BufferSize; 
            }
        }

        public int BlockSize
        {
            get{ 
                if(m_IsDisposed){
                    throw new ObjectDisposedException("WavRecorder");
                }

                return m_BlockSize; 
            }
        }

        public event BufferFullHandler BufferFull = null;

        private void OnBufferFull(byte[] buffer)
        {
            if(this.BufferFull != null){
                this.BufferFull(buffer);
            }
        }
    }
}
