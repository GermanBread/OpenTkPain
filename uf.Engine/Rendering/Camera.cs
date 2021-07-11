// System
using System;

// OpenTK
using OpenTK.Mathematics;

using uf.Utility.Globals;
using uf.GameObject.Components;

namespace uf.Rendering
{
    public static class Camera
    {
        public static Vector2 Position { get; set; } = Vector2.Zero;
        public static Vector2 Zoom { get; set; } = Vector2.One;
        public static Vector2i Resolution { get => EngineGlobals.Window.Size; set => EngineGlobals.Window.Size = value; }
        public static float Rotation { get; set; } = 0;
        /// <summary>
        /// Translates the mouse coordinate into world space. For your convenience of course.
        /// Be warned that this does not rotate your objects!
        /// </summary>
        /// <param name="ScreenCoordiante">Mouse position</param>
        /// <returns>A coordinate in world space</returns>
        public static Vector2 ScreenToWorldSpace(Vector2 ScreenCoordiante) {
            Matrix2.CreateRotation(Rotation, out var _rotAdjustment);
            Vector2 _aspectRatioAdjustement = new((float)Camera.Resolution.X / Camera.Resolution.Y, 1);
            return Vector2.Divide(ScreenCoordiante - Resolution / 2, Vector2.Divide(Resolution * Zoom, new Vector2(20, -20) * _aspectRatioAdjustement)) * _rotAdjustment;
        }
    }
}