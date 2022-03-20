// OpenTK
using OpenTK.Mathematics;

using uf.GameObject.Components;

namespace uf.GameObject
{
    public class Triangle : BaseObject
    {
        public Triangle(string scene = null) : base(scene) {
            transform.Vertices = new Vector2[] {
                new Vector2(-1,-1),
                new Vector2( 1,-1),
                new Vector2( 0, 1)
            };
            transform.Uv = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0.5f, 1),
            };
            transform.Indices = new uint[] {
                0, 1, 2,
            };
        }
    }
}