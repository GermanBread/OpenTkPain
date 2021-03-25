// OpenTK
using OpenTK.Mathematics;

using nb.Game.GameObject.Components;

namespace nb.Game.GameObject
{
    public class Triangle : BaseObject
    {
        public Triangle(string scene) : base(scene) {
            transform.Vertices = new Vector2[] {
                new Vector2(-1,-1),
                new Vector2( 1,-1),
                new Vector2( 0, 1)
            };
        }
    }
}