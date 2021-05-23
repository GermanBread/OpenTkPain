// System
using System;
using System.IO;
using System.Linq;

// BASS
using ManagedBass;

using uf.Utility.Logging;

namespace uf.Utility.Audio
{
    public class AudioClip : IDisposable
    {
        private int handle;
        public string Name;
        public string Group;
        public string FilePath { get; private set; }
        public string FileName { get {
            return Path.GetFileName(FilePath);
        } }
        public double ClipPosition { get {
            long BytePosition = Bass.ChannelGetPosition(handle);
            double SecondsPosition = Bass.ChannelBytes2Seconds(handle, BytePosition);
            return SecondsPosition;
        } set {
            long BytePosition = Bass.ChannelSeconds2Bytes(handle, value);
            if (!Bass.ChannelSetPosition(handle, BytePosition))
                Logger.Log(new LogMessage(LogSeverity.Error, $"Failed to seek {new BassException().Message}"));
        } }
        public double ClipLength { get {
            long BytePosition = Bass.ChannelGetLength(handle);
            double SecondsPosition = Bass.ChannelBytes2Seconds(handle, BytePosition);
            return SecondsPosition;
        } }
        public bool Loop { get; set; }
        public bool IsPlaying { get; private set; }
        public PlaybackState ClipStatus { get {
            return Bass.ChannelIsActive(handle);
        } }
        /// <summary>
        /// Volume, ranges from 0 to 1
        /// </summary>
        public float Volume { get {
            SampleInfo _info = new();
            Bass.SampleGetInfo(handle, ref _info);
            return _info.Volume;
        } set {
            SampleInfo _info = new();
            Bass.SampleGetInfo(handle, ref _info);
            _info.Volume = Math.Clamp(value, 0, 1);
            Bass.SampleSetInfo(handle, _info);
        } }
        public AudioClip(string Name, string Group = "default") {
            this.Name = Name;
            this.Group = Group;
        }
        public void Open(string File) {
            handle = Bass.CreateStream(File);
            if (handle == 0)
                Logger.Log(new LogMessage(LogSeverity.Error, $"Failed to load file", new BassException()));
            FilePath = Path.GetFullPath(File);
        }
        public void Play() {
            if (Bass.ChannelPlay(handle))
                IsPlaying = true;
        }
        public void Pause() {
            if (Bass.ChannelPause(handle))
                IsPlaying = false;
        }
        public void Stop() {
            if (Bass.ChannelStop(handle))
                IsPlaying = false;
        }
        /// <summary>
        /// Gets the raw waveform within 0hz-20.000hz
        /// </summary>
        /// <returns>A tuple, value 1 is the data (semi-normalized) and value 2 is the actual size of the array (how much has been read)</returns>
        public (float[], int) GetWaveform() {
            SampleInfo _info = new();
            Bass.SampleGetInfo(handle, ref _info);
            
            int _size = (int)Math.Pow(2, (int)Math.Log2(20000));
            float[] _buffer = new float[_size];
            
            // Note to self: Bass.ChannelGetData = single speaker
            int _read = Bass.ChannelGetData(handle, _buffer, _size);
            if (_read == -1)
                return (new float[_size], 0);

            _buffer = Array.ConvertAll(_buffer, elem => elem / (_info.Frequency * _info.Channels));
            _buffer = Array.ConvertAll(_buffer, elem => Math.Abs(elem));
            return (_buffer, _read);
        }
        
        /// <summary>
        /// Free unused resources
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing) {
            if (!disposed) {
                IsPlaying = false;
                Bass.ChannelStop(handle);
                Bass.StreamFree(handle);
                disposed = true;
            }
        }
    }
}