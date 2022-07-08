using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SKYNET.Wave.Native
{
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
}
