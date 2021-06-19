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
        public static Vector2 Position = Vector2.Zero;
        public static Vector2 Zoom = Vector2.One;
        public static Vector2i Resolution { get => EngineGlobals.Window.Size; set => EngineGlobals.Window.Size = value; }
        public static float Rotation = 0;
        public static Vector2 GetNormalizationFactor() {
            var _res = Resolution * new Vector2(Resolution.X / Resolution.Y, 1);
            return Vector2.ComponentMax(Vector2.ComponentMax(new(_res.X - _res.Y), new(_res.Y - _res.X)), Vector2.ComponentMin(new(_res.X), new(_res.Y)));
        }
        /// <summary>
        /// Translates the mouse coordinate into world space. For your convenience of course.
        /// Be warned that this does not rotate your objects!
        /// </summary>
        /// <param name="ScreenCoordiante">Mouse position</param>
        /// <returns>A coordinate in world space</returns>
        public static Vector2 ScreenToWorldSpace(Vector2 ScreenCoordiante) {
            Matrix2.CreateRotation(Rotation, out Matrix2 _rotMatrix);
            Vector2 _translated = (ScreenCoordiante - Vector2.Divide(Resolution, 2)) * new Vector2(2, -2) * Vector2.Divide(GetNormalizationFactor(), new Vector2(Resolution.X));
            return Vector2.Divide(_translated * _rotMatrix + Position, Zoom);
        }
    }
}