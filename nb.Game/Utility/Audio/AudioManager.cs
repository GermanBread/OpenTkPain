// System
using System.Collections.Generic;

namespace nb.Game.Utility.Audio
{
    public static class AudioManager
    {
        // TODO: Implement, Initialize BASS on startup
        public static List<AudioClip> AudioClips { get => clips; }
        private static List<AudioClip> clips = new List<AudioClip>();
        public static void GetClip(string ClipName) {

        }
    }
}