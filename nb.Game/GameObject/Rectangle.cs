// OpenTK
using OpenTK.Mathematics;

using nb.Game.GameObject.Components;

namespace nb.Game.GameObject
{
    public class Rectangle : BaseObject
    {
        public Rectangle(string scene) : base(scene) {
            Transform.Vertices = new Vector2[] {
                new Vector2(-1, -1),
                new Vector2(-1,  1),
                new Vector2( 1,  1),
                new Vector2( 1, -1)
            };
            Transform.Indices = new uint[] {
                0, 1, 3,
                1, 2, 3
            };
        }
    }
}