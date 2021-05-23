// OpenTk
using OpenTK.Mathematics;

namespace uf.GameObject.Components
{
    // Data
    public partial struct Anchor
    {
        public float X;
        public float Y;
        public Vector2 Xy { get => new(X, Y); set {
            X = value.X;
            Y = value.Y;
        } }
        public Vector2 Yx { get => Xy.Yx; set {
            Y = value.X;
            X = value.Y;
        } }
        public Anchor(float X, float Y) {
            this.X = X;
            this.Y = Y;
        }
        public Anchor(Vector2 Position) {
            Position.Deconstruct(out X, out Y);
        }
    }
    // Preset values
    public partial struct Anchor {
        public static Anchor Center { get => new(0, 0); }
        
        public static Anchor Top { get => new(0, 1); }
        public static Anchor Bottom { get => new(0, -1); }
        public static Anchor Left { get => new(-1, 0); }
        public static Anchor Right { get => new(1, 0); }

        public static Anchor TopLeft { get => new(-1, 1); }
        public static Anchor TopRight { get => new(1, 1); }
        public static Anchor BottomLeft { get => new(-1, -1); }
        public static Anchor BottomRight { get => new(1, -1); }
    }
    // Arithmetic
    public partial struct Anchor {
        public static Anchor operator +(Anchor anchor1, Anchor anchor2)
         => new(anchor1.X + anchor2.X, anchor1.Y + anchor2.Y);
        public static Anchor operator -(Anchor anchor1, Anchor anchor2)
         => new(anchor1.X - anchor2.X, anchor1.Y - anchor2.Y);
        public static Anchor operator *(Anchor anchor1, Anchor anchor2)
         => new(anchor1.X * anchor2.X, anchor1.Y * anchor2.Y);
        public static Anchor operator /(Anchor anchor1, Anchor anchor2)
         => new(anchor1.X / anchor2.X, anchor1.Y / anchor2.Y);
        public static Anchor operator *(Anchor anchor1, int scalar)
         => new(anchor1.X * scalar, anchor1.Y * scalar);
        public static Anchor operator /(Anchor anchor1, int scalar)
         => new(anchor1.X / scalar, anchor1.Y / scalar);
    }
}