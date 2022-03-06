// System
using System.Runtime.InteropServices;

// OpenTK
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace uf.Rendering
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex
    {
        public Vector2 Position;
        public Vector2 UV;
        public Vector2 InnerBounds;
        public Color4 Color;
    }
}