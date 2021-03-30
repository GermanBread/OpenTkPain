// OpenTK
using OpenTK.Mathematics;

using nb.Game.GameObject.Components;

namespace nb.Game.GameObject
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
            transform.Indices = new uint[] {
                0, 1, 3,
                1, 2, 3
            };
        }
    }
}