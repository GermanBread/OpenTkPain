// Imagesharp
using SixLabors.ImageSharp;

// OpenTK
using OpenTK.Mathematics;

namespace uf.Utility.Conversion;

public static class SizeToVector2 {
    public static Vector2 ToVector2(this Size size) {
        size.Deconstruct(out var x, out var y);
        return new Vector2(x, y);
    }
}