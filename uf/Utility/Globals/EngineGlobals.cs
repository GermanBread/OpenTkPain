// System
using System.Collections.Generic;

// OpenTk
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

using uf.Utility.Scenes;

namespace uf.Utility.Globals
{
    public static class EngineGlobals
    {
        public static List<Scene> Scenes = new List<Scene>();
        // A reference to the active game window
        public static BaseGame Window;
        public static string[] CLArgs;
    }
}