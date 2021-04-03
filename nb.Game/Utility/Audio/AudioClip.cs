// System
using System;
using System.IO;

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
        /// <returns>A tuple, value 1 is the data (interpolated if the array is not filled completely) and value 2 is the actual size of the array (how much has been read)</returns>
        public (int[], int) GetWaveform(int Count) {
            // FIXME: I doubt that this works like I think it does
            int[] _buffer = new int[Count];
            int _read = Bass.ChannelGetData(handle, _buffer, Count * 4);
            if (_read == -1)
                return (new int[Count], 0);
            return (_buffer, _read);
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