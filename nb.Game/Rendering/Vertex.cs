// System
using System.Runtime.InteropServices;

// OpenTK
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace nb.Game.Rendering
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex
    {
        public Vector2 Coordinate;
        public Vector2 UV;
        public Vector2 InnerPosition;
        public Color4 Color;
    }
}