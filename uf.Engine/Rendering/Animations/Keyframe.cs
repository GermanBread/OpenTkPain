// System
using System;

// OpenTK
using OpenTK.Mathematics;

// Unsinged Framework
using uf.GameObject.Components;

namespace uf.Rendering.Animations
{
    public struct Keyframe
    {
        public Vector2 Skew;
        public Vector2 Size;
        public Color4 Color;
        public Anchor Anchor;
        public float Rotation;
        public TimeSpan Timing;
        public Vector2 Position;
    }
}