// System
using System;

// OpenTK
using OpenTK.Mathematics;

using nb.Game.Utility.Globals;
using nb.Game.GameObject.Components;

namespace nb.Game.Rendering
{
    public static class Camera
    {
        public static Vector2 Position = new Vector2(0);
        public static Vector2 Zoom = Vector2.One;
        public static Vector2i Resolution { get => EngineGlobals.Window.Size; set => EngineGlobals.Window.Size = value; }
        public static float Rotation = 0;
        /// <summary>
        /// Translates the mouse coordinate into world space. For your convenience of course.
        /// Be warned that this does not rotate your objects!
        /// </summary>
        /// <param name="ScreenCoordiante">Mouse position</param>
        /// <returns>A coordinate in world space</returns>
        public static Vector2 ScreenToWorldSpace(Vector2 ScreenCoordiante) {
            Vector2 _normalisationHelper = new Vector2(MathF.Max(Resolution.X, Resolution.Y));
            Matrix2 _rotMatrix;
            Matrix2.CreateRotation(Rotation, out _rotMatrix);
            Vector2 _translated = (ScreenCoordiante - Vector2.Divide(Resolution, 2)) * new Vector2(2, -2) * Vector2.Divide(_normalisationHelper, new Vector2(Resolution.X));
            return Vector2.Divide(_translated * _rotMatrix + Position, Zoom);
        }
    }
}