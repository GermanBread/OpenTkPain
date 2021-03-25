// OpenTK
using OpenTK.Mathematics;

using nb.Game.GameObject.Components;

namespace nb.Game.GameObject
{
    public class Rectangle : BaseObject
    {
        public Rectangle(string scene) : base(scene) {
            transform.vertices = new Vector2[] {
                new Vector2(-.5f, -.5f),
                new Vector2(-.5f,  .5f),
                new Vector2( .5f,  .5f),
                new Vector2( .5f, -.5f)
            };
            transform.indices = new uint[] {
                0, 1, 3,
                1, 2, 3
            };
        }
    }
}