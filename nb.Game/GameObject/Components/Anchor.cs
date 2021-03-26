namespace nb.Game.GameObject.Components
{
    // Data
    public partial struct Anchor
    {
        public float X;
        public float Y;
        public Anchor(float X, float Y) {
            this.X = X;
            this.Y = Y;
        }
    }
    // Preset values
    public partial struct Anchor {
        public static Anchor Center { get => new Anchor(0, 0); }
        
        public static Anchor Top { get => new Anchor(0, 1); }
        public static Anchor Bottom { get => new Anchor(0, -1); }
        public static Anchor Left { get => new Anchor(-1, 0); }
        public static Anchor Right { get => new Anchor(1, 0); }

        public static Anchor TopLeft { get => new Anchor(-1, 1); }
        public static Anchor TopRight { get => new Anchor(1, 1); }
        public static Anchor BottomLeft { get => new Anchor(-1, -1); }
        public static Anchor BottomRight { get => new Anchor(1, -1); }
    }
    // Arithmetic
    public partial struct Anchor {
        public static Anchor operator +(Anchor anchor1,Anchor anchor2) {
            return new Anchor(anchor1.X + anchor2.X, anchor1.Y + anchor2.Y);
        }
        public static Anchor operator -(Anchor anchor1,Anchor anchor2) {
            return new Anchor(anchor1.X - anchor2.X, anchor1.Y - anchor2.Y);
        }
    }
}