// System

// OpenTK
using OpenTK.Mathematics;

// Unsigned Framework
using uf.GameObject;
using uf.GameObject.Components;

namespace uf.Rendering.Animations
{
    public class GameObjectData
    {
        public GameObjectData(Keyframe keyframe) {
            Rotation = keyframe.Rotation;
            (SkewX, SkewY) = (keyframe.Skew.X, keyframe.Skew.Y);
            (SizeX, SizeY) = (keyframe.Size.X, keyframe.Size.Y);
            (AnchorX, AnchorY) = (keyframe.Anchor.X, keyframe.Anchor.Y);
            (PositionX, PositionY) = (keyframe.Position.X, keyframe.Position.Y);
            (ColorR, ColorG, ColorB, ColorA) = (keyframe.Color.R, keyframe.Color.G, keyframe.Color.B, keyframe.Color.A);
        }
        public void ApplyTo(BaseObject gameObject) {
            gameObject.Rotation = Rotation;
            gameObject.Skew = new Vector2(SkewX, SkewY);
            gameObject.Size = new Vector2(SizeX, SizeY);
            gameObject.Anchor = new Anchor(AnchorX, AnchorY);
            gameObject.Position = new Vector2(PositionX, PositionX);
            gameObject.Color = new Color4(ColorR, ColorG, ColorB, ColorA);
        }
        public readonly float SkewX;
        public readonly float SkewY;
        public readonly float SizeX;
        public readonly float SizeY;
        public readonly float ColorR;
        public readonly float ColorG;
        public readonly float ColorB;
        public readonly float ColorA;
        public readonly float AnchorX;
        public readonly float AnchorY;
        public readonly float Rotation;
        public readonly float PositionX;
        public readonly float PositionY;
    }
}