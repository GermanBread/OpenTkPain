// OpenTK
using OpenTK.Mathematics;

using nb.Game.GameObject.Components;

namespace nb.Game.GameObject
{
    public class Triangle : BaseObject
    {
        public Triangle(string scene = null) : base(scene) {
            transform.Vertices = new Vector2[] {
                new Vector2(-1,-1),
                new Vector2( 1,-1),
                new Vector2( 0, 1)
            };
            transform.UV = new Vector2[] {
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