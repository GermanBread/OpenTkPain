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
        public Anchor(float x, float y) {
            this.X = x;
            this.Y = y;
        }
        public Anchor(Vector2 position) {
            position.Deconstruct(out X, out Y);
        }
    }
    // Preset values
    public partial struct Anchor {
        public static Anchor Center => new(0, 0);

        public static Anchor Top => new(0, 1);
        public static Anchor Bottom => new(0, -1);
        public static Anchor Left => new(-1, 0);
        public static Anchor Right => new(1, 0);

        public static Anchor TopLeft => new(-1, 1);
        public static Anchor TopRight => new(1, 1);
        public static Anchor BottomLeft => new(-1, -1);
        public static Anchor BottomRight => new(1, -1);
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