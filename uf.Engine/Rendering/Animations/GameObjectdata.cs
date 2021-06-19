// System
using System;

// OpenTK
using OpenTK.Mathematics;

// Unsinged Framework
using uf.GameObject;
using uf.GameObject.Components;

namespace uf.Rendering.Animations
{
    public class GameObjectdata
    {
        public GameObjectdata(Keyframe Keyframe) {
            Rotation = Keyframe.Rotation;
            (SkewX, SkewY) = (Keyframe.Skew.X, Keyframe.Skew.Y);
            (SizeX, SizeY) = (Keyframe.Size.X, Keyframe.Size.Y);
            (AnchorX, AnchorY) = (Keyframe.Anchor.X, Keyframe.Anchor.Y);
            (PositionX, PositionY) = (Keyframe.Position.X, Keyframe.Position.Y);
            (ColorR, ColorG, ColorB, ColorA) = (Keyframe.Color.R, Keyframe.Color.G, Keyframe.Color.B, Keyframe.Color.A);
        }
        public void ApplyTo(BaseObject GameObject) {
            GameObject.Rotation = Rotation;
            GameObject.Skew = new Vector2(SkewX, SkewY);
            GameObject.Size = new Vector2(SizeX, SizeY);
            GameObject.Anchor = new Anchor(AnchorX, AnchorY);
            GameObject.Position = new Vector2(PositionX, PositionX);
            GameObject.Color = new Color4(ColorR, ColorG, ColorB, ColorA);
        }
        public float SkewX;
        public float SkewY;
        public float SizeX;
        public float SizeY;
        public float ColorR;
        public float ColorG;
        public float ColorB;
        public float ColorA;
        public float AnchorX;
        public float AnchorY;
        public float Rotation;
        public float PositionX;
        public float PositionY;
    }
}