// System
using System.Runtime.InteropServices;

// OpenTK
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace uf.Rendering
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Tex
    {
        public Vector2 Coordinate;
        public Vector2 UV;
        public Vector2 Position;
        public Color4 Color;
        public Vector2 GlyphPosition;
    }
}