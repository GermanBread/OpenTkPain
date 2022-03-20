// System
using System.Collections.Generic;

// OpenTk
using uf.Utility.Scenes;

namespace uf.Utility.Globals
{
    public static class EngineGlobals
    {
        public static readonly List<Scene> Scenes = new();
        // A reference to the active game window
        public static BaseGame Window;
        public static string[] ClArgs;
    }
}