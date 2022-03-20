// System

using System;
using System.IO;
using ManagedBass;
using uf.Utility.Logging;
// BASS

namespace uf.Utility.Audio
{
    public sealed class AudioClip : IDisposable
    {
        private int handle;
        public readonly string Name;
        public readonly string Group;
        private string FilePath { get; set; }
        public string FileName => Path.GetFileName(FilePath);

        public double ClipPosition { get {
            var _bytePosition = Bass.ChannelGetPosition(handle);
            var _secondsPosition = Bass.ChannelBytes2Seconds(handle, _bytePosition);
            return _secondsPosition;
        } set {
            var _bytePosition = Bass.ChannelSeconds2Bytes(handle, value);
            if (!Bass.ChannelSetPosition(handle, _bytePosition))
                Logger.Log(new LogMessage(LogSeverity.Error, $"Failed to seek {new BassException().Message}"));
        } }
        public double ClipLength { get {
            var _bytePosition = Bass.ChannelGetLength(handle);
            var _secondsPosition = Bass.ChannelBytes2Seconds(handle, _bytePosition);
            return _secondsPosition;
        } }
        public bool Loop { get; set; }
        public bool IsPlaying { get; private set; }
        public PlaybackState ClipStatus => Bass.ChannelIsActive(handle);

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
        public AudioClip(string name, string group = "default") {
            Name = name;
            Group = group;
        }
        public void Open(string File) {
            handle = Bass.CreateStream(File);
            if (handle == 0)
                Logger.Log(new LogMessage(LogSeverity.Error, "Failed to load file", new BassException()));
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
            
            var _size = (int)Math.Pow(2, (int)Math.Log2(20000));
            var _buffer = new float[_size];
            
            // Note to self: Bass.ChannelGetData = single speaker
            var _read = Bass.ChannelGetData(handle, _buffer, _size);
            if (_read == -1)
                return (new float[_size], 0);

            _buffer = Array.ConvertAll(_buffer, elem => elem / (_info.Frequency * _info.Channels));
            _buffer = Array.ConvertAll(_buffer, Math.Abs);
            return (_buffer, _read);
        }
        
        /// <summary>
        /// Free unused resources
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private bool disposed;

        private void Dispose(bool disposing)
        {
            if (disposed) return;
            IsPlaying = false;
            Bass.ChannelStop(handle);
            Bass.StreamFree(handle);
            disposed = true;
        }
    }
}