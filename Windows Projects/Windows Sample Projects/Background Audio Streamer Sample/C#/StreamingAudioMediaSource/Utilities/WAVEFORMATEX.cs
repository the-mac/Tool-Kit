/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;

namespace BackgroundMediaSource
{
    /// <summary>
    /// 
    /// </summary>
    public class WAVEFORMATEX
    {
        #region Data
        public short FormatTag { get; set; }
        public short Channels { get; set; }
        public int SamplesPerSec { get; set; }
        public int AvgBytesPerSec { get; set; }
        public short BlockAlign { get; set; }
        public short BitsPerSample { get; set; }
        public short Size { get; set; }
        public const uint SizeOf = 18;
        public byte[] ext { get; set; }
        #endregion Data

        /// <summary>
        /// Convert the data to a hex string
        /// </summary>
        /// <returns></returns>
        public string ToHexString()
        {
            string s = "";

            s += ToLittleEndianString(string.Format("{0:X4}", FormatTag));
            s += ToLittleEndianString(string.Format("{0:X4}", Channels));
            s += ToLittleEndianString(string.Format("{0:X8}", SamplesPerSec));
            s += ToLittleEndianString(string.Format("{0:X8}", AvgBytesPerSec));
            s += ToLittleEndianString(string.Format("{0:X4}", BlockAlign));
            s += ToLittleEndianString(string.Format("{0:X4}", BitsPerSample));
            s += ToLittleEndianString(string.Format("{0:X4}", Size));

            return s;
        }

        /// <summary>
        /// Set the data from a byte array (usually read from a file)
        /// </summary>
        /// <param name="byteArray"></param>
        public void SetFromByteArray(byte[] byteArray)
        {
            if ((byteArray.Length + 2) < SizeOf)
            {
                throw new ArgumentException("Byte array is too small");
            }

            FormatTag = BitConverter.ToInt16(byteArray, 0);
            Channels = BitConverter.ToInt16(byteArray, 2);
            SamplesPerSec = BitConverter.ToInt32(byteArray, 4);
            AvgBytesPerSec = BitConverter.ToInt32(byteArray, 8);
            BlockAlign = BitConverter.ToInt16(byteArray, 12);
            BitsPerSample = BitConverter.ToInt16(byteArray, 14);
            if (byteArray.Length >= SizeOf)
            {
                Size = BitConverter.ToInt16(byteArray, 16);
            }
            else
            {
                Size = 0;
            }

            if (byteArray.Length > WAVEFORMATEX.SizeOf)
            {
                ext = new byte[byteArray.Length - WAVEFORMATEX.SizeOf];
                Array.Copy(byteArray, (int)WAVEFORMATEX.SizeOf, ext, 0, ext.Length);
            }
            else
            {
                ext = null;
            }
        }

        /// <summary>
        /// Convert a BigEndian string to a LittleEndian string
        /// </summary>
        /// <param name="bigEndianString"></param>
        /// <returns></returns>
        public static string ToLittleEndianString(string bigEndianString)
        {
            if (bigEndianString == null) { return ""; }

            char[] bigEndianChars = bigEndianString.ToCharArray();

            // Guard
            if (bigEndianChars.Length % 2 != 0) { return ""; }

            int i, ai, bi, ci, di;
            char a, b, c, d;
            for (i = 0; i < bigEndianChars.Length / 2; i += 2)
            {
                // front byte
                ai = i;
                bi = i + 1;

                // back byte
                ci = bigEndianChars.Length - 2 - i;
                di = bigEndianChars.Length - 1 - i;

                a = bigEndianChars[ai];
                b = bigEndianChars[bi];
                c = bigEndianChars[ci];
                d = bigEndianChars[di];

                bigEndianChars[ci] = a;
                bigEndianChars[di] = b;
                bigEndianChars[ai] = c;
                bigEndianChars[bi] = d;
            }

            return new string(bigEndianChars);
        }

        /// <summary>
        /// Ouput the data into a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            char[] rawData = new char[18];
            BitConverter.GetBytes(FormatTag).CopyTo(rawData, 0);
            BitConverter.GetBytes(Channels).CopyTo(rawData, 2);
            BitConverter.GetBytes(SamplesPerSec).CopyTo(rawData, 4);
            BitConverter.GetBytes(AvgBytesPerSec).CopyTo(rawData, 8);
            BitConverter.GetBytes(BlockAlign).CopyTo(rawData, 12);
            BitConverter.GetBytes(BitsPerSample).CopyTo(rawData, 14);
            BitConverter.GetBytes(Size).CopyTo(rawData, 16);
            return new string(rawData);
        }

        /// <summary>
        /// Calculate the duration of audio based on the size of the peekAheadBuffer
        /// </summary>
        /// <param name="cbAudioDataSize"></param>
        /// <returns></returns>
        public Int64 AudioDurationFromBufferSize(UInt32 cbAudioDataSize)
        {
            if (AvgBytesPerSec == 0)
            {
                return 0;
            }

            return (Int64)cbAudioDataSize * 10000000 / AvgBytesPerSec;
        }

        /// <summary>
        /// Calculate the peekAheadBuffer size necessary for a duration of audio
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public Int64 BufferSizeFromAudioDuration(Int64 duration)
        {
            Int64 size = duration * AvgBytesPerSec / 10000000;
            UInt32 remainder = (UInt32)(size % BlockAlign);
            if (remainder != 0)
            {
                size += BlockAlign - remainder;
            }

            return size;
        }

        /// <summary>
        /// Validate that the Wave format is consistent.
        /// </summary>
        public void ValidateWaveFormat()
        {
            if (FormatTag != FormatPCM)
            {
                throw new ArgumentException("Only PCM format is supported");
            }

            if (Channels != 1 && Channels != 2)
            {
                throw new ArgumentException("Only 1 or 2 channels are supported");
            }

            if (BitsPerSample != 8 && BitsPerSample != 16)
            {
                throw new ArgumentException("Only 8 or 16 bit samples are supported");
            }

            if (Size != 0)
            {
                throw new ArgumentException("Size must be 0");
            }

            if (BlockAlign != Channels * (BitsPerSample / 8))
            {
                throw new ArgumentException("Block Alignment is incorrect");
            }

            if (SamplesPerSec > (UInt32.MaxValue / BlockAlign))
            {
                throw new ArgumentException("SamplesPerSec overflows");
            }

            if (AvgBytesPerSec != SamplesPerSec * BlockAlign)
            {
                throw new ArgumentException("AvgBytesPerSec is wrong");
            }
        }

        const Int16 FormatPCM = 1;
        const Int16 FormatIEEE = 3;
    }
}
