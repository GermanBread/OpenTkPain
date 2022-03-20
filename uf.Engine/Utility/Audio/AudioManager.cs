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
        public static bool Ready { get; private set; }
        public static IEnumerable<AudioClip> AudioClips => clips;
        private static readonly List<AudioClip> clips = new();
        public static float GlobalVolume { get => Bass.GlobalStreamVolume / 10000f; set => Bass.GlobalStreamVolume = (int)Math.Round(value * 10000f); }
        public static void SetVolume(string @group, float volume) {
            foreach (var clip in clips.Where(x => x.Group == @group)) {
                clip.Volume = volume;
            }
        }
        public static AudioClip GetClip(string clipName) {
            return clips.FirstOrDefault(x => x.Name == clipName);
        }
        public static void DeleteClip(string clipName) {
            // There should be an easier way to do this, right? Apparently not.
            clips.Remove(clips.FirstOrDefault(x => x.Name == clipName));
        }
        public static void DeleteClip(AudioClip clip) {
            clips.Remove(clip);
        }
        /// <summary>
        /// Creates an audio clip and returns a reference
        /// </summary>
        /// <returns>Reference to the audio clip</returns>
        public static AudioClip CreateClip(string clipName, string soundFilePath) {
            if (!Ready)
                return default;
            var _clip = new AudioClip(clipName);
            _clip.Open(soundFilePath);
            clips.Add(_clip);
            return _clip;
        }
        public static AudioClip CreateClip(Resource resource) {
            if (!Ready)
                return default;
            if (resource == null)
                return null;
            var _clip = new AudioClip(resource.Name);
            _clip.Open(resource.Path);
            clips.Add(_clip);
            return _clip;
        }
        public static void Init() {
            if (Ready)
                return;
            try {
                Bass.Init();
                Ready = true;
            } catch (DllNotFoundException dex)
            {
                Logger.Log(!File.Exists("Bass.dll")
                    ? new LogMessage(LogSeverity.Warning,
                        "Bass.dll not found. It should be included but apparently isn't. You won't hear audio.\n\tYou can also just download the .dlls from here: (https://www.un4seen.com/) and install bass.dll in this folder",
                        dex)
                    : new LogMessage(LogSeverity.Warning,
                        "Bass.dll exists but could not be loaded for whatever reason that might be. You won't hear audio.\n\tYou can try and fix this by copying Bass.dll into either C:\\Windows\\System32 or /lib respectively",
                        dex));
            }
        }
        public static void Dispose() {
            if (!Ready)
                return;
            Bass.Stop();
            Bass.Free();
            Ready = false;
        }
    }
}