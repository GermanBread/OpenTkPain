// OpenTK
using OpenTK.Mathematics;

using nb.Game.Utility.Globals;

namespace nb.Game.Rendering
{
    public static class Camera
    {
        public static Vector2 Position = new Vector2(0);
        public static float Zoom = 1;
        public static Vector2i Resolution { get => EngineGlobals.Window.Size; set => EngineGlobals.Window.Size = value; }
        public static float Rotation = 0;
    }
}