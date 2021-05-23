// System
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

// BASS
using ManagedBass;

using uf.Utility.Logging;
using uf.Utility.Resources;

namespace uf.Utility.Audio
{
    public static class AudioManager
    {
        private static bool isInitialized;
        public static bool BASSReady { get => isInitialized; }
        public static List<AudioClip> AudioClips { get => clips; }
        private static  List<AudioClip> clips = new();
        public static float GlobalVolume { get => (float)Bass.GlobalStreamVolume / 10000f; set => Bass.GlobalStreamVolume = (int)Math.Round(value * 10000f); }
        public static void SetVolume(string Group, float Volume) {
            foreach (var clip in clips.Where(x => x.Group == Group)) {
                clip.Volume = Volume;
            }
        }
        public static AudioClip GetClip(string ClipName) {
            return clips.FirstOrDefault(x => x.Name == ClipName);
        }
        public static void DeleteClip(string ClipName) {
            // There should be an easier way to do this, right? Apparently not.
            clips.Remove(clips.FirstOrDefault(x => x.Name == ClipName));
        }
        public static void DeleteClip(AudioClip Clip) {
            clips.Remove(Clip);
        }
        /// <summary>
        /// Creates an audio clip and returns a reference
        /// </summary>
        /// <returns>Reference to the audio clip</returns>
        public static AudioClip CreateClip(string ClipName, string SoundFilePath) {
            if (!isInitialized)
                return default;
            var _clip = new AudioClip(ClipName);
            _clip.Open(SoundFilePath);
            clips.Add(_clip);
            return _clip;
        }
        public static AudioClip CreateClip(Resource Resource) {
            if (!isInitialized)
                return default;
            AudioClip _clip;
            if (Resource == null)
                return null;
            _clip = new AudioClip(Resource.Name);
            _clip.Open(Resource.Path);
            clips.Add(_clip);
            return _clip;
        }
        public static void Init() {
            try {
                Bass.Init();
                isInitialized = true;
            } catch (DllNotFoundException dex) {
                if (!File.Exists("Bass.dll"))
                    Logger.Log(new LogMessage(LogSeverity.Warning, "Bass.dll not found. It should be included but apparently isn't. You won't hear audio.\n\tYou can also just download the .dlls from here: (https://www.un4seen.com/) and install bass.dll in this folder", dex));
                else
                    Logger.Log(new LogMessage(LogSeverity.Warning, "Bass.dll exists but could not be loaded for whatever reason that might be. You won't hear audio.\n\tYou can try and fix this by copying Bass.dll into either C:\\Windows\\System32 or /lib respectively", dex));
            }
        }
        public static void Dispose() {
            if (!isInitialized)
                return;
            Bass.Stop();
            Bass.Free();
            isInitialized = false;
        }
    }
}