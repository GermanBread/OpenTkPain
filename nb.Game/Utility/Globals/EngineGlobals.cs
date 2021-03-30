// System
using System.Collections.Generic;

// OpenTk
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

using nb.Game.GameObject;
using nb.Game.Utility.Scenes;

namespace nb.Game.Utility.Globals
{
    public static class EngineGlobals
    {
        public static List<Scene> Scenes = new List<Scene>();
        public static Vector2i CurrentResolution;
        public static GameWindow Window;
    }
}