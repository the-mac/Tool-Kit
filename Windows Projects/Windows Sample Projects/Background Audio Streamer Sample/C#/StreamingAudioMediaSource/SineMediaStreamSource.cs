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
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace BackgroundMediaSource
{
    /// <summary>
    /// A Media Stream Source implemented to play WAVE files
    /// </summary>
    public class SineMediaStreamSource : MediaStreamSource
    {
        private MediaStreamDescription _audioDesc;
        private long _currentPosition;
        private long _startPosition;
        private long _currentTimeStamp;
        private double _frequency;
        private double _amplitude;
        private long _duration;
        private WAVEFORMATEX _waveFormat;
        private const double convertToSeconds = 10000000.0;

        private Dictionary<MediaSampleAttributeKeys, string> _emptySampleDict = new Dictionary<MediaSampleAttributeKeys, string>();

        public event EventHandler StreamComplete;

        public SineMediaStreamSource(double frequency, double amplitude, TimeSpan duration)
        {
            _frequency = frequency;
            _amplitude = amplitude;
            _duration = (long)(duration.TotalSeconds * convertToSeconds);
        }

        public SineMediaStreamSource(double amplitude, TimeSpan duration)
        {
            double twoRoot12 = Math.Pow(2, (1.0 / 12.0));
            Random noteChooser = new Random();
            int note = noteChooser.Next(13);
            double frequency = 440.0 * Math.Pow(twoRoot12, note);

            _frequency = frequency;
            _amplitude = amplitude;
            _duration = (long)(duration.TotalSeconds * convertToSeconds);
        }

        protected override void OpenMediaAsync()
        {
            // Create a parser
            Dictionary<MediaStreamAttributeKeys, string> streamAttributes = new Dictionary<MediaStreamAttributeKeys, string>();
            Dictionary<MediaSourceAttributesKeys, string> sourceAttributes = new Dictionary<MediaSourceAttributesKeys, string>();
            List<MediaStreamDescription> availableStreams = new List<MediaStreamDescription>();

            // Stream Description 
            _waveFormat = new WAVEFORMATEX();
            _waveFormat.BitsPerSample = 8;
            _waveFormat.Channels = 1;
            _waveFormat.FormatTag = 1; // Format.PCM
            _waveFormat.SamplesPerSec = 22050;
            _waveFormat.Size = 0;
            _waveFormat.BlockAlign = (short)(_waveFormat.Channels * _waveFormat.BitsPerSample / 8);
            _waveFormat.AvgBytesPerSec = _waveFormat.SamplesPerSec * _waveFormat.Channels * _waveFormat.BitsPerSample / 8;

            streamAttributes[MediaStreamAttributeKeys.CodecPrivateData] = _waveFormat.ToHexString(); // wfx
            MediaStreamDescription msd = new MediaStreamDescription(MediaStreamType.Audio, streamAttributes);

            _audioDesc = msd;
            availableStreams.Add(_audioDesc);

            sourceAttributes[MediaSourceAttributesKeys.Duration] = _duration.ToString();
            ReportOpenMediaCompleted(sourceAttributes, availableStreams);
        }

        protected void CallStreamComplete()
        {
            // This may throw a null reference exception - that indicates that the agent did not correctly
            // subscribe to StreamComplete so it could call NotifyComplete
            if (null != StreamComplete)
            {
                StreamComplete(this, new EventArgs());
            }
        }
        
        protected override void CloseMedia()
        {
            // Close the stream
            _startPosition = _currentPosition = 0;
            _audioDesc = null;
            CallStreamComplete();
        }

        protected override void GetDiagnosticAsync(MediaStreamSourceDiagnosticKind diagnosticKind)
        {
            throw new NotImplementedException();
        }

        private int AlignUp(int a, int b)
        {
            int tmp = a + b - 1;
            return tmp - (tmp % b);
        }

        private void FillBuffer(byte[] buffer, long startTime, long endTime)
        {
            double startSecond = startTime / convertToSeconds;
            long sampleLength = _waveFormat.AudioDurationFromBufferSize((uint)_waveFormat.BlockAlign);
            double sampleTime = sampleLength / convertToSeconds;
            double currentSecond = startSecond;
            double endSecond = endTime / convertToSeconds;
            for (int i = 0; i < buffer.Length; i += _waveFormat.BlockAlign)
            {
                double sample;
                double timeFromEnd = endSecond - currentSecond;
                if ((1.5 > timeFromEnd && timeFromEnd > 1.25) ||
                    (1.0 > timeFromEnd && timeFromEnd > 0.75) ||
                    (0.5 > timeFromEnd && timeFromEnd > 0.25))
                {
                    sample = 0;
                }
                else
                {
                    sample = (_amplitude * Math.Sin(2 * Math.PI * _frequency * currentSecond));
                }
                short sampleValue = (short)(sample * 128 + 128);
                buffer[i] = (byte)sampleValue;
                if (_waveFormat.Channels == 2)
                {
                    buffer[i + 1] = (byte)sampleValue;
                }

                currentSecond += sampleTime;
            }
        }

        protected override void GetSampleAsync(MediaStreamType mediaStreamType)
        {
            // Start with one second of data, rounded up to the nearest block.
            uint cbBuffer = (uint)AlignUp(
                _waveFormat.AvgBytesPerSec / 100,
                _waveFormat.BlockAlign);

            if (_currentTimeStamp < _duration)
            {
                byte[] buffer = new byte[cbBuffer];
                FillBuffer(buffer, _currentTimeStamp, _duration);
                // Send out the next sample
                using (var stream = new MemoryStream(buffer))
                {
                    MediaStreamSample msSamp = new MediaStreamSample(
                        _audioDesc,
                        stream,
                        0,
                        cbBuffer,
                        _currentTimeStamp,
                        _emptySampleDict);

                    // Move our timestamp and position forward
                    _currentTimeStamp += _waveFormat.AudioDurationFromBufferSize(cbBuffer);

                    ReportGetSampleCompleted(msSamp);
                }
            }
            else // Report EOS
            {
                ReportGetSampleCompleted(new MediaStreamSample(_audioDesc, null, 0, 0, 0, _emptySampleDict));
            }
        }

        protected override void SeekAsync(long seekToTime)
        {
            _currentPosition = _waveFormat.BufferSizeFromAudioDuration(seekToTime);
            _currentTimeStamp = seekToTime;
            ReportSeekCompleted(seekToTime);
        }

        protected override void SwitchMediaStreamAsync(MediaStreamDescription mediaStreamDescription)
        {
            throw new NotImplementedException();
        }
    }
}
