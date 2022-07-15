// System
using System.Collections.Generic;
using System.IO;
using System;

using uf.Utility.Scenes;

namespace uf.Utility.Globals
{
    public static class EngineGlobals
    {
        public static string EngineResourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EngineResources");
        public static List<Scene> Scenes = new();
        // A reference to the active game window
        public static BaseGame Window;
        public static string[] Args;
    }
}