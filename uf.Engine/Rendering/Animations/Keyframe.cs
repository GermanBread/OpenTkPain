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
        public readonly float Rotation;
        public TimeSpan Timing;
        public Vector2 Position;

        public Keyframe(Vector2 skew, Color4 color, Anchor anchor, float rotation, TimeSpan timing, Vector2 position)
        {
            Skew = skew;
            Color = color;
            Anchor = anchor;
            Rotation = rotation;
            Timing = timing;
            Position = position;
            Size = default;
        }
    }
}