// OpenTK
using OpenTK.Mathematics;

using uf.GameObject.Components;

namespace uf.GameObject
{
    public class Rectangle : BaseObject
    {
        public Rectangle(string scene = null) : base(scene) {
            transform.Vertices = new Vector2[] {
                new Vector2(-1, -1),
                new Vector2(-1,  1),
                new Vector2( 1,  1),
                new Vector2( 1, -1)
            };
            transform.Uv = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0)
            };
            transform.Indices = new uint[] {
                0, 1, 2,
                0, 2, 3
            };
        }
    }
}