// System
using System;
using System.IO;
using System.Linq;

// BASS
using ManagedBass;

namespace nb.Game.Utility.Audio
{
    public class AudioClip : IDisposable
    {
        private int handle;
        public string Name;
        public string FilePath { get; private set; }
        public string FileName { get {
            return Path.GetFileName(FilePath);
        } }
        public double ClipPosition { get {
            long BytePosition = Bass.ChannelGetPosition(handle);
            if (BytePosition == -1)
                throw new BassException();
            double SecondsPosition = Bass.ChannelBytes2Seconds(handle, BytePosition);
            if (SecondsPosition < 0)
                throw new BassException();
            return SecondsPosition;
        } set {
            long BytePosition = Bass.ChannelSeconds2Bytes(handle, value);
            if (BytePosition == -1)
                throw new BassException();
            if (!Bass.ChannelSetPosition(handle, BytePosition))
                throw new BassException();
        } }
        public double ClipLength { get {
            long BytePosition = Bass.ChannelGetLength(handle);
            if (BytePosition == -1)
                throw new BassException();
            double SecondsPosition = Bass.ChannelBytes2Seconds(handle, BytePosition);
            if (SecondsPosition < 0)
                throw new BassException();
            return SecondsPosition;
        } }
        public bool Loop { get; set; }
        public bool IsPlaying { get; private set; }
        public PlaybackState ClipStatus { get {
            return Bass.ChannelIsActive(handle);
        } }
        public AudioClip(string Name) {
            this.Name = Name;
        }
        public void Open(string File) {
            handle = Bass.CreateStream(File);
            if (handle == 0)
                throw new BassException();
            FilePath = Path.GetFullPath(File);
        }
        public void Play() {
            if (!Bass.ChannelPlay(handle))
                throw new BassException();
            IsPlaying = true;
        }
        public void Pause() {
            if (!Bass.ChannelPause(handle))
                throw new BassException();
            IsPlaying = false;
        }
        public void Stop() {
            if (!Bass.ChannelStop(handle))
                throw new BassException();
            IsPlaying = false;
        }
        /// <summary>
        /// Gets the raw waveform
        /// </summary>
        /// <param name="Count">The amount of data</param>
        /// <returns>A tuple, value 1 is the data (semi-normalized) and value 2 is the actual size of the array (how much has been read)</returns>
        public (float[], int) GetWaveform(int Count) {
            // FIXME: I doubt that this works like I think it does
            int[] _buffer = new int[Count];
            int _read = Bass.ChannelGetData(handle, _buffer, Count * 4);
            if (_read == -1)
                return (new float[Count], 0);
            float[] _output = new float[_buffer.Length];
            float _divisor = 1000000000f;
            _output = Array.ConvertAll<int, float>(_buffer, x => x / _divisor);
            _output = Array.ConvertAll(_output, x => x - _output.Min());
            return (_output, _read);
        }
        
        /// <summary>
        /// Free unused resources
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(true);
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