// System
using System;
using System.Linq;
using System.Collections.Generic;

// BASS
using ManagedBass;

using nb.Game.Utility.Resources;

namespace nb.Game.Utility.Audio
{
    public static class AudioManager
    {
        public static List<AudioClip> AudioClips { get => clips; }
        private static List<AudioClip> clips = new List<AudioClip>();
        public static AudioClip GetClip(string ClipName) {
            return clips.First(x => x.Name == ClipName);
        }
        public static void DeleteClip(string ClipName) {
            // There should be an easier way to do this, right? Apparently not.
            clips.Remove(clips.First(x => x.Name == ClipName));
        }
        public static void DeleteClip(AudioClip Clip) {
            clips.Remove(Clip);
        }
        public static float GlobalStreamVolume { get => Bass.GlobalStreamVolume / 10000f; set => Bass.GlobalStreamVolume = (int)(value * 10000); }
        /// <summary>
        /// Creates an audio clip and returns a reference
        /// </summary>
        /// <returns>Reference to the audio clip</returns>
        public static AudioClip CreateClip(string ClipName, string SoundFilePath) {
            var _clip = new AudioClip(ClipName);
            _clip.Open(SoundFilePath);
            clips.Add(_clip);
            return _clip;
        }
        /// </inheritdoc>
        public static AudioClip CreateClip(Resource Resource) {
            var _clip = new AudioClip(Resource.Name);
            _clip.Open(Resource.Path);
            clips.Add(_clip);
            return _clip;
        }
        public static void Init() {
            Bass.Init();
        }
        public static void Dispose() {
            Bass.Stop();
            Bass.Free();
        }
    }
}