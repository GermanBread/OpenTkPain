// OpenTK
using OpenTK.Mathematics;

using nb.Game.GameObject.Components;

namespace nb.Game.GameObject
{
    public class Triangle : BaseObject
    {
        public Triangle(string scene) : base(scene) {
            transform.vertices = new Vector2[] {
                new Vector2(-.5f,-.5f),
                new Vector2( .5f,-.5f),
                new Vector2( 0, .5f)
            };
        }
    }
}